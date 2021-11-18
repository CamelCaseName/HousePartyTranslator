using System;
using System.Collections.Generic;
using System.IO;

namespace HousePartyTranslator
{
    /// <summary>
    /// A class to create, load and manage settings in a file in the appdata folder.
    /// </summary>
    class SettingsManager
    {
        private static readonly string APPDATA_PATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); // AppData Local folder (different for every user)
        private static readonly string CFGFOLDER_PATH = Path.Combine(APPDATA_PATH, "House Party Translator");     // Path for program Settings folder
        private static readonly string CFGFILE_PATH = Path.Combine(CFGFOLDER_PATH, "Settings.txt");   // Path for Settings.txt file
        public static readonly char CFG_STR_DELIM = '='; // Settings file string delimiter
        public static SettingsManager main = new SettingsManager();
        public List<Setting> Settings = new List<Setting>();

        /// <summary>
        /// The constructor for a semi static instance
        /// </summary>
        public SettingsManager()
        {
            if (main != null)
            {
                return;
            }
            main = this;
        }

        /// <summary>
        /// Call this method to initialize the settings and begin loading them. Will create the file if missing.
        /// </summary>
        public static void LoadSettings()
        {
            // Does the Settings folder not exist?
            if (!Directory.Exists(CFGFOLDER_PATH))
                Directory.CreateDirectory(CFGFOLDER_PATH); // Create the Settings folder

            // Does Settings.txt not exist?
            if (!File.Exists(CFGFILE_PATH))
                CreateSettings();

            ReadSettings();
        }

        /// <summary>
        /// Creates the settings file and populates it with all settings we need
        /// </summary>
        private static void CreateSettings()
        {
            StreamWriter settingsWriter = File.CreateText(CFGFILE_PATH);

            List<Setting> defaultSettings = new List<Setting>() {
                new StringSetting("version", ""),
                new StringSetting("dbPassword", ""),
                new StringSetting("language", ""),
                new BooleanSetting("NewTemplatesNeeded", false)};

            foreach (Setting setting in defaultSettings)
            {
                settingsWriter.WriteLine(setting.ToString());
            }

            settingsWriter.Close();
        }

        /// <summary>
        /// Reads in the settings from the settings file and populate the list of settings with the correct objects
        /// </summary>
        private static void ReadSettings()
        {
            StreamReader settingsReader = File.OpenText(CFGFILE_PATH);
            string settingLine;                     // String that holds the text read from Settings file
            string[] splitSettingArray; // String array that holds the split settingLine string
            List<string[]> splitSettingArrayList = new List<string[]>(); // List that holds all the strings

            // Read the Settings file until the end of the file
            for (int index = 0; !settingsReader.EndOfStream; index++)
            {
                settingLine = settingsReader.ReadLine();

                // Does the line contain text?
                if (!string.IsNullOrWhiteSpace(settingLine))
                {
                    // Split the read text into the setting string array
                    splitSettingArray = settingLine.Split(new char[] { CFG_STR_DELIM }, StringSplitOptions.None);

                    // Add to the list
                    splitSettingArrayList.Add(splitSettingArray);
                }
            }

            // Read all the settings in the cfgList
            foreach (string[] settingStringArray in splitSettingArrayList)
            {
                //try and find the corresponding type of setting.
                if (bool.TryParse(settingStringArray[1], out bool outputBool))
                {
                    main.Settings.Add(new BooleanSetting(settingStringArray[0], outputBool));
                }
                else if (float.TryParse(settingStringArray[1], out float outputFloat))
                {
                    main.Settings.Add(new FloatSetting(settingStringArray[0], outputFloat));
                }
                else if (settingStringArray.Length > 2)
                {
                    main.Settings.Add(new CustomSetting(settingStringArray[0], settingStringArray[1]));
                }
                else
                {
                    main.Settings.Add(new StringSetting(settingStringArray[0], settingStringArray[1]));
                }
            }
            settingsReader.Close();
        }

        /// <summary>
        /// Updates the settings to the file on disk. AKA saving the settings.
        /// </summary>
        public void UpdateSettings()
        {
            StreamWriter cfgUpdater = new StreamWriter(CFGFILE_PATH);

            //write all settings as strings to file
            foreach (var setting in main.Settings)
                cfgUpdater.WriteLine(setting.ToString());

            cfgUpdater.Close();
        }
    }

}
