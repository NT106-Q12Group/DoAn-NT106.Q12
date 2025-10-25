using System.ComponentModel.DataAnnotations;

namespace CaroGame
{
    public partial class Form1 : Form
    {
        #region Properties
        ChessBoardManager ChessBoard;
        #endregion

        public Form1()
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
            this.Close();
            var DashBoard = new Dashboard();
            DashBoard.Show();
        }
    }
}
