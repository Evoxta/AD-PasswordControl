namespace EvoxtaADReset
{
    public static class AppInfo
    {
        public const string Name = "Password Control";
        public const string Version = "1.0.1";
        public const string Developer = "Ethan from Evoxta";

        public const string LatestVersionURL =
            "https://evoxta.com/adreset/latest";

        public static string UpdateNotesURL(string version)
        {
            return $"https://evoxta.com/adreset/update-notes/{version}";
        }
        public static string UpdateFileURL(string version)
        {
            return $"https://evoxta.com/adreset/update-files/{version}/EvoxtaADreset.exe";
        }
    }
}
