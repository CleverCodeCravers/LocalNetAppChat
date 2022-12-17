using LocalNetAppChat.Domain.Clientside;
using LocalNetAppChat.Domain.Clientside.OperatingModes;
using LocalNetAppChat.Domain.Shared.Outputs;

namespace LocalNetAppChat.ConsoleClient
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IOutput output = new ConsoleOutput();
            
            var parser = new ClientSideCommandLineParser();

            var commandLineParametersResult = parser.Parse(args);

            if (!commandLineParametersResult.IsSuccess)
            {
                output.WriteLine("Unfortunately there have been problems with the command line arguments.");
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
                    output.WriteLine("No mode selected");
                }
                else
                {
                    await operatingMode?.Run(parameters, output)!;                    
                }
            }
            catch (Exception ex)
            {
                output.WriteLine("Exception: " + ex.Message);
            }
        }
    }
}