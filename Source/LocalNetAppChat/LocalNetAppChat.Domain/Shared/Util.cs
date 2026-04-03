namespace LocalNetAppChat.Domain.Shared
{
    public static class Util
    {
        public static string SanitizeFilename(string filename)
        {
            return Path.GetFileName(filename);
        }

        [Obsolete("Use SanitizeFilename instead")]
        public static string SanatizeFilename(string filename)
        {
            return SanitizeFilename(filename);
        }
    }
}

