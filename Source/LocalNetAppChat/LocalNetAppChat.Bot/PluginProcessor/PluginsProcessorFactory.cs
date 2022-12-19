using LocalNetAppChat.Bot;
using LocalNetAppChat.Bot.PluginProcessor.Plugins;

namespace LocalNetAppChat.Bot

{
    public static class PluginsProcessorFactory
    {
        public static PluginsProcessorCollection Get()
        {
            var result = new PluginsProcessorCollection();

            result.Add(new PowerShellPlugin());

            return result;
        }
    }
}
