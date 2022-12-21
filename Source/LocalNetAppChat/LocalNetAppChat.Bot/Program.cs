using LocalNetAppChat.Bot.Plugins.ScriptExecution;
using LocalNetAppChat.Domain.Bots.ClientCommands;
using LocalNetAppChat.Domain.Clientside;
using LocalNetAppChat.Domain.Clientside.ServerApis;
using LocalNetAppChat.Domain.Shared.Inputs;
using LocalNetAppChat.Domain.Shared.Outputs;

namespace LocalNetAppChat.Bot
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            IOutput output = new ConsoleOutput();
            IInput input = new ConsoleInput(); 
            
            var parser = new ServerConnectionCommandLineParser();
            var commandLineParametersResult = parser.Parse(args);

            if (!commandLineParametersResult.IsSuccess)
            {
                output.WriteLine("Unfortunately there have been problems with the command line arguments.");
                return;
            }

            var parameters = commandLineParametersResult.Value;

            ILnacServer lnacServer = new LnacServer(
                parameters.Server, parameters.Port, parameters.Https, parameters.IgnoreSslErrors,
                parameters.ClientName, parameters.Key);

            var clientCommands = new ClientCommandCollection();

            if (!Plugins.DefaultFunctionality.DefaultPlugin.AddCommands(clientCommands, args))
            {
                output.WriteLine("Unfortunately there have been problems with the command line arguments.");
            }

            if (!ScriptExecutionPlugin.AddCommands(clientCommands, args))
            {
                output.WriteLine("Unfortunately there have been problems with the command line arguments.");
            }

            while (true)
            {
                try
                {
                    var messages = lnacServer.GetMessages();
                    
                    foreach (var message in messages)
                    {
                        output.WriteLine(message);
                        var result = clientCommands.Execute(message.Message.Text);
                        await lnacServer.SendMessage($"/msg {message.Message.Name} {result}");
                    }
                }
                catch (Exception e)
                {
                    output.WriteLine(e.Message + ": Retry...");
                }
            }
        }
    }
}