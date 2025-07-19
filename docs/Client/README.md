# LocalNetAppChat Client

The LNAC Client allows you to communicate with the server as well as with other sub applications.

## Features

- 9 different modes which perform different tasks

### Client Modes

#### Listener

When starting the server in this mode, the Client will only act as a listener and fetch new messages from the LNAC Server.

#### Message

Consider the message mode as a command to send a message to the server. This can either be a public message which will be sent to all other sub applications/clients or can be a direct message to a specific client.

To send a direct message to client "Bob":

```
$ LocalNetAppChat.ConsoleClient message --server "localhost" --port 51234 --key 1234 --text "/msg Bob Hey there, I am client GithubReadMe"
```

The "/msg" command is a special command that allows you to send a message to only one specific client.

#### Chat

The chat mode will allow you to send messages from the CLI as well as receive messages. Think of it as `listener` and `message` combined.

#### File Operation

The File operation mode allows the client to upload, download, delete and get a list of all scripts exisiting on the server.

- fileupload // Will upload a given file to the server.
- filedownload // Will download a file from the server and save it locally.
- filedelete // Will delete the given file from the server.
- listfiles // Will return a list of all existing files on the server.

#### Task Receiver

The task receiver mode allows the client to process tasks from the server. The client polls for new tasks with matching tags, claims them, executes a configured processor script, and reports results back to the server.

```
$ LocalNetAppChat.ConsoleClient taskreceiver --server "localhost" --port 51234 --key 1234 --tags "build,test" --processor "./run-task.ps1"
```

#### Emitter

The emitter mode allows the client to execute a command and stream its output line-by-line to the server in real-time. Each line of output is sent immediately as a message, making it perfect for log streaming, monitoring, or continuous data generation.

```
$ LocalNetAppChat.ConsoleClient emitter --server "localhost" --port 51234 --key 1234 --clientName "LogEmitter" --command "tail -f /var/log/app.log"
```

## Client CLI

```console
LocalNetAppChat.ConsoleClient [options]

  Options:

    message              Run the client in message mode
    listener             Run the client in listener mode
    fileupload           Uploads a given file to the server
    filedownload         Downloads an existing file from the server
    filedelete           Deletes an existing file from the server
    listfiles            Returns a list of all existing files on the server
    chat                 Runs the client essentially in a listener mode, but when you start typing you are delivered a prompt and with    enter you will send the message
    taskreceiver         Run the client in task receiver mode to process tasks
    emitter              Run the client in emitter mode to stream command output
    --file               Path of the file you want to delete, download or upload from/to the server
    --targetPath         Path where you want the requested File to be saved at after downloading it
    --text               The text message to send to the server. (only when in message mode!)
    --clientName         The name of your client. If not specified, your machine name will be sent as clientName to the server
    --ignoresslerrors    Ignore SSL Erros and do not print them in the console
    --server             The IP Address the server should start litening on (e.g localhost)
    --port               The port the server should connect to (default: 5000)
    --https              Whether to start the server as HTTPS or HTTP server
    --key                An Authentication password that the server requires to allow incoming requests from the client!
    --tags               Comma-separated list of tags for task filtering in task receiver mode
    --processor          Path to the script/executable to process tasks in task receiver mode
    --command            Command to execute in emitter mode (including arguments)

  Examples:

  – Start the client in listening mode
    $ LocalNetAppChat.ConsoleClient listener --server "localhost" --port 54214 --key 1234 --clientName "GithubReadMe"
  - Start the client in message mode
    $ LocalNetAppChat.ConsoleClient message --server "localhost" --port 51234 --key 1234 --text "Hey there, I am client GithubReadMe"
  - Start the client in chat mode
    LocalNetAppChat.ConsoleClient chat --server "localhost" --port 54214 --key 1234 --clientName "GithubReadMe"
  - Upload a file to the server
    $ LocalNetAppChat.ConsoleClient fileupload --server "localhost" --port 51234 --key 1234 --file "./README.md"
  - Download a file from the server
    $ LocalNetAppChat.ConsoleClient filedownload --server "localhost" --port 51234 --key 1234 --file "./README.md" --targetPath "/home/github/Projects"
  - Deletes a file from the server
    $ LocalNetAppChat.ConsoleClient filedelete --server "localhost" --port 51234 --key 1234 --file "README.md"
  - List all files existing on the server
    $ LocalNetAppChat.ConsoleClient listfiles --server "localhost" --port 51234 --key 1234
  - Run the client in task receiver mode
    $ LocalNetAppChat.ConsoleClient taskreceiver --server "localhost" --port 51234 --key 1234 --tags "build,test" --processor "./run-task.ps1"
  - Run the client in emitter mode to stream command output
    $ LocalNetAppChat.ConsoleClient emitter --server "localhost" --port 51234 --key 1234 --command "ping google.com"

```
