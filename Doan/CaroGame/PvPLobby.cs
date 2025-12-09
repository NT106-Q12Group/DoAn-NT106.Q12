using System;
using System.Drawing;
using System.Windows.Forms;
using CaroGame_TCPClient; // Namespace chứa TCPClient

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

            // Kiểm tra an toàn trước khi đăng ký sự kiện
            if (_client != null)
            {
                _client.OnMessageReceived += ProcessServerMessage;
            }
            else
            {
                MessageBox.Show("Lỗi: Mất kết nối tới Server (Client is null). Vui lòng đăng nhập lại.", "Lỗi Kết Nối");
            }
        }

        private void PvPLobby_Load(object sender, EventArgs e)
        {
            SetupUI();

            // Căn giữa lần đầu khi form hiện lên
            CenterControls();
        }

        // --- HÀM MỚI: CĂN GIỮA CÁC THÀNH PHẦN ---
        private void CenterControls()
        {
            // Căn giữa Label trạng thái
            if (lb_status != null)
            {
                // Tính toán: (Chiều rộng Form - Chiều rộng Label) / 2
                lb_status.Left = (this.ClientSize.Width - lb_status.Width) / 2;
            }

            // Căn giữa thanh Loading
            if (progressBar1 != null)
            {
                progressBar1.Left = (this.ClientSize.Width - progressBar1.Width) / 2;
            }
        }

        // Để đảm bảo khi resize form nó vẫn nằm giữa
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            CenterControls();
        }

        private void SetupUI()
        {
            // Player 1 (Mình)
            if (lb_username1 != null)
            {
                lb_username1.Text = _username;
                lb_username1.ForeColor = Color.Black;
            }
            if (pictureBox1 != null) pictureBox1.BackColor = Color.LightBlue;

            // Player 2 (Đối thủ)
            if (lb_username2 != null)
            {
                lb_username2.Text = "???";
                lb_username2.ForeColor = Color.DimGray;
                lb_username2.Font = new Font(lb_username2.Font, FontStyle.Italic);
            }
            if (pictureBox2 != null)
            {
                pictureBox2.Image = null;
                pictureBox2.BackColor = Color.Silver;
            }

            if (lb_status != null) lb_status.Text = "Waiting...";
            if (progressBar1 != null) progressBar1.Visible = false;

            if (_isQuickMatch && _client != null)
            {
                StartSearching();
            }
        }

        private void StartSearching()
        {
            if (lb_status != null)
            {
                // ĐỔI TEXT THEO Ý BẠN
                lb_status.Text = "Finding Match...";
                lb_status.ForeColor = Color.DarkOrange;

                // Căn giữa lại sau khi đổi text (vì độ dài chữ thay đổi)
                CenterControls();
            }

            if (progressBar1 != null)
            {
                progressBar1.Visible = true;
                progressBar1.Style = ProgressBarStyle.Marquee;
                CenterControls();
            }

            if (_client != null)
            {
                _client.Send($"FIND_MATCH|{_username}");
            }
        }

        public void ProcessServerMessage(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(ProcessServerMessage), new object[] { message });
                return;
            }

            string[] parts = message.Split('|');
            string command = parts[0];

            if (command == "MATCH_FOUND" && parts.Length >= 2)
            {
                _matchFound = true;
                string opponentName = parts[1];
                string mySymbol = parts.Length > 2 ? parts[2] : "O";

                OnMatchFound(opponentName, mySymbol);
            }
        }

        private void OnMatchFound(string opponentName, string mySymbol)
        {
            if (progressBar1 != null) progressBar1.Visible = false;

            if (lb_status != null)
            {
                lb_status.Text = "OPPONENT FOUND!";
                lb_status.ForeColor = Color.Green;
                CenterControls(); // Căn giữa lại
            }

            if (lb_username2 != null)
            {
                lb_username2.Text = opponentName;
                lb_username2.ForeColor = Color.Black;
                lb_username2.Font = new Font(lb_username2.Font, FontStyle.Bold);
            }

            if (pictureBox2 != null) pictureBox2.BackColor = Color.PaleVioletRed;

            System.Windows.Forms.Timer transitionTimer = new System.Windows.Forms.Timer();
            transitionTimer.Interval = 1500;
            transitionTimer.Tick += (s, e) =>
            {
                transitionTimer.Stop();
                EnterGame(opponentName, mySymbol);
            };
            transitionTimer.Start();
        }

        private void EnterGame(string opponentName, string mySymbol)
        {
            Room roomInfo = new Room();
            string p1, p2;
            int myNumber;

            if (mySymbol == "X")
            {
                p1 = _username; p2 = opponentName; myNumber = 1;
            }
            else
            {
                p1 = opponentName; p2 = _username; myNumber = 2;
            }

            var gameForm = new PvP(roomInfo, myNumber, p1, p2, _client);

            this.Hide();
            gameForm.FormClosed += (s, args) => this.Close();
            gameForm.Show();
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