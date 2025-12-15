namespace CaroGame
{
    partial class PvPLobby
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
            pictureBox1 = new PictureBox();
            lb_username1 = new Label();
            progressBar1 = new ProgressBar();
            btnBack = new Button();
            pictureBox2 = new PictureBox();
            lb_username2 = new Label();
            lb_status = new Label();
            panel1 = new Panel();
            pictureBox3 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = SystemColors.Control;
            pictureBox1.BackgroundImage = Properties.Resources.SovaAva__1_;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.BorderStyle = BorderStyle.Fixed3D;
            pictureBox1.Location = new Point(41, 62);
            pictureBox1.Margin = new Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(100, 100);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // lb_username1
            // 
            lb_username1.AutoSize = true;
            lb_username1.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lb_username1.Location = new Point(17, 177);
            lb_username1.Name = "lb_username1";
            lb_username1.Size = new Size(169, 37);
            lb_username1.TabIndex = 1;
            lb_username1.Text = "Username 1";
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(238, 187);
            progressBar1.Margin = new Padding(3, 4, 3, 4);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(147, 31);
            progressBar1.TabIndex = 2;
            progressBar1.Click += progressBar1_Click;
            // 
            // btnBack
            // 
            btnBack.Cursor = Cursors.Hand;
            btnBack.Location = new Point(282, 302);
            btnBack.Margin = new Padding(3, 4, 3, 4);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(86, 31);
            btnBack.TabIndex = 3;
            btnBack.Text = "Go Back";
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += btnBack_Click;
            // 
            // pictureBox2
            // 
            pictureBox2.BackColor = SystemColors.Control;
            pictureBox2.BorderStyle = BorderStyle.Fixed3D;
            pictureBox2.Location = new Point(463, 62);
            pictureBox2.Margin = new Padding(3, 4, 3, 4);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(114, 111);
            pictureBox2.TabIndex = 4;
            pictureBox2.TabStop = false;
            // 
            // lb_username2
            // 
            lb_username2.AutoSize = true;
            lb_username2.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lb_username2.Location = new Point(437, 177);
            lb_username2.Name = "lb_username2";
            lb_username2.Size = new Size(169, 37);
            lb_username2.TabIndex = 5;
            lb_username2.Text = "Username 2";
            // 
            // lb_status
            // 
            lb_status.AutoSize = true;
            lb_status.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lb_status.Location = new Point(232, 25);
            lb_status.Name = "lb_status";
            lb_status.Size = new Size(168, 28);
            lb_status.TabIndex = 6;
            lb_status.Text = "Finding Match....";
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Controls.Add(pictureBox3);
            panel1.Controls.Add(pictureBox2);
            panel1.Controls.Add(lb_status);
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(lb_username2);
            panel1.Controls.Add(lb_username1);
            panel1.Controls.Add(progressBar1);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(623, 278);
            panel1.TabIndex = 7;
            // 
            // pictureBox3
            // 
            pictureBox3.BackColor = Color.White;
            pictureBox3.BackgroundImage = Properties.Resources.versus;
            pictureBox3.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox3.Location = new Point(255, 62);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(114, 111);
            pictureBox3.TabIndex = 8;
            pictureBox3.TabStop = false;
            // 
            // PvPLobby
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.HotTrack;
            ClientSize = new Size(647, 346);
            Controls.Add(panel1);
            Controls.Add(btnBack);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(3, 4, 3, 4);
            Name = "PvPLobby";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PvPLobby";
            Load += PvPLobby_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private Label lb_username1;
        private ProgressBar progressBar1;
        private Button btnBack;
        private PictureBox pictureBox2;
        private Label lb_username2;
        private Label lb_status;
        private Panel panel1;
        private PictureBox pictureBox3;
    }
}