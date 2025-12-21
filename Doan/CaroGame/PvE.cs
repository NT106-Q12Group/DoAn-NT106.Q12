using System;
using System.Drawing;
using System.Windows.Forms;
using CaroGame_TCPClient;

namespace CaroGame
{
    public partial class PvE : Form
    {
        #region Properties
        ChessBoardManager ChessBoard;
        private string botDifficulty;
        private string _playerName;
        private bool undoCount = false;

        private TCPClient tcpClient; // NEW
        #endregion

        // NEW: nhận thêm TCPClient để report lên server
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
            bool isWin = (ChessBoard.Player != null && ChessBoard.Player.Count > 0 && winner == ChessBoard.Player[0].Name);

            // NEW: báo kết quả lên server
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

        private void btnChat_Click(object sender, EventArgs e)
        {
            if (panelChat != null) 
                panelChat.Visible = !panelChat.Visible;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string text = txtMessage.Text.Trim();
            if (string.IsNullOrWhiteSpace(text)) return;

            AppendMessage("You", text, Color.Blue);
            txtMessage.Clear();

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
                if (ptbOne != null) ptbOne.Visible = false;
                if (ptbZero != null) ptbZero.Visible = true;
                undoCount = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e) { }
        private void pnlChessBoard_Paint(object sender, PaintEventArgs e) { }

        private void btnNew_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc chắn muốn tạo ván đấu mới không?",
                "Tạo ván đấu mới",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
                resetChess(); ;
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
    }
}
