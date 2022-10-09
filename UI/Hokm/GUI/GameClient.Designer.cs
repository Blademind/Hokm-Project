using System;

namespace Hokm
{
    partial class GameClient
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameClient));
            this.score_panel = new System.Windows.Forms.Panel();
            this.scores_title = new System.Windows.Forms.Label();
            this.score_text = new System.Windows.Forms.Label();
            this.info_panel = new System.Windows.Forms.Panel();
            this.info_text = new System.Windows.Forms.Label();
            this.score_panel.SuspendLayout();
            this.info_panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // score_panel
            // 
            this.score_panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(234)))), ((int)(((byte)(216)))));
            this.score_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.score_panel.Controls.Add(this.scores_title);
            this.score_panel.Controls.Add(this.score_text);
            this.score_panel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.score_panel.Location = new System.Drawing.Point(813, 557);
            this.score_panel.Name = "score_panel";
            this.score_panel.Size = new System.Drawing.Size(275, 130);
            this.score_panel.TabIndex = 0;
            // 
            // scores_title
            // 
            this.scores_title.AutoSize = true;
            this.scores_title.Font = new System.Drawing.Font("Segoe UI", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.scores_title.Location = new System.Drawing.Point(83, -1);
            this.scores_title.Name = "scores_title";
            this.scores_title.Size = new System.Drawing.Size(105, 40);
            this.scores_title.TabIndex = 1;
            this.scores_title.Text = "Results";
            this.scores_title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // score_text
            // 
            this.score_text.AutoSize = true;
            this.score_text.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.score_text.Location = new System.Drawing.Point(19, 39);
            this.score_text.Name = "score_text";
            this.score_text.Size = new System.Drawing.Size(201, 74);
            this.score_text.TabIndex = 0;
            this.score_text.Text = "You / 3: SCORE \n2 / 4: SCORE";
            this.score_text.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // info_panel
            // 
            this.info_panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(234)))), ((int)(((byte)(216)))));
            this.info_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.info_panel.Controls.Add(this.info_text);
            this.info_panel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.info_panel.Location = new System.Drawing.Point(12, 27);
            this.info_panel.Name = "info_panel";
            this.info_panel.Size = new System.Drawing.Size(344, 47);
            this.info_panel.TabIndex = 1;
            // 
            // info_text
            // 
            this.info_text.AutoSize = true;
            this.info_text.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.info_text.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.info_text.Location = new System.Drawing.Point(-1, 0);
            this.info_text.Name = "info_text";
            this.info_text.Size = new System.Drawing.Size(347, 37);
            this.info_text.TabIndex = 0;
            this.info_text.Text = "Hakem: person | Hokm: sign";
            this.info_text.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // GameClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Controls.Add(this.info_panel);
            this.Controls.Add(this.score_panel);
            this.Name = "GameClient";
            this.Text = "ODEA Client";
            this.score_panel.ResumeLayout(false);
            this.score_panel.PerformLayout();
            this.info_panel.ResumeLayout(false);
            this.info_panel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel score_panel;
        private System.Windows.Forms.Label score_text;
        private System.Windows.Forms.Panel info_panel;
        private System.Windows.Forms.Label info_text;
        private System.Windows.Forms.Label scores_title;
    }
}