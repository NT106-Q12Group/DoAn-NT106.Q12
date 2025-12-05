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
            pnlNaviBar = new Panel();
            btnPvE = new Button();
            btnLeaderboard = new Button();
            label4 = new Label();
            btnSettings = new Button();
            label3 = new Label();
            btnPvP = new Button();
            label2 = new Label();
            label1 = new Label();
            ptbAvaGame = new PictureBox();
            pnlNaviBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptbAvaGame).BeginInit();
            SuspendLayout();
            // 
            // pnlNaviBar
            // 
            pnlNaviBar.Controls.Add(btnPvE);
            pnlNaviBar.Controls.Add(btnLeaderboard);
            pnlNaviBar.Controls.Add(label4);
            pnlNaviBar.Controls.Add(btnSettings);
            pnlNaviBar.Controls.Add(label3);
            pnlNaviBar.Controls.Add(btnPvP);
            pnlNaviBar.Controls.Add(label2);
            pnlNaviBar.Controls.Add(label1);
            pnlNaviBar.Location = new Point(259, 12);
            pnlNaviBar.Name = "pnlNaviBar";
            pnlNaviBar.Size = new Size(391, 241);
            pnlNaviBar.TabIndex = 3;
            pnlNaviBar.Paint += pnlNaviBar_Paint;
            // 
            // btnPvE
            // 
            btnPvE.BackColor = Color.LightBlue;
            btnPvE.BackgroundImage = Properties.Resources.match;
            btnPvE.BackgroundImageLayout = ImageLayout.Stretch;
            btnPvE.Cursor = Cursors.Hand;
            btnPvE.FlatAppearance.BorderSize = 0;
            btnPvE.FlatAppearance.MouseDownBackColor = Color.LightBlue;
            btnPvE.FlatAppearance.MouseOverBackColor = Color.LightBlue;
            btnPvE.FlatStyle = FlatStyle.Flat;
            btnPvE.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            btnPvE.Location = new Point(70, 16);
            btnPvE.Name = "btnPvE";
            btnPvE.Size = new Size(67, 56);
            btnPvE.TabIndex = 0;
            btnPvE.UseVisualStyleBackColor = false;
            btnPvE.Click += btnStart_Click;
            // 
            // btnLeaderboard
            // 
            btnLeaderboard.BackgroundImage = Properties.Resources.Caro_Game__9_;
            btnLeaderboard.BackgroundImageLayout = ImageLayout.Stretch;
            btnLeaderboard.Cursor = Cursors.Hand;
            btnLeaderboard.FlatAppearance.BorderSize = 0;
            btnLeaderboard.FlatAppearance.MouseDownBackColor = Color.LightBlue;
            btnLeaderboard.FlatAppearance.MouseOverBackColor = Color.LightBlue;
            btnLeaderboard.FlatStyle = FlatStyle.Flat;
            btnLeaderboard.Location = new Point(70, 145);
            btnLeaderboard.Name = "btnLeaderboard";
            btnLeaderboard.Size = new Size(67, 48);
            btnLeaderboard.TabIndex = 3;
            btnLeaderboard.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label4.Location = new Point(257, 196);
            label4.Name = "label4";
            label4.Size = new Size(122, 37);
            label4.TabIndex = 8;
            label4.Text = "Settings";
            // 
            // btnSettings
            // 
            btnSettings.BackgroundImage = Properties.Resources.Caro_Game__8_;
            btnSettings.BackgroundImageLayout = ImageLayout.Stretch;
            btnSettings.Cursor = Cursors.Hand;
            btnSettings.FlatAppearance.BorderSize = 0;
            btnSettings.FlatAppearance.MouseDownBackColor = Color.LightBlue;
            btnSettings.FlatAppearance.MouseOverBackColor = Color.LightBlue;
            btnSettings.FlatStyle = FlatStyle.Flat;
            btnSettings.Location = new Point(279, 145);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(67, 48);
            btnSettings.TabIndex = 4;
            btnSettings.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label3.Location = new Point(11, 196);
            label3.Name = "label3";
            label3.Size = new Size(181, 37);
            label3.TabIndex = 7;
            label3.Text = "Leaderboard";
            // 
            // btnPvP
            // 
            btnPvP.BackColor = Color.LightBlue;
            btnPvP.BackgroundImage = Properties.Resources.pvp;
            btnPvP.BackgroundImageLayout = ImageLayout.Stretch;
            btnPvP.CausesValidation = false;
            btnPvP.Cursor = Cursors.Hand;
            btnPvP.FlatAppearance.BorderSize = 0;
            btnPvP.FlatAppearance.MouseDownBackColor = Color.LightBlue;
            btnPvP.FlatAppearance.MouseOverBackColor = Color.LightBlue;
            btnPvP.FlatStyle = FlatStyle.Flat;
            btnPvP.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            btnPvP.Location = new Point(279, 16);
            btnPvP.Name = "btnPvP";
            btnPvP.Size = new Size(67, 56);
            btnPvP.TabIndex = 4;
            btnPvP.TextAlign = ContentAlignment.MiddleLeft;
            btnPvP.UseVisualStyleBackColor = false;
            btnPvP.Click += btnPvP_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label2.Location = new Point(236, 75);
            label2.Name = "label2";
            label2.Size = new Size(148, 37);
            label2.TabIndex = 6;
            label2.Text = "PvP Mode";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label1.Location = new Point(29, 75);
            label1.Name = "label1";
            label1.Size = new Size(145, 37);
            label1.TabIndex = 5;
            label1.Text = "PvE Mode";
            // 
            // ptbAvaGame
            // 
            ptbAvaGame.BackgroundImage = Properties.Resources.CaroPicGame;
            ptbAvaGame.BackgroundImageLayout = ImageLayout.Zoom;
            ptbAvaGame.BorderStyle = BorderStyle.Fixed3D;
            ptbAvaGame.Location = new Point(12, 12);
            ptbAvaGame.Name = "ptbAvaGame";
            ptbAvaGame.Size = new Size(240, 241);
            ptbAvaGame.TabIndex = 1;
            ptbAvaGame.TabStop = false;
            // 
            // Dashboard
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightBlue;
            ClientSize = new Size(660, 261);
            Controls.Add(ptbAvaGame);
            Controls.Add(pnlNaviBar);
            Font = new Font("Segoe UI", 9F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Dashboard";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Dashboard";
            pnlNaviBar.ResumeLayout(false);
            pnlNaviBar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ptbAvaGame).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Panel pnlNaviBar;
        private Button btnSettings;
        private Button btnLeaderboard;
        private PictureBox ptbAvaGame;
        private Button btnPvE;
        private Button btnPvP;
        private Label label3;
        private Label label2;
        private Label label1;
        private Label label4;
    }
}