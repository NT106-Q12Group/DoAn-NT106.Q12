namespace CaroGame
{
    partial class BotDifficulty
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
            btn_easy = new Button();
            btn_medium = new Button();
            btn_hard = new Button();
            btn_extremely_hard = new Button();
            lb_title = new Label();
            SuspendLayout();
            // 
            // btn_easy
            // 
            btn_easy.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 163);
            btn_easy.Location = new Point(287, 160);
            btn_easy.Name = "btn_easy";
            btn_easy.Size = new Size(176, 43);
            btn_easy.TabIndex = 0;
            btn_easy.Text = "Dễ";
            btn_easy.UseVisualStyleBackColor = true;
            btn_easy.Click += btn_easy_Click;
            // 
            // btn_medium
            // 
            btn_medium.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 163);
            btn_medium.Location = new Point(287, 209);
            btn_medium.Name = "btn_medium";
            btn_medium.Size = new Size(176, 43);
            btn_medium.TabIndex = 1;
            btn_medium.Text = "Thường";
            btn_medium.UseVisualStyleBackColor = true;
            btn_medium.Click += btn_medium_Click;
            // 
            // btn_hard
            // 
            btn_hard.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 163);
            btn_hard.Location = new Point(287, 258);
            btn_hard.Name = "btn_hard";
            btn_hard.Size = new Size(176, 43);
            btn_hard.TabIndex = 2;
            btn_hard.Text = "Khó";
            btn_hard.UseVisualStyleBackColor = true;
            btn_hard.Click += btn_hard_Click;
            // 
            // btn_extremely_hard
            // 
            btn_extremely_hard.Font = new Font("Segoe UI", 13.8F, FontStyle.Bold, GraphicsUnit.Point, 163);
            btn_extremely_hard.Location = new Point(287, 307);
            btn_extremely_hard.Name = "btn_extremely_hard";
            btn_extremely_hard.Size = new Size(176, 43);
            btn_extremely_hard.TabIndex = 3;
            btn_extremely_hard.Text = "Cực Khó";
            btn_extremely_hard.UseVisualStyleBackColor = true;
            btn_extremely_hard.Click += btn_extremely_hard_Click;
            // 
            // lb_title
            // 
            lb_title.AutoSize = true;
            lb_title.Font = new Font("Segoe UI Semibold", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 163);
            lb_title.Location = new Point(230, 75);
            lb_title.Name = "lb_title";
            lb_title.Size = new Size(285, 38);
            lb_title.TabIndex = 4;
            lb_title.Text = "Chọn độ khó của Bot";
            // 
            // BotDifficulty
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lb_title);
            Controls.Add(btn_extremely_hard);
            Controls.Add(btn_hard);
            Controls.Add(btn_medium);
            Controls.Add(btn_easy);
            Name = "BotDifficulty";
            Text = "BotDifficulty";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btn_easy;
        private Button btn_medium;
        private Button btn_hard;
        private Button btn_extremely_hard;
        private Label lb_title;
    }
}