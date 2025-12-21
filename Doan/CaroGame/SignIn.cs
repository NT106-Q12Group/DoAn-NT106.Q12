using CaroGame_TCPClient;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CaroGame
{
    public partial class SignIn : Form
    {
        private const string PH_USERNAME = "Username";
        private const string PH_PASSWORD = "Password";

        private readonly TCPClient _client;
        private bool _signingIn = false;
        private string _currentUser = "";
        private bool _pswPlaceholderActive = true;

        public SignIn() : this(new TCPClient("3.230.162.159", 25565)) { }

        public SignIn(TCPClient sharedClient)
        {
            InitializeComponent();
            _client = sharedClient;

            AcceptButton = btn_signin;

            SetPlaceholder(tb_username, PH_USERNAME);
            SetPswPlaceholder(tb_psw, PH_PASSWORD);

            cb_showpsw.CheckedChanged += cb_showpsw_CheckedChanged;

            tb_username.KeyPress += (s, e) =>
            {
                if (char.IsWhiteSpace(e.KeyChar))
                {
                    e.Handled = true;
                    MessageBox.Show("Username cannot contain spaces!", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            tb_psw.KeyPress += (s, e) =>
            {
                if (char.IsWhiteSpace(e.KeyChar))
                {
                    e.Handled = true;
                    MessageBox.Show("Password cannot contain spaces!", "Validation",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
        }

        // mask password theo placeholder + checkbox
        private void UpdatePasswordMasking()
        {
            tb_psw.UseSystemPasswordChar = (!_pswPlaceholderActive && !cb_showpsw.Checked);
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
                    UpdatePasswordMasking();
                }
            };

            tb.TextChanged += (s, e) =>
            {
                if (_pswPlaceholderActive) return;
                UpdatePasswordMasking();
            };
        }

        private void cb_showpsw_CheckedChanged(object? sender, EventArgs e)
        {
            UpdatePasswordMasking();
        }

        // login + mở dashboard
        private void btn_signin_Click(object? sender, EventArgs e)
        {
            if (_signingIn) return;
            _signingIn = true;
            btn_signin.Enabled = false;

            try
            {
                if (tb_username.Text == PH_USERNAME || string.IsNullOrWhiteSpace(tb_username.Text))
                {
                    MessageBox.Show("Please enter your username!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tb_username.Focus();
                    return;
                }

                if (tb_psw.Text == PH_PASSWORD || string.IsNullOrWhiteSpace(tb_psw.Text))
                {
                    MessageBox.Show("Please enter your password!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tb_psw.Focus();
                    return;
                }

                if (!_client.IsConnected())
                {
                    if (!_client.Connect())
                    {
                        MessageBox.Show("Cannot connect to server. Please check if the server is running.",
                            "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                string loginResp = _client.Login(tb_username.Text.Trim(), tb_psw.Text);
                var p1 = loginResp.Split('|');

                if (p1.Length > 0 && p1[0].Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                {
                    string uname = p1.Length > 1 ? p1[1] : tb_username.Text.Trim();
                    _currentUser = uname;

                    string email = "";
                    string birthday = "";

                    string getResp = _client.GetUser(uname);
                    var p2 = getResp.Split('|');
                    if (p2.Length > 0 && p2[0].Equals("SUCCESS", StringComparison.OrdinalIgnoreCase))
                    {
                        email = p2.Length > 2 ? p2[2] : "";
                        birthday = p2.Length > 3 ? p2[3] : "";
                    }

                    var pv = new PlayerView
                    {
                        PlayerID = 0,
                        PlayerName = uname,
                        Email = email,
                        Birthday = birthday,
                        SessionPassword = (tb_psw.Text == PH_PASSWORD) ? "" : tb_psw.Text
                    };

                    _client.StartListening();

                    var dash = new Dashboard(uname, _client);
                    dash.SetPlayer(pv);

                    // dashboard đóng thì quay lại SignIn
                    dash.FormClosed += (s, _) =>
                    {
                        this.Show();
                        this.Activate();
                    };

                    this.Hide();
                    dash.Show();

                    this.BeginInvoke((Action)(() =>
                    {
                        MessageBox.Show(this, "Signed in successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }));
                }
                else
                {
                    string msg = p1.Length > 1 ? p1[1] : "Login failed.";
                    MessageBox.Show(msg, "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btn_signin.Enabled = true;
                _signingIn = false;
            }
        }

        private void linkedlb_signup_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            var signUp = new SignUp(_client);
            signUp.Show(this);

            signUp.FormClosed += (s, _) =>
            {
                if (!this.IsDisposed)
                {
                    this.Show();
                    this.Activate();
                }
            };

            this.Hide();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                if (_client.IsConnected())
                {
                    if (!string.IsNullOrEmpty(_currentUser))
                        _client.Logout(_currentUser);

                    _client.Disconnect();
                }
            }
            catch { }

            base.OnFormClosing(e);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var resetPsw = new ResetPassword(_client))
            {
                this.Hide();
                resetPsw.ShowDialog(this);
                this.Show();
                this.Activate();
            }
        }
    }
}
