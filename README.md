# Platform for Situated Intelligence

![Build status](https://dev.azure.com/msresearch/psi/_apis/build/status/psi-github-ci?branchName=master)
[![Join the chat at https://gitter.im/Microsoft/psi](https://badges.gitter.im/Microsoft/psi.svg)](https://gitter.im/Microsoft/psi?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

**Platform for Situated Intelligence** (or in short, \\psi, pronounced like the greek letter) is an open, extensible framework for development and research of multimodal, integrative-AI systems. Examples include multimodal interactive systems such as social robots and embodied conversational agents, systems for ambient intelligence and smart spaces, applications based on small devices that work with streaming sensor data, etc. In essence, any application that processes streaming, sensor data (such as audio, video, depth, etc.), combines multiple (AI) technologies, and operates under latency constraints can benefit from the affordances the provided by the framework.

The framework accelerates the development of these applications by providing: 
-	a modern, performant **infrastructure** for working with multimodal, temporally streaming data
-	a set of **tools** for multimodal data visualization, annotation, and processing
-	an ecosystem of **components** for various sensors, processing technologies, and effectors

<br>

![Psi Overview](https://www.microsoft.com/en-us/research/uploads/prod/2018/01/Psi-Gif2-960-Corrected.gif)

A high-level overview of the framework is available in [this blog post](https://www.microsoft.com/en-us/research/blog/platform-for-situated-intelligence-an-open-source-framework-for-multimodal-integrative-ai/). An on-demand webinar is also available containing a brief introduction and tutorial on how to code with \psi – here is the [registration link](https://note.microsoft.com/MSR-Webinar-Platform-Situated-Intelligence-Registration-On-Demand.html).

# What’s New
__12/07/2020__: We have published a new beta release, version 0.14.35.3, which includes a new ONNX model runner for ImageNet models, new components for screen and window capture, updates to annotation editing in PsiStudio, as well as a number of bug fixes and updates -- see the [full release notes](https://github.com/microsoft/psi/wiki/Release-Notes) for more details.

__09/30/2020__: We have added three additional samples: a basic [HelloWorld]( https://github.com/Microsoft/psi-samples/tree/main/Samples/HelloWorld) sample illustrating the simplest starting point for a \psi application, a more complex one demonstrating how to do some basic audio capture and processing to construct [a simple voice activity detector]( https://github.com/Microsoft/psi-samples/tree/main/Samples/SimpleVoiceActivityDetector), and a third sample that combines information from Azure Kinect with Cognitive Services vision and speech to [detect objects that a person is pointing to](https://github.com/microsoft/psi-samples/tree/main/Samples/WhatIsThat).

__09/02/2020__: We published [a blog post](https://www.microsoft.com/en-us/research/blog/platform-for-situated-intelligence-an-open-source-framework-for-multimodal-integrative-ai/) with a high-level overview of the framework.

__08/31/2020__: We released version [0.13.38.2]( https://github.com/microsoft/psi/releases/tag/v0.13.38.2-beta), which brings important updates to Platform for Situated Intelligence Studio (including data annotation), updates to the runtime to support 3rd party data store sources, and components for running ONNX models. See the [release notes]( https://github.com/microsoft/psi/wiki/Release-Notes#20200831-beta-release-version-013382) for a more complete description of updates.

# Getting Started

The core \\psi infrastructure is built on .NET Standard and therefore runs both on Windows and Linux. Some [components](https://github.com/microsoft/psi/wiki/List-of-Components) and tools are more specific and are available only on one or the other operating system. You can build \\psi applications either by [leveraging \\psi NuGet packages](https://github.com/microsoft/psi/wiki/Using-via-NuGet-Packages), or by [cloning and building the source code](https://github.com/microsoft/psi/wiki/Building-the-Codebase). 

__A Quick Introduction.__ To learn more about \\psi and how to build applications with it, we recommend you start with the [Brief Introduction](https://github.com/microsoft/psi/wiki/Brief-Introduction) tutorial, which will walk you through for some of the main concepts. It shows how to create a simple program, describes the core concept of a stream, and explains how to transform, synchronize, visualize, persist and replay streams from disk.

__Samples.__ If you would like to directly start from sample code, a number of small [sample applications](https://github.com/microsoft/psi/wiki/Samples) are also available, and several of them have walkthroughs that explain how the sample was constructed and point to additional documentation. We recommend you start with the samples below, listed in increasing order of complexity:
| Name | Description | Cross-plat | Requirements |
| :----------- | :---------- | :--: | :--: |
| [HelloWorld](https://github.com/Microsoft/psi-samples/tree/main/Samples/HelloWorld) <br> ![HelloWorldPreview](https://github.com/microsoft/psi-samples/blob/main/Samples/HelloWorld/Images/HelloWorldPreview.png) | This sample provides the simplest starting point for creating a \\psi application: it illustrates how to create and run a simple \psi pipeline containing a single stream. | Yes | None |
| [SimpleVoiceActivityDetector](https://github.com/Microsoft/psi-samples/tree/main/Samples/SimpleVoiceActivityDetector) <br> ![SimpleVADPreview](https://github.com/microsoft/psi-samples/blob/main/Samples/SimpleVoiceActivityDetector/Images/SimpleVADPreview.png) | This sample captures audio from a microphone and performs voice activity detection, i.e., it computes a boolean signal indicating whether or not the audio contains voiced speech. | Yes | Microphone |
| [WebcamWithAudio for Windows](https://github.com/Microsoft/psi-samples/tree/main/Samples/WebcamWithAudioSample) or [Linux](https://github.com/Microsoft/psi-samples/tree/main/Samples/LinuxWebcamWithAudioSample) <br> ![WebcamPreview](https://github.com/microsoft/psi-samples/blob/main/Samples/WebcamWithAudioSample/Images/WebcamPreview.gif) | This sample shows how to display images from a camera and the audio energy level from a microphone and illustrates the basics of stream synchronization. | Yes | Webcam and Microphone | 
| [WhatIsThat](https://github.com/microsoft/psi-samples/tree/main/Samples/WhatIsThat) <br> ![WhatIsThatPreview](https://github.com/microsoft/psi-samples/blob/main/Samples/WhatIsThat/Images/WhatIsThatPreview.gif)| This sample implements a simple application that uses an Azure Kinect sensor to detect the objects a person is pointing to. | Windows-only | Azure Kinect + Cognitive Services |


__Documentation.__ The documentation for \\psi is available in the [github project wiki](https://github.com/microsoft/psi/wiki). It contains many additional resources, including [tutorials]( https://github.com/microsoft/psi/wiki/Tutorials), other [specialized topics]( https://github.com/microsoft/psi/wiki/Other-Topics), and a full [API reference](https://microsoft.github.io/psi/api/Microsoft.Psi.html) that can help you learn more about the framework.

# Getting Help
If you find a bug or if you would like to request a new feature or additional documentation, please file an [issue in github](https://github.com/microsoft/psi/issues). Use the [`bug`](https://github.com/microsoft/psi/labels/bug) label when filing issues that represent code defects, and provide enough information to reproduce the bug. Use the [`feature request`](https://github.com/microsoft/psi/labels/feature%20request) label to request new features, and use the [`documentation`](https://github.com/microsoft/psi/labels/documentation) label to request additional documentation. 

# Contributing

We are looking forward to engaging with the community to improve and evolve Platform for Situated Intelligence! We welcome contributions in many forms: from simply using it and filing issues and bugs, to writing and releasing your own new components, to creating pull requests for bug fixes or new features. The [Contributing Guidelines](https://github.com/microsoft/psi/wiki/Contributing) page in the wiki describes many ways in which you can get involved, and some useful things to know before contributing to the code base.

To find more information about our future plans, please see the [Roadmap](https://github.com/microsoft/psi/wiki/Roadmap) document.

# Who is Using

Platform for Situated Intelligence has been and is currently used in several industry and academic research labs, including (but not limited to):
* the [Situated Interaction](https://www.microsoft.com/en-us/research/project/situated-interaction/) project, as well as other research projects at Microsoft Research.
* the [MultiComp Lab](http://multicomp.cs.cmu.edu/) at Carnegie Mellon University.
* the [Speech Language and Interactive Machines](https://coen.boisestate.edu/slim/) research group at Boise State University.
* the [Qualitative Reasoning Group](http://www.qrg.northwestern.edu/), Northwestern University. 
* the [Intelligent Human Perception Lab](https://www.ihp-lab.org), at USC Institute for Creative Technologies.
* the [Teledia research group](https://www.cs.cmu.edu/~cprose), at Carnegie Mellon University.
* the [F&M Computational, Affective, Robotic, and Ethical Sciences (F&M CARES) lab](https://fandm-cares.github.io/), at Franklin and Marshall College.
* the [Transportation, Bots, & Disability Lab](https://tbd.ri.cmu.edu) at the Carnegie Mellon University.

If you would like to be added to this list, just file a [GitHub issue](https://github.com/Microsoft/psi/issues) and label it with the [`whoisusing`](https://github.com/Microsoft/psi/labels/whoisusing) label. Add a url for your research lab, website or project that you would like us to link to. 

# Disclaimer

The codebase is currently in beta and various aspects of the framework are under active development. There are probably still bugs in the code and we may make breaking API changes. 

# License

Platform for Situated Intelligence is available under an [MIT License](LICENSE.txt). See also [Third Party Notices](ThirdPartyNotices.txt).

# Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft trademarks or logos is subject to and must follow [Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general). Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship. Any use of third-party trademarks or logos are subject to those third-party's policies.

# Acknowledgments

We would like to thank our internal collaborators and external early adopters, including (but not limited to): [Daniel McDuff](http://alumni.media.mit.edu/~djmcduff/), [Kael Rowan](https://www.microsoft.com/en-us/research/people/kaelr/), [Lev Nachmanson](https://www.microsoft.com/en-us/research/people/levnach/) and [Mike Barnett](https://www.microsoft.com/en-us/research/people/mbarnett) at MSR, Chirag Raman and Louis-Phillipe Morency in the [MultiComp Lab](http://multicomp.cs.cmu.edu/) at CMU, as well as researchers in the [SLIM research group](https://coen.boisestate.edu/slim/) at Boise State and the [Qualitative Reasoning Group](http://www.qrg.northwestern.edu/) at Northwestern University.

