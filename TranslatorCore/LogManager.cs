﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Translator.Core
{
    /// <summary>
    /// A static class to log strings with a timestamp to a logfile located in the corresponding appdata folder
    /// </summary>
    public static class LogManager
    {
        private static readonly string APPDATA_PATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); // AppData Local folder (different for every user)
        public static readonly string CFGFOLDER_PATH = Path.Combine(APPDATA_PATH, "HousePartyTranslator");     // Path for program Settings folder
        private static readonly string CFGFILE_PATH = Path.Combine(CFGFOLDER_PATH, "log.txt");   // Path for Settings.txt file
        private static readonly List<string> FileLines = new List<string>();

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
        public static void Log(string message)
        {
            Log(message, Level.Info);
        }

        /// <summary>
        /// A Method for adding a string with timestamp at the end of the log
        /// </summary>
        /// <param name="message">The string to be added.</param>
        /// <param name="level">The level of the logged message</param>
        public static void Log(string message, Level level)
        {
            string _message = $"[{level}] | {DateTime.Now} | {message} \n";

            //add the message as lines to our list of all lines
            FileLines.AddRange(_message.Split('\n'));
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