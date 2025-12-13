using System;
using CaroGame_TCPClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CaroGame
{
    public partial class SignUp : Form
    {
        private const string PH_EMAIL = "Email";
        private const string PH_USERNAME = "Username";
        private const string PH_PASSWORD = "Password";
        private const string PH_CONFIRM = "Confirm password";

        private bool _pswPlaceholderActive = true;
        private bool _cfPlaceholderActive = true;

        private readonly TCPClient _client;

        public SignUp(TCPClient client)
        {
            InitializeComponent();
            _client = client;

            SetPlaceholder(tb_email, PH_EMAIL);
            SetPlaceholder(tb_username, PH_USERNAME);

            SetPswPlaceholder(tb_psw, PH_PASSWORD);
            SetCfPswPlaceholder(tb_cfpsw, PH_CONFIRM);

            tb_psw.KeyPress += BlockWhitespace_KeyPress;
            tb_cfpsw.KeyPress += BlockWhitespace_KeyPress;

            // Nếu Designer đã gắn event CheckedChanged thì không cần += ở đây nữa.
            // (nhưng có cũng không sao, miễn 2 hàm handler tồn tại)
            cb_showpsw.CheckedChanged += cb_showpsw_CheckedChanged;
            cb_showcfpsw.CheckedChanged += cb_showcfpsw_CheckedChanged;

            if (dateTimePicker1 != null)
            {
                dateTimePicker1.Format = DateTimePickerFormat.Custom;
                dateTimePicker1.CustomFormat = "yyyy-MM-dd";
                dateTimePicker1.MaxDate = DateTime.Today;
                dateTimePicker1.Value = DateTime.Today.AddYears(-18);
            }
        }

        private void UpdatePasswordMasking()
        {
            // password
            tb_psw.UseSystemPasswordChar = (!_pswPlaceholderActive && !cb_showpsw.Checked);
            // confirm password
            tb_cfpsw.UseSystemPasswordChar = (!_cfPlaceholderActive && !cb_showcfpsw.Checked);
        }

        // ✅ GIỮ ĐÚNG TÊN để khớp với Designer
        private void cb_showpsw_CheckedChanged(object? sender, EventArgs e)
        {
            UpdatePasswordMasking();
        }

        // ✅ GIỮ ĐÚNG TÊN để khớp với Designer
        private void cb_showcfpsw_CheckedChanged(object? sender, EventArgs e)
        {
            UpdatePasswordMasking();
        }

        private void SetPlaceholder(TextBox tb, string text)
        {
            tb.Text = text;
            tb.ForeColor = Color.Gray;

            tb.Enter += (s, e) =>
            {
                if (tb.Text == text)
                {
                    tb.Text = "";
                    tb.ForeColor = Color.Black;
                }
            };

            tb.Leave += (s, e) =>
            {
                if (string.IsNullOrEmpty(tb.Text))
                {
                    tb.Text = text;
                    tb.ForeColor = Color.Gray;
                }
            };
        }

        private void SetPswPlaceholder(TextBox tb, string text)
        {
            _pswPlaceholderActive = true;
            tb.Text = text;
            tb.ForeColor = Color.Gray;
            tb.UseSystemPasswordChar = false;

            UpdatePasswordMasking();

            tb.Enter += (s, e) =>
            {
                if (_pswPlaceholderActive)
                {
                    tb.Text = "";
                    tb.ForeColor = Color.Black;
                    _pswPlaceholderActive = false;
                    UpdatePasswordMasking();
                }
            };

            tb.Leave += (s, e) =>
            {
                if (string.IsNullOrEmpty(tb.Text))
                {
                    _pswPlaceholderActive = true;
                    tb.Text = text;
                    tb.ForeColor = Color.Gray;
                    tb.UseSystemPasswordChar = false; // placeholder hiện chữ
                }
                UpdatePasswordMasking();
            };

            tb.TextChanged += (s, e) =>
            {
                if (_pswPlaceholderActive) return;
                UpdatePasswordMasking();
            };
        }

        private void SetCfPswPlaceholder(TextBox tb, string text)
        {
            _cfPlaceholderActive = true;
            tb.Text = text;
            tb.ForeColor = Color.Gray;
            tb.UseSystemPasswordChar = false;

            UpdatePasswordMasking();

            tb.Enter += (s, e) =>
            {
                if (_cfPlaceholderActive)
                {
                    tb.Text = "";
                    tb.ForeColor = Color.Black;
                    _cfPlaceholderActive = false;
                    UpdatePasswordMasking();
                }
            };

            tb.Leave += (s, e) =>
            {
                if (string.IsNullOrEmpty(tb.Text))
                {
                    _cfPlaceholderActive = true;
                    tb.Text = text;
                    tb.ForeColor = Color.Gray;
                    tb.UseSystemPasswordChar = false; // placeholder hiện chữ
                }
                UpdatePasswordMasking();
            };

            tb.TextChanged += (s, e) =>
            {
                if (_cfPlaceholderActive) return;
                UpdatePasswordMasking();
            };
        }

        private void BlockWhitespace_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Password must not contain spaces!", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // 2 linklabel của bạn: cho cả 2 gọi chung để khỏi rối
        private void GoBackToSignIn()
        {
            if (this.Owner is SignIn si && !si.IsDisposed)
            {
                si.Show();
                si.Activate();
                this.Close();
                return;
            }

            var si2 = Application.OpenForms.OfType<SignIn>().FirstOrDefault();
            if (si2 != null && !si2.IsDisposed)
            {
                si2.Show();
                si2.Activate();
                this.Close();
                return;
            }

            var signin = new SignIn(_client);
            signin.FormClosed += (s, _) => Application.Exit();
            signin.Show();
            this.Close();
        }

        private void linklb_signin_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            GoBackToSignIn();
        }

        private void linkedlb_signin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            GoBackToSignIn();
        }

        private void btn_signup_Click(object? sender, EventArgs e)
        {
            var ep = new ErrorProvider();

            string email = tb_email.Text.Trim();
            string username = tb_username.Text.Trim();

            string password = _pswPlaceholderActive ? "" : tb_psw.Text;
            string confirm = _cfPlaceholderActive ? "" : tb_cfpsw.Text;

            DateTime birthDate = dateTimePicker1.Value.Date;
            string birth = birthDate.ToString("yyyy-MM-dd");

            bool ok = true;

            if (email == PH_EMAIL || !email.Contains('@') || !email.Contains('.'))
            {
                ep.SetError(tb_email, "Invalid email address!");
                ok = false;
            }
            else ep.SetError(tb_email, "");

            if (username == PH_USERNAME || username.Length < 4 || username.Length > 20)
            {
                ep.SetError(tb_username, "Username must be 4–20 characters!");
                ok = false;
            }
            else if (!username.All(char.IsLetterOrDigit))
            {
                ep.SetError(tb_username, "Username must be alphanumeric (A–Z, 0–9) only!");
                ok = false;
            }
            else ep.SetError(tb_username, "");

            bool hasUpper = false, hasLower = false, hasDigit = false, hasSpecial = false;
            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsLower(c)) hasLower = true;
                else if (char.IsDigit(c)) hasDigit = true;
                else if (!char.IsLetterOrDigit(c)) hasSpecial = true;
            }

            if (password.Length < 8 || !hasUpper || !hasLower || !hasDigit || !hasSpecial)
            {
                ep.SetError(tb_psw, "Password must be ≥8 and include uppercase, lowercase, digit, and special character!");
                ok = false;
            }
            else ep.SetError(tb_psw, "");

            if (!string.Equals(password, confirm, StringComparison.Ordinal))
            {
                ep.SetError(tb_cfpsw, "Password confirmation does not match!");
                ok = false;
            }
            else ep.SetError(tb_cfpsw, "");

            int age = DateTime.Today.Year - birthDate.Year;
            if (birthDate > DateTime.Today.AddYears(-age)) age--;

            if (age < 13)
            {
                MessageBox.Show("You must be at least 13 years old!", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ok) return;

            try
            {
                if (!_client.IsConnected())
                {
                    if (!_client.Connect())
                    {
                        MessageBox.Show("Cannot connect to server. Please check if the server is running.", "Connection Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                string response = _client.Register(username, password, email, birth);
                var parts = response.Split('|');

                if (parts.Length > 0 && parts[0].Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(parts.Length > 1 ? parts[1] : "Registration successful!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    GoBackToSignIn(); // ✅ không new SignIn
                }
                else
                {
                    string msg = parts.Length > 1 ? parts[1] : "Registration failed.";
                    MessageBox.Show(msg, "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
