namespace CaroGame
{
    partial class Menu
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
            btn_newgame = new Button();
            btn_diffselect = new Button();
            btn_pause = new Button();
            btn_return = new Button();
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox4 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            SuspendLayout();
            // 
            // btn_newgame
            // 
            btn_newgame.Cursor = Cursors.Hand;
            btn_newgame.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            btn_newgame.Location = new Point(76, 10);
            btn_newgame.Margin = new Padding(2);
            btn_newgame.Name = "btn_newgame";
            btn_newgame.Size = new Size(209, 53);
            btn_newgame.TabIndex = 0;
            btn_newgame.Text = "Game mới";
            btn_newgame.UseVisualStyleBackColor = true;
            btn_newgame.Click += btn_newgame_Click_1;
            // 
            // btn_diffselect
            // 
            btn_diffselect.Cursor = Cursors.Hand;
            btn_diffselect.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            btn_diffselect.Location = new Point(76, 75);
            btn_diffselect.Margin = new Padding(2);
            btn_diffselect.Name = "btn_diffselect";
            btn_diffselect.Size = new Size(209, 53);
            btn_diffselect.TabIndex = 1;
            btn_diffselect.Text = "Chọn chế độ";
            btn_diffselect.UseVisualStyleBackColor = true;
            // 
            // btn_pause
            // 
            btn_pause.Cursor = Cursors.Hand;
            btn_pause.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            btn_pause.Location = new Point(76, 140);
            btn_pause.Margin = new Padding(2);
            btn_pause.Name = "btn_pause";
            btn_pause.Size = new Size(209, 53);
            btn_pause.TabIndex = 2;
            btn_pause.Text = "Tạm dừng";
            btn_pause.UseVisualStyleBackColor = true;
            btn_pause.Click += btn_pause_Click;
            // 
            // btn_return
            // 
            btn_return.Cursor = Cursors.Hand;
            btn_return.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            btn_return.Location = new Point(76, 205);
            btn_return.Margin = new Padding(2);
            btn_return.Name = "btn_return";
            btn_return.Size = new Size(209, 53);
            btn_return.TabIndex = 3;
            btn_return.Text = "Tiếp tục";
            btn_return.UseVisualStyleBackColor = true;
            btn_return.Click += btn_return_Click_1;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = SystemColors.ActiveCaption;
            pictureBox1.BackgroundImage = Properties.Resources.play;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.BorderStyle = BorderStyle.Fixed3D;
            pictureBox1.Location = new Point(14, 205);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(57, 53);
            pictureBox1.TabIndex = 4;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.BackColor = SystemColors.ActiveCaption;
            pictureBox2.BackgroundImage = Properties.Resources.pause;
            pictureBox2.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox2.BorderStyle = BorderStyle.Fixed3D;
            pictureBox2.Location = new Point(14, 140);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(57, 53);
            pictureBox2.TabIndex = 5;
            pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.BackColor = SystemColors.ActiveCaption;
            pictureBox3.BackgroundImage = Properties.Resources.folder;
            pictureBox3.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox3.BorderStyle = BorderStyle.Fixed3D;
            pictureBox3.Location = new Point(14, 10);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(57, 53);
            pictureBox3.TabIndex = 6;
            pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            pictureBox4.BackColor = SystemColors.ActiveCaption;
            pictureBox4.BackgroundImage = Properties.Resources.file;
            pictureBox4.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox4.BorderStyle = BorderStyle.Fixed3D;
            pictureBox4.Location = new Point(14, 75);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(57, 53);
            pictureBox4.TabIndex = 7;
            pictureBox4.TabStop = false;
            // 
            // Menu
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.HotTrack;
            ClientSize = new Size(300, 270);
            Controls.Add(pictureBox4);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            Controls.Add(btn_return);
            Controls.Add(btn_pause);
            Controls.Add(btn_diffselect);
            Controls.Add(btn_newgame);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(2);
            Name = "Menu";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Menu";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button btn_newgame;
        private Button btn_diffselect;
        private Button btn_pause;
        private Button btn_return;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
    }
}