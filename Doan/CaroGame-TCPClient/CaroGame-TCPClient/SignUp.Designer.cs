namespace CaroGame_TCPClient
{
    partial class SignUp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tb_username = new TextBox();
            tb_cfpsw = new TextBox();
            tb_birth = new TextBox();
            tb_psw = new TextBox();
            tb_email = new TextBox();
            btn_signup = new Button();
            lb_signin = new Label();
            linkedlb_signin = new LinkLabel();
            cb_showpsw = new CheckBox();
            cb_showcfpsw = new CheckBox();
            lblSignUp = new Label();
            lblUsernameSU = new Label();
            lblPasswordSU = new Label();
            lblCFPassword = new Label();
            lblEmail = new Label();
            lblBirth = new Label();
            pictureBox1 = new PictureBox();
            tb_fullname = new TextBox();
            lblFullName = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // tb_username
            // 
            tb_username.Font = new Font("Segoe UI", 10F);
            tb_username.Location = new Point(29, 284);
            tb_username.Margin = new Padding(3, 4, 3, 4);
            tb_username.Name = "tb_username";
            tb_username.Size = new Size(445, 30);
            tb_username.TabIndex = 0;
            // 
            // tb_cfpsw
            // 
            tb_cfpsw.Font = new Font("Segoe UI", 10F);
            tb_cfpsw.Location = new Point(29, 422);
            tb_cfpsw.Margin = new Padding(3, 4, 3, 4);
            tb_cfpsw.Name = "tb_cfpsw";
            tb_cfpsw.Size = new Size(445, 30);
            tb_cfpsw.TabIndex = 1;
            // 
            // tb_birth
            // 
            tb_birth.Font = new Font("Segoe UI", 10F);
            tb_birth.Location = new Point(29, 550);
            tb_birth.Margin = new Padding(3, 4, 3, 4);
            tb_birth.Name = "tb_birth";
            tb_birth.Size = new Size(445, 30);
            tb_birth.TabIndex = 3;
            // 
            // tb_psw
            // 
            tb_psw.Font = new Font("Segoe UI", 10F);
            tb_psw.Location = new Point(29, 354);
            tb_psw.Margin = new Padding(3, 4, 3, 4);
            tb_psw.Name = "tb_psw";
            tb_psw.Size = new Size(445, 30);
            tb_psw.TabIndex = 4;
            // 
            // tb_email
            // 
            tb_email.Font = new Font("Segoe UI", 10F);
            tb_email.Location = new Point(29, 484);
            tb_email.Margin = new Padding(3, 4, 3, 4);
            tb_email.Name = "tb_email";
            tb_email.Size = new Size(445, 30);
            tb_email.TabIndex = 5;
            // 
            // btn_signup
            // 
            btn_signup.Cursor = Cursors.Hand;
            btn_signup.Location = new Point(206, 603);
            btn_signup.Margin = new Padding(3, 4, 3, 4);
            btn_signup.Name = "btn_signup";
            btn_signup.Size = new Size(86, 31);
            btn_signup.TabIndex = 6;
            btn_signup.Text = "Sign Up";
            btn_signup.UseVisualStyleBackColor = true;
            btn_signup.Click += btn_signup_Click;
            // 
            // lb_signin
            // 
            lb_signin.AutoSize = true;
            lb_signin.Location = new Point(130, 640);
            lb_signin.Name = "lb_signin";
            lb_signin.Size = new Size(178, 20);
            lb_signin.TabIndex = 7;
            lb_signin.Text = "Already have an account?";
            // 
            // linkedlb_signin
            // 
            linkedlb_signin.AutoSize = true;
            linkedlb_signin.Location = new Point(307, 640);
            linkedlb_signin.Name = "linkedlb_signin";
            linkedlb_signin.Size = new Size(54, 20);
            linkedlb_signin.TabIndex = 8;
            linkedlb_signin.TabStop = true;
            linkedlb_signin.Text = "Sign In";
            linkedlb_signin.LinkClicked += linkedlb_signin_LinkClicked;
            // 
            // cb_showpsw
            // 
            cb_showpsw.AutoSize = true;
            cb_showpsw.Cursor = Cursors.Hand;
            cb_showpsw.Location = new Point(340, 396);
            cb_showpsw.Margin = new Padding(3, 4, 3, 4);
            cb_showpsw.Name = "cb_showpsw";
            cb_showpsw.Size = new Size(134, 24);
            cb_showpsw.TabIndex = 9;
            cb_showpsw.Text = "Show password";
            cb_showpsw.UseVisualStyleBackColor = true;
            cb_showpsw.CheckedChanged += cb_showpsw_CheckedChanged;
            // 
            // cb_showcfpsw
            // 
            cb_showcfpsw.AutoSize = true;
            cb_showcfpsw.Cursor = Cursors.Hand;
            cb_showcfpsw.Location = new Point(340, 328);
            cb_showcfpsw.Margin = new Padding(3, 4, 3, 4);
            cb_showcfpsw.Name = "cb_showcfpsw";
            cb_showcfpsw.Size = new Size(134, 24);
            cb_showcfpsw.TabIndex = 10;
            cb_showcfpsw.Text = "Show password";
            cb_showcfpsw.UseVisualStyleBackColor = true;
            cb_showcfpsw.CheckedChanged += cb_showcfpsw_CheckedChanged;
            // 
            // lblSignUp
            // 
            lblSignUp.AutoSize = true;
            lblSignUp.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblSignUp.Location = new Point(189, 135);
            lblSignUp.Name = "lblSignUp";
            lblSignUp.Size = new Size(130, 41);
            lblSignUp.TabIndex = 11;
            lblSignUp.Text = "Sign Up";
            // 
            // lblUsernameSU
            // 
            lblUsernameSU.AutoSize = true;
            lblUsernameSU.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblUsernameSU.Location = new Point(29, 257);
            lblUsernameSU.Name = "lblUsernameSU";
            lblUsernameSU.Size = new Size(89, 23);
            lblUsernameSU.TabIndex = 12;
            lblUsernameSU.Text = "Username";
            // 
            // lblPasswordSU
            // 
            lblPasswordSU.AutoSize = true;
            lblPasswordSU.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPasswordSU.Location = new Point(29, 327);
            lblPasswordSU.Name = "lblPasswordSU";
            lblPasswordSU.Size = new Size(85, 23);
            lblPasswordSU.TabIndex = 13;
            lblPasswordSU.Text = "Password";
            // 
            // lblCFPassword
            // 
            lblCFPassword.AutoSize = true;
            lblCFPassword.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblCFPassword.Location = new Point(29, 395);
            lblCFPassword.Name = "lblCFPassword";
            lblCFPassword.Size = new Size(156, 23);
            lblCFPassword.TabIndex = 14;
            lblCFPassword.Text = "Confirm Password";
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblEmail.Location = new Point(29, 457);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(54, 23);
            lblEmail.TabIndex = 15;
            lblEmail.Text = "Email";
            // 
            // lblBirth
            // 
            lblBirth.AutoSize = true;
            lblBirth.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblBirth.Location = new Point(29, 523);
            lblBirth.Name = "lblBirth";
            lblBirth.Size = new Size(50, 23);
            lblBirth.TabIndex = 16;
            lblBirth.Text = "Birth";
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = Properties.Resources.CaroPicGame;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Location = new Point(192, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(120, 120);
            pictureBox1.TabIndex = 17;
            pictureBox1.TabStop = false;
            // 
            // tb_fullname
            // 
            tb_fullname.Font = new Font("Segoe UI", 10F);
            tb_fullname.Location = new Point(29, 213);
            tb_fullname.Name = "tb_fullname";
            tb_fullname.Size = new Size(445, 30);
            tb_fullname.TabIndex = 18;
            // 
            // lblFullName
            // 
            lblFullName.AutoSize = true;
            lblFullName.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblFullName.Location = new Point(29, 187);
            lblFullName.Name = "lblFullName";
            lblFullName.Size = new Size(91, 23);
            lblFullName.TabIndex = 19;
            lblFullName.Text = "Full Name";
            // 
            // SignUp
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(505, 669);
            Controls.Add(lblFullName);
            Controls.Add(tb_fullname);
            Controls.Add(pictureBox1);
            Controls.Add(lblBirth);
            Controls.Add(lblEmail);
            Controls.Add(lblCFPassword);
            Controls.Add(lblPasswordSU);
            Controls.Add(lblUsernameSU);
            Controls.Add(lblSignUp);
            Controls.Add(cb_showcfpsw);
            Controls.Add(cb_showpsw);
            Controls.Add(linkedlb_signin);
            Controls.Add(lb_signin);
            Controls.Add(btn_signup);
            Controls.Add(tb_email);
            Controls.Add(tb_psw);
            Controls.Add(tb_birth);
            Controls.Add(tb_cfpsw);
            Controls.Add(tb_username);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            Name = "SignUp";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Sign Up";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tb_username;
        private TextBox tb_cfpsw;
        private TextBox tb_birth;
        private TextBox tb_psw;
        private TextBox tb_email;
        private Button btn_signup;
        private Label lb_signin;
        private LinkLabel linkedlb_signin;
        private CheckBox cb_showpsw;
        private CheckBox cb_showcfpsw;
        private Label lblSignUp;
        private Label lblUsernameSU;
        private Label lblPasswordSU;
        private Label lblCFPassword;
        private Label lblEmail;
        private Label lblBirth;
        private PictureBox pictureBox1;
        private TextBox tb_fullname;
        private Label lblFullName;
    }
}