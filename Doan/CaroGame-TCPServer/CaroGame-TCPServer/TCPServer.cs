using System;
using System.Collections.Generic;
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
        private List<TcpClient> Clients;
        private int ConnectionCount;

        private object lockObj = new object();

        // --- QUẢN LÝ TRẠNG THÁI ---

        // 1. Map Username -> Socket (Để gửi tin)
        private static Dictionary<string, TcpClient> OnlineUsers = new Dictionary<string, TcpClient>();

        // 2. Hàng chờ tìm trận
        private static Dictionary<string, TcpClient> WaitingQueue = new Dictionary<string, TcpClient>();

        // 3. Map Người -> Đối thủ (Để biết ai đang đánh với ai)
        private static Dictionary<string, string> ActiveMatches = new Dictionary<string, string>();

        // 4. [MỚI] Map Người -> Phe (0: X, 1: O)
        // Để khi forward nước đi, Server điền thêm thông tin Side vào gói tin
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
            string currentUsername = null;

            try
            {
                stream = client.GetStream();
                byte[] buffer = new byte[4096];

                while (isRunning && client.Connected)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // Log nhẹ để debug
                    if (!request.StartsWith("MOVE")) // Đỡ spam log khi di chuyển nhiều
                        Console.WriteLine($"[RECV] {currentUsername ?? clientIP}: {request}");

                    string response = ProcessRequest(request, client, ref currentUsername);

                    if (!string.IsNullOrEmpty(response))
                    {
                        SendResponse(stream, response);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {currentUsername}: {ex.Message}");
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

        // --- XỬ LÝ LỆNH TỪ CLIENT ---
        private string ProcessRequest(string request, TcpClient client, ref string currentUsername)
        {
            string[] parts = request.Split('|');
            string cmd = parts[0];

            switch (cmd)
            {
                case "SIGNIN":
                    string loginResult = HandleSignIn(parts);
                    if (loginResult.StartsWith("Success"))
                    {
                        currentUsername = parts[1];
                        lock (lockObj) OnlineUsers[currentUsername] = client;
                    }
                    return loginResult;

                case "SIGNUP": return HandleRegister(parts);
                case "GETPLAYER": return HandleGetPlayer(parts);

                // --- GAME LOGIC ---
                case "FIND_MATCH":
                    HandleFindMatch(parts[1], client);
                    return null; // Không trả lời ngay

                case "CANCEL_MATCH":
                    HandleCancelMatch(parts[1]);
                    return null;

                case "MOVE":
                    HandleMove(parts, currentUsername);
                    return null; // Đã forward cho đối thủ

                case "CHAT":
                    HandleChat(parts, currentUsername);
                    return null;

                case "REQUEST_UNDO": // [FIX] Xử lý Undo
                    HandleUndoRequest(currentUsername);
                    return null;

                case "SIGNOUT":
                    return "Success|Signed out";

                default: return "Error|Unknown command";
            }
        }

        // --- LOGIC GAME CHI TIẾT ---

        private void HandleFindMatch(string username, TcpClient client)
        {
            lock (lockObj)
            {
                if (WaitingQueue.Count == 0)
                {
                    // Người đầu tiên vào hàng chờ
                    WaitingQueue[username] = client;
                    Console.WriteLine($"[MATCH] {username} waiting...");
                }
                else
                {
                    // Có người đang đợi -> Ghép cặp
                    var opponent = WaitingQueue.GetEnumerator();
                    opponent.MoveNext();
                    string oppName = opponent.Current.Key;

                    if (username == oppName) return; // Tránh tự tìm chính mình

                    WaitingQueue.Remove(oppName);

                    // Lưu trạng thái trận đấu
                    ActiveMatches[username] = oppName;
                    ActiveMatches[oppName] = username;

                    // [QUAN TRỌNG] Phân định phe: Người đợi (Host) là X(0), Người mới vào là O(1)
                    PlayerSides[oppName] = 0;  // X
                    PlayerSides[username] = 1; // O

                    Console.WriteLine($"[MATCH] Start: {oppName}(X) vs {username}(O)");

                    // Gửi thông báo bắt đầu
                    // Format: MATCH_FOUND | TênĐịch | PheCủaMình(0/1) | PheChữ(X/O)
                    SendToPlayer(oppName, $"MATCH_FOUND|{username}|0|X");
                    SendToPlayer(username, $"MATCH_FOUND|{oppName}|1|O");
                }
            }
        }

        private void HandleMove(string[] parts, string sender)
        {
            // Client gửi: MOVE | x | y
            lock (lockObj)
            {
                if (ActiveMatches.ContainsKey(sender))
                {
                    string opponent = ActiveMatches[sender];
                    string x = parts[1];
                    string y = parts[2];

                    // Lấy phe của người gửi để báo cho đối thủ biết nên vẽ X hay O
                    int senderSide = PlayerSides.ContainsKey(sender) ? PlayerSides[sender] : -1;

                    // Chuyển tiếp tới đối thủ
                    // Server gửi: MOVE | x | y | SideCủaNgườiVừaĐi
                    SendToPlayer(opponent, $"MOVE|{x}|{y}|{senderSide}");

                    // Server gửi lại cho chính người đi (để xác nhận là Server đã nhận -> Client mới vẽ)
                    // (Theo logic Client mới: Gửi đi -> Chờ Server phản hồi -> Mới vẽ)
                    SendToPlayer(sender, $"MOVE|{x}|{y}|{senderSide}");

                    // Cập nhật lượt đi tiếp theo
                    SendToPlayer(sender, $"NEXT_TURN|{opponent}");
                    SendToPlayer(opponent, $"NEXT_TURN|{opponent}");
                }
            }
        }

        private void HandleUndoRequest(string requestor)
        {
            lock (lockObj)
            {
                if (ActiveMatches.ContainsKey(requestor))
                {
                    string opponent = ActiveMatches[requestor];

                    // Logic đơn giản: Cho phép luôn (hoặc có thể gửi yêu cầu hỏi ý kiến đối thủ)
                    // Ở đây ta làm theo hướng dẫn: Server gửi SUCCESS cho cả 2

                    SendToPlayer(requestor, "UNDO_SUCCESS");
                    SendToPlayer(opponent, "UNDO_SUCCESS");

                    // Sau khi Undo 2 nước (về lượt người xin undo), thì lượt là của requestor
                    SendToPlayer(requestor, $"NEXT_TURN|{requestor}");
                    SendToPlayer(opponent, $"NEXT_TURN|{requestor}");

                    Console.WriteLine($"[GAME] Undo granted for {requestor}");
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
                    string msg = parts[1];
                    SendToPlayer(opponent, $"CHAT|{msg}");
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
                }
            }
        }

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
                        ActiveMatches.Remove(opponent); // Xóa trận đấu

                        // Báo đối thủ thắng
                        SendToPlayer(opponent, "OPPONENT_LEFT|Win");
                    }
                }
                if (Clients.Contains(client)) Clients.Remove(client);
            }
            try { client.Close(); } catch { }
        }

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
                catch { /* Client rớt mạng, để Cleanup lo */ }
            }
        }

        // --- CÁC HÀM GIẢ LẬP DB (HOẶC GỌI SQL THẬT TẠI ĐÂY) ---
        private string HandleSignIn(string[] parts)
        {
            // Thay bằng logic SQL của bạn
            // Ví dụ: return PlayerADO.Login(parts[1], parts[2]);
            return "Success|User|Email|Dob";
        }

        private string HandleRegister(string[] parts)
        {
            // Thay bằng logic SQL
            return "Success|Registered";
        }

        private string HandleGetPlayer(string[] parts)
        {
            return "Success|User|Email|Dob";
        }
    }
}