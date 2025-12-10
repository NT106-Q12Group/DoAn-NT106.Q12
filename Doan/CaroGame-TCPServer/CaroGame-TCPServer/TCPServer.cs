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
        private List<TcpClient> Clients;
        private int ConnectionCount;

        // --- CÁC DICTIONARY QUẢN LÝ TRẠNG THÁI ---

        // 1. Danh sách người đang Online
        private static Dictionary<string, TcpClient> OnlineUsers = new Dictionary<string, TcpClient>();

        // 2. Hàng chờ tìm trận ngẫu nhiên (Quick Match)
        private static Dictionary<string, TcpClient> WaitingQueue = new Dictionary<string, TcpClient>();

        // 3. [MỚI] Danh sách phòng Custom đang chờ (Key: RoomID, Value: HostClient)
        private static Dictionary<string, TcpClient> CustomWaitingRooms = new Dictionary<string, TcpClient>();

        // 4. Danh sách cặp đấu đang diễn ra
        private static Dictionary<string, string> ActiveMatches = new Dictionary<string, string>();

        // 5. Lưu phe của người chơi trong trận (1=X, 2=O)
        private static Dictionary<string, int> PlayerSides = new Dictionary<string, int>();

        private object lockObj = new object();

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

                    string logRequest = HidePassword(request);
                    Console.WriteLine($"[RECV] {clientIP} ({currentUsername ?? "Guest"}): {logRequest}");

                    string response = ProcessRequest(request, client, ref currentUsername);

                    if (!string.IsNullOrEmpty(response))
                    {
                        byte[] responseData = Encoding.UTF8.GetBytes(response);
                        stream.Write(responseData, 0, responseData.Length);
                        stream.Flush();

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
                CleanupClient(client, currentUsername);
            }
        }

        private void CleanupClient(TcpClient client, string username)
        {
            lock (lockObj)
            {
                if (!string.IsNullOrEmpty(username))
                {
                    if (OnlineUsers.ContainsKey(username)) OnlineUsers.Remove(username);
                    if (WaitingQueue.ContainsKey(username)) WaitingQueue.Remove(username);

                    // [MỚI] Xóa khỏi phòng chờ Custom nếu đang tạo phòng mà thoát
                    // Duyệt ngược dictionary để tìm và xóa
                    foreach (var item in CustomWaitingRooms)
                    {
                        if (item.Value == client)
                        {
                            CustomWaitingRooms.Remove(item.Key);
                            break;
                        }
                    }

                    if (ActiveMatches.ContainsKey(username))
                    {
                        string opponent = ActiveMatches[username];
                        ActiveMatches.Remove(username);

                        SendToPlayer(opponent, "OPPONENT_LEFT|Your opponent disconnected.");

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
                        if (result.StartsWith("Success"))
                        {
                            currentUsername = parts[1];
                            lock (lockObj)
                            {
                                OnlineUsers[currentUsername] = client;
                            }
                        }
                        return result;

                    case "SIGNUP": return HandleRegister(parts);
                    case "GETPLAYER": return HandleGetPlayer(parts);
                    case "GETEMAIL": return HandleGetEmail(parts);
                    case "UPDATEPASS": return HandleUpdatePassword(parts);

                    // --- GAME LOGIC ---
                    case "FIND_MATCH":
                        HandleFindMatch(parts[1], client);
                        return null;

                    case "CANCEL_MATCH":
                        HandleCancelMatch(parts[1]);
                        return null;

                    // [MỚI] TẠO PHÒNG
                    case "CREATE_ROOM":
                        HandleCreateRoom(parts[1], client);
                        return null;

                    // [MỚI] VÀO PHÒNG
                    case "JOIN_ROOM":
                        // JOIN_ROOM | Username | RoomID
                        HandleJoinRoom(parts[1], parts[2], client);
                        return null;

                    case "MOVE":
                        HandleMove(parts, currentUsername);
                        return null;

                    case "CHAT":
                        HandleChat(parts, currentUsername);
                        return null;

                    case "REQUEST_UNDO":
                        HandleUndoRequest(currentUsername);
                        return null;

                    case "SURRENDER":
                        HandleSurrender(currentUsername);
                        return null;

                    case "SIGNOUT": return "Success|Signed out";

                    default:
                        return "Error|Unknown command";
                }
            }
            catch (Exception ex)
            {
                return $"Error|Exception: {ex.Message}";
            }
        }

        // --- [MỚI] XỬ LÝ TẠO PHÒNG ---
        private void HandleCreateRoom(string username, TcpClient client)
        {
            lock (lockObj)
            {
                // Tạo ID ngẫu nhiên 5 số
                Random rnd = new Random();
                string roomId = rnd.Next(10000, 99999).ToString();
                while (CustomWaitingRooms.ContainsKey(roomId)) roomId = rnd.Next(10000, 99999).ToString();

                CustomWaitingRooms.Add(roomId, client);

                // Gửi lại ID cho người tạo
                SendDirect(client, $"ROOM_CREATED|{roomId}");
                Console.WriteLine($"[ROOM] {username} created room {roomId}");
            }
        }

        // --- [MỚI] XỬ LÝ VÀO PHÒNG ---
        private void HandleJoinRoom(string joinerName, string roomId, TcpClient joinerClient)
        {
            lock (lockObj)
            {
                if (CustomWaitingRooms.ContainsKey(roomId))
                {
                    TcpClient hostClient = CustomWaitingRooms[roomId];

                    if (!hostClient.Connected)
                    {
                        CustomWaitingRooms.Remove(roomId);
                        SendDirect(joinerClient, "JOIN_FAIL|Phòng này chủ phòng đã mất kết nối.");
                        return;
                    }

                    // Tìm tên chủ phòng
                    string hostName = GetUsernameBySocket(hostClient);
                    if (hostName == null) { SendDirect(joinerClient, "JOIN_FAIL|Lỗi xác thực."); return; }
                    if (hostName == joinerName) { SendDirect(joinerClient, "JOIN_FAIL|Không thể tự vào phòng mình."); return; }

                    // Xóa phòng chờ
                    CustomWaitingRooms.Remove(roomId);

                    // Thiết lập trận đấu
                    ActiveMatches[hostName] = joinerName;
                    ActiveMatches[joinerName] = hostName;
                    PlayerSides[hostName] = 1; // Chủ phòng là X
                    PlayerSides[joinerName] = 2; // Khách là O

                    Console.WriteLine($"[MATCH] Custom Room {roomId}: {hostName} vs {joinerName}");

                    // Gửi lệnh vào game
                    SendDirect(hostClient, $"MATCH_FOUND|{joinerName}|X");
                    SendDirect(joinerClient, $"MATCH_FOUND|{hostName}|O");
                }
                else
                {
                    SendDirect(joinerClient, "JOIN_FAIL|Phòng không tồn tại.");
                }
            }
        }

        private string GetUsernameBySocket(TcpClient client)
        {
            foreach (var item in OnlineUsers) if (item.Value == client) return item.Key;
            return null;
        }

        private void HandleSurrender(string sender)
        {
            lock (lockObj)
            {
                if (ActiveMatches.ContainsKey(sender))
                {
                    string opponent = ActiveMatches[sender];
                    SendToPlayer(opponent, "OPPONENT_LEFT|Đối thủ đã đầu hàng. Bạn thắng!");
                    ActiveMatches.Remove(sender);
                    if (ActiveMatches.ContainsKey(opponent)) ActiveMatches.Remove(opponent);
                    Console.WriteLine($"[GAME] {sender} surrendered against {opponent}");
                }
            }
        }

        private void SendDirect(TcpClient client, string msg)
        {
            try
            {
                if (client != null && client.Connected)
                {
                    NetworkStream s = client.GetStream();
                    byte[] data = Encoding.UTF8.GetBytes(msg);
                    s.Write(data, 0, data.Length);
                    s.Flush();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] SendDirect: {ex.Message}");
            }
        }

        private void HandleFindMatch(string username, TcpClient client)
        {
            lock (lockObj)
            {
                if (WaitingQueue.ContainsKey(username))
                {
                    WaitingQueue[username] = client;
                }

                string opponentName = null;
                foreach (var waiter in WaitingQueue)
                {
                    if (waiter.Key != username)
                    {
                        opponentName = waiter.Key;
                        break;
                    }
                }

                if (opponentName != null)
                {
                    TcpClient opponentClient = WaitingQueue[opponentName];

                    if (!opponentClient.Connected)
                    {
                        WaitingQueue.Remove(opponentName);
                        if (!WaitingQueue.ContainsKey(username))
                            WaitingQueue.Add(username, client);
                        return;
                    }

                    WaitingQueue.Remove(opponentName);
                    if (WaitingQueue.ContainsKey(username)) WaitingQueue.Remove(username);

                    ActiveMatches[username] = opponentName;
                    ActiveMatches[opponentName] = username;
                    PlayerSides[opponentName] = 1;
                    PlayerSides[username] = 2;

                    Console.WriteLine($"[MATCH] Found: {opponentName} vs {username}");

                    SendDirect(client, $"MATCH_FOUND|{opponentName}|O");
                    SendDirect(opponentClient, $"MATCH_FOUND|{username}|X");
                }
                else
                {
                    if (!WaitingQueue.ContainsKey(username))
                    {
                        WaitingQueue.Add(username, client);
                        Console.WriteLine($"[MATCH] {username} added to queue.");
                    }
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

        private void HandleMove(string[] parts, string sender)
        {
            lock (lockObj)
            {
                if (ActiveMatches.ContainsKey(sender))
                {
                    string opponent = ActiveMatches[sender];
                    string x = parts[1];
                    string y = parts[2];
                    int side = PlayerSides.ContainsKey(sender) ? PlayerSides[sender] : -1;

                    SendToPlayer(opponent, $"MOVE|{x}|{y}|{side}");
                    SendToPlayer(sender, $"MOVE|{x}|{y}|{side}");

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

        private void SendToPlayer(string username, string msg)
        {
            if (OnlineUsers.ContainsKey(username))
            {
                try
                {
                    TcpClient c = OnlineUsers[username];
                    if (c != null && c.Connected)
                    {
                        NetworkStream s = c.GetStream();
                        byte[] data = Encoding.UTF8.GetBytes(msg);
                        s.Write(data, 0, data.Length);
                        s.Flush();
                    }
                }
                catch { }
            }
        }

        public void Stop()
        {
            try
            {
                isRunning = false;
                Server?.Stop();
                lock (lockObj)
                {
                    OnlineUsers.Clear();
                    WaitingQueue.Clear();
                    CustomWaitingRooms.Clear();
                    ActiveMatches.Clear();
                }
                lock (Clients)
                {
                    foreach (var client in Clients) client.Close();
                    Clients.Clear();
                }
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] Server stopped manually.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error stopping server: {ex.Message}");
            }
        }

        // --- CÁC HÀM DB ---
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
        private string HandleGetEmail(string[] parts)
        {
            if (parts.Length < 2) return "Error|Missing username";
            var p = PlayerADO.GetPlayerByPlayerName(parts[1]);
            if (p != null && !string.IsNullOrEmpty(p.Email)) return $"Success|{p.Email}";
            return "Error|Email not found";
        }
        private string HandleUpdatePassword(string[] parts)
        {
            if (parts.Length < 3) return "Error|Missing data";
            bool ok = PlayerADO.UpdatePassword(parts[1], parts[2]);
            if (ok) return "Success|Updated";
            return "Error|Update failed";
        }
        private string HidePassword(string text)
        {
            if (text.Contains("SIGNIN") || text.Contains("SIGNUP")) return text.Split('|')[0] + "|***HIDDEN***";
            return text;
        }
    }
}