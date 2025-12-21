using System;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Windows.Forms;
using CaroGame_TCPClient;

namespace CaroGame
{
    public partial class UserInfo : Form
    {
        private readonly PlayerView _player;
        private readonly TCPClient _client;

        private bool _allowShowPasswordTick = false;

        private string _email = "";
        private string _birthday = "";

        // Khởi tạo form với player + client đang dùng
        public UserInfo(PlayerView player, TCPClient client)
        {
            InitializeComponent();
            _player = player;
            _client = client;

            Load += Dashboard_Load;
        }

        // Nạp dữ liệu player lên UI + reset trạng thái checkbox show password
        private void Dashboard_Load(object? sender, EventArgs e)
        {
            tb_username.ForeColor = Color.Black;
            tb_email.ForeColor = Color.Black;
            tb_birthdate.ForeColor = Color.Black;

            tb_username.Text = string.IsNullOrWhiteSpace(_player.PlayerName) ? "-" : _player.PlayerName.Trim();

            _email = string.IsNullOrWhiteSpace(_player.Email) ? "" : _player.Email.Trim();
            _birthday = string.IsNullOrWhiteSpace(_player.Birthday) ? "" : _player.Birthday.Trim();

            tb_email.Text = string.IsNullOrWhiteSpace(_email) ? "-" : _email;
            tb_birthdate.Text = string.IsNullOrWhiteSpace(_birthday) ? "-" : _birthday;

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

        // Toggle show/hide password (bắt confirm trước khi bật)
        private void cb_showcfpswUInfo_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_showcfpswUInfo.Checked)
            {
                if (!_allowShowPasswordTick)
                {
                    cb_showcfpswUInfo.CheckedChanged -= cb_showcfpswUInfo_CheckedChanged;
                    cb_showcfpswUInfo.Checked = false;
                    cb_showcfpswUInfo.CheckedChanged += cb_showcfpswUInfo_CheckedChanged;

                    if (!RequirePasswordConfirmation("Nhập mật khẩu hiện tại để hiện password")) return;

                    _allowShowPasswordTick = true;

                    cb_showcfpswUInfo.CheckedChanged -= cb_showcfpswUInfo_CheckedChanged;
                    cb_showcfpswUInfo.Checked = true;
                    cb_showcfpswUInfo.CheckedChanged += cb_showcfpswUInfo_CheckedChanged; // FIX: đúng event CheckedChanged

                    RevealPassword();
                    return;
                }

                RevealPassword();
                _allowShowPasswordTick = false;
                return;
            }

            _allowShowPasswordTick = false;
            MaskPassword();
        }

        // Popup nhập mật khẩu hiện tại và verify với server
        private bool RequirePasswordConfirmation(string prompt)
        {
            using (var dlg = new ConfirmPasswordDialog(prompt))
            {
                dlg.StartPosition = FormStartPosition.CenterParent;
                var dr = dlg.ShowDialog(this);
                if (dr != DialogResult.OK) return false;

                if (!VerifyCurrentPassword(dlg.Password))
                {
                    MessageBox.Show(this, "Mật khẩu không đúng.", "Xác nhận thất bại",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                return true;
            }
        }

        // Verify password hiện tại bằng API server
        private bool VerifyCurrentPassword(string currentPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_player.PlayerName)) return false;
                if (_client == null || !_client.IsConnected()) return false;

                string resp = _client.VerifyPassword(_player.PlayerName.Trim(), currentPassword);
                var parts = (resp ?? "").Split('|');

                return (parts.Length > 0 && parts[0].Equals("SUCCESS", StringComparison.OrdinalIgnoreCase));
            }
            catch
            {
                return false;
            }
        }

        // Hiện password trong textbox
        private void RevealPassword()
        {
            if (tb_password == null) return;
            tb_password.UseSystemPasswordChar = false;
        }

        // Ẩn password trong textbox
        private void MaskPassword()
        {
            if (tb_password == null) return;
            tb_password.UseSystemPasswordChar = true;
        }

        // Đổi email: confirm mật khẩu -> validate -> gọi server update
        private void btnEditEmail_Click(object sender, EventArgs e)
        {
            if (!RequirePasswordConfirmation("Nhập mật khẩu hiện tại để cập nhật email")) return;

            using (var dlg = new InputDialog("Cập nhật Email", "Nhập email mới", _email))
            {
                dlg.StartPosition = FormStartPosition.CenterParent;
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                string newEmail = (dlg.Value ?? "").Trim();
                if (!IsValidEmail(newEmail))
                {
                    MessageBox.Show(this, "Email không hợp lệ.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (_client == null || !_client.IsConnected())
                {
                    MessageBox.Show(this, "Mất kết nối server.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    string resp = _client.UpdateEmail(_player.PlayerName, newEmail);
                    var p = (resp ?? "").Split('|');

                    if (p.Length > 0 && p[0].Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        _email = newEmail;
                        if (tb_email != null) tb_email.Text = _email;

                        MessageBox.Show(this, "Cập nhật email thành công!", "Thành công!",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string msg = (p.Length > 1) ? p[1] : "Update email failed.";
                        MessageBox.Show(this, msg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Lỗi: " + ex.Message, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Đổi ngày sinh: confirm mật khẩu -> chọn ngày -> gọi server update
        private void btnEditBirth_Click(object sender, EventArgs e)
        {
            if (!RequirePasswordConfirmation("Nhập mật khẩu hiện tại để cập nhật ngày sinh")) return;

            using (var dlg = new DatePickerDialog("Cập nhật Birthday", "Chọn ngày sinh", _birthday))
            {
                dlg.StartPosition = FormStartPosition.CenterParent;
                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                string newBirth = dlg.SelectedDate.ToString("dd-MM-yyyy");

                if (_client == null || !_client.IsConnected())
                {
                    MessageBox.Show(this, "Mất kết nối server.", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    string resp = _client.UpdateBirthday(_player.PlayerName, newBirth);
                    var p = (resp ?? "").Split('|');

                    if (p.Length > 0 && p[0].Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        _birthday = newBirth;
                        if (tb_birthdate != null) tb_birthdate.Text = _birthday;

                        MessageBox.Show(this, "Cập nhật birthday thành công!", "Thành công!",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        string msg = (p.Length > 1) ? p[1] : "Update birthday failed.";
                        MessageBox.Show(this, msg, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "Lỗi: " + ex.Message, "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Check format email bằng MailAddress
        private static bool IsValidEmail(string email)
        {
            try
            {
                var _ = new MailAddress(email);
                return true;
            }
            catch { return false; }
        }

        // Check chuỗi ngày theo dd-MM-yyyy (nếu cần)
        private static bool IsValidBirthday(string s)
        {
            return DateTime.TryParseExact(
                s,
                "dd-MM-yyyy",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out _);
        }

        // Logout: gọi server, disconnect client, quay về SignIn
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

        // Back: đóng form hiện tại
        private void btnBack_Click(object sender, EventArgs e) => this.Close();

        // Mở màn hình đổi mật khẩu (modal) và restore lại UserInfo sau khi đóng
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
            private readonly TextBox tb;
            private readonly CheckBox cbShow;

            public string Password => tb.Text;

            // Dialog xác nhận mật khẩu (1 ô nhập + toggle hiện/ẩn)
            public ConfirmPasswordDialog(string promptText = "Nhập mật khẩu hiện tại để xác nhận")
            {
                Text = "Xác nhận";
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                ShowInTaskbar = false;
                StartPosition = FormStartPosition.CenterParent;
                Width = 430;
                Height = 210;

                var lbl = new Label
                {
                    Text = promptText,
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

                cbShow = new CheckBox
                {
                    Text = "Hiện",
                    AutoSize = true,
                    Left = tb.Left,
                    Top = tb.Top + tb.Height + 8
                };

                cbShow.CheckedChanged += (s, e) =>
                {
                    tb.UseSystemPasswordChar = !cbShow.Checked;
                    tb.Focus();
                    tb.SelectionStart = tb.TextLength;
                };

                int btnW = 140, btnH = 38, gap = 16;
                int totalW = btnW * 2 + gap;
                int startX = (ClientSize.Width - totalW) / 2;
                int topY = cbShow.Top + cbShow.Height + 12;

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
                Controls.Add(cbShow);
                Controls.Add(btnOk);
                Controls.Add(btnCancel);
            }
        }

        private class InputDialog : Form
        {
            private readonly TextBox tb;
            public string Value => tb.Text;

            // Dialog nhập 1 dòng text (dùng cho email)
            public InputDialog(string title, string label, string initial)
            {
                Text = title;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                ShowInTaskbar = false;
                Width = 460;
                Height = 210;

                var lbl = new Label
                {
                    Text = label,
                    AutoSize = false,
                    Width = 430,
                    Height = 30,
                    Left = 10,
                    Top = 15,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                tb = new TextBox
                {
                    Width = 360,
                    Left = 45,
                    Top = 55,
                    Text = initial ?? ""
                };

                int btnW = 150, btnH = 40, gap = 16;
                int totalW = btnW * 2 + gap;
                int startX = (ClientSize.Width - totalW) / 2;
                int topY = 105;

                var btnOk = new Button
                {
                    Text = "Lưu",
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

        private class DatePickerDialog : Form
        {
            private readonly DateTimePicker dtp;
            public DateTime SelectedDate => dtp.Value.Date;

            // Dialog chọn ngày theo format dd-MM-yyyy
            public DatePickerDialog(string title, string label, string initial)
            {
                Text = title;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                ShowInTaskbar = false;
                Width = 460;
                Height = 230;

                var lbl = new Label
                {
                    Text = label,
                    AutoSize = false,
                    Width = 430,
                    Height = 30,
                    Left = 10,
                    Top = 15,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                dtp = new DateTimePicker
                {
                    Width = 200,
                    Left = (ClientSize.Width - 200) / 2,
                    Top = 60,
                    Format = DateTimePickerFormat.Custom,
                    CustomFormat = "dd-MM-yyyy",
                    MaxDate = DateTime.Today
                };

                if (DateTime.TryParseExact(
                        initial ?? "",
                        "dd-MM-yyyy",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out var d))
                {
                    dtp.Value = d;
                }

                int btnW = 150, btnH = 40, gap = 16;
                int totalW = btnW * 2 + gap;
                int startX = (ClientSize.Width - totalW) / 2;
                int topY = 120;

                var btnOk = new Button
                {
                    Text = "Lưu",
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
                Controls.Add(dtp);
                Controls.Add(btnOk);
                Controls.Add(btnCancel);
            }
        }
    }
}
