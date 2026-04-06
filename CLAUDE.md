# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

LocalNetAppChat (LNAC) is a unified CLI tool for local network communication written in C#/.NET 10.0. A single `lnac` binary provides:
- **Server mode**: ASP.NET Core Web API handling message routing, file storage, and task management
- **Client modes**: Send/receive messages, file operations, task processing, emitter
- **Bot mode**: Plugin-based command execution (PowerShell/Python scripts)

## Essential Development Commands

### Building the Project
```bash
cd Source/LocalNetAppChat
dotnet build

# Or use the build script
./build.ps1 build
```

### Running Tests
```bash
cd Source/LocalNetAppChat
dotnet test
```

### Running the Application
```bash
cd Source/LocalNetAppChat/LocalNetAppChat.Cli

# Server mode
dotnet run -- server --key "MySecretKey"

# Client modes
dotnet run -- message --server localhost --key "MySecretKey" --text "Hello"
dotnet run -- listener --server localhost --key "MySecretKey"
dotnet run -- chat --server localhost --key "MySecretKey"

# Bot mode
dotnet run -- bot --server localhost --key "MySecretKey" --scriptspath ./scripts
```

## Architecture Overview

### Project Structure
```
Source/LocalNetAppChat/
├── CommandLineArguments/           # Shared CLI parsing utilities
├── LocalNetAppChat.Cli/           # Unified CLI application (server + client + bot)
│   └── Plugins/                   # Bot plugin implementations (Ping, Execute, Tasks)
├── LocalNetAppChat.Domain/        # Shared domain logic and models
├── LocalNetAppChat.Server.Domain/ # Server-specific domain logic
├── LocalNetAppChat.Domain.Tests/
├── LocalNetAppChat.Server.Domain.Tests/
└── LocalNetAppChat.Bot.Tests/
```

### Key Architectural Patterns

1. **Unified CLI with Subcommands**
   - Single entry point (`lnac`) routes to server, client, or bot mode
   - Server mode starts ASP.NET Core with Minimal API endpoints
   - Client modes use the OperatingMode strategy pattern
   - Bot mode runs a polling loop with plugin-based command execution

2. **Message Processing Pipeline**
   - Server uses a pipeline pattern for processing incoming messages
   - Pipeline: AddId → AddTimestamp → ExtractReceiver
   - Located in `LocalNetAppChat.Server.Domain/Messaging/MessageProcessing/`

3. **Plugin Architecture (Bot mode)**
   - Bot plugins implement `IClientCommand` interface
   - Plugins respond to specific commands (e.g., `/ping`, `exec`)
   - Plugin implementations in `LocalNetAppChat.Cli/Plugins/`

4. **Client Operating Modes**
   - `listener`: Receive messages only
   - `message`: Send single message
   - `chat`: Interactive send/receive
   - `bot`: Automated command execution with plugins
   - `emitter`: Stream command output to server
   - `taskreceiver`: Process distributed tasks
   - `fileupload/download/delete/listfiles`: File operations

5. **Security**
   - API key authentication via `X-API-Key` header (or query parameter for backward compat)
   - Constant-time key comparison (CryptographicOperations.FixedTimeEquals)
   - Rate limiting: 100 req/min per IP
   - Security headers: X-Content-Type-Options, X-Frame-Options
   - HTTPS support via `--https` flag

## Common Development Tasks

1. **Adding New Bot Commands**: Create plugin in `LocalNetAppChat.Cli/Plugins/` implementing `IClientCommand`

2. **Modifying Message Processing**: Update pipeline in `LocalNetAppChat.Server.Domain/Messaging/MessageProcessing/`

3. **Adding Client Modes**: Create new `IOperatingMode` in `LocalNetAppChat.Domain/Clientside/OperatingModes/`

4. **Testing**: Add unit tests in corresponding `.Tests` projects maintaining existing NUnit patterns
