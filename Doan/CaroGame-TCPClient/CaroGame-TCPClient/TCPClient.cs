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
        private readonly object _reqLock = new object();
        private volatile bool _pauseListening = false;


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
                client.ReceiveTimeout = 500;
                stream = client.GetStream();
                stream.ReadTimeout = 500;
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
            try { stream?.Close(); stream?.Dispose(); } catch { }
            try { client?.Close(); } catch { }

            stream = null;
            client = null;
        }

        // --- HÀM 1: Gửi Request và Chờ Phản Hồi (Sync) ---
        // Dùng cho: Login, Register, GetInfo (khi chưa vào bàn chơi)
        private string SendRequest(string request)
        {
            lock (_reqLock)
            {
                try
                {
                    if (!isConnected || stream == null)
                    {
                        if (!Connect()) return "ERROR|Cannot connect to server";
                    }

                    _pauseListening = true;   // ✅ pause trước khi write/read
                    Thread.Sleep(30);         // ✅ nhường cho listener thoát vòng loop (do timeout)

                    byte[] data = Encoding.UTF8.GetBytes(request);
                    stream!.Write(data, 0, data.Length);
                    stream!.Flush();

                    byte[] buffer = new byte[4096];
                    int bytesRead = stream!.Read(buffer, 0, buffer.Length); // read response của request này

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
                finally
                {
                    _pauseListening = false; // ✅ bật lại listener
                }
            }
        }

        private string SendRequestNewConnection(string request)
        {
            try
            {
                using var temp = new TcpClient();
                temp.Connect(serverIP, serverPort);

                using var s = temp.GetStream();
                s.ReadTimeout = 5000;
                s.WriteTimeout = 5000;

                byte[] data = Encoding.UTF8.GetBytes(request);
                s.Write(data, 0, data.Length);
                s.Flush();

                byte[] buffer = new byte[4096];
                int bytesRead = s.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) return "ERROR|Server disconnected";

                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
            catch (Exception ex)
            {
                return $"ERROR|Connection error: {ex.Message}";
            }
        }


        // --- HÀM 2: Gửi Nhanh (Async - Fire & Forget) ---
        // Dùng cho: Trong trận đấu (Move, Chat, Undo)
        public void Send(string data)
        {
            try
            {
                if (isConnected && stream != null)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush(); // Đẩy đi ngay
                }
            }
            catch
            {
                Disconnect();
                OnMessageReceived?.Invoke("DISCONNECT|Send failed");
            }
        }

        // --- HÀM 3: Luồng Lắng Nghe (Core Loop) ---
        // Chạy ngầm liên tục để hứng tin nhắn từ Server
        public void StartListening()
        {
            if (listenerThread != null && listenerThread.IsAlive) return;

            listenerThread = new Thread(() =>
            {
                byte[] buffer = new byte[4096];

                while (isConnected && client != null && stream != null)
                {
                    try
                    {
                        if (_pauseListening)
                        {
                            Thread.Sleep(20);
                            continue;
                        }

                        int bytesRead = stream.Read(buffer, 0, buffer.Length); // có timeout
                        if (bytesRead == 0)
                        {
                            Disconnect();
                            OnMessageReceived?.Invoke("DISCONNECT|Server closed");
                            break;
                        }

                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        OnMessageReceived?.Invoke(message);
                    }
                    catch (IOException)
                    {
                        // timeout -> loop tiếp, không disconnect
                        continue;
                    }
                    catch
                    {
                        Disconnect();
                        OnMessageReceived?.Invoke("DISCONNECT|Connection lost");
                        break;
                    }
                }
            });

            listenerThread.IsBackground = true;
            listenerThread.Start();
        }

        // --- CÁC HÀM NGHIỆP VỤ (API) ---

        public string Register(string username, string password, string email, string birthday)
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

        public string VerifyPassword(string username, string password)
        {
            string hashedPassword = HashUtil.Sha256(password);
            return SendRequestNewConnection($"VERIFY_PASSWORD|{username}|{hashedPassword}");
        }

        public string UpdateEmail(string username, string email)
        {
            return SendRequestNewConnection($"UPDATEEMAIL|{username}|{email}");
        }

        public string UpdateBirthday(string username, string birthday)
        {
            return SendRequestNewConnection($"UPDATEBIRTHDAY|{username}|{birthday}");
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

        // --- CÁC HÀM GAME ---

        public void FindMatch(string username)
        {
            Send($"FIND_MATCH|{username}");
        }

        public void CancelMatch(string username)
        {
            Send($"CANCEL_MATCH|{username}");
        }

        // Trong class TCPClient
        public void CreateRoom(string username)
        {
            Send($"CREATE_ROOM|{username}");
        }

        public void JoinRoom(string username, string roomId)
        {
            Send($"JOIN_ROOM|{username}|{roomId}");
        }

        // Hàm này ít dùng trực tiếp, thường dùng SendPacket
        public void SendMove(int x, int y, int roomId)
        {
            Send($"MOVE|{x}|{y}|{roomId}");
        }

        // Hàm Undo
        public void RequestUndo()
        {
            Send("REQUEST_UNDO");
        }

        public void SendPacket(Packet packet)
        {
            string data = "";
            if (packet.Command == "MOVE")
                // Gửi tọa độ nước đi (Server sẽ tự điền Side)
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