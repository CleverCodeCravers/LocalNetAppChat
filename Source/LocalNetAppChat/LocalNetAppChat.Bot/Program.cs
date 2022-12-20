using LocalNetAppChat.Bot.PluginProcessor;
using LocalNetAppChat.Bot.PluginProcessor.Plugins;
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

            var scriptsPathParameter = "";
            var executors = new ScriptExecutorCollection();
            if (!string.IsNullOrEmpty(scriptsPathParameter))
            {
                executors = ScriptExecutorFactory.Get(scriptsPathParameter);
            }
            // TODO: Wenn die über die kommandozeile Parameter ein ScriptsPath angegeben wurde, dann über die Factory die echten Interpreter holen
            var parameters = commandLineParametersResult.Value;
            ILnacServer lnacServer = new LnacServer(
                parameters.Server, parameters.Port, parameters.Https, parameters.IgnoreSslErrors,
                parameters.ClientName, parameters.Key);

            var clientCommands = new ClientCommandCollection();
            clientCommands.Add(new ExecuteScriptClientCommand(executors));

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
                    output.WriteLine(e.ToString());
                }
            }
        }
    }
}