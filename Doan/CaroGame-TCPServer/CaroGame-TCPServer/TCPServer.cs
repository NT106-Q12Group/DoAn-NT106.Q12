using CaroGame_TCPServer.Player;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CaroGame_TCPServer
{
    public class TCPServer
    {
        private TcpListener Server;
        private int port;
        private bool isRunning;

        // Quản lý danh sách kết nối
        private List<TcpClient> Clients;

        // --- LOGIC GHÉP CẶP ---
        // Lưu thông tin người đang đợi: Key = Username, Value = ClientSocket
        private static Dictionary<string, TcpClient> WaitingQueue = new Dictionary<string, TcpClient>();
        private object queueLock = new object();

        public TCPServer(int serverPort)
        {
            port = serverPort;
            Clients = new List<TcpClient>();
            isRunning = false;
        }

        public void StartServer()
        {
            try
            {
                Server = new TcpListener(IPAddress.Any, port);
                Server.Start();
                isRunning = true;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] Server is running on port {port}.");
                Console.ResetColor();

                Thread listenThread = new Thread(ListenForClients);
                listenThread.IsBackground = true;
                listenThread.Start();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] StartServer: {ex.Message}");
                Console.ResetColor();
            }
        }

        private void ListenForClients()
        {
            while (isRunning)
            {
                try
                {
                    TcpClient client = Server.AcceptTcpClient();
                    lock (Clients) { Clients.Add(client); }

                    string clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] New connection: {clientIP}");

                    Thread clientThread = new Thread(() => HandleClient(client));
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }
                catch (SocketException) { break; }
                catch (Exception ex) { Console.WriteLine($"[ERROR] AcceptClient: {ex.Message}"); }
            }
        }

        // --- HÀM XỬ LÝ CHÍNH (ĐÃ FIX READ/WRITE) ---
        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = null;
            StreamReader reader = null;
            StreamWriter writer = null;
            string clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            string currentUsername = null; // Lưu tên người dùng của socket này

            try
            {
                stream = client.GetStream();
                // Dùng UTF8 để đọc tiếng Việt và AutoFlush để gửi ngay lập tức
                reader = new StreamReader(stream, Encoding.UTF8);
                writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

                while (isRunning && client.Connected)
                {
                    // 1. Đọc tin nhắn (Chờ đến khi có \n)
                    string request = reader.ReadLine();
                    if (request == null) break; // Client ngắt kết nối

                    // Log request (ẩn pass)
                    string logRequest = HidePassword(request);
                    Console.WriteLine($"[RECV] {clientIP}: {logRequest}");

                    // 2. Xử lý logic và lấy phản hồi
                    string response = ProcessRequest(request, client, ref currentUsername);

                    // 3. Gửi phản hồi (Kèm \n để Client không bị treo)
                    if (!string.IsNullOrEmpty(response))
                    {
                        writer.WriteLine(response);
                        string logResponse = HidePassword(response);
                        Console.WriteLine($"[SEND] {clientIP}: {logResponse}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Client {clientIP}: {ex.Message}");
            }
            finally
            {
                // Dọn dẹp khi ngắt kết nối
                CleanupClient(client, currentUsername);
            }
        }

        private void CleanupClient(TcpClient client, string username)
        {
            // Xóa khỏi hàng chờ tìm trận nếu đang chờ
            if (!string.IsNullOrEmpty(username))
            {
                lock (queueLock)
                {
                    if (WaitingQueue.ContainsKey(username))
                    {
                        WaitingQueue.Remove(username);
                        Console.WriteLine($"[INFO] Removed {username} from waiting queue.");
                    }
                }
            }

            lock (Clients) { Clients.Remove(client); }
            client.Close();
            Console.WriteLine($"[INFO] Client disconnected.");
        }

        // --- XỬ LÝ LOGIC (Login, Register, Matchmaking) ---
        private string ProcessRequest(string request, TcpClient client, ref string currentUsername)
        {
            try
            {
                // Tách chuỗi bằng cả '|' và ';' để hỗ trợ cả 2 code cũ và mới
                string[] parts = request.Split(new char[] { '|', ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) return "Error|Invalid request";

                string cmd = parts[0].ToUpperInvariant();

                switch (cmd)
                {
                    case "SIGNIN":
                        string loginResult = HandleSignIn(parts);
                        if (loginResult.StartsWith("Success"))
                        {
                            // Lưu lại username để dùng cho matchmaking sau này
                            currentUsername = parts[1];
                        }
                        return loginResult;

                    case "SIGNUP":
                        return HandleRegister(parts);

                    case "GETPLAYER":
                        return HandleGetPlayer(parts);

                    // --- LOGIC MỚI: TÌM TRẬN ---
                    case "FIND_MATCH":
                        return HandleFindMatch(parts, client);

                    case "CANCEL_MATCH":
                        return HandleCancelMatch(parts);

                    // --- LOGIC MỚI: ĐÁNH CỜ & CHAT ---
                    // Server đóng vai trò trung chuyển (Relay)
                    case "MOVE":
                    case "CHAT":
                    case "SURRENDER":
                        // Logic này cần xử lý phòng (Room) phức tạp hơn.
                        // Tạm thời trả về null để không lỗi
                        return null;

                    case "SIGNOUT":
                        return "Success|Signed out";

                    default:
                        return "Error|Unknown command";
                }
            }
            catch (Exception ex)
            {
                return $"Error|Exception: {ex.Message}";
            }
        }

        // --- XỬ LÝ GHÉP CẶP (MATCHMAKING) ---
        private string HandleFindMatch(string[] parts, TcpClient playerClient)
        {
            // Cấu trúc: FIND_MATCH;Username
            if (parts.Length < 2) return "Error;Missing username";
            string username = parts[1];

            lock (queueLock)
            {
                // 1. Nếu hàng chờ đang trống -> Thêm mình vào đợi
                if (WaitingQueue.Count == 0)
                {
                    WaitingQueue[username] = playerClient;
                    Console.WriteLine($"[MATCH] {username} added to queue. Waiting...");
                    return null; // Không trả lời ngay, đợi tìm thấy đối thủ
                }
                else
                {
                    // 2. Nếu có người đang đợi -> Lấy người đó ra ghép cặp
                    // Lấy người đầu tiên trong Dictionary
                    var opponent = WaitingQueue.GetEnumerator();
                    opponent.MoveNext();
                    string oppName = opponent.Current.Key;
                    TcpClient oppClient = opponent.Current.Value;

                    // Kiểm tra xem đối thủ còn kết nối không
                    if (!oppClient.Connected)
                    {
                        WaitingQueue.Remove(oppName);
                        WaitingQueue[username] = playerClient; // Lại phải đợi tiếp
                        return null;
                    }

                    // Xóa đối thủ khỏi hàng chờ
                    WaitingQueue.Remove(oppName);

                    // Tạo Room ID ngẫu nhiên
                    int roomId = new Random().Next(1000, 9999);

                    // --- GỬI TIN NHẮN CHO ĐỐI THỦ (NGƯỜI ĐỢI TRƯỚC) ---
                    try
                    {
                        StreamWriter oppWriter = new StreamWriter(oppClient.GetStream(), Encoding.UTF8) { AutoFlush = true };
                        // Gửi: MATCH_FOUND;TênMình;RoomID
                        oppWriter.WriteLine($"MATCH_FOUND;{username};{roomId}");
                        Console.WriteLine($"[MATCH] Matched {oppName} vs {username}");
                    }
                    catch
                    {
                        // Nếu gửi lỗi thì coi như người kia rớt mạng
                        WaitingQueue[username] = playerClient;
                        return null;
                    }

                    // --- TRẢ VỀ TIN NHẮN CHO MÌNH (NGƯỜI VÀO SAU) ---
                    // Trả về: MATCH_FOUND;TênĐốiThủ;RoomID
                    return $"MATCH_FOUND;{oppName};{roomId}";
                }
            }
        }

        private string HandleCancelMatch(string[] parts)
        {
            string username = parts[1];
            lock (queueLock)
            {
                if (WaitingQueue.ContainsKey(username))
                {
                    WaitingQueue.Remove(username);
                    Console.WriteLine($"[MATCH] {username} cancelled matchmaking.");
                }
            }
            return null; // Không cần phản hồi
        }

        // --- CÁC HÀM CŨ GIỮ NGUYÊN (NHƯNG ĐÃ CLEAN CODE) ---

        private string HandleSignIn(string[] parts)
        {
            if (parts.Length < 3) return "Error|Missing info";
            string u = parts[1], p = parts[2];

            // Giả lập logic Authenticate (Thay bằng PlayerADO của bạn)
            var player = PlayerADO.Authenticate(u, p);

            if (player != null)
                return $"Success|{player.PlayerName}|{player.Email}|{player.Birthday}";
            else
                return "Error|Login failed";
        }

        private string HandleRegister(string[] parts)
        {
            if (parts.Length < 5) return "Error|Missing info";
            var p = new Player.Player(parts[1], parts[2], parts[3], parts[4]);

            // Giả lập logic Register
            if (PlayerADO.RegisterPlayer(p)) return "Success|Registered";
            else return "Error|Username exists";
        }

        private string HandleGetPlayer(string[] parts)
        {
            if (parts.Length < 2) return "Error|Missing name";
            var p = PlayerADO.GetPlayerByPlayerName(parts[1]);
            if (p != null) return $"Success|{p.PlayerName}|{p.Email}|{p.Birthday}";
            return "Error|Not found";
        }

        private string HidePassword(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            if (text.Contains("SIGNIN") || text.Contains("SIGNUP"))
                return text.Split('|')[0] + "|***HIDDEN***";
            return text;
        }

        public void Stop()
        {
            isRunning = false;
            Server?.Stop();
        }
    }
}