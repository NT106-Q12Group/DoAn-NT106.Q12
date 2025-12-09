using System;
using System.Drawing;
using System.Windows.Forms;
using CaroGame_TCPClient; // Đảm bảo using đúng namespace chứa TCPClient của bạn

namespace CaroGame
{
    public partial class PvPLobby : Form
    {
        private string _username;
        private TCPClient _client;
        private bool _isQuickMatch;

        // Constructor mặc định để tránh lỗi Designer
        public PvPLobby()
        {
            InitializeComponent();
        }

        // Constructor chính nhận dữ liệu từ Dashboard
        // Trong PvPLobby.cs
        public PvPLobby(string username, TCPClient client, bool isQuickMatch = false)
        {
            InitializeComponent();
            _username = username;
            _client = client;
            _isQuickMatch = isQuickMatch;

            // --- ĐĂNG KÝ NHẬN TIN NHẮN TẠI ĐÂY ---
            _client.OnMessageReceived += ProcessServerMessage;
        }

        // Khi đóng form nhớ hủy đăng ký để tránh lỗi
        private void PvPLobby_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_client != null) _client.OnMessageReceived -= ProcessServerMessage;
        }

        private void PvPLobby_Load(object sender, EventArgs e)
        {
            SetupUI();
        }

        private void SetupUI()
        {
            // 1. Hiển thị thông tin của CHÍNH MÌNH (Player 1)
            if (lb_username1 != null) lb_username1.Text = _username;

            // Set Avatar cho mình (Ví dụ màu Xanh)
            if (pictureBox1 != null)
            {
                pictureBox1.BackColor = Color.LightBlue;
                // pictureBox1.Image = ... (Nếu bạn có ảnh avatar)
            }

            // 2. Reset thông tin đối thủ (Player 2)
            if (lb_username2 != null) lb_username2.Text = "Waiting...";
            if (pictureBox2 != null) pictureBox2.Image = null; pictureBox2.BackColor = Color.Transparent;

            // 3. Nếu là chế độ Tìm trận nhanh -> Bắt đầu tìm ngay
            if (_isQuickMatch)
            {
                StartSearching();
            }
        }

        private void StartSearching()
        {
            // Cập nhật trạng thái UI
            if (lb_status != null)
            {
                lb_status.Text = "SEARCHING FOR OPPONENT...";
                lb_status.ForeColor = Color.Orange;
            }

            if (progressBar1 != null)
            {
                progressBar1.Visible = true;
                progressBar1.Style = ProgressBarStyle.Marquee; // Chạy hiệu ứng loading
            }

            // GỬI LỆNH TÌM TRẬN LÊN SERVER
            if (_client != null)
            {
                // Server code mới mong đợi: "FIND_MATCH;[Username]"
                _client.Send($"FIND_MATCH;{_username}");
            }
        }

        // ---------------------------------------------------------
        // HÀM QUAN TRỌNG: Gọi hàm này từ nơi bạn nhận dữ liệu (TCPClient)
        // ---------------------------------------------------------
        public void ProcessServerMessage(string message)
        {
            // Đảm bảo chạy trên luồng giao diện (UI Thread)
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(ProcessServerMessage), new object[] { message });
                return;
            }

            // Phân tích tin nhắn: MATCH_FOUND;TênĐốiThủ;RoomID
            string[] parts = message.Split(';');
            string command = parts[0];

            if (command == "MATCH_FOUND" && parts.Length >= 3)
            {
                string opponentName = parts[1];
                int roomId = int.Parse(parts[2]);

                OnMatchFound(opponentName, roomId);
            }
        }

        private void OnMatchFound(string opponentName, int roomId)
        {
            // 1. Dừng thanh loading
            if (progressBar1 != null) progressBar1.Visible = false;

            // 2. Thông báo tìm thấy
            if (lb_status != null)
            {
                lb_status.Text = "MATCH FOUND!";
                lb_status.ForeColor = Color.Green;
            }

            // 3. Hiển thị thông tin đối thủ
            if (lb_username2 != null) lb_username2.Text = opponentName;
            if (pictureBox2 != null) pictureBox2.BackColor = Color.Red; // Avatar đối thủ (màu Đỏ)

            // 4. Đợi 1.5 giây để người chơi nhìn thấy đối thủ rồi mới vào game
            // SỬA Ở ĐÂY: Thêm 'System.Windows.Forms.' vào trước Timer
            System.Windows.Forms.Timer transitionTimer = new System.Windows.Forms.Timer();
            transitionTimer.Interval = 1500; // 1.5s
            transitionTimer.Tick += (s, e) =>
            {
                transitionTimer.Stop();
                EnterGame(roomId, opponentName);
            };
            transitionTimer.Start();
        }

        private void EnterGame(int roomId, string opponentName)
        {
            // Tạo đối tượng Room với ID server cấp
            Room roomInfo = new Room();
            // Nếu class Room của bạn có thuộc tính ID, hãy gán nó: roomInfo.ID = roomId;

            // Chuyển sang form PvP
            // Giả sử server ghép xong thì bạn là Player 1 (hoặc bạn cần logic để xác định X/O sau)
            var gameForm = new PvP(roomInfo, 1, _username, opponentName, _client);

            gameForm.Show();
            this.Hide();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            // Nếu đang tìm trận mà bấm Back -> Gửi lệnh HỦY
            if (_client != null && _isQuickMatch)
            {
                _client.Send($"CANCEL_MATCH;{_username}");
            }

            // Quay về Dashboard
            Dashboard d = new Dashboard(_username, _client);
            d.Show();
            this.Close();
        }
    }
}