using LocalNetAppChat.Domain.Clientside;
using LocalNetAppChat.Domain.Clientside.OperatingModes;
using LocalNetAppChat.Domain.Clientside.ServerApis;
using LocalNetAppChat.Domain.Shared.Inputs;
using LocalNetAppChat.Domain.Shared.Outputs;

namespace LocalNetAppChat.ConsoleClient
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            IOutput output = new ConsoleOutput();
            IInput input = new ConsoleInput();

            var parser = new ClientSideCommandLineParser();

            var commandLineParametersResult = parser.Parse(args);

            if (!commandLineParametersResult.IsSuccess)
            {
                output.WriteLine("Unfortunately there have been problems with the command line arguments.");
                return;
            }

            var parameters = commandLineParametersResult.Value;

            var apiAccessor = new WebApiServerApiAccessor(
                parameters.Server, parameters.Port, parameters.Https, parameters.IgnoreSslErrors,
                parameters.Key,
                parameters.ClientName
            );
            
            ILnacClient lnacClient = new LnacClient(apiAccessor, parameters.ClientName);

            var operatingModeCollection = new OperatingModeCollection();
            operatingModeCollection.Add(new SendMessageOperatingMode());
            operatingModeCollection.Add(new ListenerOperatingMode());
            operatingModeCollection.Add(new ChatOperatingMode());
            operatingModeCollection.Add(new UploadFileOperatingMode());
            operatingModeCollection.Add(new ListAllFilesOperatingMode());
            operatingModeCollection.Add(new DownloadFileOperatingMode());
            operatingModeCollection.Add(new DeleteFileOperatingMode());

            var operatingMode = operatingModeCollection.GetResponsibleOperatingMode(parameters);
            if (operatingMode == null)
            {
                output.WriteLine("No mode selected");
            }
            else
            {
                await operatingMode?.Run(parameters, output, lnacClient, input)!;
            }
        }
    }
}