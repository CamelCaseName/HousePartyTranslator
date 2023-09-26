using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Translator.Core.Helpers
{
    /// <summary>
    /// A static class to log strings with a timestamp to a logfile located in the corresponding appdata folder
    /// </summary>
    public static class LogManager
    {
        private static readonly string APPDATA_PATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); // AppData Local folder (different for every user)
        public static readonly string CFGFOLDER_PATH = Path.Combine(APPDATA_PATH, "HousePartyTranslator");     // Path for program Settings folder
        private static readonly string CFGFILE_PATH = Path.Combine(CFGFOLDER_PATH, "log.txt");   // Path for Settings.txt file
        private static readonly List<string> FileLines = new();

        public enum Level
        {
            Info,
            Warning,
            Error,
            Crash
        }

        static LogManager()
        {
            // Does the folder not exist?
            if (!Directory.Exists(CFGFOLDER_PATH))
                _ = Directory.CreateDirectory(CFGFOLDER_PATH); // Create the folder

            // Does log.txt not exist?
            if (!File.Exists(CFGFILE_PATH))
                CreateLogFile();

            //get all lines from the log file so far
            FileLines.AddRange(File.ReadAllLines(CFGFILE_PATH));
        }

        public static void SaveLogFile()
        {
            //remove old lines after we reach a certain length of the file
            if (FileLines.Count > 5000)
            {
                for (int i = 0; i < FileLines.Count - 5000; i++)
                {
                    FileLines.RemoveAt(0);
                }
            }

            //write back file
            File.WriteAllLines(CFGFILE_PATH, FileLines);
        }

        /// <summary>
        /// A Method for adding a string with timestamp at the end of the log
        /// </summary>
        /// <param name="message">The string to be added.</param>
        public static void Log(string message, [CallerLineNumber] int line = 0, [CallerFilePath] string path = "") => Log(message, Level.Info, line, path);

        public static void Log(object message, [CallerLineNumber] int line = 0, [CallerFilePath] string path = "") => Log(message.ToString() ?? "message was null", Level.Info, line, path);

        [Conditional("DEBUG")]
        public static void LogDebug(string message, [CallerLineNumber] int line = 0, [CallerFilePath] string path = "") => Log(message, line, path);

        [Conditional("DEBUG")]
        public static void LogDebug(object message, [CallerLineNumber] int line = 0, [CallerFilePath] string path = "") => Log(message, line, path);

        /// <summary>
        /// A Method for adding a string with timestamp at the end of the log
        /// </summary>
        /// <param name="message">The string to be added.</param>
        /// <param name="level">The level of the logged message</param>
        public static void Log(string message, Level level, [CallerLineNumber] int line = 0, [CallerFilePath] string path = "")
        {
            string file = ExtractCSFileNameFromPath(path);
            string _message = $"[{level}] {DateTime.Now} | {file}:{line} | {message}";

#if !(DEBUG || DEBUG_USER)
            //add the message as lines to our list of all lines
            FileLines.AddRange(_message.Split('\n'));
#else
            //Debug.WriteLine(Environment.StackTrace);
            Debug.WriteLine(_message[^1] == '\n' ? _message[..^1] : _message);
#endif
        }

        public static string ExtractCSFileNameFromPath(string path)
        {
            string[] folders = path.Split('\\');
            if (folders.Length < 4) throw new ArgumentException("path is not deep enough", nameof(path));
            return folders[^3] == "HousePartyTranslator"
                        ? folders[^2][10..] + '\\' + folders[^1]
                        : folders[^4] == "HousePartyTranslator"
                        ? folders[^3][10..] + '\\' + folders[^2] + '\\' + folders[^1]
                        : folders[^4][10..] + '\\' + folders[^3] + '\\' + folders[^2] + '\\' + folders[^1];
        }

        public static void Log(object message, Level level, [CallerLineNumber] int line = 0, [CallerFilePath] string path = "")
        {
            Log(message.ToString() ?? "message was null", level, line, path);
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
