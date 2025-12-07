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
    public partial class Dashboard : Form
    {
        private Room room;
        private int playerNumber;
        public Dashboard()
        {
            InitializeComponent();
        }

        private void btnPvE_Click(object sender, EventArgs e)
        {
            var newGameForm = new BotDifficulty();
            newGameForm.Show();
            this.Hide();
        }

        private void pnlNaviBar_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnPvP_Click(object sender, EventArgs e)
        {
            if (pnlPvPMenu != null)
                pnlPvPMenu.Visible = !pnlPvPMenu.Visible;
            //var newGameForm = new PvP();
            //newGameForm.Show();
            //this.Hide();
        }

        private void btnCreateRoom_Click(object sender, EventArgs e)
        {
            Room newRoom = RoomManager.createRoom();
            var newWaitingRoom = new WaitingRoom(newRoom, 1);

            newWaitingRoom.Show();
            this.Hide();
        }

        private void btnJoinRoom_Click(object sender, EventArgs e)
        {
            if (pnlDashBoard != null)
            {
                pnlDashBoard.Visible = !pnlDashBoard.Visible;
                pnlPvPMenu.Visible = !pnlPvPMenu.Visible;
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (pnlDashBoard != null)
                pnlDashBoard.Visible = !pnlDashBoard.Visible;
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            string id = tbRoomID.Text.Trim();

            Room joinedRoom = RoomManager.JoinRoom(id, out int playerNumber);

            if (joinedRoom == null)
            {
                MessageBox.Show("Phòng không tồn tại hoặc đã đủ 2 người!");
                return;
            }

            var room = new WaitingRoom(joinedRoom, playerNumber);

            room.Show();
            this.Hide();
        }

        private void btnPlayInstant_Click(object sender, EventArgs e)
        {
            var newGameForm = new PvP(room, playerNumber);
            newGameForm.Show();
            this.Hide();
        }
    }
}
