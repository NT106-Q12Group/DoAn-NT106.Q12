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

                // 1. Tạo bảng nếu chưa tồn tại (Đã thêm cột Score)
                string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Player (
                    PlayerID INTEGER PRIMARY KEY AUTOINCREMENT,
                    PlayerName TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL,
                    Email TEXT,
                    Birthday TEXT,
                    Score INTEGER DEFAULT 0
                );";

                using (var cmd = new SQLiteCommand(createTableQuery, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                // 2. Kiểm tra và thêm cột Score nếu bảng đã tồn tại từ trước nhưng thiếu cột này
                // (Giúp code hoạt động ngay cả với file db cũ)
                string checkColumnQuery = "SELECT COUNT(*) FROM pragma_table_info('Player') WHERE name='Score';";
                using (var checkCmd = new SQLiteCommand(checkColumnQuery, connection))
                {
                    long count = (long)checkCmd.ExecuteScalar();

                    // Nếu count == 0 nghĩa là cột Score chưa tồn tại
                    if (count == 0)
                    {
                        string addColumnQuery = "ALTER TABLE Player ADD COLUMN Score INTEGER DEFAULT 0;";
                        using (var alterCmd = new SQLiteCommand(addColumnQuery, connection))
                        {
                            alterCmd.ExecuteNonQuery();
                            Console.WriteLine("Updated database: Added 'Score' column.");
                        }
                    }
                }

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