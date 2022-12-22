# LocalNetAppChat Server

The LNAC Server is responsible for handling communication and incoming requests from the clients. It is the main core of the application and has a lot of features.

## Features

The LNAC Server is responsible for almost everything:

- Basic communication handling (Sending and Receiving messages to/from clients)
- Basic Storage API (Can receive, upload and delete Files existing in the script folder on the folder)
- Parsing and Handling `Direct Messages` between clients

## Server CLI

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
