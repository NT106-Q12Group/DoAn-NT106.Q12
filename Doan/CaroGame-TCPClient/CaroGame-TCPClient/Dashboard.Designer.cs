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
            SuspendLayout();
            // 
            // tb_username
            // 
            tb_username.Location = new Point(286, 111);
            tb_username.Name = "tb_username";
            tb_username.ReadOnly = true;
            tb_username.Size = new Size(204, 23);
            tb_username.TabIndex = 0;
            // 
            // tb_email
            // 
            tb_email.Location = new Point(286, 163);
            tb_email.Name = "tb_email";
            tb_email.ReadOnly = true;
            tb_email.Size = new Size(204, 23);
            tb_email.TabIndex = 4;
            // 
            // tb_birthdate
            // 
            tb_birthdate.Location = new Point(286, 216);
            tb_birthdate.Name = "tb_birthdate";
            tb_birthdate.ReadOnly = true;
            tb_birthdate.Size = new Size(204, 23);
            tb_birthdate.TabIndex = 5;
            // 
            // btn_signout
            // 
            btn_signout.Location = new Point(351, 271);
            btn_signout.Name = "btn_signout";
            btn_signout.Size = new Size(75, 23);
            btn_signout.TabIndex = 6;
            btn_signout.Text = "Sign Out";
            btn_signout.UseVisualStyleBackColor = true;
            btn_signout.Click += btn_signout_Click;
            // 
            // Dashboard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(btn_signout);
            Controls.Add(tb_birthdate);
            Controls.Add(tb_email);
            Controls.Add(tb_username);
            Name = "Dashboard";
            Text = "Dashboard";
            Load += Dashboard_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox tb_username;
        private TextBox tb_email;
        private TextBox tb_birthdate;
        private Button btn_signout;
    }
}