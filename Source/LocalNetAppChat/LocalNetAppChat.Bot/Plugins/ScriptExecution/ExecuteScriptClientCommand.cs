﻿using LocalNetAppChat.Bot.Plugins.ScriptExecution.ScriptExecutors;
using LocalNetAppChat.Domain.Bots.ClientCommands;
using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Bot.Plugins.ScriptExecution
{


    public class ExecuteScriptClientCommand : IClientCommand
    {

        private ScriptExecutorCollection _executors = new();
        public ExecuteScriptClientCommand(ScriptExecutorCollection executors)
        {
            _executors = executors;
        }

        public bool IsReponsibleFor(string keyword)
        {
            return keyword == "exec";
        }


        public string Execute(string arguments)
        {
            var rest = arguments;
            var scriptName = CommandMessageTokenizer.GetToken(ref rest);
            return _executors.Execute(scriptName, rest);
        }
    }

}
