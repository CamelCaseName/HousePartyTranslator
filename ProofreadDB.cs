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

        static ProofreadDB()
        {
            sqlConnection = new MySqlConnection();
            //dbPath = Path.GetFullPath(".\\ProofreadDB.mdf");
            //sqlConnection.ConnectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={dbPath};Integrated Security=True;";

        }

        public static void InitializeDB()
        {
            sqlConnection.ConnectionString = "Server=www.rinderha.cc;Uid=user;Pwd=YRKLcAgKIKjFeWHKJFGswaUIHBfrTw;Database=main;";
            sqlConnection.Open();
            Console.WriteLine(sqlConnection.State.ToString());
            insertApproved = new MySqlCommand("", sqlConnection);
            Console.WriteLine("DB opened");

            //checking template version
            MySqlCommand getVersion = new MySqlCommand("SELECT STORY FROM translations WHERE ID = \"version\";", sqlConnection);
            MySqlDataReader reader = getVersion.ExecuteReader();
            reader.Read();
            string DBVersion = reader.GetString(0);
            reader.Close();
            if (DBVersion != SoftwareVersion)
            {
                MessageBox.Show($"Current software version({SoftwareVersion}) and data version({DBVersion}) differ." +
                    $" You may acquire the latest version of this program");
            }
        }

        public static bool SetStringAccepted(string id, string fileName = " ", string story = " ", string comments = " ")
        {
            string insertCommand = @"INSERT INTO translations (id, story, filename, translated, approved, language, comment) 
                                   VALUES(@id, @story, @fileName, @translated, @approved, @language, @comments)";
            insertApproved.CommandText = insertCommand;
            insertApproved.Parameters.Clear();
            insertApproved.Parameters.AddWithValue("@id", story + fileName + id);
            insertApproved.Parameters.AddWithValue("@story", story);
            insertApproved.Parameters.AddWithValue("@fileName", fileName);
            insertApproved.Parameters.AddWithValue("@translated", 1);
            insertApproved.Parameters.AddWithValue("@approved", 1);
            insertApproved.Parameters.AddWithValue("@language", "de");
            insertApproved.Parameters.AddWithValue("@comments", " ");


            if (insertApproved.ExecuteNonQuery() == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool AddTemplateString(string id, string fileName = " ", string story = " ", string language = "de", string english = " ")
        {
            string insertCommand = @"REPLACE INTO translations VALUES(@id, @story, @fileName, @translated, @approved, @language, @comments, @english)";
            insertApproved.CommandText = insertCommand;
            insertApproved.Parameters.Clear();
            insertApproved.Parameters.AddWithValue("@id", story + fileName + id);
            insertApproved.Parameters.AddWithValue("@story", story);
            insertApproved.Parameters.AddWithValue("@fileName", fileName);
            insertApproved.Parameters.AddWithValue("@translated", 1);
            insertApproved.Parameters.AddWithValue("@approved", 1);
            insertApproved.Parameters.AddWithValue("@language", "de");
            insertApproved.Parameters.AddWithValue("@comments", " ");
            insertApproved.Parameters.AddWithValue("@english", english);


            if (insertApproved.ExecuteNonQuery() == 1)
            {
                Console.WriteLine("inserted row");
            }
            else
            {
                Console.WriteLine("sth broken");
            }
            return false;
        }
    }
}
