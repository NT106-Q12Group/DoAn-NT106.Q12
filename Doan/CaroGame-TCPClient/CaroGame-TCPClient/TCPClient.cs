using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace CaroGame_TCPClient
{
    public class TCPClient
    {
        private string serverIP;
        private int serverPort;
        private TcpClient? client;
        private bool isConnected;
        private NetworkStream? stream;

        // Sự kiện bắn tin nhắn từ Server ra ngoài Form
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
            // Đóng stream và client
            try { stream?.Close(); stream?.Dispose(); } catch { }
            try { client?.Close(); } catch { }

            stream = null;
            client = null;
        }

        // --- HÀM 1: Dùng cho Login/Register (Chưa vào game) ---
        // Lưu ý: Chỉ dùng hàm này KHI CHƯA gọi StartListening().
        // Nếu đã vào game mà gọi hàm này sẽ gây xung đột luồng.
        private string SendRequest(string request)
        {
            try
            {
                if (!isConnected || stream == null)
                {
                    if (!Connect()) return "ERROR|Cannot connect to server";
                }

                // Nếu luồng nghe đang chạy mà gọi hàm này thì rất nguy hiểm,
                // nhưng với logic Login trước -> Play sau thì tạm ổn.

                byte[] data = Encoding.UTF8.GetBytes(request);
                stream!.Write(data, 0, data.Length);
                stream!.Flush();

                byte[] buffer = new byte[4096];
                int bytesRead = stream!.Read(buffer, 0, buffer.Length);

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

        // --- HÀM 2: Dùng trong Game (Gửi nhanh, không chờ phản hồi tại chỗ) ---
        public void Send(string data)
        {
            try
            {
                if (isConnected && stream != null)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush(); // Đẩy dữ liệu đi ngay
                }
            }
            catch
            {
                Disconnect();
            }
        }

        // --- HÀM 3: Luồng lắng nghe (Core của Game) ---
        // Gọi hàm này 1 lần duy nhất sau khi Login thành công
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
                        // Hàm này sẽ dừng (block) ở đây cho đến khi Server gửi gì đó về
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);

                        if (bytesRead == 0)
                        {
                            Disconnect();
                            OnMessageReceived?.Invoke("DISCONNECT|Server closed");
                            break;
                        }

                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                        // [QUAN TRỌNG] Bắn tin nhắn ra ngoài để Form xử lý
                        // Form sẽ hứng event này để vẽ bàn cờ, update turn, chat...
                        OnMessageReceived?.Invoke(message);
                    }
                }
                catch
                {
                    Disconnect();
                    OnMessageReceived?.Invoke("DISCONNECT|Connection lost");
                }
            });
            listenerThread.IsBackground = true;
            listenerThread.Start();
        }

        // --- CÁC HÀM NGHIỆP VỤ (AUTH) ---
        public string Register(string fullname, string username, string password, string email, string birthday)
        {
            string hashedPassword = HashUtil.Sha256(password);
            return SendRequest($"SIGNUP|{username}|{hashedPassword}|{email}|{birthday}");
        }

        public string Login(string username, string password)
        {
            string hashedPassword = HashUtil.Sha256(password);
            return SendRequest($"SIGNIN|{username}|{hashedPassword}");
        }

        public string UpdatePassword(string username, string newPassword)
        {
            string hashedPassword = HashUtil.Sha256(newPassword);
            return SendRequest($"UPDATEPASS|{username}|{hashedPassword}");
        }

        public string GetUser(string username) => SendRequest($"GETPLAYER|{username}");
        public string GetFullName(string username) => SendRequest($"GETNAME|{username}");
        public string GetEmail(string username) => SendRequest($"GETEMAIL|{username}");

        public string Logout(string username)
        {
            Send($"SIGNOUT|{username}");
            Disconnect();
            return "Success";
        }

        public bool IsConnected() => isConnected;

        // --- CÁC HÀM GAME (MATCHMAKING & PLAY) ---

        public void FindMatch(string username)
        {
            Send($"FIND_MATCH|{username}");
        }

        public void CancelMatch(string username)
        {
            Send($"CANCEL_MATCH|{username}");
        }

        public void SendMove(int x, int y, int roomId)
        {
            Send($"MOVE|{x}|{y}|{roomId}");
        }

        public void SendPacket(Packet packet)
        {
            string data = "";
            if (packet.Command == "MOVE")
                // Đảm bảo định dạng khớp với Server (dùng | hay ;)
                data = $"MOVE|{packet.Point.X}|{packet.Point.Y}";
            else if (packet.Command == "CHAT")
                data = $"CHAT|{packet.Message}";

            if (!string.IsNullOrEmpty(data))
                Send(data);
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