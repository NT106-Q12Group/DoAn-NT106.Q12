namespace CaroGame_TCPClient
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
            email_tb = new TextBox();
            SuspendLayout();
            // 
            // username_tb
            // 
            username_tb.Location = new Point(327, 28);
            username_tb.Name = "username_tb";
            username_tb.Size = new Size(125, 27);
            username_tb.TabIndex = 0;
            // 
            // OTP_tb
            // 
            OTP_tb.Location = new Point(327, 144);
            OTP_tb.Name = "OTP_tb";
            OTP_tb.Size = new Size(125, 27);
            OTP_tb.TabIndex = 1;
            // 
            // newPassword_tb
            // 
            newPassword_tb.Location = new Point(327, 212);
            newPassword_tb.Name = "newPassword_tb";
            newPassword_tb.Size = new Size(125, 27);
            newPassword_tb.TabIndex = 2;
            // 
            // confirmPassword_tb
            // 
            confirmPassword_tb.Location = new Point(327, 289);
            confirmPassword_tb.Name = "confirmPassword_tb";
            confirmPassword_tb.Size = new Size(125, 27);
            confirmPassword_tb.TabIndex = 3;
            // 
            // sendOtp_btn
            // 
            sendOtp_btn.Location = new Point(533, 76);
            sendOtp_btn.Name = "sendOtp_btn";
            sendOtp_btn.Size = new Size(94, 29);
            sendOtp_btn.TabIndex = 4;
            sendOtp_btn.Text = "button1";
            sendOtp_btn.UseVisualStyleBackColor = true;
            sendOtp_btn.Click += sendOtp_btn_Click;
            // 
            // save_btn
            // 
            save_btn.Location = new Point(339, 375);
            save_btn.Name = "save_btn";
            save_btn.Size = new Size(94, 29);
            save_btn.TabIndex = 5;
            save_btn.Text = "button2";
            save_btn.UseVisualStyleBackColor = true;
            save_btn.Click += save_btn_Click;
            // 
            // confirmOtp_btn
            // 
            confirmOtp_btn.Location = new Point(533, 144);
            confirmOtp_btn.Name = "confirmOtp_btn";
            confirmOtp_btn.Size = new Size(94, 29);
            confirmOtp_btn.TabIndex = 6;
            confirmOtp_btn.Text = "button1";
            confirmOtp_btn.UseVisualStyleBackColor = true;
            confirmOtp_btn.Click += confirmOtp_btn_Click;
            // 
            // email_tb
            // 
            email_tb.Location = new Point(327, 93);
            email_tb.Name = "email_tb";
            email_tb.Size = new Size(125, 27);
            email_tb.TabIndex = 7;
            // 
            // ResetPassword
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(email_tb);
            Controls.Add(confirmOtp_btn);
            Controls.Add(save_btn);
            Controls.Add(sendOtp_btn);
            Controls.Add(confirmPassword_tb);
            Controls.Add(newPassword_tb);
            Controls.Add(OTP_tb);
            Controls.Add(username_tb);
            Name = "ResetPassword";
            Text = "Form1";
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
        private TextBox email_tb;
    }
}