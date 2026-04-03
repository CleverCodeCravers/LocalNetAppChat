# LocalNetAppChat

[![.github/workflows/bot.yml](https://github.com/CleverCodeCravers/LocalNetAppChat/actions/workflows/bot.yml/badge.svg)](https://github.com/CleverCodeCravers/LocalNetAppChat/actions/workflows/bot.yml)
[![.github/workflows/client.yml](https://github.com/CleverCodeCravers/LocalNetAppChat/actions/workflows/client.yml/badge.svg)](https://github.com/CleverCodeCravers/LocalNetAppChat/actions/workflows/client.yml)

Server : [![.github/workflows/server.yml](https://github.com/CleverCodeCravers/LocalNetAppChat/actions/workflows/server.yml/badge.svg)](https://github.com/CleverCodeCravers/LocalNetAppChat/actions/workflows/server.yml) [![Coverage Status](https://coveralls.io/repos/github/CleverCodeCravers/LocalNetAppChat/badge.svg?branch=main)](https://coveralls.io/github/CleverCodeCravers/LocalNetAppChat?branch=main)

**Zero-Config .NET LAN Automation** -- a lightweight server/client C# tool that gives your apps a way to communicate, execute scripts, and share files across your local network. No Docker, no SSH, no complex setup -- just download and run.

<!-- TOC -->

-   [Vision](#vision)
-   [Features](#features)
-   [Usage](#usage)
    -   [Server CLI](./docs/Server/README.md)
    -   [Client CLI](./docs/Client/README.md)
    -   [Bot CLI](./docs/Bot/README.md)
-   [Installation](#installation)
    -   [Server](#server)
    -   [Client](#client)
    -   [Bot](#bot)
-   [Contributions](#contributions)
-   [Questions?](#questions?)

<!-- /TOC -->

## Why LNAC?

Unlike Ansible (requires SSH), Rundeck (requires a web server), or NATS (requires infrastructure knowledge), LNAC is designed for **zero-config simplicity**:

-   **One binary per component** -- download, run, done
-   **No agents to install** -- clients connect to a central server via HTTP
-   **No infrastructure needed** -- no Docker, no databases, no message brokers
-   **.NET native** -- integrates naturally with C#/.NET ecosystems
-   **Cross-platform** -- Windows, Linux, macOS

Perfect for small teams, lab environments, build farms, or anyone who needs lightweight automation without the overhead of enterprise tools.

## Vision

LNAC gives you a central service to group computers together and automate tasks across your local network.

The focus usage points are:

-   Collecting execution log information (sending messages that can be reviewed by a human)
-   Sending commands to bots either direct or as a broadcast and receiving their results
-   Exchanging files between the computers that take part
-   Distributing work across multiple machines with the task system

Usage Scenarios include:

-   [CI/CD-Pipeline](./docs/usage-cicd-pipeline.md)
-   [Central Log](./docs/usage-central-log.md)
-   [Math Calculation Bots](./docs/scenarios/math-bots/README.md)
-   [Distributed Build System](./docs/scenarios/distributed-build/README.md)
-   [Monitoring & Alerting](./docs/scenarios/monitoring-alerting/README.md)
-   [File Sync & Backup](./docs/scenarios/file-sync/README.md)
-   [More Scenarios...](./docs/scenarios/README.md)

## Features

-   Available for Windows, Linux and macOS
-   Easy to use
-   Adds a few simple entry points / command line tools that enable network communication between a group of hosts and sub applications
-   **Task System**: Distribute work across multiple clients with tag-based routing
-   **Duplicate Prevention**: Messages with same ID are rejected within 1 hour
-   **Comprehensive Logging**: Daily rotating logs with Serilog integration
-   **Direct Messaging**: Send messages to specific clients using `/msg ClientName`
-   **File Storage**: Central file repository accessible by all clients
-   **Emitter Mode**: Stream command output line-by-line in real-time
-   Ability to execute and run tasks between your local apps through command line over the network
-   Encrypted communication
-   and more

## Showcase

![](./docs//Showcase.gif)

## Installation

### Server

To start using LNAC, you have to install the LNAC Server on your host machine, which will be responsible for handling the communication between your clients. You can download the server from the [Release Page](https://github.com/stho32/LocalNetAppChat/releases) for your preferred operating system.

### Client

The client app is responsible for sending messages to other clients and receiving new updates/messages from the server. You can download the client from the [Release Page](https://github.com/stho32/LocalNetAppChat/releases) as well.

### Bot

The LNAC Bot behaves like a listener and gets the messages from the server. If one of the messages that the clients have sent includes one of the bot commands, it will perform an operation accordingly, for example, if a client sent `/ping`, the bot will send a message to the server including the following: `responding to ping from {clientName} ==> a very dear pong from " + botName`.

To install the bot, simply download it from the [Release Page](https://github.com/stho32/LocalNetAppChat/releases).

## Contributions

Software contributions are welcome. If you are not a dev, testing and reporting bugs can also be very helpful!

## Questions?

Please open an issue if you have questions, wish to request a feature, etc.
