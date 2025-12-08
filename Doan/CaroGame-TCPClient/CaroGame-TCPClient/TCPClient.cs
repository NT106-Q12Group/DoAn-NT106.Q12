using System;
using System.Text;
using System.Net.Sockets;

namespace CaroGame_TCPClient
{
    public class TCPClient
    {
        private string serverIP;
        private int serverPort;
        private TcpClient? client;
        private bool isConnected;
        private NetworkStream? stream;

        public TCPClient(string IP, int port)
        {
            serverIP = IP;
            serverPort = port;
            client = null;
            stream = null;
            isConnected = false;
        }

        public bool Connect()
        {
            try
            {
                if (isConnected) return true;
                client = new TcpClient();
                client.Connect(serverIP, serverPort);
                stream = client.GetStream();
                isConnected = true;
                return true;
            }
            catch (Exception e)
            {
                isConnected = false;
                Console.WriteLine($"[DEBUG] Connect error: {e.Message}");
                // tạm thời cho hiện MessageBox nếu là WinForms/WPF
                System.Windows.Forms.MessageBox.Show(
                    "Connect error: " + e.Message,
                    "DEBUG",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error
                );
                return false;
            }
        }

        public void Disconnect()
        {
            try { stream?.Close(); stream?.Dispose(); } catch { }
            try { client?.Close(); } catch { }
            stream = null;
            client = null;
            isConnected = false;
        }

        private string SendRequest(string request)
        {
            try
            {
                if (!isConnected || stream == null)
                {
                    if (!Connect())
                        return "ERROR|Cannot connect to server";
                }

                var s = stream!;
                byte[] data = Encoding.UTF8.GetBytes(request);
                s.Write(data, 0, data.Length);

                byte[] buffer = new byte[4096];
                int bytesRead = s.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    Disconnect();
                    return "ERROR|Server disconnected";
                }

                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
            catch (Exception ex)
            {
                Disconnect();
                return $"ERROR|Connection error: {ex.Message}";
            }
        }

        public string Register(string username, string password, string email, string birthday)
        {
            string hashedPassword = HashUtil.Sha256(password);
            string request = $"SIGNUP|{username}|{hashedPassword}|{email}|{birthday}";
            return SendRequest(request);
        }

        public string Login(string username, string password)
        {
            string hashedPassword = HashUtil.Sha256(password);
            string request = $"SIGNIN|{username}|{hashedPassword}";
            return SendRequest(request);
        }

        public string GetUser(string username)
        {
            string request = $"GETPLAYER|{username}";
            return SendRequest(request);
        }

        public string Logout(string username)
        {
            string request = $"SIGNOUT|{username}";
            string response = SendRequest(request);
            Disconnect();
            return response;
        }

        public bool TestConnection()
        {
            if (isConnected) return true;
            return Connect();
        }

        public bool IsConnected()
        {
            return isConnected;
        }

        public string GetEmail(string username)
        {
            string request = $"GETEMAIL|{username}";
            return SendRequest(request);
        }

        public string UpdatePassword(string username, string newPassword)
        {
            string hashedPassword = HashUtil.Sha256(newPassword);

            string request = $"UPDATEPASS|{username}|{hashedPassword}";

            return SendRequest(request);
        }

    }

    public static class HashUtil
    {
        public static string Sha256(string input)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
    }
}
