# LocalNetAppChat

LocalNetAppChat (LNAC) is a server/client command line interface that gives your apps a way to communicate with each other localy.

<!-- TOC -->

- [Features](#features)
- [Installation](#installation)
  - [Server](#server)
  - [Client](#client)
- [Usage](#usage)
  - [Server CLI](#server-cli)
  - [Client CLI](#client-cli)
- [ToDo](#todo)
- [Contributions](#contributions)
- [Questions?](#questions?)

<!-- /TOC -->

## Features

- Available for Windows, Linux and macOS
- Easy to use
- Direct Communication between apps
- Ability to execute and run tasks between your local apps through command line
- Encrypted communication
- and more

## Installation

### Server

To start using LNAC, you have to install the LNAC Server on your host machine, which will be responsible for handling the communication between your clients. You can download the server from the [Release Page](https://github.com/stho32/LocalNetAppChat/releases) for your preffered operating system.

### Client

The client app is responsible for sending messages to other clients and recieving new updates/messages from the server. You can download the client from the [Release Page](https://github.com/stho32/LocalNetAppChat/releases) as well.

## Usage

### Server CLI

After installing the server and the client apps, start the server client with your preffered options:

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

The client can be run in two modes, the `message` and `listener` mode. The `message` mode allows the client to send messages to the server that then will be displayed either to all the clients or only to one client. When running the client in `listener` mode, it will only listen to incoming messages done by other clients from the server.

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

## ToDo

- [x] add documentation
- [ ] add task management
  - [ ] is task management another bot?
- [ ] powershell clients
- [ ] do message management more correctly (forget them after 1 hour)
- [ ] microsoft teams relay?
- [ ] discord relay?
- [ ] performance testing with naseifs typescript implementation

## Contributions

Software contributions are welcome. If you are not a dev, testing and reproting bugs can also be very helpful!

## Questions?

Please open an issue if you have questions, wish to request a feature, etc.
