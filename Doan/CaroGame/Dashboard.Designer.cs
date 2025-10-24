namespace CaroGame
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
            btnOnline = new Button();
            pnlNaviBar = new Panel();
            btnSettings = new Button();
            btnLeaderboard = new Button();
            btnOffline = new Button();
            pnlWorkPlace = new Panel();
            ptbAvaGame = new PictureBox();
            btnStart = new Button();
            pnlNaviBar.SuspendLayout();
            pnlWorkPlace.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptbAvaGame).BeginInit();
            SuspendLayout();
            // 
            // btnOnline
            // 
            btnOnline.BackgroundImage = Properties.Resources.Caro_Game__10_;
            btnOnline.BackgroundImageLayout = ImageLayout.Stretch;
            btnOnline.Location = new Point(24, 3);
            btnOnline.Name = "btnOnline";
            btnOnline.Size = new Size(67, 50);
            btnOnline.TabIndex = 1;
            btnOnline.UseVisualStyleBackColor = true;
            // 
            // pnlNaviBar
            // 
            pnlNaviBar.Controls.Add(btnSettings);
            pnlNaviBar.Controls.Add(btnLeaderboard);
            pnlNaviBar.Controls.Add(btnOffline);
            pnlNaviBar.Controls.Add(btnOnline);
            pnlNaviBar.Location = new Point(12, 12);
            pnlNaviBar.Name = "pnlNaviBar";
            pnlNaviBar.Size = new Size(117, 223);
            pnlNaviBar.TabIndex = 3;
            // 
            // btnSettings
            // 
            btnSettings.BackgroundImage = Properties.Resources.Caro_Game__8_;
            btnSettings.BackgroundImageLayout = ImageLayout.Stretch;
            btnSettings.Location = new Point(24, 171);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(67, 50);
            btnSettings.TabIndex = 4;
            btnSettings.UseVisualStyleBackColor = true;
            // 
            // btnLeaderboard
            // 
            btnLeaderboard.BackgroundImage = Properties.Resources.Caro_Game__9_;
            btnLeaderboard.BackgroundImageLayout = ImageLayout.Stretch;
            btnLeaderboard.Location = new Point(24, 115);
            btnLeaderboard.Name = "btnLeaderboard";
            btnLeaderboard.Size = new Size(67, 50);
            btnLeaderboard.TabIndex = 3;
            btnLeaderboard.UseVisualStyleBackColor = true;
            // 
            // btnOffline
            // 
            btnOffline.BackgroundImage = Properties.Resources.Caro_Game__11_;
            btnOffline.BackgroundImageLayout = ImageLayout.Stretch;
            btnOffline.Location = new Point(24, 59);
            btnOffline.Name = "btnOffline";
            btnOffline.Size = new Size(67, 50);
            btnOffline.TabIndex = 2;
            btnOffline.UseVisualStyleBackColor = true;
            // 
            // pnlWorkPlace
            // 
            pnlWorkPlace.Controls.Add(ptbAvaGame);
            pnlWorkPlace.Controls.Add(btnStart);
            pnlWorkPlace.Location = new Point(135, 12);
            pnlWorkPlace.Name = "pnlWorkPlace";
            pnlWorkPlace.Size = new Size(523, 223);
            pnlWorkPlace.TabIndex = 4;
            // 
            // ptbAvaGame
            // 
            ptbAvaGame.BackgroundImage = Properties.Resources.Caro_Game__12_;
            ptbAvaGame.BackgroundImageLayout = ImageLayout.Zoom;
            ptbAvaGame.Location = new Point(166, 25);
            ptbAvaGame.Name = "ptbAvaGame";
            ptbAvaGame.Size = new Size(196, 130);
            ptbAvaGame.TabIndex = 1;
            ptbAvaGame.TabStop = false;
            // 
            // btnStart
            // 
            btnStart.BackColor = Color.Lime;
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            btnStart.Location = new Point(186, 161);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(159, 56);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = false;
            // 
            // Dashboard
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DodgerBlue;
            ClientSize = new Size(670, 248);
            Controls.Add(pnlWorkPlace);
            Controls.Add(pnlNaviBar);
            Name = "Dashboard";
            Text = "Dashboard";
            pnlNaviBar.ResumeLayout(false);
            pnlWorkPlace.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)ptbAvaGame).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Button btnOnline;
        private Panel pnlNaviBar;
        private Panel pnlWorkPlace;
        private Button btnSettings;
        private Button btnLeaderboard;
        private Button btnOffline;
        private PictureBox ptbAvaGame;
        private Button btnStart;
    }
}