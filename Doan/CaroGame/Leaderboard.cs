using CaroGame_TCPClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaroGame
{
    public partial class Leaderboard : Form
    {
        public Leaderboard()
        {
            InitializeComponent();
            SetupGridView();
            LoadLeaderboard();
        }

        private void LoadLeaderboard()
        {
            TCPClient.Instance.RequestLeaderboard();
        }

        private void SetupGridView()
        {
            dgv_leaderboard.ColumnCount = 3;
            dgv_leaderboard.Columns[0].Name = "Hạng";
            dgv_leaderboard.Columns[1].Name = "Người Chơi";
            dgv_leaderboard.Columns[2].Name = "Điểm Số";

            dgv_leaderboard.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv_leaderboard.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv_leaderboard.AllowUserToAddRows = false;
        }

        public void UpdateLeaderboardUI(string dataFromServer)
        {
            // dataFromServer dạng: "Name1:100|Name2:90|Name3:80"

            string[] players = dataFromServer.Split('|');

            this.Invoke((MethodInvoker)delegate
            {
                dgv_leaderboard.Rows.Clear();
                int rank = 1;

                foreach (string p in players)
                {
                    if (string.IsNullOrWhiteSpace(p)) continue;

                    // Tách tên và điểm (Ví dụ: "Nguyen:10")
                    string[] info = p.Split(':');
                    if (info.Length >= 2)
                    {
                        string name = info[0];
                        string score = info[1];

                        dgv_leaderboard.Rows.Add(rank, name, score);
                        rank++;
                    }
                }
            });
        }
    }
}
