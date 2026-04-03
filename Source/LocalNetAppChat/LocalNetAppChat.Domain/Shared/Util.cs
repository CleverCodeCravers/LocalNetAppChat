namespace LocalNetAppChat.Domain.Shared
{
    public static class Util
    {
        public static string SanitizeFilename(string filename)
        {
            // Handle both / and \ as separators regardless of platform
            var lastSlash = filename.LastIndexOfAny(new[] { '/', '\\' });
            return lastSlash >= 0 ? filename[(lastSlash + 1)..] : filename;
        }

        [Obsolete("Use SanitizeFilename instead")]
        public static string SanatizeFilename(string filename)
        {
            return SanitizeFilename(filename);
        }
    }
}

