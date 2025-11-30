namespace CaroGame_TCPClient
{
    partial class SignIn
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
            tb_psw = new TextBox();
            btn_signin = new Button();
            cb_showpsw = new CheckBox();
            lb_signup = new Label();
            linkedlb_signup = new LinkLabel();
            lblUsernameSI = new Label();
            lblPasswordSI = new Label();
            pictureBox1 = new PictureBox();
            linkLabel1 = new LinkLabel();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // tb_username
            // 
            tb_username.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tb_username.Font = new Font("Segoe UI", 10F);
            tb_username.Location = new Point(23, 216);
            tb_username.Margin = new Padding(3, 4, 3, 4);
            tb_username.Name = "tb_username";
            tb_username.Size = new Size(250, 30);
            tb_username.TabIndex = 0;
            // 
            // tb_psw
            // 
            tb_psw.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tb_psw.Font = new Font("Segoe UI", 10F);
            tb_psw.Location = new Point(23, 289);
            tb_psw.Margin = new Padding(3, 4, 3, 4);
            tb_psw.Name = "tb_psw";
            tb_psw.Size = new Size(250, 30);
            tb_psw.TabIndex = 1;
            // 
            // btn_signin
            // 
            btn_signin.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btn_signin.Cursor = Cursors.Hand;
            btn_signin.Location = new Point(100, 357);
            btn_signin.Margin = new Padding(3, 4, 3, 4);
            btn_signin.Name = "btn_signin";
            btn_signin.Size = new Size(86, 31);
            btn_signin.TabIndex = 2;
            btn_signin.Text = "Sign in";
            btn_signin.UseVisualStyleBackColor = true;
            btn_signin.Click += btn_signin_Click;
            // 
            // cb_showpsw
            // 
            cb_showpsw.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cb_showpsw.AutoSize = true;
            cb_showpsw.Cursor = Cursors.Hand;
            cb_showpsw.Location = new Point(139, 263);
            cb_showpsw.Margin = new Padding(3, 4, 3, 4);
            cb_showpsw.Name = "cb_showpsw";
            cb_showpsw.Size = new Size(134, 24);
            cb_showpsw.TabIndex = 3;
            cb_showpsw.Text = "Show password";
            cb_showpsw.UseVisualStyleBackColor = true;
            cb_showpsw.CheckedChanged += cb_showpsw_CheckedChanged;
            // 
            // lb_signup
            // 
            lb_signup.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lb_signup.AutoSize = true;
            lb_signup.Location = new Point(22, 390);
            lb_signup.Name = "lb_signup";
            lb_signup.Size = new Size(187, 20);
            lb_signup.TabIndex = 4;
            lb_signup.Text = "Don’t have an account yet?";
            // 
            // linkedlb_signup
            // 
            linkedlb_signup.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            linkedlb_signup.AutoSize = true;
            linkedlb_signup.Location = new Point(208, 390);
            linkedlb_signup.Name = "linkedlb_signup";
            linkedlb_signup.Size = new Size(64, 20);
            linkedlb_signup.TabIndex = 5;
            linkedlb_signup.TabStop = true;
            linkedlb_signup.Text = "Sign Up.";
            linkedlb_signup.LinkClicked += linkedlb_signup_LinkClicked;
            // 
            // lblUsernameSI
            // 
            lblUsernameSI.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblUsernameSI.AutoSize = true;
            lblUsernameSI.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblUsernameSI.Location = new Point(23, 189);
            lblUsernameSI.Name = "lblUsernameSI";
            lblUsernameSI.Size = new Size(89, 23);
            lblUsernameSI.TabIndex = 6;
            lblUsernameSI.Text = "Username";
            // 
            // lblPasswordSI
            // 
            lblPasswordSI.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblPasswordSI.AutoSize = true;
            lblPasswordSI.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPasswordSI.Location = new Point(23, 262);
            lblPasswordSI.Name = "lblPasswordSI";
            lblPasswordSI.Size = new Size(85, 23);
            lblPasswordSI.TabIndex = 7;
            lblPasswordSI.Text = "Password";
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = Properties.Resources.CaroPicGame;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Location = new Point(66, 15);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(158, 163);
            pictureBox1.TabIndex = 8;
            pictureBox1.TabStop = false;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(23, 325);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(125, 20);
            linkLabel1.TabIndex = 9;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Forgot Password?";
            // 
            // SignIn
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(295, 424);
            Controls.Add(linkLabel1);
            Controls.Add(pictureBox1);
            Controls.Add(lblPasswordSI);
            Controls.Add(lblUsernameSI);
            Controls.Add(linkedlb_signup);
            Controls.Add(lb_signup);
            Controls.Add(cb_showpsw);
            Controls.Add(btn_signin);
            Controls.Add(tb_psw);
            Controls.Add(tb_username);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            Name = "SignIn";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Sign In";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tb_username;
        private TextBox tb_psw;
        private Button btn_signin;
        private CheckBox cb_showpsw;
        private Label lb_signup;
        private LinkLabel linkedlb_signup;
        private Label lblUsernameSI;
        private Label lblPasswordSI;
        private PictureBox pictureBox1;
        private LinkLabel linkLabel1;
    }
}