using System;
using System.Collections.Generic;
using System.IO;

namespace HousePartyTranslator
{
    class SettingsManager
    {
        private static readonly string APPDATA_PATH = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData); // AppData Local folder (different for every user)
        private static readonly string CFGFOLDER_PATH = Path.Combine(APPDATA_PATH, "House Party Translator");     // Path for program Settings folder
        private static readonly string CFGFILE_PATH = Path.Combine(CFGFOLDER_PATH, "Settings.txt");   // Path for Settings.txt file
        public static readonly char CFG_STR_DELIM = '='; // Settings file string delimiter
        public static SettingsManager main = new SettingsManager();
        public List<Setting> Settings = new List<Setting>();

        public SettingsManager()
        {
            if (main != null)
            {
                return;
            }
            main = this;
        }

        public static void LoadSettings()
        {
            // Does the Settings folder not exist?
            if (!Directory.Exists(CFGFOLDER_PATH))
                Directory.CreateDirectory(CFGFOLDER_PATH); // Create the Settings File Exmaple folder

            // Does Settings.txt not exist?
            if (!File.Exists(CFGFILE_PATH))
                CreateSettings();

            ReadSettings();
        }

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
        private static void ReadSettings()
        {
            StreamReader settingsReader = File.OpenText(CFGFILE_PATH);
            string settingLine;                     // String that holds the text read from Settings file
            string[] splitSettingArray; // String array that holds the split settingLine string
            List<string[]> splitSettingArrayList = new List<string[]>(); // List that holds all the cfgSettingArr objects

            // Read the Settings file until the end of the file
            for (int index = 0; !settingsReader.EndOfStream; index++)
            {
                settingLine = settingsReader.ReadLine();

                // Does the line contain text?
                if (!string.IsNullOrWhiteSpace(settingLine))
                {
                    // Split the read text into cfgSettingArr
                    splitSettingArray = settingLine.Split(new char[] { CFG_STR_DELIM }, StringSplitOptions.None);

                    // Add to the cfgList
                    splitSettingArrayList.Add(splitSettingArray);
                }
            }

            // Read all the settings in the cfgList
            foreach (string[] settingStringArray in splitSettingArrayList)
            {
                if (int.TryParse(settingStringArray[1], out int outputInt))
                {
                    main.Settings.Add(new IntegerSetting(settingStringArray[0], outputInt));
                }
                else if (bool.TryParse(settingStringArray[1], out bool outputBool))
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

        public void UpdateSettings()
        {
            StreamWriter cfgUpdater = new StreamWriter(CFGFILE_PATH);

            foreach (var setting in main.Settings)
                cfgUpdater.WriteLine(setting.ToString());

            cfgUpdater.Close();
        }
    }

}
