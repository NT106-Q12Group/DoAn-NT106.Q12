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

        // Constructor mặc định
        public PvPLobby()
        {
            InitializeComponent();
        }

        // Constructor chính
        public PvPLobby(string username, TCPClient client, bool isQuickMatch = true)
        {
            InitializeComponent();
            _username = username;
            _client = client;
            _isQuickMatch = isQuickMatch;

            // --- QUAN TRỌNG: Gắn sự kiện FormClosed thủ công ---
            // Để đảm bảo dù bấm nút Back hay nút X đỏ thì đều chạy hàm dọn dẹp
            this.FormClosed += PvPLobby_FormClosed;

            // Kiểm tra an toàn trước khi đăng ký sự kiện mạng
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

            // Căn giữa giao diện lúc mới mở
            CenterControls();
        }

        // --- CĂN GIỮA CÁC THÀNH PHẦN ---
        private void CenterControls()
        {
            if (lb_status != null)
            {
                lb_status.Left = (this.ClientSize.Width - lb_status.Width) / 2;
            }

            if (progressBar1 != null)
            {
                progressBar1.Left = (this.ClientSize.Width - progressBar1.Width) / 2;
            }
        }

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

            // Player 2 (Đối thủ - Đang chờ)
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

            // Tự động tìm trận nếu là QuickMatch
            if (_isQuickMatch && _client != null)
            {
                StartSearching();
            }
        }

        private void StartSearching()
        {
            if (lb_status != null)
            {
                lb_status.Text = "Finding Match...";
                lb_status.ForeColor = Color.DarkOrange;
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

        private void OnMatchFound(string opponentName, string mySymbol)
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

        // --- NÚT BACK (Sẽ kích hoạt FormClosed) ---
        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // --- SỰ KIỆN ĐÓNG FORM (Xử lý hủy trận) ---
        private void PvPLobby_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_client != null)
            {
                // 1. Hủy đăng ký sự kiện tin nhắn
                _client.OnMessageReceived -= ProcessServerMessage;

                // 2. Nếu thoát mà chưa tìm thấy trận -> Gửi lệnh HỦY
                if (!_matchFound && _isQuickMatch)
                {
                    _client.Send($"CANCEL_MATCH|{_username}");
                }
            }
        }

        private void progressBar1_Click(object sender, EventArgs e) { }
    }
}