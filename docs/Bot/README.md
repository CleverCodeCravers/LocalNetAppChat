# LocalNetAppChat Bot

The LNAC Bot only reacts to messages that are sent directly to it. It's main purpose for now is to execute special commands on the machine it is running on and send the result of the execution to the client who requested it. It's customizable and offers a Powershell and Python plugins by default.

## Features

- Can execute Powershell/Python commands on the machine // for the moment
- Only reacts if directly spoken to

## Commands

### Default Commands
- `/ping` - Simple ping pong command to test bot connectivity

### Script Execution Commands (Private Messages Only)
- `exec filename parameter1 parameter2 parameter 3` - Execute an existing script on the given path
- `help filename` - Special command for PowerShell scripts only. Parses the parameters defined at the top of the script
- `listcommands` - Returns a list of all existing scripts on the given path

### Task Commands
- `/task "description" tags:tag1,tag2 params:{"key":"value"}` - Create a new task with optional tags and parameters
- `/tasklist tags:tag1,tag2` - List pending tasks (optional tag filter)
- `/taskstatus <task-id>` - Check the status of a specific task

## Bot CLI

```console
LocalNetAppChat.Bot [options]

  Options:

    --server             The IP Address the bot should connect to (e.g localhost)
    --port               The port that the bot should connect to (default: 5000)
    --https              Whether to connect per HTTP or HTTPs
    --key                An Authentication password that the bot should send along the requests to be able to perform tasks. (default: 1234)
    --clientName         Specifies the bot name, otherwise the name of the machine will be used
    --ignoresslerrors    Whether to ignore SSL Erros in console.
    --scriptspath        Path of the scripts folder

  Examples:

  – Start the bot
    $ LocalNetAppChat.Bot --server "localhost" --port 54214 --key 1234 --clientName "TheBestBot" --scriptspath "./home/ScriptsFolder"

```
