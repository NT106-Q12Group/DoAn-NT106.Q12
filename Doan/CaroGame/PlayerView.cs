using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaroGame_TCPClient
{
    public sealed class PlayerView
    {
        public int PlayerID { get; init; }
        public string PlayerName { get; init; } = "";
        public string Email { get; init; } = "";
        public string Birthday { get; init; } = "";
        public string SessionPassword { get; init; } = "";
    }
}


