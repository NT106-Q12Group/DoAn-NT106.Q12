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
    public partial class WaitingRoom : Form
    {
        private Room room;
        private int playerNumber;

        public WaitingRoom(Room room, int playerNumber)
        {
            InitializeComponent();

            this.room = room;
            this.playerNumber = playerNumber;

            rtbID.Text = room.roomID;
            updateUI();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
            var DashBoard = new Dashboard();
            DashBoard.Show();
        }

        private void btnReady_Click(object sender, EventArgs e)
        {
            if (ptbReady1 != null)
                ptbReady1.Visible = !ptbReady1.Visible;

            if (room.player1Ready && room.player2Ready)
            {
                var PvPForm = new PvP(room, playerNumber); // truyền room cho form game
                PvPForm.Show();
                this.Hide();
            }
        }

        private void updateUI()
        {
            ptbReady1.Visible = room.player1Ready;
            ptbReady2.Visible = room.player2Ready;
        }
    }
}
