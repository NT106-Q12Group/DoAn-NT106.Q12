using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

// Nếu bạn có namespace Database, hãy giữ lại using CaroGame_TCPServer.Player;
// Ở đây mình comment lại để code chạy được độc lập (Mock Mode)
// using CaroGame_TCPServer.Player; 

namespace CaroGame_TCPServer
{
    public class TCPServer
    {
        private TcpListener Server;
        private int port;
        private bool isRunning;
        private List<TcpClient> Clients;
        private int ConnectionCount;

        private object lockObj = new object();

        // --- CÁC DICTIONARY QUẢN LÝ TRẠNG THÁI ---

        // 1. Danh sách người đang Online (Username -> Socket)
        private static Dictionary<string, TcpClient> OnlineUsers = new Dictionary<string, TcpClient>();

        // 2. Hàng chờ tìm trận (Username -> Socket)
        private static Dictionary<string, TcpClient> WaitingQueue = new Dictionary<string, TcpClient>();

        // 3. Danh sách cặp đấu (Người chơi -> Đối thủ)
        private static Dictionary<string, string> ActiveMatches = new Dictionary<string, string>();

        // 4. [MỚI] Danh sách Phe (Username -> 0 hoặc 1) | 0: X, 1: O
        private static Dictionary<string, int> PlayerSides = new Dictionary<string, int>();

        public TCPServer(int serverPort)
        {
            port = serverPort;
            Clients = new List<TcpClient>();
            isRunning = false;
            ConnectionCount = 0;
        }

        public void StartServer()
        {
            try
            {
                Server = new TcpListener(IPAddress.Any, port);
                Server.Start();
                isRunning = true;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] Server running on port {port}.");
                Console.ResetColor();

                Thread listenThread = new Thread(ListenForClients);
                listenThread.IsBackground = true;
                listenThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] StartServer: {ex.Message}");
            }
        }

        public void Stop()
        {
            isRunning = false;
            Server?.Stop();
            lock (lockObj)
            {
                OnlineUsers.Clear();
                WaitingQueue.Clear();
                ActiveMatches.Clear();
                PlayerSides.Clear();
            }

            // Đóng kết nối clients
            lock (Clients)
            {
                foreach (var c in Clients) c.Close();
                Clients.Clear();
            }
            Console.WriteLine("[INFO] Server stopped.");
        }

        private void ListenForClients()
        {
            while (isRunning)
            {
                try
                {
                    TcpClient client = Server.AcceptTcpClient();
                    lock (Clients) Clients.Add(client);

                    Thread clientThread = new Thread(() => HandleClient(client));
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }
                catch { break; }
            }
        }

        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = null;
            string clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            string currentUsername = null; // Username hiện tại của socket này

            try
            {
                stream = client.GetStream();
                byte[] buffer = new byte[4096];

                while (isRunning && client.Connected)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // Log request (trừ lệnh MOVE để đỡ spam)
                    if (!request.StartsWith("MOVE"))
                        Console.WriteLine($"[RECV] {currentUsername ?? clientIP}: {HidePassword(request)}");

                    // Xử lý Request
                    string response = ProcessRequest(request, client, ref currentUsername);

                    // Gửi phản hồi (nếu có)
                    if (!string.IsNullOrEmpty(response))
                    {
                        SendResponse(stream, response);
                        if (!response.StartsWith("MOVE")) // Log response
                            Console.WriteLine($"[SEND] {currentUsername ?? clientIP}: {HidePassword(response)}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {currentUsername ?? clientIP}: {ex.Message}");
            }
            finally
            {
                CleanupClient(client, currentUsername);
            }
        }

        private void SendResponse(NetworkStream stream, string response)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(response);
                stream.Write(data, 0, data.Length);
                stream.Flush();
            }
            catch { }
        }

        // --- BỘ ĐIỀU HƯỚNG LỆNH ---
        private string ProcessRequest(string request, TcpClient client, ref string currentUsername)
        {
            string[] parts = request.Split('|');
            if (parts.Length == 0) return "Error|Invalid";

            string cmd = parts[0].ToUpper();

            switch (cmd)
            {
                case "SIGNIN":
                    string loginResult = HandleSignIn(parts);
                    if (loginResult.StartsWith("Success"))
                    {
                        // [FIX] Cập nhật username ngay khi đăng nhập thành công
                        currentUsername = parts[1];
                        lock (lockObj) OnlineUsers[currentUsername] = client;
                    }
                    return loginResult;

                case "SIGNUP": return HandleRegister(parts);
                case "GETPLAYER": return HandleGetPlayer(parts);
                case "GETEMAIL": return HandleGetEmail(parts);
                case "UPDATEPASS": return HandleUpdatePassword(parts);

                // --- GAME LOGIC ---
                case "FIND_MATCH":
                    HandleFindMatch(parts[1], client);
                    return null; // Xử lý bất đồng bộ, không return ngay

                case "CANCEL_MATCH":
                    HandleCancelMatch(parts[1]);
                    return null;

                case "MOVE":
                    HandleMove(parts, currentUsername);
                    return null; // Forward cho các bên, không return text

                case "CHAT":
                    HandleChat(parts, currentUsername);
                    return null;

                case "REQUEST_UNDO":
                    HandleUndoRequest(currentUsername);
                    return null;

                case "SIGNOUT": return "Success|Signed out";

                default: return "Error|Unknown command";
            }
        }

        // --- LOGIC TÌM TRẬN & GHÉP CẶP ---
        private void HandleFindMatch(string username, TcpClient client)
        {
            lock (lockObj)
            {
                // Nếu hàng chờ trống
                if (WaitingQueue.Count == 0)
                {
                    WaitingQueue[username] = client;
                    Console.WriteLine($"[MATCH] {username} added to queue.");
                }
                else
                {
                    // Lấy người đang đợi
                    var opponent = WaitingQueue.GetEnumerator();
                    opponent.MoveNext();
                    string oppName = opponent.Current.Key;

                    // Tránh trường hợp client lỗi gửi cùng 1 tên
                    if (username == oppName) return;

                    // Xóa khỏi hàng chờ
                    WaitingQueue.Remove(oppName);

                    // Thiết lập đối thủ
                    ActiveMatches[username] = oppName;
                    ActiveMatches[oppName] = username;

                    // [QUAN TRỌNG] Thiết lập Phe (0: X, 1: O)
                    PlayerSides[oppName] = 0;  // Người đợi trước là X (đi trước)
                    PlayerSides[username] = 1; // Người vào sau là O (đi sau)

                    Console.WriteLine($"[MATCH] START: {oppName} (X) vs {username} (O)");

                    // Gửi thông báo bắt đầu cho cả 2
                    // Format: MATCH_FOUND | TênĐịch | PheCủaMình(0/1) | KýHiệu(X/O)
                    SendToPlayer(oppName, $"MATCH_FOUND|{username}|0|X");
                    SendToPlayer(username, $"MATCH_FOUND|{oppName}|1|O");
                }
            }
        }

        private void HandleCancelMatch(string username)
        {
            lock (lockObj)
            {
                if (WaitingQueue.ContainsKey(username))
                {
                    WaitingQueue.Remove(username);
                    Console.WriteLine($"[MATCH] {username} cancelled search.");
                }
            }
        }

        // --- LOGIC XỬ LÝ NƯỚC ĐI ---
        private void HandleMove(string[] parts, string sender)
        {
            // Format: MOVE | x | y
            lock (lockObj)
            {
                if (ActiveMatches.ContainsKey(sender))
                {
                    string opponent = ActiveMatches[sender];
                    string x = parts[1];
                    string y = parts[2];

                    // Lấy phe của người gửi (để Client biết vẽ X hay O)
                    int side = PlayerSides.ContainsKey(sender) ? PlayerSides[sender] : -1;

                    // 1. Gửi cho Đối thủ (để vẽ nước đi của địch)
                    SendToPlayer(opponent, $"MOVE|{x}|{y}|{side}");

                    // 2. Gửi lại cho Người đánh (để xác nhận Server đã nhận -> Vẽ nước đi của mình)
                    SendToPlayer(sender, $"MOVE|{x}|{y}|{side}");

                    // 3. (Tùy chọn) Gửi lệnh NEXT_TURN để quản lý lượt chặt chẽ
                    // Ví dụ: Báo cho cả 2 biết bây giờ đến lượt Opponent đi
                    // SendToPlayer(sender, $"NEXT_TURN|{opponent}");
                    // SendToPlayer(opponent, $"NEXT_TURN|{opponent}");

                    Console.WriteLine($"[GAME] {sender} ({x},{y}) -> {opponent}");
                }
            }
        }

        private void HandleUndoRequest(string sender)
        {
            lock (lockObj)
            {
                if (ActiveMatches.ContainsKey(sender))
                {
                    string opponent = ActiveMatches[sender];

                    // Chấp nhận Undo luôn (hoặc logic hỏi ý kiến tùy bạn)
                    // Gửi lệnh UNDO_SUCCESS cho cả 2 client để xóa nước cờ
                    SendToPlayer(sender, "UNDO_SUCCESS");
                    SendToPlayer(opponent, "UNDO_SUCCESS");

                    Console.WriteLine($"[GAME] Undo accepted for {sender}");
                }
            }
        }

        private void HandleChat(string[] parts, string sender)
        {
            lock (lockObj)
            {
                if (ActiveMatches.ContainsKey(sender))
                {
                    string opponent = ActiveMatches[sender];
                    SendToPlayer(opponent, $"CHAT|{parts[1]}");
                }
            }
        }

        // --- HÀM GỬI TIN NHẮN RIÊNG ---
        private void SendToPlayer(string username, string msg)
        {
            if (OnlineUsers.ContainsKey(username))
            {
                try
                {
                    TcpClient c = OnlineUsers[username];
                    if (c.Connected)
                    {
                        NetworkStream s = c.GetStream();
                        byte[] data = Encoding.UTF8.GetBytes(msg);
                        s.Write(data, 0, data.Length);
                        s.Flush();
                    }
                }
                catch { /* Client rớt mạng, Cleanup sẽ xử lý */ }
            }
        }

        // --- CLEANUP ---
        private void CleanupClient(TcpClient client, string username)
        {
            lock (lockObj)
            {
                if (!string.IsNullOrEmpty(username))
                {
                    OnlineUsers.Remove(username);
                    WaitingQueue.Remove(username);
                    PlayerSides.Remove(username);

                    if (ActiveMatches.ContainsKey(username))
                    {
                        string opponent = ActiveMatches[username];
                        ActiveMatches.Remove(username);
                        ActiveMatches.Remove(opponent); // Giải tán cặp đấu

                        SendToPlayer(opponent, "OPPONENT_LEFT|Your opponent disconnected.");
                    }
                }
                if (Clients.Contains(client)) Clients.Remove(client);
            }
            try { client.Close(); } catch { }
            Console.WriteLine($"[INFO] Client {username ?? "Unknown"} disconnected.");
        }

        // --- CÁC HÀM XỬ LÝ ACCOUNT (MOCK MODE - ĐỂ TEST KHÔNG CẦN SQL) ---

        private string HandleSignIn(string[] parts)
        {
            if (parts.Length < 2) return "Error|Missing info";

            // [FIX QUAN TRỌNG]: Trả về đúng tên người dùng gửi lên
            // Thay vì "Success|User...", ta trả về "Success|{parts[1]}..."
            // parts[1] chính là username người dùng nhập vào Client
            return $"Success|{parts[1]}|test@gmail.com|01-01-2000";
        }

        private string HandleRegister(string[] parts)
        {
            // Mock: Luôn đăng ký thành công
            return "Success|Registered successfully (Mock)";
        }

        private string HandleGetPlayer(string[] parts)
        {
            // Mock: Trả về thông tin giả
            if (parts.Length < 2) return "Error|Missing name";
            return $"Success|{parts[1]}|test@gmail.com|01-01-2000";
        }

        private string HandleGetEmail(string[] parts) => $"Success|{parts[1]}@gmail.com";

        private string HandleUpdatePassword(string[] parts) => "Success|Updated";

        private string HidePassword(string text)
        {
            if (text.Contains("SIGNIN") || text.Contains("SIGNUP"))
                try { return text.Split('|')[0] + "|***HIDDEN***"; } catch { return text; }
            return text;
        }
    }
}