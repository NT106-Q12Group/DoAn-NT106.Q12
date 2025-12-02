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
            SuspendLayout();
            // 
            // btn_newgame
            // 
            btn_newgame.Cursor = Cursors.Hand;
            btn_newgame.Font = new Font("Segoe UI", 13F);
            btn_newgame.Location = new Point(33, 11);
            btn_newgame.Margin = new Padding(2);
            btn_newgame.Name = "btn_newgame";
            btn_newgame.Size = new Size(261, 53);
            btn_newgame.TabIndex = 0;
            btn_newgame.Text = "Game mới";
            btn_newgame.UseVisualStyleBackColor = true;
            btn_newgame.Click += btn_newgame_Click;
            // 
            // btn_diffselect
            // 
            btn_diffselect.Cursor = Cursors.Hand;
            btn_diffselect.Font = new Font("Segoe UI", 13F);
            btn_diffselect.Location = new Point(33, 68);
            btn_diffselect.Margin = new Padding(2);
            btn_diffselect.Name = "btn_diffselect";
            btn_diffselect.Size = new Size(261, 53);
            btn_diffselect.TabIndex = 1;
            btn_diffselect.Text = "Chọn chế độ";
            btn_diffselect.UseVisualStyleBackColor = true;
            // 
            // btn_pause
            // 
            btn_pause.Cursor = Cursors.Hand;
            btn_pause.Font = new Font("Segoe UI", 13F);
            btn_pause.Location = new Point(33, 125);
            btn_pause.Margin = new Padding(2);
            btn_pause.Name = "btn_pause";
            btn_pause.Size = new Size(261, 53);
            btn_pause.TabIndex = 2;
            btn_pause.Text = "Tạm dừng";
            btn_pause.UseVisualStyleBackColor = true;
            btn_pause.Click += btn_pause_Click;
            // 
            // btn_return
            // 
            btn_return.Cursor = Cursors.Hand;
            btn_return.Location = new Point(97, 194);
            btn_return.Margin = new Padding(2);
            btn_return.Name = "btn_return";
            btn_return.Size = new Size(141, 31);
            btn_return.TabIndex = 3;
            btn_return.Text = "Quay lại";
            btn_return.UseVisualStyleBackColor = true;
            btn_return.Click += btn_return_Click;
            // 
            // Menu
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(327, 236);
            Controls.Add(btn_return);
            Controls.Add(btn_pause);
            Controls.Add(btn_diffselect);
            Controls.Add(btn_newgame);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(2);
            Name = "Menu";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Menu";
            ResumeLayout(false);
        }

        #endregion

        private Button btn_newgame;
        private Button btn_diffselect;
        private Button btn_pause;
        private Button btn_return;
    }
}