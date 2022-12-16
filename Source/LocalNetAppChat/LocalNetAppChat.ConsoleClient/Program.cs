using LocalNetAppChat.Domain.Clientside;
using LocalNetAppChat.Domain.Clientside.OperatingModes;

namespace LocalNetAppChat.ConsoleClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var parser = new ClientSideCommandLineParser();

            var commandLineParametersResult = parser.Parse(args);

            if (!commandLineParametersResult.IsSuccess)
            {
                Console.WriteLine("Unfortunately there have been problems with the command line arguments.");
                Console.WriteLine("");
                return;
            }

            var parameters = commandLineParametersResult.Value;

            try
            {
                var operatingModeCollection = new OperatingModeCollection();
                operatingModeCollection.Add(new SendMessageOperatingMode());
                operatingModeCollection.Add(new ListenerOperatingMode());

                var operatingMode = operatingModeCollection.GetResponsibleOperatingMode(parameters);
                if (operatingMode == null)
                {
                    Console.WriteLine("No mode selected");
                }
                else
                {
                    await operatingMode?.Run(parameters);                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("Exception: " + ex.Message);
                Console.WriteLine("");
            }
        }
    }
}