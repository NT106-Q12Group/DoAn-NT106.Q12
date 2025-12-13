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
            string message = "Signed out successfully!";
            string caption = "Logout";
            MessageBoxIcon icon = MessageBoxIcon.Information;

            try
            {
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

                // ✅ LẤY SIGNIN CŨ (MainForm) - KHÔNG TẠO MỚI
                var signin = Application.OpenForms.OfType<SignIn>().FirstOrDefault();

                // Đóng UserInfo trước
                this.Close();

                // Đóng Dashboard (Owner) để quay về SignIn
                if (this.Owner != null && !this.Owner.IsDisposed)
                    this.Owner.Close();

                // Sau khi đóng dashboard, event dash.FormClosed sẽ Show() SignIn cũ rồi.
                // Nếu muốn chắc chắn: show + messagebox
                if (signin != null && !signin.IsDisposed)
                {
                    signin.BeginInvoke((Action)(() =>
                    {
                        signin.Show();
                        signin.Activate();
                        MessageBox.Show(signin, message, caption, MessageBoxButtons.OK, icon);
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while signing out: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnEditPassword_Click(object sender, EventArgs e)
        {
            using (var resetPsw = new ResetPassword(_client))
            {
                this.Hide();

                // mở modal, khóa các form khác (không lòi Dashboard)
                resetPsw.StartPosition = FormStartPosition.CenterParent;
                resetPsw.ShowDialog(this);

                // chỉ show lại nếu UserInfo vẫn còn sống
                if (!this.IsDisposed)
                {
                    this.Show();
                    this.Activate();
                }
            }
        }

    }
}
