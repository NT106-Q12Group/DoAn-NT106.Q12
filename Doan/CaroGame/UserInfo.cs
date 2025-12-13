using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CaroGame_TCPClient;

namespace CaroGame
{
    public partial class UserInfo : Form
    {
        private readonly PlayerView _player;
        private readonly TCPClient _client;

        private bool _allowShowPasswordTick = false;

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

            if (tb_password != null)
            {
                tb_password.ReadOnly = true;
                tb_password.Text = _player.SessionPassword ?? "";
                tb_password.UseSystemPasswordChar = true;
            }

            cb_showcfpswUInfo.CheckedChanged -= cb_showcfpswUInfo_CheckedChanged;
            cb_showcfpswUInfo.Checked = false;
            cb_showcfpswUInfo.CheckedChanged += cb_showcfpswUInfo_CheckedChanged;
        }

        private void cb_showcfpswUInfo_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_showcfpswUInfo.Checked)
            {
                if (!_allowShowPasswordTick)
                {
                    cb_showcfpswUInfo.CheckedChanged -= cb_showcfpswUInfo_CheckedChanged;
                    cb_showcfpswUInfo.Checked = false; // tick chỉ xuất hiện sau khi confirm OK
                    cb_showcfpswUInfo.CheckedChanged += cb_showcfpswUInfo_CheckedChanged;

                    using (var dlg = new ConfirmPasswordDialog())
                    {
                        dlg.StartPosition = FormStartPosition.CenterParent;
                        var dr = dlg.ShowDialog(this);

                        if (dr != DialogResult.OK) return;

                        if (!VerifyCurrentPassword(dlg.Password))
                        {
                            MessageBox.Show(this, "Mật khẩu không đúng.", "Xác nhận thất bại",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        _allowShowPasswordTick = true;

                        cb_showcfpswUInfo.CheckedChanged -= cb_showcfpswUInfo_CheckedChanged;
                        cb_showcfpswUInfo.Checked = true;
                        cb_showcfpswUInfo.CheckedChanged += cb_showcfpswUInfo_CheckedChanged;

                        cb_showcfpswUInfo_CheckedChanged(cb_showcfpswUInfo, EventArgs.Empty);
                    }

                    return;
                }

                _allowShowPasswordTick = false;
                RevealPassword();
                return;
            }

            _allowShowPasswordTick = false;
            MaskPassword();
        }

        private bool VerifyCurrentPassword(string currentPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_player.PlayerName)) return false;
                if (_client == null || !_client.IsConnected()) return false;

                string resp = _client.Login(_player.PlayerName.Trim(), currentPassword);
                var parts = (resp ?? "").Split('|');
                return (parts.Length > 0 && parts[0].Equals("SUCCESS", StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }

        private void RevealPassword()
        {
            if (tb_password == null) return;
            tb_password.UseSystemPasswordChar = false;
        }

        private void MaskPassword()
        {
            if (tb_password == null) return;
            tb_password.UseSystemPasswordChar = true;
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

                var signin = Application.OpenForms.OfType<SignIn>().FirstOrDefault();

                this.Close();

                if (this.Owner != null && !this.Owner.IsDisposed)
                    this.Owner.Close();

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

        private void btnBack_Click(object sender, EventArgs e) => this.Close();

        private void btnEditPassword_Click(object sender, EventArgs e)
        {
            using (var resetPsw = new ResetPassword(_client))
            {
                this.Hide();
                resetPsw.StartPosition = FormStartPosition.CenterParent;
                resetPsw.ShowDialog(this);

                if (!this.IsDisposed)
                {
                    this.Show();
                    this.Activate();
                }
            }
        }

        private class ConfirmPasswordDialog : Form
        {
            private TextBox tb;
            public string Password => tb.Text;

            public ConfirmPasswordDialog()
            {
                Text = "Xác nhận";
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                ShowInTaskbar = false;
                Width = 430;
                Height = 190;

                var lbl = new Label
                {
                    Text = "Nhập mật khẩu hiện tại để hiện password",
                    AutoSize = false,
                    Width = 400,
                    Height = 30,
                    Left = 10,
                    Top = 15,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                tb = new TextBox
                {
                    Width = 320,
                    Left = 50,
                    Top = 55,
                    UseSystemPasswordChar = true
                };

                int btnW = 140, btnH = 38, gap = 16;
                int totalW = btnW * 2 + gap;

                int startX = (ClientSize.Width - totalW) / 2;
                int topY = 105;

                var btnOk = new Button
                {
                    Text = "Xác nhận",
                    Width = btnW,
                    Height = btnH,
                    Left = startX,
                    Top = topY,
                    DialogResult = DialogResult.OK
                };

                var btnCancel = new Button
                {
                    Text = "Huỷ",
                    Width = btnW,
                    Height = btnH,
                    Left = startX + btnW + gap,
                    Top = topY,
                    DialogResult = DialogResult.Cancel
                };

                AcceptButton = btnOk;
                CancelButton = btnCancel;

                Controls.Add(lbl);
                Controls.Add(tb);
                Controls.Add(btnOk);
                Controls.Add(btnCancel);
            }
        }
    }
}
