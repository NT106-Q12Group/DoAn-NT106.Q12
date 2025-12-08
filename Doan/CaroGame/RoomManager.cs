using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaroGame
{
    internal class RoomManager
    {
        private static HashSet<string> UsedRoomIDs = new HashSet<string>();
        private static Dictionary<string, Room> Rooms = new Dictionary<string, Room>();
        private static Random rand = new Random();

        public static Room createRoom()
        {
            string id;

            do
            {
                id = rand.Next(0, 999999).ToString("D6");
            }
            while (UsedRoomIDs.Contains(id));

            UsedRoomIDs.Add(id);

            var room = new Room()
            {
                roomID = id,
                playerCount = 1,
            };

            Rooms[id] = room;

            return room;
        }

        private static object roomLock = new object();

        public static Room JoinRoom(string roomId, out int playerNumber)
        {
            lock (roomLock)
            {
                playerNumber = 0;

                //chưa tạo phòng thì phòng không tồn tại
                if (!Rooms.ContainsKey(roomId))
                    return null;

                var room = Rooms[roomId];

                //Kiểm tra nếu phòng >= 2 thì trả về rỗng
                if (room.playerCount >= 2)
                    return null;

                room.playerCount++;
                playerNumber = room.playerCount;

                return room;
            }
        }
    }
}
