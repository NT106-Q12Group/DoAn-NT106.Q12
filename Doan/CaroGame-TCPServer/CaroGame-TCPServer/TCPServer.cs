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
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] Server is now running on port {port}.");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] Waiting for client connection...");
                Console.ResetColor();
                Console.WriteLine("");

                Thread listenThread = new Thread(ListenForClients);
                listenThread.IsBackground = true;
                listenThread.Start();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ERROR] An error occurred while starting server. ({ex.Message})");
                Console.ResetColor();
                Console.WriteLine("");
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
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] New client detected: {clientIP}");
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] Total current client connections: {Clients.Count}");
                    Console.WriteLine("");

                    Thread clientThread = new Thread(() => HandleClient(client));
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }
                catch (SocketException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ERROR] An error occurred while accepting client connection. ({ex.Message})");
                    Console.ResetColor();
                    Console.WriteLine("");
                }
            }
        }

        private void HandleClient(TcpClient client)
        {
            NetworkStream stream = null;
            string clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

            try
            {
                stream = client.GetStream();
                byte[] buffer = new byte[2048];
                while (isRunning && client.Connected)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    string logRequest = HidePassword(request);
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] Request from {clientIP}: {logRequest}");

                    string response = ProcessRequest(request);

                    byte[] responseData = Encoding.UTF8.GetBytes(response);
                    stream.Write(responseData, 0, responseData.Length);
                    string logResponse = HidePassword(response);
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] Response to {clientIP}: {logResponse}");
                    Console.WriteLine("");
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ERROR] An error occurred while processing client {clientIP}. ({ex.Message})");
                Console.ResetColor();
                Console.WriteLine("");
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                }

                lock (Clients)
                {
                    Clients.Remove(client);
                }

                client.Close();
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] Client {clientIP} disconnected.");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] Total current client connections: {Clients.Count}");
            }
        }

        private string HidePassword(string text)
        {
            try
            {
                var parts = text.Split('|');
                if (parts.Length == 0) return text;

                var cmd = parts[0].ToUpperInvariant();

                if (cmd == "SIGNIN" && parts.Length >= 3)
                {
                    return $"{parts[0]}|{parts[1]}|***";
                }

                if (cmd == "SIGNUP" && parts.Length >= 5)
                {
                    string username = parts[1];
                    string email = parts.Length >= 4 ? parts[3] : "";
                    string birthday = parts.Length >= 5 ? parts[4] : "";
                    return $"{parts[0]}|{username}|***|{email}|{birthday}";
                }

                return text;
            }
            catch
            {
                return text;
            }
        }

        private string ProcessRequest(string request)
        {
            try
            {
                string[] parts = request.Split('|', StringSplitOptions.None);
                if (parts.Length == 0 || string.IsNullOrWhiteSpace(parts[0])) return "Error|Invalid request!";

                string cmd = parts[0].ToUpperInvariant();

                switch (cmd)
                {
                    case "SIGNIN":
                        return HandleSignIn(parts);
                    case "SIGNUP":
                        return HandleRegister(parts);
                    case "GETPLAYER":
                        return HandleGetPlayer(parts);
                    case "SIGNOUT":
                        return HandleSignOut(parts);
                    default:
                        return "Error|Invalid command!";
                }
            }
            catch (Exception ex)
            {
                return $"Error|An error occurred while processing request! ({ex.Message})";
            }
        }

        private string HandleSignIn(string[] parts)
        {
            try
            {
                if (parts.Length < 3) return "Error|Missing arguments. Please provide all required fields.";

                string username = parts[1];
                string psw = parts[2];

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(psw))
                    return "Error|Username and password cannot be blank.";

                var player = PlayerADO.Authenticate(username, psw);

                if (player != null)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] Player '{username}' logged in successfully.");
                    Console.ResetColor();
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] Total current clients online: {Clients.Count}");
                    Console.WriteLine();
                    return $"Success|{player.PlayerName}|{player.Email}|{player.Birthday}";
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [WARNING] Login failed: Invalid username or password.");
                    Console.ResetColor();
                    Console.WriteLine("");
                    return "Error|Invalid username or password.";
                }
            }
            catch (Exception ex)
            {
                return $"Error|Authentication failed due to an unexpected error. ({ex.Message})";
            }
        }

        private string HandleRegister(string[] parts)
        {
            try
            {
                if (parts.Length < 5) return "Error|Missing arguments. Please provide all required fields.";

                string username = parts[1];
                string psw = parts[2];
                string email = parts[3];
                string birthday = parts[4];

                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(psw))
                    return "Error|Username and password cannot be blank.";

                var newPlayer = new Player.Player(username, psw, email, birthday);

                if (PlayerADO.RegisterPlayer(newPlayer))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] Player '{username}' registered successfully.");
                    Console.ResetColor();
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] Total current clients online: {Clients.Count}");
                    Console.WriteLine();
                    return "Success|Registered successfully.";
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [WARNING] Register failed: Player '{username}' already exists.");
                    Console.ResetColor();
                    return "Error|Player already exists.";
                }
            }
            catch (Exception ex)
            {
                return $"Error|Registration failed due to an unexpected error. ({ex.Message})";
            }
        }

        private string HandleGetPlayer(string[] parts)
        {
            try
            {
                if (parts.Length < 2) return "Error|Missing arguments. Please provide the player name.";
                string playername = parts[1];
                var player = PlayerADO.GetPlayerByPlayerName(playername);
                if (player == null) return "Error|Player not found.";
                return $"Success|{player.PlayerName}|{player.Email}|{player.Birthday}";
            }
            catch (Exception ex)
            {
                return $"Error|Failed to get player. ({ex.Message})";
            }
        }

        private string HandleSignOut(string[] parts)
        {
            try
            {
                if (parts.Length < 2) return "Success|Signed out.";
                string username = parts[1];
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] User '{username}' signed out.");
                Console.ResetColor();
                return "Success|Signed out.";
            }
            catch (Exception ex)
            {
                return $"Error|Sign-out failed. ({ex.Message})";
            }
        }

        public void Stop()
        {
            try
            {
                isRunning = false;

                lock (Clients)
                {
                    foreach (var c in Clients) c.Close();
                    Clients.Clear();
                }

                if (Server != null) Server.Stop();

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] Server stopped.");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [INFO] Total connection attempts: {ConnectionCount}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Error while stopping server: {ex.Message}");
                Console.ResetColor();
            }
        }

        public int GetClientCount()
        {
            lock (Clients)
            {
                return Clients.Count;
            }
        }

        public bool IsRunning()
        {
            return isRunning;
        }
    }
}