namespace LocalNetAppChat.Bot.Plugins.ScriptExecution.ScriptExecutors
{
    public class PowerShellScriptExecutor : ScriptExecutor
    {
        public PowerShellScriptExecutor(string scriptsPath) : base("powershell.exe", "-NoProfile -ExecutionPolicy ByPass -File {0} {1}", scriptsPath, ".ps1")
        {

        }
    }
}
