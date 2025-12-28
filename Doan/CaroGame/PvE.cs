using System;
using System.Drawing;
using System.Windows.Forms;
using CaroGame_TCPClient;

namespace CaroGame
{
    public partial class PvE : Form
    {
        private ChessBoardManager ChessBoard;
        private string botDifficulty;
        private string _playerName;
        private bool undoCount = false;

        private TCPClient tcpClient;

        private Panel _pnlEmojiPicker;

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

        public PvE(string difficulty, string playerName, TCPClient client = null)
        {
            InitializeComponent();

            _playerName = playerName;
            tcpClient = client;

            if (label1 != null) label1.Text = _playerName;

            botDifficulty = difficulty;

            ChessBoard = new ChessBoardManager(pnlChessBoard, GameMode.PvE);
            ChessBoard.GameEnded += OnGameEnded;
            ChessBoard.OnTurnChanged += TurnUI_PvE;

            ChessBoard.DrawChessBoard();
            SetBotDifficulty(botDifficulty);

            if (panelChat != null) panelChat.Visible = false;

            CreateEmojiPickerRuntime();
            this.Shown += (_, __) => EnsureEmojiPickerLayout();
            this.Resize += (_, __) => EnsureEmojiPickerLayout();
        }

        private void SetBotDifficulty(string difficulty)
        {
            switch (difficulty)
            {
                case "Easy": ChessBoard.Difficulty = DifficultyLevel.Easy; break;
                case "Medium": ChessBoard.Difficulty = DifficultyLevel.Medium; break;
                case "Hard": ChessBoard.Difficulty = DifficultyLevel.Hard; break;
                case "Extremely Hard": ChessBoard.Difficulty = DifficultyLevel.ExtremelyHard; break;
                default: ChessBoard.Difficulty = DifficultyLevel.Easy; break;
            }
        }

        private void ReportPvEResultToServer(bool isWin)
        {
            try
            {
                if (tcpClient != null && tcpClient.IsConnected())
                {
                    string result = isWin ? "WIN" : "LOSE";
                    tcpClient.Send($"PVE_RESULT|{_playerName}|{result}|{botDifficulty}");
                }
            }
            catch { }
        }

        private void OnGameEnded(string winner)
        {
            bool isWin =
                (ChessBoard.Player != null && ChessBoard.Player.Count > 0 &&
                 winner == ChessBoard.Player[0].Name);

            ReportPvEResultToServer(isWin);

            Form resultForm = isWin ? new WinMatch() : new LoseMatch();

            if (resultForm is WinMatch win)
            {
                win.winRematch += () => { resultForm.Close(); resetChess(); };
                win.winExit += () => { resultForm.Close(); this.Close(); };
            }
            else if (resultForm is LoseMatch lose)
            {
                lose.loseRematch += () => { resultForm.Close(); resetChess(); };
                lose.loseExit += () => { resultForm.Close(); this.Close(); };
            }

            resultForm.Show();
        }

        private void resetChess()
        {
            ChessBoard.resetGame();

            undoCount = false;
            if (ptbOne != null) ptbOne.Visible = true;
            if (ptbZero != null) ptbZero.Visible = false;

            if (panelChat != null) panelChat.Visible = false;
            HideEmojiPicker();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát trận đấu với Bot không?",
                "Xác nhận thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
                this.Close();
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

        private void btnUndo_Click(object sender, EventArgs e)
        {
            if (undoCount)
            {
                MessageBox.Show("Bạn đã dùng Undo rồi.", "Undo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Điều kiện giống PvP: cả 2 bên phải đi ít nhất 1 nước => tổng >= 2
            if (ChessBoard.MoveCount < 2)
            {
                MessageBox.Show("Chưa thể Undo: cả 2 bên phải đi ít nhất 1 nước cờ trước.", "Undo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Bạn có chắc chắn muốn Undo 1 nước cờ không?",
                "Xác nhận Undo",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes) return;

            bool undoSuccess = ChessBoard.undoTurnPvE();
            if (undoSuccess)
            {
                if (ptbOne != null) ptbOne.Visible = false;
                if (ptbZero != null) ptbZero.Visible = true;
                undoCount = true;
            }
            else
            {
                MessageBox.Show("Undo thất bại.", "Undo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn tạo ván đấu mới không?",
                "Tạo ván đấu mới",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
                resetChess();
        }

        private void TurnUI_PvE(bool isPlayerTurn)
        {
            if (pgbP1 != null)
            {
                pgbP1.Visible = isPlayerTurn;
                pgbP1.Style = ProgressBarStyle.Blocks;
                pgbP1.Value = isPlayerTurn ? 100 : 0;
            }

            if (pgbP2 != null)
            {
                pgbP2.Visible = !isPlayerTurn;
                pgbP2.Style = ProgressBarStyle.Blocks;
                pgbP2.Value = isPlayerTurn ? 0 : 100;
            }
        }

        private void CreateEmojiPickerRuntime()
        {
            if (_pnlEmojiPicker != null) return;

            _pnlEmojiPicker = new Panel();
            _pnlEmojiPicker.Name = "pnlEmojiPicker_Runtime";
            _pnlEmojiPicker.Visible = false;
            _pnlEmojiPicker.AutoScroll = true;
            _pnlEmojiPicker.BorderStyle = BorderStyle.FixedSingle;

            if (panelChat != null)
            {
                panelChat.Controls.Add(_pnlEmojiPicker);
                _pnlEmojiPicker.BringToFront();
            }
            else
            {
                this.Controls.Add(_pnlEmojiPicker);
                _pnlEmojiPicker.BringToFront();
            }

            BuildEmojiButtons();
            EnsureEmojiPickerLayout();
        }

        private void BuildEmojiButtons()
        {
            if (_pnlEmojiPicker == null) return;

            _pnlEmojiPicker.Controls.Clear();

            int btnSize = 32, cols = 8, spacing = 4;

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
                    if (txtMessage == null) return;
                    txtMessage.Text += ((Button)s).Text;
                    txtMessage.SelectionStart = txtMessage.Text.Length;
                    txtMessage.Focus();
                };

                _pnlEmojiPicker.Controls.Add(btn);
            }
        }

        private void EnsureEmojiPickerLayout()
        {
            if (_pnlEmojiPicker == null) return;

            if (panelChat != null && !panelChat.Visible)
            {
                _pnlEmojiPicker.Visible = false;
                return;
            }

            Control host = panelChat ?? (Control)this;

            int w = Math.Min(300, host.ClientSize.Width - 10);
            int h = 200;

            _pnlEmojiPicker.Width = Math.Max(200, w);
            _pnlEmojiPicker.Height = h;

            if (txtMessage != null && txtMessage.Parent == host)
            {
                int pad = 6;
                int x = txtMessage.Left;
                int y = txtMessage.Top - _pnlEmojiPicker.Height - pad;

                if (y < 0) y = txtMessage.Bottom + pad;

                if (x + _pnlEmojiPicker.Width > host.ClientSize.Width)
                    x = host.ClientSize.Width - _pnlEmojiPicker.Width - pad;
                if (x < pad) x = pad;

                _pnlEmojiPicker.Left = x;
                _pnlEmojiPicker.Top = y;
            }
            else
            {
                _pnlEmojiPicker.Left = 6;
                _pnlEmojiPicker.Top = Math.Max(6, host.ClientSize.Height - _pnlEmojiPicker.Height - 6);
            }

            _pnlEmojiPicker.BringToFront();
        }

        private void ShowEmojiPicker()
        {
            CreateEmojiPickerRuntime();

            if (panelChat != null && !panelChat.Visible)
            {
                MessageBox.Show("Chat đang tắt. Bấm nút Chat để mở.", "Emoji",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _pnlEmojiPicker.Visible = !_pnlEmojiPicker.Visible;
            if (_pnlEmojiPicker.Visible) EnsureEmojiPickerLayout();
        }

        private void HideEmojiPicker()
        {
            if (_pnlEmojiPicker != null) _pnlEmojiPicker.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e) { }

        private void pnlChessBoard_Paint(object sender, PaintEventArgs e) { }
        private void btnChat_Click(object sender, EventArgs e)
        {
            if (panelChat == null) return;

            if (panelChat.Visible)
            {
                panelChat.Visible = false;
                HideEmojiPicker();
                return;
            }

            DialogResult res = MessageBox.Show(
                "Vì đây là PvE nên tính năng chat không khả dụng.\n" +
                "Nhưng nếu bạn là một người tự kỷ thì bạn có thể mở chat lên và chat một mình.\n\n" +
                "Bạn có muốn mở chat không?",
                "PvE Chat",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Information
            );

            panelChat.Visible = (res == DialogResult.Yes);

            if (!panelChat.Visible) HideEmojiPicker();
            EnsureEmojiPickerLayout();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (panelChat != null && !panelChat.Visible)
            {
                MessageBox.Show("Chat đang tắt. Bấm nút Chat để mở.", "Chat",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string text = txtMessage?.Text.Trim();
            if (string.IsNullOrWhiteSpace(text)) return;

            AppendMessage("You", text, Color.Blue);
            txtMessage.Clear();
            txtMessage.Focus();
        }

        private void btn_emoji_Click(object sender, EventArgs e) => ShowEmojiPicker();
    }
}
