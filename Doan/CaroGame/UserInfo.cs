using System;
using System.Drawing;
using CaroGame_TCPClient;
using System.Windows.Forms;

namespace CaroGame
{
    public partial class UserInfo : Form
    {
        private readonly PlayerView _player;
        private readonly TCPClient _client;

        public Action<PlayerView>? OnBack;

        public UserInfo(PlayerView player, TCPClient client)
        {
            InitializeComponent();
            _player = player;
            _client = client;

            Load += Dashboard_Load;
        }

        private void Dashboard_Load(object? sender, EventArgs e)
        {
            tb_username.ForeColor = Color.Black;
            tb_email.ForeColor = Color.Black;
            tb_birthdate.ForeColor = Color.Black;

            tb_username.Text = string.IsNullOrWhiteSpace(_player.PlayerName) ? "-" : _player.PlayerName.Trim();
            tb_email.Text = string.IsNullOrWhiteSpace(_player.Email) ? "-" : _player.Email.Trim();
            tb_birthdate.Text = string.IsNullOrWhiteSpace(_player.Birthday) ? "-" : _player.Birthday.Trim();

            tb_username.ReadOnly = true;
            tb_email.ReadOnly = true;
            tb_birthdate.ReadOnly = true;
        }

        private void btn_signout_Click(object? sender, EventArgs e)
        {
            try
            {
                string message = "Signed out successfully!";
                string caption = "Logout";
                MessageBoxIcon icon = MessageBoxIcon.Information;

                if (!string.IsNullOrEmpty(_player.PlayerName) && _client.IsConnected())
                {
                    var resp = _client.Logout(_player.PlayerName);
                    var parts = resp.Split('|');
                    if (!(parts.Length > 0 && parts[0].Equals("SUCCESS", StringComparison.OrdinalIgnoreCase)))
                    {
                        message = "Server did not confirm sign out.";
                        icon = MessageBoxIcon.Warning;
                    }
                }

                try { _client.Disconnect(); } catch { }

                Hide();
                var signin = new SignIn(_client);
                signin.Show();

                BeginInvoke((Action)(() =>
                {
                    MessageBox.Show(message, caption, MessageBoxButtons.OK, icon);
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while signing out: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }   

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(_player.PlayerName) && _client.IsConnected())
                    _client.Logout(_player.PlayerName);
                _client.Disconnect();
            }
            catch { }
            base.OnFormClosing(e);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            OnBack?.Invoke(_player);
            this.Close();
        }
    }
}
