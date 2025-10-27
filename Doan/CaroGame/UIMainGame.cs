using System.ComponentModel.DataAnnotations;

namespace CaroGame
{
    public partial class UIMainGame : Form
    {
        #region Properties
        ChessBoardManager ChessBoard;
        #endregion

        public UIMainGame()
        {
            InitializeComponent();

            ChessBoard = new ChessBoardManager(pnlChessBoard);

            ChessBoard.DrawChessBoard();
        }

        private void Btn_Click(object? sender, EventArgs e)
        {
            Button btn = sender as Button;
        }



        private void btnMenu_Click(object sender, EventArgs e)
        {
            var menuForm = new Menu();
            menuForm.Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            ActiveForm.Close();
            var DashBoard = new Dashboard();
            DashBoard.Show();
        }


        private void btnChat_Click(object sender, EventArgs e)
        {
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
                AppendMessage("Bot", "Đã nhận: " + text, Color.Green);
            };
            t.Start();
        }
        private void AppendMessage(string sender, string message, Color color)
        {
            rtbChat.SelectionStart = rtbChat.TextLength;
            rtbChat.SelectionColor = color;
            rtbChat.SelectionFont = new Font("Segoe UI", 10, FontStyle.Bold);
            rtbChat.AppendText($"{sender}: ");

            rtbChat.SelectionFont = new Font("Segoe UI", 10, FontStyle.Regular);
            rtbChat.SelectionColor = Color.Black;
            rtbChat.AppendText(message + Environment.NewLine + Environment.NewLine);

            rtbChat.ScrollToCaret();
        }

    }
}
