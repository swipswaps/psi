﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

namespace Microsoft.Psi.Data
{
    using System;
    using System.Threading;
    using Microsoft.Psi;
    using Microsoft.Psi.Common;
    using Microsoft.Psi.Components;
    using Microsoft.Psi.Persistence;
    using Microsoft.Psi.Serialization;

    /// <summary>
    /// Component that writes messages to a multi-stream store.
    /// </summary>
    /// <remarks>
    /// The store can be backed by a file on disk, can be ephemeral (in-memory) for inter-process communication
    /// or can be a network protocol for cross-machine communication.
    /// </remarks>
    public abstract class Exporter : Subpipeline, IDisposable
    {
        internal static readonly string StreamMetadataNamespace = "store\\metadata";

        private readonly PsiStoreWriter writer;
        private readonly Merger<Message<BufferReader>, string> merger;
        private readonly Pipeline pipeline;
        private readonly ManualResetEvent throttle = new ManualResetEvent(true);
        private KnownSerializers serializers;

        /// <summary>
        /// Initializes a new instance of the <see cref="Exporter"/> class.
        /// </summary>
        /// <param name="pipeline">The pipeline to add the component to.</param>
        /// <param name="name">The name of the application that generated the persisted files, or the root name of the files.</param>
        /// <param name="path">The directory in which the main persisted file resides or will reside, or null to create a volatile data store.</param>
        /// <param name="createSubdirectory">If true, a numbered sub-directory is created for this store.</param>
        /// <param name="serializers">
        /// A collection of known serializers, or null to infer it from the data being written to the store.
        /// The known serializer set can be accessed and modified afterwards via the <see cref="Serializers"/> property.
        /// </param>
        protected internal Exporter(Pipeline pipeline, string name, string path, bool createSubdirectory = true, KnownSerializers serializers = null)
            : base(pipeline, $"{nameof(Exporter)}[{name}]")
        {
            this.pipeline = pipeline;
            this.serializers = serializers ?? new KnownSerializers();
            this.writer = new PsiStoreWriter(name, path, createSubdirectory);

            // write the version info
            this.writer.WriteToCatalog(this.serializers.RuntimeVersion);

            // copy the schemas present so far and also make sure the catalog captures schemas added in the future
            this.serializers.SchemaAdded += (o, e) => this.writer.WriteToCatalog(e);
            foreach (var schema in this.serializers.Schemas.Values)
            {
                this.writer.WriteToCatalog(schema);
            }

            this.merger = new Merger<Message<BufferReader>, string>(this, (_, m) =>
            {
                this.Throttle.WaitOne();
                this.writer.Write(m.Data.Data, m.Data.Envelope);
            });
        }

        /// <summary>
        /// Gets the name of the store being written to.
        /// </summary>
        public new string Name => this.writer.Name;

        /// <summary>
        /// Gets the path to the store being written to if the store is persisted to disk, or null if the store is volatile.
        /// </summary>
        public string Path => this.writer.Path;

        /// <summary>
        /// Gets the set of types that this Importer can deserialize.
        /// Types can be added or re-mapped using the <see cref="KnownSerializers.Register{T}(string, CloningFlags)"/> method.
        /// </summary>
        public KnownSerializers Serializers => this.serializers;

        /// <summary>
        /// Gets the event that allows remoting to throttle data reading to match a specified network bandwidth.
        /// </summary>
        internal ManualResetEvent Throttle => this.throttle;

        /// <summary>
        /// Closes the store.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            if (this.writer != null)
            {
                this.writer.Dispose();
            }

            this.throttle.Dispose();
        }

        /// <summary>
        /// Writes the messages from the specified stream to the matching stream in this store.
        /// </summary>
        /// <typeparam name="TMessage">The type of messages in the stream.</typeparam>
        /// <param name="source">The source stream to write.</param>
        /// <param name="name">The name of the stream.</param>
        /// <param name="largeMessages">Indicates whether the stream contains large messages (typically >4k). If true, the messages will be written to the large message file.</param>
        /// <param name="deliveryPolicy">An optional delivery policy.</param>
        public void Write<TMessage>(Emitter<TMessage> source, string name, bool largeMessages = false, DeliveryPolicy<TMessage> deliveryPolicy = null)
        {
            this.WriteToStorage(source, name, largeMessages, deliveryPolicy);
        }

        /// <summary>
        /// Writes the messages from the specified stream to the matching stream in this store.
        /// Additionally stores supplemental metadata value.
        /// </summary>
        /// <typeparam name="TMessage">The type of messages in the stream.</typeparam>
        /// <typeparam name="TSupplementalMetadata">The type of supplemental stream metadata.</typeparam>
        /// <param name="source">The source stream to write.</param>
        /// <param name="supplementalMetadataValue">Supplemental metadata value.</param>
        /// <param name="name">The name of the stream.</param>
        /// <param name="largeMessages">Indicates whether the stream contains large messages (typically >4k). If true, the messages will be written to the large message file.</param>
        /// <param name="deliveryPolicy">An optional delivery policy.</param>
        public void Write<TMessage, TSupplementalMetadata>(Emitter<TMessage> source, TSupplementalMetadata supplementalMetadataValue, string name, bool largeMessages = false, DeliveryPolicy<TMessage> deliveryPolicy = null)
        {
            var meta = this.WriteToStorage(source, name, largeMessages, deliveryPolicy);
            meta.SetSupplementalMetadata(supplementalMetadataValue, this.serializers);
        }

        /// <summary>
        /// Writes the envelopes of messages from the specified stream to the store.
        /// </summary>
        /// <typeparam name="T">The type of messages in the stream.</typeparam>
        /// <param name="source">The source stream to write.</param>
        /// <param name="name">The name of the stream.</param>
        /// <param name="deliveryPolicy">An optional delivery policy.</param>
        public void WriteEnvelopes<T>(Emitter<T> source, string name, DeliveryPolicy<T> deliveryPolicy = null)
        {
            // make sure we can serialize this type
            var handler = this.serializers.GetHandler<int>();

            // add another input to the merger to hook up the serializer to
            // and check for duplicate names in the process
            var mergeInput = this.merger.Add(name);

            // name the stream if it's not already named
            var connector = new MessageEnvelopeConnector<T>(source.Pipeline, this, null);

            // defaults to lossless delivery policy unless otherwise specified
            source.PipeTo(connector, deliveryPolicy ?? DeliveryPolicy.Unlimited);
            source.Name = connector.Out.Name = name;
            source.Closed += closeTime => this.writer.CloseStream(source.Id, closeTime);

            // tell the writer to write the serialized stream
            var meta = this.writer.OpenStream(source.Id, name, false, handler.Name);

            // register this stream with the store catalog
            this.pipeline.ConfigurationStore.Set(Exporter.StreamMetadataNamespace, name, meta);

            // hook up the serializer
            var serializer = new SerializerComponent<int>(this, this.serializers);

            // The serializer and merger will act synchronously and throttle the connector for as long as
            // the merger is busy writing data. This will cause messages to be queued or dropped at the input
            // to the connector (per the user-supplied deliveryPolicy) until the merger is able to accept
            // the next serialized data message.
            serializer.PipeTo(mergeInput, allowWhileRunning: true, DeliveryPolicy.SynchronousOrThrottle);
            connector.PipeTo(serializer, allowWhileRunning: true, DeliveryPolicy.SynchronousOrThrottle);
        }

        /// <summary>
        /// Writes the messages from the specified stream to the matching stream in this store.
        /// </summary>
        /// <typeparam name="TMessage">The type of messages in the stream.</typeparam>
        /// <param name="source">The source stream to write.</param>
        /// <param name="name">The name of the stream.</param>
        /// <param name="metadata">Source stream metadata.</param>
        /// <param name="deliveryPolicy">An optional delivery policy.</param>
        internal void Write<TMessage>(Emitter<TMessage> source, string name, PsiStreamMetadata metadata, DeliveryPolicy<TMessage> deliveryPolicy = null)
        {
            this.WriteToStorage(source, name, metadata.IsIndexed, deliveryPolicy).UpdateSupplementalMetadataFrom(metadata);
        }

        internal void Write(Emitter<Message<BufferReader>> source, PsiStreamMetadata meta, DeliveryPolicy<Message<BufferReader>> deliveryPolicy = null)
        {
            var mergeInput = this.merger.Add(meta.Name); // this checks for duplicates

            var connector = this.CreateInputConnectorFrom<Message<BufferReader>>(source.Pipeline, null);
            source.PipeTo(connector);
            source.Name = source.Name ?? meta.Name;
            connector.Out.Name = meta.Name;

            this.writer.OpenStream(meta);

            // defaults to lossless delivery policy unless otherwise specified
            connector.PipeTo(mergeInput, true, deliveryPolicy ?? DeliveryPolicy.Unlimited);
        }

        /// <summary>
        /// Writes the messages from the specified stream to the matching stream in this store.
        /// </summary>
        /// <typeparam name="TMessage">The type of messages in the stream.</typeparam>
        /// <param name="source">The source stream to write.</param>
        /// <param name="name">The name of the stream.</param>
        /// <param name="largeMessages">Indicates whether the stream contains large messages (typically >4k). If true, the messages will be written to the large message file.</param>
        /// <param name="deliveryPolicy">An optional delivery policy.</param>
        /// <returns>Stream metadata.</returns>
        private PsiStreamMetadata WriteToStorage<TMessage>(Emitter<TMessage> source, string name, bool largeMessages = false, DeliveryPolicy<TMessage> deliveryPolicy = null)
        {
            // make sure we can serialize this type
            var handler = this.serializers.GetHandler<TMessage>();

            // add another input to the merger to hook up the serializer to
            // and check for duplicate names in the process
            var mergeInput = this.merger.Add(name);

            // name the stream if it's not already named
            var connector = new MessageConnector<TMessage>(source.Pipeline, this, null);

            // defaults to lossless delivery policy unless otherwise specified
            source.PipeTo(connector, deliveryPolicy ?? DeliveryPolicy.Unlimited);
            source.Name = connector.Out.Name = name;
            source.Closed += closeTime => this.writer.CloseStream(source.Id, closeTime);

            // tell the writer to write the serialized stream
            var meta = this.writer.OpenStream(source.Id, name, largeMessages, handler.Name);

            // register this stream with the store catalog
            this.pipeline.ConfigurationStore.Set(Exporter.StreamMetadataNamespace, name, meta);

            // hook up the serializer
            var serializer = new SerializerComponent<TMessage>(this, this.serializers);

            // The serializer and merger will act synchronously and throttle the connector for as long as
            // the merger is busy writing data. This will cause messages to be queued or dropped at the input
            // to the connector (per the user-supplied deliveryPolicy) until the merger is able to accept
            // the next serialized data message.
            serializer.PipeTo(mergeInput, allowWhileRunning: true, DeliveryPolicy.SynchronousOrThrottle);
            connector.PipeTo(serializer, allowWhileRunning: true, DeliveryPolicy.SynchronousOrThrottle);

            return meta;
        }
    }
}
