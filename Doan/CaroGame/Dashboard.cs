using CaroGame_TCPClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaroGame
{
    public partial class Dashboard : Form
    {
        private Room room;
        private int playerNumber;
        private Form _userInfoForm;
        private string _loggedInUser;
        private PlayerView pv;

        // Biến quan trọng để lưu kết nối mạng
        // Nếu biến này null => Đang offline hoặc chưa đăng nhập
        private TCPClient _client;

        // Constructor 1: Mặc định (Dùng cho Designer hoặc Test giao diện)
        public Dashboard()
        {
            InitializeComponent();
            _loggedInUser = "Player";
        }

        // Constructor 2: Nhận user (Offline test)
        public Dashboard(string username)
        {
            InitializeComponent();
            _loggedInUser = username;
        }

        // Constructor 3: Dùng cho Login -> Dashboard (QUAN TRỌNG NHẤT)
        // Đây là constructor bạn PHẢI dùng ở SignIn.cs
        public Dashboard(string username, TCPClient client)
        {
            InitializeComponent();
            _loggedInUser = username;
            _client = client; // Lưu kết nối mạng vào biến toàn cục
        }

        // Constructor 4: Dùng khi quay lại từ PvP (Giữ phòng và Client)
        public Dashboard(Room room, int playerNumber, string username, TCPClient client)
        {
            InitializeComponent();
            this.room = room;
            this.playerNumber = playerNumber;
            this._loggedInUser = username;
            this._client = client;
        }

        private void btnPvE_Click(object sender, EventArgs e)
        {
            var newGameForm = new BotDifficulty(_loggedInUser);
            newGameForm.Show();
            this.Hide();
        }

        private void pnlNaviBar_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnPvP_Click(object sender, EventArgs e)
        {
            if (pnlPvPMenu != null)
                pnlPvPMenu.Visible = !pnlPvPMenu.Visible;
        }

        private void btnCreateRoom_Click(object sender, EventArgs e)
        {
            // Logic tạo phòng Local (Cũ)
            Room newRoom = RoomManager.createRoom();
            room = newRoom;
            playerNumber = 1;

            var newWaitingRoom = new WaitingRoom(newRoom, 1);
            newWaitingRoom.Show();
            this.Hide();
        }

        private void btnJoinRoom_Click(object sender, EventArgs e)
        {
            if (pnlDashBoard != null)
            {
                pnlDashBoard.Visible = !pnlDashBoard.Visible;
                pnlPvPMenu.Visible = !pnlPvPMenu.Visible;
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (pnlDashBoard != null)
                pnlDashBoard.Visible = !pnlDashBoard.Visible;
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            string id = tbRoomID.Text.Trim();

            Room joinedRoom = RoomManager.JoinRoom(id, out int pNumber);

            if (joinedRoom == null)
            {
                MessageBox.Show("Phòng không tồn tại hoặc đã đủ 2 người!");
                return;
            }

            room = joinedRoom;
            playerNumber = pNumber;

            var waitingroom = new WaitingRoom(joinedRoom, pNumber);
            waitingroom.Show();
            this.Hide();
        }

        private void LoadPlayer(string username, string email, string birthday)
        {
            pv = new PlayerView
            {
                PlayerID = 0,
                PlayerName = username,
                Email = email,
                Birthday = birthday
            };
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            // Cần truyền Client vào UserInfo để đổi pass được
            var userInfoForm = new UserInfo(pv, _client);
            userInfoForm.FormClosed += (s, args) => this.Show();
            this.Hide();
            userInfoForm.Show();
        }

        // --- NÚT PLAY INSTANT (QUICK MATCH) ---
        private void btnPlayInstant_Click(object sender, EventArgs e)
        {
            // 1. KIỂM TRA KẾT NỐI TRƯỚC KHI MỞ LOBBY
            // Nếu _client bị null (do mở bằng Dashboard() thường) -> Báo lỗi
            if (_client == null || !_client.IsConnected())
            {
                MessageBox.Show("Tính năng này yêu cầu kết nối Server (Online).\nVui lòng đăng nhập lại từ màn hình SignIn.",
                                "Lỗi Kết Nối",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            // 2. Mở màn hình chờ Quick Match (PvPLobby)
            // Truyền username và biến client đang kết nối sang đó
            // isQuickMatch = true (mặc định) để nó tự tìm trận ngay
            var waitingScreen = new PvPLobby(_loggedInUser, _client, true);

            this.Hide();

            // Khi form chờ đóng lại, hiện lại Dashboard
            waitingScreen.FormClosed += (s, args) => this.Show();

            waitingScreen.Show();
        }
    }
}