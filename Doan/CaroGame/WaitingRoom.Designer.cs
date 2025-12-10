namespace CaroGame
{
    partial class WaitingRoom
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
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            panel1 = new Panel();
            ptbReady2 = new PictureBox();
            ptbReady1 = new PictureBox();
            rtbID = new RichTextBox();
            label6 = new Label();
            label2 = new Label();
            btnReady = new Button();
            btnExit = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)ptbReady2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ptbReady1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = Properties.Resources.versus;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Location = new Point(203, 108);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(154, 151);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.BackColor = SystemColors.Control;
            pictureBox2.BorderStyle = BorderStyle.Fixed3D;
            pictureBox2.Location = new Point(400, 51);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(114, 111);
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.BackColor = SystemColors.Control;
            pictureBox3.BorderStyle = BorderStyle.Fixed3D;
            pictureBox3.Location = new Point(46, 51);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(114, 111);
            pictureBox3.TabIndex = 2;
            pictureBox3.TabStop = false;
            // 
            // panel1
            // 
            panel1.BackColor = Color.White;
            panel1.BorderStyle = BorderStyle.Fixed3D;
            panel1.Controls.Add(ptbReady2);
            panel1.Controls.Add(ptbReady1);
            panel1.Controls.Add(rtbID);
            panel1.Controls.Add(label6);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(pictureBox3);
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(pictureBox2);
            panel1.Location = new Point(12, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(561, 284);
            panel1.TabIndex = 3;
            // 
            // ptbReady2
            // 
            ptbReady2.BackColor = Color.Transparent;
            ptbReady2.BackgroundImage = Properties.Resources.check;
            ptbReady2.BackgroundImageLayout = ImageLayout.Stretch;
            ptbReady2.Location = new Point(514, 38);
            ptbReady2.Name = "ptbReady2";
            ptbReady2.Size = new Size(24, 24);
            ptbReady2.TabIndex = 11;
            ptbReady2.TabStop = false;
            ptbReady2.Visible = false;
            // 
            // ptbReady1
            // 
            ptbReady1.BackColor = Color.Transparent;
            ptbReady1.BackgroundImage = Properties.Resources.check;
            ptbReady1.BackgroundImageLayout = ImageLayout.Stretch;
            ptbReady1.Location = new Point(160, 38);
            ptbReady1.Name = "ptbReady1";
            ptbReady1.Size = new Size(24, 24);
            ptbReady1.TabIndex = 10;
            ptbReady1.TabStop = false;
            ptbReady1.Visible = false;
            // 
            // rtbID
            // 
            rtbID.BackColor = Color.White;
            rtbID.Enabled = false;
            rtbID.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            rtbID.Location = new Point(217, 10);
            rtbID.Name = "rtbID";
            rtbID.ReadOnly = true;
            rtbID.ScrollBars = RichTextBoxScrollBars.None;
            rtbID.Size = new Size(125, 34);
            rtbID.TabIndex = 9;
            rtbID.Text = "";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label6.Location = new Point(383, 165);
            label6.Name = "label6";
            label6.Size = new Size(146, 37);
            label6.TabIndex = 8;
            label6.Text = "Username";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label2.Location = new Point(28, 165);
            label2.Name = "label2";
            label2.Size = new Size(146, 37);
            label2.TabIndex = 7;
            label2.Text = "Username";
            // 
            // btnReady
            // 
            btnReady.Cursor = Cursors.Hand;
            btnReady.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            btnReady.Location = new Point(212, 317);
            btnReady.Name = "btnReady";
            btnReady.Size = new Size(167, 51);
            btnReady.TabIndex = 6;
            btnReady.Text = "Ready";
            btnReady.UseVisualStyleBackColor = true;
            btnReady.Click += btnReady_Click;
            // 
            // btnExit
            // 
            btnExit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExit.BackgroundImage = Properties.Resources.Caro_Game__7_;
            btnExit.BackgroundImageLayout = ImageLayout.Stretch;
            btnExit.Cursor = Cursors.Hand;
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.FlatAppearance.MouseDownBackColor = SystemColors.HotTrack;
            btnExit.FlatAppearance.MouseOverBackColor = SystemColors.HotTrack;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.Location = new Point(508, 317);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(65, 51);
            btnExit.TabIndex = 9;
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            // 
            // WaitingRoom
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.HotTrack;
            ClientSize = new Size(585, 384);
            Controls.Add(btnExit);
            Controls.Add(btnReady);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "WaitingRoom";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "WaitingRoom";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)ptbReady2).EndInit();
            ((System.ComponentModel.ISupportInitialize)ptbReady1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private Panel panel1;
        private Label label6;
        private Label label2;
        private RichTextBox rtbID;
        private Button btnReady;
        private Button btnExit;
        private PictureBox ptbReady2;
        private PictureBox ptbReady1;
    }
}