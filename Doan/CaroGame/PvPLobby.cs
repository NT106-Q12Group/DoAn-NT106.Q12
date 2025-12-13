using System;
using System.Drawing;
using System.Windows.Forms;
using CaroGame_TCPClient;

namespace CaroGame
{
    public partial class PvPLobby : Form
    {
        private string _username;
        private TCPClient _client;
        private bool _isQuickMatch;
        private bool _matchFound = false;

        public PvPLobby()
        {
            InitializeComponent();
        }

        public PvPLobby(string username, TCPClient client, bool isQuickMatch = true)
        {
            InitializeComponent();
            _username = username;
            _client = client;
            _isQuickMatch = isQuickMatch;

            this.FormClosed += PvPLobby_FormClosed;

            if (_client != null)
                _client.OnMessageReceived += ProcessServerMessage;
        }

        private void PvPLobby_Load(object sender, EventArgs e)
        {
            SetupUI();
            ReLayoutLobbyUI();
        }

        public void ProcessServerMessage(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(ProcessServerMessage), new object[] { message });
                return;
            }

            try
            {
                string[] parts = message.Trim().Split('|');
                string command = parts[0];

                if (command == "MATCH_FOUND")
                {
                    _matchFound = true;

                    if (_client != null)
                        _client.OnMessageReceived -= ProcessServerMessage;

                    string opponentName = parts.Length > 1 ? parts[1] : "Unknown";
                    string sideRaw = parts.Length > 2 ? parts[2] : "O";

                    OnMatchFound(opponentName, sideRaw);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi Lobby: " + ex.Message);
            }
        }

        private void OnMatchFound(string opponentName, string sideRaw)
        {
            if (progressBar1 != null) progressBar1.Visible = false;

            if (lb_status != null)
            {
                lb_status.Text = "MATCH FOUND!";
                lb_status.ForeColor = Color.Green;
            }

            if (lb_username2 != null)
            {
                lb_username2.Text = opponentName;
                lb_username2.ForeColor = Color.Black;
                lb_username2.Font = new Font(lb_username2.Font, FontStyle.Bold);
            }

            if (pictureBox2 != null)
                pictureBox2.BackColor = Color.PaleVioletRed;

            ReLayoutLobbyUI();

            var transitionTimer = new System.Windows.Forms.Timer();
            transitionTimer.Interval = 1500;
            transitionTimer.Tick += (s, e) =>
            {
                transitionTimer.Stop();
                EnterGame(opponentName, sideRaw);
            };
            transitionTimer.Start();
        }

        private void EnterGame(string opponentName, string sideRaw)
        {
            if (_client != null)
                _client.OnMessageReceived -= ProcessServerMessage;

            Room roomInfo = new Room();
            string p1, p2;
            int mySide; // 0 = X, 1 = O

            if (sideRaw == "1" || sideRaw.ToUpper() == "X")
            {
                p1 = _username;
                p2 = opponentName;
                mySide = 0;
            }
            else
            {
                p1 = opponentName;
                p2 = _username;
                mySide = 1;
            }

            var gameForm = new PvP(roomInfo, mySide, p1, p2, _client);

            this.Hide();
            gameForm.FormClosed += (s, args) => this.Close();
            gameForm.Show();
        }

        // UI layout helpers
        private void CenterLabelInPictureBox(Label lb, PictureBox pb, int yOffset = 0)
        {
            if (lb == null || pb == null) return;

            lb.Left = pb.Left + (pb.Width - lb.Width) / 2;
            lb.Top = pb.Bottom + 8 + yOffset;
        }

        private void CenterLabelInForm(Label lb, int top)
        {
            if (lb == null) return;
            lb.Left = (this.ClientSize.Width - lb.Width) / 2;
            lb.Top = top;
        }

        private void CenterProgressInForm(ProgressBar pb, int top)
        {
            if (pb == null) return;
            pb.Left = (this.ClientSize.Width - pb.Width) / 2;
            pb.Top = top;
        }

        private void ReLayoutLobbyUI()
        {
            CenterLabelInPictureBox(lb_username1, pictureBox1);
            CenterLabelInPictureBox(lb_username2, pictureBox2);

            int statusTop = 35;
            int progressTop = statusTop + 35;

            CenterLabelInForm(lb_status, statusTop);
            CenterProgressInForm(progressBar1, progressTop);

            // Fix progressBar bị pictureBox3 che: đưa lên trên cùng
            if (pictureBox3 != null) pictureBox3.SendToBack();
            if (lb_status != null) lb_status.BringToFront();
            if (progressBar1 != null) progressBar1.BringToFront();

            if (pictureBox3 != null)
            {
                // Căn giữa theo form
                pictureBox3.Left = (this.ClientSize.Width - pictureBox3.Width) / 2;

                // Đẩy xuống thấp hơn status + progress
                pictureBox3.Top = progressBar1.Top + progressBar1.Height + 10;
            }

        }


        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ReLayoutLobbyUI();
        }

        private void SetupUI()
        {
            if (lb_username1 != null)
            {
                lb_username1.Text = _username;
                lb_username1.ForeColor = Color.Black;
                lb_username1.Font = new Font(lb_username1.Font, FontStyle.Bold);
            }

            if (pictureBox1 != null)
                pictureBox1.BackColor = Color.LightBlue;

            if (lb_username2 != null)
            {
                lb_username2.Text = "???";
                lb_username2.ForeColor = Color.DimGray;
                lb_username2.Font = new Font(lb_username2.Font, FontStyle.Bold);
            }

            if (pictureBox2 != null)
            {
                pictureBox2.Image = null;
                pictureBox2.BackColor = Color.Silver;
            }

            if (lb_status != null)
            {
                lb_status.Text = "Waiting...";
                lb_status.ForeColor = Color.Black;
            }

            if (progressBar1 != null)
                progressBar1.Visible = false;

            if (_isQuickMatch && _client != null)
                StartSearching();

            ReLayoutLobbyUI();
        }

        private void StartSearching()
        {
            if (lb_status != null)
            {
                lb_status.Text = "Finding Match...";
                lb_status.ForeColor = Color.DarkOrange;
            }

            if (progressBar1 != null)
            {
                progressBar1.Visible = true;
                progressBar1.Style = ProgressBarStyle.Marquee;
            }

            ReLayoutLobbyUI();

            if (_client != null)
                _client.Send($"FIND_MATCH|{_username}");
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PvPLobby_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_client != null)
            {
                _client.OnMessageReceived -= ProcessServerMessage;

                if (!_matchFound && _isQuickMatch)
                    _client.Send($"CANCEL_MATCH|{_username}");
            }
        }

        private void progressBar1_Click(object sender, EventArgs e) { }
    }
}
