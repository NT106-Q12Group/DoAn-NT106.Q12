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
            SuspendLayout();
            // 
            // username_tb
            // 
            username_tb.Location = new Point(157, 90);
            username_tb.Name = "username_tb";
            username_tb.Size = new Size(125, 27);
            username_tb.TabIndex = 0;
            // 
            // OTP_tb
            // 
            OTP_tb.Location = new Point(157, 159);
            OTP_tb.Name = "OTP_tb";
            OTP_tb.Size = new Size(125, 27);
            OTP_tb.TabIndex = 1;
            // 
            // newPassword_tb
            // 
            newPassword_tb.Location = new Point(157, 228);
            newPassword_tb.Name = "newPassword_tb";
            newPassword_tb.Size = new Size(125, 27);
            newPassword_tb.TabIndex = 2;
            // 
            // confirmPassword_tb
            // 
            confirmPassword_tb.Location = new Point(157, 295);
            confirmPassword_tb.Name = "confirmPassword_tb";
            confirmPassword_tb.Size = new Size(125, 27);
            confirmPassword_tb.TabIndex = 3;
            // 
            // sendOtp_btn
            // 
            sendOtp_btn.Location = new Point(323, 93);
            sendOtp_btn.Name = "sendOtp_btn";
            sendOtp_btn.Size = new Size(94, 29);
            sendOtp_btn.TabIndex = 4;
            sendOtp_btn.Text = "Send OTP";
            sendOtp_btn.UseVisualStyleBackColor = true;
            sendOtp_btn.Click += sendOtp_btn_Click;
            // 
            // save_btn
            // 
            save_btn.Location = new Point(132, 377);
            save_btn.Name = "save_btn";
            save_btn.Size = new Size(172, 29);
            save_btn.TabIndex = 5;
            save_btn.Text = "Save";
            save_btn.UseVisualStyleBackColor = true;
            save_btn.Click += save_btn_Click;
            // 
            // confirmOtp_btn
            // 
            confirmOtp_btn.Location = new Point(323, 157);
            confirmOtp_btn.Name = "confirmOtp_btn";
            confirmOtp_btn.Size = new Size(94, 29);
            confirmOtp_btn.TabIndex = 6;
            confirmOtp_btn.Text = "Confirm";
            confirmOtp_btn.UseVisualStyleBackColor = true;
            confirmOtp_btn.Click += confirmOtp_btn_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(157, 20);
            label1.Name = "label1";
            label1.Size = new Size(111, 20);
            label1.TabIndex = 8;
            label1.Text = "Quên Mật Khẩu";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(44, 93);
            label2.Name = "label2";
            label2.Size = new Size(75, 20);
            label2.TabIndex = 9;
            label2.Text = "Username";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(44, 166);
            label3.Name = "label3";
            label3.Size = new Size(37, 20);
            label3.TabIndex = 10;
            label3.Text = "Otp:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(13, 231);
            label4.Name = "label4";
            label4.Size = new Size(106, 20);
            label4.TabIndex = 11;
            label4.Text = "New password";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(13, 298);
            label5.Name = "label5";
            label5.Size = new Size(129, 20);
            label5.TabIndex = 12;
            label5.Text = "Confirm password";
            // 
            // ResetPassword
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(503, 450);
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
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
    }
}