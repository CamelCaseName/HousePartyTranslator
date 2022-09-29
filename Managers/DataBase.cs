using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HousePartyTranslator.Managers
{
    /// <summary>
    /// A static class to interface with the database running on https://www.rinderha.cc for use with the Translation Helper for the game House Party.
    /// </summary>
    internal static class DataBase
    {
        public static string DBVersion;
        private static readonly MySqlConnection sqlConnection = new MySqlConnection();
        private static MySqlCommand MainCommand;
        private static MySqlDataReader MainReader;
        private static string SoftwareVersion;
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
            MainCommand.CommandText = command;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id + language);
            MainCommand.Parameters.AddWithValue("@language", language);

            CheckOrReopenConnection();
            MainReader = MainCommand.ExecuteReader();


            if (MainReader.HasRows && !MainReader.IsDBNull(0))
            {
                translation = new LineData()
                {
                    Category = (StringCategory)MainReader.GetInt32("category"),
                    Comments = !MainReader.IsDBNull(7) ? MainReader.GetString("comment").Split('#') : new string[] { },
                    FileName = fileName,
                    ID = CleanId(id, story, fileName, false),
                    IsApproved = MainReader.GetInt32("approved") > 0,
                    IsTemplate = false,
                    IsTranslated = MainReader.GetInt32("translated") > 0,
                    Story = story,
                    TemplateString = "",
                    TranslationString = MainReader.GetString("translation")
                };
                wasSuccessfull = true;
            }
            else
            {
                translation = new LineData("", "", "", StringCategory.Neither);
            }
            MainReader.Close();
            return wasSuccessfull;
        }

        public static bool GetAllLineData(string fileName, string story, out Dictionary<string, LineData> LineDataList, string language)
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
            MainCommand.CommandText = command;
            MainCommand.Parameters.Clear();
            if (story != "Hints") MainCommand.Parameters.AddWithValue("@filename", fileName);
            MainCommand.Parameters.AddWithValue("@story", story);
            MainCommand.Parameters.AddWithValue("@language", language);

            LineDataList = new Dictionary<string, LineData>();

            CheckOrReopenConnection();
            MainReader = MainCommand.ExecuteReader();

            if (MainReader.HasRows)
            {
                while (MainReader.Read())
                {
                    if (!MainReader.IsDBNull(0))
                    {
                        LineDataList.Add(CleanId(MainReader.GetString("id"), story, fileName, false), new LineData()
                        {
                            Category = (StringCategory)MainReader.GetInt32("category"),
                            Comments = !MainReader.IsDBNull(7) ? MainReader.GetString("comment").Split('#') : new string[] { },
                            FileName = fileName,
                            ID = CleanId(MainReader.GetString("id"), story, fileName, false),
                            IsApproved = MainReader.GetInt32("approved") > 0,
                            IsTemplate = false,
                            IsTranslated = MainReader.GetInt32("translated") > 0,
                            Story = story,
                            TemplateString = "",
                            TranslationString = MainReader.GetString("translation")
                        });
                    }
                }
                wasSuccessfull = true;
            }
            else
            {
                MessageBox.Show("Ids can't be loaded", "Info", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            MainReader.Close();
            return wasSuccessfull;
        }

        /// <summary>
        /// Returns a list of all ids and categorys for the given file in a list of LineData.
        /// </summary>
        /// <param name="fileName">The name of the file read from without the extension.</param>
        /// <param name="story">The name of the story the file is from, should be the name of the parent folder.</param>
        /// <param name="LineDataList">A list of all ids and categorys in LineData objects.</param>
        /// <param name="language"> The translated language in ISO 639-1 notation.</param>
        /// <returns>
        /// True if ids are found for this file.
        /// </returns>
        public static bool GetAllLineDataTemplate(string fileName, string story, out Dictionary<string, LineData> LineDataList)
        {
            Application.UseWaitCursor = true;
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
            MainCommand.CommandText = command;
            MainCommand.Parameters.Clear();
            if (story != "Hints") MainCommand.Parameters.AddWithValue("@filename", fileName);
            MainCommand.Parameters.AddWithValue("@story", story);

            CheckOrReopenConnection();
            MainReader = MainCommand.ExecuteReader();
            LineDataList = new Dictionary<string, LineData>();

            if (MainReader.HasRows)
            {
                while (MainReader.Read())
                {
                    LineDataList.Add(CleanId(MainReader.GetString("id"), story, fileName, true),
                        new LineData(
                            CleanId(MainReader.GetString("id"), story, fileName, true),
                            story,
                            fileName,
                            (StringCategory)MainReader.GetInt32("category"),
                            MainReader.GetString("english"),
                            true));
                }
            }
            else
            {
                MessageBox.Show("Ids can't be loaded", "Info", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                LogManager.LogEvent("No template ids found for " + story + "/" + fileName);
            }
            MainReader.Close();

            Application.UseWaitCursor = false;
            return LineDataList.Count > 0;
        }

        /// <summary>
        /// Establishes the connection and handles the password stuff
        /// </summary>
        /// <param name="mainWindow">the window to spawn the password box as a child of</param>
        private static void EstablishConnection(Fenster mainWindow)
        {
            Application.UseWaitCursor = true;
            bool isConnected = false;
            while (!isConnected)
            {
                string connString = GetConnString();
                if (connString == "")
                {
                    Application.UseWaitCursor = false;
                    //Pasword has to be set first time
                    Password Passwordbox = new Password();
                    DialogResult passwordResult = Passwordbox.ShowDialog(mainWindow);
                    if (passwordResult == DialogResult.OK)
                    {
                        Properties.Settings.Default.dbPassword = Passwordbox.GetPassword();
                        Properties.Settings.Default.Save();
                    }
                    else
                    {
                        if (Passwordbox.GetPassword().Length > 1)
                        {
                            MessageBox.Show("Invalid password", "Wrong password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            Environment.Exit(-1);
                        }
                    }

                    Application.UseWaitCursor = true;
                }
                else
                {
                    sqlConnection.ConnectionString = connString;
                    try
                    {
                        sqlConnection.Open();
                        if (sqlConnection.State != System.Data.ConnectionState.Open)
                        {
                            //password may have to be changed
                            MessageBox.Show("Can't connect to the database, contact CamelCaseName (Lenny)");
                            Application.Exit();
                        }
                        else
                        {
                            isConnected = true;
                        }
                    }
                    catch (MySqlException e)
                    {
                        MessageBox.Show($"Invalid password\nChange in \"Settings\" window, then restart!\n\n {e.Message}", "Wrong password", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.UseWaitCursor = false;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Needs to be called in order to use the class, checks the connection and displays the current version information in the window title.
        /// </summary>
        /// <param name="mainWindow">The windows to change the title of.</param>
        public static void InitializeDB(Fenster mainWindow)
        {
            //establish connection and handle password
            EstablishConnection(mainWindow);

            MainCommand = new MySqlCommand("", sqlConnection);
            //Console.WriteLine("DB opened");

            //checking template version
            MySqlCommand getVersion = new MySqlCommand("SELECT story " +
                                                       FROM +
                                                       "WHERE ID = \"version\";", sqlConnection);
            MainReader = getVersion.ExecuteReader();
            MainReader.Read();
            DBVersion = MainReader.GetString(0);
            MainReader.Close();

            string fileVersion = Properties.Settings.Default.version;
            if (fileVersion == "")
            {
                // get software version from db
                SoftwareVersion = DBVersion;
                Properties.Settings.Default.version = DBVersion;
            }
            else
            {
                //add . if it is missing
                if (!fileVersion.Contains("."))
                {
                    fileVersion = "1." + fileVersion;
                    Properties.Settings.Default.version = fileVersion;
                }
                //try casting wi9th invariant culture, log and try and work around it if it fails
                if (!double.TryParse(DBVersion, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double _dbVersion))
                {
                    _dbVersion = 1.0;
                    LogManager.LogEvent($"invalid string cast to double. Offender: {DBVersion}", LogManager.Level.Warning);
                }
                if (!double.TryParse(fileVersion, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double _fileVersion))
                {
                    _fileVersion = 1.0;
                    LogManager.LogEvent($"invalid string cast to double. Offender: {fileVersion}", LogManager.Level.Warning);
                }

                //save comparison
                if (_dbVersion > _fileVersion)
                {
                    //update local software version from db
                    SoftwareVersion = DBVersion;
                    Properties.Settings.Default.version = DBVersion;
                }
                else
                {
                    //set version from settings
                    SoftwareVersion = fileVersion;
                }
            }
            Properties.Settings.Default.Save();

            //set global variable for later actions
            TranslationManager.IsUpToDate = DBVersion == SoftwareVersion;
            if (!TranslationManager.IsUpToDate)
            {
                MessageBox.Show($"Current software version({SoftwareVersion}) and data version({DBVersion}) differ." +
                            $" You may acquire the latest version of this program. " +
                            $"If you know that you have newer strings, you may select the template files to upload the new versions!",
                            "Updating string database",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
            }
            mainWindow.Text += " (File Version: " + SoftwareVersion + ", DB Version: " + DBVersion + ", Application version: " + SoftwareVersionManager.LocalVersion + ")";
            mainWindow.Update();

            Application.UseWaitCursor = false;
        }

        public static bool RemoveOldTemplates(string fileName, string story)
        {
            string command = DELETE + @"WHERE filename = @filename AND story = @story AND SUBSTRING(id, -8) = @templateid";
            MainCommand.CommandText = command;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@story", story);
            MainCommand.Parameters.AddWithValue("@fileName", fileName);
            MainCommand.Parameters.AddWithValue("@templateid", "template");

            //return if at least one row was changed
            return ExecuteOrReOpen(MainCommand);
        }

        public static bool RemoveOldTemplates(string story)
        {
            string command = DELETE + @"WHERE story = @story AND SUBSTRING(id, -8) = @templateid";
            MainCommand.CommandText = command;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@story", story);
            MainCommand.Parameters.AddWithValue("@templateid", "template");

            //return if at least one row was changed
            return ExecuteOrReOpen(MainCommand);
        }

        /// <summary>
        /// Set the english template for string in the database.
        /// </summary>
        /// <param name="id"> The id of that string as found in the file before the "|".</param>
        /// <param name="fileName">The name of the file read from without the extension.</param>
        /// <param name="story">The name of the story the file is from, should be the name of the parent folder.</param>
        /// <param name="template">The template for the given id.</param>
        /// <returns>
        /// True if exactly one row was set, false if it was not the case.
        /// </returns>
        public static bool UploadTemplates(Dictionary<string, LineData> lines)
        {
            int c = 0;
            for (int x = 0; x < ((lines.Count / 500) + 0.5); x++)
            {
                StringBuilder builder = new StringBuilder(INSERT + " (id, story, filename, category, english) VALUES ", lines.Count * 100);

                //add all values
                int v = c;
                for (int j = 0; j < 500; j++)
                {
                    builder.Append($"(@id{v}, @story{v}, @fileName{v}, @category{v}, @english{v}),");
                    v++;
                    if (v >= lines.Values.Count) break;
                }

                builder.Remove(builder.Length - 1, 1);
                string command = builder.ToString() + " ON DUPLICATE KEY UPDATE english = VALUES(english);";

                MainCommand.CommandText = command;
                MainCommand.Parameters.Clear();

                //insert all the parameters
                for (int k = 0; k < 500; k++)
                {
                    var line = lines.Values.ElementAt(c);
                    MainCommand.Parameters.AddWithValue($"@id{c}", line.Story + line.FileName + line.ID + "template");
                    MainCommand.Parameters.AddWithValue($"@story{c}", line.Story);
                    MainCommand.Parameters.AddWithValue($"@fileName{c}", line.FileName);
                    MainCommand.Parameters.AddWithValue($"@category{c}", (int)line.Category);
                    MainCommand.Parameters.AddWithValue($"@english{c}", line.TemplateString);
                    ++c;
                    if (c >= lines.Values.Count) break;
                }

                ExecuteOrReOpen(MainCommand);
            }

            //return if at least ione row was changed
            return true;
        }

        /// <summary>
        /// Sets the translation of a string in the database in the given language.
        /// </summary>
        /// <param name="id">The id of that string as found in the file before the "|".</param>
        /// <param name="fileName">The name of the file read from without the extension.</param>
        /// <param name="story">The name of the story the file is from, should be the name of the parent folder.</param>
        /// <param name="translation">The translation of the string with the given id.</param>
        /// <param name="language">The translated language in ISO 639-1 notation.</param>
        /// <returns>
        /// True if at least one row was set, false if it was not the case.
        /// </returns>
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
            MainCommand.CommandText = command;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", lineData.Story + lineData.FileName + lineData.ID + language);
            MainCommand.Parameters.AddWithValue("@story", lineData.Story);
            MainCommand.Parameters.AddWithValue("@fileName", lineData.FileName);
            MainCommand.Parameters.AddWithValue("@category", (int)lineData.Category);
            MainCommand.Parameters.AddWithValue("@translated", 1);
            MainCommand.Parameters.AddWithValue("@approved", lineData.IsApproved ? 1 : 0);
            MainCommand.Parameters.AddWithValue("@language", language);
            MainCommand.Parameters.AddWithValue($"@comment", comment);
            MainCommand.Parameters.AddWithValue("@translation", lineData.TranslationString);

            return ExecuteOrReOpen(MainCommand);
        }

        /// <summary>
        /// Updates all translated strings for the selected file
        /// </summary>
        /// <param name="translationData">A list of all loaded lines for this file</param>
        /// <param name="fileName">The name of the file read from without the extension.</param>
        /// <param name="storyName">The name of the story the file is from, should be the name of the parent folder.</param>
        /// <param name="language">The translated language in ISO 639-1 notation.</param>
        /// <returns></returns>
        public static bool UpdateTranslations(Dictionary<string, LineData> translationData, string language)
        {
            string storyName = translationData.ElementAt(0).Value.Story;
            string fileName = translationData.ElementAt(0).Value.FileName;
            StringBuilder builder = new StringBuilder(INSERT + @" (id, story, filename, category, translated, approved, language, comment, translation) VALUES ", translationData.Count * 100);

            //add all values
            for (int j = 0; j < translationData.Count; j++)
            {
                builder.Append($"(@id{j}, @story{j}, @filename{j}, @category{j}, @translated{j}, @approved{j}, @language{j}, @comment{j}, @translation{j}),");
            }

            builder.Remove(builder.Length - 1, 1);

            MainCommand.CommandText = builder.ToString() + ("  ON DUPLICATE KEY UPDATE translation = VALUES(translation), comment = VALUES(comment), approved = VALUES(approved)");
            MainCommand.Parameters.Clear();

            int i = 0;
            //insert all the parameters
            foreach (var item in translationData.Values)
            {
                string comment = "";
                for (int j = 0; j < item.Comments?.Length; j++)
                {
                    if (item.Comments[j].Length > 1)
                        comment += item.Comments[j] + "#";
                }

                MainCommand.Parameters.AddWithValue($"@id{i}", storyName + fileName + item.ID + language);
                MainCommand.Parameters.AddWithValue($"@story{i}", storyName);
                MainCommand.Parameters.AddWithValue($"@fileName{i}", fileName);
                MainCommand.Parameters.AddWithValue($"@category{i}", (int)item.Category);
                MainCommand.Parameters.AddWithValue($"@translated{i}", 1);
                MainCommand.Parameters.AddWithValue($"@approved{i}", item.IsApproved ? 1 : 0);
                MainCommand.Parameters.AddWithValue($"@language{i}", language);
                MainCommand.Parameters.AddWithValue($"@comment{i}", comment);
                MainCommand.Parameters.AddWithValue($"@translation{i}", item.TranslationString);
                ++i;
            }

            return ExecuteOrReOpen(MainCommand);
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
            MainCommand.CommandText = command;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@story", Properties.Settings.Default.version);
            MainCommand.Parameters.AddWithValue("@version", "version");

            //return if at least ione row was changed
            return ExecuteOrReOpen(MainCommand);
        }

        private static string CleanId(string DataBaseId, string story, string fileName, bool isTemplate)
        {
            if (story == "Hints" && isTemplate) fileName = "English";
            string tempID = DataBaseId.Substring((story + fileName).Length);
            return tempID.Remove(tempID.Length - (isTemplate ? 8 : TabManager.ActiveTranslationManager.Language.Length));
        }

        /// <summary>
        /// Executes the command, returns true if succeeded. reopens the connection if necessary
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private static bool ExecuteOrReOpen(MySqlCommand command)
        {
            if (CheckOrReopenConnection())
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
                        LogManager.LogEvent($"While trying to execute the following command{command.CommandText},\n this happened:\n" + e.ToString(), LogManager.Level.Error);
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
        private static bool CheckOrReopenConnection()
        {
            //try to reopen the connection if it died
            int tries = 0;
            while (sqlConnection.State != System.Data.ConnectionState.Open && tries < 10)
            {
                ++tries;
                sqlConnection.Close();
                System.Threading.Thread.Sleep(500);
                sqlConnection.Open();
            }

            if (sqlConnection.State != System.Data.ConnectionState.Open)
            {
                //password may have to be changed
                MessageBox.Show("Can't connect to the database, contact CamelCaseName (Lenny)");
                Application.Exit();
                return false;
            }
            else return true;
        }

        private static string GetConnString()
        {
            string password = Properties.Settings.Default.dbPassword;
            string returnString;
            if (password != "")
            {
                returnString = "Server=www.rinderha.cc;Uid=user;Pwd=" + password + ";Database=main;";
            }
            else
            {
                returnString = "";
            }
            return returnString;
        }
    }
}