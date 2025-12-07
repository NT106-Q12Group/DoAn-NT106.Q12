    using System.ComponentModel.DataAnnotations;

    namespace CaroGame
    {
        public partial class PvP : Form
        {
            #region Properties
            ChessBoardManager ChessBoard;
            #endregion
            public Room room;
            public int playerNumber;
            public PvP(Room room, int playerNumber)
            {
                InitializeComponent();

                ChessBoard = new ChessBoardManager(pnlChessBoard);

                ChessBoard.DrawChessBoard();

                this.room = room;
                this.playerNumber = playerNumber;
            }

            private void Btn_Click(object? sender, EventArgs e)
            {
                Button btn = sender as Button;
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
