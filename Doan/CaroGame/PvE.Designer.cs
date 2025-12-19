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
            ptbOne = new PictureBox();
            ptbAvaBot = new PictureBox();
            ptbAvaP1 = new PictureBox();
            btnExit = new Button();
            btnChat = new Button();
            btnNew = new Button();
            label2 = new Label();
            label1 = new Label();
            pgbP2 = new ProgressBar();
            ptbO = new PictureBox();
            pgbP1 = new ProgressBar();
            ptbX = new PictureBox();
            ptbZero = new PictureBox();
            btnUndo = new Button();
            panelChat = new Panel();
            btnSend = new Button();
            txtMessage = new TextBox();
            rtbChat = new RichTextBox();
            pnlInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptbOne).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptbAvaBot).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptbAvaP1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptbO).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptbX).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptbZero).BeginInit();
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
            pnlInfo.BackColor = Color.White;
            pnlInfo.Controls.Add(ptbOne);
            pnlInfo.Controls.Add(ptbAvaBot);
            pnlInfo.Controls.Add(ptbAvaP1);
            pnlInfo.Controls.Add(btnExit);
            pnlInfo.Controls.Add(btnChat);
            pnlInfo.Controls.Add(btnNew);
            pnlInfo.Controls.Add(label2);
            pnlInfo.Controls.Add(label1);
            pnlInfo.Controls.Add(pgbP2);
            pnlInfo.Controls.Add(ptbO);
            pnlInfo.Controls.Add(pgbP1);
            pnlInfo.Controls.Add(ptbX);
            pnlInfo.Controls.Add(ptbZero);
            pnlInfo.Controls.Add(btnUndo);
            pnlInfo.Location = new Point(632, 12);
            pnlInfo.Name = "pnlInfo";
            pnlInfo.Size = new Size(450, 311);
            pnlInfo.TabIndex = 2;
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
            ptbOne.TabIndex = 11;
            ptbOne.TabStop = false;
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
            btnExit.BackColor = Color.Transparent;
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
            btnExit.UseVisualStyleBackColor = false;
            btnExit.Click += btnExit_Click;
            // 
            // btnChat
            // 
            btnChat.Anchor = AnchorStyles.Top;
            btnChat.BackColor = Color.Transparent;
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
            btnChat.UseVisualStyleBackColor = false;
            // 
            // btnNew
            // 
            btnNew.Anchor = AnchorStyles.Top;
            btnNew.BackColor = Color.Transparent;
            btnNew.BackgroundImage = Properties.Resources.reset;
            btnNew.BackgroundImageLayout = ImageLayout.Stretch;
            btnNew.Cursor = Cursors.Hand;
            btnNew.FlatAppearance.BorderColor = SystemColors.ButtonHighlight;
            btnNew.FlatAppearance.BorderSize = 0;
            btnNew.FlatAppearance.MouseDownBackColor = SystemColors.ButtonHighlight;
            btnNew.FlatAppearance.MouseOverBackColor = SystemColors.ButtonHighlight;
            btnNew.FlatStyle = FlatStyle.Flat;
            btnNew.Location = new Point(41, 243);
            btnNew.Name = "btnNew";
            btnNew.Size = new Size(57, 57);
            btnNew.TabIndex = 5;
            btnNew.UseVisualStyleBackColor = false;
            btnNew.Click += btnNew_Click;
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
            // ptbZero
            // 
            ptbZero.BackColor = Color.Transparent;
            ptbZero.BackgroundImage = Properties.Resources.zero;
            ptbZero.BackgroundImageLayout = ImageLayout.Stretch;
            ptbZero.Cursor = Cursors.Hand;
            ptbZero.Location = new Point(189, 239);
            ptbZero.Name = "ptbZero";
            ptbZero.Size = new Size(20, 20);
            ptbZero.TabIndex = 12;
            ptbZero.TabStop = false;
            ptbZero.Visible = false;
            // 
            // btnUndo
            // 
            btnUndo.Anchor = AnchorStyles.Top;
            btnUndo.BackColor = Color.Transparent;
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
            // panelChat
            // 
            panelChat.BackColor = Color.DeepSkyBlue;
            panelChat.BorderStyle = BorderStyle.Fixed3D;
            panelChat.Controls.Add(btnSend);
            panelChat.Controls.Add(txtMessage);
            panelChat.Controls.Add(rtbChat);
            panelChat.Location = new Point(633, 329);
            panelChat.Name = "panelChat";
            panelChat.Size = new Size(449, 284);
            panelChat.TabIndex = 3;
            panelChat.Visible = false;
            // 
            // btnSend
            // 
            btnSend.Cursor = Cursors.Hand;
            btnSend.Location = new Point(335, 228);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(102, 49);
            btnSend.TabIndex = 2;
            btnSend.Text = "Send";
            btnSend.UseVisualStyleBackColor = true;
            // 
            // txtMessage
            // 
            txtMessage.Location = new Point(6, 228);
            txtMessage.Multiline = true;
            txtMessage.Name = "txtMessage";
            txtMessage.Size = new Size(324, 49);
            txtMessage.TabIndex = 1;
            // 
            // rtbChat
            // 
            rtbChat.BackColor = Color.White;
            rtbChat.Location = new Point(6, 16);
            rtbChat.Name = "rtbChat";
            rtbChat.ReadOnly = true;
            rtbChat.Size = new Size(431, 206);
            rtbChat.TabIndex = 0;
            rtbChat.Text = "";
            // 
            // PvE
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.HotTrack;
            ClientSize = new Size(1098, 626);
            Controls.Add(panelChat);
            Controls.Add(pnlInfo);
            Controls.Add(pnlChessBoard);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            Name = "PvE";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Cờ Caro";
            Load += Form1_Load;
            pnlInfo.ResumeLayout(false);
            pnlInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ptbOne).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptbAvaBot).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptbAvaP1).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptbO).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptbX).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptbZero).EndInit();
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
        private Button btnNew;
        private Label label2;
        private Label label1;
        private ProgressBar pgbP2;
        private PictureBox ptbO;
        private ProgressBar pgbP1;
        private PictureBox ptbX;
        private Panel panelChat;
        private Button btnSend;
        private TextBox txtMessage;
        private RichTextBox rtbChat;
        private PictureBox ptbOne;
        private PictureBox ptbZero;
    }
}