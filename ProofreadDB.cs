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
        //private static string dbPath;
        private static readonly string SoftwareVersion = "0.20";
        private static string DBVersion;

        static ProofreadDB()
        {
            sqlConnection = new MySqlConnection();
            //dbPath = Path.GetFullPath(".\\ProofreadDB.mdf");
            //sqlConnection.ConnectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={dbPath};Integrated Security=True;";

        }

        public static void InitializeDB()
        {
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
        }

        public static bool SetStringAccepted(string id, string fileName = " ", string story = " ", string language = "de", string comments = " ")
        {
            string insertCommand = @"REPLACE INTO translations (id, story, filename, translated, approved, language, comment) 
                                   VALUES(@id, @story, @fileName, @translated, @approved, @language, @comments)";
            insertApproved.CommandText = insertCommand;
            insertApproved.Parameters.Clear();
            insertApproved.Parameters.AddWithValue("@id", story + fileName + id);
            insertApproved.Parameters.AddWithValue("@story", story);
            insertApproved.Parameters.AddWithValue("@fileName", fileName);
            insertApproved.Parameters.AddWithValue("@translated", 1);
            insertApproved.Parameters.AddWithValue("@approved", 1);
            insertApproved.Parameters.AddWithValue("@language", language);
            insertApproved.Parameters.AddWithValue("@comments", comments);


            if (insertApproved.ExecuteNonQuery() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool AddTemplateString(string id, string story, string fileName, string english, string language = "de")
        {
            string insertCommand = @"REPLACE INTO translations (id, story, filename, language, english) 
                                                        VALUES(@id, @story, @fileName, @language, @english)";
            insertApproved.CommandText = insertCommand;
            insertApproved.Parameters.Clear();
            insertApproved.Parameters.AddWithValue("@id", story + fileName + id);
            insertApproved.Parameters.AddWithValue("@story", story);
            insertApproved.Parameters.AddWithValue("@fileName", fileName);
            insertApproved.Parameters.AddWithValue("@language", language);
            insertApproved.Parameters.AddWithValue("@english", english);

            //return if at least ione row was changed
            return insertApproved.ExecuteNonQuery() > 0;
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
