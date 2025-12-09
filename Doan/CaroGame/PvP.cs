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

            // X luôn đi trước
            ChessBoard.IsMyTurn = (ChessBoard.MySide == 0);

            // 3. ĐĂNG KÝ SỰ KIỆN CLICK
            ChessBoard.PlayerClickedNode += ChessBoard_PlayerClickedNode;

            // 4. ĐĂNG KÝ NHẬN TIN TỪ SERVER
            if (tcpClient != null)
            {
                // Hủy đăng ký cũ nếu có để tránh duplicate
                tcpClient.OnMessageReceived -= HandleServerMessage;
                tcpClient.OnMessageReceived += HandleServerMessage;
            }

            ChessBoard.DrawChessBoard();
        }

        // --- GỬI DỮ LIỆU ---
        private void ChessBoard_PlayerClickedNode(Point point)
        {
            if (tcpClient != null && tcpClient.IsConnected())
            {
                // Gửi Packet MOVE (MOVE|X|Y)
                tcpClient.SendPacket(new Packet("MOVE", point));
            }
        }

        // --- [QUAN TRỌNG] XỬ LÝ TIN NHẮN TỪ SERVER ---
        private void HandleServerMessage(string data)
        {
            this.Invoke((MethodInvoker)delegate
            {
                try
                {
                    string[] parts = data.Split('|');
                    string command = parts[0];

                    if (command == "MOVE")
                    {
                        // [FIXED] Parse đúng thứ tự X, Y và Side
                        // Server gửi: MOVE | x | y | side

                        int x = int.Parse(parts[1]); // X là cột
                        int y = int.Parse(parts[2]); // Y là dòng
                        int side = int.Parse(parts[3]); // Bắt buộc phải có side từ Server

                        // Gọi hàm xử lý chung (Vẽ + Đổi lượt)
                        ChessBoard.ProcessMove(x, y, side);
                    }
                    else if (command == "CHAT")
                    {
                        if (parts.Length >= 2)
                        {
                            AppendMessage("Opponent", parts[1], Color.Red);
                        }
                    }
                    else if (command == "UNDO_SUCCESS")
                    {
                        // Khi Server đồng ý Undo -> Thực hiện xóa nước cờ
                        ChessBoard.ExecuteUndoPvP();

                        // Cập nhật lại UI nút undo
                        undoCount = true;
                        ptbOne.Visible = false;
                        ptbZero.Visible = true;
                    }
                    else if (command == "NEXT_TURN")
                    {
                        // Server gửi: NEXT_TURN|Username_Cua_Nguoi_Duoc_Di
                        // Logic này để đảm bảo chắc chắn lượt đi đúng (dự phòng cho ProcessMove)
                        string nextUser = parts[1];
                        if (nextUser == player1Name) // Nếu tên gửi về là tên mình
                        {
                            ChessBoard.IsMyTurn = true;
                        }
                        else
                        {
                            ChessBoard.IsMyTurn = false;
                        }
                    }
                    else if (command == "OPPONENT_LEFT")
                    {
                        MessageBox.Show("Đối thủ đã thoát trận! Bạn thắng.");
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    // Console.WriteLine(ex.Message);
                }
            });
        }

        // --- CÁC HÀM UI ---

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (undoCount) return; // Nếu đã dùng quyền undo rồi thì thôi

            if (tcpClient != null)
            {
                // Gửi yêu cầu Undo lên Server
                tcpClient.Send("REQUEST_UNDO");
                // KHÔNG tự gọi hàm Undo ở đây, phải chờ Server trả lời UNDO_SUCCESS
            }
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
            if (tcpClient != null)
            {
                tcpClient.OnMessageReceived -= HandleServerMessage;
                // Gửi lệnh thoát trận để Server báo cho đối thủ
                tcpClient.CancelMatch(player1Name);
            }

            this.Close();
            // Mở lại Dashboard
            var DashBoard = new Dashboard(room, playerNumber, player1Name, tcpClient);
            DashBoard.Show();
        }

        // --- GIỮ NGUYÊN PHẦN CÒN LẠI (Emoji, Menu, v.v.) ---

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

                // Lưu ý: Việc reset game trong PvP phải cẩn thận.
                // Thường thì chỉ Reset bàn cờ khi HẾT VÁN.
                // Nếu reset giữa chừng sẽ bị lệch với đối thủ.
                // Ở đây tạm thời ta khởi tạo lại Board.

                ChessBoard = new ChessBoardManager(pnlChessBoard, GameMode.PvP);
                ChessBoard.MySide = (playerNumber == 1) ? 0 : 1;
                ChessBoard.PlayerClickedNode += ChessBoard_PlayerClickedNode;
                ChessBoard.DrawChessBoard();

                // Set lại lượt
                ChessBoard.IsMyTurn = (ChessBoard.MySide == 0);
            }
            else
            {
                menuForm.Close();
                menuForm = null;
            }
        }

        // Emoji logic
        private readonly string[] _emoticons = new string[] { "😀", "😃", "😄", "😁", "😆", "😅", "😂", "🤣" }; // Rút gọn
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
        private void btnChat_Click(object sender, EventArgs e) { if (panelChat != null) panelChat.Visible = !panelChat.Visible; }
        private void Btn_Click(object? sender, EventArgs e) { } // Placeholder

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (tcpClient != null) tcpClient.OnMessageReceived -= HandleServerMessage;
            base.OnFormClosing(e);
        }
    }
}