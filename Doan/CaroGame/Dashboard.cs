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
                _client.OnMessageReceived -= Dashboard_OnMessageReceived;
                _client.OnMessageReceived += Dashboard_OnMessageReceived;
            }
        }

        // Dashboard chỉ cần bắt MATCH_FOUND cho quick match (nếu bạn còn dùng ở đây)
        private void Dashboard_OnMessageReceived(string data)
        {
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

                        // Nếu bạn vẫn dùng quick match kiểu cũ ở Dashboard:
                        if (command == "MATCH_FOUND")
                        {
                            string opponentName = parts.Length > 1 ? parts[1] : "Unknown";
                            string sideRaw = parts.Length > 2 ? parts[2] : "O";

                            int mySide = (sideRaw.ToUpper() == "X") ? 0 : 1;

                            string p1 = (mySide == 0) ? _loggedInUser : opponentName;
                            string p2 = (mySide == 0) ? opponentName : _loggedInUser;

                            Room newRoom = new Room();

                            if (_client != null) _client.OnMessageReceived -= Dashboard_OnMessageReceived;

                            PvP pvpForm = new PvP(newRoom, mySide, p1, p2, _client);
                            pvpForm.FormClosed += (s, args) =>
                            {
                                this.Show();
                                RegisterServerListener();
                            };

                            this.Hide();
                            pvpForm.Show();
                        }
                        else if (command == "JOIN_FAIL")
                        {
                            string reason = parts.Length > 1 ? parts[1] : "Lỗi không xác định";
                            MessageBox.Show(reason, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch { }
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

        // Quick match
        private void btnPlayInstant_Click(object sender, EventArgs e)
        {
            if (_client == null || !_client.IsConnected())
            {
                MessageBox.Show("Mất kết nối Server! Vui lòng đăng nhập lại.", "Lỗi Mạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_client != null) _client.OnMessageReceived -= Dashboard_OnMessageReceived;

            var waitingScreen = new PvPLobby(_loggedInUser, _client, true);

            this.Hide();
            waitingScreen.FormClosed += (s, args) =>
            {
                this.Show();
                RegisterServerListener();
            };
            waitingScreen.Show();
        }

        // Create Room -> mở WaitingRoom (host)
        private void btnCreateRoom_Click(object sender, EventArgs e)
        {
            if (_client == null || !_client.IsConnected())
            {
                MessageBox.Show("Mất kết nối!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_client != null) _client.OnMessageReceived -= Dashboard_OnMessageReceived;

            var waitingRoom = new WaitingRoom(_client, _loggedInUser, isHost: true);

            this.Hide();
            waitingRoom.FormClosed += (s, args) =>
            {
                this.Show();
                RegisterServerListener();
            };
            waitingRoom.Show();
        }

        // Join Room -> mở WaitingRoom (joiner)
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

            if (_client != null) _client.OnMessageReceived -= Dashboard_OnMessageReceived;

            var waitingRoom = new WaitingRoom(_client, _loggedInUser, isHost: false, roomId: id);

            this.Hide();
            waitingRoom.FormClosed += (s, args) =>
            {
                this.Show();
                RegisterServerListener();
            };
            waitingRoom.Show();
        }

        private bool _loggingOut = false;
        public void BeginLogout() => _loggingOut = true;

        private void btnSettings_Click(object sender, EventArgs e)
        {
            using (var ui = new UserInfo(pv, _client))
            {
                this.Hide();

                ui.ShowDialog(this); // this = Owner

                // ✅ nếu đang logout thì KHÔNG show lại dashboard nữa
                if (!_loggingOut && !this.IsDisposed)
                {
                    this.Show();
                    this.Activate();
                }
            }
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
            if (_client != null)
            {
                _client.OnMessageReceived -= Dashboard_OnMessageReceived;
            }
            base.OnFormClosing(e);
        }

        private void pnlNaviBar_Paint(object sender, PaintEventArgs e) { }
    }
}
