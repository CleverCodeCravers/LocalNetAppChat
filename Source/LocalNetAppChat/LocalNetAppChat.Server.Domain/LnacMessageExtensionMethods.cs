using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Server.Domain
{
    public static class LnacMessageExtensionMethods
    {
        public static ReceivedMessage ToReceivedMessage(this LnacMessage message)
        {
            return new ReceivedMessage(-1, DateTime.MinValue, "", message);
        }
    }
}
