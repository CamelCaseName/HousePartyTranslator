using System;
using System.IO;

namespace HousePartyTranslator.Managers
{
    /// <summary>
    /// A static class to log strings with a timestamp to a logfile located in the corresponding appdata folder
    /// </summary>
    static class LogManager
    {
        private static readonly string APPDATA_PATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); // AppData Local folder (different for every user)
        private static readonly string CFGFOLDER_PATH = Path.Combine(APPDATA_PATH, "House Party Translator");     // Path for program Settings folder
        private static readonly string CFGFILE_PATH = Path.Combine(CFGFOLDER_PATH, "log.txt");   // Path for Settings.txt file

        /// <summary>
        /// A Method for adding a string with timestamp at the end of the log
        /// </summary>
        /// <param name="EventString">The string to be added.</param>
        public static void LogEvent(string EventString)
        {
            // Does the folder not exist?
            if (!Directory.Exists(CFGFOLDER_PATH))
                Directory.CreateDirectory(CFGFOLDER_PATH); // Create the folder

            // Does log.txt not exist?
            if (!File.Exists(CFGFILE_PATH))
                CreateLogFile();

            string time = DateTime.Now.ToString();
            string message = $"Logged event at {time}: \n {EventString} \n\n";
            File.AppendAllText(CFGFILE_PATH, message);
        }

        /// <summary>
        /// Creates the logfile
        /// </summary>
        private static void CreateLogFile()
        {
            StreamWriter LogWriter = File.CreateText(CFGFILE_PATH);

            LogWriter.WriteLine();

            LogWriter.Close();
        }
    }
}
