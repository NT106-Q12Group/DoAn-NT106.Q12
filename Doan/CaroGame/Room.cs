using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaroGame
{
    public class Room
    {
        // --- CÁC THUỘC TÍNH CỦA BẠN ---
        public string roomID;
        public int playerCount;

        public bool player1Ready = false;
        public bool player2Ready = false;

        public string player1name;
        public string player2Name;

        // --- BỔ SUNG CONSTRUCTOR ---

        // 1. Constructor mặc định (để tránh lỗi khi khởi tạo rỗng)
        public Room()
        {
        }

        // 2. Constructor nhanh (Dùng trong PvPLobby khi tìm thấy trận)
        public Room(string id, string p1, string p2)
        {
            this.roomID = id;
            this.player1name = p1;
            this.player2Name = p2;
            this.playerCount = 2; // Tìm thấy trận nghĩa là đã đủ 2 người
        }
    }
}