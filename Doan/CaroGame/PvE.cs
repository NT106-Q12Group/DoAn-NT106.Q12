using System;
using System.Drawing;
using System.Windows.Forms;

namespace CaroGame
{
    public partial class PvE : Form
    {
        #region Properties
        ChessBoardManager ChessBoard;
        private string botDifficulty;
        private string _playerName;
        private Menu menuForm;
        private bool undoCount = false;
        #endregion

        // Constructor nhận tham số difficulty
        public PvE(string difficulty, string playerName)
        {
            InitializeComponent();
            _playerName = playerName;

            if (label1 != null) label1.Text = _playerName;

            botDifficulty = difficulty;

            ChessBoard = new ChessBoardManager(pnlChessBoard, GameMode.PvE);
            ChessBoard.GameEnded += OnGameEnded;
            ChessBoard.DrawChessBoard();

            SetBotDifficulty(botDifficulty);
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

        private void OnGameEnded(string winner)
        {
            bool isWin = (ChessBoard.Player != null && ChessBoard.Player.Count > 0 && winner == ChessBoard.Player[0].Name);
            Form result = isWin ? new WinMatch() : new LoseMatch();

            if (result is WinMatch win)
            {
                win.winRematch += () => { result.Close(); resetChess(); };
                win.winExit += () => { result.Close(); this.Close(); };
            }
            else if (result is LoseMatch lose)
            {
                lose.loseRematch += () => { result.Close(); resetChess(); };
                lose.loseExit += () => { result.Close(); this.Close(); };
            }
            result.Show();
        }

        private void resetChess()
        {
            ChessBoard.resetGame();
            undoCount = false;
            ptbOne.Visible = true;
            ptbZero.Visible = false;
        }

        // --- CÁC HÀM UI ---

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

        // --- [FIXED] NÚT THOÁT PVE ---
        private void btnExit_Click(object sender, EventArgs e)
        {
            // 1. Hiện bảng xác nhận thoát
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn thoát trận đấu với Bot không?",
                "Xác nhận thoát",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // 2. Nếu chọn Yes -> Đóng form -> Dashboard cũ tự hiện lại
            if (result == DialogResult.Yes)
            {
                this.Close();
                // Không cần new Dashboard() nữa
            }
        }

        private void btnChat_Click(object sender, EventArgs e)
        {
            if (panelChat != null) panelChat.Visible = !panelChat.Visible;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string text = txtMessage.Text.Trim();
            if (string.IsNullOrWhiteSpace(text)) return;

            AppendMessage("You", text, Color.Blue);
            txtMessage.Clear();

            // Giả lập Bot trả lời
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
            t.Interval = 1000;
            t.Tick += (s, ev) =>
            {
                t.Stop();
                string[] botReplies = { "Suy nghĩ kỹ đi!", "Nước cờ hay đấy!", "Đừng hòng thắng tôi.", "..." };
                Random r = new Random();
                AppendMessage("Bot", botReplies[r.Next(botReplies.Length)], Color.Green);
            };
            t.Start();
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
            bool undoSuccess = ChessBoard.undoTurnPvE();
            if (undoSuccess && !undoCount)
            {
                ptbOne.Visible = false;
                ptbZero.Visible = true;
                undoCount = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e) { }
        private void pnlChessBoard_Paint(object sender, PaintEventArgs e) { }
    }
}