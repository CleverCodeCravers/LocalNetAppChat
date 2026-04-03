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

    --listenOn           The IP Address the server should start listening on (e.g localhost)
    --port               The port the server should connect to (default: 5000)
    --https              Whether to start the server as HTTPS or HTTP server
    --key                Authentication key (REQUIRED). Can also be set via LNAC_KEY environment variable.
    --message-lifetime   Message lifetime in minutes (default: 10)

  Examples:

  – Start the server with a key
    $ LocalNetAppChat.Server --key "MySecretKey"
  - Start the server in HTTPS mode
    $ LocalNetAppChat.Server --key "MySecretKey" --https
  - Start the server with custom settings
    $ LocalNetAppChat.Server --listenOn "54.15.12.1" --port "54822" --https --key "MySecretKey"
  - Start the server using environment variable
    $ export LNAC_KEY="MySecretKey"
    $ LocalNetAppChat.Server

```

## Server API Endpoints

Authentication is provided via `X-API-Key` header (recommended) or `key` query parameter (legacy).
Rate limiting: 100 requests per minute per IP address.

### Messaging
- `GET /receive?clientName={clientName}` - Poll for new messages
- `POST /send` - Send a message (body: LnacMessage JSON)

### File Storage
- `POST /upload` - Upload a file (multipart/form-data)
- `GET /download?filename={filename}` - Download a file
- `GET /listallfiles` - List all files
- `POST /deletefile?filename={filename}` - Delete a file

### Task Management
- `POST /tasks/create` - Create a new task (body: TaskMessage JSON)
- `GET /tasks/pending?tags={tags}` - Get pending tasks (optional tag filter)
- `POST /tasks/claim?taskId={id}&clientName={name}` - Claim a task
- `POST /tasks/complete?taskId={id}&clientName={name}&success={bool}&result={result}` - Complete a task
- `GET /tasks/status?taskId={id}` - Get task status

## Logging

The server uses Serilog for comprehensive logging:
- Console output with timestamps
- Daily rotating log files in `logs/` directory
- 14-day retention policy
- Log files named: `lnac-server-YYYYMMDD.log`

## Security

- Key-based authentication required for all endpoints (via `X-API-Key` header or query parameter)
- Rate limiting: 100 requests per minute per IP address
- Messages are rejected if duplicate ID is detected within 1 hour
- HTTPS support with `--https` flag
- Security headers: `X-Content-Type-Options: nosniff`, `X-Frame-Options: DENY`
