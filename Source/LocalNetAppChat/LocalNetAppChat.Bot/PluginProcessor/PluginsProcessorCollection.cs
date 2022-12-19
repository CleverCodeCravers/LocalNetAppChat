
using LocalNetAppChat.Bot.PluginProcessor;
namespace LocalNetAppChat.Bot

{
    public class PluginsProcessorCollection
    {
       private List<IPlugin> plugins = new List<IPlugin>();

        public void Add(IPlugin plugin)
        {
            plugins.Add(plugin);
        }

        public void ExecuteCommand(string command)
        {

            foreach (var plugin in plugins)
            {
               plugin.ExecuteCommand(command);
            }
        }
    }
}
