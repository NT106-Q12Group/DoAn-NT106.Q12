using CaroGame_TCPServer.Player;
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

        private static Dictionary<string, TcpClient> OnlineUsers = new Dictionary<string, TcpClient>();
        private static Dictionary<string, TcpClient> WaitingQueue = new Dictionary<string, TcpClient>(); // Quick Match
        private static Dictionary<string, string> ActiveMatches = new Dictionary<string, string>();
        private static Dictionary<string, int> PlayerSides = new Dictionary<string, int>(); // 1=X, 2=O

        private static HashSet<string> RematchWaiting = new HashSet<string>();
        private static HashSet<string> ResetWaiting = new HashSet<string>();

        // ===== Custom Room State =====
        private class CustomRoomState
        {
            public string RoomId;
            public string HostName;
            public string GuestName;
            public TcpClient HostClient;
            public TcpClient GuestClient;
            public bool HostReady;
            public bool GuestReady;
            public bool IsStarting;
        }

        private static Dictionary<string, CustomRoomState> CustomRooms = new Dictionary<string, CustomRoomState>();

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
                    OnlineUsers.Remove(username);
                    WaitingQueue.Remove(username);
                    RematchWaiting.Remove(username);

                    // Remove khỏi custom rooms nếu là host/guest
                    string removeRoomId = null;
                    foreach (var kv in CustomRooms)
                    {
                        var r = kv.Value;
                        if (r.HostClient == client || r.GuestClient == client)
                        {
                            removeRoomId = kv.Key;
                            break;
                        }
                    }
                    if (removeRoomId != null)
                    {
                        var r = CustomRooms[removeRoomId];
                        string other = null;
                        TcpClient otherClient = null;

                        if (r.HostClient == client)
                        {
                            other = r.GuestName;
                            otherClient = r.GuestClient;
                        }
                        else
                        {
                            other = r.HostName;
                            otherClient = r.HostClient;
                        }

                        CustomRooms.Remove(removeRoomId);

                        if (!string.IsNullOrEmpty(other) && otherClient != null && otherClient.Connected)
                            SendDirect(otherClient, "JOIN_FAIL|Phòng đã bị đóng (đối thủ thoát).");
                    }

                    // Nếu đang trong trận
                    if (ActiveMatches.ContainsKey(username))
                    {
                        string opponent = ActiveMatches[username];

                        ActiveMatches.Remove(username);
                        PlayerSides.Remove(username);
                        RematchWaiting.Remove(opponent);

                        SendToPlayer(opponent, "OPPONENT_LEFT|Your opponent disconnected.");

                        if (ActiveMatches.ContainsKey(opponent)) ActiveMatches.Remove(opponent);
                        if (PlayerSides.ContainsKey(opponent)) PlayerSides.Remove(opponent);
                    }
                }

                lock (Clients) Clients.Remove(client);
                try { client.Close(); } catch { }
            }

            Console.WriteLine($"[INFO] Client {username ?? "Unknown"} disconnected.");
        }

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
                        {
                            string result = HandleSignIn(parts);
                            if (result.StartsWith("Success"))
                            {
                                currentUsername = parts[1];
                                lock (lockObj) { OnlineUsers[currentUsername] = client; }
                            }
                            return result;
                        }

                    case "SIGNUP": return HandleRegister(parts);
                    case "GETPLAYER": return HandleGetPlayer(parts);
                    case "GETEMAIL": return HandleGetEmail(parts);
                    case "UPDATEPASS": return HandleUpdatePassword(parts);
                    case "UPDATEEMAIL": return HandleUpdateEmail(parts);
                    case "UPDATEBIRTHDAY": return HandleUpdateBirthday(parts);

                    case "GET_LEADERBOARD":
                        {
                            var topPlayers = PlayerADO.GetTopPlayer(10);
                            Console.WriteLine($"[INFO] Retrieved top {topPlayers.Count} players.");

                            StringBuilder sb = new StringBuilder();
                            foreach (var p in topPlayers)
                            {
                                sb.Append($"|{p.PlayerName}:{p.Score}");
                            }

                            string response = "LEADERBOARD_DATA" + sb.ToString();
                            SendToPlayer(currentUsername, response);
                            return "";
                        }

                    // --- GAME LOGIC ---
                    // Quick Match
                    case "FIND_MATCH":
                        HandleFindMatch(parts[1], client);
                        return null;

                    case "CANCEL_MATCH":
                        HandleCancelMatch(parts[1]);
                        return null;

                    // Custom Room
                    case "CREATE_ROOM":
                        HandleCreateRoom(parts[1], client);
                        return null;

                    case "JOIN_ROOM":
                        HandleJoinRoom(parts[1], parts[2], client);
                        return null;

                    case "READY":
                        // READY|username|roomId|1/0
                        HandleReady(parts[1], parts[2], parts[3]);
                        return null;

                    // Game
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

                    case "REMATCH_REQUEST":
                        HandleRematchRequest(currentUsername);
                        return null;

                    case "REMATCH_ACCEPT":
                        HandleRematchAccept(currentUsername);
                        return null;

                    case "REMATCH_DECLINE":
                        HandleRematchDecline(currentUsername);
                        return null;

                    case "RESET_REQUEST":
                        HandleResetRequest(currentUsername);
                        return null;

                    case "RESET_ACCEPT":
                        HandleResetAccept(currentUsername);
                        return null;

                    case "RESET_DECLINE":
                        HandleResetDecline(currentUsername);
                        return null;

                    case "GAME_RESULT":
                        HandleGameResult(currentUsername, parts.Length > 1 ? parts[1] : "");
                        return null;
                    
                    case "PVE_RESULT":
                        HandlePvEResult(currentUsername,
                            parts.Length > 1 ? parts[1] : "",
                            parts.Length > 2 ? parts[2] : "");
                        return null;

                    case "VERIFY_PASSWORD":
                        return HandleVerifyPassword(parts);

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

        // ===================== CUSTOM ROOM =====================

        private void HandleCreateRoom(string username, TcpClient client)
        {
            lock (lockObj)
            {
                Random rnd = new Random();
                string roomId = rnd.Next(0, 999999).ToString("D6");
                while (CustomRooms.ContainsKey(roomId))
                    roomId = rnd.Next(0, 999999).ToString("D6");

                CustomRooms[roomId] = new CustomRoomState
                {
                    RoomId = roomId,
                    HostName = username,
                    HostClient = client,
                    GuestClient = null,
                    HostReady = false,
                    GuestReady = false,
                    IsStarting = false
                };

                SendDirect(client, $"ROOM_CREATED|{roomId}|{username}");
                Console.WriteLine($"[ROOM] {username} created room {roomId}");
            }
        }

        private void HandleJoinRoom(string joinerName, string roomId, TcpClient joinerClient)
        {
            lock (lockObj)
            {
                if (!CustomRooms.ContainsKey(roomId))
                {
                    SendDirect(joinerClient, "JOIN_FAIL|Phòng không tồn tại.");
                    return;
                }

                var room = CustomRooms[roomId];

                if (room.HostClient == null || !room.HostClient.Connected)
                {
                    CustomRooms.Remove(roomId);
                    SendDirect(joinerClient, "JOIN_FAIL|Chủ phòng đã mất kết nối.");
                    return;
                }

                if (room.HostName == joinerName)
                {
                    SendDirect(joinerClient, "JOIN_FAIL|Không thể tự vào phòng mình.");
                    return;
                }

                if (room.GuestClient != null && room.GuestClient.Connected)
                {
                    SendDirect(joinerClient, "JOIN_FAIL|Phòng đã đủ 2 người.");
                    return;
                }

                room.GuestName = joinerName;
                room.GuestClient = joinerClient;
                room.HostReady = false;
                room.GuestReady = false;
                room.IsStarting = false;

                SendDirect(room.HostClient, $"ROOM_JOINED|{roomId}|{room.HostName}|{room.GuestName}");
                SendDirect(room.GuestClient, $"ROOM_JOINED|{roomId}|{room.HostName}|{room.GuestName}");

                BroadcastReadyUpdate(room);

                Console.WriteLine($"[ROOM] {roomId} joined: {room.HostName} vs {room.GuestName}");
            }
        }

        private void HandleReady(string username, string roomId, string readyRaw)
        {
            lock (lockObj)
            {
                if (!CustomRooms.ContainsKey(roomId)) return;

                var room = CustomRooms[roomId];
                bool ready = (readyRaw == "1");

                if (username == room.HostName) room.HostReady = ready;
                else if (username == room.GuestName) room.GuestReady = ready;
                else return;

                BroadcastReadyUpdate(room);

                if (room.HostClient == null || room.GuestClient == null) { room.IsStarting = false; return; }
                if (!room.HostClient.Connected || !room.GuestClient.Connected) { room.IsStarting = false; return; }

                if (room.HostReady && room.GuestReady && !room.IsStarting)
                {
                    room.IsStarting = true;

                    SendDirect(room.HostClient, "GAME_STARTING|3");
                    SendDirect(room.GuestClient, "GAME_STARTING|3");

                    new Thread(() =>
                    {
                        Thread.Sleep(3000);

                        lock (lockObj)
                        {
                            if (!CustomRooms.ContainsKey(roomId)) return;
                            var r = CustomRooms[roomId];

                            if (r.HostClient == null || r.GuestClient == null) return;
                            if (!r.HostClient.Connected || !r.GuestClient.Connected) return;

                            // Nếu trong lúc đếm mà ai đó unready
                            if (!r.HostReady || !r.GuestReady) { r.IsStarting = false; return; }

                            ActiveMatches[r.HostName] = r.GuestName;
                            ActiveMatches[r.GuestName] = r.HostName;

                            PlayerSides[r.HostName] = 1;   // host X
                            PlayerSides[r.GuestName] = 2; // guest O

                            RematchWaiting.Remove(r.HostName);
                            RematchWaiting.Remove(r.GuestName);

                            SendDirect(r.HostClient, $"MATCH_FOUND|{r.GuestName}|X");
                            SendDirect(r.GuestClient, $"MATCH_FOUND|{r.HostName}|O");

                            Console.WriteLine($"[MATCH] Custom start: {r.HostName}(X) vs {r.GuestName}(O)");

                            // Optional: xóa phòng để không join lại
                            // CustomRooms.Remove(roomId);
                        }
                    })
                    { IsBackground = true }.Start();
                }
                else
                {
                    room.IsStarting = false;
                }
            }
        }

        private void BroadcastReadyUpdate(CustomRoomState room)
        {
            string msg = $"READY_UPDATE|{room.RoomId}|{(room.HostReady ? 1 : 0)}|{(room.GuestReady ? 1 : 0)}";
            SendDirect(room.HostClient, msg);
            if (room.GuestClient != null) SendDirect(room.GuestClient, msg);
        }

        // ===================== QUICK MATCH =====================

        private void HandleFindMatch(string username, TcpClient client)
        {
            lock (lockObj)
            {
                WaitingQueue[username] = client;

                string opponentName = null;
                foreach (var waiter in WaitingQueue)
                {
                    if (waiter.Key != username)
                    {
                        opponentName = waiter.Key;
                        break;
                    }
                }

                if (opponentName == null)
                {
                    Console.WriteLine($"[MATCH] {username} added to queue.");
                    return;
                }

                TcpClient opponentClient = WaitingQueue[opponentName];
                if (opponentClient == null || !opponentClient.Connected)
                {
                    WaitingQueue.Remove(opponentName);
                    return;
                }

                WaitingQueue.Remove(opponentName);
                WaitingQueue.Remove(username);

                ActiveMatches[username] = opponentName;
                ActiveMatches[opponentName] = username;

                PlayerSides[opponentName] = 1; // opponent X
                PlayerSides[username] = 2;     // me O

                RematchWaiting.Remove(username);
                RematchWaiting.Remove(opponentName);

                Console.WriteLine($"[MATCH] Found: {opponentName} vs {username}");

                SendDirect(client, $"MATCH_FOUND|{opponentName}|O");
                SendDirect(opponentClient, $"MATCH_FOUND|{username}|X");
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

        // ===================== GAME =====================

        private void HandleMove(string[] parts, string sender)
        {
            lock (lockObj)
            {
                if (string.IsNullOrEmpty(sender)) return;
                if (!ActiveMatches.ContainsKey(sender)) return;
                if (parts.Length < 3) return;

                string opponent = ActiveMatches[sender];
                string x = parts[1];
                string y = parts[2];

                int side = PlayerSides.ContainsKey(sender) ? PlayerSides[sender] : -1;

                SendToPlayer(opponent, $"MOVE|{x}|{y}|{side}");
                SendToPlayer(sender, $"MOVE|{x}|{y}|{side}");
            }
        }

        private void HandleUndoRequest(string sender)
        {
            lock (lockObj)
            {
                if (string.IsNullOrEmpty(sender)) return;
                if (!ActiveMatches.ContainsKey(sender)) return;

                string opponent = ActiveMatches[sender];

                SendToPlayer(sender, "UNDO_SUCCESS");
                SendToPlayer(opponent, "UNDO_SUCCESS");
            }
        }

        private void HandleChat(string[] parts, string sender)
        {
            lock (lockObj)
            {
                if (string.IsNullOrEmpty(sender)) return;
                if (!ActiveMatches.ContainsKey(sender)) return;
                if (parts.Length < 2) return;

                string opponent = ActiveMatches[sender];
                SendToPlayer(opponent, $"CHAT|{parts[1]}");
            }
        }

        private void HandleSurrender(string sender)
        {
            lock (lockObj)
            {
                if (string.IsNullOrEmpty(sender)) return;
                if (!ActiveMatches.ContainsKey(sender)) return;

                string opponent = ActiveMatches[sender];

                SendToPlayer(opponent, "OPPONENT_LEFT|Đối thủ đã thoát / đầu hàng. Bạn thắng!");
                PlayerADO.UpdateScore(opponent, 1); // Thắng +1 điểm
                ActiveMatches.Remove(sender);
                PlayerSides.Remove(sender);
                RematchWaiting.Remove(sender);

                if (ActiveMatches.ContainsKey(opponent)) ActiveMatches.Remove(opponent);
                if (PlayerSides.ContainsKey(opponent)) PlayerSides.Remove(opponent);
                RematchWaiting.Remove(opponent);
            }
        }

        // ===================== REMATCH =====================

        private void HandleRematchRequest(string sender)
        {
            lock (lockObj)
            {
                if (string.IsNullOrEmpty(sender)) return;
                if (!ActiveMatches.ContainsKey(sender)) return;

                string opponent = ActiveMatches[sender];

                RematchWaiting.Add(sender);
                SendToPlayer(opponent, $"REMATCH_OFFER|{sender}");
                SendToPlayer(sender, "REMATCH_SENT");
            }
        }

        private void HandleRematchAccept(string sender)
        {
            lock (lockObj)
            {
                if (string.IsNullOrEmpty(sender)) return;
                if (!ActiveMatches.ContainsKey(sender)) return;

                string opponent = ActiveMatches[sender];

                if (!RematchWaiting.Contains(opponent))
                {
                    RematchWaiting.Add(sender);
                    SendToPlayer(opponent, $"REMATCH_OFFER|{sender}");
                    return;
                }

                RematchWaiting.Remove(opponent);
                RematchWaiting.Remove(sender);

                if (PlayerSides.ContainsKey(sender)) PlayerSides[sender] = (PlayerSides[sender] == 1) ? 2 : 1;
                if (PlayerSides.ContainsKey(opponent)) PlayerSides[opponent] = (PlayerSides[opponent] == 1) ? 2 : 1;

                string senderSide = PlayerSides.ContainsKey(sender) ? (PlayerSides[sender] == 1 ? "X" : "O") : "O";
                string oppSide = PlayerSides.ContainsKey(opponent) ? (PlayerSides[opponent] == 1 ? "X" : "O") : "X";

                SendToPlayer(sender, $"REMATCH_START|{senderSide}");
                SendToPlayer(opponent, $"REMATCH_START|{oppSide}");
            }
        }

        private void HandleRematchDecline(string sender)
        {
            lock (lockObj)
            {
                if (string.IsNullOrEmpty(sender)) return;
                if (!ActiveMatches.ContainsKey(sender)) return;

                string opponent = ActiveMatches[sender];

                RematchWaiting.Remove(sender);
                RematchWaiting.Remove(opponent);

                SendToPlayer(sender, $"REMATCH_DECLINED|{sender}");
                SendToPlayer(opponent, $"REMATCH_DECLINED|{sender}");
            }
        }

        // ===================== RESET =====================
        private void HandleResetRequest(string sender)
        {
            lock (lockObj)
            {
                if (string.IsNullOrEmpty(sender)) return;
                if (!ActiveMatches.ContainsKey(sender)) return;
                if (ResetWaiting.Contains(sender)) return;

                string opponent = ActiveMatches[sender];

                ResetWaiting.Add(sender);
                SendToPlayer(opponent, $"RESET_OFFER|{sender}");
                SendToPlayer(sender, "RESET_SENT");
            }
        }

        private void HandleResetAccept(string sender)
        {
            lock (lockObj)
            {
                if (string.IsNullOrEmpty(sender)) return;
                if (!ActiveMatches.ContainsKey(sender)) return;

                string opponent = ActiveMatches[sender];

                if (!ResetWaiting.Contains(opponent)) return;

                ResetWaiting.Remove(opponent);

                SendToPlayer(sender, $"RESET_EXECUTE|{sender}");
                SendToPlayer(opponent, $"RESET_EXECUTE|{sender}");
                Console.WriteLine($"[RESET] {opponent} accepted reset from {sender}");
            }
        }

        private void HandleResetDecline(string sender)
        {
            lock (lockObj)
            {
                if (string.IsNullOrEmpty(sender)) return;
                if (!ActiveMatches.ContainsKey(sender)) return;

                string opponent = ActiveMatches[sender];

                if (!ResetWaiting.Contains(opponent)) return;

                ResetWaiting.Remove(opponent);

                SendToPlayer(opponent, $"RESET_DECLINED|{sender}");
            }
        }

        private string HandleVerifyPassword(string[] parts)
        {
            if (parts.Length < 3) return "Error|Missing info";

            // chỉ verify đúng/sai, không trả thông tin user
            var player = PlayerADO.Authenticate(parts[1], parts[2]);
            if (player != null) return "Success|OK";
            return "Error|Invalid password";
        }


        // ===================== LEADERBOARD (stub) =====================

        private void HandleGameResult(string username, string resultRaw)
        {
            // TODO: update DB leaderboard theo username + WIN/LOSE
            // resultRaw: "WIN" / "LOSE"
            if (string.IsNullOrEmpty(username)) return;

            string r = (resultRaw ?? "").Trim().ToUpperInvariant();
            Console.WriteLine($"[RESULT] {username}: {r}");

            // Ví dụ:
            // if (r == "WIN") PlayerADO.AddWin(username);
            // else if (r == "LOSE") PlayerADO.AddLose(username);
        }

        // ===================== SEND HELPERS =====================

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
            catch { }
        }

        private void SendToPlayer(string username, string msg)
        {
            TcpClient c = null;

            lock (lockObj)
            {
                if (string.IsNullOrEmpty(username)) return;
                if (!OnlineUsers.ContainsKey(username)) return;
                c = OnlineUsers[username];
            }

            SendDirect(c, msg);
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
                    ActiveMatches.Clear();
                    PlayerSides.Clear();
                    RematchWaiting.Clear();
                    CustomRooms.Clear();
                }

                lock (Clients)
                {
                    foreach (var client in Clients)
                    {
                        try { client.Close(); } catch { }
                    }
                    Clients.Clear();
                }

                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] Server stopped manually.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error stopping server: {ex.Message}");
            }
        }

        // ===================== DB / ACCOUNT =====================

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

        private string HandleUpdateEmail(string[] parts)
        {
            if (parts.Length < 3) return "Error|Missing data";
            bool ok = PlayerADO.UpdateEmail(parts[1], parts[2]);
            if (ok) return "Success|Updated";
            return "Error|Update failed";
        }

        private void HandlePvEResult(string username, string resultRaw, string difficultyRaw)
        {
            if (string.IsNullOrEmpty(username)) return;

            string r = (resultRaw ?? "").Trim().ToUpperInvariant();      // WIN / LOSE
            string diff = (difficultyRaw ?? "").Trim();                  // Easy/Medium/Hard/Extremely Hard

            Console.WriteLine($"[PVE_RESULT] {username}: {r} | Diff={diff}");

            // TODO: update leaderboard PvE
            // Gợi ý:
            // - Win mới tính điểm, Lose không tính (tùy bạn)
            // - Difficulty càng cao điểm càng nhiều
            //
            // Ví dụ pseudo:
            // int score = 0;
            // if (r == "WIN")
            // {
            //     score = diff == "Easy" ? 1 :
            //             diff == "Medium" ? 2 :
            //             diff == "Hard" ? 3 :
            //             diff == "Extremely Hard" ? 5 : 1;
            //     PlayerADO.AddPvEScore(username, score); // bạn tự implement
            // }
        }


        private string HandleUpdateBirthday(string[] parts)
        {
            if (parts.Length < 3) return "Error|Missing data";
            bool ok = PlayerADO.UpdateBirthday(parts[1], parts[2]);
            if (ok) return "Success|Updated";
            return "Error|Update failed";
        }

        private string HidePassword(string text)
        {
            if (text.Contains("SIGNIN") || text.Contains("SIGNUP") || text.Contains("VERIFY_PASSWORD"))
                return text.Split('|')[0] + "|***HIDDEN***";
            return text;
        }
    }
}
