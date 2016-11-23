namespace Tim.DataAccess.Configuration
{
    public static class MessageLogConfiguration
    {
        public static string BaseLogFolder { get; private set; }

        public static string DataLogSubFolder { get; private set; }

        public static string ErrorLogSubfolder { get; private set; }

        public static string SdwhErrorLogSubFolder { get; private set; }



        public static void SetConfiguration(string baseLogFolder, string dataLogSubFolder, string errorLogSubfolder, string sdwhErrorLogSubFolder)
        {
            BaseLogFolder = baseLogFolder;
            DataLogSubFolder = dataLogSubFolder;
            ErrorLogSubfolder = errorLogSubfolder;
            SdwhErrorLogSubFolder = sdwhErrorLogSubFolder;
        }
    }
}

