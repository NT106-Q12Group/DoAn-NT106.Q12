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
        private List<TcpClient> Clients; // Danh sách kết nối thô
        private int ConnectionCount;

        // --- CÁC DICTIONARY QUẢN LÝ TRẠNG THÁI (MỚI) ---

        // 1. Danh sách người đang Online (Username -> Socket)
        // Dùng để tìm socket của đối thủ khi cần gửi nước đi
        private static Dictionary<string, TcpClient> OnlineUsers = new Dictionary<string, TcpClient>();

        // 2. Hàng chờ tìm trận (Username -> Socket)
        private static Dictionary<string, TcpClient> WaitingQueue = new Dictionary<string, TcpClient>();

        // 3. Danh sách cặp đấu (Người chơi -> Đối thủ)
        // Dùng để biết ai đang đánh với ai
        private static Dictionary<string, string> ActiveMatches = new Dictionary<string, string>();

        private object lockObj = new object(); // Khóa để tránh lỗi đa luồng

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
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] Waiting for connections...");
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
                    lock (Clients)
                    {
                        Clients.Add(client);
                        ConnectionCount++;
                    }

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

        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = null;
            string clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
            string currentUsername = null; // Lưu username của client này sau khi Login

            try
            {
                stream = client.GetStream();
                byte[] buffer = new byte[4096]; // Tăng buffer lên chút

                while (isRunning && client.Connected)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // Log request (ẩn password)
                    string logRequest = HidePassword(request);
                    Console.WriteLine($"[RECV] {clientIP} ({currentUsername ?? "Guest"}): {logRequest}");

                    // Xử lý Request
                    string response = ProcessRequest(request, client, ref currentUsername);

                    // Nếu có phản hồi (response != null), gửi lại cho Client
                    // Lưu ý: FIND_MATCH và MOVE sẽ trả về null vì chúng xử lý gửi riêng
                    if (!string.IsNullOrEmpty(response))
                    {
                        byte[] responseData = Encoding.UTF8.GetBytes(response);
                        stream.Write(responseData, 0, responseData.Length);
                        stream.Flush(); // Đẩy dữ liệu đi ngay

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
                // Dọn dẹp khi Client ngắt kết nối
                CleanupClient(client, currentUsername);
            }
        }

        // --- HÀM DỌN DẸP KHI NGẮT KẾT NỐI ---
        private void CleanupClient(TcpClient client, string username)
        {
            lock (lockObj)
            {
                if (!string.IsNullOrEmpty(username))
                {
                    // Xóa khỏi danh sách online
                    if (OnlineUsers.ContainsKey(username)) OnlineUsers.Remove(username);

                    // Xóa khỏi hàng chờ nếu đang đợi
                    if (WaitingQueue.ContainsKey(username)) WaitingQueue.Remove(username);

                    // Xử lý nếu đang trong trận đấu (báo đối thủ thắng)
                    if (ActiveMatches.ContainsKey(username))
                    {
                        string opponent = ActiveMatches[username];
                        ActiveMatches.Remove(username);

                        // Báo cho đối thủ biết mình đã thoát
                        SendToPlayer(opponent, "OPPONENT_LEFT|Your opponent disconnected.");

                        // Xóa đối thủ khỏi map trận đấu luôn (để họ quay về trạng thái rảnh)
                        if (ActiveMatches.ContainsKey(opponent)) ActiveMatches.Remove(opponent);
                    }
                }

                lock (Clients) Clients.Remove(client);
                try { client.Close(); } catch { }
            }
            Console.WriteLine($"[INFO] Client {username ?? "Unknown"} disconnected.");
        }

        // --- XỬ LÝ REQUEST (ĐIỀU HƯỚNG) ---
        private string ProcessRequest(string request, TcpClient client, ref string currentUsername)
        {
            try
            {
                string[] parts = request.Split('|');
                if (parts.Length == 0) return "Error|Invalid request";

                string cmd = parts[0].ToUpperInvariant();

                switch (cmd)
                {
                    case "SIGNIN":
                        string result = HandleSignIn(parts);
                        // Nếu đăng nhập thành công, lưu vào danh sách Online
                        if (result.StartsWith("Success"))
                        {
                            currentUsername = parts[1];
                            lock (lockObj)
                            {
                                OnlineUsers[currentUsername] = client;
                            }
                        }
                        return result;

                    case "SIGNUP":
                        return HandleRegister(parts);

                    case "GETPLAYER":
                        return HandleGetPlayer(parts);

                    case "GETEMAIL": // Thêm hỗ trợ lấy email cho ResetPassword
                        return HandleGetEmail(parts);

                    case "UPDATEPASS": // Thêm hỗ trợ đổi pass
                        return HandleUpdatePassword(parts);

                    // --- CÁC LỆNH GAME MỚI ---

                    case "FIND_MATCH":
                        HandleFindMatch(parts[1], client);
                        return null; // Không trả lời ngay, đợi tìm thấy trận mới báo

                    case "CANCEL_MATCH":
                        HandleCancelMatch(parts[1]);
                        return null;

                    case "MOVE": // MOVE|x|y|roomId (ở đây ta dùng map ActiveMatches nên roomId có thể optional)
                        HandleMove(parts, currentUsername);
                        return null;

                    case "SIGNOUT":
                        return HandleSignOut(parts);

                    default:
                        return "Error|Unknown command";
                }
            }
            catch (Exception ex)
            {
                return $"Error|Exception: {ex.Message}";
            }
        }

        // --- LOGIC TÌM TRẬN ---
        private void HandleFindMatch(string username, TcpClient client)
        {
            lock (lockObj)
            {
                // Nếu hàng chờ trống -> Thêm mình vào đợi
                if (WaitingQueue.Count == 0)
                {
                    WaitingQueue[username] = client;
                    Console.WriteLine($"[MATCH] {username} added to queue.");
                }
                else
                {
                    // Lấy người đang đợi ra (Người A)
                    var opponent = WaitingQueue.GetEnumerator();
                    opponent.MoveNext();
                    string oppName = opponent.Current.Key;
                    TcpClient oppClient = opponent.Current.Value;

                    // Xóa A khỏi hàng chờ
                    WaitingQueue.Remove(oppName);

                    // Kiểm tra nếu A trùng tên với mình (lỗi logic client)
                    if (username == oppName) return;

                    // Lưu cặp đấu: A vs B và B vs A
                    ActiveMatches[username] = oppName;
                    ActiveMatches[oppName] = username;

                    // Gửi thông báo START GAME cho cả 2
                    // Format: MATCH_FOUND|TênĐốiThủ|KýHiệu(X hoặc O)
                    SendToPlayer(username, $"MATCH_FOUND|{oppName}|O"); // Người vào sau đánh O (ví dụ)
                    SendToPlayer(oppName, $"MATCH_FOUND|{username}|X"); // Người đợi trước đánh X

                    Console.WriteLine($"[MATCH] Started: {oppName} (X) vs {username} (O)");
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

        // --- LOGIC CHUYỂN TIẾP NƯỚC ĐI (RELAY) ---
        private void HandleMove(string[] parts, string senderName)
        {
            // Format: MOVE|x|y|...
            lock (lockObj)
            {
                if (ActiveMatches.ContainsKey(senderName))
                {
                    string opponentName = ActiveMatches[senderName];
                    string x = parts[1];
                    string y = parts[2];

                    // Gửi nước đi sang cho đối thủ
                    // Format gửi về Client: MOVE|x|y
                    SendToPlayer(opponentName, $"MOVE|{x}|{y}");
                    Console.WriteLine($"[GAME] {senderName} moved ({x},{y}) -> {opponentName}");
                }
            }
        }

        // --- HÀM GỬI DỮ LIỆU ĐẾN MỘT NGƯỜI CHƠI CỤ THỂ ---
        private void SendToPlayer(string username, string message)
        {
            if (OnlineUsers.ContainsKey(username))
            {
                try
                {
                    TcpClient client = OnlineUsers[username];
                    if (client.Connected)
                    {
                        NetworkStream ns = client.GetStream();
                        byte[] data = Encoding.UTF8.GetBytes(message);
                        ns.Write(data, 0, data.Length);
                        ns.Flush(); // Quan trọng
                    }
                }
                catch
                {
                    // Nếu lỗi gửi thì coi như người đó rớt mạng, Cleanup sau
                }
            }
        }

        // --- THÊM ĐOẠN NÀY VÀO CUỐI FILE TCPServer.cs ---

        public void Stop()
        {
            try
            {
                isRunning = false;

                // 1. Dừng lắng nghe kết nối mới
                Server?.Stop();

                // 2. Dọn dẹp các danh sách logic
                lock (lockObj)
                {
                    OnlineUsers.Clear();
                    WaitingQueue.Clear();
                    ActiveMatches.Clear();
                }

                // 3. Đóng tất cả các kết nối Client hiện tại
                lock (Clients)
                {
                    foreach (var client in Clients)
                    {
                        client.Close();
                    }
                    Clients.Clear();
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] Server stopped manually.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error stopping server: {ex.Message}");
            }
        }

        // --- CÁC HÀM XỬ LÝ DATABASE (NHƯ CŨ) ---
        private string HandleSignIn(string[] parts)
        {
            if (parts.Length < 3) return "Error|Missing info";
            var player = PlayerADO.Authenticate(parts[1], parts[2]);
            if (player != null) return $"Success|{player.PlayerName}|{player.Email}|{player.Birthday}";
            return "Error|Invalid username or password";
        }

        private string HandleRegister(string[] parts)
        {
            if (parts.Length < 5) return "Error|Missing info";
            // var p = new Player.Player(parts[1], parts[2], parts[3], parts[4]);
            // Fake logic nếu chưa có SQL, nếu có rồi thì uncomment dòng dưới
            // if (PlayerADO.RegisterPlayer(p)) return "Success|Registered";

            // Code cũ của bạn:
            var newPlayer = new Player.Player(parts[1], parts[2], parts[3], parts[4]);
            if (PlayerADO.RegisterPlayer(newPlayer)) return "Success|Registered successfully.";
            return "Error|Username exists";
        }

        private string HandleGetPlayer(string[] parts)
        {
            if (parts.Length < 2) return "Error|Missing name";
            var p = PlayerADO.GetPlayerByPlayerName(parts[1]);
            if (p != null) return $"Success|{p.PlayerName}|{p.Email}|{p.Birthday}";
            return "Error|Not found";
        }

        private string HandleSignOut(string[] parts)
        {
            return "Success|Signed out";
        }

        // Thêm hàm lấy Email (cho Reset Password)
        private string HandleGetEmail(string[] parts)
        {
            if (parts.Length < 2) return "Error|Missing username";
            // Giả lập hoặc gọi ADO
            var p = PlayerADO.GetPlayerByPlayerName(parts[1]);
            if (p != null && !string.IsNullOrEmpty(p.Email)) return $"Success|{p.Email}";
            return "Error|Email not found";
        }

        // Thêm hàm đổi Pass
        private string HandleUpdatePassword(string[] parts)
        {
            if (parts.Length < 3) return "Error|Missing data";
            // Gọi ADO UpdatePassword(username, newPassHash)
            // if (PlayerADO.UpdatePassword(parts[1], parts[2])) return "Success|Updated";
            return "Success|Updated (Fake)"; // Sửa lại gọi ADO thật nhé
        }

        private string HidePassword(string text)
        {
            if (text.Contains("SIGNIN") || text.Contains("SIGNUP"))
                return text.Split('|')[0] + "|***HIDDEN***";
            return text;
        }
    }
}