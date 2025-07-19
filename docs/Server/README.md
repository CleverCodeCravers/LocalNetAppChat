# LocalNetAppChat Server

The LNAC Server is responsible for handling communication and incoming requests from the clients. It is the main core of the application and has a lot of features.

## Features

The LNAC Server is responsible for almost everything:

- Basic communication handling (Sending and Receiving messages to/from clients)
- Basic Storage API (Can receive, upload and delete Files existing in the script folder on the folder)
- Parsing and Handling `Direct Messages` between clients
- Duplicate message prevention (1 hour retention)
- Task management system for distributed work
- Comprehensive logging with daily rotation (14-day retention)

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

## Server API Endpoints

### Messaging
- `GET /receive?key={key}&clientName={clientName}` - Poll for new messages
- `POST /send?key={key}` - Send a message (body: LnacMessage JSON)

### File Storage
- `POST /upload?key={key}` - Upload a file (multipart/form-data)
- `GET /download?key={key}&filename={filename}` - Download a file
- `GET /listallfiles?key={key}` - List all files
- `POST /deletefile?key={key}&filename={filename}` - Delete a file

### Task Management
- `POST /tasks/create?key={key}` - Create a new task (body: TaskMessage JSON)
- `GET /tasks/pending?key={key}&tags={tags}` - Get pending tasks (optional tag filter)
- `POST /tasks/claim?key={key}&taskId={id}&clientName={name}` - Claim a task
- `POST /tasks/complete?key={key}&taskId={id}&clientName={name}&success={bool}&result={result}` - Complete a task
- `GET /tasks/status?key={key}&taskId={id}` - Get task status

## Logging

The server uses Serilog for comprehensive logging:
- Console output with timestamps
- Daily rotating log files in `logs/` directory
- 14-day retention policy
- Log files named: `lnac-server-YYYYMMDD.log`

## Security

- Key-based authentication required for all endpoints
- Messages are rejected if duplicate ID is detected within 1 hour
- HTTPS support with `--https` flag
