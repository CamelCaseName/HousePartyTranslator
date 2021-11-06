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

        static ProofreadDB()
        {
            sqlConnection = new SqlConnection();
            string dbPath = Path.GetFullPath(".\\ProofreadDB.mdf");
            sqlConnection.ConnectionString = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={dbPath};Integrated Security=True;";
            sqlConnection.Open();
            Console.WriteLine("DB opened");
        }

        public static bool SetStringAccepted(string id, string fileName, string story)
        {
            
            return false;
        }
    }
}
