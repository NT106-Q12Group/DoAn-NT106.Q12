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
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(232, 212);
            pictureBox1.Margin = new Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(107, 108);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // lb_username1
            // 
            lb_username1.AutoSize = true;
            lb_username1.Location = new Point(245, 324);
            lb_username1.Name = "lb_username1";
            lb_username1.Size = new Size(83, 20);
            lb_username1.TabIndex = 1;
            lb_username1.Text = "Username1";
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(389, 252);
            progressBar1.Margin = new Padding(3, 4, 3, 4);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(131, 31);
            progressBar1.TabIndex = 2;
            progressBar1.Click += progressBar1_Click;
            // 
            // btnBack
            // 
            btnBack.Location = new Point(14, 4);
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
            pictureBox2.Location = new Point(569, 212);
            pictureBox2.Margin = new Padding(3, 4, 3, 4);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(107, 108);
            pictureBox2.TabIndex = 4;
            pictureBox2.TabStop = false;
            // 
            // lb_username2
            // 
            lb_username2.AutoSize = true;
            lb_username2.Location = new Point(586, 324);
            lb_username2.Name = "lb_username2";
            lb_username2.Size = new Size(83, 20);
            lb_username2.TabIndex = 5;
            lb_username2.Text = "Username2";
            // 
            // lb_status
            // 
            lb_status.AutoSize = true;
            lb_status.Location = new Point(400, 228);
            lb_status.Name = "lb_status";
            lb_status.Size = new Size(115, 20);
            lb_status.TabIndex = 6;
            lb_status.Text = "Finding Match....";
            // 
            // PvPLobby
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 600);
            Controls.Add(lb_status);
            Controls.Add(lb_username2);
            Controls.Add(pictureBox2);
            Controls.Add(btnBack);
            Controls.Add(progressBar1);
            Controls.Add(lb_username1);
            Controls.Add(pictureBox1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "PvPLobby";
            Text = "PvPLobby";
            Load += PvPLobby_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Label lb_username1;
        private ProgressBar progressBar1;
        private Button btnBack;
        private PictureBox pictureBox2;
        private Label lb_username2;
        private Label lb_status;
    }
}