namespace CaroGame
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
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // tb_username
            // 
            tb_username.Font = new Font("Segoe UI", 10F);
            tb_username.Location = new Point(26, 160);
            tb_username.Name = "tb_username";
            tb_username.Size = new Size(390, 25);
            tb_username.TabIndex = 1;
            // 
            // tb_cfpsw
            // 
            tb_cfpsw.Font = new Font("Segoe UI", 10F);
            tb_cfpsw.Location = new Point(26, 264);
            tb_cfpsw.Name = "tb_cfpsw";
            tb_cfpsw.Size = new Size(390, 25);
            tb_cfpsw.TabIndex = 3;
            // 
            // tb_birth
            // 
            tb_birth.Font = new Font("Segoe UI", 10F);
            tb_birth.Location = new Point(26, 360);
            tb_birth.Name = "tb_birth";
            tb_birth.Size = new Size(390, 25);
            tb_birth.TabIndex = 5;
            // 
            // tb_psw
            // 
            tb_psw.Font = new Font("Segoe UI", 10F);
            tb_psw.Location = new Point(26, 213);
            tb_psw.Name = "tb_psw";
            tb_psw.Size = new Size(390, 25);
            tb_psw.TabIndex = 2;
            // 
            // tb_email
            // 
            tb_email.Font = new Font("Segoe UI", 10F);
            tb_email.Location = new Point(26, 310);
            tb_email.Name = "tb_email";
            tb_email.Size = new Size(390, 25);
            tb_email.TabIndex = 4;
            // 
            // btn_signup
            // 
            btn_signup.Cursor = Cursors.Hand;
            btn_signup.Location = new Point(181, 400);
            btn_signup.Name = "btn_signup";
            btn_signup.Size = new Size(75, 23);
            btn_signup.TabIndex = 7;
            btn_signup.Text = "Sign Up";
            btn_signup.UseVisualStyleBackColor = true;
            btn_signup.Click += btn_signup_Click;
            // 
            // lb_signin
            // 
            lb_signin.AutoSize = true;
            lb_signin.Location = new Point(114, 430);
            lb_signin.Name = "lb_signin";
            lb_signin.Size = new Size(142, 15);
            lb_signin.TabIndex = 7;
            lb_signin.Text = "Already have an account?";
            // 
            // linkedlb_signin
            // 
            linkedlb_signin.AutoSize = true;
            linkedlb_signin.Cursor = Cursors.Hand;
            linkedlb_signin.Location = new Point(269, 430);
            linkedlb_signin.Name = "linkedlb_signin";
            linkedlb_signin.Size = new Size(43, 15);
            linkedlb_signin.TabIndex = 8;
            linkedlb_signin.TabStop = true;
            linkedlb_signin.Text = "Sign In";
            linkedlb_signin.LinkClicked += linkedlb_signin_LinkClicked;
            // 
            // cb_showpsw
            // 
            cb_showpsw.AutoSize = true;
            cb_showpsw.Cursor = Cursors.Hand;
            cb_showpsw.Location = new Point(298, 194);
            cb_showpsw.Name = "cb_showpsw";
            cb_showpsw.Size = new Size(108, 19);
            cb_showpsw.TabIndex = 9;
            cb_showpsw.Text = "Show password";
            cb_showpsw.UseVisualStyleBackColor = true;
            cb_showpsw.CheckedChanged += cb_showpsw_CheckedChanged;
            // 
            // cb_showcfpsw
            // 
            cb_showcfpsw.AutoSize = true;
            cb_showcfpsw.Cursor = Cursors.Hand;
            cb_showcfpsw.Location = new Point(298, 244);
            cb_showcfpsw.Name = "cb_showcfpsw";
            cb_showcfpsw.Size = new Size(108, 19);
            cb_showcfpsw.TabIndex = 10;
            cb_showcfpsw.Text = "Show password";
            cb_showcfpsw.UseVisualStyleBackColor = true;
            cb_showcfpsw.CheckedChanged += cb_showcfpsw_CheckedChanged;
            // 
            // lblSignUp
            // 
            lblSignUp.AutoSize = true;
            lblSignUp.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblSignUp.Location = new Point(165, 101);
            lblSignUp.Name = "lblSignUp";
            lblSignUp.Size = new Size(103, 32);
            lblSignUp.TabIndex = 11;
            lblSignUp.Text = "Sign Up";
            // 
            // lblUsernameSU
            // 
            lblUsernameSU.AutoSize = true;
            lblUsernameSU.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblUsernameSU.Location = new Point(26, 140);
            lblUsernameSU.Name = "lblUsernameSU";
            lblUsernameSU.Size = new Size(76, 19);
            lblUsernameSU.TabIndex = 12;
            lblUsernameSU.Text = "Username";
            // 
            // lblPasswordSU
            // 
            lblPasswordSU.AutoSize = true;
            lblPasswordSU.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPasswordSU.Location = new Point(26, 193);
            lblPasswordSU.Name = "lblPasswordSU";
            lblPasswordSU.Size = new Size(73, 19);
            lblPasswordSU.TabIndex = 13;
            lblPasswordSU.Text = "Password";
            // 
            // lblCFPassword
            // 
            lblCFPassword.AutoSize = true;
            lblCFPassword.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblCFPassword.Location = new Point(26, 244);
            lblCFPassword.Name = "lblCFPassword";
            lblCFPassword.Size = new Size(131, 19);
            lblCFPassword.TabIndex = 14;
            lblCFPassword.Text = "Confirm Password";
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblEmail.Location = new Point(26, 290);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(45, 19);
            lblEmail.TabIndex = 15;
            lblEmail.Text = "Email";
            // 
            // lblBirth
            // 
            lblBirth.AutoSize = true;
            lblBirth.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblBirth.Location = new Point(26, 340);
            lblBirth.Name = "lblBirth";
            lblBirth.Size = new Size(41, 19);
            lblBirth.TabIndex = 16;
            lblBirth.Text = "Birth";
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = Properties.Resources.CaroPicGame;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.BorderStyle = BorderStyle.Fixed3D;
            pictureBox1.Location = new Point(168, 9);
            pictureBox1.Margin = new Padding(3, 2, 3, 2);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(106, 91);
            pictureBox1.TabIndex = 17;
            pictureBox1.TabStop = false;
            // 
            // SignUp
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightBlue;
            ClientSize = new Size(442, 448);
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
    }
}