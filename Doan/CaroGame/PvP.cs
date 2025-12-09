using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using CaroGame_TCPClient;

namespace CaroGame
{
    public partial class PvP : Form
    {
        #region Properties
        ChessBoardManager ChessBoard;
        #endregion

        public Room room;
        public int playerNumber; // 1 = Host (X), 2 = Guest (O)
        private bool undoCount = false;

        // Các biến kết nối mạng
        private string player1Name;
        private string player2Name;
        private TCPClient tcpClient;

        // --- CONSTRUCTOR ---
        public PvP(Room room, int playerNumber, string p1, string p2, TCPClient client)
        {
            InitializeComponent();
            this.room = room;
            this.playerNumber = playerNumber;
            this.player1Name = p1;
            this.player2Name = p2;
            this.tcpClient = client;

            InitGame();
        }

        // Constructor cũ
        public PvP(Room room, int playerNumber)
        {
            InitializeComponent();
            this.room = room;
            this.playerNumber = playerNumber;
            InitGame();
        }

        private void InitGame()
        {
            CheckForIllegalCrossThreadCalls = false;
            SetupEmojiPickerPanel();

            // 1. Khởi tạo bàn cờ PvP
            ChessBoard = new ChessBoardManager(pnlChessBoard, GameMode.PvP);

            // 2. Xác định phe (MySide)
            // Host (1) là quân X (0), Guest (2) là quân O (1)
            ChessBoard.MySide = (playerNumber == 1) ? 0 : 1;

            // Nếu MySide == 0 (X) thì được đánh trước
            ChessBoard.IsMyTurn = (ChessBoard.MySide == 0);

            // 3. ĐĂNG KÝ SỰ KIỆN CLICK (Gửi tọa độ lên Server)
            ChessBoard.PlayerClickedNode += ChessBoard_PlayerClickedNode;

            // 4. [FIX QUAN TRỌNG] Đăng ký nhận tin nhắn từ Server
            // Thay vì tạo Thread loop, ta hứng sự kiện từ TCPClient
            if (tcpClient != null)
            {
                tcpClient.OnMessageReceived += HandleServerMessage;
            }

            ChessBoard.DrawChessBoard();
        }

        // --- GỬI DỮ LIỆU ---
        private void ChessBoard_PlayerClickedNode(Point point)
        {
            if (tcpClient != null && tcpClient.IsConnected())
            {
                // Gửi Packet MOVE (Server sẽ broadcast lại cho cả 2 người)
                tcpClient.SendPacket(new Packet("MOVE", point));
            }
        }

        // --- [FIX] XỬ LÝ TIN NHẮN TỪ SERVER (Thay thế ListenFromServer) ---
        private void HandleServerMessage(string data)
        {
            // Đảm bảo chạy trên luồng UI
            this.Invoke((MethodInvoker)delegate
            {
                try
                {
                    string[] parts = data.Split('|');
                    string command = parts[0];

                    if (command == "MOVE")
                    {
                        // Server gửi: MOVE|Row|Col|PlayerSide
                        // (Lưu ý: Bạn cần đảm bảo Server gửi đủ 4 tham số này)
                        int r = int.Parse(parts[1]);
                        int c = int.Parse(parts[2]);

                        // Nếu Server chưa gửi side, ta tạm suy luận (nhưng tốt nhất Server nên gửi)
                        // Ở đây mình giả định Server gửi kèm Side ở vị trí 3
                        int side = (parts.Length > 3) ? int.Parse(parts[3]) : -1;

                        // Nếu Server cũ chỉ gửi Row|Col, ta phải đoán phe:
                        if (side == -1)
                        {
                            // Nếu đến lượt mình mà nhận được MOVE -> Thì đó là move của Địch
                            // (Logic này hơi rủi ro, nên fix Server gửi Side là tốt nhất)
                            side = ChessBoard.IsMyTurn ? (ChessBoard.MySide == 0 ? 1 : 0) : ChessBoard.MySide;
                        }

                        // Gọi hàm vẽ thống nhất
                        ChessBoard.ProcessMove(c, r, side);
                    }
                    else if (command == "CHAT")
                    {
                        // CHAT|Message
                        if (parts.Length >= 2)
                        {
                            string msg = parts[1];
                            AppendMessage("Opponent", msg, Color.Red);
                        }
                    }
                    else if (command == "UNDO_SUCCESS")
                    {
                        // [FIX] Khi Server đồng ý Undo -> Thực hiện xóa nước cờ
                        ChessBoard.ExecuteUndoPvP();

                        // Reset lại trạng thái nút Undo nếu cần
                        undoCount = true;
                        ptbOne.Visible = false;
                        ptbZero.Visible = true;
                    }
                    else if (command == "NEXT_TURN")
                    {
                        // Nếu Server quản lý lượt đi chặt chẽ
                        // NEXT_TURN|Username
                        // if (parts[1] == myUsername) ChessBoard.IsMyTurn = true;
                    }
                }
                catch (Exception ex)
                {
                    // Log error nếu cần
                }
            });
        }

        // --- CÁC HÀM UI KHÁC ---

        private void OnGameEnded(string winner)
        {
            MessageBox.Show($"{winner} chiến thắng!!!");
            resetChess();
        }

        private void resetChess()
        {
            ChessBoard.resetGame();
            undoCount = false;
            ptbOne.Visible = true;
            ptbZero.Visible = false;

            // Khi reset game PvP, cần set lại lượt đúng luật
            ChessBoard.IsMyTurn = (ChessBoard.MySide == 0);
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string text = txtMessage.Text.Trim();
            if (string.IsNullOrWhiteSpace(text)) return;

            AppendMessage("You", text, Color.Blue);

            if (tcpClient != null)
                tcpClient.SendPacket(new Packet("CHAT", text));

            txtMessage.Clear();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            // Hủy đăng ký sự kiện trước khi thoát để tránh lỗi
            if (tcpClient != null)
                tcpClient.OnMessageReceived -= HandleServerMessage;

            this.Close();
            var DashBoard = new Dashboard(room, playerNumber, player1Name, tcpClient);
            DashBoard.Show();
        }

        // [FIX] Nút Undo giờ sẽ gửi YÊU CẦU lên Server
        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (undoCount) return; // Đã undo rồi thì thôi

            if (tcpClient != null)
            {
                // Gửi lệnh xin Undo
                tcpClient.Send("REQUEST_UNDO");

                // Lưu ý: Không gọi ChessBoard.ExecuteUndoPvP() ở đây!
                // Phải đợi Server trả về "UNDO_SUCCESS" trong HandleServerMessage thì mới gọi.
            }
        }

        // ... (Giữ nguyên các hàm UI Chat, Emoji, Menu) ...

        // Code giữ nguyên để tránh lỗi Designer
        private void Btn_Click(object? sender, EventArgs e) { Button btn = sender as Button; }

        private void btnChat_Click(object sender, EventArgs e) { if (panelChat != null) panelChat.Visible = !panelChat.Visible; }

        private void AppendMessage(string sender, string message, Color color)
        {
            if (rtbChat == null) return;
            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.SelectionColor = color;
            rtbChat.SelectionFont = new Font("Segoe UI", 10, FontStyle.Bold);
            rtbChat.AppendText($"{sender}: ");
            rtbChat.SelectionFont = new Font("Segoe UI", 10, FontStyle.Regular);
            rtbChat.SelectionColor = Color.Black;
            rtbChat.AppendText(message + Environment.NewLine + Environment.NewLine);
            rtbChat.ScrollToCaret();
        }

        private Menu menuForm;
        private void btnMenu_Click(object sender, EventArgs e)
        {
            if (menuForm == null || menuForm.IsDisposed)
            {
                menuForm = new Menu();
                menuForm.StartPosition = FormStartPosition.Manual;
                menuForm.Location = new Point(this.Left + 22, this.Top + 50);
                menuForm.Show(this);

                // Khi reset trong Menu, nhớ gán lại các Event
                ChessBoard = new ChessBoardManager(pnlChessBoard, GameMode.PvP);
                ChessBoard.MySide = (playerNumber == 1) ? 0 : 1;
                ChessBoard.PlayerClickedNode += ChessBoard_PlayerClickedNode;
                ChessBoard.DrawChessBoard();
            }
            else
            {
                menuForm.Close();
                menuForm = null;
            }
        }

        // Emoji logic (Giữ nguyên)
        private readonly string[] _emoticons = new string[] { "😀", "😃", "😄", "😁" /*...*/ }; // (Cắt bớt cho gọn)
        private void SetupEmojiPickerPanel()
        {
            if (pnlEmojiPicker == null) return;
            pnlEmojiPicker.Visible = false;
            pnlEmojiPicker.Controls.Clear();
            pnlEmojiPicker.AutoScroll = true;
        }
        private void ShowEmojiPicker()
        {
            if (pnlEmojiPicker == null) return;
            if (pnlEmojiPicker.Visible && pnlEmojiPicker.Controls.Count > 0) { pnlEmojiPicker.Visible = false; return; }
            pnlEmojiPicker.Visible = true;
            pnlEmojiPicker.BringToFront();
            pnlEmojiPicker.Controls.Clear();
            int btnSize = 32; int cols = 8; int spacing = 4;
            for (int i = 0; i < _emoticons.Length; i++)
            {
                var btn = new Button();
                btn.Font = new Font("Segoe UI Emoji", 16F, FontStyle.Regular);
                btn.Text = _emoticons[i];
                btn.Width = btn.Height = btnSize;
                int col = i % cols; int row = i / cols;
                btn.Left = col * (btnSize + spacing); btn.Top = row * (btnSize + spacing);
                btn.Click += (s, e) => { txtMessage.Text += ((Button)s).Text; txtMessage.SelectionStart = txtMessage.Text.Length; txtMessage.Focus(); };
                pnlEmojiPicker.Controls.Add(btn);
            }
        }
        private void btn_emoji_Click(object sender, EventArgs e) { ShowEmojiPicker(); }

        // Hủy đăng ký khi đóng form
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (tcpClient != null) tcpClient.OnMessageReceived -= HandleServerMessage;
            base.OnFormClosing(e);
        }
    }
}