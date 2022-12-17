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

            while (true)
            {
                try
                {
                    var messages = lnacServer.GetMessages();
                    
                    foreach (var message in messages)
                    {
                        output.WriteLine(message);

                        if (message.Message.Text == "/ping")
                        {
                            await lnacServer.SendMessage($"responding to ping from {message.Message.Name} ==> a very dear pong from " + parameters.ClientName);
                        }
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