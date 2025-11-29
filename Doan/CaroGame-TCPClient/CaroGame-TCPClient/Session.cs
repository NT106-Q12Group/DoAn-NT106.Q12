using System;

namespace CaroGame_TCPClient
{
    public static class Session
    {
        public static TCPClient Client { get; } = new TCPClient("127.0.0.1", 25565);
        public static string CurrentUser { get; set; } = "";
    }
}
