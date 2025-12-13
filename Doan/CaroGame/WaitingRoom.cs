using System;
using System.Drawing;
using System.Windows.Forms;
using CaroGame_TCPClient;

namespace CaroGame
{
    public partial class WaitingRoom : Form
    {
        private readonly TCPClient _client;
        private readonly string _username;

        private string _roomId = "";
        private readonly bool _isHost;
        private bool _myReady = false;

        private Label _statusLabel;                 // tự tạo nếu form không có label
        private System.Windows.Forms.Timer _countdownTimer;
        private int _secondsLeft = 0;

        public WaitingRoom(TCPClient client, string username, bool isHost, string roomId = "")
        {
            InitializeComponent();

            _client = client;
            _username = username;
            _isHost = isHost;
            _roomId = roomId;

            EnsureStatusLabel();

            if (_client != null)
            {
                _client.OnMessageReceived -= OnServerMessage;
                _client.OnMessageReceived += OnServerMessage;
            }

            InitUI();

            if (_isHost)
                _client.Send($"CREATE_ROOM|{_username}");
            else
            {
                if (rtbID != null) rtbID.Text = _roomId;
                _client.Send($"JOIN_ROOM|{_username}|{_roomId}");
            }
        }

        private void EnsureStatusLabel()
        {
            // Nếu bạn có sẵn label tên khác thì set vào _statusLabel ở đây.
            // Còn không có thì tạo label mới để khỏi lỗi "label_status does not exist"
            _statusLabel = new Label();
            _statusLabel.AutoSize = true;
            _statusLabel.ForeColor = Color.DarkOrange;
            _statusLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _statusLabel.Visible = false;
            _statusLabel.Text = "";

            // đặt gần nút ready (nếu có)
            if (btnReady != null)
            {
                _statusLabel.Left = btnReady.Left;
                _statusLabel.Top = btnReady.Top;
            }
            else
            {
                _statusLabel.Left = 20;
                _statusLabel.Top = 20;
            }

            Controls.Add(_statusLabel);
            _statusLabel.BringToFront();
        }

        private void InitUI()
        {
            if (rtbID != null) rtbID.ReadOnly = true;

            if (ptbReady1 != null) ptbReady1.Visible = false;
            if (ptbReady2 != null) ptbReady2.Visible = false;

            if (btnReady != null) btnReady.Visible = true;

            _statusLabel.Visible = false;
            _statusLabel.Text = "";
        }

        private void OnServerMessage(string data)
        {
            if (IsDisposed || !IsHandleCreated) return;

            Invoke((MethodInvoker)(() =>
            {
                try
                {
                    string[] parts = data.Split('|');
                    string cmd = parts[0];

                    switch (cmd)
                    {
                        case "ROOM_CREATED":
                            _roomId = parts.Length > 1 ? parts[1] : "";
                            if (rtbID != null) rtbID.Text = _roomId;
                            break;

                        case "ROOM_JOINED":
                            _roomId = parts.Length > 1 ? parts[1] : _roomId;
                            if (rtbID != null) rtbID.Text = _roomId;
                            break;

                        case "READY_UPDATE":
                            if (parts.Length < 4) return;
                            bool hostReady = parts[2] == "1";
                            bool guestReady = parts[3] == "1";

                            if (ptbReady1 != null) ptbReady1.Visible = hostReady;
                            if (ptbReady2 != null) ptbReady2.Visible = guestReady;
                            break;

                        case "GAME_STARTING":
                            if (parts.Length < 2) return;
                            if (!int.TryParse(parts[1], out _secondsLeft)) _secondsLeft = 3;
                            StartCountdown();
                            break;

                        case "MATCH_FOUND":
                            // MATCH_FOUND|opponent|X/O
                            string opponent = parts.Length > 1 ? parts[1] : "Unknown";
                            string sideRaw = parts.Length > 2 ? parts[2] : "O";

                            int mySide = (sideRaw.ToUpper() == "X") ? 0 : 1;

                            string p1 = (mySide == 0) ? _username : opponent;
                            string p2 = (mySide == 0) ? opponent : _username;

                            var roomInfo = new Room();
                            var gameForm = new PvP(roomInfo, mySide, p1, p2, _client);

                            _client.OnMessageReceived -= OnServerMessage;

                            Hide();
                            gameForm.FormClosed += (s, e) => Close();
                            gameForm.Show();
                            break;

                        case "JOIN_FAIL":
                            string reason = parts.Length > 1 ? parts[1] : "Join thất bại.";
                            MessageBox.Show(reason, "Join Fail", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Close();
                            break;
                    }
                }
                catch { }
            }));
        }

        private void StartCountdown()
        {
            if (btnReady != null) btnReady.Visible = false;

            _statusLabel.Visible = true;
            _statusLabel.Text = $"Game starting in {_secondsLeft}";

            _countdownTimer?.Stop();
            _countdownTimer = new System.Windows.Forms.Timer();
            _countdownTimer.Interval = 1000;
            _countdownTimer.Tick += (s, e) =>
            {
                _statusLabel.Text = $"Game starting in {_secondsLeft}";
                _secondsLeft--;

                if (_secondsLeft < 0)
                    _countdownTimer.Stop();
            };
            _countdownTimer.Start();
        }

        private void btnReady_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_roomId) || _client == null) return;

            _myReady = !_myReady;
            _client.Send($"READY|{_username}|{_roomId}|{(_myReady ? 1 : 0)}");
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                if (_client != null)
                    _client.OnMessageReceived -= OnServerMessage;
            }
            catch { }

            base.OnFormClosing(e);
        }
    }
}
