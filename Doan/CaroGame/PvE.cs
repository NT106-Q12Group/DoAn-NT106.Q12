using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Windows.Forms;
using static CaroGame.ChessBoardManager;

namespace CaroGame
{
    public partial class PvE : Form
    {
        #region Properties
        ChessBoardManager ChessBoard;
        private string botDifficulty;
        private Menu menuForm;
        #endregion

        // Constructor nhận tham số difficulty
        public PvE(string difficulty)
        {
            InitializeComponent();
            botDifficulty = difficulty;

            ChessBoard = new ChessBoardManager(pnlChessBoard);
            ChessBoard.DrawChessBoard();

            // GỌI HÀM SET DIFFICULTY NGAY TẠI ĐÂY
            SetBotDifficulty(botDifficulty);
        }

        private void SetBotDifficulty(string difficulty)
        {
            // Chuyển từ string (bên Form chọn) sang Enum (bên logic Bot)
            // Và gán vào biến Difficulty của ChessBoard
            switch (difficulty)
            {
                case "Easy":
                    ChessBoard.Difficulty = DifficultyLevel.Easy;
                    // MessageBox.Show("Đã set Easy"); // Debug nếu cần
                    break;
                case "Medium":
                    ChessBoard.Difficulty = DifficultyLevel.Medium;
                    break;
                case "Hard":
                    ChessBoard.Difficulty = DifficultyLevel.Hard;
                    break;
                case "Extremely Hard":
                    ChessBoard.Difficulty = DifficultyLevel.ExtremelyHard;
                    break;
                default:
                    ChessBoard.Difficulty = DifficultyLevel.Easy;
                    break;
            }
        }

        #region Các hàm để trống (Fix lỗi "Does not exist")
        // Giữ lại 2 hàm này để Designer không báo lỗi, nhưng để trống vì logic đã chuyển lên Constructor
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void pnlChessBoard_Paint(object sender, PaintEventArgs e)
        {
        }
        #endregion

        #region Các chức năng giống PvP (Menu, Exit, Chat)

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

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            var DashBoard = new Dashboard();
            DashBoard.Show();
        }

        private void btnChat_Click(object sender, EventArgs e)
        {
            // Kiểm tra null để tránh lỗi nếu chưa đặt tên panel
            if (panelChat != null)
                panelChat.Visible = !panelChat.Visible;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string text = txtMessage.Text.Trim();
            if (string.IsNullOrWhiteSpace(text)) return;

            AppendMessage("You", text, Color.Blue);
            txtMessage.Clear();

            // Giả lập Bot trả lời sau 1 giây
            System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
            t.Interval = 1000;
            t.Tick += (s, ev) =>
            {
                t.Stop();
                AppendMessage("Bot", "Đã nhận: " + text, Color.Green);
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

        #endregion
    }
}