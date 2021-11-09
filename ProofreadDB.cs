using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HousePartyTranslator
{
    static class ProofreadDB
    {
        private static MySqlConnection sqlConnection;
        private static MySqlCommand MainCommand;
        private static readonly string SoftwareVersion = "0.20";
        private static string DBVersion;

        static ProofreadDB()
        {
            sqlConnection = new MySqlConnection();
        }

        public static void InitializeDB(Fenster mainWindow)
        {
            Application.UseWaitCursor = true;
            sqlConnection.ConnectionString = GetConnString();
            sqlConnection.Open();
            Console.WriteLine(sqlConnection.State.ToString());
            MainCommand = new MySqlCommand("", sqlConnection);
            Console.WriteLine("DB opened");

            //checking template version
            MySqlCommand getVersion = new MySqlCommand("SELECT story FROM translations WHERE ID = \"version\";", sqlConnection);
            MySqlDataReader reader = getVersion.ExecuteReader();
            reader.Read();
            DBVersion = reader.GetString(0);
            reader.Close();
            //set global variable for later actions
            TranslationManager.main.IsUpToDate = DBVersion == SoftwareVersion;
            if (!TranslationManager.main.IsUpToDate)
            {
                MessageBox.Show($"Current software version({SoftwareVersion}) and data version({DBVersion}) differ." +
                    $" You may acquire the latest version of this program. " +
                    $"If you know that you have newer strings, you may select the template files to upload the new versions!", "Updating string database");
            }
            else
            {
                mainWindow.Text += " (Software Version: " + SoftwareVersion + ", DB Version: " + DBVersion + ")";
                mainWindow.Update();
            }
            Application.UseWaitCursor = false;
        }

        public static bool SetStringApprovedState(string id, string fileName, string story, bool isApproved, string language = "de")
        {
            string insertCommand = @"REPLACE INTO translations (id, story, filename, translated, approved, language) 
                                   VALUES(@id, @story, @fileName, @translated, @approved, @language);";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id);
            MainCommand.Parameters.AddWithValue("@story", story);
            MainCommand.Parameters.AddWithValue("@fileName", fileName);
            MainCommand.Parameters.AddWithValue("@translated", 1);
            MainCommand.Parameters.AddWithValue("@approved", isApproved ? 1 : 0);
            MainCommand.Parameters.AddWithValue("@language", language);

            return MainCommand.ExecuteNonQuery() == 1;
        }

        public static bool GetStringApprovedState(string id, string fileName, string story, string language = "de")
        {
            string insertCommand = @"SELECT approved FROM translations WHERE id = @id AND language = @language;";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id);
            MainCommand.Parameters.AddWithValue("@language", language);

            MySqlDataReader reader = MainCommand.ExecuteReader();
            reader.Read();
            int isApproved = reader.GetInt32(0);
            return isApproved == 1;
        }

        public static bool SetStringTranslatedState(string id, string fileName, string story, bool isTranslated, string language = "de")
        {
            string insertCommand = @"REPLACE INTO translations (id, story, filename, translated, language) 
                                   VALUES(@id, @story, @fileName, @translated, @language);";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id);
            MainCommand.Parameters.AddWithValue("@story", story);
            MainCommand.Parameters.AddWithValue("@fileName", fileName);
            MainCommand.Parameters.AddWithValue("@translated", isTranslated ? 1 : 0);
            MainCommand.Parameters.AddWithValue("@language", language);

            return MainCommand.ExecuteNonQuery() == 1;
        }

        public static bool GetStringTranslatedState(string id, string fileName, string story, string language = "de")
        {
            string insertCommand = @"SELECT translated FROM translations WHERE id = @id AND language = @language;";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id);
            MainCommand.Parameters.AddWithValue("@language", language);

            MySqlDataReader reader = MainCommand.ExecuteReader();
            reader.Read();
            int isTranslated = reader.GetInt32(0);
            return isTranslated == 1;
        }

        public static bool SetStringTranslation(string id, string fileName, string story, string translation, string language = "de")
        {
            string insertCommand = @"REPLACE INTO translations (id, translated, language, translation) 
                                   VALUES(@id, @translated, @language, @translation);";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id);
            MainCommand.Parameters.AddWithValue("@translated", 1);
            MainCommand.Parameters.AddWithValue("@language", language);
            MainCommand.Parameters.AddWithValue("@translation", translation);

            return MainCommand.ExecuteNonQuery() == 1;
        }

        public static bool GetStringTranslation(string id, string fileName, string story, out string translation, string language = "de")
        {
            string insertCommand = @"SELECT translation FROM translations WHERE id = @id AND language = @language;";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id);
            MainCommand.Parameters.AddWithValue("@language", language);

            MySqlDataReader reader = MainCommand.ExecuteReader();
            reader.Read();
            translation = reader.GetString(0);
            return translation != "";
        }

        public static bool AddTranslationComment(string id, string fileName, string story, string comment, string language = "de")
        {
            //use # for comment seperator
            GetTranslationComments(id, fileName, story, out string internalComment, language);

            internalComment += ("#" + comment);

            string insertCommand = @"UPDATE translations SET COMMENT = @comment WHERE id = @id AND language = @language;";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id);
            MainCommand.Parameters.AddWithValue("@language", language);
            MainCommand.Parameters.AddWithValue("@comment", internalComment);

            return MainCommand.ExecuteNonQuery() == 1;
        }

        public static bool SetTranslationComments(string id, string fileName, string story, string[] comments, string language = "de")
        {
            //use # for comment seperator
            string internalComment = "";
            foreach (string scomment in comments)
            {
                internalComment += (scomment + "#");
            }
            //remove last #
            internalComment.Remove(internalComment.Length - 1, 1);

            string insertCommand = @"REPLACE INTO translations (id, language, comment) 
                                                        VALUES(@id, @language, @comment);";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id);
            MainCommand.Parameters.AddWithValue("@language", language);
            MainCommand.Parameters.AddWithValue("@comment", internalComment);

            //return if at least ione row was changed
            return MainCommand.ExecuteNonQuery() > 0;

        }

        /// use # for comment seperator
        public static bool GetTranslationComments(string id, string fileName, string story, out string comments, string language = "de")
        {
            string insertCommand = @"SELECT comment FROM translations WHERE id = @id AND language = @language;";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id);
            MainCommand.Parameters.AddWithValue("@language", language);

            MySqlDataReader reader = MainCommand.ExecuteReader();
            reader.Read();
            comments = reader.GetString(0);
            return comments != "";
        }

        public static bool GetTranslationComments(string id, string fileName, string story, out string[] comments, string language = "de")
        {
            //use # for comment seperator
            string commentString;
            if (GetTranslationComments(id, fileName, story, out commentString, language))
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

        public static bool GetAllTranslatedStringForFile(string fileName, string story, out List<LineData> translations, string language = "de")
        {
            string insertCommand = @"SELECT id, translation FROM translations WHERE filename = @filename AND story = @story AND language = @language;";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@filename", fileName);
            MainCommand.Parameters.AddWithValue("@story", story);
            MainCommand.Parameters.AddWithValue("@language", language);

            MySqlDataReader reader = MainCommand.ExecuteReader();
            translations = new List<LineData>();

            while (reader.Read())
            {
                translations.Add(new LineData(reader.GetString("id"), story, fileName, reader.GetString("translation"), true));
            }

            return translations.Count > 0;
        }

        public static bool GetAllApprovalStatesForFile(string fileName, string story, out List<LineData> approvalStates, string language = "de")
        {
            string insertCommand = @"SELECT id, approved FROM translations WHERE filename = @filename AND story = @story AND language = @language;";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@filename", fileName);
            MainCommand.Parameters.AddWithValue("@story", story);
            MainCommand.Parameters.AddWithValue("@language", language);

            MySqlDataReader reader = MainCommand.ExecuteReader();
            approvalStates = new List<LineData>();
            bool internalIsApproved;

            while (reader.Read())
            {
                internalIsApproved = reader.GetInt32("approved") == 1 ? true : false;
                approvalStates.Add(new LineData(reader.GetString("id"), story, fileName, internalIsApproved));
            }

            return approvalStates.Count > 0;
        }

        public static bool SetStringTemplate(string id, string story, string fileName, string template)
        {
            string insertCommand = @"REPLACE INTO translations (id, story, filename, english) 
                                                        VALUES(@id, @story, @fileName, @english);";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id);
            MainCommand.Parameters.AddWithValue("@story", story);
            MainCommand.Parameters.AddWithValue("@fileName", fileName);
            MainCommand.Parameters.AddWithValue("@english", template);

            //return if at least ione row was changed
            return MainCommand.ExecuteNonQuery() > 0;
        }

        public static bool GetStringTemplate(string id, string fileName, string story, out string template)
        {
            string insertCommand = @"SELECT english FROM translations WHERE id = @id AND language = @language;";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@id", story + fileName + id);
            MainCommand.Parameters.AddWithValue("@language", "en");

            MySqlDataReader reader = MainCommand.ExecuteReader();
            reader.Read();
            template = reader.GetString(0);
            return template != "";
        }

        public static void ExecuteCustomCommand(string command, out MySqlDataReader ResultReader)
        {
            MainCommand.Parameters.Clear();
            MainCommand.CommandText = command;
            ResultReader = MainCommand.ExecuteReader();
        }

        public static bool UpdateDBVersion()
        {
            string insertCommand = @"UPDATE translations SET story = @story WHERE ID = @version;";
            MainCommand.CommandText = insertCommand;
            MainCommand.Parameters.Clear();
            MainCommand.Parameters.AddWithValue("@story", "0." + (int.Parse(DBVersion.Split('.')[1]) + 1).ToString());
            MainCommand.Parameters.AddWithValue("@version", "version");

            //return if at least ione row was changed
            return MainCommand.ExecuteNonQuery() > 0;
        }

        private static string GetConnString()
        {
            string newText = "";
            string text = "Yox|ox7}}}$xcdnoxbk$ii1_cn7\u007fyox1Z}n7SXAFiKmACA`Lo]BA@LMy}k_CBHlx^}1Nk~khkyo7gkcd1";

            for (int i = 0; i < text.Length; i++)
            {
                int charValue = Convert.ToInt32(text[i]); //get the ASCII value of the character
                charValue ^= 10; //xor the value

                newText += char.ConvertFromUtf32(charValue); //convert back to string
            }

            return newText;
        }
    }
}
