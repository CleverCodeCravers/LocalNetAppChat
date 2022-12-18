# LocalNetAppChat

LocalNetAppChat (LNAC) is a server/client C# command line interface that gives your apps a way to communicate with each other over the local network.

<!-- TOC -->

- [Features](#features)
- [Installation](#installation)
  - [Server](#server)
  - [Client](#client)
  - [Bot](#bot)
- [Usage](#usage)
  - [Server CLI](#server-cli)
  - [Client CLI](#client-cli)
  - [Bot CLI](#bot-cli)
- [Contributions](#contributions)
- [Questions?](#questions?)

<!-- /TOC -->

## Features

- Available for Windows, Linux and macOS
- Easy to use
- Adds a few simple entry points / command line tools that enable network communication between a group of hosts and sub applications.
- Ability to execute and run tasks between your local apps through command line over the network
- Encrypted communication
- and more

## Installation

### Server

To start using LNAC, you have to install the LNAC Server on your host machine, which will be responsible for handling the communication between your clients. You can download the server from the [Release Page](https://github.com/stho32/LocalNetAppChat/releases) for your preferred operating system.

### Client

The client app is responsible for sending messages to other clients and receiving new updates/messages from the server. You can download the client from the [Release Page](https://github.com/stho32/LocalNetAppChat/releases) as well.

### Bot

The LNAC Bot behaves like a listener and gets the messages from the server. If one of the messages that the clients have sent includes one of the bot commands, it will perform an operation accordingly, for example, if a client sent `/ping`, the bot will send a message to the server including the following: `responding to ping from {clientName} ==> a very dear pong from " + botName`.

To install the bot, simply download it from the [Release Page](https://github.com/stho32/LocalNetAppChat/releases).

## Usage

### Server CLI

After installing the server and the client apps, start the server client with your preferred options:

```console
LocalNetAppChat.Server [options]

  Options:

    --listenOn           The IP Address the server should start litening on (e.g localhost)
    --port               The port the server should connect to (default: 5000)
    --https              Whether to start the server as HTTPS or HTTP server
    --key                An Authentication password that the client should send along the requests to be able to perform tasks. (default: 1234)

  Examples:

  – Start the server with the default settings
    $ LocalNetAppChat.Server
  - Start the server in HTTPS mode
    $ LocalNetAppChat.Server --https
  - Start the server with different ip and port and custom key
    $ LocalNetAppChat.Server --listenOn "54.15.12.1" --port "54822" --https --key "HeythereGithubExample"

```

### Client CLI

The client can be run in two modes, the `message` and `listener` mode. The `message` mode allows the client to send messages to the server that then will be displayed either to all the clients or only to one client. When running the client in `listener` mode, it will only listen for incoming messages done by other clients from the server.

```console
LocalNetAppChat.ConsoleClient [options]

  Options:

    message              Run the client in message mode
    listener             Run the client in listener mode
    --text               The text message to send to the server. (only when in message mode!)
    --clientName         The name of your client. If not specified, your machine name will be sent as clientName to the server
    --ignoresslerrors    Ignore SSL Erros and do not print them in the console
    --server             The IP Address the server should start litening on (e.g localhost)
    --port               The port the server should connect to (default: 5000)
    --https              Whether to start the server as HTTPS or HTTP server
    --key                An Authentication password that the server requires to allow incoming requests from the client!

  Examples:

  – Start the client in listening mode
    $ LocalNetAppChat.ConsoleClient listener --server "localhost" --port 54214 --key 1234 --clientName "GithubReadMe"
  - Start the client in message mode
    $ LocalNetAppChat.ConsoleClient message --server "localhost" --port 51234 --key 1234 --text "Hey there, I am client GithubReadMe"

```

### Bot CLI

```console
LocalNetAppChat.Bot [options]

  Options:

    --server             The IP Address the bot should connect to (e.g localhost)
    --port               The port that the bot should connect to (default: 5000)
    --https              Whether to connect per HTTP or HTTPs
    --key                An Authentication password that the bot should send along the requests to be able to perform tasks. (default: 1234)
    --clientName         Specifies the bot name, otherwise the name of the machine will be used
    --ignoresslerrors    Whether to ignore SSL Erros in console.

  Examples:

  – Start the bot
    $ LocalNetAppChat.Bot --server "localhost" --port 54214 --key 1234 --clientName "TheBestBot"

```

## Contributions

Software contributions are welcome. If you are not a dev, testing and reporting bugs can also be very helpful!

## Questions?

Please open an issue if you have questions, wish to request a feature, etc.
