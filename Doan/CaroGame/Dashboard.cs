using CaroGame_TCPClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaroGame
{
    public partial class Dashboard : Form
    {
        private Room room;
        private int playerNumber;
        private Form _userInfoForm;
        private string _loggedInUser;
        private PlayerView pv;
        private TCPClient _client;

        public Dashboard()
        {
            InitializeComponent();
            _loggedInUser = "Player"; // Mặc định nếu không truyền gì
        }

        public Dashboard(string username)
        {
            InitializeComponent();
            _loggedInUser = username;
        }

        private void btnPvE_Click(object sender, EventArgs e)
        {
            var newGameForm = new BotDifficulty(_loggedInUser);
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
            room = newRoom;
            playerNumber = 1;

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

            Room joinedRoom = RoomManager.JoinRoom(id, out int pNumber);

            if (joinedRoom == null)
            {
                MessageBox.Show("Phòng không tồn tại hoặc đã đủ 2 người!");
                return;
            }

            room = joinedRoom;
            playerNumber = pNumber;

            var waitingroom = new WaitingRoom(joinedRoom, pNumber);
            waitingroom.Show();
            this.Hide();
        }

        private void LoadPlayer(string username, string email, string birthday)
        {
            pv = new PlayerView
            {
                PlayerID = 0,
                PlayerName = username,
                Email = email,
                Birthday = birthday
            };
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            var userInfoForm = new UserInfo(pv, _client);
            userInfoForm.FormClosed += (s, args) => this.Show();
            this.Hide();
            userInfoForm.Show();
        }

        private void btnPlayInstant_Click(object sender, EventArgs e)
        {
            var newGameForm = new PvP(room, playerNumber);
            newGameForm.Show();
            this.Hide();
        }
    }
}
