using LocalNetAppChat.Domain.Clientside.ServerApis;
using LocalNetAppChat.Domain.Shared.Inputs;
using LocalNetAppChat.Domain.Shared.Outputs;

namespace LocalNetAppChat.Domain.Clientside.OperatingModes;

public class ListAllFilesOperatingMode : IOperatingMode
{
  public bool IsResponsibleFor(ClientSideCommandLineParameters parameters)
  {
    return parameters.ListServerFiles;
  }

  public async Task Run(ClientSideCommandLineParameters parameters, IOutput output, ILnacServer lnacServer, IInput input)
  {
    var files = lnacServer.GetServerFiles();
    foreach (var file in files)
    {
      output.WriteLineUnformatted(file);
    }
  }
}