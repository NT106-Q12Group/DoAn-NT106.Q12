namespace CaroGame
{
    partial class Form1
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
            label3 = new Label();
            pnlInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptbAvaP2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptbAvaP1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptbO).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptbX).BeginInit();
            SuspendLayout();
            // 
            // pnlLeaderBoard
            // 
            pnlLeaderBoard.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pnlLeaderBoard.BackColor = SystemColors.ButtonHighlight;
            pnlLeaderBoard.Location = new Point(834, 465);
            pnlLeaderBoard.Margin = new Padding(4);
            pnlLeaderBoard.Name = "pnlLeaderBoard";
            pnlLeaderBoard.Size = new Size(476, 321);
            pnlLeaderBoard.TabIndex = 2;
            // 
            // pnlInfo
            // 
            pnlInfo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
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
            pnlInfo.Location = new Point(799, 15);
            pnlInfo.Margin = new Padding(4);
            pnlInfo.Name = "pnlInfo";
            pnlInfo.Size = new Size(531, 384);
            pnlInfo.TabIndex = 1;
            // 
            // ptbAvaP2
            // 
            ptbAvaP2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ptbAvaP2.BackgroundImageLayout = ImageLayout.Stretch;
            ptbAvaP2.Location = new Point(399, 159);
            ptbAvaP2.Margin = new Padding(4);
            ptbAvaP2.Name = "ptbAvaP2";
            ptbAvaP2.Size = new Size(112, 126);
            ptbAvaP2.TabIndex = 10;
            ptbAvaP2.TabStop = false;
            // 
            // ptbAvaP1
            // 
            ptbAvaP1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ptbAvaP1.BackgroundImageLayout = ImageLayout.Stretch;
            ptbAvaP1.Location = new Point(21, 15);
            ptbAvaP1.Margin = new Padding(4);
            ptbAvaP1.Name = "ptbAvaP1";
            ptbAvaP1.Size = new Size(112, 124);
            ptbAvaP1.TabIndex = 9;
            ptbAvaP1.TabStop = false;
            // 
            // btnExit
            // 
            btnExit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExit.BackgroundImage = Properties.Resources.Caro_Game__7_;
            btnExit.BackgroundImageLayout = ImageLayout.Stretch;
            btnExit.Location = new Point(409, 292);
            btnExit.Margin = new Padding(4);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(102, 79);
            btnExit.TabIndex = 8;
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            // 
            // btnChat
            // 
            btnChat.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnChat.BackgroundImage = Properties.Resources.Caro_Game__6_;
            btnChat.BackgroundImageLayout = ImageLayout.Stretch;
            btnChat.Location = new Point(280, 292);
            btnChat.Margin = new Padding(4);
            btnChat.Name = "btnChat";
            btnChat.Size = new Size(102, 79);
            btnChat.TabIndex = 7;
            btnChat.UseVisualStyleBackColor = true;
            // 
            // btnUndo
            // 
            btnUndo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnUndo.BackgroundImage = Properties.Resources.Caro_Game__5_;
            btnUndo.BackgroundImageLayout = ImageLayout.Stretch;
            btnUndo.Location = new Point(151, 292);
            btnUndo.Margin = new Padding(4);
            btnUndo.Name = "btnUndo";
            btnUndo.Size = new Size(102, 79);
            btnUndo.TabIndex = 6;
            btnUndo.UseVisualStyleBackColor = true;
            // 
            // btnMenu
            // 
            btnMenu.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnMenu.BackgroundImage = Properties.Resources.Caro_Game__4_;
            btnMenu.BackgroundImageLayout = ImageLayout.Stretch;
            btnMenu.Location = new Point(21, 292);
            btnMenu.Margin = new Padding(4);
            btnMenu.Name = "btnMenu";
            btnMenu.Size = new Size(102, 79);
            btnMenu.TabIndex = 5;
            btnMenu.UseVisualStyleBackColor = true;
            btnMenu.Click += btnMenu_Click;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Font = new Font("Showcard Gothic", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.Location = new Point(248, 159);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(127, 30);
            label2.TabIndex = 4;
            label2.Text = "Player 2";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Font = new Font("Showcard Gothic", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(151, 15);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(126, 30);
            label1.TabIndex = 2;
            label1.Text = "Player 1";
            // 
            // pgbP2
            // 
            pgbP2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pgbP2.Location = new Point(4, 249);
            pgbP2.Margin = new Padding(4);
            pgbP2.Name = "pgbP2";
            pgbP2.Size = new Size(379, 36);
            pgbP2.TabIndex = 3;
            // 
            // ptbO
            // 
            ptbO.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ptbO.BackgroundImage = Properties.Resources.Caro_Game__3_;
            ptbO.BackgroundImageLayout = ImageLayout.Stretch;
            ptbO.Location = new Point(21, 161);
            ptbO.Margin = new Padding(4);
            ptbO.Name = "ptbO";
            ptbO.Size = new Size(112, 80);
            ptbO.TabIndex = 2;
            ptbO.TabStop = false;
            // 
            // pgbP1
            // 
            pgbP1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pgbP1.Location = new Point(151, 102);
            pgbP1.Margin = new Padding(4);
            pgbP1.Name = "pgbP1";
            pgbP1.Size = new Size(404, 36);
            pgbP1.TabIndex = 1;
            // 
            // ptbX
            // 
            ptbX.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ptbX.BackgroundImage = Properties.Resources.Caro_Game__2_;
            ptbX.BackgroundImageLayout = ImageLayout.Stretch;
            ptbX.Location = new Point(399, 15);
            ptbX.Margin = new Padding(4);
            ptbX.Name = "ptbX";
            ptbX.Size = new Size(112, 80);
            ptbX.TabIndex = 0;
            ptbX.TabStop = false;
            // 
            // pnlChessBoard
            // 
            pnlChessBoard.BackColor = SystemColors.ActiveCaptionText;
            pnlChessBoard.Location = new Point(16, 15);
            pnlChessBoard.Margin = new Padding(4);
            pnlChessBoard.Name = "pnlChessBoard";
            pnlChessBoard.Size = new Size(775, 775);
            pnlChessBoard.TabIndex = 0;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Font = new Font("Snap ITC", 19.8000011F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.Yellow;
            label3.Location = new Point(916, 406);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(298, 51);
            label3.TabIndex = 9;
            label3.Text = "Leaderboard";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.MidnightBlue;
            ClientSize = new Size(1355, 802);
            Controls.Add(label3);
            Controls.Add(pnlLeaderBoard);
            Controls.Add(pnlInfo);
            Controls.Add(pnlChessBoard);
            Margin = new Padding(4);
            Name = "Form1";
            Text = "Form1";
            pnlInfo.ResumeLayout(false);
            pnlInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ptbAvaP2).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptbAvaP1).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptbO).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptbX).EndInit();
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
    }
}
