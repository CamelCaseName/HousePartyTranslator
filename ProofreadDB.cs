using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace HousePartyTranslator
{
    static class ProofreadDB
    {
        private static SqlConnection sqlConnection;
        private static SqlCommand insertApproved;
        private static string dbPath;

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
            insertApproved = new SqlCommand("", sqlConnection);
            Console.WriteLine("DB opened");
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
