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
            pnlDashBoard = new Panel();
            btnBack = new Button();
            label5 = new Label();
            label6 = new Label();
            tbRoomID = new TextBox();
            btnJoin = new Button();
            pnlPvPMenu = new Panel();
            btnJoinRoom = new Button();
            btnCreateRoom = new Button();
            btnPlayInstant = new Button();
            btnPvE = new Button();
            btnLeaderboard = new Button();
            btnPvP = new Button();
            label2 = new Label();
            label1 = new Label();
            label4 = new Label();
            btnSettings = new Button();
            label3 = new Label();
            ptbAvaGame = new PictureBox();
            pnlNaviBar.SuspendLayout();
            pnlDashBoard.SuspendLayout();
            pnlPvPMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptbAvaGame).BeginInit();
            SuspendLayout();
            // 
            // pnlNaviBar
            // 
            pnlNaviBar.Controls.Add(pnlDashBoard);
            pnlNaviBar.Controls.Add(pnlPvPMenu);
            pnlNaviBar.Controls.Add(btnPvE);
            pnlNaviBar.Controls.Add(btnLeaderboard);
            pnlNaviBar.Controls.Add(btnPvP);
            pnlNaviBar.Controls.Add(label2);
            pnlNaviBar.Controls.Add(label1);
            pnlNaviBar.Controls.Add(label4);
            pnlNaviBar.Controls.Add(btnSettings);
            pnlNaviBar.Controls.Add(label3);
            pnlNaviBar.Location = new Point(259, 12);
            pnlNaviBar.Name = "pnlNaviBar";
            pnlNaviBar.Size = new Size(391, 241);
            pnlNaviBar.TabIndex = 3;
            pnlNaviBar.Paint += pnlNaviBar_Paint;
            // 
            // pnlDashBoard
            // 
            pnlDashBoard.BackColor = Color.White;
            pnlDashBoard.BorderStyle = BorderStyle.Fixed3D;
            pnlDashBoard.Controls.Add(btnBack);
            pnlDashBoard.Controls.Add(label5);
            pnlDashBoard.Controls.Add(label6);
            pnlDashBoard.Controls.Add(tbRoomID);
            pnlDashBoard.Controls.Add(btnJoin);
            pnlDashBoard.Location = new Point(58, 25);
            pnlDashBoard.Name = "pnlDashBoard";
            pnlDashBoard.Size = new Size(275, 193);
            pnlDashBoard.TabIndex = 10;
            pnlDashBoard.Visible = false;
            // 
            // btnBack
            // 
            btnBack.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnBack.Location = new Point(162, 141);
            btnBack.Name = "btnBack";
            btnBack.Size = new Size(94, 41);
            btnBack.TabIndex = 6;
            btnBack.Text = "Back";
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += btnBack_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label5.Location = new Point(91, 14);
            label5.Name = "label5";
            label5.Size = new Size(93, 37);
            label5.TabIndex = 5;
            label5.Text = "Room";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label6.Location = new Point(19, 60);
            label6.Name = "label6";
            label6.Size = new Size(32, 25);
            label6.TabIndex = 4;
            label6.Text = "ID";
            // 
            // tbRoomID
            // 
            tbRoomID.Font = new Font("Segoe UI", 12F);
            tbRoomID.Location = new Point(19, 88);
            tbRoomID.Name = "tbRoomID";
            tbRoomID.Size = new Size(237, 34);
            tbRoomID.TabIndex = 2;
            // 
            // btnJoin
            // 
            btnJoin.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnJoin.Location = new Point(19, 141);
            btnJoin.Name = "btnJoin";
            btnJoin.Size = new Size(94, 41);
            btnJoin.TabIndex = 2;
            btnJoin.Text = "Join";
            btnJoin.UseVisualStyleBackColor = true;
            btnJoin.Click += btnJoin_Click;
            // 
            // pnlPvPMenu
            // 
            pnlPvPMenu.BackColor = SystemColors.ButtonHighlight;
            pnlPvPMenu.BorderStyle = BorderStyle.Fixed3D;
            pnlPvPMenu.Controls.Add(btnJoinRoom);
            pnlPvPMenu.Controls.Add(btnCreateRoom);
            pnlPvPMenu.Controls.Add(btnPlayInstant);
            pnlPvPMenu.Location = new Point(0, 115);
            pnlPvPMenu.Name = "pnlPvPMenu";
            pnlPvPMenu.Size = new Size(391, 126);
            pnlPvPMenu.TabIndex = 9;
            pnlPvPMenu.Visible = false;
            // 
            // btnJoinRoom
            // 
            btnJoinRoom.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnJoinRoom.Location = new Point(9, 13);
            btnJoinRoom.Name = "btnJoinRoom";
            btnJoinRoom.Size = new Size(152, 48);
            btnJoinRoom.TabIndex = 2;
            btnJoinRoom.Text = "Join Room";
            btnJoinRoom.UseVisualStyleBackColor = true;
            btnJoinRoom.Click += btnJoinRoom_Click;
            // 
            // btnCreateRoom
            // 
            btnCreateRoom.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnCreateRoom.Location = new Point(225, 13);
            btnCreateRoom.Name = "btnCreateRoom";
            btnCreateRoom.Size = new Size(152, 48);
            btnCreateRoom.TabIndex = 1;
            btnCreateRoom.Text = "Create Room";
            btnCreateRoom.UseVisualStyleBackColor = true;
            btnCreateRoom.Click += btnCreateRoom_Click;
            // 
            // btnPlayInstant
            // 
            btnPlayInstant.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnPlayInstant.Location = new Point(119, 67);
            btnPlayInstant.Name = "btnPlayInstant";
            btnPlayInstant.Size = new Size(152, 48);
            btnPlayInstant.TabIndex = 0;
            btnPlayInstant.Text = "Play Instant";
            btnPlayInstant.UseVisualStyleBackColor = true;
            // 
            // btnPvE
            // 
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
            btnPvE.Click += btnPvE_Click;
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
            // btnPvP
            // 
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
            pnlDashBoard.ResumeLayout(false);
            pnlDashBoard.PerformLayout();
            pnlPvPMenu.ResumeLayout(false);
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
        private Panel pnlPvPMenu;
        private Button btnCreateRoom;
        private Button btnPlayInstant;
        private Button btnJoinRoom;
        private Panel pnlDashBoard;
        private Label label5;
        private Label label6;
        private TextBox tbRoomID;
        private Button btnJoin;
        private Button btnBack;
    }
}