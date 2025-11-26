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
            btnOnline.Location = new Point(30, 4);
            btnOnline.Margin = new Padding(4, 4, 4, 4);
            btnOnline.Name = "btnOnline";
            btnOnline.Size = new Size(84, 62);
            btnOnline.TabIndex = 1;
            btnOnline.UseVisualStyleBackColor = true;
            // 
            // pnlNaviBar
            // 
            pnlNaviBar.Controls.Add(btnSettings);
            pnlNaviBar.Controls.Add(btnLeaderboard);
            pnlNaviBar.Controls.Add(btnOffline);
            pnlNaviBar.Controls.Add(btnOnline);
            pnlNaviBar.Location = new Point(15, 15);
            pnlNaviBar.Margin = new Padding(4, 4, 4, 4);
            pnlNaviBar.Name = "pnlNaviBar";
            pnlNaviBar.Size = new Size(146, 279);
            pnlNaviBar.TabIndex = 3;
            // 
            // btnSettings
            // 
            btnSettings.BackgroundImage = Properties.Resources.Caro_Game__8_;
            btnSettings.BackgroundImageLayout = ImageLayout.Stretch;
            btnSettings.Location = new Point(30, 214);
            btnSettings.Margin = new Padding(4, 4, 4, 4);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(84, 62);
            btnSettings.TabIndex = 4;
            btnSettings.UseVisualStyleBackColor = true;
            // 
            // btnLeaderboard
            // 
            btnLeaderboard.BackgroundImage = Properties.Resources.Caro_Game__9_;
            btnLeaderboard.BackgroundImageLayout = ImageLayout.Stretch;
            btnLeaderboard.Location = new Point(30, 144);
            btnLeaderboard.Margin = new Padding(4, 4, 4, 4);
            btnLeaderboard.Name = "btnLeaderboard";
            btnLeaderboard.Size = new Size(84, 62);
            btnLeaderboard.TabIndex = 3;
            btnLeaderboard.UseVisualStyleBackColor = true;
            // 
            // btnOffline
            // 
            btnOffline.BackgroundImage = Properties.Resources.Caro_Game__11_;
            btnOffline.BackgroundImageLayout = ImageLayout.Stretch;
            btnOffline.Location = new Point(30, 74);
            btnOffline.Margin = new Padding(4, 4, 4, 4);
            btnOffline.Name = "btnOffline";
            btnOffline.Size = new Size(84, 62);
            btnOffline.TabIndex = 2;
            btnOffline.UseVisualStyleBackColor = true;
            // 
            // pnlWorkPlace
            // 
            pnlWorkPlace.Controls.Add(ptbAvaGame);
            pnlWorkPlace.Controls.Add(btnStart);
            pnlWorkPlace.Location = new Point(169, 15);
            pnlWorkPlace.Margin = new Padding(4, 4, 4, 4);
            pnlWorkPlace.Name = "pnlWorkPlace";
            pnlWorkPlace.Size = new Size(654, 279);
            pnlWorkPlace.TabIndex = 4;
            // 
            // ptbAvaGame
            // 
            ptbAvaGame.BackgroundImage = Properties.Resources.Caro_Game__12_;
            ptbAvaGame.BackgroundImageLayout = ImageLayout.Zoom;
            ptbAvaGame.Location = new Point(208, 31);
            ptbAvaGame.Margin = new Padding(4, 4, 4, 4);
            ptbAvaGame.Name = "ptbAvaGame";
            ptbAvaGame.Size = new Size(245, 162);
            ptbAvaGame.TabIndex = 1;
            ptbAvaGame.TabStop = false;
            // 
            // btnStart
            // 
            btnStart.BackColor = Color.Lime;
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            btnStart.Location = new Point(232, 201);
            btnStart.Margin = new Padding(4, 4, 4, 4);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(199, 70);
            btnStart.TabIndex = 0;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += btnStart_Click;
            // 
            // Dashboard
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DodgerBlue;
            ClientSize = new Size(838, 310);
            Controls.Add(pnlWorkPlace);
            Controls.Add(pnlNaviBar);
            Margin = new Padding(4, 4, 4, 4);
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