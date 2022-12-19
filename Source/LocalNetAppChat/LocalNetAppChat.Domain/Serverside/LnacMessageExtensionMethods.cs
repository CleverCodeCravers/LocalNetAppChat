using LocalNetAppChat.Domain.Shared;

namespace LocalNetAppChat.Domain.Serverside
{
    public static class LnacMessageExtensionMethods
    {
        public static ReceivedMessage ToReceivedMessage(this LnacMessage message)
        {
            return new ReceivedMessage(-1, DateTime.MinValue, "", message);
        }
    }
}
