using System;
using System.Drawing;
using System.Windows.Forms;
using CaroGame_TCPClient;

namespace CaroGame
{
    public partial class PvPLobby : Form
    {
        private string _username;
        private TCPClient _client;
        private bool _isQuickMatch;
        private bool _matchFound = false;

        public PvPLobby()
        {
            InitializeComponent();
        }

        public PvPLobby(string username, TCPClient client, bool isQuickMatch = true)
        {
            InitializeComponent();
            _username = username;
            _client = client;
            _isQuickMatch = isQuickMatch;

            this.FormClosed += PvPLobby_FormClosed;

            if (_client != null)
            {
                // Đăng ký nhận tin nhắn
                _client.OnMessageReceived += ProcessServerMessage;
            }
        }

        private void PvPLobby_Load(object sender, EventArgs e)
        {
            SetupUI();
            CenterControls();
        }

        public void ProcessServerMessage(string message)
        {
            // Đảm bảo chạy trên UI Thread
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(ProcessServerMessage), new object[] { message });
                return;
            }

            try
            {
                // [DEBUG] Uncomment dòng này để chắc chắn tin nhắn đã đến Client
                // MessageBox.Show("Lobby nhận: " + message); 

                string[] parts = message.Trim().Split('|');
                string command = parts[0];

                if (command == "MATCH_FOUND")
                {
                    // Hủy đăng ký sự kiện ngay để không nhận tin trùng lặp
                    if (_client != null) _client.OnMessageReceived -= ProcessServerMessage;

                    // Kiểm tra kỹ độ dài mảng để tránh lỗi IndexOutOfRange
                    string opponentName = parts.Length > 1 ? parts[1] : "Unknown";
                    string sideRaw = parts.Length > 2 ? parts[2] : "O";

                    // Gọi hàm chuyển cảnh
                    OnMatchFound(opponentName, sideRaw);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Lobby: " + ex.Message);
            }
        }

        private void OnMatchFound(string opponentName, string sideRaw)
        {
            if (progressBar1 != null) progressBar1.Visible = false;

            if (lb_status != null)
            {
                lb_status.Text = "OPPONENT FOUND!";
                lb_status.ForeColor = Color.Green;
                CenterControls();
            }

            if (lb_username2 != null)
            {
                lb_username2.Text = opponentName;
                lb_username2.ForeColor = Color.Black;
                lb_username2.Font = new Font(lb_username2.Font, FontStyle.Bold);
            }

            if (pictureBox2 != null) pictureBox2.BackColor = Color.PaleVioletRed;

            // Đếm ngược 1.5s rồi vào game
            System.Windows.Forms.Timer transitionTimer = new System.Windows.Forms.Timer();
            transitionTimer.Interval = 1500;
            transitionTimer.Tick += (s, e) =>
            {
                transitionTimer.Stop();
                EnterGame(opponentName, sideRaw);
            };
            transitionTimer.Start();
        }

        private void EnterGame(string opponentName, string sideRaw)
        {
            // Hủy đăng ký sự kiện ở Lobby để tránh xung đột với bàn cờ
            if (_client != null) _client.OnMessageReceived -= ProcessServerMessage;

            Room roomInfo = new Room();
            string p1, p2;
            int mySide; // 0 = X, 1 = O

            // [FIX LOGIC]: Xử lý xác định phe dựa trên tin nhắn Server
            // Nếu sideRaw là "1" hoặc "X" -> Mình là Player 1 (X) -> mySide = 0
            if (sideRaw == "1" || sideRaw.ToUpper() == "X")
            {
                p1 = _username;      // Mình là X
                p2 = opponentName;   // Đối thủ là O
                mySide = 0;          // Phe 0 đi trước
            }
            else
            {
                p1 = opponentName;   // Đối thủ là X
                p2 = _username;      // Mình là O
                mySide = 1;          // Phe 1 đi sau
            }

            // Mở bàn cờ với Constructor mới (nhận mySide)
            var gameForm = new PvP(roomInfo, mySide, p1, p2, _client);

            this.Hide();

            // Khi bàn cờ đóng thì đóng luôn Lobby
            gameForm.FormClosed += (s, args) => this.Close();

            gameForm.Show();
        }

        // --- CÁC HÀM UI (GIỮ NGUYÊN) ---
        private void CenterControls()
        {
            if (lb_status != null) lb_status.Left = (this.ClientSize.Width - lb_status.Width) / 2;
            if (progressBar1 != null) progressBar1.Left = (this.ClientSize.Width - progressBar1.Width) / 2;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CenterControls();
        }

        private void SetupUI()
        {
            if (lb_username1 != null) { lb_username1.Text = _username; lb_username1.ForeColor = Color.Black; }
            if (pictureBox1 != null) pictureBox1.BackColor = Color.LightBlue;

            if (lb_username2 != null) { lb_username2.Text = "???"; lb_username2.ForeColor = Color.DimGray; }
            if (pictureBox2 != null) { pictureBox2.Image = null; pictureBox2.BackColor = Color.Silver; }

            if (lb_status != null) lb_status.Text = "Waiting...";
            if (progressBar1 != null) progressBar1.Visible = false;

            if (_isQuickMatch && _client != null) StartSearching();
        }

        private void StartSearching()
        {
            if (lb_status != null) { lb_status.Text = "Finding Match..."; lb_status.ForeColor = Color.DarkOrange; CenterControls(); }
            if (progressBar1 != null) { progressBar1.Visible = true; progressBar1.Style = ProgressBarStyle.Marquee; CenterControls(); }

            if (_client != null) _client.Send($"FIND_MATCH|{_username}");
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PvPLobby_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_client != null)
            {
                _client.OnMessageReceived -= ProcessServerMessage;
                if (!_matchFound && _isQuickMatch)
                {
                    _client.Send($"CANCEL_MATCH|{_username}");
                }
            }
        }

        private void progressBar1_Click(object sender, EventArgs e) { }
    }
}