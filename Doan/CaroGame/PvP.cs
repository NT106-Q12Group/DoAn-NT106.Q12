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
        private string player1Name;
        private string player2Name;
        private TCPClient tcpClient;

        // --- CONSTRUCTOR ---
        public PvP(Room room, int mySide, string p1, string p2, TCPClient client)
        {
            InitializeComponent();
            this.room = room;
            this.MySide = mySide;
            this.player1Name = p1;
            this.player2Name = p2;
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

            SetupPlayerInfo();

            // Khởi tạo bàn cờ PvP
            ChessBoard = new ChessBoardManager(pnlChessBoard, GameMode.PvP);
            ChessBoard.MySide = this.MySide;
            ChessBoard.IsMyTurn = (this.MySide == 0);

            this.Text = $"PvP Room - Bạn là: {(this.MySide == 0 ? "X (Đi trước)" : "O (Đi sau)")}";

            ChessBoard.PlayerClickedNode -= ChessBoard_PlayerClickedNode;
            ChessBoard.PlayerClickedNode += ChessBoard_PlayerClickedNode;

            ChessBoard.GameEnded -= ChessBoard_GameEnded;
            ChessBoard.GameEnded += ChessBoard_GameEnded;

            if (tcpClient != null)
            {
                tcpClient.OnMessageReceived -= HandleServerMessage;
                tcpClient.OnMessageReceived += HandleServerMessage;
            }

            ChessBoard.DrawChessBoard();
        }

        private void SetupPlayerInfo()
        {
            if (label1 != null) label1.Text = player1Name;
            if (label2 != null) label2.Text = player2Name;

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

            // Avatar placeholder
            try
            {
                if (ptbAvaP1 != null) { ptbAvaP1.Image = null; ptbAvaP1.BackColor = Color.LightGray; }
                if (ptbAvaP2 != null) { ptbAvaP2.Image = null; ptbAvaP2.BackColor = Color.LightGray; }
            }
            catch { }
        }

        private void ChessBoard_GameEnded(string winnerName)
        {
            MessageBox.Show($"Trận đấu kết thúc!\nNgười chiến thắng: {winnerName}", "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

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
                        this.Close(); // Đóng form -> Dashboard tự hiện lại
                    }
                }
                catch (Exception ex) { }
            });
        }

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

        // --- [FIXED] NÚT THOÁT GAME ---
        private void btnExit_Click(object sender, EventArgs e)
        {
            // 1. Hiện bảng hỏi xác nhận
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát trận đấu?\nBạn sẽ bị xử thua ngay lập tức.",
                "Xác nhận thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            // 2. Nếu chọn YES thì mới thực hiện
            if (result == DialogResult.Yes)
            {
                if (tcpClient != null)
                {
                    // Hủy nhận tin nhắn để tránh lỗi
                    tcpClient.OnMessageReceived -= HandleServerMessage;

                    // Gửi lệnh đầu hàng
                    tcpClient.Send("SURRENDER");
                }

                // 3. CHỈ ĐÓNG FORM HIỆN TẠI
                // Dashboard cũ sẽ tự hiện lên nhờ sự kiện FormClosed đã đăng ký bên Dashboard.cs
                this.Close();

                // [ĐÃ XÓA] var DashBoard = new Dashboard(...) -> Không tạo Dashboard mới nữa
            }
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