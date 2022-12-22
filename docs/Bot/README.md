# LocalNetAppChat Bot

The LNAC Bot only reacts to messages that are sent directly to it. It's main purpose for now is to execute special commands on the machine it is running on and send the result of the execution to the client who requested it. It's customizable and offers a Powershell and Python plugins by default.

## Features

- Can execute Powershell/Python commands on the machine // for the moment
- Only reacts if directly spoken to

## Commands

- `exec filename parameter1 parameter2 parameter 3` will execute an existing script on the given path
- `help filename` is a special command for powershell scripts only. It parses the Parameters that are defined at the top of the powershell script
- `listcommands` will return a list of all existing scripts on the given path.
- `/ping` well, just another ping pong command :)

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
