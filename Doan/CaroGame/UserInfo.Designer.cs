namespace CaroGame
{
    partial class UserInfo
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
            tb_email = new TextBox();
            tb_birthdate = new TextBox();
            btn_signout = new Button();
            pictureBox1 = new PictureBox();
            lblUsernameUInfo = new Label();
            lblEmailUInfo = new Label();
            lblBirthUinfo = new Label();
            label4 = new Label();
            tb_password = new TextBox();
            lblPasswordUinfo = new Label();
            cb_showcfpswUInfo = new CheckBox();
            btnEditPassword = new Button();
            btnEditEmail = new Button();
            btnEditBirth = new Button();
            btnBack = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // tb_username
            // 
            tb_username.BorderStyle = BorderStyle.FixedSingle;
            tb_username.Enabled = false;
            tb_username.Font = new Font("Segoe UI", 10F);
            tb_username.Location = new Point(27, 247);
            tb_username.Margin = new Padding(3, 4, 3, 4);
            tb_username.Name = "tb_username";
            tb_username.ReadOnly = true;
            tb_username.Size = new Size(382, 30);
            tb_username.TabIndex = 1;
            // 
            // tb_email
            // 
            tb_email.BorderStyle = BorderStyle.FixedSingle;
            tb_email.Enabled = false;
            tb_email.Font = new Font("Segoe UI", 10F);
            tb_email.Location = new Point(27, 387);
            tb_email.Margin = new Padding(3, 4, 3, 4);
            tb_email.Name = "tb_email";
            tb_email.ReadOnly = true;
            tb_email.Size = new Size(382, 30);
            tb_email.TabIndex = 2;
            // 
            // tb_birthdate
            // 
            tb_birthdate.BorderStyle = BorderStyle.FixedSingle;
            tb_birthdate.Enabled = false;
            tb_birthdate.Font = new Font("Segoe UI", 10F);
            tb_birthdate.Location = new Point(27, 457);
            tb_birthdate.Margin = new Padding(3, 4, 3, 4);
            tb_birthdate.Name = "tb_birthdate";
            tb_birthdate.ReadOnly = true;
            tb_birthdate.Size = new Size(382, 30);
            tb_birthdate.TabIndex = 3;
            // 
            // btn_signout
            // 
            btn_signout.Cursor = Cursors.Hand;
            btn_signout.Location = new Point(124, 509);
            btn_signout.Margin = new Padding(3, 4, 3, 4);
            btn_signout.Name = "btn_signout";
            btn_signout.Size = new Size(86, 31);
            btn_signout.TabIndex = 4;
            btn_signout.Text = "Sign Out";
            btn_signout.UseVisualStyleBackColor = true;
            btn_signout.Click += btn_signout_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = Properties.Resources.accountCaro;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.BorderStyle = BorderStyle.Fixed3D;
            pictureBox1.Location = new Point(164, 52);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(142, 150);
            pictureBox1.TabIndex = 5;
            pictureBox1.TabStop = false;
            // 
            // lblUsernameUInfo
            // 
            lblUsernameUInfo.AutoSize = true;
            lblUsernameUInfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblUsernameUInfo.Location = new Point(27, 220);
            lblUsernameUInfo.Name = "lblUsernameUInfo";
            lblUsernameUInfo.Size = new Size(89, 23);
            lblUsernameUInfo.TabIndex = 6;
            lblUsernameUInfo.Text = "Username";
            // 
            // lblEmailUInfo
            // 
            lblEmailUInfo.AutoSize = true;
            lblEmailUInfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblEmailUInfo.Location = new Point(27, 360);
            lblEmailUInfo.Name = "lblEmailUInfo";
            lblEmailUInfo.Size = new Size(54, 23);
            lblEmailUInfo.TabIndex = 7;
            lblEmailUInfo.Text = "Email";
            // 
            // lblBirthUinfo
            // 
            lblBirthUinfo.AutoSize = true;
            lblBirthUinfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblBirthUinfo.Location = new Point(27, 430);
            lblBirthUinfo.Name = "lblBirthUinfo";
            lblBirthUinfo.Size = new Size(50, 23);
            lblBirthUinfo.TabIndex = 8;
            lblBirthUinfo.Text = "Birth";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label4.Location = new Point(124, 10);
            label4.Name = "label4";
            label4.Size = new Size(226, 37);
            label4.TabIndex = 9;
            label4.Text = "User Infomation";
            // 
            // tb_password
            // 
            tb_password.BorderStyle = BorderStyle.FixedSingle;
            tb_password.Enabled = false;
            tb_password.Font = new Font("Segoe UI", 10F);
            tb_password.Location = new Point(27, 318);
            tb_password.Name = "tb_password";
            tb_password.Size = new Size(382, 30);
            tb_password.TabIndex = 11;
            // 
            // lblPasswordUinfo
            // 
            lblPasswordUinfo.AutoSize = true;
            lblPasswordUinfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPasswordUinfo.Location = new Point(27, 292);
            lblPasswordUinfo.Name = "lblPasswordUinfo";
            lblPasswordUinfo.Size = new Size(85, 23);
            lblPasswordUinfo.TabIndex = 12;
            lblPasswordUinfo.Text = "Password";
            // 
            // cb_showcfpswUInfo
            // 
            cb_showcfpswUInfo.AutoSize = true;
            cb_showcfpswUInfo.Cursor = Cursors.Hand;
            cb_showcfpswUInfo.Location = new Point(275, 291);
            cb_showcfpswUInfo.Name = "cb_showcfpswUInfo";
            cb_showcfpswUInfo.Size = new Size(134, 24);
            cb_showcfpswUInfo.TabIndex = 13;
            cb_showcfpswUInfo.Text = "Show password";
            cb_showcfpswUInfo.UseVisualStyleBackColor = true;
            // 
            // btnEditPassword
            // 
            btnEditPassword.BackgroundImage = Properties.Resources.edit;
            btnEditPassword.BackgroundImageLayout = ImageLayout.Stretch;
            btnEditPassword.Cursor = Cursors.Hand;
            btnEditPassword.FlatAppearance.BorderSize = 0;
            btnEditPassword.FlatAppearance.MouseDownBackColor = Color.LightBlue;
            btnEditPassword.FlatAppearance.MouseOverBackColor = Color.LightBlue;
            btnEditPassword.FlatStyle = FlatStyle.Flat;
            btnEditPassword.Location = new Point(418, 316);
            btnEditPassword.Name = "btnEditPassword";
            btnEditPassword.Size = new Size(30, 29);
            btnEditPassword.TabIndex = 16;
            btnEditPassword.UseVisualStyleBackColor = true;
            btnEditPassword.Click += btnEditPassword_Click;
            // 
            // btnEditEmail
            // 
            btnEditEmail.BackgroundImage = Properties.Resources.edit;
            btnEditEmail.BackgroundImageLayout = ImageLayout.Stretch;
            btnEditEmail.Cursor = Cursors.Hand;
            btnEditEmail.FlatAppearance.BorderSize = 0;
            btnEditEmail.FlatAppearance.MouseDownBackColor = Color.LightBlue;
            btnEditEmail.FlatAppearance.MouseOverBackColor = Color.LightBlue;
            btnEditEmail.FlatStyle = FlatStyle.Flat;
            btnEditEmail.Location = new Point(418, 385);
            btnEditEmail.Name = "btnEditEmail";
            btnEditEmail.Size = new Size(30, 29);
            btnEditEmail.TabIndex = 17;
            btnEditEmail.UseVisualStyleBackColor = true;
            // 
            // btnEditBirth
            // 
            btnEditBirth.BackgroundImage = Properties.Resources.edit;
            btnEditBirth.BackgroundImageLayout = ImageLayout.Stretch;
            btnEditBirth.Cursor = Cursors.Hand;
            btnEditBirth.FlatAppearance.BorderSize = 0;
            btnEditBirth.FlatAppearance.MouseDownBackColor = Color.LightBlue;
            btnEditBirth.FlatAppearance.MouseOverBackColor = Color.LightBlue;
            btnEditBirth.FlatStyle = FlatStyle.Flat;
            btnEditBirth.Location = new Point(418, 455);
            btnEditBirth.Name = "btnEditBirth";
            btnEditBirth.Size = new Size(30, 29);
            btnEditBirth.TabIndex = 18;
            btnEditBirth.UseVisualStyleBackColor = true;
            // 
            // btnBack
            // 
            btnBack.Cursor = Cursors.Hand;
            btnBack.Location = new Point(264, 509);
            btnBack.Margin = new Padding(3, 4, 3, 4);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(86, 31);
            btnBack.TabIndex = 19;
            btnBack.Text = "Back";
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += btnBack_Click;
            // 
            // UserInfo
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightBlue;
            ClientSize = new Size(470, 555);
            Controls.Add(btnBack);
            Controls.Add(btnEditBirth);
            Controls.Add(btnEditEmail);
            Controls.Add(btnEditPassword);
            Controls.Add(cb_showcfpswUInfo);
            Controls.Add(lblPasswordUinfo);
            Controls.Add(tb_password);
            Controls.Add(label4);
            Controls.Add(lblBirthUinfo);
            Controls.Add(lblEmailUInfo);
            Controls.Add(lblUsernameUInfo);
            Controls.Add(pictureBox1);
            Controls.Add(btn_signout);
            Controls.Add(tb_birthdate);
            Controls.Add(tb_email);
            Controls.Add(tb_username);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            Name = "UserInfo";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Dashboard";
            Load += Dashboard_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tb_username;
        private TextBox tb_email;
        private TextBox tb_birthdate;
        private Button btn_signout;
        private PictureBox pictureBox1;
        private Label lblUsernameUInfo;
        private Label lblEmailUInfo;
        private Label lblBirthUinfo;
        private Label label4;
        private TextBox tb_password;
        private Label lblPasswordUinfo;
        private CheckBox cb_showcfpswUInfo;
        private Button btnEditPassword;
        private Button btnEditEmail;
        private Button btnEditBirth;
        private Button btnBack;
    }
}