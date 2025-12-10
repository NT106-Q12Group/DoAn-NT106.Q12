namespace CaroGame
{
    partial class ResetPassword
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
            username_tb = new TextBox();
            OTP_tb = new TextBox();
            newPassword_tb = new TextBox();
            confirmPassword_tb = new TextBox();
            sendOtp_btn = new Button();
            save_btn = new Button();
            confirmOtp_btn = new Button();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            checkBox1 = new CheckBox();
            checkBox2 = new CheckBox();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // username_tb
            // 
            username_tb.Font = new Font("Segoe UI", 10F);
            username_tb.Location = new Point(34, 219);
            username_tb.Name = "username_tb";
            username_tb.Size = new Size(246, 30);
            username_tb.TabIndex = 0;
            // 
            // OTP_tb
            // 
            OTP_tb.Font = new Font("Segoe UI", 10F);
            OTP_tb.Location = new Point(34, 286);
            OTP_tb.Name = "OTP_tb";
            OTP_tb.Size = new Size(246, 30);
            OTP_tb.TabIndex = 1;
            // 
            // newPassword_tb
            // 
            newPassword_tb.Font = new Font("Segoe UI", 10F);
            newPassword_tb.Location = new Point(34, 364);
            newPassword_tb.Name = "newPassword_tb";
            newPassword_tb.Size = new Size(346, 30);
            newPassword_tb.TabIndex = 2;
            // 
            // confirmPassword_tb
            // 
            confirmPassword_tb.Font = new Font("Segoe UI", 10F);
            confirmPassword_tb.Location = new Point(34, 433);
            confirmPassword_tb.Name = "confirmPassword_tb";
            confirmPassword_tb.Size = new Size(346, 30);
            confirmPassword_tb.TabIndex = 3;
            // 
            // sendOtp_btn
            // 
            sendOtp_btn.Cursor = Cursors.Hand;
            sendOtp_btn.Font = new Font("Segoe UI", 9F);
            sendOtp_btn.Location = new Point(286, 219);
            sendOtp_btn.Name = "sendOtp_btn";
            sendOtp_btn.Size = new Size(94, 30);
            sendOtp_btn.TabIndex = 4;
            sendOtp_btn.Text = "Send OTP";
            sendOtp_btn.UseVisualStyleBackColor = true;
            sendOtp_btn.Click += sendOtp_btn_Click;
            // 
            // save_btn
            // 
            save_btn.Cursor = Cursors.Hand;
            save_btn.Font = new Font("Segoe UI", 9F);
            save_btn.Location = new Point(139, 488);
            save_btn.Name = "save_btn";
            save_btn.Size = new Size(141, 37);
            save_btn.TabIndex = 5;
            save_btn.Text = "Change password";
            save_btn.UseVisualStyleBackColor = true;
            save_btn.Click += save_btn_Click;
            // 
            // confirmOtp_btn
            // 
            confirmOtp_btn.Cursor = Cursors.Hand;
            confirmOtp_btn.Font = new Font("Segoe UI", 9F);
            confirmOtp_btn.Location = new Point(286, 286);
            confirmOtp_btn.Name = "confirmOtp_btn";
            confirmOtp_btn.Size = new Size(94, 30);
            confirmOtp_btn.TabIndex = 6;
            confirmOtp_btn.Text = "Confirm";
            confirmOtp_btn.UseVisualStyleBackColor = true;
            confirmOtp_btn.Click += confirmOtp_btn_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
            label1.Location = new Point(81, 144);
            label1.Name = "label1";
            label1.Size = new Size(259, 35);
            label1.TabIndex = 8;
            label1.Text = "FORGOT PASSWORD";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label2.Location = new Point(34, 193);
            label2.Name = "label2";
            label2.Size = new Size(89, 23);
            label2.TabIndex = 9;
            label2.Text = "Username";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label3.Location = new Point(34, 260);
            label3.Name = "label3";
            label3.Size = new Size(42, 23);
            label3.TabIndex = 10;
            label3.Text = "OTP";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label4.Location = new Point(34, 335);
            label4.Name = "label4";
            label4.Size = new Size(127, 23);
            label4.TabIndex = 11;
            label4.Text = "New password";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label5.Location = new Point(34, 405);
            label5.Name = "label5";
            label5.Size = new Size(157, 23);
            label5.TabIndex = 12;
            label5.Text = "Confirm password";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Cursor = Cursors.Hand;
            checkBox1.Location = new Point(246, 334);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(134, 24);
            checkBox1.TabIndex = 13;
            checkBox1.Text = "Show password";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Cursor = Cursors.Hand;
            checkBox2.Location = new Point(246, 406);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(134, 24);
            checkBox2.TabIndex = 14;
            checkBox2.Text = "Show password";
            checkBox2.UseVisualStyleBackColor = true;
            checkBox2.CheckedChanged += checkBox2_CheckedChanged;
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = Properties.Resources.CaroPicGame;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.BorderStyle = BorderStyle.Fixed3D;
            pictureBox1.Location = new Point(149, 13);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(121, 120);
            pictureBox1.TabIndex = 18;
            pictureBox1.TabStop = false;
            // 
            // ResetPassword
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightBlue;
            ClientSize = new Size(411, 538);
            Controls.Add(pictureBox1);
            Controls.Add(checkBox2);
            Controls.Add(checkBox1);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(confirmOtp_btn);
            Controls.Add(save_btn);
            Controls.Add(sendOtp_btn);
            Controls.Add(confirmPassword_tb);
            Controls.Add(newPassword_tb);
            Controls.Add(OTP_tb);
            Controls.Add(username_tb);
            FormBorderStyle = FormBorderStyle.None;
            Name = "ResetPassword";
            Text = "Form1";
            Load += ResetPassword_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox username_tb;
        private TextBox OTP_tb;
        private TextBox newPassword_tb;
        private TextBox confirmPassword_tb;
        private Button sendOtp_btn;
        private Button save_btn;
        private Button confirmOtp_btn;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private PictureBox pictureBox1;
    }
}