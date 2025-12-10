using CaroGame_TCPServer.Databases;
using System;
using System.Data;
using System.Data.SQLite;

namespace CaroGame_TCPServer.Player
{
    public class PlayerADO
    {
        private static SQLiteConnection CreateConn() => Databases.Databases.GetConnection();
        private static string Now() => DateTime.Now.ToString("HH:mm:ss");

        public static bool PlayerExists(string playername)
        {
            const string sql = @"SELECT 1 FROM Player WHERE PlayerName = @playername LIMIT 1;";
            try
            {
                using var conn = CreateConn();
                conn.Open();
                using var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.Add("@playername", DbType.String).Value = playername ?? string.Empty;
                var result = cmd.ExecuteScalar();
                return result != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{Now()}] [ERROR] [PlayerExists] {ex.Message}");
                throw;
            }
        }

        public static bool RegisterPlayer(Player player)
        {
            if (player is null ||
                string.IsNullOrWhiteSpace(player.PlayerName) ||
                string.IsNullOrWhiteSpace(player.Password))
            {
                Console.WriteLine($"[{Now()}] [ERROR] Registration failed: missing required fields.");
                return false;
            }

            try
            {
                if (PlayerExists(player.PlayerName))
                {
                    Console.WriteLine($"[{Now()}] [ERROR] Username already exists: '{player.PlayerName}'.");
                    return false;
                }

                const string sql = @"
INSERT INTO Player (PlayerName, Password, Email, Birthday)
VALUES (@playername, @psw, @email, @birth);";

                using var conn = CreateConn();
                conn.Open();

                using var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.Add("@playername", DbType.String).Value = player.PlayerName;
                cmd.Parameters.Add("@psw", DbType.String).Value = player.Password;
                cmd.Parameters.Add("@email", DbType.String).Value =
                    string.IsNullOrWhiteSpace(player.Email) ? (object)DBNull.Value : player.Email;
                cmd.Parameters.Add("@birth", DbType.String).Value =
                    string.IsNullOrWhiteSpace(player.Birthday) ? (object)DBNull.Value : player.Birthday;

                var rows = cmd.ExecuteNonQuery();
                var success = rows > 0;

                Console.WriteLine(success
                    ? $"[{Now()}] [INFO] Registration succeeded: {player.PlayerName}."
                    : $"[{Now()}] [ERROR] Registration failed: no rows inserted.");

                return success;
            }
            catch (SQLiteException ex) when (ex.ResultCode == SQLiteErrorCode.Constraint || ex.ErrorCode == (int)SQLiteErrorCode.Constraint)
            {
                Console.WriteLine($"[{Now()}] [ERROR] Registration failed (constraint): {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{Now()}] [ERROR] Registration failed (unexpected): {ex.Message}");
                return false;
            }
        }

        public static Player? Authenticate(string playername, string hashedpsw)
        {
            const string sql = @"
SELECT PlayerId, PlayerName, Password, Email, Birthday
FROM Player
WHERE PlayerName = @playername AND Password = @hashedpsw
LIMIT 1;";

            try
            {
                using var conn = CreateConn();
                conn.Open();
                using var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.Add("@playername", DbType.String).Value = playername ?? string.Empty;
                cmd.Parameters.Add("@hashedpsw", DbType.String).Value = hashedpsw ?? string.Empty;

                using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (!reader.Read())
                {
                    Console.WriteLine($"[{Now()}] [WARN] Authentication failed for: {playername}");
                    return null;
                }

                var p = new Player
                {
                    PlayerID = reader.GetInt32(0),
                    PlayerName = reader.GetString(1),
                    Password = reader.GetString(2),
                    Email = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    Birthday = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                };

                Console.WriteLine($"[{Now()}] [INFO] Authentication succeeded for: {playername}");
                return p;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{Now()}] [ERROR] [Authenticate] {ex.Message}");
                return null;
            }
        }

        public static Player? GetPlayerByPlayerName(string playername)
        {
            const string sql = @"
SELECT PlayerId, PlayerName, Password, Email, Birthday
FROM Player
WHERE PlayerName = @playername
LIMIT 1;";

            try
            {
                using var conn = CreateConn();
                conn.Open();

                using var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.Add("@playername", DbType.String).Value = playername ?? string.Empty;

                using var reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (!reader.Read()) return null;

                return new Player
                {
                    PlayerID = reader.GetInt32(0),
                    PlayerName = reader.GetString(1),
                    Password = reader.GetString(2),
                    Email = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    Birthday = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{Now()}] [ERROR] Failed to get player by name: {ex.Message}");
                return null;
            }
        }

        public static bool UpdatePlayer(Player player)
        {
            const string sql = @"
UPDATE Player 
SET Email = @email, Birthday = @birthday 
WHERE PlayerName = @playername;";

            try
            {
                using var conn = CreateConn();
                conn.Open();

                using var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.Add("@email", DbType.String).Value = player?.Email ?? string.Empty;
                cmd.Parameters.Add("@birthday", DbType.String).Value = player?.Birthday ?? string.Empty;
                cmd.Parameters.Add("@playername", DbType.String).Value = player?.PlayerName ?? string.Empty;

                var rows = cmd.ExecuteNonQuery();
                var success = rows > 0;

                Console.WriteLine(success
                    ? $"[{Now()}] [INFO] Player updated: {player?.PlayerName}"
                    : $"[{Now()}] [WARN] No player updated (name not found?).");

                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{Now()}] [ERROR] Failed to update player: {ex.Message}");
                return false;
            }
        }

        public static bool UpdatePassword(string playername, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(playername) || string.IsNullOrWhiteSpace(newPassword))
            {
                Console.WriteLine($"[{Now()}] [ERROR] UpdatePassword failed: missing username or new password.");
                return false;
            }

            const string sql = @"UPDATE Player SET Password = @password WHERE PlayerName = @playername;";

            try
            {
                using var conn = CreateConn();
                conn.Open();

                using var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.Add("@password", DbType.String).Value = newPassword;
                cmd.Parameters.Add("@playername", DbType.String).Value = playername;

                var rows = cmd.ExecuteNonQuery();
                var success = rows > 0;

                Console.WriteLine(success
                    ? $"[{Now()}] [INFO] Password updated for: {playername}"
                    : $"[{Now()}] [WARN] Password update failed (user not found?): {playername}");

                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{Now()}] [ERROR] Failed to update password: {ex.Message}");
                return false;
            }
        }

        public static bool DeletePlayer(string playername)
        {
            const string sql = @"DELETE FROM Player WHERE PlayerName = @playername;";

            try
            {
                using var conn = CreateConn();
                conn.Open();

                using var cmd = new SQLiteCommand(sql, conn);
                cmd.Parameters.Add("@playername", DbType.String).Value = playername ?? string.Empty;

                var rows = cmd.ExecuteNonQuery();
                var success = rows > 0;

                Console.WriteLine(success
                    ? $"[{Now()}] [INFO] Player deleted: {playername}"
                    : $"[{Now()}] [WARN] No player deleted (name not found?).");

                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{Now()}] [ERROR] Failed to delete player: {ex.Message}");
                return false;
            }
        }
    }
}
