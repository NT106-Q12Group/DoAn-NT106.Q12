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
            SuspendLayout();
            // 
            // tb_username
            // 
            tb_username.Location = new Point(283, 33);
            tb_username.Name = "tb_username";
            tb_username.Size = new Size(196, 23);
            tb_username.TabIndex = 0;
            // 
            // tb_cfpsw
            // 
            tb_cfpsw.Location = new Point(283, 144);
            tb_cfpsw.Name = "tb_cfpsw";
            tb_cfpsw.Size = new Size(196, 23);
            tb_cfpsw.TabIndex = 1;
            // 
            // tb_birth
            // 
            tb_birth.Location = new Point(283, 264);
            tb_birth.Name = "tb_birth";
            tb_birth.Size = new Size(196, 23);
            tb_birth.TabIndex = 3;
            // 
            // tb_psw
            // 
            tb_psw.Location = new Point(283, 87);
            tb_psw.Name = "tb_psw";
            tb_psw.Size = new Size(196, 23);
            tb_psw.TabIndex = 4;
            // 
            // tb_email
            // 
            tb_email.Location = new Point(283, 203);
            tb_email.Name = "tb_email";
            tb_email.Size = new Size(196, 23);
            tb_email.TabIndex = 5;
            // 
            // btn_signup
            // 
            btn_signup.Location = new Point(350, 315);
            btn_signup.Name = "btn_signup";
            btn_signup.Size = new Size(75, 23);
            btn_signup.TabIndex = 6;
            btn_signup.Text = "Sign Up";
            btn_signup.UseVisualStyleBackColor = true;
            btn_signup.Click += btn_signup_Click;
            // 
            // lb_signin
            // 
            lb_signin.AutoSize = true;
            lb_signin.Location = new Point(283, 341);
            lb_signin.Name = "lb_signin";
            lb_signin.Size = new Size(142, 15);
            lb_signin.TabIndex = 7;
            lb_signin.Text = "Already have an account?";
            // 
            // linkedlb_signin
            // 
            linkedlb_signin.AutoSize = true;
            linkedlb_signin.Location = new Point(431, 341);
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
            cb_showpsw.Location = new Point(485, 91);
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
            cb_showcfpsw.Location = new Point(485, 148);
            cb_showcfpsw.Name = "cb_showcfpsw";
            cb_showcfpsw.Size = new Size(108, 19);
            cb_showcfpsw.TabIndex = 10;
            cb_showcfpsw.Text = "Show password";
            cb_showcfpsw.UseVisualStyleBackColor = true;
            cb_showcfpsw.CheckedChanged += cb_showcfpsw_CheckedChanged;
            // 
            // SignUp
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
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
            Name = "SignUp";
            Text = "SignUp";
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
    }
}