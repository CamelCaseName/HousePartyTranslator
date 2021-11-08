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
            MySqlCommand getVersion = new MySqlCommand("SELECT STORY FROM translations WHERE ID = \"version\";", sqlConnection);
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
                                   VALUES(@id, @story, @fileName, @translated, @approved, @language)";
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
            return false;
        }

        public static bool SetStringTranslatedState(string id, string fileName, string story, bool isTranslated, string language = "de")
        {
            return false;
        }

        public static bool GetStringTranslatedState(string id, string fileName, string story, string language = "de")
        {
            return false;
        }
        
        public static bool SetStringTranslation(string id, string fileName, string story, string translation, string language = "de")
        {
            return false;
        }

        public static bool GetStringTranslation(string id, string fileName, string story, out string translation, string language = "de")
        {
            translation = "";
            return false;
        }

        public static bool AddTranslationComment(string id, string fileName, string story, string comment, string language = "de")
        {
            return false;
        }

        public static bool SetTranslationComments(string id, string fileName, string story, string[] comments, string language = "de")
        {
            return false;
        }

        public static bool GetTranslationComments(string id, string fileName, string story, out string comments, string language = "de")
        {
            //comments seperated by ";"
            comments = "";
            return false;
        }

        public static bool GetTranslationComments(string id, string fileName, string story, out string[] comments, string language = "de")
        {
            comments = null;
            return false;
        }

        public static bool GetAllTranslatedStringForFile(string id, string fileName, string story, out List<LineData> translations, string language = "de")
        {
            translations = new List<LineData>();
            return false;
        }

        public static bool SetStringTemplate(string id, string story, string fileName, string template)
        {
            string insertCommand = @"REPLACE INTO translations (id, story, filename, english) 
                                                        VALUES(@id, @story, @fileName, @english)";
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
            template = "";
            return false;
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
