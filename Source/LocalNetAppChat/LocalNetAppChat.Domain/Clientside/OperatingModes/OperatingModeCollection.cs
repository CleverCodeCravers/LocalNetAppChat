namespace LocalNetAppChat.Domain.Clientside.OperatingModes;

public class OperatingModeCollection
{
    private readonly List<IOperatingMode> _operatingModes = new();
    
    public void Add(IOperatingMode operatingMode)
    {
        _operatingModes.Add(operatingMode);
    }
    
    public IOperatingMode? GetResponsibleOperatingMode(ClientSideCommandLineParameters commandLineParameters)
    {
        return _operatingModes.FirstOrDefault(operatingMode => operatingMode.IsResponsibleFor(commandLineParameters));
    }
}