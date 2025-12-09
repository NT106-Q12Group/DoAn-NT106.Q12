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

        // Biến quan trọng để lưu kết nối mạng
        private TCPClient _client;

        // Biến trạng thái tìm trận
        private bool isFindingMatch = false;

        // Event để SignIn bắt được khi bấm nút User Info (nếu dùng)
        public event Action OnOpenUserInfo;

        // --- CONSTRUCTORS ---

        // 1. Mặc định
        public Dashboard()
        {
            InitializeComponent();
            _loggedInUser = "Player";
        }

        // 2. Offline Test
        public Dashboard(string username)
        {
            InitializeComponent();
            _loggedInUser = username;
            if (lblWelcome != null) lblWelcome.Text = "Welcome, " + username;
        }

        // 3. [QUAN TRỌNG] Dùng cho Login -> Dashboard
        public Dashboard(string username, TCPClient client)
        {
            InitializeComponent();
            _loggedInUser = username;
            _client = client;

            if (lblWelcome != null) lblWelcome.Text = "Welcome, " + username;

            // Đăng ký lắng nghe Server ngay khi vào Dashboard
            RegisterServerListener();
        }

        // 4. Dùng khi quay lại từ PvP
        public Dashboard(Room room, int playerNumber, string username, TCPClient client)
        {
            InitializeComponent();
            this.room = room;
            this.playerNumber = playerNumber;
            this._loggedInUser = username;
            this._client = client;

            if (lblWelcome != null) lblWelcome.Text = "Welcome, " + username;

            // Đăng ký lắng nghe lại
            RegisterServerListener();
        }

        // --- HÀM HỖ TRỢ LẮNG NGHE SERVER ---
        private void RegisterServerListener()
        {
            if (_client != null)
            {
                // Hủy đăng ký cũ trước khi thêm mới để tránh nhận tin nhắn 2 lần
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
                    // Debug: Console.WriteLine("Dash received: " + data);
                    string[] parts = data.Split('|');
                    string command = parts[0];

                    // 1. XỬ LÝ KHI TÌM ĐƯỢC TRẬN
                    // Server gửi: MATCH_FOUND | RoomID | MySide | OpponentName
                    if (command == "MATCH_FOUND")
                    {
                        // Reset trạng thái nút
                        isFindingMatch = false;
                        if (btnPlayInstant != null) btnPlayInstant.Text = "Play Instant";

                        // Parse dữ liệu
                        string roomID = parts.Length > 1 ? parts[1] : "Room_001";
                        int mySide = parts.Length > 2 ? int.Parse(parts[2]) : 2;
                        string opponentName = parts.Length > 3 ? parts[3] : "Unknown";

                        // Xác định tên người chơi
                        string p1 = (mySide == 1) ? _loggedInUser : opponentName; // Host (Side 1)
                        string p2 = (mySide == 1) ? opponentName : _loggedInUser; // Guest (Side 2)

                        // Tạo Room ảo
                        Room newRoom = new Room(1, roomID, "");

                        // Hủy lắng nghe ở Dashboard để chuyển quyền cho PvP
                        if (_client != null) _client.OnMessageReceived -= Dashboard_OnMessageReceived;

                        // MỞ BÀN CỜ PVP
                        // Lưu ý: PvP constructor nhận mySide (1 hoặc 2) để biết mình là X hay O
                        // Nếu PvP của bạn dùng 1=Host, 2=Guest thì truyền mySide là đúng.
                        PvP pvpForm = new PvP(newRoom, mySide, p1, p2, _client);

                        // Khi tắt PvP thì hiện lại Dashboard và lắng nghe lại
                        pvpForm.FormClosed += (s, args) => {
                            this.Show();
                            RegisterServerListener(); // Nghe lại tin nhắn
                        };

                        this.Hide();
                        pvpForm.Show();
                    }
                }
                catch (Exception ex)
                {
                    // MessageBox.Show("Lỗi Dashboard: " + ex.Message);
                }
            });
        }

        // --- CÁC SỰ KIỆN NÚT BẤM ---

        private void btnPvE_Click(object sender, EventArgs e)
        {
            var newGameForm = new BotDifficulty(_loggedInUser);
            newGameForm.Show();
            this.Hide();
        }

        // NÚT PLAY INSTANT (QUICK MATCH)
        private void btnPlayInstant_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra kết nối
            if (_client == null || !_client.IsConnected())
            {
                MessageBox.Show("Mất kết nối Server! Vui lòng đăng nhập lại.", "Lỗi Mạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!isFindingMatch)
            {
                // Gửi lệnh tìm trận
                _client.Send("FIND_MATCH"); // Kiểm tra lại lệnh bên Server xem đúng không

                isFindingMatch = true;
                btnPlayInstant.Text = "Finding... (Cancel)";
            }
            else
            {
                // Hủy tìm trận
                _client.Send("CANCEL_FIND");

                isFindingMatch = false;
                btnPlayInstant.Text = "Play Instant";
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            // Cần truyền Client vào UserInfo để đổi pass được
            // Load lại thông tin user nếu cần thiết
            if (pv == null) LoadPlayer(_loggedInUser, "", ""); // Dummy load

            var userInfoForm = new UserInfo(pv, _client);
            userInfoForm.FormClosed += (s, args) => this.Show();
            this.Hide();
            userInfoForm.Show();
        }

        // --- CÁC HÀM CŨ (Giữ nguyên để tránh lỗi Designer) ---

        private void btnPvP_Click(object sender, EventArgs e)
        {
            if (pnlPvPMenu != null) pnlPvPMenu.Visible = !pnlPvPMenu.Visible;
        }

        private void btnCreateRoom_Click(object sender, EventArgs e)
        {
            // Logic cũ (Offline LAN)
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
            if (pnlDashBoard != null) pnlDashBoard.Visible = !pnlDashBoard.Visible;
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            string id = tbRoomID.Text.Trim();
            Room joinedRoom = RoomManager.JoinRoom(id, out int pNumber);

            if (joinedRoom == null)
            {
                MessageBox.Show("Phòng không tồn tại hoặc đã đủ người!");
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

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Hủy đăng ký sự kiện khi đóng form để tránh lỗi
            if (_client != null)
            {
                _client.OnMessageReceived -= Dashboard_OnMessageReceived;
            }
            base.OnFormClosing(e);
        }

        private void pnlNaviBar_Paint(object sender, PaintEventArgs e) { }
    }
}