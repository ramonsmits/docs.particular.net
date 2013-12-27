---
title: Overview of the Particular Software Messaging Platform
summary: 'An introduction to the platform, its tools and products, including NServiceBus, ServiceMatrix, ServicePulse and ServiceInsight'
tags:
- Messaging Platform 
- Introduction
- Overview
- Service Oriented Architecture
- SOA
- Event-Driven Architecture
- EDA
---

[![NServicebus](/images/particular-software-logo.jpg "Particular Service Platform")](http://particular.net) 

# Overview of the Particular Service Platform


## Introduction

The Particular Service Platform is an integrated set of products and tools enabling you to develop, test, and run distributed systems.

OK, but what does this actually mean ?

Developing distributed applications and systems is hard. Really hard.
Just think of all you need to do:
* Envision and design the distributed system 
* Create a POC
* Demo it and communicate your intent to peers, managers and customers
* Receive feedback and iterate to make sure you're on the right track
* Add business logic and your system's unique "secret sauce" 
* Test and ensure it all fits together and works as planned 
* Deploy to production and continuously monitor 
* Track customer usage and handle any exceptions
* Improve, fix, upgrade and update while making sure it all works flawlessly

This is quite a challenge and, though the industry has been making  progress in methodology and tooling, it still requires a steep learning curve and significant effort to master distributed systems development. 

**Our goal is simple: To make designing, developing and running distributed systems fun (Udi:?), easy and fast.**

The Particular Service Platform, developed by the makers of NServiceBus, takes distributed development to the next level of ease and efficiency by:
* Supporting the entire lifecycle of distributed applications development with a comprehensive set of tools 
* Following established best practices and methodological guidelines
* Building it for (and with) NServiceBus, the most popular Service Bus for the .NET environment
* Offering end-to-end Support for a wide range of messaging transports including MSMQ, RabbitMQ, ActiveMQ, SQL Server, Windows Azure Queues, Windows Azure Service Bus, WebsphereMQ and more)          

## ![](/images/particular-software-platform.jpg "Particular Service Platform")





## [![NServicebus](/images/nservicebus-logo.jpg "NServiceBus")](http://particular.net/NServiceBus) 

### The most developer-friendly service bus for .NET

Enterprise-grade scalability and reliability for your workflows and integrations without any messy XML configuration - just pure-code bliss. Simple and easy publish/subscribe communication over any transport, on premise and in the cloud.

More than 50,000 developers worldwide from a [wide range of business domains and company sizes](http://particular.net/customers)rely on NServiceBus every day. It is backed by a rock-solid [distributed development methodology](http://particular.net/adsd), a [worldwide community of experts, consultants and contributors](http://particular.net/champions) and is supported by the [Particular Software team](http://particular.net/support). 


#### Product Highlights

* High performance and scalability
* Reliable integration with automatic retries
* Workflow and background task scheduling
* Centralized auditing of all message flows
* Publish/Subscribe support for reduced coupling
* Replication-free for cross-site communication
* Supports multiple transports, out-of-the-box
* Easy to extend and configure
* Runs on-premise, in the Cloud, or in a hybrid scenario
* Supports a wide range of messaging transports:
   *  MSMQ
   *  RabbitMQ
   *  ActiveMQ
   *  SQL Server
   *  Windows Azure Queues
   *  Windows Azure Service Bus
   *  WebsphereMQ  

#### Getting Started

* **[Download the Particular Software Platform](http://particular.net/downloads)**: the simplest, fastest way to get NServiceBus, free eBook chapters of "Learning NServiceBus," and the entire Particular Software platform    
* **[Hands-on Labs](http://particular.net/HandsOnLabs)**: try NServiceBus in a full-featured virtual lab, the fastest and easiest way to test drive the NServiceBus development experience (no complex setup or installation required). 
* **["Learning NServiceBus" by David Boike (Packt Publishing)](http://www.packtpub.com/build-distributed-software-systems-using-dot-net-enterprise-service-bus/book)**: Your Guide to Becoming a Messaging Expert. 
"Learning NServiceBus" details the process of building a software system based on SOA principles. From sending a simple message to publishing events, David Boike shows you all you need to know to understand and implement an SOA-based message driven systems.
* **[Code-first Step-by-step guide](http://docs.particular.net/nservicebus/NServiceBus-Step-by-Step-Guide)**: the starting point in the [NServiceBus documentation portal](docs.particular.net), covering NServiceBus features and usage patterns.     


## [![ServiceMatrix](/images/servicematrix-logo.jpg "ServiceMatrix for Visual Studio")](http://particular.net/ServiceMatrix)

### Jump-start system design and development 

ServiceMatrix enables you to generate a fully functional distributed application skeleton in a matter of minutes, dramatically reducing the learning curve, POC effort and time-to-market.

![ServiceMatrix](/images/servicematrix-visual.jpg "ServiceMatrix in Visual Studio")

#### Product Highlights

* Design your distributed application on the Canvas surface and have it up-and-running within minutes.
* Multiple visual perspectives help you to easily grasp complex interactions:
   * which components make up which services
   * which contracts those services expose
   * where messages are sent
   * which events those messages cause
   * which services are subscribed to those events
* Extensible and customizable code    
* Supports all NServiceBus transports
* Fully integrated with the Particular Software platform to provide runtime analysis and monitoring tools, OOTB. 

#### Getting Started

* **Introduction to ServiceMatrix** (5 mins video) (TODO)
* **Hands-on-Labs** (TODO)
* **Getting Started with ServiceMatrix** article (TODO)
* **Extending ServiceMatrix generated code** article (TODO)




## [![ServiceInsight](/images/serviceinsight-logo.jpg "ServiceInsight")](http://particular.net/ServiceInsight)

### Essential insights for distributed solutions

Complete under-the-hood view of the your system's behavior, clearly displayed and visualized, immediately available and up-to-date. Whether in development, testing or production - ServiceInsight provides all the system-critical information you need. 


![ServiceInsight](/images/serviceinsight-visual.jpg "ServiceInsight in action")


#### Product Highlights

* View all your endpoints, messages and interactions from a single screen
* Insightful visualizations into your system behavior: 
   * Message flow
   * Saga usage, evolution and properties   
   * Endpoint interactions
   * Timing and performance
* Full-text search, filter and sorting capabilities on all message headers, properties and body
* Detailed error information with full stacktrace
* Performance information for 
* Retry failed messages
* Use Auto-Refresh to display near real-time updated information from across your distributed application
* Supports all NServiceBus transports
* Fully Integrated with the Particular Software Platform:
   * Enhances ServiceMatrix to view your design's runtime behavior, on-the-fly and in realtime. 
   * Launch from ServicePulse to gain insights and analyze failed messages context and root cause
   * Share data and communicate with colleagues using Message URI's  


#### Getting Started

* **Introduction to ServiceInsight** (5 mins video) [Requires update]
* **Getting Started with ServiceInsight** article (TODO)
* **HOL** (?) 

## [![ServicePulse](/images/servicepulse-logo.jpg "ServicePulse")](http://particular.net/ServicePulse)

### Integrated monitoring of your distributed system 

Real-time monitoring that is custom tailored to fit distributed applications in general, and *your application's* specific needs in particular.  


![ServicePulse](/images/servicepulse-visual.jpg "ServicePulse")


#### Product Highlights

* Multiple built-in and customizable Indicators:
   * Active Endpoint heartbeat monitoring and notifications
   * Failed Messages notifications and details
   * Customized validation of your system's health using  Custom Checks 
* Retry	failed messages
* Tight integration with ServiceInsight for advanced context visualization and data
* Supports all NServiceBus transports
* Fully Integrated with the Particular Software Platform:
* Near real-time status updates (using SignalR)
* Accessible from any modern browser and device
* Extensible and customizable
* Full set of REST API exposed for custom development
   

#### Getting Started

* **Introduction to ServicePulse** (5 mins video) [Requires update]
* **Getting Started with ServicePulse** article (TODO)
* **Howto: Author Custom Checks for ServicePulse** article (TODO)
* **Configuring NServiceBus Endpoints for Monitoring by ServicePulse** article (TODO)
* **Introduction to the ServicePulse HTTP API (ServiceControl)** article (TODO)
* **HOL** (?) 


## Licensing

TODO

## Support

TODO
