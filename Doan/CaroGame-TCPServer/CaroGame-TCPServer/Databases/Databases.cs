using System;
using System.Data.SQLite;
using System.IO;
using System.Text;

namespace CaroGame_TCPServer.Databases
{
    public class Databases
    {
        private static readonly string DB_Name = "Player.db";
        private static readonly string ConnectionString = $"Data Source={DB_Name};Version=3;";

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }

        public static void InitializeDatabase()
        {
            try
            {
                Console.OutputEncoding = Encoding.UTF8;

                if (!File.Exists(DB_Name))
                {
                    SQLiteConnection.CreateFile(DB_Name);
                    Console.WriteLine("Created database successfully!");
                }

                using var connection = GetConnection();
                connection.Open();

                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Player (
                PlayerID INTEGER PRIMARY KEY AUTOINCREMENT,
                PlayerName TEXT NOT NULL UNIQUE,
                Password TEXT NOT NULL,
                Email TEXT,
                Birthday TEXT);";

                using var cmd = new SQLiteCommand(createTableQuery, connection);
                cmd.ExecuteNonQuery();
                Console.WriteLine("Checked table: Player table is ready.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating or checking the database: {ex.Message}");
            }
        }

        public static bool ConnectionChecked()
        {
            try
            {
                using var connection = GetConnection();
                connection.Open();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection check failed: {ex.Message}");
                return false;
            }
        }
    }
}
