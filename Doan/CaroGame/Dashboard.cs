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
                // Hủy đăng ký cũ trước khi đăng ký mới để tránh nhận tin nhắn 2 lần (duplicate events)
                _client.OnMessageReceived -= Dashboard_OnMessageReceived;
                _client.OnMessageReceived += Dashboard_OnMessageReceived;
            }
        }

        private void Dashboard_OnMessageReceived(string data)
        {
            // Kiểm tra nếu Form đã đóng hoặc chưa tạo Handle thì dừng ngay để tránh lỗi Invoke khi đóng app
            if (this.IsDisposed || !this.IsHandleCreated) return;

            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    if (this.IsDisposed) return;

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

                            string p1 = (mySide == 0) ? _loggedInUser : opponentName; // Side 0 là X (đi trước)
                            string p2 = (mySide == 0) ? opponentName : _loggedInUser;

                            Room newRoom = new Room(roomID, p1, p2);

                            // Hủy đăng ký Dashboard trước khi mở PvP để tránh xung đột xử lý tin nhắn
                            if (_client != null) _client.OnMessageReceived -= Dashboard_OnMessageReceived;

                            PvP pvpForm = new PvP(newRoom, mySide, p1, p2, _client);

                            pvpForm.FormClosed += (s, args) => {
                                this.Show();
                                RegisterServerListener(); // Đăng ký lại lắng nghe khi kết thúc trận đấu
                            };

                            this.Hide();
                            pvpForm.Show();
                        }
                        else if (command == "JOIN_FAIL")
                        {
                            string reason = parts.Length > 1 ? parts[1] : "Lỗi không xác định";
                            MessageBox.Show(reason, "Lỗi vào phòng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex) { }
                });
            }
            catch { }
        }

        private void btnPvE_Click(object sender, EventArgs e)
        {
            var newGameForm = new BotDifficulty(_loggedInUser);
            newGameForm.FormClosed += (s, args) => this.Show();
            this.Hide();
            newGameForm.Show();
        }

        private void btnPlayInstant_Click(object sender, EventArgs e)
        {
            if (_client == null || !_client.IsConnected())
            {
                MessageBox.Show("Mất kết nối Server! Vui lòng đăng nhập lại.", "Lỗi Mạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_client != null) _client.OnMessageReceived -= Dashboard_OnMessageReceived;

            var waitingScreen = new PvPLobby(_loggedInUser, _client, true); // true = Quick Match

            this.Hide();
            waitingScreen.FormClosed += (s, args) =>
            {
                this.Show();
                RegisterServerListener();
            };
            waitingScreen.Show();
        }

        private void btnCreateRoom_Click(object sender, EventArgs e)
        {
            if (_client == null || !_client.IsConnected())
            {
                MessageBox.Show("Mất kết nối!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _client.Send($"CREATE_ROOM|{_loggedInUser}");

            if (_client != null) _client.OnMessageReceived -= Dashboard_OnMessageReceived;

            var waitingScreen = new PvPLobby(_loggedInUser, _client, false); // false = Custom Room (Chờ ID)

            this.Hide();
            waitingScreen.FormClosed += (s, args) =>
            {
                this.Show();
                RegisterServerListener();
            };
            waitingScreen.Show();
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
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

            // Gửi lệnh Join và chờ Server phản hồi (Dashboard vẫn đang lắng nghe để hứng MATCH_FOUND hoặc JOIN_FAIL)
            _client.Send($"JOIN_ROOM|{_loggedInUser}|{id}");
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            // Hủy sự kiện ở Dashboard để tránh xung đột với UserInfo (nếu UserInfo cũng dùng mạng)
            if (_client != null) _client.OnMessageReceived -= Dashboard_OnMessageReceived;

            var userInfoForm = new UserInfo(pv, _client);
            this.Hide();

            userInfoForm.FormClosed += (s, args) =>
            {
                this.Show();
                RegisterServerListener();
            };

            userInfoForm.Show();
        }

        public void SetPlayer(PlayerView player)
        {
            pv = player;
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
            // Hủy đăng ký sự kiện khi đóng form để tránh gọi Invoke lên form đã đóng
            if (_client != null)
            {
                _client.OnMessageReceived -= Dashboard_OnMessageReceived;
            }
            base.OnFormClosing(e);
        }

        private void pnlNaviBar_Paint(object sender, PaintEventArgs e) { }
    }
}