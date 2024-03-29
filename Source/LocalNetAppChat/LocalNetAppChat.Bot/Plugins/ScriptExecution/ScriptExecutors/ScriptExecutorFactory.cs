﻿namespace LocalNetAppChat.Bot.Plugins.ScriptExecution.ScriptExecutors
{
    public static class ScriptExecutorFactory
    {
        public static ScriptExecutorCollection Get(string scriptsPath)
        {
            var result = new ScriptExecutorCollection();

            result.Add(new PowerShellScriptExecutor(scriptsPath));
            result.Add(new PythonScriptExecutor(scriptsPath));

            return result;
        }
    }
}
