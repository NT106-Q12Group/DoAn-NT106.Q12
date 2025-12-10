using System;
using System.Drawing;
using System.IO;
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

        // 0: X (P1), 1: O (P2)
        public int MySide { get; set; }

        private bool isMyUndoRequest = false;
        private bool undoCount = false;

        // Các biến kết nối mạng
        private string player1Name; // Player 1 (X)
        private string player2Name; // Player 2 (O)
        private TCPClient tcpClient;

        // --- CONSTRUCTOR ---
        public PvP(Room room, int mySide, string p1, string p2, TCPClient client)
        {
            InitializeComponent();
            this.room = room;
            this.MySide = mySide;
            this.player1Name = p1; // Người cầm X
            this.player2Name = p2; // Người cầm O
            this.tcpClient = client;

            InitGame();
        }

        // Constructor Offline/Fallback
        public PvP(Room room, int playerNumber)
        {
            InitializeComponent();
            this.room = room;
            this.MySide = (playerNumber == 1) ? 0 : 1;
            this.player1Name = "Player 1";
            this.player2Name = "Player 2";
            InitGame();
        }

        private void InitGame()
        {
            CheckForIllegalCrossThreadCalls = false;
            SetupEmojiPickerPanel();
            pnlChessBoard.BringToFront();

            // 1. Cài đặt thông tin Player (Avatar, Tên)
            SetupPlayerInfo();

            // 2. Khởi tạo bàn cờ PvP
            ChessBoard = new ChessBoardManager(pnlChessBoard, GameMode.PvP);
            ChessBoard.MySide = this.MySide;

            // 3. Quy tắc: Phe 0 (X) luôn đi trước
            ChessBoard.IsMyTurn = (this.MySide == 0);

            // Cập nhật Title Bar
            this.Text = $"PvP Room - Bạn là: {(this.MySide == 0 ? "X (Đi trước)" : "O (Đi sau)")}";

            // 4. Đăng ký sự kiện
            ChessBoard.PlayerClickedNode -= ChessBoard_PlayerClickedNode;
            ChessBoard.PlayerClickedNode += ChessBoard_PlayerClickedNode;

            ChessBoard.GameEnded -= ChessBoard_GameEnded;
            ChessBoard.GameEnded += ChessBoard_GameEnded;

            // 5. Đăng ký mạng
            if (tcpClient != null)
            {
                tcpClient.OnMessageReceived -= HandleServerMessage;
                tcpClient.OnMessageReceived += HandleServerMessage;
            }

            ChessBoard.DrawChessBoard();
        }

        // --- [MỚI] HÀM CÀI ĐẶT UI PLAYER ---
        private void SetupPlayerInfo()
        {
            // 1. Cài đặt tên (Label)
            if (label1 != null) label1.Text = player1Name; // P1 (X)
            if (label2 != null) label2.Text = player2Name; // P2 (O)

            // 2. Highlight tên của MÌNH (In đậm + Đổi màu)
            if (MySide == 0) // Mình là P1
            {
                if (label1 != null) { label1.ForeColor = Color.Red; label1.Font = new Font(label1.Font, FontStyle.Bold); }
                if (label2 != null) { label2.ForeColor = Color.Black; label2.Font = new Font(label2.Font, FontStyle.Regular); }
            }
            else // Mình là P2
            {
                if (label1 != null) { label1.ForeColor = Color.Black; label1.Font = new Font(label1.Font, FontStyle.Regular); }
                if (label2 != null) { label2.ForeColor = Color.Blue; label2.Font = new Font(label2.Font, FontStyle.Bold); }
            }

            // 3. Cài đặt Avatar (Load ảnh X và O vào PictureBox)
            // ptbAvaP1 -> X, ptbAvaP2 -> O
            string path = Application.StartupPath + "\\Resources\\";

            try
            {
                if (ptbAvaP1 != null)
                {
                    ptbAvaP1.SizeMode = PictureBoxSizeMode.StretchImage;
                    if (File.Exists(path + "Caro Game.png"))
                        ptbAvaP1.Image = Image.FromFile(path + "Caro Game.png"); // Ảnh X
                }

                if (ptbAvaP2 != null)
                {
                    ptbAvaP2.SizeMode = PictureBoxSizeMode.StretchImage;
                    if (File.Exists(path + "Caro Game (1).png"))
                        ptbAvaP2.Image = Image.FromFile(path + "Caro Game (1).png"); // Ảnh O
                }
            }
            catch { /* Bỏ qua lỗi nếu không tìm thấy ảnh */ }
        }

        private void ChessBoard_GameEnded(string winnerName)
        {
            MessageBox.Show($"Trận đấu kết thúc!\nNgười chiến thắng: {winnerName}", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // --- GỬI DỮ LIỆU ---
        private void ChessBoard_PlayerClickedNode(Point point)
        {
            if (tcpClient != null && tcpClient.IsConnected())
            {
                tcpClient.SendPacket(new Packet("MOVE", point));
            }
        }

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
                        if (parts.Length < 4) return;
                        int x = int.Parse(parts[1]);
                        int y = int.Parse(parts[2]);
                        int side = int.Parse(parts[3]);

                        if (side == -1) side = ChessBoard.MoveCount % 2;
                        ChessBoard.ProcessMove(x, y, side);
                    }
                    else if (command == "CHAT")
                    {
                        if (parts.Length >= 2)
                            AppendMessage(parts.Length > 2 ? parts[2] : "Opponent", parts[1], Color.Red);
                    }
                    else if (command == "UNDO_SUCCESS")
                    {
                        ChessBoard.ExecuteUndoPvP();
                        if (isMyUndoRequest)
                        {
                            undoCount = true;
                            if (ptbOne != null) ptbOne.Visible = false;
                            if (ptbZero != null) ptbZero.Visible = true;
                            isMyUndoRequest = false;
                        }
                    }
                    else if (command == "OPPONENT_LEFT")
                    {
                        MessageBox.Show("Đối thủ đã thoát trận! Bạn thắng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                    }
                }
                catch (Exception ex) { }
            });
        }

        // --- UI HANDLERS ---
        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (undoCount) return;
            if (tcpClient != null)
            {
                isMyUndoRequest = true;
                tcpClient.Send("REQUEST_UNDO");
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string text = txtMessage.Text.Trim();
            if (string.IsNullOrWhiteSpace(text)) return;
            AppendMessage("You", text, Color.Blue);
            if (tcpClient != null) tcpClient.SendPacket(new Packet("CHAT", text));
            txtMessage.Clear();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (tcpClient != null)
            {
                tcpClient.OnMessageReceived -= HandleServerMessage;
                tcpClient.CancelMatch(player1Name);
            }
            this.Close();
            // Mở lại Dashboard
            var DashBoard = new Dashboard(player1Name, tcpClient);
            DashBoard.Show();
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
            }
            else { menuForm.Close(); menuForm = null; }
        }

        // Emoji logic
        private readonly string[] _emoticons = new string[] {
            "😀", "😃", "😄", "😁", "😆", "😅", "😂", "🤣", "🥲", "☺️", "😊", "😇",
            "🙂", "🙃", "😉", "😌", "😍", "🥰", "😘", "😗", "😋", "😛", "😝", "😜",
            "🤪", "🤨", "🧐", "🤓", "😎", "🥸", "🤩", "🥳", "😏", "😒", "😞", "😔",
            "😟", "😕", "🙁", "☹️", "😣", "😖", "😫", "😩", "🥺", "😢", "😭", "😤",
            "😠", "😡", "🤬", "🤯", "😳", "🥵", "🥶", "😱", "😨", "😰", "😥", "😓",
            "🤗", "🤔", "🤭", "🤫", "🤥", "😶", "😐", "😑", "😬", "🙄", "😯", "😦",
            "😧", "😮", "😲", "🥱", "😴", "🤤", "😪", "😵", "🤐", "🥴", "🤢", "🤮",
            "🤧", "😷", "🤒", "🤕", "🤑", "🤠", "😈", "👿", "👹", "👺", "🤡", "💩",
            "👻", "💀", "☠️", "👽", "👾", "🤖", "🎃",
            "😺", "😸", "😹", "😻", "😼", "😽", "🙀", "😿", "😾",
            "👋", "🤚", "🖐", "✋", "🖖", "👌", "🤌", "🤏", "✌️", "🤞", "🤟", "🤘",
            "🤙", "👈", "👉", "👆", "👇", "☝️", "👍", "👎", "✊", "👊", "🤛", "🤜",
            "👏", "🙌", "👐", "🤲", "🤝", "🙏", "💪", "💅", "🤳",
            "❤️", "🧡", "💛", "💚", "💙", "💜", "🖤", "🤍", "🤎", "💔", "❣️", "💕",
            "💞", "💓", "💗", "💖", "💘", "💝", "💋", "💌",
            "👀", "👁", "🧠", "🔥", "✨", "🌟", "💫", "💥", "💢", "💦", "💤", "🎵",
            "🎶", "✅", "❌", "💯", "⚠️", "⛔️", "🎉", "🎈", "🎁"
        };

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
        private void Btn_Click(object? sender, EventArgs e) { }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (tcpClient != null) tcpClient.OnMessageReceived -= HandleServerMessage;
            base.OnFormClosing(e);
        }
    }
}