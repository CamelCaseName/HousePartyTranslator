using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HousePartyTranslator.Managers
{
    /// <summary>
    /// A static class to interface with the database running on https://www.rinderha.cc for use with the Translation Helper for the game House Party.
    /// </summary>
    static class DataBaseManager
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



        /// <summary>
        /// Add a comment to the translation to the string defined by id and language.
        /// </summary>
        /// <param name="id">The id of that string as found in the file before the "|".</param> 
        /// <param name="fileName">The name of the file read from without the extension.</param> 
        /// <param name="story">The name of the story the file is from, should be the name of the parent folder.</param> 
        /// <param name="comment"> The comment to be added.</param>
        /// <param name="language">The translated language in ISO 639-1 notation.</param> 
        /// <returns>
        /// True if exactly one row was set, false if it was not the case.
        /// </returns>
        public static bool AddTranslationComment(string id, string fileName, string story, string comment, string language = "de")
        {
            CheckConnection();
            //use # for comment seperator
            GetTranslationComments(id, fileName, story, out string internalComment, language);

            internalComment += ("#" + comment);

            string insertCommand = UPDATE + @"
                                     SET COMMENT = @comment 
                                     WHERE id = @id AND language = @language;";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id + language);
            MainCommand.Parameters.AddWithValue("@language", language);
            MainCommand.Parameters.AddWithValue("@comment", internalComment);

            return ExecuteOrReOpen(MainCommand);
        }

        /// <summary>
        /// Executes a custom SQL command, returns a MySqlDataReader.
        /// </summary>
        /// <param name="command">The command to execute.</param> 
        /// <param name="ResultReader">The result reader will be written to this parameter.</param> 
        public static void ExecuteCustomCommand(string command, out MySqlDataReader ResultReader)
        {
            CheckConnection();
            MainCommand.Parameters.Clear();
            MainCommand.CommandText = command;
            ResultReader = MainCommand.ExecuteReader();
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
        public static bool GetAllLineDataBasicForFile(string fileName, string story, out List<LineData> LineDataList)
        {
            CheckConnection();
            Application.UseWaitCursor = true;
            string insertCommand;
            if (story == "Hints")
            {
                insertCommand = @"SELECT id, category, english 
                                    " + FROM + @" 
                                    WHERE story = @story AND language IS NULL
                                    ORDER BY category ASC;";
            }
            else
            {
                insertCommand = @"SELECT id, category, english 
                                    " + FROM + @" 
                                    WHERE filename = @filename AND story = @story AND language IS NULL
                                    ORDER BY category ASC;";
            }
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@filename", fileName);
            MainCommand.Parameters.AddWithValue("@story", story);

            MainReader = MainCommand.ExecuteReader();
            LineDataList = new List<LineData>();

            if (MainReader.HasRows)
            {
                while (MainReader.Read())
                {
                    LineDataList.Add(
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
            }
            MainReader.Close();

            Application.UseWaitCursor = false;
            return LineDataList.Count > 0;
        }

        /// <summary>
        /// Gets all approval states for a certain file.
        /// </summary>
        /// <param name="fileName">The name of the file read from without the extension.</param>
        /// <param name="story">The name of the story the file is from, should be the name of the parent folder.</param>
        /// <param name="approvalStates">A list of approvals in the LineData class.</param>
        /// <param name="language"> The translated language in ISO 639-1 notation.</param>
        /// <returns>
        /// True if approvals are found for the file.
        /// </returns>
        public static bool GetAllStatesForFile(string fileName, string story, out List<LineData> approvalStates, string language = "de")
        {
            CheckConnection();
            Application.UseWaitCursor = true;
            string insertCommand = @"SELECT id, category, approved, translated, translation, english 
                                     " + FROM + @" 
                                     WHERE filename = @filename AND story = @story AND language = @language;";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@filename", fileName);
            MainCommand.Parameters.AddWithValue("@story", story);
            MainCommand.Parameters.AddWithValue("@language", language);

            MainReader = MainCommand.ExecuteReader();
            approvalStates = new List<LineData>();

            if (MainReader.HasRows)
            {
                while (MainReader.Read())
                {
                    approvalStates.Add(
                        new LineData(
                            CleanId(MainReader.GetString("id"), story, fileName, false),
                            story,
                            fileName,
                            (StringCategory)MainReader.GetInt32("category"),
                            MainReader.GetInt32("approved") == 1,
                            "",
                            MainReader.GetString("translation")
                            ));
                }
            }
            else
            {
                //its annoying and useless by now
                //MessageBox.Show("No string is approved so far.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            MainReader.Close();

            Application.UseWaitCursor = false;
            return approvalStates.Count > 0;
        }

        /// <summary>
        /// Gets all translations for a given file.
        /// </summary>
        /// <param name="fileName">The name of the file read from without the extension.</param>
        /// <param name="story">The name of the story the file is from, should be the name of the parent folder.</param>
        /// <param name="translations">A list of translations in the LineData class.</param>
        /// <param name="language"> The translated language in ISO 639-1 notation.</param>
        /// <returns>
        /// True if at least one translation came back:).
        /// </returns>
        public static bool GetAllTranslatedStringForFile(string fileName, string story, out List<LineData> translations, string language = "de")
        {
            CheckConnection();
            Application.UseWaitCursor = true;
            string insertCommand = @"SELECT id, category, translation 
                                     " + FROM + @" 
                                     WHERE filename = @filename AND story = @story AND language = @language;";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@filename", fileName);
            MainCommand.Parameters.AddWithValue("@story", story);
            MainCommand.Parameters.AddWithValue("@language", language);

            MainReader = MainCommand.ExecuteReader();
            translations = new List<LineData>();

            if (MainReader.HasRows)
            {
                while (MainReader.Read())
                {
                    translations.Add(
                        new LineData(
                            CleanId(MainReader.GetString("id"), story, fileName, false),
                            story,
                            fileName,
                            (StringCategory)MainReader.GetInt32("category"),
                            MainReader.GetString("translation")));
                }
            }
            else
            {
                //also useless because it also shows when nothing is translated
                //MessageBox.Show("Translations can't be loaded", "Info", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            MainReader.Close();

            Application.UseWaitCursor = false;
            return translations.Count > 0;
        }

        /// <summary>
        /// Get the Approval state of a string in the database.
        /// </summary>
        /// <param name="id">The id of that string as found in the file before the "|".</param> 
        /// <param name="fileName">The name of the file read from without the extension.</param> 
        /// <param name="story">The name of the story the file is from, should be the name of the parent folder.</param> 
        /// <param name="language">The translated language in ISO 639-1 notation.</param> 
        /// <returns>
        /// True if the selected string is approved, false if not.
        /// </returns>
        public static bool GetStringApprovedState(string id, string fileName, string story, string language = "de")
        {
            CheckConnection();
            string insertCommand = @"SELECT approved 
                                     " + FROM + @" 
                                     WHERE id = @id;";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id + language);

            MainReader = MainCommand.ExecuteReader();
            MainReader.Read();
            int isApproved;
            if (MainReader.HasRows)
            {
                isApproved = MainReader.GetInt32("approved");
            }
            else
            {
                isApproved = 0;
            }
            MainReader.Close();
            return isApproved == 1;
        }

        /// <summary>
        /// Gets the english teplate string for the specified id.
        /// </summary>
        /// <param name="id"> The id of that string as found in the file before the "|".</param>
        /// <param name="fileName">The name of the file read from without the extension.</param> 
        /// <param name="story">The name of the story the file is from, should be the name of the parent folder.</param> 
        /// <param name="template">The template string will be written to this parameter.</param> 
        /// <returns>
        /// True if a template was found.
        /// </returns>
        public static bool GetStringTemplate(string id, string fileName, string story, out string template)
        {
            CheckConnection();
            Application.UseWaitCursor = true;
            if (story == "Hints")
            {
                fileName = "English";
            }
            string insertCommand = @"SELECT english 
                                     " + FROM + @" 
                                     WHERE id = @id";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id + "template");

            MainReader = MainCommand.ExecuteReader();
            MainReader.Read();
            if (MainReader.HasRows)
            {
                template = MainReader.GetString(0);
            }
            else
            {
                template = "**Template can't be loaded**\n(a restart can help)";
            }
            MainReader.Close();
            Application.UseWaitCursor = false;
            return template != "";
        }

        /// <summary>
        /// Get the isTranslated state of a string in the database (without considereing if an actual translation is present or not!).
        /// </summary>
        /// <param name="id">The id of that string as found in the file before the "|".</param> 
        /// <param name="fileName">The name of the file read from without the extension.</param> 
        /// <param name="story">The name of the story the file is from, should be the name of the parent folder.</param> 
        /// <param name="language">The translated language in ISO 639-1 notation.</param> 
        /// <returns>
        /// True if the requested string is considered translated, false if not.
        /// </returns>
        public static bool GetStringTranslatedState(string id, string fileName, string story, string language = "de")
        {
            CheckConnection();
            string insertCommand = @"SELECT translated 
                                     " + FROM + @"  
                                     WHERE id = @id AND language = @language;";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id + language);
            MainCommand.Parameters.AddWithValue("@language", language);

            MainReader = MainCommand.ExecuteReader();
            MainReader.Read();
            int isTranslated;
            if (MainReader.HasRows)
            {
                isTranslated = MainReader.GetInt32(0);
            }
            else
            {
                isTranslated = 0;
                MessageBox.Show("Translation state can't be loaded");
            }
            MainReader.Close();
            return isTranslated == 1;
        }

        /// <summary>
        /// Gets the translation of a string in the database in the given language.
        /// </summary>
        /// <param name="id">The id of that string as found in the file before the "|".</param> 
        /// <param name="fileName">The name of the file read from without the extension.</param> 
        /// <param name="story">The name of the story the file is from, should be the name of the parent folder.</param> 
        /// <param name="translation">The translatin will be written to that string.</param> 
        /// <param name="language">The translated language in ISO 639-1 notation.</param> 
        /// <returns>
        /// True if a translation was found.
        /// </returns>
        public static bool GetStringTranslation(string id, string fileName, string story, out string translation, string language = "de")
        {
            CheckConnection();
            string insertCommand = @"SELECT translation 
                                     " + FROM + @" 
                                     WHERE id = @id AND language = @language;";
            bool wasSuccessfull = false;
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id + language);
            MainCommand.Parameters.AddWithValue("@language", language);

            MainReader = MainCommand.ExecuteReader();
            MainReader.Read();

            if (MainReader.HasRows && !MainReader.IsDBNull(0))
            {
                translation = MainReader.GetString("translation");
                wasSuccessfull = true;
            }
            else
            {
                translation = "**Translation can't be loaded**";
            }
            MainReader.Close();
            return wasSuccessfull;
        }

        /// <summary>
        /// Gets a string consisting of all comments for the string, seperated by '#'
        /// </summary>
        /// <param name="id">The id of that string as found in the file before the "|".</param> 
        /// <param name="fileName">The name of the file read from without the extension.</param> 
        /// <param name="story">The name of the story the file is from, should be the name of the parent folder.</param> 
        /// <param name="comments">The returned comments are written tho this string.</param> 
        /// <param name="language">The translated language in ISO 639-1 notation.</param> 
        /// <returns>
        /// True if exactly one row was set, false if it was not the case.
        /// </returns>
        public static bool GetTranslationComments(string id, string fileName, string story, out string comments, string language = "de")
        {
            CheckConnection();
            string insertCommand = @"SELECT comment 
                                     " + FROM + @" 
                                     WHERE id = @id AND language = @language;";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id + language);
            MainCommand.Parameters.AddWithValue("@language", language);

            MainReader = MainCommand.ExecuteReader();
            MainReader.Read();

            if (MainReader.HasRows && !MainReader.IsDBNull(0))
            {
                comments = MainReader.GetString("comment");
            }
            else
            {
                comments = "";
            }
            MainReader.Close();
            return comments != "";
        }

        /// <summary>
        /// Gets an array of all comments from the specified string.
        /// </summary>
        /// <param name="id">The id of that string as found in the file before the "|".</param> 
        /// <param name="fileName">The name of the file read from without the extension.</param> 
        /// <param name="story">The name of the story the file is from, should be the name of the parent folder.</param> 
        /// <param name="comments">The returned comments are placed in this array.</param> 
        /// <param name="language">The translated language in ISO 639-1 notation.</param> 
        /// <returns>
        /// True if exactly one row was set, false if it was not the case.
        /// </returns>
        public static bool GetTranslationComments(string id, string fileName, string story, out string[] comments, string language = "de")
        {
            CheckConnection();
            //use # for comment seperator
            if (GetTranslationComments(id, fileName, story, out string commentString, language))
            {
                comments = commentString.Split('#');
                return true;
            }
            else
            {
                comments = new string[0] { };
                return false;
            }
        }

        /// <summary>
        /// Needs to be called in order to use the class, checks the connection and displays the current version information in the window title.
        /// </summary>
        /// <param name="mainWindow">The windows to change the title of.</param> 
        public static void InitializeDB(Fenster mainWindow)
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
                //add 0. if it is missing
                if (!fileVersion.Contains("."))
                {
                    fileVersion = "1." + fileVersion;
                    Properties.Settings.Default.version = fileVersion;
                }

                if (double.Parse(DBVersion, System.Globalization.NumberStyles.Any) > double.Parse(fileVersion, System.Globalization.NumberStyles.Any))
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

        /// <summary>
        /// Set the Approval state of a string in the database.
        /// </summary>
        /// <param name="id">The id of that string as found in the file before the "|".</param> 
        /// <param name="fileName">The name of the file read from without the extension.</param> 
        /// <param name="story">The name of the story the file is from, should be the name of the parent folder.</param> 
        /// <param name="isApproved">The approval state to set the string to (0 or 1).</param> 
        /// <param name="language">The translated language in ISO 639-1 notation.</param> 
        /// <returns>
        /// True if at least one row was returned.
        /// </returns>
        public static bool SetStringApprovedState(string id, string fileName, string story, StringCategory category, bool isApproved, string language = "de")
        {
            string insertCommand = INSERT + @" (id, story, filename, category, translated, approved, language) 
VALUES(@id, @story, @fileName, @category, @translated, @approved, @language)
ON DUPLICATE KEY UPDATE approved = @approved;";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id + language);
            MainCommand.Parameters.AddWithValue("@story", story);
            MainCommand.Parameters.AddWithValue("@fileName", fileName);
            MainCommand.Parameters.AddWithValue("@category", (int)category);
            MainCommand.Parameters.AddWithValue("@translated", 1);
            MainCommand.Parameters.AddWithValue("@approved", isApproved ? 1 : 0);
            MainCommand.Parameters.AddWithValue("@language", language);

            return ExecuteOrReOpen(MainCommand);
        }

        /// <summary>
        /// Updates the selected translated strings (specified by the indices) for the selected file
        /// </summary>
        /// <param name="translationData">A list of all loaded lines for this file</param>
        /// <param name="fileName">The name of the file read from without the extension.</param>
        /// <param name="storyName">The name of the story the file is from, should be the name of the parent folder.</param>
        /// <param name="language">The translated language in ISO 639-1 notation.</param>
        /// <param name="indices">The list of indices to update.</param>
        /// <returns></returns>
        public static bool SetStringSelectedTranslations(List<LineData> translationData, string fileName, string storyName, string language, List<int> indices)
        {
            foreach (var index in indices)
            {
                LineData line = translationData[index];
                string insertCommand = INSERT + @" (id, story, filename, category, translated, approved, language, translation) 
                                     VALUES(@id, @story, @filename, @category, @translated, @approved, @language, @translation)
                                     ON DUPLICATE KEY UPDATE translation = @translation;";
                MainCommand.CommandText = insertCommand;
                MainCommand.Parameters.Clear();
                MainCommand.Parameters.AddWithValue("@id", storyName + fileName + line.ID + language);
                MainCommand.Parameters.AddWithValue("@story", storyName);
                MainCommand.Parameters.AddWithValue("@fileName", fileName);
                MainCommand.Parameters.AddWithValue("@category", (int)line.Category);
                MainCommand.Parameters.AddWithValue("@translated", 1);
                MainCommand.Parameters.AddWithValue("@approved", 0);
                MainCommand.Parameters.AddWithValue("@language", language);
                MainCommand.Parameters.AddWithValue("@translation", line.TranslationString);
            }
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
        public static bool SetStringTemplate(string id, string fileName, string story, StringCategory category, string template)
        {
            string insertCommand = INSERT + @" (id, story, filename, category, english) 
                                     VALUES(@id, @story, @fileName, @category, @english)
                                     ON DUPLICATE KEY UPDATE english = @english;";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id + "template");
            MainCommand.Parameters.AddWithValue("@story", story);
            MainCommand.Parameters.AddWithValue("@fileName", fileName);
            MainCommand.Parameters.AddWithValue("@category", (int)category);
            MainCommand.Parameters.AddWithValue("@english", template);

            //return if at least ione row was changed
            return ExecuteOrReOpen(MainCommand);
        }

        /// <summary>
        /// Set the isTranslated state of a string in the database (without considereing if an actual translation is present or not!).
        /// </summary>
        /// <param name="id"> The id of that string as found in the file before the "|".</param>
        /// <param name="fileName">The name of the file read from without the extension.</param> 
        /// <param name="story">The name of the story the file is from, should be the name of the parent folder.</param> 
        /// <param name="isTranslated">The translation state to set the string to (0 or 1).</param> 
        /// <param name="language">The translated language in ISO 639-1 notation.</param> 
        /// <returns>
        /// True if at least one row was set.
        /// </returns>
        public static bool SetStringTranslatedState(string id, string fileName, string story, StringCategory category, bool isTranslated, string language = "de")
        {
            string insertCommand = FROM + @" (id, story, filename, category, translated, language) 
                                     VALUES(@id, @story, @fileName, @category, @translated, @language)
                                     ON DUPLICATE KEY UPDATE translated = @translated;";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id + language);
            MainCommand.Parameters.AddWithValue("@story", story);
            MainCommand.Parameters.AddWithValue("@fileName", fileName);
            MainCommand.Parameters.AddWithValue("@category", (int)category);
            MainCommand.Parameters.AddWithValue("@translated", isTranslated ? 1 : 0);
            MainCommand.Parameters.AddWithValue("@language", language);

            return ExecuteOrReOpen(MainCommand);
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
        public static bool SetStringTranslation(string id, string fileName, string story, StringCategory category, string translation, string language = "de")
        {
            string insertCommand = INSERT + @" (id, story, filename, category, translated, approved, language, translation) 
                                     VALUES(@id, @story, @filename, @category, @translated, @approved, @language, @translation)
                                     ON DUPLICATE KEY UPDATE translation = @translation;";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id + language);
            MainCommand.Parameters.AddWithValue("@story", story);
            MainCommand.Parameters.AddWithValue("@fileName", fileName);
            MainCommand.Parameters.AddWithValue("@category", (int)category);
            MainCommand.Parameters.AddWithValue("@translated", 1);
            MainCommand.Parameters.AddWithValue("@approved", 0);
            MainCommand.Parameters.AddWithValue("@language", language);
            MainCommand.Parameters.AddWithValue("@translation", translation);

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
        public static bool SetStringTranslations(List<LineData> translationData, string fileName, string storyName, string language)
        {
            foreach (var line in translationData)
            {
                string insertCommand = INSERT + @" (id, story, filename, category, translated, approved, language, translation) 
                                     VALUES(@id, @story, @filename, @category, @translated, @approved, @language, @translation)
                                     ON DUPLICATE KEY UPDATE translation = @translation;";
                MainCommand.CommandText = insertCommand;
                MainCommand.Parameters.Clear();
                MainCommand.Parameters.AddWithValue("@id", storyName + fileName + line.ID + language);
                MainCommand.Parameters.AddWithValue("@story", storyName);
                MainCommand.Parameters.AddWithValue("@fileName", fileName);
                MainCommand.Parameters.AddWithValue("@category", (int)line.Category);
                MainCommand.Parameters.AddWithValue("@translated", 1);
                MainCommand.Parameters.AddWithValue("@approved", 0);
                MainCommand.Parameters.AddWithValue("@language", language);
                MainCommand.Parameters.AddWithValue("@translation", line.TranslationString);
            }
            return ExecuteOrReOpen(MainCommand);
        }

        /// <summary>
        /// Set all comments on the string defined by id and language.
        /// </summary>
        /// <param name="id">The id of that string as found in the file before the "|".</param> 
        /// <param name="fileName">The name of the file read from without the extension.</param> 
        /// <param name="story">The name of the story the file is from, should be the name of the parent folder.</param> 
        /// <param name="comments">An array of strings which consist of all comments.</param> 
        /// <param name="language"> The translated language in ISO 639-1 notation.</param>
        /// <returns>
        /// True if exactly one row was set, false if it was not the case.
        /// </returns>
        public static bool SetTranslationComments(string id, string fileName, string story, string[] comments, string language = "de")
        {
            //use # for comment seperator
            string internalComment = "";
            foreach (string scomment in comments)
            {
                internalComment += scomment + "#";
            }

            if (internalComment.Length > 0)
            {
                //remove last #
                internalComment = internalComment.Remove(internalComment.Length - 1, 1);
            }

            internalComment = internalComment.Replace(Environment.NewLine, "");

            string insertCommand = INSERT + @" (id, language, comment) 
                                        VALUES(@id, @language, @comment)
                                        ON DUPLICATE KEY UPDATE comment = @comment;";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id + language);
            MainCommand.Parameters.AddWithValue("@language", language);
            MainCommand.Parameters.AddWithValue("@comment", internalComment);

            //return if at least ione row was changed
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
            string insertCommand = UPDATE + @"
                                     SET story = @story 
                                     WHERE ID = @version;";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@story", "0." + (int.Parse(DBVersion.Split('.')[1]) + 1).ToString());
            MainCommand.Parameters.AddWithValue("@version", "version");

            //return if at least ione row was changed
            return ExecuteOrReOpen(MainCommand);
        }

        /// <summary>
        /// Reopens the connection if necessary
        /// </summary>
        private static void CheckConnection()
        {
            if (sqlConnection.State != System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
                sqlConnection.Open();
            }
        }

        private static string CleanId(string DataBaseId, string story, string fileName, bool isTemplate)
        {
            if (story == "Hints" && isTemplate) fileName = "English";
            string tempID = DataBaseId.Substring((story + fileName).Length);
            return tempID.Remove(tempID.Length - (isTemplate ? 8 : 2));
        }

        /// <summary>
        /// Executes the command, returns true if succeeded. reopens the connection if necessary
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private static bool ExecuteOrReOpen(MySqlCommand command)
        {
            //try to reopen the connection if it died
            int tries = 10;
            while (sqlConnection.State != System.Data.ConnectionState.Open && tries > 0)
            {
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

            return command.ExecuteNonQuery() > 0;
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
