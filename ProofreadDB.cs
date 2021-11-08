using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace HousePartyTranslator
{
    static class ProofreadDB
    {
        private static MySqlConnection sqlConnection;
        private static MySqlCommand insertApproved;
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
            insertApproved = new MySqlCommand("", sqlConnection);
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
            insertApproved.CommandText = insertCommand;
            insertApproved.Parameters.Clear();
            insertApproved.Parameters.AddWithValue("@id", story + fileName + id);
            insertApproved.Parameters.AddWithValue("@story", story);
            insertApproved.Parameters.AddWithValue("@fileName", fileName);
            insertApproved.Parameters.AddWithValue("@translated", 1);
            insertApproved.Parameters.AddWithValue("@approved", isApproved ? 1 : 0);
            insertApproved.Parameters.AddWithValue("@language", language);

            return insertApproved.ExecuteNonQuery() == 1;
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

        //comments seperated by ";"
        public static bool AddTranslationComment(string id, string fileName, string story, string comment, string language = "de")
        {
            return false;
        }

        //comments seperated by ";"
        public static bool SetTranslationComments(string id, string fileName, string story, string[] comments, string language = "de")
        {
            return false;
        }

        //comments seperated by ";"
        public static bool GetTranslationComments(string id, string fileName, string story, out string comments, string language = "de")
        {
            comments = "";
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
            insertApproved.CommandText = insertCommand;
            insertApproved.Parameters.Clear();
            insertApproved.Parameters.AddWithValue("@id", story + fileName + id);
            insertApproved.Parameters.AddWithValue("@story", story);
            insertApproved.Parameters.AddWithValue("@fileName", fileName);
            insertApproved.Parameters.AddWithValue("@english", template);

            //return if at least ione row was changed
            return insertApproved.ExecuteNonQuery() > 0;
        }

        public static bool GetStringTemplate(string id, string fileName, string story, out string template)
        {
            template = "";
            return false;
        }

        public static bool UpdateDBVersion()
        {
            string insertCommand = @"UPDATE translations SET story = @story WHERE ID = @version;";
            insertApproved.CommandText = insertCommand;
            insertApproved.Parameters.Clear();
            insertApproved.Parameters.AddWithValue("@story", "0." + (int.Parse(DBVersion.Split('.')[1]) + 1).ToString());
            insertApproved.Parameters.AddWithValue("@version", "version");

            //return if at least ione row was changed
            return insertApproved.ExecuteNonQuery() > 0;
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
