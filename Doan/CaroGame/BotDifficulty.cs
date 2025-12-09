using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaroGame
{
    public partial class BotDifficulty : Form
    {
        private string botDifficulty; // Biến lưu trữ chế độ khó của bot
        private string _playerName;

        public BotDifficulty(string playerName)
        {
            InitializeComponent();
            _playerName = playerName;
        }

        // Khi nhấn nút Easy
        private void btn_easy_Click(object sender, EventArgs e)
        {
            botDifficulty = "Easy"; // Gán chế độ Easy
            StartBotGame();  // Gọi hàm bắt đầu trò chơi với chế độ Easy
        }

        // Khi nhấn nút Medium
        private void btn_medium_Click(object sender, EventArgs e)
        {
            botDifficulty = "Medium"; // Gán chế độ Medium
            StartBotGame();  // Gọi hàm bắt đầu trò chơi với chế độ Medium
        }

        // Khi nhấn nút Hard
        private void btn_hard_Click(object sender, EventArgs e)
        {
            botDifficulty = "Hard"; // Gán chế độ Hard
            StartBotGame();  // Gọi hàm bắt đầu trò chơi với chế độ Hard
        }

        // Khi nhấn nút Extremely Hard
        private void btn_extremely_hard_Click(object sender, EventArgs e)
        {
            botDifficulty = "Extremely Hard"; // Gán chế độ Extremely Hard
            StartBotGame();  // Gọi hàm bắt đầu trò chơi với chế độ Extremely Hard
        }

        private void StartBotGame()
        {
            // Gọi hàm thiết lập chế độ chơi cho bot và bắt đầu trò chơi
            SetBotDifficulty(botDifficulty);

            // Ẩn BotDifficulty và hiển thị Form chơi chính
            var gameForm = new PvE(botDifficulty, _playerName);
            gameForm.Show();
            this.Hide(); // Ẩn BotDifficulty form
        }

        private void SetBotDifficulty(string difficulty)
        {
            switch (difficulty)
            {
                case "Easy":
                    // Cấu hình bot cho chế độ Easy
                    break;
                case "Medium":
                    // Cấu hình bot cho chế độ Medium
                    break;
                case "Hard":
                    // Cấu hình bot cho chế độ Hard
                    break;
                case "Extremely Hard":
                    // Cấu hình bot cho chế độ Extremely Hard
                    break;
                default:
                    // Nếu không có chế độ nào được chọn, có thể xử lý mặc định
                    break;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
