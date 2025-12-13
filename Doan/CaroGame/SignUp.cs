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

        private readonly TCPClient _client;

        public SignUp(TCPClient client)
        {
            InitializeComponent();
            _client = client;

            // Placeholder for textboxes
            SetPlaceholder(tb_email, PH_EMAIL);
            SetPlaceholder(tb_username, PH_USERNAME);
            SetPswPlaceholder(tb_psw, PH_PASSWORD);
            SetPswPlaceholder(tb_cfpsw, PH_CONFIRM);

            // Block whitespace in password
            tb_psw.KeyPress += BlockWhitespace_KeyPress;
            tb_cfpsw.KeyPress += BlockWhitespace_KeyPress;

            // Show/hide password
            cb_showpsw.CheckedChanged += cb_showpsw_CheckedChanged;
            cb_showcfpsw.CheckedChanged += cb_showcfpsw_CheckedChanged;

            // DateTimePicker settings (Birthdate)
            if (dateTimePicker1 != null)
            {
                dateTimePicker1.Format = DateTimePickerFormat.Custom;
                dateTimePicker1.CustomFormat = "yyyy-MM-dd";
                dateTimePicker1.MaxDate = DateTime.Today;              // no future dates
                dateTimePicker1.Value = DateTime.Today.AddYears(-18);  // default 18 years old (optional)
            }
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
            tb.Text = text;
            tb.ForeColor = Color.Gray;
            tb.UseSystemPasswordChar = false;

            tb.Enter += (s, e) =>
            {
                if (tb.Text == text)
                {
                    tb.UseSystemPasswordChar = true;
                    tb.Text = "";
                    tb.ForeColor = Color.Black;
                }
            };

            tb.Leave += (s, e) =>
            {
                if (string.IsNullOrEmpty(tb.Text))
                {
                    tb.UseSystemPasswordChar = false;
                    tb.Text = text;
                    tb.ForeColor = Color.Gray;
                }
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

        private void cb_showpsw_CheckedChanged(object? sender, EventArgs e)
        {
            if (tb_psw.Text == PH_PASSWORD)
            {
                tb_psw.UseSystemPasswordChar = false;
                return;
            }
            tb_psw.UseSystemPasswordChar = !cb_showpsw.Checked;
        }

        private void cb_showcfpsw_CheckedChanged(object? sender, EventArgs e)
        {
            if (tb_cfpsw.Text == PH_CONFIRM)
            {
                tb_cfpsw.UseSystemPasswordChar = false;
                return;
            }
            tb_cfpsw.UseSystemPasswordChar = !cb_showcfpsw.Checked;
        }

        private void linklb_signin_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            var signIn = new SignIn(_client);
            Hide();
            signIn.FormClosed += (s, _) => Show();
            signIn.Show();
        }

        private void btn_signup_Click(object? sender, EventArgs e)
        {
            var ep = new ErrorProvider();

            string email = tb_email.Text.Trim();
            string username = tb_username.Text.Trim();
            string password = tb_psw.Text;
            string confirm = tb_cfpsw.Text;

            // Birth from DateTimePicker
            DateTime birthDate = dateTimePicker1.Value.Date;
            string birth = birthDate.ToString("yyyy-MM-dd");

            bool ok = true;

            // Email
            if (email == PH_EMAIL || !email.Contains('@') || !email.Contains('.'))
            {
                ep.SetError(tb_email, "Invalid email address!");
                ok = false;
            }
            else ep.SetError(tb_email, "");

            // Username
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

            // Password placeholders
            if (password == PH_PASSWORD) password = "";
            if (confirm == PH_CONFIRM) confirm = "";

            // Password strength
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

            // Confirm password
            if (!string.Equals(password, confirm, StringComparison.Ordinal))
            {
                ep.SetError(tb_cfpsw, "Password confirmation does not match!");
                ok = false;
            }
            else ep.SetError(tb_cfpsw, "");

            // Age validation (>= 13)
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

                    var signIn = new SignIn(_client);
                    Hide();
                    signIn.FormClosed += (s, _) => Show();
                    signIn.Show();
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

        private void linkedlb_signin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // NOTE: nếu SignIn của bạn cần TCPClient thì dùng new SignIn(_client)
            new SignIn(_client).Show();
            this.Hide();
        }
    }
}
