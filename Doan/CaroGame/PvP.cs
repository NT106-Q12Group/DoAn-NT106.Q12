using System;
using System.Drawing;
using System.Windows.Forms;
using CaroGame_TCPClient;

namespace CaroGame
{
    public partial class PvP : Form
    {
        #region Properties
        private ChessBoardManager ChessBoard;
        #endregion

        public Room room;

        // 0: X (P1), 1: O (P2)
        public int MySide { get; set; }

        private bool isMyUndoRequest = false;
        private bool undoCount = false;

        private string player1Name;
        private string player2Name;
        private TCPClient tcpClient;

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
            if (pnlChessBoard != null) pnlChessBoard.BringToFront();

            SetupPlayerInfo();

            ChessBoard = new ChessBoardManager(pnlChessBoard, GameMode.PvP);
            ChessBoard.MySide = this.MySide;

            // Quy tắc: Phe 0 (X) luôn đi trước
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

            if (MySide == 0)
            {
                if (label1 != null) { label1.ForeColor = Color.Red; label1.Font = new Font(label1.Font, FontStyle.Bold); }
                if (label2 != null) { label2.ForeColor = Color.Black; label2.Font = new Font(label2.Font, FontStyle.Regular); }
            }
            else
            {
                if (label1 != null) { label1.ForeColor = Color.Black; label1.Font = new Font(label1.Font, FontStyle.Regular); }
                if (label2 != null) { label2.ForeColor = Color.Blue; label2.Font = new Font(label2.Font, FontStyle.Bold); }
            }

            try
            {
                if (ptbAvaP1 != null) { ptbAvaP1.Image = null; ptbAvaP1.BackColor = Color.LightGray; }
                if (ptbAvaP2 != null) { ptbAvaP2.Image = null; ptbAvaP2.BackColor = Color.LightGray; }
            }
            catch { }
        }

        private void ChessBoard_GameEnded(string winnerName)
        {
            MessageBox.Show(
                $"Trận đấu kết thúc!\nNgười chiến thắng: {winnerName}",
                "Kết quả",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void ChessBoard_PlayerClickedNode(Point point)
        {
            if (tcpClient != null && tcpClient.IsConnected())
                tcpClient.SendPacket(new Packet("MOVE", point));
        }

        private void HandleServerMessage(string data)
        {
            if (IsDisposed || !IsHandleCreated) return;

            BeginInvoke((MethodInvoker)delegate
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
                        MessageBox.Show("Đối thủ đã thoát trận! Bạn thắng.", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Close();
                    }
                }
                catch { }
            });
        }

        // ================== ORIGINAL HANDLERS (không dấu _) ==================

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
            string text = txtMessage?.Text.Trim();
            if (string.IsNullOrWhiteSpace(text)) return;

            AppendMessage("You", text, Color.Blue);

            if (tcpClient != null)
                tcpClient.SendPacket(new Packet("CHAT", text));

            txtMessage.Clear();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát trận đấu?\nBạn sẽ bị xử thua ngay lập tức.",
                "Xác nhận thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                if (tcpClient != null)
                {
                    tcpClient.OnMessageReceived -= HandleServerMessage;
                    tcpClient.Send("SURRENDER");
                }
                Close();
            }
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            if (menuForm == null || menuForm.IsDisposed)
            {
                menuForm = new Menu();
                menuForm.StartPosition = FormStartPosition.Manual;
                menuForm.Location = new Point(this.Left + 22, this.Top + 50);
                menuForm.Show(this);
            }
            else
            {
                menuForm.Close();
                menuForm = null;
            }
        }

        private void btnChat_Click(object sender, EventArgs e)
        {
            if (panelChat != null) panelChat.Visible = !panelChat.Visible;
        }

        // ================== ALIAS HANDLERS (có dấu _) để KHỚP DESIGNER ==================
        // Nếu Designer đang gọi btn_Undo_Click / btn_Send_Click / btn_Chat_Click... thì sẽ không lỗi nữa.

        private void btn_Undo_Click(object sender, EventArgs e) => btnUndo_Click(sender, e);
        private void btn_Send_Click(object sender, EventArgs e) => btnSend_Click(sender, e);
        private void btn_Exit_Click(object sender, EventArgs e) => btnExit_Click(sender, e);
        private void btn_Menu_Click(object sender, EventArgs e) => btnMenu_Click(sender, e);
        private void btn_Chat_Click(object sender, EventArgs e) => btnChat_Click(sender, e);

        // ================== CHAT UI ==================

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

        // ================== EMOJI ==================

        private readonly string[] _emoticons = new string[] {
            "😀","😃","😄","😁","😆","😅","😂","🤣","🥲","☺️","😊","😇",
            "🙂","🙃","😉","😌","😍","🥰","😘","😗","😋","😛","😝","😜",
            "🤪","🤨","🧐","🤓","😎","🥸","🤩","🥳","😏","😒","😞","😔",
            "😟","😕","🙁","☹️","😣","😖","😫","😩","🥺","😢","😭","😤",
            "😠","😡","🤬","🤯","😳","🥵","🥶","😱","😨","😰","😥","😓",
            "🤗","🤔","🤭","🤫","🤥","😶","😐","😑","😬","🙄","😯","😦",
            "😧","😮","😲","🥱","😴","🤤","😪","😵","🤐","🥴","🤢","🤮",
            "🤧","😷","🤒","🤕","🤑","🤠","😈","👿","👹","👺","🤡","💩",
            "👻","💀","☠️","👽","👾","🤖","🎃",
            "😺","😸","😹","😻","😼","😽","🙀","😿","😾",
            "👋","🤚","🖐","✋","🖖","👌","🤌","🤏","✌️","🤞","🤟","🤘",
            "🤙","👈","👉","👆","👇","☝️","👍","👎","✊","👊","🤛","🤜",
            "👏","🙌","👐","🤲","🤝","🙏","💪","💅","🤳",
            "❤️","🧡","💛","💚","💙","💜","🖤","🤍","🤎","💔","❣️","💕",
            "💞","💓","💗","💖","💘","💝","💋","💌",
            "👀","👁","🧠","🔥","✨","🌟","💫","💥","💢","💦","💤","🎵",
            "🎶","✅","❌","💯","⚠️","⛔️","🎉","🎈","🎁"
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
            if (pnlEmojiPicker == null || txtMessage == null) return;

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

                btn.Click += (s, e) =>
                {
                    txtMessage.Text += ((Button)s).Text;
                    txtMessage.SelectionStart = txtMessage.Text.Length;
                    txtMessage.Focus();
                };

                pnlEmojiPicker.Controls.Add(btn);
            }
        }

        // emoji của bạn vốn đã có dấu "_" đúng rồi, giữ nguyên
        private void btn_emoji_Click(object sender, EventArgs e) => ShowEmojiPicker();

        // ================== FORM CLOSING ==================

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (tcpClient != null)
                tcpClient.OnMessageReceived -= HandleServerMessage;

            base.OnFormClosing(e);
        }

        private void Btn_Click(object? sender, EventArgs e) { }
    }
}
