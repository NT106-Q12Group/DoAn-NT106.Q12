using System;

namespace CaroGame_TCPClient
{
    public static class Session
    {
        public static TCPClient Client { get; } = new TCPClient("3.230.162.159", 25565);
        public static string CurrentUser { get; set; } = "";
    }
}
