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
            sqlConnection = new SqlConnection();
            dbPath = Path.GetFullPath(".\\ProofreadDB.mdf");
            sqlConnection.ConnectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={dbPath};Integrated Security=True;";
            sqlConnection.Open();
            insertApproved = new SqlCommand("", sqlConnection);
            Console.WriteLine("DB opened");
        }

        public static bool SetStringAccepted(string id, string fileName = " ", string story = " ", string comments = " ")
        {
            string insertCommand = @"INSERT INTO dbo.Translations VALUES(@id, @story, @fileName, @translated, @approved, @language, @comments)";
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
    }
}
