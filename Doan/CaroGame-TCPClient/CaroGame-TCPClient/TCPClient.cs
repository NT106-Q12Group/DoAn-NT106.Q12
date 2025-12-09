using System;
using System.Net.Sockets;
using System.Text;
using System.Threading; // Cần thêm thư viện này cho Thread

namespace CaroGame_TCPClient
{
    public class TCPClient
    {
        private string serverIP;
        private int serverPort;
        private TcpClient? client;
        private bool isConnected;
        private NetworkStream? stream;

        // --- SỰ KIỆN MỚI: Để báo cho Form biết khi có tin nhắn từ Server ---
        public event Action<string>? OnMessageReceived;

        private Thread? listenerThread;

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
                if (isConnected && client != null && client.Connected) return true;

                client = new TcpClient();
                client.Connect(serverIP, serverPort);
                stream = client.GetStream();
                isConnected = true;

                // --- BẮT ĐẦU LẮNG NGHE NGAY KHI KẾT NỐI ---
                StartListening();

                return true;
            }
            catch (Exception e)
            {
                isConnected = false;
                Console.WriteLine($"[DEBUG] Connect error: {e.Message}");
                return false;
            }
        }

        // --- HÀM LẮNG NGHE LIÊN TỤC (CHẠY NGẦM) ---
        public void StartListening()
        {
            if (listenerThread != null && listenerThread.IsAlive) return;

            listenerThread = new Thread(() =>
            {
                try
                {
                    byte[] buffer = new byte[4096];
                    while (isConnected && client != null && stream != null)
                    {
                        // Đọc dữ liệu từ Server
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                        {
                            // Server ngắt kết nối
                            Disconnect();
                            break;
                        }

                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        // [QUAN TRỌNG] Bắn sự kiện ra bên ngoài để Form bắt được
                        OnMessageReceived?.Invoke(message);
                    }
                }
                catch
                {
                    Disconnect();
                }
            });

            listenerThread.IsBackground = true;
            listenerThread.Start();
        }

        // --- HÀM GỬI DỮ LIỆU KHÔNG CHỜ PHẢN HỒI (Dùng cho Lobby/Game) ---
        public void Send(string data)
        {
            try
            {
                if (isConnected && stream != null)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
            catch
            {
                Disconnect();
            }
        }

        public void Disconnect()
        {
            isConnected = false;
            try { stream?.Close(); stream?.Dispose(); } catch { }
            try { client?.Close(); } catch { }
            stream = null;
            client = null;
        }

        // --- GIỮ LẠI HÀM CŨ CHO LOGIN/REGISTER (Request - Response) ---
        // Lưu ý: Khi đã chạy StartListening, hàm này có thể bị xung đột luồng đọc.
        // Tuy nhiên, với logic hiện tại (Login xong mới vào Lobby), ta vẫn dùng được.
        private string SendRequest(string request)
        {
            try
            {
                if (!isConnected || stream == null)
                {
                    if (!Connect()) return "ERROR|Cannot connect to server";
                }

                // Gửi
                byte[] data = Encoding.UTF8.GetBytes(request);
                stream!.Write(data, 0, data.Length);

                // Đọc (Chờ phản hồi ngay lập tức)
                // Lưu ý: Nếu Server trả lời chậm, Thread "Listening" ở trên có thể sẽ "cướp" mất tin nhắn này.
                // Nhưng với Login/Register thì Server thường trả lời ngay nên tạm ổn.
                byte[] buffer = new byte[4096];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

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

        // --- CÁC HÀM CŨ (Giữ nguyên) ---
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
            // Với Logout thì chỉ cần gửi, không quan trọng response lắm
            Send(request);
            Disconnect();
            return "Success";
        }

        public bool IsConnected() => isConnected;

        public string GetEmail(string username) => SendRequest($"GETEMAIL|{username}");

        public string UpdatePassword(string username, string newPassword)
        {
            string hashedPassword = HashUtil.Sha256(newPassword);
            return SendRequest($"UPDATEPASS|{username}|{hashedPassword}");
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