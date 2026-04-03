namespace LocalNetAppChat.Cli.Plugins.ScriptExecution.ScriptExecutors
{
    public class PowerShellScriptExecutor : ScriptExecutor
    {
        public PowerShellScriptExecutor(string scriptsPath) : base("powershell.exe", new[] { "-NoProfile", "-ExecutionPolicy", "ByPass", "-File" }, scriptsPath, ".ps1")
        {

        }
    }
}
