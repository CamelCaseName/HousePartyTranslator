using MySql.Data.MySqlClient;
using System;
using System.Linq;
using System.Text;
using Translator.Core.Helpers;
using Translator.UICompatibilityLayer;

namespace Translator.Core
{
    /// <summary>
    /// A static class to interface with the database running on https://www.rinderha.cc for use with the Translation Helper for the game House Party.
    /// </summary>
    public static class DataBase<TLineItem, TUIHandler, TTabController, TTab>
        where TLineItem : class, ILineItem, new()
        where TUIHandler : class, IUIHandler<TLineItem, TTabController, TTab>, new()
        where TTabController : class, ITabController<TLineItem, TTab>, new()
        where TTab : class, ITab<TLineItem>, new()
    {
        public static string DBVersion { get; private set; } = "0.0.0";
        public static string AppTitle { get; private set; } = string.Empty;
        private static string SoftwareVersion = "0.0.0.0";
        private static TUIHandler UI = new();
        public static bool IsOnline { get; private set; } = false;

#if DEBUG
		private static readonly string FROM = "FROM debug ";
		private static readonly string INSERT = "INSERT INTO debug ";
		private static readonly string UPDATE = "UPDATE debug ";
		private static readonly string DELETE = "DELETE FROM debug ";
#else
        private static readonly string DELETE = "DELETE FROM translations ";
        private static readonly string FROM = "FROM translations ";
        private static readonly string INSERT = "INSERT INTO translations ";
        private static readonly string UPDATE = "UPDATE translations ";
#endif

        public static bool GetLineData(string id, string fileName, string story, out LineData translation, string language)
        {
            string command = @"SELECT * " + FROM + @" WHERE id = @id AND language = @language;";
            bool wasSuccessfull = false;
            using MySqlConnection connection = new(GetConnString());
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();
            using MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = command;
            cmd.Parameters.Clear();
            _ = cmd.Parameters.AddWithValue("@id", story + fileName + id + language);
            _ = cmd.Parameters.AddWithValue("@language", language);

            if (CheckOrReopenConnection(connection))
            {
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
                        TemplateString = "",
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

        public static bool GetAllLineData(string fileName, string story, out FileData LineDataList, string language)
        {
            bool wasSuccessfull = false;
            string command;
            if (story == "Hints")
            {
                command = @"SELECT * " + FROM + @"
                            WHERE story = @story AND language = @language
                            ORDER BY category ASC;";
            }
            else
            {
                command = @"SELECT * " + FROM + @"
                            WHERE filename = @filename AND story = @story AND language = @language
                            ORDER BY category ASC;";
            }
            using MySqlConnection connection = new(GetConnString());
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();
            using MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = command;
            cmd.Parameters.Clear();

            if (story != "Hints") _ = cmd.Parameters.AddWithValue("@filename", fileName);
            _ = cmd.Parameters.AddWithValue("@story", story);
            _ = cmd.Parameters.AddWithValue("@language", language);

            LineDataList = new FileData();

            if (CheckOrReopenConnection(connection))
            {
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
                                TemplateString = "",
                                TranslationString = reader.GetString("translation")
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
                    _ = UI.WarningOk("Ids can't be loaded", "Potential issue");
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
            UI.SignalUserWait();
            string command;
            if (story == "Hints")
            {
                command = @"SELECT id, category, english
                                    " + FROM + @"
                                    WHERE story = @story AND language IS NULL
                                    ORDER BY category ASC;";
            }
            else
            {
                command = @"SELECT id, category, english
                                    " + FROM + @"
                                    WHERE filename = @filename AND story = @story AND language IS NULL
                                    ORDER BY category ASC;";
            }
            using MySqlConnection connection = new(GetConnString());
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();
            using MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = command;
            cmd.Parameters.Clear();
            if (story != "Hints") _ = cmd.Parameters.AddWithValue("@filename", fileName);
            _ = cmd.Parameters.AddWithValue("@story", story);

            LineDataList = new FileData();

            if (CheckOrReopenConnection(connection))
            {
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
                                    reader.GetString("english"),
                                    true));
                    }
                }
                else
                {
                    _ = UI.WarningOk("Ids can't be loaded", "Info");
                    LogManager.Log("No template ids found for " + story + "/" + fileName);
                }
                reader.Close();
            }
            UI.SignalUserEndWait();
            return LineDataList.Count > 0;
        }

        /// <summary>
        /// Establishes the connection and handles the password stuff
        /// </summary>
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
                    if (e.Number == 0)
                    {
                        //0 means offline
                        _ = UI.WarningOk("You seem to be offline, functionality limited! You can continue, but you should then provide the templates yourself. " +
                            "If you are sure you have internet, please check your networking and firewall settings and restart.", "No Internet!");
                    }
                    else if (e.Number == 1045)
                    {
                        //means invalid creds
                        _ = UI.ErrorOk($"Invalid password\nChange in \"Settings\" window, then restart!\n\n {e.Message}", "Wrong password");
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// Needs to be called in order to use the class, checks the connection and displays the current version information in the window title.
        /// </summary>
        public static void Initialize(TUIHandler uIHandler, string AppVersion)
        {
            UI = uIHandler;
            //establish connection and handle password
            EstablishConnection();

            if (!IsOnline)
            {
                AppTitle = "Translator (File Version: " + SoftwareVersion + ", DB Version: *Offline*, Application version: " + AppVersion + ")";
            }
            else
            {
                using MySqlConnection connection = new(GetConnString());
                if (connection.State != System.Data.ConnectionState.Open) connection.Open();
                using var cmd = new MySqlCommand("", connection);
                _ = CheckOrReopenConnection(connection);
                //Console.WriteLine("DB opened");

                //checking template version
                var getVersion = new MySqlCommand("SELECT story " +
                                                    FROM +
                                                    "WHERE ID = \"version\";", connection);
                using MySqlDataReader reader = getVersion.ExecuteReader();
                _ = reader.Read();
                DBVersion = reader.GetString(0);
                reader.Close();

                string fileVersion = Settings.Default.FileVersion;
                if (fileVersion == "")
                {
                    // get software version from db
                    SoftwareVersion = DBVersion;
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
                        SoftwareVersion = DBVersion;
                        Settings.Default.FileVersion = DBVersion;
                    }
                    else
                    {
                        //set version from settings
                        SoftwareVersion = fileVersion;
                    }
                }
                Settings.Default.Save();

                //set global variable for later actions
                TranslationManager<TLineItem, TUIHandler, TTabController, TTab>.IsUpToDate = DBVersion == SoftwareVersion;
                if (!TranslationManager<TLineItem, TUIHandler, TTabController, TTab>.IsUpToDate && Settings.Default.AdvancedModeEnabled)
                {
                    _ = UI.WarningOk($"Current software version({SoftwareVersion}) and data version({DBVersion}) differ. " + "You may acquire the latest version of this program. " + "If you know that you have newer strings, you may select the template files to upload the new versions!", "Updating string database");
                }
                AppTitle = "Translator (File Version: " + SoftwareVersion + ", DB Version: " + DBVersion + ", Application version: " + AppVersion + ")";
            }
        }

        public static bool RemoveOldTemplates(string fileName, string story)
        {
            string command = DELETE + @"WHERE filename = @filename AND story = @story AND SUBSTRING(id, -8) = @templateid";
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
            string command = DELETE + @"WHERE story = @story AND SUBSTRING(id, -8) = @templateid";
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
        /// Set the english template for string in the database.
        /// </summary>
        /// <param name="lines">FileData object with the relevant info</param>
        /// <returns>
        /// True if exactly one row was set, false if it was not the case.
        /// </returns>
        public static bool UploadTemplates(FileData lines)
        {
            int c = 0;
            for (int x = 0; x < ((lines.Count / 500) + 0.5); x++)
            {
                var builder = new StringBuilder(INSERT + " (id, story, filename, category, english) VALUES ", lines.Count * 100);

                //add all values
                int v = c;
                for (int j = 0; j < 500; j++)
                {
                    _ = builder.Append($"(@id{v}, @story{v}, @fileName{v}, @category{v}, @english{v}),");
                    v++;
                    if (v >= lines.Values.Count) break;
                }

                _ = builder.Remove(builder.Length - 1, 1);
                string command = builder.ToString() + " ON DUPLICATE KEY UPDATE english = VALUES(english);";

                using MySqlConnection connection = new(GetConnString());
                using MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = command;
                cmd.Parameters.Clear();

                //insert all the parameters
                for (int k = 0; k < 500; k++)
                {
                    LineData line = lines.Values.ElementAt(c);
                    _ = cmd.Parameters.AddWithValue($"@id{c}", line.Story + line.FileName + line.ID + "template");
                    _ = cmd.Parameters.AddWithValue($"@story{c}", line.Story);
                    _ = cmd.Parameters.AddWithValue($"@fileName{c}", line.FileName);
                    _ = cmd.Parameters.AddWithValue($"@category{c}", (int)line.Category);
                    _ = cmd.Parameters.AddWithValue($"@english{c}", line.TemplateString);
                    ++c;
                    if (c >= lines.Values.Count) break;
                }

                _ = ExecuteOrReOpen(cmd);
            }

            //return if at least ione row was changed
            return true;
        }

        /// <summary>
        /// Sets the translation of a string in the database in the given language.
        /// </summary>
        /// <param name="lineData">LineData with the lines to update<param>
        /// <param name="language">The translated language in ISO 639-1 notation.</param>
        /// <returns> True if at least one row was set, false if it was not the case.</returns>
        public static bool UpdateTranslation(LineData lineData, string language)
        {
            string comment = "";
            for (int j = 0; j < lineData.Comments?.Length; j++)
            {
                comment += lineData.Comments[j] + "#";
            }
            string command = INSERT + @" (id, story, filename, category, translated, approved, language, comment, translation)
                                     VALUES(@id, @story, @filename, @category, @translated, @approved, @language, @comment, @translation)
                                     ON DUPLICATE KEY UPDATE translation = @translation;";
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
            _ = cmd.Parameters.AddWithValue("@translation", lineData.TranslationString);

            return ExecuteOrReOpen(cmd);
        }

        /// <summary>
        /// Updates all translated strings for the selected file
        /// </summary>
        /// <param name="translationData">A list of all loaded lines for this file</param>
        /// <param name="language">The translated language in ISO 639-1 notation.</param>
        /// <returns></returns>
        public static bool UpdateTranslations(FileData translationData, string language)
        {
            if (translationData.Count > 0)
            {
                string storyName = translationData.ElementAt(0).Value.Story;
                string fileName = translationData.ElementAt(0).Value.FileName;
                var builder = new StringBuilder(INSERT + @" (id, story, filename, category, translated, approved, language, comment, translation) VALUES ", translationData.Count * 100);

                //add all values
                for (int j = 0; j < translationData.Count; j++)
                {
                    _ = builder.Append($"(@id{j}, @story{j}, @filename{j}, @category{j}, @translated{j}, @approved{j}, @language{j}, @comment{j}, @translation{j}),");
                }

                _ = builder.Remove(builder.Length - 1, 1);

                using MySqlConnection connection = new(GetConnString());
                using MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = builder.ToString() + ("  ON DUPLICATE KEY UPDATE translation = VALUES(translation), comment = VALUES(comment), approved = VALUES(approved)");
                cmd.Parameters.Clear();

                int i = 0;
                //insert all the parameters
                foreach (LineData item in translationData.Values)
                {
                    string comment = "";
                    for (int j = 0; j < item.Comments?.Length; j++)
                    {
                        if (item.Comments[j].Length > 1)
                            comment += item.Comments[j] + "#";
                    }

                    _ = cmd.Parameters.AddWithValue($"@id{i}", storyName + fileName + item.ID + language);
                    _ = cmd.Parameters.AddWithValue($"@story{i}", storyName);
                    _ = cmd.Parameters.AddWithValue($"@fileName{i}", fileName);
                    _ = cmd.Parameters.AddWithValue($"@category{i}", (int)item.Category);
                    _ = cmd.Parameters.AddWithValue($"@translated{i}", 1);
                    _ = cmd.Parameters.AddWithValue($"@approved{i}", item.IsApproved ? 1 : 0);
                    _ = cmd.Parameters.AddWithValue($"@language{i}", language);
                    _ = cmd.Parameters.AddWithValue($"@comment{i}", comment);
                    _ = cmd.Parameters.AddWithValue($"@translation{i}", item.TranslationString);
                    ++i;
                }

                return ExecuteOrReOpen(cmd);
            }
            return false;
        }

        /// <summary>
        /// Increases the verison count on the database by 0.01, eg: 0.19 -> 0.20
        /// </summary>
        /// <returns>
        /// Returns true if the update worked.
        /// </returns>
        public static bool UpdateDBVersion()
        {
            string command = UPDATE + @"
                                     SET story = @story
                                     WHERE ID = @version;";
            using MySqlConnection connection = new(GetConnString());
            using MySqlCommand cmd = connection.CreateCommand();
            cmd.CommandText = command;
            cmd.Parameters.Clear();
            _ = cmd.Parameters.AddWithValue("@story", Settings.Default.FileVersion);
            _ = cmd.Parameters.AddWithValue("@version", "version");

            //return if at least ione row was changed
            return ExecuteOrReOpen(cmd);
        }

        private static string CleanId(string DataBaseId, string story, string fileName, bool isTemplate)
        {
            if (story == "Hints" && isTemplate) fileName = "English";
            string tempID = DataBaseId[(story + fileName).Length..];
            return tempID.Remove(tempID.Length - (isTemplate ? 8 : TranslationManager<TLineItem, TUIHandler, TTabController, TTab>.Language.Length));
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
                bool executedSuccessfully = false;
                int tries = 0; //reset try count
                while (!executedSuccessfully && tries < 10)
                {
                    try
                    {
                        ++tries;
                        executedSuccessfully = command.ExecuteNonQuery() > 0;
                    }
                    catch (Exception e)
                    {
                        if (!executedSuccessfully) System.Threading.Thread.Sleep(500 * tries);
                        LogManager.Log($"While trying to execute the following command  {command.CommandText.TrimWithDelim("[...]", 1000)},\n this happened:\n" + e.ToString(), LogManager.Level.Error);
                    }
                }

                return executedSuccessfully;
            }
            return false;
        }

        /// <summary>
        /// Checks the connection, and tries to reopen it up to 10 times if it is closed
        /// </summary>
        /// <returns>true if the connection is open and could be opened within 10 tries</returns>
        private static bool CheckOrReopenConnection(MySqlConnection connection)
        {
            //end early
            if (connection.State == System.Data.ConnectionState.Open) return IsOnline = true;

            //try to reopen the connection if it is not open
            int tries = 0;
            while (connection.State != System.Data.ConnectionState.Open && tries < 25)
            {
                ++tries;
                if (connection.State == System.Data.ConnectionState.Open) return IsOnline = true;
                System.Threading.Thread.Sleep(100 * tries);
            }
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
                    int errorCode;
                    if (e.InnerException != null && e.InnerException.GetType().IsAssignableTo(typeof(MySqlException))) errorCode = ((MySqlException)e.InnerException).Number;
                    else errorCode = e.Number;

                    if (errorCode == 0)
                    {
                        //0 means offline
                        _ = UI.WarningOk("You seem to be offline, functionality limited! You can continue, but you should then provide the templates yourself. " +
                                        "If you are sure you have internet, please check your networking and firewall settings and restart.", "No Internet!");
                        return IsOnline = false;
                    }
                    else if (errorCode == 1045 || errorCode == 1042)
                    {
                        //means invalid creds or empty host/password
                        _ = UI.ErrorOk($"Invalid password\nChange in \"Settings\" window, then restart!\n\n {e.Message}", "Wrong password");
                        return IsOnline = false;
                    }
                }
                return IsOnline = connection.State == System.Data.ConnectionState.Open;
            }
            else return IsOnline = true;
        }

        private static string GetConnString()
        {
            string password = Settings.Default.DbPassword;
            string returnString;
            if (password != "")
            {
                returnString = "Server=www.rinderha.cc;Uid=user;Pwd=" + password + ";Database=main;Pooling=True;MinimumPoolSize=10;MaximumPoolSize=150;";
            }
            else
            {
                returnString = "";
            }
            return returnString;
        }
    }
}