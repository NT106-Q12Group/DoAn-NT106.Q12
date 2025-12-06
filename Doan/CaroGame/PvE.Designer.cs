namespace CaroGame
{
    partial class PvE
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
            pnlChessBoard = new Panel();
            pnlInfo = new Panel();
            ptbAvaBot = new PictureBox();
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
            panelChat = new Panel();
            label4 = new Label();
            btnSend = new Button();
            txtMessage = new TextBox();
            rtbChat = new RichTextBox();
            pnlInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptbAvaBot).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptbAvaP1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptbO).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptbX).BeginInit();
            panelChat.SuspendLayout();
            SuspendLayout();
            // 
            // pnlChessBoard
            // 
            pnlChessBoard.BackColor = SystemColors.ControlLightLight;
            pnlChessBoard.Cursor = Cursors.Hand;
            pnlChessBoard.ForeColor = Color.Black;
            pnlChessBoard.Location = new Point(12, 12);
            pnlChessBoard.Name = "pnlChessBoard";
            pnlChessBoard.Size = new Size(601, 601);
            pnlChessBoard.TabIndex = 1;
            pnlChessBoard.Paint += pnlChessBoard_Paint;
            // 
            // pnlInfo
            // 
            pnlInfo.Anchor = AnchorStyles.Left;
            pnlInfo.BackColor = SystemColors.ButtonHighlight;
            pnlInfo.Controls.Add(ptbAvaBot);
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
            pnlInfo.Size = new Size(450, 311);
            pnlInfo.TabIndex = 2;
            // 
            // ptbAvaBot
            // 
            ptbAvaBot.Anchor = AnchorStyles.Top;
            ptbAvaBot.BackgroundImage = Properties.Resources.bot__3_;
            ptbAvaBot.BackgroundImageLayout = ImageLayout.Stretch;
            ptbAvaBot.BorderStyle = BorderStyle.Fixed3D;
            ptbAvaBot.Location = new Point(336, 126);
            ptbAvaBot.Name = "ptbAvaBot";
            ptbAvaBot.Size = new Size(104, 101);
            ptbAvaBot.TabIndex = 10;
            ptbAvaBot.TabStop = false;
            // 
            // ptbAvaP1
            // 
            ptbAvaP1.Anchor = AnchorStyles.Top;
            ptbAvaP1.BackgroundImage = Properties.Resources.accountCaro;
            ptbAvaP1.BackgroundImageLayout = ImageLayout.Stretch;
            ptbAvaP1.BorderStyle = BorderStyle.Fixed3D;
            ptbAvaP1.Location = new Point(9, 11);
            ptbAvaP1.Name = "ptbAvaP1";
            ptbAvaP1.Size = new Size(106, 99);
            ptbAvaP1.TabIndex = 9;
            ptbAvaP1.TabStop = false;
            // 
            // btnExit
            // 
            btnExit.Anchor = AnchorStyles.Top;
            btnExit.BackgroundImage = Properties.Resources.Caro_Game__7_;
            btnExit.BackgroundImageLayout = ImageLayout.Stretch;
            btnExit.Cursor = Cursors.Hand;
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.FlatAppearance.MouseDownBackColor = SystemColors.ButtonHighlight;
            btnExit.FlatAppearance.MouseOverBackColor = SystemColors.ButtonHighlight;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.Location = new Point(347, 243);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(65, 51);
            btnExit.TabIndex = 8;
            btnExit.UseVisualStyleBackColor = true;
            // 
            // btnChat
            // 
            btnChat.Anchor = AnchorStyles.Top;
            btnChat.BackgroundImage = Properties.Resources.Caro_Game__6_;
            btnChat.BackgroundImageLayout = ImageLayout.Stretch;
            btnChat.Cursor = Cursors.Hand;
            btnChat.FlatAppearance.BorderColor = SystemColors.ActiveCaptionText;
            btnChat.FlatAppearance.BorderSize = 0;
            btnChat.FlatAppearance.MouseDownBackColor = SystemColors.ButtonHighlight;
            btnChat.FlatAppearance.MouseOverBackColor = SystemColors.ButtonHighlight;
            btnChat.FlatStyle = FlatStyle.Flat;
            btnChat.Location = new Point(246, 243);
            btnChat.Name = "btnChat";
            btnChat.Size = new Size(65, 51);
            btnChat.TabIndex = 7;
            btnChat.UseVisualStyleBackColor = true;
            // 
            // btnUndo
            // 
            btnUndo.Anchor = AnchorStyles.Top;
            btnUndo.BackColor = SystemColors.ButtonHighlight;
            btnUndo.BackgroundImage = Properties.Resources.Caro_Game__5_;
            btnUndo.BackgroundImageLayout = ImageLayout.Stretch;
            btnUndo.Cursor = Cursors.Hand;
            btnUndo.FlatAppearance.BorderSize = 0;
            btnUndo.FlatAppearance.MouseDownBackColor = SystemColors.ButtonHighlight;
            btnUndo.FlatAppearance.MouseOverBackColor = SystemColors.ButtonHighlight;
            btnUndo.FlatStyle = FlatStyle.Flat;
            btnUndo.Location = new Point(143, 243);
            btnUndo.Name = "btnUndo";
            btnUndo.Size = new Size(65, 51);
            btnUndo.TabIndex = 6;
            btnUndo.UseVisualStyleBackColor = false;
            // 
            // btnMenu
            // 
            btnMenu.Anchor = AnchorStyles.Top;
            btnMenu.BackgroundImage = Properties.Resources.menuCaro;
            btnMenu.BackgroundImageLayout = ImageLayout.Stretch;
            btnMenu.Cursor = Cursors.Hand;
            btnMenu.FlatAppearance.BorderColor = SystemColors.ButtonHighlight;
            btnMenu.FlatAppearance.BorderSize = 0;
            btnMenu.FlatAppearance.MouseDownBackColor = SystemColors.ButtonHighlight;
            btnMenu.FlatAppearance.MouseOverBackColor = SystemColors.ButtonHighlight;
            btnMenu.FlatStyle = FlatStyle.Flat;
            btnMenu.Location = new Point(41, 243);
            btnMenu.Name = "btnMenu";
            btnMenu.Size = new Size(66, 51);
            btnMenu.TabIndex = 5;
            btnMenu.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top;
            label2.AutoSize = true;
            label2.Font = new Font("Showcard Gothic", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(207, 126);
            label2.Name = "label2";
            label2.Size = new Size(123, 26);
            label2.TabIndex = 4;
            label2.Text = "SuperBot";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.Font = new Font("Showcard Gothic", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(121, 11);
            label1.Name = "label1";
            label1.Size = new Size(107, 26);
            label1.TabIndex = 2;
            label1.Text = "Player 1";
            // 
            // pgbP2
            // 
            pgbP2.Anchor = AnchorStyles.Top;
            pgbP2.Location = new Point(9, 198);
            pgbP2.Name = "pgbP2";
            pgbP2.Size = new Size(321, 29);
            pgbP2.TabIndex = 3;
            // 
            // ptbO
            // 
            ptbO.Anchor = AnchorStyles.Top;
            ptbO.BackgroundImage = Properties.Resources.Caro_Game__1_;
            ptbO.BackgroundImageLayout = ImageLayout.Stretch;
            ptbO.Location = new Point(9, 128);
            ptbO.Name = "ptbO";
            ptbO.Size = new Size(89, 64);
            ptbO.TabIndex = 2;
            ptbO.TabStop = false;
            // 
            // pgbP1
            // 
            pgbP1.Anchor = AnchorStyles.Top;
            pgbP1.Location = new Point(121, 81);
            pgbP1.Name = "pgbP1";
            pgbP1.Size = new Size(319, 29);
            pgbP1.TabIndex = 1;
            // 
            // ptbX
            // 
            ptbX.Anchor = AnchorStyles.Top;
            ptbX.BackgroundImage = Properties.Resources.Caro_Game;
            ptbX.BackgroundImageLayout = ImageLayout.Stretch;
            ptbX.Location = new Point(351, 11);
            ptbX.Name = "ptbX";
            ptbX.Size = new Size(89, 64);
            ptbX.TabIndex = 0;
            ptbX.TabStop = false;
            // 
            // panelChat
            // 
            panelChat.BackColor = Color.DeepSkyBlue;
            panelChat.Controls.Add(label4);
            panelChat.Controls.Add(btnSend);
            panelChat.Controls.Add(txtMessage);
            panelChat.Controls.Add(rtbChat);
            panelChat.Location = new Point(633, 329);
            panelChat.Name = "panelChat";
            panelChat.Size = new Size(449, 284);
            panelChat.TabIndex = 3;
            panelChat.Visible = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 15F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label4.Location = new Point(189, 2);
            label4.Name = "label4";
            label4.Size = new Size(65, 35);
            label4.TabIndex = 3;
            label4.Text = "Chat";
            // 
            // btnSend
            // 
            btnSend.Location = new Point(335, 236);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(82, 37);
            btnSend.TabIndex = 2;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(30, 236);
            txtMessage.Multiline = true;
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(300, 37);
            txtMessage.TabIndex = 1;
            // 
            // rtbChat
            // 
            rtbChat.Location = new Point(30, 37);
            rtbChat.Name = "rtbChat";
            rtbChat.ReadOnly = true;
            rtbChat.Size = new Size(387, 185);
            rtbChat.TabIndex = 0;
            rtbChat.Text = "";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.HotTrack;
            ClientSize = new Size(1098, 626);
            Controls.Add(panelChat);
            Controls.Add(pnlInfo);
            Controls.Add(pnlChessBoard);
            Name = "PvE";
            Text = "Cờ Caro";
            Load += Form1_Load;
            pnlInfo.ResumeLayout(false);
            pnlInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ptbAvaBot).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptbAvaP1).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptbO).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptbX).EndInit();
            panelChat.ResumeLayout(false);
            panelChat.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlChessBoard;
        private Panel pnlInfo;
        private PictureBox ptbAvaBot;
        private PictureBox ptbAvaP1;
        private Button btnExit;
        private Button btnChat;
        private Button btnUndo;
        private Button btnMenu;
        private Label label2;
        private Label label1;
        private ProgressBar pgbP2;
        private PictureBox ptbO;
        private ProgressBar pgbP1;
        private PictureBox ptbX;
        private Panel panelChat;
        private Label label4;
        private Button btnSend;
        private TextBox txtMessage;
        private RichTextBox rtbChat;
    }
}