namespace CaroGame
{
    partial class Leaderboard
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
            dgv_leaderboard = new DataGridView();
            label3 = new Label();
            ((System.ComponentModel.ISupportInitialize)dgv_leaderboard).BeginInit();
            SuspendLayout();
            // 
            // dgv_leaderboard
            // 
            dgv_leaderboard.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
            dgv_leaderboard.BackgroundColor = Color.LightBlue;
            dgv_leaderboard.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv_leaderboard.Dock = DockStyle.Bottom;
            dgv_leaderboard.Location = new Point(0, 53);
            dgv_leaderboard.Margin = new Padding(2);
            dgv_leaderboard.Name = "dgv_leaderboard";
            dgv_leaderboard.RowHeadersWidth = 62;
            dgv_leaderboard.Size = new Size(640, 307);
            dgv_leaderboard.TabIndex = 0;
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top;
            label3.AutoSize = true;
            label3.Font = new Font("Snap ITC", 19.8000011F, FontStyle.Italic, GraphicsUnit.Point, 0);
            label3.ForeColor = Color.Yellow;
            label3.Location = new Point(191, 7);
            label3.Name = "label3";
            label3.Size = new Size(251, 44);
            label3.TabIndex = 10;
            label3.Text = "Leaderboard";
            // 
            // Leaderboard
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.HotTrack;
            ClientSize = new Size(640, 360);
            Controls.Add(label3);
            Controls.Add(dgv_leaderboard);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Leaderboard";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Leaderboard";
            ((System.ComponentModel.ISupportInitialize)dgv_leaderboard).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgv_leaderboard;
        private Label label3;
    }
}