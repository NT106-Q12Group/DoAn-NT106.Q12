using System;
using System.Windows.Forms;

namespace CaroGame
{
    public partial class BotDifficulty : Form
    {
        private string botDifficulty;
        private string _playerName;

        public BotDifficulty(string playerName)
        {
            InitializeComponent();
            _playerName = playerName;
        }

        private void btn_easy_Click(object sender, EventArgs e)
        {
            botDifficulty = "Easy";
            StartBotGame();
        }

        private void btn_medium_Click(object sender, EventArgs e)
        {
            botDifficulty = "Medium";
            StartBotGame();
        }

        private void btn_hard_Click(object sender, EventArgs e)
        {
            botDifficulty = "Hard";
            StartBotGame();
        }

        private void btn_extremely_hard_Click(object sender, EventArgs e)
        {
            botDifficulty = "Extremely Hard";
            StartBotGame();
        }

        private void StartBotGame()
        {
            var gameForm = new PvE(botDifficulty, _playerName);

            this.Hide();

            gameForm.FormClosed += (s, args) => this.Close();
            gameForm.Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}