using CaroGame_TCPClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CaroGame
{
    public partial class Dashboard : Form
    {
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

        private void RegisterServerListener()
        {
            if (_client != null)
            {
                // Ensure we don't subscribe multiple times
                _client.OnMessageReceived -= Dashboard_OnMessageReceived;
                _client.OnMessageReceived += Dashboard_OnMessageReceived;
            }
        }

        // --- [CORE] PROCESS MESSAGES FROM SERVER ---
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
                        int mySide = (parts.Length > 2 && int.TryParse(parts[2], out int side)) ? side : 2;
                        string opponentName = parts.Length > 3 ? parts[3] : "Unknown";

                        string p1 = (mySide == 0) ? _loggedInUser : opponentName; // Side 0 là X
                        string p2 = (mySide == 0) ? opponentName : _loggedInUser;

                        Room newRoom = new Room(roomID, p1, p2);

                        // Unsubscribe Dashboard before opening PvP
                        if (_client != null) _client.OnMessageReceived -= Dashboard_OnMessageReceived;

                        PvP pvpForm = new PvP(newRoom, mySide, p1, p2, _client);

                        pvpForm.FormClosed += (s, args) => {
                            this.Show();
                            RegisterServerListener(); // Re-subscribe when game ends
                        };

                        this.Hide();
                        pvpForm.Show();
                    }
                    // [MỚI] Xử lý khi vào phòng thất bại
                    else if (command == "JOIN_FAIL")
                    {
                        string reason = parts.Length > 1 ? parts[1] : "Lỗi không xác định";
                        MessageBox.Show(reason, "Lỗi vào phòng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                }
            });
        }

        // --- EVENT HANDLERS ---
        private void btnPvE_Click(object sender, EventArgs e)
        {
            var newGameForm = new BotDifficulty(_loggedInUser);
            newGameForm.FormClosed += (s, args) => this.Show();
            this.Hide();
            newGameForm.Show();
        }

        // --- PLAY INSTANT BUTTON (Quick Match) ---
        private void btnPlayInstant_Click(object sender, EventArgs e)
        {
            if (_client == null || !_client.IsConnected())
            {
                MessageBox.Show("Mất kết nối Server! Vui lòng đăng nhập lại.", "Lỗi Mạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_client != null)
                _client.OnMessageReceived -= Dashboard_OnMessageReceived;

            // Mở Lobby ở chế độ Quick Match (true)
            var waitingScreen = new PvPLobby(_loggedInUser, _client, true);

            this.Hide();
            waitingScreen.FormClosed += (s, args) =>
            {
                this.Show();
                RegisterServerListener();
            };
            waitingScreen.Show();
        }

        // --- [FIXED] CREATE ROOM BUTTON ---
        private void btnCreateRoom_Click(object sender, EventArgs e)
        {
            if (_client == null || !_client.IsConnected())
            {
                MessageBox.Show("Mất kết nối!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Gửi lệnh tạo phòng
            _client.Send($"CREATE_ROOM|{_loggedInUser}");

            // Tạm thời Unsubscribe để PvPLobby hứng sự kiện ROOM_CREATED
            if (_client != null) _client.OnMessageReceived -= Dashboard_OnMessageReceived;

            // Mở Lobby ở chế độ Custom (false)
            var waitingScreen = new PvPLobby(_loggedInUser, _client, false);

            this.Hide();
            waitingScreen.FormClosed += (s, args) =>
            {
                this.Show();
                RegisterServerListener();
            };
            waitingScreen.Show();
        }

        // --- [FIXED] JOIN ROOM BUTTON ---
        private void btnJoin_Click(object sender, EventArgs e)
        {
            // Lấy ID từ TextBox (Giả sử bạn có tbRoomID)
            string id = tbRoomID.Text.Trim();
            if (string.IsNullOrEmpty(id))
            {
                MessageBox.Show("Vui lòng nhập ID phòng!");
                return;
            }

            if (_client == null || !_client.IsConnected())
            {
                MessageBox.Show("Mất kết nối!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Gửi lệnh JOIN và chờ Server phản hồi (MATCH_FOUND hoặc JOIN_FAIL)
            // Dashboard vẫn lắng nghe nên không cần mở Lobby
            _client.Send($"JOIN_ROOM|{_loggedInUser}|{id}");
        }

        // ------------------------------------

        public void SetPlayer(PlayerView player)
        {
            pv = player;
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            var userInfoForm = new UserInfo(pv, _client);
            this.Hide();
            userInfoForm.FormClosed += (s, args) => this.Show();
            userInfoForm.Show();
        }

        private void btnPvP_Click(object sender, EventArgs e)
        {
            if (pnlPvPMenu != null) pnlPvPMenu.Visible = !pnlPvPMenu.Visible;
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

        private void LoadPlayer(string username, string email, string birthday)
        {
            pv = new PlayerView { PlayerID = 0, PlayerName = username, Email = email, Birthday = birthday };
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_client != null)
            {
                _client.OnMessageReceived -= Dashboard_OnMessageReceived;
            }
            base.OnFormClosing(e);
        }

        private void pnlNaviBar_Paint(object sender, PaintEventArgs e) { }
    }
}