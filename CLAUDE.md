# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

LocalNetAppChat (LNAC) is a client-server application for local network communication written in C#/.NET 7.0. It consists of:
- **Server**: ASP.NET Core Web API handling message routing and file storage
- **Client**: Console application for sending/receiving messages
- **Bot**: Plugin-based application for executing scripts and responding to commands

## Essential Development Commands

### Building the Project
```bash
# Build entire solution
cd Source/LocalNetAppChat
dotnet build

# Build specific project
dotnet build LocalNetAppChat.Server/LocalNetAppChat.Server.csproj

# Build in Release mode
dotnet build --configuration Release
```

### Running Tests
```bash
# Run all tests
cd Source/LocalNetAppChat
dotnet test

# Run tests for specific project
dotnet test LocalNetAppChat.Domain.Tests/LocalNetAppChat.Domain.Tests.csproj
dotnet test LocalNetAppChat.Server.Domain.Tests/LocalNetAppChat.Server.Domain.Tests.csproj
```

### Running Applications
```bash
# Run Server
cd Source/LocalNetAppChat/LocalNetAppChat.Server
dotnet run -- --key "MySecretKey"

# Run Client (example: chat mode)
cd Source/LocalNetAppChat/LocalNetAppChat.ConsoleClient
dotnet run -- chat --server localhost --key "MySecretKey" --clientName "TestClient"

# Run Bot
cd Source/LocalNetAppChat/LocalNetAppChat.Bot
dotnet run -- --server localhost --key "MySecretKey" --clientName "TestBot"
```

## Architecture Overview

### Project Structure
```
Source/LocalNetAppChat/
├── CommandLineArguments/           # Shared CLI parsing utilities
├── LocalNetAppChat.Bot/           # Bot application with plugin system
│   └── Plugins/                   # Plugin implementations (Ping, Execute, etc.)
├── LocalNetAppChat.ConsoleClient/ # Client console application
├── LocalNetAppChat.Domain/        # Shared domain logic and models
├── LocalNetAppChat.Server/        # Web API server application
└── LocalNetAppChat.Server.Domain/ # Server-specific domain logic
```

### Key Architectural Patterns

1. **Message Processing Pipeline**
   - Server uses a pipeline pattern for processing incoming messages
   - Pipeline components: SecurityValidation → MessageParsing → DirectMessageProcessing → Storage
   - Located in `LocalNetAppChat.Server.Domain/MessageProcessing/`

2. **Plugin Architecture (Bot)**
   - Bot plugins implement `IPlugin` interface
   - Plugins respond to specific commands (e.g., `/ping`, `exec`)
   - New plugins can be added in `LocalNetAppChat.Bot/Plugins/`

3. **Command Line Parsing**
   - Shared `CommandLineArguments` library handles CLI parsing
   - Uses custom tokenizer for parsing complex command strings
   - Supports both simple arguments and key-value pairs

4. **Client Operating Modes**
   - `listener`: Receive messages only
   - `message`: Send single message
   - `chat`: Interactive send/receive
   - `fileupload/download/delete/listfiles`: File operations

5. **Security Model**
   - Simple key-based authentication (shared secret)
   - All clients must provide matching key to connect
   - Server validates key on every request

### Important Design Decisions

1. **Direct Messaging**: Recent enhancement allows 1:n messaging where messages can be sent to specific clients using `/msg ClientName message`

2. **File Storage**: Server maintains central file storage accessible by all authenticated clients

3. **Script Execution**: Bot can execute PowerShell and Python scripts from designated scripts folder

4. **Cross-Platform**: Uses .NET 7.0 for Windows, Linux, and macOS support

5. **Communication Protocol**: HTTP/HTTPS with optional SSL certificate validation bypass for development

## Common Development Tasks

When implementing new features:

1. **Adding New Bot Commands**: Create new plugin in `LocalNetAppChat.Bot/Plugins/` implementing `IPlugin`

2. **Modifying Message Processing**: Update pipeline components in `LocalNetAppChat.Server.Domain/MessageProcessing/`

3. **Adding Client Commands**: Extend command parsing in `LocalNetAppChat.ConsoleClient/`

4. **Testing**: Add unit tests in corresponding `.Tests` projects maintaining existing patterns