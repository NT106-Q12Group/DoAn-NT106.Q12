namespace CaroGame_TCPClient
{
    partial class Dashboard
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
            lblFullNameUInfo = new Label();
            tb_fullname = new TextBox();
            tb_password = new TextBox();
            lblPasswordUinfo = new Label();
            cb_showcfpswUInfo = new CheckBox();
            btnEditFullName = new Button();
            btnEditUsername = new Button();
            btnEditPassword = new Button();
            btnEditEmail = new Button();
            btnEditBirth = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // tb_username
            // 
            tb_username.BorderStyle = BorderStyle.FixedSingle;
            tb_username.Font = new Font("Segoe UI", 10F);
            tb_username.Location = new Point(27, 308);
            tb_username.Margin = new Padding(3, 4, 3, 4);
            tb_username.Name = "tb_username";
            tb_username.ReadOnly = true;
            tb_username.Size = new Size(382, 30);
            tb_username.TabIndex = 0;
            // 
            // tb_email
            // 
            tb_email.BorderStyle = BorderStyle.FixedSingle;
            tb_email.Font = new Font("Segoe UI", 10F);
            tb_email.Location = new Point(27, 448);
            tb_email.Margin = new Padding(3, 4, 3, 4);
            tb_email.Name = "tb_email";
            tb_email.ReadOnly = true;
            tb_email.Size = new Size(382, 30);
            tb_email.TabIndex = 4;
            // 
            // tb_birthdate
            // 
            tb_birthdate.BorderStyle = BorderStyle.FixedSingle;
            tb_birthdate.Font = new Font("Segoe UI", 10F);
            tb_birthdate.Location = new Point(27, 518);
            tb_birthdate.Margin = new Padding(3, 4, 3, 4);
            tb_birthdate.Name = "tb_birthdate";
            tb_birthdate.ReadOnly = true;
            tb_birthdate.Size = new Size(382, 30);
            tb_birthdate.TabIndex = 5;
            // 
            // btn_signout
            // 
            btn_signout.Cursor = Cursors.Hand;
            btn_signout.Location = new Point(187, 568);
            btn_signout.Margin = new Padding(3, 4, 3, 4);
            btn_signout.Name = "btn_signout";
            btn_signout.Size = new Size(86, 31);
            btn_signout.TabIndex = 6;
            btn_signout.Text = "Sign Out";
            btn_signout.UseVisualStyleBackColor = true;
            btn_signout.Click += btn_signout_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = Properties.Resources.accountCaro1;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.BorderStyle = BorderStyle.Fixed3D;
            pictureBox1.Location = new Point(164, 52);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(142, 150);
            pictureBox1.TabIndex = 7;
            pictureBox1.TabStop = false;
            // 
            // lblUsernameUInfo
            // 
            lblUsernameUInfo.AutoSize = true;
            lblUsernameUInfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblUsernameUInfo.Location = new Point(27, 281);
            lblUsernameUInfo.Name = "lblUsernameUInfo";
            lblUsernameUInfo.Size = new Size(89, 23);
            lblUsernameUInfo.TabIndex = 8;
            lblUsernameUInfo.Text = "Username";
            // 
            // lblEmailUInfo
            // 
            lblEmailUInfo.AutoSize = true;
            lblEmailUInfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblEmailUInfo.Location = new Point(27, 421);
            lblEmailUInfo.Name = "lblEmailUInfo";
            lblEmailUInfo.Size = new Size(54, 23);
            lblEmailUInfo.TabIndex = 9;
            lblEmailUInfo.Text = "Email";
            // 
            // lblBirthUinfo
            // 
            lblBirthUinfo.AutoSize = true;
            lblBirthUinfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblBirthUinfo.Location = new Point(27, 491);
            lblBirthUinfo.Name = "lblBirthUinfo";
            lblBirthUinfo.Size = new Size(50, 23);
            lblBirthUinfo.TabIndex = 10;
            lblBirthUinfo.Text = "Birth";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label4.Location = new Point(124, 10);
            label4.Name = "label4";
            label4.Size = new Size(226, 37);
            label4.TabIndex = 11;
            label4.Text = "User Infomation";
            // 
            // lblFullNameUInfo
            // 
            lblFullNameUInfo.AutoSize = true;
            lblFullNameUInfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblFullNameUInfo.Location = new Point(27, 212);
            lblFullNameUInfo.Name = "lblFullNameUInfo";
            lblFullNameUInfo.Size = new Size(91, 23);
            lblFullNameUInfo.TabIndex = 12;
            lblFullNameUInfo.Text = "Full Name";
            // 
            // tb_fullname
            // 
            tb_fullname.BorderStyle = BorderStyle.FixedSingle;
            tb_fullname.Font = new Font("Segoe UI", 10F);
            tb_fullname.Location = new Point(27, 240);
            tb_fullname.Name = "tb_fullname";
            tb_fullname.Size = new Size(382, 30);
            tb_fullname.TabIndex = 13;
            // 
            // tb_password
            // 
            tb_password.BorderStyle = BorderStyle.FixedSingle;
            tb_password.Font = new Font("Segoe UI", 10F);
            tb_password.Location = new Point(27, 379);
            tb_password.Name = "tb_password";
            tb_password.Size = new Size(382, 30);
            tb_password.TabIndex = 14;
            // 
            // lblPasswordUinfo
            // 
            lblPasswordUinfo.AutoSize = true;
            lblPasswordUinfo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPasswordUinfo.Location = new Point(27, 353);
            lblPasswordUinfo.Name = "lblPasswordUinfo";
            lblPasswordUinfo.Size = new Size(85, 23);
            lblPasswordUinfo.TabIndex = 15;
            lblPasswordUinfo.Text = "Password";
            // 
            // cb_showcfpswUInfo
            // 
            cb_showcfpswUInfo.AutoSize = true;
            cb_showcfpswUInfo.Cursor = Cursors.Hand;
            cb_showcfpswUInfo.Location = new Point(275, 352);
            cb_showcfpswUInfo.Name = "cb_showcfpswUInfo";
            cb_showcfpswUInfo.Size = new Size(134, 24);
            cb_showcfpswUInfo.TabIndex = 16;
            cb_showcfpswUInfo.Text = "Show password";
            cb_showcfpswUInfo.UseVisualStyleBackColor = true;
            // 
            // btnEditFullName
            // 
            btnEditFullName.BackColor = Color.Transparent;
            btnEditFullName.BackgroundImage = Properties.Resources.editCaro;
            btnEditFullName.BackgroundImageLayout = ImageLayout.Stretch;
            btnEditFullName.Cursor = Cursors.Hand;
            btnEditFullName.FlatAppearance.BorderSize = 0;
            btnEditFullName.FlatStyle = FlatStyle.Flat;
            btnEditFullName.Location = new Point(418, 238);
            btnEditFullName.Name = "btnEditFullName";
            btnEditFullName.Size = new Size(30, 29);
            btnEditFullName.TabIndex = 17;
            btnEditFullName.UseVisualStyleBackColor = false;
            // 
            // btnEditUsername
            // 
            btnEditUsername.BackgroundImage = Properties.Resources.editCaro;
            btnEditUsername.BackgroundImageLayout = ImageLayout.Stretch;
            btnEditUsername.Cursor = Cursors.Hand;
            btnEditUsername.FlatAppearance.BorderSize = 0;
            btnEditUsername.FlatStyle = FlatStyle.Flat;
            btnEditUsername.Location = new Point(418, 306);
            btnEditUsername.Name = "btnEditUsername";
            btnEditUsername.Size = new Size(30, 29);
            btnEditUsername.TabIndex = 18;
            btnEditUsername.UseVisualStyleBackColor = true;
            // 
            // btnEditPassword
            // 
            btnEditPassword.BackgroundImage = Properties.Resources.editCaro;
            btnEditPassword.BackgroundImageLayout = ImageLayout.Stretch;
            btnEditPassword.Cursor = Cursors.Hand;
            btnEditPassword.FlatAppearance.BorderSize = 0;
            btnEditPassword.FlatStyle = FlatStyle.Flat;
            btnEditPassword.Location = new Point(418, 377);
            btnEditPassword.Name = "btnEditPassword";
            btnEditPassword.Size = new Size(30, 29);
            btnEditPassword.TabIndex = 19;
            btnEditPassword.UseVisualStyleBackColor = true;
            // 
            // btnEditEmail
            // 
            btnEditEmail.BackgroundImage = Properties.Resources.editCaro;
            btnEditEmail.BackgroundImageLayout = ImageLayout.Stretch;
            btnEditEmail.Cursor = Cursors.Hand;
            btnEditEmail.FlatAppearance.BorderSize = 0;
            btnEditEmail.FlatStyle = FlatStyle.Flat;
            btnEditEmail.Location = new Point(418, 446);
            btnEditEmail.Name = "btnEditEmail";
            btnEditEmail.Size = new Size(30, 29);
            btnEditEmail.TabIndex = 20;
            btnEditEmail.UseVisualStyleBackColor = true;
            // 
            // btnEditBirth
            // 
            btnEditBirth.BackgroundImage = Properties.Resources.editCaro;
            btnEditBirth.BackgroundImageLayout = ImageLayout.Stretch;
            btnEditBirth.Cursor = Cursors.Hand;
            btnEditBirth.FlatAppearance.BorderSize = 0;
            btnEditBirth.FlatStyle = FlatStyle.Flat;
            btnEditBirth.Location = new Point(418, 516);
            btnEditBirth.Name = "btnEditBirth";
            btnEditBirth.Size = new Size(30, 29);
            btnEditBirth.TabIndex = 21;
            btnEditBirth.UseVisualStyleBackColor = true;
            // 
            // Dashboard
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightBlue;
            ClientSize = new Size(470, 614);
            Controls.Add(btnEditBirth);
            Controls.Add(btnEditEmail);
            Controls.Add(btnEditPassword);
            Controls.Add(btnEditUsername);
            Controls.Add(btnEditFullName);
            Controls.Add(cb_showcfpswUInfo);
            Controls.Add(lblPasswordUinfo);
            Controls.Add(tb_password);
            Controls.Add(tb_fullname);
            Controls.Add(lblFullNameUInfo);
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
            Name = "Dashboard";
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
        private Label lblFullNameUInfo;
        private TextBox tb_fullname;
        private TextBox tb_password;
        private Label lblPasswordUinfo;
        private CheckBox cb_showcfpswUInfo;
        private Button btnEditFullName;
        private Button btnEditUsername;
        private Button btnEditPassword;
        private Button btnEditEmail;
        private Button btnEditBirth;
    }
}