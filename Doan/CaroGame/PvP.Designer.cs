namespace CaroGame
{
    partial class PvP
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
            ptbOne = new PictureBox();
            ptbZero = new PictureBox();
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
            rtbChat = new RichTextBox();
            pnlEmojiPicker = new Panel();
            btn_emoji = new Button();
            btnSend = new Button();
            txtMessage = new TextBox();
            label3 = new Label();
            pnlInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptbOne).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptbZero).BeginInit();
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
            pnlLeaderBoard.BorderStyle = BorderStyle.Fixed3D;
            pnlLeaderBoard.Location = new Point(667, 372);
            pnlLeaderBoard.Name = "pnlLeaderBoard";
            pnlLeaderBoard.Size = new Size(381, 241);
            pnlLeaderBoard.TabIndex = 2;
            // 
            // pnlInfo
            // 
            pnlInfo.Anchor = AnchorStyles.Top;
            pnlInfo.BackColor = Color.White;
            pnlInfo.Controls.Add(ptbOne);
            pnlInfo.Controls.Add(ptbZero);
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
            pnlInfo.Location = new Point(632, 12);
            pnlInfo.Name = "pnlInfo";
            pnlInfo.Size = new Size(449, 311);
            pnlInfo.TabIndex = 1;
            // 
            // ptbOne
            // 
            ptbOne.BackColor = Color.Transparent;
            ptbOne.BackgroundImage = Properties.Resources.number_11;
            ptbOne.BackgroundImageLayout = ImageLayout.Stretch;
            ptbOne.Cursor = Cursors.Hand;
            ptbOne.Location = new Point(189, 239);
            ptbOne.Name = "ptbOne";
            ptbOne.Size = new Size(20, 20);
            ptbOne.TabIndex = 13;
            ptbOne.TabStop = false;
            // 
            // ptbZero
            // 
            ptbZero.BackColor = Color.Transparent;
            ptbZero.BackgroundImage = Properties.Resources.zero;
            ptbZero.BackgroundImageLayout = ImageLayout.Stretch;
            ptbZero.Cursor = Cursors.Hand;
            ptbZero.Location = new Point(189, 239);
            ptbZero.Name = "ptbZero";
            ptbZero.Size = new Size(20, 20);
            ptbZero.TabIndex = 14;
            ptbZero.TabStop = false;
            ptbZero.Visible = false;
            // 
            // ptbAvaP2
            // 
            ptbAvaP2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ptbAvaP2.BackgroundImage = Properties.Resources.Songoku;
            ptbAvaP2.BackgroundImageLayout = ImageLayout.Stretch;
            ptbAvaP2.BorderStyle = BorderStyle.Fixed3D;
            ptbAvaP2.Location = new Point(336, 117);
            ptbAvaP2.Name = "ptbAvaP2";
            ptbAvaP2.Size = new Size(104, 101);
            ptbAvaP2.TabIndex = 10;
            ptbAvaP2.TabStop = false;
            // 
            // ptbAvaP1
            // 
            ptbAvaP1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ptbAvaP1.BackgroundImage = Properties.Resources.SovaAva__1_;
            ptbAvaP1.BackgroundImageLayout = ImageLayout.Stretch;
            ptbAvaP1.BorderStyle = BorderStyle.Fixed3D;
            ptbAvaP1.Location = new Point(9, 3);
            ptbAvaP1.Name = "ptbAvaP1";
            ptbAvaP1.Size = new Size(106, 99);
            ptbAvaP1.TabIndex = 9;
            ptbAvaP1.TabStop = false;
            // 
            // btnExit
            // 
            btnExit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExit.BackgroundImage = Properties.Resources.exit;
            btnExit.BackgroundImageLayout = ImageLayout.Stretch;
            btnExit.Cursor = Cursors.Hand;
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.FlatAppearance.MouseDownBackColor = SystemColors.ButtonHighlight;
            btnExit.FlatAppearance.MouseOverBackColor = SystemColors.ButtonHighlight;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.Location = new Point(351, 240);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(67, 67);
            btnExit.TabIndex = 8;
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            // 
            // btnChat
            // 
            btnChat.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnChat.BackgroundImage = Properties.Resources.chat;
            btnChat.BackgroundImageLayout = ImageLayout.Stretch;
            btnChat.Cursor = Cursors.Hand;
            btnChat.FlatAppearance.BorderColor = SystemColors.ActiveCaptionText;
            btnChat.FlatAppearance.BorderSize = 0;
            btnChat.FlatAppearance.MouseDownBackColor = SystemColors.ButtonHighlight;
            btnChat.FlatAppearance.MouseOverBackColor = SystemColors.ButtonHighlight;
            btnChat.FlatStyle = FlatStyle.Flat;
            btnChat.Location = new Point(250, 243);
            btnChat.Name = "btnChat";
            btnChat.Size = new Size(60, 60);
            btnChat.TabIndex = 7;
            btnChat.UseVisualStyleBackColor = true;
            btnChat.Click += btnChat_Click;
            // 
            // btnUndo
            // 
            btnUndo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnUndo.BackColor = SystemColors.ButtonHighlight;
            btnUndo.BackgroundImage = Properties.Resources.undo;
            btnUndo.BackgroundImageLayout = ImageLayout.Stretch;
            btnUndo.Cursor = Cursors.Hand;
            btnUndo.FlatAppearance.BorderSize = 0;
            btnUndo.FlatAppearance.MouseDownBackColor = SystemColors.ButtonHighlight;
            btnUndo.FlatAppearance.MouseOverBackColor = SystemColors.ButtonHighlight;
            btnUndo.FlatStyle = FlatStyle.Flat;
            btnUndo.Location = new Point(137, 232);
            btnUndo.Name = "btnUndo";
            btnUndo.Size = new Size(80, 80);
            btnUndo.TabIndex = 6;
            btnUndo.UseVisualStyleBackColor = false;
            btnUndo.Click += btnUndo_Click;
            // 
            // btnMenu
            // 
            btnMenu.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMenu.BackgroundImage = Properties.Resources.reset;
            btnMenu.BackgroundImageLayout = ImageLayout.Stretch;
            btnMenu.Cursor = Cursors.Hand;
            btnMenu.FlatAppearance.BorderColor = SystemColors.ButtonHighlight;
            btnMenu.FlatAppearance.BorderSize = 0;
            btnMenu.FlatAppearance.MouseDownBackColor = SystemColors.ButtonHighlight;
            btnMenu.FlatAppearance.MouseOverBackColor = SystemColors.ButtonHighlight;
            btnMenu.FlatStyle = FlatStyle.Flat;
            btnMenu.Location = new Point(41, 243);
            btnMenu.Name = "btnMenu";
            btnMenu.Size = new Size(57, 57);
            btnMenu.TabIndex = 5;
            btnMenu.UseVisualStyleBackColor = true;
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
            label1.Location = new Point(121, 12);
            label1.Name = "label1";
            label1.Size = new Size(107, 26);
            label1.TabIndex = 2;
            label1.Text = "Player 1";
            // 
            // pgbP2
            // 
            pgbP2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pgbP2.Location = new Point(9, 199);
            pgbP2.Name = "pgbP2";
            pgbP2.Size = new Size(321, 29);
            pgbP2.TabIndex = 3;
            // 
            // ptbO
            // 
            ptbO.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ptbO.BackgroundImage = Properties.Resources.Caro_Game__1_;
            ptbO.BackgroundImageLayout = ImageLayout.Stretch;
            ptbO.Location = new Point(9, 129);
            ptbO.Name = "ptbO";
            ptbO.Size = new Size(89, 64);
            ptbO.TabIndex = 2;
            ptbO.TabStop = false;
            // 
            // pgbP1
            // 
            pgbP1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pgbP1.Location = new Point(121, 82);
            pgbP1.Name = "pgbP1";
            pgbP1.Size = new Size(319, 29);
            pgbP1.TabIndex = 1;
            // 
            // ptbX
            // 
            ptbX.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ptbX.BackgroundImage = Properties.Resources.Caro_Game;
            ptbX.BackgroundImageLayout = ImageLayout.Stretch;
            ptbX.Location = new Point(351, 12);
            ptbX.Name = "ptbX";
            ptbX.Size = new Size(89, 64);
            ptbX.TabIndex = 0;
            ptbX.TabStop = false;
            // 
            // pnlChessBoard
            // 
            pnlChessBoard.BackColor = SystemColors.ControlLightLight;
            pnlChessBoard.Cursor = Cursors.Hand;
            pnlChessBoard.ForeColor = Color.Black;
            pnlChessBoard.Location = new Point(13, 12);
            pnlChessBoard.Name = "pnlChessBoard";
            pnlChessBoard.Size = new Size(601, 601);
            pnlChessBoard.TabIndex = 0;
            // 
            // panelChat
            // 
            panelChat.BackColor = Color.DeepSkyBlue;
            panelChat.BorderStyle = BorderStyle.Fixed3D;
            panelChat.Controls.Add(rtbChat);
            panelChat.Controls.Add(pnlEmojiPicker);
            panelChat.Controls.Add(btn_emoji);
            panelChat.Controls.Add(btnSend);
            panelChat.Controls.Add(txtMessage);
            panelChat.Location = new Point(631, 329);
            panelChat.Name = "panelChat";
            panelChat.Size = new Size(449, 284);
            panelChat.TabIndex = 1;
            panelChat.Visible = false;
            // 
            // rtbChat
            // 
            rtbChat.BackColor = Color.White;
            rtbChat.Location = new Point(7, 16);
            rtbChat.Name = "rtbChat";
            rtbChat.ReadOnly = true;
            rtbChat.Size = new Size(431, 214);
            rtbChat.TabIndex = 0;
            rtbChat.Text = "";
            // 
            // pnlEmojiPicker
            // 
            pnlEmojiPicker.Location = new Point(19, 94);
            pnlEmojiPicker.Name = "pnlEmojiPicker";
            pnlEmojiPicker.Size = new Size(397, 136);
            pnlEmojiPicker.TabIndex = 5;
            // 
            // btn_emoji
            // 
            btn_emoji.Cursor = Cursors.Hand;
            btn_emoji.Location = new Point(313, 236);
            btn_emoji.Name = "btn_emoji";
            btn_emoji.Size = new Size(38, 39);
            btn_emoji.TabIndex = 4;
            btn_emoji.Text = "😊";
            btn_emoji.UseVisualStyleBackColor = true;
            btn_emoji.Click += btn_emoji_Click;
            // 
            // btnSend
            // 
            btnSend.Cursor = Cursors.Hand;
            btnSend.Location = new Point(357, 236);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(81, 37);
            btnSend.TabIndex = 2;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            btnSend.Click += btnSend_Click;
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(7, 238);
            txtMessage.Multiline = true;
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(300, 37);
            txtMessage.TabIndex = 1;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top;
            label3.AutoSize = true;
            label3.Font = new Font("Snap ITC", 19.8000011F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.Yellow;
            label3.Location = new Point(734, 326);
            label3.Name = "label3";
            label3.Size = new Size(251, 44);
            label3.TabIndex = 9;
            label3.Text = "Leaderboard";
            // 
            // PvP
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.HotTrack;
            ClientSize = new Size(1098, 626);
            Controls.Add(panelChat);
            Controls.Add(label3);
            Controls.Add(pnlLeaderBoard);
            Controls.Add(pnlInfo);
            Controls.Add(pnlChessBoard);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            Name = "PvP";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Cờ Caro";
            pnlInfo.ResumeLayout(false);
            pnlInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ptbOne).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptbZero).EndInit();
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
        private PictureBox ptbAvaP1;
        private Panel panelChat;
        private Button btnSend;
        private TextBox txtMessage;
        private RichTextBox rtbChat;
        private Button btn_emoji;
        private Panel pnlEmojiPicker;
        private PictureBox ptbOne;
        private PictureBox ptbZero;
        private PictureBox ptbAvaP2;
    }
}
