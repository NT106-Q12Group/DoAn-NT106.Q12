namespace CaroGame
{
    partial class UIMainGame
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pnlLeaderBoard = new Panel();
            pnlInfo = new Panel();
            ptbAvaP2 = new PictureBox();
            ptbAvaP1 = new PictureBox();
            btnExit = new Button();
            btnChat = new Button();
            btnUndo = new Button();
            btnMenu = new Button();
            label2 = new Label();
            label1 = new Label();
            pgbP2 = new ProgressBar();
            ptbO = new PictureBox();
            pgbP1 = new ProgressBar();
            ptbX = new PictureBox();
            pnlChessBoard = new Panel();
            panelChat = new Panel();
            label4 = new Label();
            btnSend = new Button();
            txtMessage = new TextBox();
            rtbChat = new RichTextBox();
            label3 = new Label();
            pnlInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptbAvaP2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptbAvaP1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptbO).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptbX).BeginInit();
            panelChat.SuspendLayout();
            SuspendLayout();
            // 
            // pnlLeaderBoard
            // 
            pnlLeaderBoard.Anchor = AnchorStyles.Top;
            pnlLeaderBoard.BackColor = SystemColors.ButtonHighlight;
            pnlLeaderBoard.Location = new Point(669, 372);
            pnlLeaderBoard.Name = "pnlLeaderBoard";
            pnlLeaderBoard.Size = new Size(381, 241);
            pnlLeaderBoard.TabIndex = 2;
            // 
            // pnlInfo
            // 
            pnlInfo.Anchor = AnchorStyles.Top;
            pnlInfo.BackColor = SystemColors.ButtonHighlight;
            pnlInfo.Controls.Add(ptbAvaP2);
            pnlInfo.Controls.Add(ptbAvaP1);
            pnlInfo.Controls.Add(btnExit);
            pnlInfo.Controls.Add(btnChat);
            pnlInfo.Controls.Add(btnUndo);
            pnlInfo.Controls.Add(btnMenu);
            pnlInfo.Controls.Add(label2);
            pnlInfo.Controls.Add(label1);
            pnlInfo.Controls.Add(pgbP2);
            pnlInfo.Controls.Add(ptbO);
            pnlInfo.Controls.Add(pgbP1);
            pnlInfo.Controls.Add(ptbX);
            pnlInfo.Location = new Point(634, 12);
            pnlInfo.Name = "pnlInfo";
            pnlInfo.Size = new Size(449, 311);
            pnlInfo.TabIndex = 1;
            // 
            // ptbAvaP2
            // 
            ptbAvaP2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ptbAvaP2.BackgroundImageLayout = ImageLayout.Stretch;
            ptbAvaP2.Location = new Point(351, 127);
            ptbAvaP2.Name = "ptbAvaP2";
            ptbAvaP2.Size = new Size(89, 101);
            ptbAvaP2.TabIndex = 10;
            ptbAvaP2.TabStop = false;
            // 
            // ptbAvaP1
            // 
            ptbAvaP1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ptbAvaP1.BackgroundImageLayout = ImageLayout.Stretch;
            ptbAvaP1.Location = new Point(351, 12);
            ptbAvaP1.Name = "ptbAvaP1";
            ptbAvaP1.Size = new Size(89, 99);
            ptbAvaP1.TabIndex = 9;
            ptbAvaP1.TabStop = false;
            // 
            // btnExit
            // 
            btnExit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExit.BackgroundImage = Properties.Resources.Caro_Game__7_;
            btnExit.BackgroundImageLayout = ImageLayout.Stretch;
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Location = new Point(351, 239);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(81, 63);
            btnExit.TabIndex = 8;
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            // 
            // btnChat
            // 
            btnChat.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnChat.BackgroundImage = Properties.Resources.Caro_Game__6_;
            btnChat.BackgroundImageLayout = ImageLayout.Stretch;
            btnChat.FlatAppearance.BorderSize = 0;
            btnChat.Location = new Point(248, 239);
            btnChat.Name = "btnChat";
            btnChat.Size = new Size(81, 63);
            btnChat.TabIndex = 7;
            btnChat.UseVisualStyleBackColor = true;
            btnChat.Click += btnChat_Click;
            // 
            // btnUndo
            // 
            btnUndo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnUndo.BackColor = SystemColors.ButtonHighlight;
            btnUndo.BackgroundImage = Properties.Resources.Caro_Game__5_;
            btnUndo.BackgroundImageLayout = ImageLayout.Stretch;
            btnUndo.FlatAppearance.BorderSize = 0;
            btnUndo.Location = new Point(145, 239);
            btnUndo.Name = "btnUndo";
            btnUndo.Size = new Size(81, 63);
            btnUndo.TabIndex = 6;
            btnUndo.UseVisualStyleBackColor = false;
            // 
            // btnMenu
            // 
            btnMenu.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMenu.BackgroundImage = Properties.Resources.Caro_Game__4_;
            btnMenu.BackgroundImageLayout = ImageLayout.Stretch;
            btnMenu.FlatAppearance.BorderSize = 0;
            btnMenu.Location = new Point(41, 239);
            btnMenu.Name = "btnMenu";
            btnMenu.Size = new Size(81, 63);
            btnMenu.TabIndex = 5;
            btnMenu.UseVisualStyleBackColor = true;
            btnMenu.Click += btnMenu_Click;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Font = new Font("Showcard Gothic", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(222, 127);
            label2.Name = "label2";
            label2.Size = new Size(108, 26);
            label2.TabIndex = 4;
            label2.Text = "Player 2";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Font = new Font("Showcard Gothic", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(222, 12);
            label1.Name = "label1";
            label1.Size = new Size(107, 26);
            label1.TabIndex = 2;
            label1.Text = "Player 1";
            // 
            // pgbP2
            // 
            pgbP2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pgbP2.Location = new Point(27, 199);
            pgbP2.Name = "pgbP2";
            pgbP2.Size = new Size(303, 29);
            pgbP2.TabIndex = 3;
            // 
            // ptbO
            // 
            ptbO.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ptbO.BackgroundImage = Properties.Resources.Caro_Game__3_;
            ptbO.BackgroundImageLayout = ImageLayout.Stretch;
            ptbO.Location = new Point(41, 129);
            ptbO.Name = "ptbO";
            ptbO.Size = new Size(89, 64);
            ptbO.TabIndex = 2;
            ptbO.TabStop = false;
            // 
            // pgbP1
            // 
            pgbP1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pgbP1.Location = new Point(27, 82);
            pgbP1.Name = "pgbP1";
            pgbP1.Size = new Size(303, 29);
            pgbP1.TabIndex = 1;
            // 
            // ptbX
            // 
            ptbX.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ptbX.BackgroundImage = Properties.Resources.Caro_Game__2_;
            ptbX.BackgroundImageLayout = ImageLayout.Stretch;
            ptbX.Location = new Point(41, 12);
            ptbX.Name = "ptbX";
            ptbX.Size = new Size(89, 64);
            ptbX.TabIndex = 0;
            ptbX.TabStop = false;
            // 
            // pnlChessBoard
            // 
            pnlChessBoard.BackColor = SystemColors.ControlLightLight;
            pnlChessBoard.ForeColor = Color.Black;
            pnlChessBoard.Location = new Point(13, 12);
            pnlChessBoard.Name = "pnlChessBoard";
            pnlChessBoard.Size = new Size(601, 601);
            pnlChessBoard.TabIndex = 0;
            // 
            // panelChat
            // 
            panelChat.BackColor = Color.DeepSkyBlue;
            panelChat.Controls.Add(label4);
            panelChat.Controls.Add(btnSend);
            panelChat.Controls.Add(txtMessage);
            panelChat.Controls.Add(rtbChat);
            panelChat.Location = new Point(634, 329);
            panelChat.Name = "panelChat";
            panelChat.Size = new Size(449, 284);
            panelChat.TabIndex = 1;
            panelChat.Visible = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 15F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(185, 6);
            label4.Name = "label4";
            label4.Size = new Size(65, 35);
            label4.TabIndex = 3;
            label4.Text = "Chat";
            // 
            // btnSend
            // 
            btnSend.Location = new Point(335, 243);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(81, 37);
            btnSend.TabIndex = 2;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(29, 243);
            txtMessage.Multiline = true;
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(301, 37);
            txtMessage.TabIndex = 1;
            // 
            // rtbChat
            // 
            rtbChat.Location = new Point(29, 43);
            rtbChat.Name = "rtbChat";
            rtbChat.ReadOnly = true;
            rtbChat.Size = new Size(387, 185);
            rtbChat.TabIndex = 0;
            rtbChat.Text = "";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top;
            label3.AutoSize = true;
            label3.Font = new Font("Snap ITC", 19.8000011F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.Yellow;
            label3.Location = new Point(736, 326);
            label3.Name = "label3";
            label3.Size = new Size(251, 44);
            label3.TabIndex = 9;
            label3.Text = "Leaderboard";
            // 
            // UIMainGame
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.MidnightBlue;
            ClientSize = new Size(1103, 624);
            Controls.Add(panelChat);
            Controls.Add(label3);
            Controls.Add(pnlLeaderBoard);
            Controls.Add(pnlInfo);
            Controls.Add(pnlChessBoard);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "UIMainGame";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Cờ Caro";
            pnlInfo.ResumeLayout(false);
            pnlInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ptbAvaP2).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptbAvaP1).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptbO).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptbX).EndInit();
            panelChat.ResumeLayout(false);
            panelChat.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Panel pnlLeaderBoard;
        private Panel pnlInfo;
        private PictureBox ptbX;
        private Panel pnlChessBoard;
        private Label label1;
        private ProgressBar pgbP1;
        private ProgressBar pgbP2;
        private PictureBox ptbO;
        private Label label2;
        private Button btnExit;
        private Button btnChat;
        private Button btnUndo;
        private Button btnMenu;
        private Label label3;
        private PictureBox ptbAvaP2;
        private PictureBox ptbAvaP1;
        private Panel panelChat;
        private Label label4;
        private Button btnSend;
        private TextBox txtMessage;
        private RichTextBox rtbChat;
    }
}
