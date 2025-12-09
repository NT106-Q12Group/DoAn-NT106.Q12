using System;
using System.Text;
using System.Net.Sockets;
using System.Threading; // Cần thư viện này để chạy luồng ngầm

namespace CaroGame_TCPClient
{
    public class TCPClient
    {
        private string serverIP;
        private int serverPort;
        private TcpClient? client;
        private bool isConnected;
        private NetworkStream? stream;

        // --- SỰ KIỆN MỚI: Để báo tin nhắn từ Server về Form (dùng khi Tìm trận/Chơi game) ---
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

                // Lưu ý: Chưa gọi StartListening() ở đây. 
                // Ta sẽ gọi nó thủ công sau khi Đăng nhập thành công để tránh xung đột luồng.

                return true;
            }
            catch (Exception e)
            {
                isConnected = false;
                Console.WriteLine($"[DEBUG] Connect error: {e.Message}");
                return false;
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

        // --- HÀM 1: GỬI VÀ CHỜ PHẢN HỒI (Dùng cho Login, Register, GetEmail...) ---
        private string SendRequest(string request)
        {
            try
            {
                if (!isConnected || stream == null)
                {
                    if (!Connect()) return "ERROR|Cannot connect to server";
                }

                var s = stream!;
                byte[] data = Encoding.UTF8.GetBytes(request);
                s.Write(data, 0, data.Length);

                // --- QUAN TRỌNG: FIX LỖI KẸT DỮ LIỆU ---
                s.Flush(); // Ép dữ liệu gửi đi ngay lập tức
                // ---------------------------------------

                // Nếu đang trong chế độ Lắng nghe (Game), không được dùng hàm này để đọc
                // Tuy nhiên, logic hiện tại tách biệt Login (Request) và Game (Listener) nên ổn.
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

        // --- HÀM 2: GỬI NHANH (Dùng cho Tìm trận, Đánh cờ - Không chờ phản hồi tại chỗ) ---
        public void Send(string data)
        {
            try
            {
                if (isConnected && stream != null)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush(); // Quan trọng
                }
            }
            catch { Disconnect(); }
        }

        // --- HÀM 3: BẮT ĐẦU NGHE (Gọi hàm này sau khi Login thành công) ---
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
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead == 0) { Disconnect(); break; }

                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        // Bắn sự kiện ra ngoài Form
                        OnMessageReceived?.Invoke(message);
                    }
                }
                catch { Disconnect(); }
            });
            listenerThread.IsBackground = true;
            listenerThread.Start();
        }

        // --- CÁC HÀM NGHIỆP VỤ CŨ (GIỮ NGUYÊN) ---

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

        public string UpdatePassword(string username, string newPassword)
        {
            string hashedPassword = HashUtil.Sha256(newPassword);
            return SendRequest($"UPDATEPASS|{username}|{hashedPassword}");
        }


        public string GetUser(string username)
        {
            return SendRequest($"GETPLAYER|{username}");
        }

        public string Logout(string username)
        {
            // Với Logout, gửi thông báo rồi cắt luôn, không cần chờ phản hồi
            Send($"SIGNOUT|{username}");
            Disconnect();
            return "Success";
        }

        public string GetEmail(string username)
        {
            return SendRequest($"GETEMAIL|{username}");
        }

        public bool IsConnected() => isConnected;

        // --- CÁC HÀM MỚI CHO GAME (MATCHMAKING) ---

        public void FindMatch(string username)
        {
            // Gửi lệnh tìm trận, kết quả sẽ trả về qua OnMessageReceived
            Send($"FIND_MATCH|{username}");
        }

        public void CancelMatch(string username)
        {
            Send($"CANCEL_MATCH|{username}");
        }

        // Hàm đánh cờ (sẽ dùng sau)
        public void SendMove(int x, int y, int roomId)
        {
            Send($"MOVE|{x}|{y}|{roomId}");
        }

        public void SendPacket(Packet packet)
        {
            // Chuyển Packet thành chuỗi string theo đúng format Server bạn muốn
            string data = "";
            if (packet.Command == "MOVE")
                data = $"MOVE|{packet.Point.X}|{packet.Point.Y}"; // Lưu ý: Server bạn dùng dấu | hay ; ?
                                                                  // Trong code cũ bạn dùng |, nên ở đây mình dùng | cho đồng bộ
            else if (packet.Command == "CHAT")
                data = $"CHAT|{packet.Message}";

            // Gọi hàm Send có sẵn
            Send(data);
        }

        public string Receive()
        {
            if (!isConnected || stream == null) return null;
            try
            {
                byte[] buffer = new byte[4096];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                    return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
            catch { }
            return null;
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