namespace LocalNetAppChat.Domain

{
    public static class Util
    {
        public static string SanatizeFilename(string filename)
        {
            string result = filename;

            string invalidChars = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (char c in invalidChars)
            {
                result = result.Replace(c.ToString(), "");
            }

            return result;
        }
    }

}

