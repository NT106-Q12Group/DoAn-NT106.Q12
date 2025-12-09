using System;
using System.Drawing; // Cần tham chiếu System.Drawing

namespace CaroGame_TCPClient // <--- ĐỔI TÊN NAMESPACE Ở ĐÂY
{
    [Serializable]
    public class Packet
    {
        public string Command { get; set; }
        public Point Point { get; set; }
        public string Message { get; set; }

        public Packet() { }

        public Packet(string command, Point point)
        {
            this.Command = command;
            this.Point = point;
        }

        public Packet(string command, string message)
        {
            this.Command = command;
            this.Message = message;
        }
    }
}