using CaroGame_TCPClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CaroGame
{
    public partial class Dashboard : Form
    {
        private Room room;
        private int playerNumber;
        private string _loggedInUser;
        private PlayerView pv;
        private TCPClient _client;
        private bool isFindingMatch = false;

        // --- CONSTRUCTORS ---
        public Dashboard() 
        { 
            InitializeComponent(); 
            _loggedInUser = "Player"; 
        }
        public Dashboard(string username) 
        { 
            InitializeComponent(); 
            _loggedInUser = username; 
        }

        public Dashboard(string username, TCPClient client)
        {
            InitializeComponent();
            _loggedInUser = username;
            _client = client;
            RegisterServerListener();
        }

        public Dashboard(Room room, int playerNumber, string username, TCPClient client)
        {
            InitializeComponent();
            this.room = room;
            this.playerNumber = playerNumber;
            this._loggedInUser = username;
            this._client = client;
            RegisterServerListener();
        }

        private void RegisterServerListener()
        {
            if (_client != null)
            {
                _client.OnMessageReceived -= Dashboard_OnMessageReceived;
                _client.OnMessageReceived += Dashboard_OnMessageReceived;
            }
        }

        // --- [CORE] XỬ LÝ TIN NHẮN TỪ SERVER ---
        private void Dashboard_OnMessageReceived(string data)
        {
            this.Invoke((MethodInvoker)delegate
            {
                try
                {
                    string[] parts = data.Split('|');
                    string command = parts[0];

                    if (command == "MATCH_FOUND")
                    {
                        isFindingMatch = false;
                        if (btnPlayInstant != null) btnPlayInstant.Text = "Play Instant";

                        string roomID = parts.Length > 1 ? parts[1] : "Room_001";
                        int mySide = parts.Length > 2 ? int.Parse(parts[2]) : 2;
                        string opponentName = parts.Length > 3 ? parts[3] : "Unknown";

                        string p1 = (mySide == 1) ? _loggedInUser : opponentName;
                        string p2 = (mySide == 1) ? opponentName : _loggedInUser;

                        // [ĐÃ SỬA LỖI Ở ĐÂY] 
                        // Bỏ số 1 đi, chỉ truyền (ID, Player1, Player2) cho đúng với class Room của bạn
                        Room newRoom = new Room(roomID, p1, p2);

                        if (_client != null) _client.OnMessageReceived -= Dashboard_OnMessageReceived;

                        PvP pvpForm = new PvP(newRoom, mySide, p1, p2, _client);

                        pvpForm.FormClosed += (s, args) => {
                            this.Show();
                            RegisterServerListener();
                        };

                        this.Hide();
                        pvpForm.Show();
                    }
                }
                catch (Exception ex) { }
            });
        }

        // --- CÁC SỰ KIỆN NÚT BẤM ---
        private void btnPvE_Click(object sender, EventArgs e)
        {
            var newGameForm = new BotDifficulty(_loggedInUser);

            // --- [FIX QUAN TRỌNG] ---
            // Khi form Bot đóng lại -> Tự động hiện lại Dashboard cũ (vẫn còn kết nối mạng)
            // Không cần tạo new Dashboard()
            newGameForm.FormClosed += (s, args) => this.Show();

            this.Hide();
            newGameForm.Show();
        }

        // Tìm đến hàm này và thay thế nội dung bên trong
        private void btnPlayInstant_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra kết nối mạng
            if (_client == null || !_client.IsConnected())
            {
                MessageBox.Show("Mất kết nối Server! Vui lòng đăng nhập lại.", "Lỗi Mạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Tạm ngưng Dashboard lắng nghe để nhường quyền cho PvPLobby
            // (Nếu không ngắt, cả 2 form cùng nhận tin nhắn sẽ gây lỗi)
            if (_client != null)
            {
                _client.OnMessageReceived -= Dashboard_OnMessageReceived;
            }

            // 3. Mở form PvPLobby
            // Tham số true nghĩa là "Tự động tìm trận ngay khi mở"
            var waitingScreen = new PvPLobby(_loggedInUser, _client, true);

            this.Hide();

            // 4. Khi tắt PvPLobby thì hiện lại Dashboard và nghe tin nhắn lại
            waitingScreen.FormClosed += (s, args) =>
            {
                this.Show();
                RegisterServerListener(); // Đăng ký nghe lại
            };

            waitingScreen.Show();
        }

        public void SetPlayer(PlayerView player)
        {
            pv = player;
        }


        private void btnSettings_Click(object sender, EventArgs e) 
        { 
            var userInfoForm = new UserInfo(pv, _client);
            this.Hide();

            userInfoForm.FormClosed += (s, args) => 
            {
                this.Show();
            };

            userInfoForm.Show(); 
        }

        // --- CÁC HÀM CŨ ---
        private void btnPvP_Click(object sender, EventArgs e) { if (pnlPvPMenu != null) pnlPvPMenu.Visible = !pnlPvPMenu.Visible; }
        private void btnCreateRoom_Click(object sender, EventArgs e) { Room newRoom = RoomManager.createRoom(); room = newRoom; playerNumber = 1; var newWaitingRoom = new WaitingRoom(newRoom, 1); newWaitingRoom.Show(); this.Hide(); }
        private void btnJoinRoom_Click(object sender, EventArgs e) { if (pnlDashBoard != null) { pnlDashBoard.Visible = !pnlDashBoard.Visible; pnlPvPMenu.Visible = !pnlPvPMenu.Visible; } }
        private void btnBack_Click(object sender, EventArgs e) { if (pnlDashBoard != null) pnlDashBoard.Visible = !pnlDashBoard.Visible; }
        private void btnJoin_Click(object sender, EventArgs e) { string id = tbRoomID.Text.Trim(); Room joinedRoom = RoomManager.JoinRoom(id, out int pNumber); if (joinedRoom == null) { MessageBox.Show("Phòng không tồn tại!"); return; } room = joinedRoom; playerNumber = pNumber; var waitingroom = new WaitingRoom(joinedRoom, pNumber); waitingroom.Show(); this.Hide(); }
        private void LoadPlayer(string username, string email, string birthday) { pv = new PlayerView { PlayerID = 0, PlayerName = username, Email = email, Birthday = birthday }; }
        protected override void OnFormClosing(FormClosingEventArgs e) { if (_client != null) { _client.OnMessageReceived -= Dashboard_OnMessageReceived; } base.OnFormClosing(e); }
        private void pnlNaviBar_Paint(object sender, PaintEventArgs e) { }
    }
}