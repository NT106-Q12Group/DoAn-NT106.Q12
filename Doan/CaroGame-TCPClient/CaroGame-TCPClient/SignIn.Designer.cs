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
            SuspendLayout();
            // 
            // tb_username
            // 
            tb_username.Location = new Point(129, 162);
            tb_username.Name = "tb_username";
            tb_username.Size = new Size(183, 23);
            tb_username.TabIndex = 0;
            // 
            // tb_psw
            // 
            tb_psw.Location = new Point(129, 202);
            tb_psw.Name = "tb_psw";
            tb_psw.Size = new Size(183, 23);
            tb_psw.TabIndex = 1;
            // 
            // btn_signin
            // 
            btn_signin.Location = new Point(181, 231);
            btn_signin.Name = "btn_signin";
            btn_signin.Size = new Size(75, 23);
            btn_signin.TabIndex = 2;
            btn_signin.Text = "Sign in";
            btn_signin.UseVisualStyleBackColor = true;
            btn_signin.Click += btn_signin_Click;
            // 
            // cb_showpsw
            // 
            cb_showpsw.AutoSize = true;
            cb_showpsw.Location = new Point(318, 206);
            cb_showpsw.Name = "cb_showpsw";
            cb_showpsw.Size = new Size(108, 19);
            cb_showpsw.TabIndex = 3;
            cb_showpsw.Text = "Show password";
            cb_showpsw.UseVisualStyleBackColor = true;
            cb_showpsw.CheckedChanged += cb_showpsw_CheckedChanged;
            // 
            // lb_signup
            // 
            lb_signup.AutoSize = true;
            lb_signup.Location = new Point(106, 270);
            lb_signup.Name = "lb_signup";
            lb_signup.Size = new Size(150, 15);
            lb_signup.TabIndex = 4;
            lb_signup.Text = "Don’t have an account yet?";
            // 
            // linkedlb_signup
            // 
            linkedlb_signup.AutoSize = true;
            linkedlb_signup.Location = new Point(262, 270);
            linkedlb_signup.Name = "linkedlb_signup";
            linkedlb_signup.Size = new Size(51, 15);
            linkedlb_signup.TabIndex = 5;
            linkedlb_signup.TabStop = true;
            linkedlb_signup.Text = "Sign Up.";
            linkedlb_signup.LinkClicked += linkedlb_signup_LinkClicked;
            // 
            // SignIn
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(linkedlb_signup);
            Controls.Add(lb_signup);
            Controls.Add(cb_showpsw);
            Controls.Add(btn_signin);
            Controls.Add(tb_psw);
            Controls.Add(tb_username);
            Name = "SignIn";
            Text = "SignIn";
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
    }
}