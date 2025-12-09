using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using CaroGame_TCPClient; // Namespace chứa TCPClient và Packet

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

        // Luồng lắng nghe tin nhắn từ Server
        private Thread listenThread;
        private bool _stopListening = false;

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

        // Constructor cũ (giữ để tránh lỗi Designer)
        public PvP(Room room, int playerNumber)
        {
            InitializeComponent();
            this.room = room;
            this.playerNumber = playerNumber;
            InitGame();
        }

        private void InitGame()
        {
            // Tắt check thread để update UI từ luồng mạng dễ hơn
            CheckForIllegalCrossThreadCalls = false;

            SetupEmojiPickerPanel();

            // 1. Khởi tạo bàn cờ PvP
            ChessBoard = new ChessBoardManager(pnlChessBoard, GameMode.PvP);

            // 2. Xác định phe (MySide)
            // Host (1) là quân X (0), Guest (2) là quân O (1)
            ChessBoard.MySide = (playerNumber == 1) ? 0 : 1;
            // X luôn đi trước -> nếu MySide == 0 thì mình được đánh trước
            ChessBoard.IsMyTurn = (ChessBoard.MySide == 0);

            // 3. ĐĂNG KÝ SỰ KIỆN GỬI (Quan trọng!)
            ChessBoard.PlayerClickedNode += ChessBoard_PlayerClickedNode;

            ChessBoard.DrawChessBoard();

            // 4. BẮT ĐẦU LẮNG NGHE SERVER (Luồng riêng)
            listenThread = new Thread(ListenFromServer);
            listenThread.IsBackground = true;
            listenThread.Start();
        }

        // --- GỬI DỮ LIỆU ---
        private void ChessBoard_PlayerClickedNode(Point point)
        {
            if (tcpClient != null && tcpClient.IsConnected())
            {
                // Gửi Packet MOVE
                tcpClient.SendPacket(new Packet("MOVE", point));
            }
        }

        // --- NHẬN DỮ LIỆU ---
        private void ListenFromServer()
        {
            while (!_stopListening)
            {
                try
                {
                    if (tcpClient == null) break;

                    string receivedData = tcpClient.Receive();

                    if (string.IsNullOrEmpty(receivedData))
                        continue;

                    ProcessData(receivedData);
                }
                catch
                {
                    break;
                }
            }
        }


        // --- XỬ LÝ LOGIC ---
        private void ProcessData(string data)
        {
            // [FIX QUAN TRỌNG]: Dùng Split('|') thay vì (';') để khớp với TCPClient
            try
            {
                if (data.StartsWith("MOVE"))
                {
                    // Format nhận về: MOVE|Row|Col
                    string[] parts = data.Split('|');
                    int r = int.Parse(parts[1]);
                    int c = int.Parse(parts[2]);

                    Point enemyPoint = new Point(c, r); // Point(Col, Row)

                    // Vẽ lên bàn cờ (Dùng Invoke cho chắc ăn)
                    this.Invoke((MethodInvoker)delegate {
                        ChessBoard.OtherPlayerMoved(enemyPoint);
                    });
                }
                else if (data.StartsWith("CHAT"))
                {
                    // Format nhận về: CHAT|Nội_dung
                    string[] parts = data.Split('|');
                    if (parts.Length >= 2)
                    {
                        string msg = parts[1];
                        this.Invoke((MethodInvoker)delegate {
                            AppendMessage("Opponent", msg, Color.Red);
                        });
                    }
                }
            }
            catch { }
        }

        // Khi tắt Form -> Hủy luồng mạng
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Báo cho thread dừng vòng while
            _stopListening = true;

            // Ngắt kết nối TCP để Receive() thoát ra
            tcpClient?.Disconnect();

            // Đợi thread kết thúc (tùy, không bắt buộc)
            if (listenThread != null && listenThread.IsAlive)
            {
                try { listenThread.Join(200); } catch { }
            }

            base.OnFormClosing(e);
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
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string text = txtMessage.Text.Trim();
            if (string.IsNullOrWhiteSpace(text)) return;

            AppendMessage("You", text, Color.Blue);

            // Gửi Chat qua mạng
            if (tcpClient != null)
            {
                tcpClient.SendPacket(new Packet("CHAT", text));
            }

            txtMessage.Clear();
        }

        // ... (Giữ nguyên các hàm UI Emoji, Menu, Exit bên dưới của bạn) ...

        // Code giữ nguyên để tránh lỗi Designer
        private void Btn_Click(object? sender, EventArgs e) { Button btn = sender as Button; }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            // Quay về Dashboard
            var DashBoard = new Dashboard(room, playerNumber, player1Name, tcpClient);
            DashBoard.Show();
        }

        private void btnChat_Click(object sender, EventArgs e)
        {
            if (panelChat != null) panelChat.Visible = !panelChat.Visible;
        }

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

                // RESET GAME: Cần khởi tạo lại đúng logic PvP
                // [FIX]: Đảm bảo đăng ký lại sự kiện khi new lại bàn cờ
                ChessBoard = new ChessBoardManager(pnlChessBoard, GameMode.PvP);
                ChessBoard.MySide = (playerNumber == 1) ? 0 : 1;
                ChessBoard.PlayerClickedNode += ChessBoard_PlayerClickedNode;

                ChessBoard.DrawChessBoard();
                this.room = room;
                this.playerNumber = playerNumber;
            }
            else
            {
                menuForm.Close();
                menuForm = null;
            }
        }

        private void btnUndo_Click(object sender, EventArgs e)
        {
            bool undoSuccess = ChessBoard.undoTurnPvP();
            if (undoSuccess && !undoCount)
            {
                ptbOne.Visible = false;
                ptbZero.Visible = true;
                undoCount = true;
            }
        }

        // --- EMOJI LOGIC (Giữ nguyên) ---
        private readonly string[] _emoticons = new string[] { "😀", "😃", "😄", "😁", "😆", "😅", "😂", "🤣", "🥲", "😊", "😇", "🙂", "🙃", "😉", "😌", "😍", "🥰", "😘", "😗", "😙", "😚", "😋", "😜", "😝", "😛", "🤑", "🤗", "🤭", "🤫", "🤔", "🤐", "😐", "😑", "😶", "😶‍🌫️", "🙄", "😏", "😒", "😞", "😔", "😟", "😕", "🙁", "☹️", "😣", "😖", "😫", "😩", "🥺", "😢", "😭", "😤", "😠", "😡", "🤬", "🤯", "😳", "🥵", "🥶", "😱", "😨", "😰", "😥", "😓", "🤤", "😪", "😴", "😬", "😮‍💨", "🫠", "😵", "😵‍💫", "🤐", "🥴", "😷", "🤒", "🤕", "🤢", "🤮", "🤧", "😇", "🥳", "🥸", "😎", "🤓", "🧐", "😕", "😟", "🙁", "☹️", "😮", "😯", "😲", "😳", "🥺", "🥹", "😦", "😧", "😨", "😩", "😰", "😱", "😪", "😵", "🤐", "🥴", "😷", "🤒", "🤕", "🤢", "🤮", "🤧", "😇", "🥳", "🥸", "😎", "🤓", "🧐", "👋", "🤚", "🖐️", "✋", "🖖", "👌", "🤌", "🤏", "✌️", "🤞", "🤟", "🤘", "🤙", "👈", "👉", "👆", "🖕", "👇", "☝️", "👍", "👎", "✊", "👊", "🤛", "🤜", "👏", "🙌", "🫶", "👐", "🤲", "🙏", "💪", "🦾", "🦵", "🦿", "🦶", "👂", "🦻", "👃", "👣", "👀", "👁️", "🫦", "👄", "🦷", "🦴", "👅", "💋", "👄", "💘", "💝", "💖", "💗", "💓", "💞", "💕", "💌", "💟", "❣️", "💔", "❤️", "🧡", "💛", "💚", "💙", "💜", "🤎", "🖤", "🤍", "🏁", "🚩", "🎌", "🏴", "🏳️", "🏳️‍🌈", "🏳️‍⚧️", "🏴‍☠️" };

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

            if (pnlEmojiPicker.Visible && pnlEmojiPicker.Controls.Count > 0)
            {
                pnlEmojiPicker.Visible = false;
                return;
            }

            pnlEmojiPicker.Visible = true;
            pnlEmojiPicker.BringToFront();
            pnlEmojiPicker.Controls.Clear();

            int btnSize = 32;
            int cols = 8;
            int spacing = 4;

            for (int i = 0; i < _emoticons.Length; i++)
            {
                var btn = new Button();
                btn.Font = new Font("Segoe UI Emoji", 16F, FontStyle.Regular);
                btn.Text = _emoticons[i];
                btn.Width = btn.Height = btnSize;
                int col = i % cols;
                int row = i / cols;
                btn.Left = col * (btnSize + spacing);
                btn.Top = row * (btnSize + spacing);
                btn.Margin = new Padding(0);
                btn.Padding = new Padding(0);
                btn.Click += (s, e) => { txtMessage.Text += ((Button)s).Text; txtMessage.SelectionStart = txtMessage.Text.Length; txtMessage.Focus(); };
                pnlEmojiPicker.Controls.Add(btn);
            }
        }
        private void btn_emoji_Click(object sender, EventArgs e) { ShowEmojiPicker(); }
    }
}