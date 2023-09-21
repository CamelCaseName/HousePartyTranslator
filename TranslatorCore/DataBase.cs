using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Translator.Core.Data;
using Translator.Core.Helpers;
using Translator.Core.UICompatibilityLayer;

namespace Translator.Core
{
    /// <summary>
    /// A static class to interface with the database running on https://www.rinderha.cc for use with the Translation Helper for the game House Party.
    /// </summary>
    public static class DataBase
    {
        private static IUIHandler? UI;
        public static string AppTitle { get; private set; } = string.Empty;
        public static string DBVersion { get; private set; } = "0.00";
        public static bool IsOnline { get; private set; } = false;

#if DEBUG || DEBUG_ADMIN
        private static readonly string FROM = " FROM debug";
        private static readonly string INSERT = " INSERT INTO debug";
        private static readonly string UPDATE = " UPDATE debug";
#else
        private static readonly string FROM = " FROM translations";
        private static readonly string INSERT = " INSERT INTO translations";
        private static readonly string UPDATE = " UPDATE translations";
#endif
        private static readonly Queue<MySqlCommand> OfflineToDo = new();

        public static bool GetAllFilesAndStories(out HashSet<string> stories, out HashSet<string> files)
        {
            UI!.SignalUserWait();
            string command = "SELECT DISTINCT story, filename"
                + FROM
                + " WHERE deleted = 0 AND language IS NULL"
                + " ORDER BY story ASC";

            using MySqlConnection connection = new(GetConnString());
            stories = new(Utils.OfficialStories);
            files = new(Utils.OfficialFileNames);

            if (CheckOrReopenConnection(connection))
            {
                using MySqlCommand cmd = connection.CreateCommand();

                cmd.CommandText = command;
                cmd.Parameters.Clear();
                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    string story;
                    string file;
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            story = reader.GetString(0);
                            if (story != string.Empty)
                                _ = stories.Add(story);
                        }
                        if (!reader.IsDBNull(1))
                        {
                            file = reader.GetString(1);
                            if (file != string.Empty)
                                _ = files.Add(file);
                        }
                    }
                }
                else
                {
                    _ = UI.WarningOk("No files found in the database??", "Info");
                    LogManager.Log("No files found in the database at all??");
                }
                reader.Close();
            }
            UI.SignalUserEndWait();

            return files.Count > 0 || stories.Count > 0;
        }

        public static bool GetAllFilesAndStories(out string[] stories, out string[] files)
        {
            if (GetAllFilesAndStories(out HashSet<string> _stories, out HashSet<string> _files))
            {
                stories = new string[_stories.Count];
                _stories.CopyTo(stories);
                files = new string[_files.Count];
                _files.CopyTo(files);
                return true;
            }
            else
            {
                stories = Array.Empty<string>();
                files = Array.Empty<string>();
                return false;
            }
        }

        public static bool GetAllFilesAndStoriesSorted(out HashSet<string> stories, out Dictionary<string, List<string>> files)
        {
            UI!.SignalUserWait();
            string command = "SELECT DISTINCT story, filename"
                + FROM
                + " WHERE deleted = 0 AND language IS NULL"
                + " ORDER BY story ASC";

            using MySqlConnection connection = new(GetConnString());
            stories = new();
            files = new();

            if (CheckOrReopenConnection(connection))
            {
                using MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = command;
                cmd.Parameters.Clear();
                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    string story;
                    string file;
                    while (reader.Read())
                    {
                        if (reader.IsDBNull(0) || reader.IsDBNull(1))
                            continue;

                        story = reader.GetString(0);
                        file = reader.GetString(1);

                        if (file == string.Empty || story == string.Empty)
                            continue;

                        _ = stories.Add(story);
                        if (!files.ContainsKey(story))
                            files.Add(story, new List<string> { file });
                        else
                            files[story].Add(file);
                    }
                }
                else
                {
                    _ = UI.WarningOk("No files found in the database??", "Info");
                    LogManager.Log("No files found in the database at all??");
                }
                reader.Close();
            }
            UI.SignalUserEndWait();

            return files.Count > 0 || stories.Count > 0;
        }

        public static bool GetAllLineData(string fileName, string story, out FileData LineDataList, string language)
        {
            bool wasSuccessfull = false;
            string command = "SELECT *"
                    + FROM
                    + " WHERE filename = @filename AND story = @story AND language = @language AND deleted = 0"
                    + " ORDER BY category ASC;";

            using MySqlConnection connection = new(GetConnString());

            LineDataList = new FileData(story, fileName);

            if (CheckOrReopenConnection(connection))
            {
                using MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = command;
                cmd.Parameters.Clear();

                _ = cmd.Parameters.AddWithValue("@language", language);
                _ = cmd.Parameters.AddWithValue("@filename", fileName);
                _ = cmd.Parameters.AddWithValue("@story", story);
                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0) & !reader.IsDBNull(9))
                        {
                            string id = CleanId(reader.GetString("id"), story, fileName, false);
                            var _lineData = new LineData()
                            {
                                Category = (StringCategory)reader.GetInt32("category"),
                                Comments = !reader.IsDBNull(7) ? reader.GetString("comment").Split('#') : Array.Empty<string>(),
                                FileName = fileName,
                                ID = id,
                                IsApproved = reader.GetInt32("approved") > 0,
                                IsTemplate = false,
                                IsTranslated = reader.GetInt32("translated") > 0,
                                Story = story,
                                TemplateString = string.Empty,
                                TranslationString = reader.GetString("translation").RemoveVAHints()
                            };
                            if (LineDataList.ContainsKey(id))
                            {
                                LineDataList[id] = _lineData;
                            }
                            else
                            {
                                LineDataList.Add(id, _lineData);
                            }
                        }
                    }
                    wasSuccessfull = true;
                }
                else
                {
                    _ = UI!.WarningOk("No translations approved so far", "Potential issue");
                }
                reader.Close();
            }
            return wasSuccessfull;
        }

        /// <summary>
        /// Returns a list of all ids and categorys for the given file in a list of LineData.
        /// </summary>
        /// <param name="fileName">The name of the file read from without the extension.</param>
        /// <param name="story">The name of the story the file is from, should be the name of the parent folder.</param>
        /// <param name="LineDataList">A list of all ids and categorys in LineData objects.</param>
        /// <returns>
        /// True if ids are found for this file.
        /// </returns>
        public static bool GetAllLineDataTemplate(string fileName, string story, out FileData LineDataList)
        {
            string command = story == "Hints"
                ? "SELECT id, category, english"
                    + FROM
                    + " WHERE story = @story AND language IS NULL AND deleted = 0"
                    + " ORDER BY category ASC;"
                : "SELECT id, category, english"
                    + FROM
                    + " WHERE filename = @filename AND story = @story AND language IS NULL AND deleted = 0"
                    + " ORDER BY category ASC;";
            using MySqlConnection connection = new(GetConnString());
            LineDataList = new FileData(story, fileName);

            if (CheckOrReopenConnection(connection))
            {
                using MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = command;
                cmd.Parameters.Clear();

                if (story != "Hints")
                    _ = cmd.Parameters.AddWithValue("@filename", fileName);
                _ = cmd.Parameters.AddWithValue("@story", story);

                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0) && !reader.IsDBNull(2))
                            LineDataList.Add(CleanId(reader.GetString("id"), story, fileName, true),
                                new LineData(
                                    CleanId(reader.GetString("id"), story, fileName, true),
                                    story,
                                    fileName,
                                    !reader.IsDBNull(1) ? (StringCategory)reader.GetInt32("category") : StringCategory.General,
                                    reader.GetString("english").Trim(),
                                    true));
                    }
                }
                else
                {
                    _ = UI?.WarningOk("Ids can't be loaded", "Info");
                    LogManager.Log("No template ids found for " + story + "/" + fileName);
                }
                reader.Close();
            }
            return LineDataList.Count > 0;
        }

        public static bool GetFilesForStory(string story, out string[] names)
        {
            UI!.SignalUserWait();
            string command = "SELECT DISTINCT filename"
                + FROM
                + " WHERE story = @story AND language IS NULL AND deleted = 0"
                + " ORDER BY filename ASC";

            using MySqlConnection connection = new(GetConnString());
            List<string> files = new();

            if (CheckOrReopenConnection(connection))
            {
                using MySqlCommand cmd = connection.CreateCommand();

                cmd.CommandText = command;
                cmd.Parameters.Clear();
                _ = cmd.Parameters.AddWithValue("@story", story);
                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    string file;
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            file = reader.GetString(0);
                            if (file != string.Empty)
                                files.Add(file);
                        }
                    }
                }
                else
                {
                    _ = UI.WarningOk("No stories found for the story " + story, "Info");
                    LogManager.Log("No stories found for the story " + story);
                }
                reader.Close();
            }
            UI.SignalUserEndWait();
            names = files.ToArray();
            return files.Count > 0;
        }

        public static bool GetLanguagesForFile(string story, string file, out string[] languages)
        {
            UI!.SignalUserWait();
            string command = "SELECT DISTINCT language"
                + FROM
                + " WHERE story = @story AND filename = @file AND deleted = 0 AND language IS NOT NULL"
                + " ORDER BY language ASC";

            using MySqlConnection connection = new(GetConnString());
            List<string> _languages = new();

            if (CheckOrReopenConnection(connection))
            {
                using MySqlCommand cmd = connection.CreateCommand();

                cmd.CommandText = command;
                cmd.Parameters.Clear();
                _ = cmd.Parameters.AddWithValue("@story", story);
                _ = cmd.Parameters.AddWithValue("@file", file);
                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    string lang;
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            lang = reader.GetString(0);
                            if (lang != string.Empty)
                                _languages.Add(lang);
                        }
                    }
                }
                else
                {
                    _ = UI.WarningOk("No languages found for " + story + " /" + file, "Info");
                    LogManager.Log("No languages found for " + story + " /" + file);
                }
                reader.Close();
            }
            UI.SignalUserEndWait();
            languages = _languages.ToArray();
            return languages.Length > 0;
        }

        public static bool GetLanguagesForStory(string story, out string[] languages)
        {
            UI!.SignalUserWait();
            string command = "SELECT DISTINCT language"
                + FROM
                + " WHERE story = @story AND deleted = 0 AND language IS NOT NULL"
                + " ORDER BY language ASC";

            using MySqlConnection connection = new(GetConnString());
            List<string> _languages = new();

            if (CheckOrReopenConnection(connection))
            {
                using MySqlCommand cmd = connection.CreateCommand();

                cmd.CommandText = command;
                cmd.Parameters.Clear();
                _ = cmd.Parameters.AddWithValue("@story", story);
                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    string lang;
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            lang = reader.GetString(0);
                            if (lang != string.Empty)
                                _languages.Add(lang);
                        }
                    }
                }
                else
                {
                    _ = UI.WarningOk("No languages found for " + story, "Info");
                    LogManager.Log("No languages found for " + story);
                }
                reader.Close();
            }
            UI.SignalUserEndWait();
            languages = _languages.ToArray();
            return languages.Length > 0;
        }

        public static bool GetLineData(string id, string fileName, string story, out LineData translation, string language)
        {
            string command = "SELECT *"
                + FROM
                + " WHERE id = @id AND language = @language AND deleted = 0;";
            bool wasSuccessfull = false;
            using MySqlConnection connection = new(GetConnString());

            if (CheckOrReopenConnection(connection))
            {
                using MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = command;
                cmd.Parameters.Clear();
                _ = cmd.Parameters.AddWithValue("@id", story + fileName + id + language);
                _ = cmd.Parameters.AddWithValue("@language", language);
                using MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows && !reader.IsDBNull(0))
                {
                    translation = new LineData()
                    {
                        Category = (StringCategory)reader.GetInt32("category"),
                        Comments = !reader.IsDBNull(7) ? reader.GetString("comment").Split('#') : Array.Empty<string>(),
                        FileName = fileName,
                        ID = CleanId(id, story, fileName, false),
                        IsApproved = reader.GetInt32("approved") > 0,
                        IsTemplate = false,
                        IsTranslated = reader.GetInt32("translated") > 0,
                        Story = story,
                        TemplateString = string.Empty,
                        TranslationString = reader.GetString("translation")
                    };
                    wasSuccessfull = true;
                }
                else
                {
                    translation = new LineData(id, story, fileName, StringCategory.General);
                }
                reader.Close();
            }
            else
            {
                translation = new LineData(id, story, fileName, StringCategory.General);
            }
            return wasSuccessfull;
        }

        public static bool GetStoriesInTranslation(string language, out string[] names)
        {
            UI!.SignalUserWait();
            string command = "SELECT DISTINCT story"
                + FROM
                + " WHERE language = @language AND deleted = 0"
                + " ORDER BY story ASC";

            using MySqlConnection connection = new(GetConnString());
            List<string> stories = new();

            if (CheckOrReopenConnection(connection))
            {
                using MySqlCommand cmd = connection.CreateCommand();

                cmd.CommandText = command;
                cmd.Parameters.Clear();
                _ = cmd.Parameters.AddWithValue("@language", language);
                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    string story;
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            story = reader.GetString(0);
                            if (story != string.Empty)
                                stories.Add(story);
                        }
                    }
                }
                else
                {
                    _ = UI.WarningOk("No stories found in " + language, "Info");
                    LogManager.Log("No stories found in " + language);
                }
                reader.Close();
            }
            UI.SignalUserEndWait();
            names = stories.ToArray();
            return stories.Count > 0;
        }

        public static bool GetStoriesWithTemplates(out string[] names)
        {
            UI!.SignalUserWait();
            string command = "SELECT DISTINCT story"
                + FROM
                + " WHERE language IS NULL AND deleted = 0"
                + " ORDER BY story ASC";

            using MySqlConnection connection = new(GetConnString());
            List<string> stories = new();

            if (CheckOrReopenConnection(connection))
            {
                using MySqlCommand cmd = connection.CreateCommand();

                cmd.CommandText = command;
                cmd.Parameters.Clear();
                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    string story;
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0))
                        {
                            story = reader.GetString(0);
                            if (story != string.Empty)
                                stories.Add(story);
                        }
                    }
                }
                else
                {
                    _ = UI.WarningOk("No story templates found?", "Info");
                    LogManager.Log("No story templates found?");
                }
                reader.Close();
            }
            UI.SignalUserEndWait();
            names = stories.ToArray();
            return stories.Count > 0;
        }

        /// <summary>
        /// Needs to be called in order to use the class, checks the connection and displays the current version information in the window title.
        /// </summary>
        public static void Initialize(IUIHandler uIHandler, string AppVersion)
        {
            UI = uIHandler ?? throw new ArgumentNullException(nameof(uIHandler));
            //establish connection and handle password
            EstablishConnection();

            if (!IsOnline)
            {
#if DEBUG || DEBUG_ADMIN
                AppTitle = "Translator (DEBUG) (File Version: " + Settings.Default.FileVersion + ", DB Version: *Offline*, Application version: " + AppVersion + ")";
#else
                AppTitle = "Translator (File Version: " + Settings.Default.FileVersion + ", DB Version: *Offline*, Application version: " + AppVersion + ")";
#endif
            }
            else
            {
                using MySqlConnection connection = new(GetConnString());
                if (connection.State != System.Data.ConnectionState.Open) connection.Open();
                using var cmd = new MySqlCommand(string.Empty, connection);
                _ = CheckOrReopenConnection(connection);
                //Console.WriteLine("DB opened");

                //checking template version
                var getVersion = new MySqlCommand(
                    "SELECT story"
                    + FROM
                    + " WHERE ID = \"version\";",
                    connection);
                using MySqlDataReader reader = getVersion.ExecuteReader();
                _ = reader.Read();
                DBVersion = reader.GetString(0);
                reader.Close();

                string fileVersion = Settings.Default.FileVersion;
                if (fileVersion == string.Empty)
                {
                    // get software version from db
                    Settings.Default.FileVersion = DBVersion;
                }
                else
                {
                    //add . if it is missing
                    if (!fileVersion.Contains('.'))
                    {
                        fileVersion = "1." + fileVersion;
                        Settings.Default.FileVersion = fileVersion;
                    }
                    //try casting wi9th invariant culture, log and try and work around it if it fails
                    if (!double.TryParse(DBVersion, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double _dbVersion))
                    {
                        _dbVersion = 1.0;
                        LogManager.Log($"invalid string cast to double. Offender: {DBVersion}", LogManager.Level.Warning);
                    }
                    if (!double.TryParse(fileVersion, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double _fileVersion))
                    {
                        _fileVersion = 1.0;
                        LogManager.Log($"invalid string cast to double. Offender: {fileVersion}", LogManager.Level.Warning);
                    }

                    //save comparison
                    if (_dbVersion > _fileVersion)
                    {
                        //update local software version from db
                        Settings.Default.FileVersion = DBVersion;
                    }
                }
                Settings.Default.Save();

                //set global variable for later actions
                TranslationManager.IsUpToDate = DBVersion == Settings.Default.FileVersion;
                if (!TranslationManager.IsUpToDate && Settings.Default.AdvancedModeEnabled)
                {
                    _ = UI.WarningOk($"Current software version({Settings.Default.FileVersion}) and data version({DBVersion}) differ. " + "You may acquire the latest version of this program. " + "If you know that you have newer strings, you may select the template stories to upload the new versions!", "Updating string database");
                }
#if DEBUG || DEBUG_ADMIN
                AppTitle = "Translator (DEBUG) (File Version: " + Settings.Default.FileVersion + ", DB Version: " + DBVersion + ", Application version: " + AppVersion + ")";
#else
                AppTitle = "Translator (File Version: " + Settings.Default.FileVersion + ", DB Version: " + DBVersion + ", Application version: " + AppVersion + ")";
#endif
            }
        }

        public static bool RemoveOldTemplates(string fileName, string story)
        {
            string command = UPDATE
                + " SET deleted = 1"
                + " WHERE filename = @filename AND story = @story AND SUBSTRING(id, -8) = @templateid";
            using MySqlConnection connection = new(GetConnString());
            using MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = command;
            cmd.Parameters.Clear();
            _ = cmd.Parameters.AddWithValue("@story", story);
            _ = cmd.Parameters.AddWithValue("@fileName", fileName);
            _ = cmd.Parameters.AddWithValue("@templateid", "template");

            //return if at least one row was changed
            return ExecuteOrReOpen(cmd);
        }

        public static bool RemoveOldTemplates(string story)
        {
            string command = UPDATE
                + " SET deleted = 1"
                + " WHERE story = @story AND SUBSTRING(id, -8) = @templateid";
            using MySqlConnection connection = new(GetConnString());
            using MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = command;
            cmd.Parameters.Clear();
            _ = cmd.Parameters.AddWithValue("@story", story);
            _ = cmd.Parameters.AddWithValue("@templateid", "template");

            //return if at least one row was changed
            return ExecuteOrReOpen(cmd);
        }

        /// <summary>
        /// Increases the verison count on the database by 0.01, eg: 0.19 -> 0.20
        /// </summary>
        /// <returns>
        /// Returns true if the update worked.
        /// </returns>
        public static bool UpdateDBVersion()
        {
            string command = UPDATE
                + " SET story = @story"
                + " WHERE ID = @version;";
            using MySqlConnection connection = new(GetConnString());
            using MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = command;
            cmd.Parameters.Clear();
            _ = cmd.Parameters.AddWithValue("@story", Settings.Default.FileVersion);
            _ = cmd.Parameters.AddWithValue("@version", "version");

            //return if at least ione row was changed
            return ExecuteOrReOpen(cmd);
        }

        /// <summary>
        /// Sets the translation of a string in the database in the given story.
        /// </summary>
        /// <param name="lineData">LineData with the templates to update<param>
        /// <param name="language">The language of the translation in ISO 639-1 notation.</param>
        /// <returns> True if at least one row was set, false if it was not the case.</returns>
        public static bool UpdateTranslation(LineData lineData, string language)
        {
            string comment = string.Empty;
            for (int j = 0; j < lineData.Comments?.Length; j++)
            {
                comment += lineData.Comments[j] + "#";
            }
            string command = INSERT
                + " (id, story, filename, category, translated, approved, language, comment, translation, deleted)"
                + " VALUES(@id, @story, @filename, @category, @translated, @approved, @language, @comment, @translation, @deleted)"
                + " ON DUPLICATE KEY UPDATE translation = @translation;";
            using MySqlConnection connection = new(GetConnString());
            using MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = command;
            cmd.Parameters.Clear();
            _ = cmd.Parameters.AddWithValue("@id", lineData.Story + lineData.FileName + lineData.ID + language);
            _ = cmd.Parameters.AddWithValue("@story", lineData.Story);
            _ = cmd.Parameters.AddWithValue("@fileName", lineData.FileName);
            _ = cmd.Parameters.AddWithValue("@category", (int)lineData.Category);
            _ = cmd.Parameters.AddWithValue("@translated", 1);
            _ = cmd.Parameters.AddWithValue("@approved", lineData.IsApproved ? 1 : 0);
            _ = cmd.Parameters.AddWithValue("@language", language);
            _ = cmd.Parameters.AddWithValue($"@comment", comment);
            _ = cmd.Parameters.AddWithValue("@translation", lineData.TranslationString.RemoveVAHints());
            _ = cmd.Parameters.AddWithValue($"@deleted", 0);

            return ExecuteOrReOpen(cmd);
        }

        /// <summary>
        /// Updates all translated strings for the selected file
        /// </summary>
        /// <param name="translationData">A list of all loaded templates for this file</param>
        /// <param name="language">The language of the translation in ISO 639-1 notation.</param>
        /// <returns></returns>
        public static bool UpdateTranslations(FileData translationData, string language)
        {
            //copy so we save, regardless of what happens to the translationmamanger that called the save
            FileData updateData = new(translationData);
            if (updateData.Count > 0)
            {
                int c = 0;
                using MySqlConnection connection = new(GetConnString());
                string storyName = updateData.ElementAt(0).Value.Story;
                string fileName = updateData.ElementAt(0).Value.FileName;
                for (int x = 0; x <= ((updateData.Count / 400) + 0.5); x++)
                {
                    var builder = new StringBuilder(
                        INSERT
                        + " (id,  story, filename, category, translated, approved, language, comment, translation, deleted) VALUES ",
                        updateData.Count * 100);

                    //add all values
                    int v = c;
                    for (int j = 0; j < 400; j++)
                    {
                        _ = builder.Append($"(@id{v}, @story{v}, @filename{v}, @category{v}, @translated{v}, @approved{v}, @language{v}, @comment{v}, @translation{v}, @deleted{v}),");

                        v++;
                        if (v >= updateData.Values.Count) break;
                    }

                    _ = builder.Remove(builder.Length - 1, 1);
                    using MySqlCommand cmd = connection.CreateCommand();
                    cmd.CommandText = builder.ToString() + " ON DUPLICATE KEY UPDATE translation = VALUES(translation), comment = VALUES(comment), approved = VALUES(approved), story = VALUES(story), category = VALUES(category), filename = VALUES(filename)";
                    cmd.Parameters.Clear();

                    if (c < updateData.Count)
                    {
                        //insert all the parameters
                        for (int k = 0; k < 400; k++)
                        {
                            LineData item = updateData.Values.ElementAt(c);
                            string comment = string.Empty;
                            for (int j = 0; j < item.Comments?.Length; j++)
                            {
                                if (item.Comments[j].Length > 1)
                                    comment += item.Comments[j] + "#";
                            }

                            _ = cmd.Parameters.AddWithValue($"@id{c}", storyName + fileName + item.ID + language);
                            _ = cmd.Parameters.AddWithValue($"@story{c}", storyName);
                            _ = cmd.Parameters.AddWithValue($"@filename{c}", fileName);
                            _ = cmd.Parameters.AddWithValue($"@category{c}", (int)item.Category);
                            _ = cmd.Parameters.AddWithValue($"@translated{c}", 1);
                            _ = cmd.Parameters.AddWithValue($"@approved{c}", item.IsApproved ? 1 : 0);
                            _ = cmd.Parameters.AddWithValue($"@language{c}", language);
                            _ = cmd.Parameters.AddWithValue($"@comment{c}", comment);
                            _ = cmd.Parameters.AddWithValue($"@translation{c}", item.TranslationString.RemoveVAHints());
                            _ = cmd.Parameters.AddWithValue($"@deleted{c}", 0);
                            ++c;
                            if (c >= updateData.Count)
                                break;
                        }

                        _ = ExecuteOrReOpen(cmd);
                    }
                }

                //return if at least ione row was changed
                return true;
            }
            return false;
        }

        /// <summary>
        /// Set the english template for string in the database.
        /// </summary>
        /// <param name="templates">FileData object with the relevant info</param>
        /// <returns>
        /// True if exactly one row was set, false if it was not the case.
        /// </returns>
        public static bool UpdateTemplates(FileData templates)
        {
            using MySqlConnection connection = new(GetConnString());
            using MySqlCommand cmd = connection.CreateCommand();

            //Save old template
            _ = GetAllLineDataTemplate(templates.FileName, templates.StoryName, out FileData? oldTemplates);

            bool result = RemoveOldTemplates(templates.FileName, templates.StoryName);

            //upload new
            UploadAllTemplates(templates, cmd);

            //generate "diff" and unapprove templates where the template changed
            List<string> idsToUnapprove = new();
            FileData diff = new(templates.StoryName, templates.FileName);
            foreach (KeyValuePair<string, LineData> newTemplate in templates)
            {
                if (!oldTemplates.TryGetValue(newTemplate.Key, out LineData? oldTemplateLine))
                {
                    diff.Add(newTemplate.Key, newTemplate.Value);
                    continue;
                }

                if (oldTemplateLine.TemplateString != newTemplate.Value.TemplateString)
                {
                    diff.Add(newTemplate.Key, newTemplate.Value);
                    idsToUnapprove.Add(oldTemplateLine.ID);
                }
            }
            if (idsToUnapprove.Count == 0) return result;

            SaveAndExportManager.SaveTemplateDiff(diff);

            int x = 0;
            var command = new StringBuilder(UPDATE
                + " SET approved = 0"
                + " WHERE story = @story AND filename = @filename AND language IS NOT NULL AND (");

            cmd.Parameters.Clear();
            foreach (string id in idsToUnapprove)
            {
                command.Append($"SUBSTR(id, 1, LENGTH(id) - LENGTH(language)) = @id{x} OR");
                x++;
            }
            command.Length -= 3;
            command.Append(");");

            cmd.CommandText = command.ToString();
            _ = cmd.Parameters.AddWithValue("@story", templates.StoryName);
            _ = cmd.Parameters.AddWithValue("@fileName", templates.FileName);
            x = 0;
            foreach (string id in idsToUnapprove)
            {
                _ = cmd.Parameters.AddWithValue($"@id{x}", templates.StoryName + templates.FileName + id);
                x++;
            }

            return ExecuteOrReOpen(cmd);
        }

        private static void UploadAllTemplates(FileData lines, MySqlCommand cmd)
        {
            int c = 0;
            for (int x = 0; x < ((lines.Count / 400) + 0.5); x++)
            {
                var builder = new StringBuilder(
                    INSERT
                    + " (id, story, filename, category, english, deleted) VALUES ",
                    lines.Count * 100);

                //add all values
                int v = c;
                for (int j = 0; j < 400; j++)
                {
                    if (v >= lines.Values.Count) break;
                    _ = builder.Append($"(@id{v}, @story{v}, @fileName{v}, @category{v}, @english{v}, @deleted{v}),");
                    v++;
                }

                //we can exit if we sent everything, this an occurr if the story has exactly a multiple of 400 entries
                if (v == c) break;

                _ = builder.Remove(builder.Length - 1, 1);

                cmd.CommandText = builder.ToString() + " ON DUPLICATE KEY UPDATE english = VALUES(english), deleted = VALUES(deleted);";
                cmd.Parameters.Clear();

                //insert all the parameters
                for (int k = 0; k < 400; k++)
                {
                    if (c >= lines.Values.Count) break;
                    LineData line = lines.Values.ElementAt(c);
                    _ = cmd.Parameters.AddWithValue($"@id{c}", line.Story + line.FileName + line.ID + "template");
                    _ = cmd.Parameters.AddWithValue($"@story{c}", line.Story);
                    _ = cmd.Parameters.AddWithValue($"@fileName{c}", line.FileName);
                    _ = cmd.Parameters.AddWithValue($"@category{c}", (int)line.Category);
                    _ = cmd.Parameters.AddWithValue($"@english{c}", line.TemplateString.Trim());
                    _ = cmd.Parameters.AddWithValue($"@deleted{c}", 0);
                    ++c;
                }

                _ = ExecuteOrReOpen(cmd);
            }
        }

        /// <summary>
        /// Checks the connection, and tries to reopen it up to 10 times if it is closed
        /// </summary>
        /// <returns>true if the connection is open and could be opened within 10 tries</returns>
        private static bool CheckOrReopenConnection(MySqlConnection connection)
        {
            //end early
            if (connection.State == System.Data.ConnectionState.Open) return IsOnline = true;
            //if we are still offline
            if (connection.State != System.Data.ConnectionState.Open)
            {
                try
                {
                    connection.Close();
                    connection.Open();
                }
                catch (MySqlException e)
                {
                    int errorCode = e.InnerException is not null && e.InnerException.GetType().IsAssignableTo(typeof(MySqlException))
                        ? ((MySqlException)e.InnerException).Number
                        : e.Number;
                    if (errorCode is 0 or 1042)
                    {
                        //0 means offline
                        _ = UI!.WarningOk("You seem to be offline, functionality limited! You can continue, but you should then provide the templates yourself. " +
                                        "If you are sure you have internet, please check your networking and firewall settings and restart.", "No Internet!");
                        return IsOnline = false;
                    }
                    else if (errorCode == 1045)
                    {
                        //means invalid creds or empty host/password
                        _ = UI!.ErrorOk($"Invalid password\nChange in \"Settings\" window, then restart!\n\n {e.Message}", "Wrong password");
                        return IsOnline = false;
                    }
                }
                //if we have no exception of that kind we can try and just reopen
                int tries = 0;
                while (connection.State != System.Data.ConnectionState.Open && tries < 10)
                {
                    ++tries;
                    if (connection.State == System.Data.ConnectionState.Open) return IsOnline = true;
                    connection.Open();
                    System.Threading.Thread.Sleep(100 * tries);
                }
                return IsOnline = connection.State == System.Data.ConnectionState.Open;
            }
            else return IsOnline = true;
        }

        private static string CleanId(string DataBaseId, string story, string fileName, bool isTemplate)
        {
            if (story == "Hints" && isTemplate) fileName = "English";
            string tempID = DataBaseId[(story + fileName).Length..];
            if (tempID.Length - (isTemplate ? 8 : TranslationManager.Language.Length) <= 0) Debugger.Break();
            return tempID.Remove(tempID.Length - (isTemplate ? 8 : TranslationManager.Language.Length));
        }

        /// <summary>
        /// Establishes the connection and handles the password stuff
        /// </summary>
        [DebuggerNonUserCode()]
        private static void EstablishConnection()
        {
            bool ValidPassword = false;
            using MySqlConnection connection = new(GetConnString());
            while (!IsOnline && !ValidPassword)
            {
                try
                {
                    if (connection.State != System.Data.ConnectionState.Open) connection.Open();
                    ValidPassword = CheckOrReopenConnection(connection);
                }
                catch (MySqlException e)
                {
                    if (e.Number is 0 or 1042)
                    {
                        //0 means offline
                        _ = UI!.WarningOk("You seem to be offline, functionality limited! You can continue, but you should then provide the templates yourself. " +
                                        "If you are sure you have internet, please check your networking and firewall settings and restart.", "No Internet!");
                        IsOnline = false;
                    }
                    else if (e.Number == 1045)
                    {
                        //means invalid creds or empty host/password
                        _ = UI!.ErrorOk($"Invalid password\nChange in \"Settings\" window, then restart!\n\n {e.Message}", "Wrong password");
                        IsOnline = false;
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// Executes the command, returns true if succeeded. reopens the connection if necessary
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private static bool ExecuteOrReOpen(MySqlCommand command)
        {
            if (CheckOrReopenConnection(command.Connection))
            {
                if (OfflineToDo.Count > 0)
                {
                    LogManager.Log("Queued commands found, executing those first");
                    UI?.InfoOk("You are back online, syncing progress to server", "Back online!");

                    while (OfflineToDo.Count > 0)
                    {
                        try
                        {
                            OfflineToDo.Dequeue().ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            LogManager.Log($"While trying to execute a queued command, this happened:\n" + e.ToString(), LogManager.Level.Error);
                        }
                    }

                    LogManager.Log("All queued commands executed");
                    UI?.InfoOk("Sync done!", "Back online!");
                }

                bool executedSuccessfully = false;
                int tries = 0; //reset try count
                while (!executedSuccessfully && tries < 10)
                {
                    Exception? e = null;
                    try
                    {
                        ++tries;
                        executedSuccessfully = command.ExecuteNonQuery() > 0;
                    }
                    catch (Exception _e)
                    {
                        if (!executedSuccessfully) System.Threading.Thread.Sleep(500 * tries);
                        e = _e;
                    }
                    if (!executedSuccessfully)
                        LogManager.Log($"even after executing the {command.CommandText.TrimWithDelim("[...]", 1000)},\n for ten times we got an exception, probably no internet. " + e?.ToString(), LogManager.Level.Error);
                }

                return executedSuccessfully;
            }
            else
            {
                OfflineToDo.Enqueue(command);
                LogManager.Log("Database cannot be reached, queueing command");
            }
            return false;
        }

        private static string GetConnString()
        {
            string password = Settings.Default.DbPassword;
            string returnString = password != string.Empty
                ? "Server=www.rinderha.cc;Uid=user;Pwd=" + password + ";Database=main;Pooling=True;MinimumPoolSize=10;MaximumPoolSize=150;"
                : string.Empty;
            return returnString;
        }

        public static bool GetAllLinesAndTemplateForStory(string story, string language, out Dictionary<string, FileData> lines, out Dictionary<string, FileData> templates)
        {
            UI!.SignalUserWait();
            string command = "SELECT id, filename, english, translation, category, translated, approved"
                + FROM
                + " WHERE story = @story AND (LANGUAGE IS NULL OR LANGUAGE = @language) AND deleted = 0;";

            using MySqlConnection connection = new(GetConnString());
            lines = new();
            templates = new();

            if (CheckOrReopenConnection(connection))
            {
                using MySqlCommand cmd = connection.CreateCommand();

                cmd.CommandText = command;
                cmd.Parameters.Clear();
                _ = cmd.Parameters.AddWithValue("@language", language);
                _ = cmd.Parameters.AddWithValue("@story", story);
                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    string file = string.Empty;
                    string id = string.Empty;
                    while (reader.Read())
                    {
                        file = reader.GetString(1);
                        if (!lines.ContainsKey(file))
                        {
                            //add empty filedata if we have a new file in the results
                            lines.Add(file, new FileData(story, file));
                            templates.Add(file, new FileData(story, file));
                        }
                        //if translation is null, we have template
                        if (reader.IsDBNull(3))
                        {
                            id = CleanId(reader.GetString("id"), story, file, true);
                            templates[file].Add(id, new LineData(
                                id,
                                story,
                                file,
                                (StringCategory)reader.GetInt32("category"),
                                reader.GetString("english"),
                                true));
                        }
                        //if it is not, we have a translation
                        else
                        {
                            id = CleanId(reader.GetString("id"), story, file, false);
                            lines[file].Add(id, new LineData(
                                id,
                                story,
                                file,
                                (StringCategory)reader.GetInt32("category"),
                                reader.GetString("translation"))
                            {
                                IsApproved = reader.GetInt32("approved") == 1,
                                IsTranslated = reader.GetInt32("translated") == 1
                            });
                        }
                    }
                }
                else
                {
                    _ = UI.WarningOk("No templates or templates found for " + story + " in " + language, "Info");
                    LogManager.Log("No stories found for " + story + " in " + language);
                }
                reader.Close();
            }
            UI.SignalUserEndWait();
            return templates.Count > 0;
        }

        internal static bool GetAllLinesAndTemplateForFile(string story, string file, string language, out FileData lines, out FileData templates)
        {
            UI!.SignalUserWait();
            string command = "SELECT id, english, translation, category, translated, approved"
                + FROM
                + " WHERE story = @story AND filename = @filename AND (LANGUAGE IS NULL OR LANGUAGE = @language) AND deleted = 0;";

            using MySqlConnection connection = new(GetConnString());
            lines = new(story, file);
            templates = new(story, file);

            if (CheckOrReopenConnection(connection))
            {
                using MySqlCommand cmd = connection.CreateCommand();

                cmd.CommandText = command;
                cmd.Parameters.Clear();
                _ = cmd.Parameters.AddWithValue("@language", language);
                _ = cmd.Parameters.AddWithValue("@story", story);
                _ = cmd.Parameters.AddWithValue("@filename", file);
                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    string id = string.Empty;
                    while (reader.Read())
                    {
                        //if translation is null, we have template
                        if (reader.IsDBNull(2))
                        {
                            id = CleanId(reader.GetString("id"), story, file, true);
                            templates.Add(id, new LineData(
                                id,
                                story,
                                file,
                                (StringCategory)reader.GetInt32("category"),
                                reader.GetString("english"),
                                true));
                        }
                        //if it is not, we have a translation
                        else
                        {
                            id = CleanId(reader.GetString("id"), story, file, false);
                            lines.Add(id, new LineData(
                                id,
                                story,
                                file,
                                (StringCategory)reader.GetInt32("category"),
                                reader.GetString("translation"))
                            {
                                IsApproved = reader.GetInt32("approved") == 1,
                                IsTranslated = reader.GetInt32("translated") == 1
                            });
                        }
                    }
                }
                else
                {
                    _ = UI.WarningOk("No templates or templates found for " + story + " in " + language, "Info");
                    LogManager.Log("No stories found for " + story + " in " + language);
                }
                reader.Close();
            }
            UI.SignalUserEndWait();
            return templates.Count > 0;
        }
    }
}