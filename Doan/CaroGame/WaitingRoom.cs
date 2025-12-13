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

        // UI runtime
        private Label _statusLabel;
        private System.Windows.Forms.Timer _countdownTimer;
        private int _secondsLeft = 0;

        // players
        private string _hostName = "";
        private string _guestName = "";

        // ===== Slot mapping (auto-detect left/right) =====
        private Label _lbLeft;
        private Label _lbRight;
        private PictureBox _pbLeft;
        private PictureBox _pbRight;

        public WaitingRoom(TCPClient client, string username, bool isHost, string roomId = "")
        {
            InitializeComponent();

            _client = client;
            _username = username;
            _isHost = isHost;
            _roomId = roomId;

            EnsureStatusLabel();
            InitUI();

            if (_client != null)
            {
                _client.OnMessageReceived -= OnServerMessage;
                _client.OnMessageReceived += OnServerMessage;
            }

            // Set UI user ngay từ đầu (đỡ trống)
            if (_isHost)
            {
                _hostName = _username;
                _guestName = "???";
                UpdatePlayerUI();
                _client.Send($"CREATE_ROOM|{_username}");
            }
            else
            {
                _guestName = _username;
                _hostName = "???";
                UpdatePlayerUI();

                if (rtbID != null) rtbID.Text = _roomId;
                _client.Send($"JOIN_ROOM|{_username}|{_roomId}");
            }
        }

        private void EnsureStatusLabel()
        {
            _statusLabel = new Label();
            _statusLabel.AutoSize = true;
            _statusLabel.ForeColor = Color.DarkOrange;
            _statusLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _statusLabel.Visible = false;
            _statusLabel.Text = "";

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
            // ===== Detect slots by position =====
            DetectSlots();

            // ===== Center Room ID =====
            CenterRoomId();

            if (rtbID != null) rtbID.ReadOnly = true;

            if (ptbReady1 != null) ptbReady1.Visible = false;
            if (ptbReady2 != null) ptbReady2.Visible = false;

            if (btnReady != null) btnReady.Visible = true;

            _statusLabel.Visible = false;
            _statusLabel.Text = "";

            // avatar tạm thời để trống
            if (pictureBox2 != null)
            {
                pictureBox2.Image = null;
                pictureBox2.BackColor = Color.LightGray;
            }
            if (pictureBox3 != null)
            {
                pictureBox3.Image = null;
                pictureBox3.BackColor = Color.LightGray;
            }

            UpdatePlayerUI();

            // nếu form resize thì vẫn giữ căn giữa
            this.Resize -= WaitingRoom_Resize;
            this.Resize += WaitingRoom_Resize;
        }

        private void WaitingRoom_Resize(object sender, EventArgs e)
        {
            CenterRoomId();
            CenterNameLabels();
            CenterStatusLabel();
        }

        private void DetectSlots()
        {
            // labels: label2 + label6
            if (label2 != null && label6 != null)
            {
                if (label2.Left <= label6.Left) { _lbLeft = label2; _lbRight = label6; }
                else { _lbLeft = label6; _lbRight = label2; }
            }
            else
            {
                _lbLeft = label2;
                _lbRight = label6;
            }

            // pictureBoxes: pictureBox2 + pictureBox3
            if (pictureBox2 != null && pictureBox3 != null)
            {
                if (pictureBox2.Left <= pictureBox3.Left) { _pbLeft = pictureBox2; _pbRight = pictureBox3; }
                else { _pbLeft = pictureBox3; _pbRight = pictureBox2; }
            }
            else
            {
                _pbLeft = pictureBox2;
                _pbRight = pictureBox3;
            }
        }

        private void CenterRoomId()
        {
            if (rtbID == null) return;

            // Căn giữa TEXT trong RichTextBox
            rtbID.SelectAll();
            rtbID.SelectionAlignment = HorizontalAlignment.Center;
            rtbID.DeselectAll();

            // Căn giữa CONTROL theo form
            int centerX = (ClientSize.Width - rtbID.Width) / 2;
            if (centerX < 0) centerX = 0;
            rtbID.Left = centerX;
        }

        private void UpdatePlayerUI()
        {
            // Quy ước: trái = host, phải = guest
            if (_lbLeft != null) _lbLeft.Text = string.IsNullOrWhiteSpace(_hostName) ? "???" : _hostName;
            if (_lbRight != null) _lbRight.Text = string.IsNullOrWhiteSpace(_guestName) ? "???" : _guestName;

            CenterNameLabels();
        }

        private void CenterNameLabels()
        {
            // Căn giữa label dưới đúng pictureBox slot (không bị đảo nữa)
            CenterLabelUnderPictureBox(_lbLeft, _pbLeft, 8);
            CenterLabelUnderPictureBox(_lbRight, _pbRight, 8);
        }

        private void CenterLabelUnderPictureBox(Label lb, PictureBox pb, int yOffset)
        {
            if (lb == null || pb == null) return;

            lb.AutoSize = true;
            lb.TextAlign = ContentAlignment.MiddleCenter;

            lb.Left = pb.Left + (pb.Width - lb.Width) / 2;
            lb.Top = pb.Bottom + yOffset;
        }

        private void CenterStatusLabel()
        {
            if (_statusLabel == null) return;

            if (btnReady != null)
            {
                _statusLabel.Left = btnReady.Left + (btnReady.Width - _statusLabel.Width) / 2;
                _statusLabel.Top = btnReady.Top + (btnReady.Height - _statusLabel.Height) / 2;
            }
        }

        private void OnServerMessage(string data)
        {
            if (IsDisposed || !IsHandleCreated) return;

            BeginInvoke((MethodInvoker)(() =>
            {
                try
                {
                    string[] parts = data.Split('|');
                    string cmd = parts[0];

                    switch (cmd)
                    {
                        case "ROOM_CREATED":
                            // ROOM_CREATED|roomId|username
                            _roomId = parts.Length > 1 ? parts[1] : _roomId;
                            _hostName = parts.Length > 2 ? parts[2] : _hostName;
                            _guestName = "???";

                            if (rtbID != null) rtbID.Text = _roomId;
                            CenterRoomId();
                            UpdatePlayerUI();
                            break;

                        case "ROOM_JOINED":
                            // ROOM_JOINED|roomId|host|guest
                            _roomId = parts.Length > 1 ? parts[1] : _roomId;
                            _hostName = parts.Length > 2 ? parts[2] : _hostName;
                            _guestName = parts.Length > 3 ? parts[3] : _guestName;

                            if (rtbID != null) rtbID.Text = _roomId;
                            CenterRoomId();
                            UpdatePlayerUI();
                            break;

                        case "READY_UPDATE":
                            // READY_UPDATE|roomId|hostReady|guestReady
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
            CenterStatusLabel();

            _countdownTimer?.Stop();
            _countdownTimer = new System.Windows.Forms.Timer();
            _countdownTimer.Interval = 1000;

            _countdownTimer.Tick += (s, e) =>
            {
                _statusLabel.Text = $"Game starting in {_secondsLeft}";
                CenterStatusLabel();
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
