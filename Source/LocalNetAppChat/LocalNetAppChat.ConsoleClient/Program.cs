using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using LocalNetAppChat.Domain.Clientside;
using LocalNetAppChat.Domain.Clientside.OperatingModes;
using LocalNetAppChat.Domain.Shared;

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
    operatingMode?.Run(parameters);
    
} catch (Exception ex) {
    
    Console.WriteLine("");
    Console.WriteLine("Exception: " + ex.Message);
    Console.WriteLine("");
    
}





