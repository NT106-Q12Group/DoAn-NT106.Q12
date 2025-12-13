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
            ((System.ComponentModel.ISupportInitialize)dgv_leaderboard).BeginInit();
            SuspendLayout();
            // 
            // dgv_leaderboard
            // 
            dgv_leaderboard.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
            dgv_leaderboard.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv_leaderboard.Dock = DockStyle.Bottom;
            dgv_leaderboard.Location = new Point(0, 66);
            dgv_leaderboard.Name = "dgv_leaderboard";
            dgv_leaderboard.RowHeadersWidth = 62;
            dgv_leaderboard.Size = new Size(800, 384);
            dgv_leaderboard.TabIndex = 0;
            // 
            // Leaderboard
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(dgv_leaderboard);
            Name = "Leaderboard";
            Text = "Leaderboard";
            ((System.ComponentModel.ISupportInitialize)dgv_leaderboard).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgv_leaderboard;
    }
}