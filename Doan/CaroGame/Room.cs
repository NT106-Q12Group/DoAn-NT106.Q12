using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaroGame
{
    public class Room
    {
        public string roomID;
        public int playerCount;

        public bool player1Ready = false;
        public bool player2Ready = false;

        public string player1name;
        public string player2Name;
    }
}
