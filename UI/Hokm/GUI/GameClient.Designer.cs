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
            this.info_panel = new System.Windows.Forms.Panel();
            this.info_title = new System.Windows.Forms.Label();
            this.info_text = new System.Windows.Forms.Label();
            this.score_text = new System.Windows.Forms.Label();
            this.scores_title = new System.Windows.Forms.Label();
            this.score_panel = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.winning_panel = new System.Windows.Forms.Panel();
            this.round_title = new System.Windows.Forms.Label();
            this.winner_label = new System.Windows.Forms.Label();
            this.p_id_1 = new System.Windows.Forms.Label();
            this.p_id_0 = new System.Windows.Forms.Label();
            this.p_id_2 = new System.Windows.Forms.Label();
            this.p_id_3 = new System.Windows.Forms.Label();
            this.ending_panel = new System.Windows.Forms.Panel();
            this.ending_winner = new System.Windows.Forms.Label();
            this.ending_title = new System.Windows.Forms.Label();
            this.game_again_but = new System.Windows.Forms.Button();
            this.exit_but = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.info_panel.SuspendLayout();
            this.score_panel.SuspendLayout();
            this.winning_panel.SuspendLayout();
            this.ending_panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // info_panel
            // 
            this.info_panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(234)))), ((int)(((byte)(216)))));
            this.info_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.info_panel.Controls.Add(this.info_title);
            this.info_panel.Controls.Add(this.info_text);
            this.info_panel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.info_panel.Location = new System.Drawing.Point(32, 30);
            this.info_panel.Name = "info_panel";
            this.info_panel.Size = new System.Drawing.Size(250, 130);
            this.info_panel.TabIndex = 1;
            // 
            // info_title
            // 
            this.info_title.AutoSize = true;
            this.info_title.Font = new System.Drawing.Font("Segoe UI", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.info_title.Location = new System.Drawing.Point(31, -1);
            this.info_title.Name = "info_title";
            this.info_title.Size = new System.Drawing.Size(183, 40);
            this.info_title.TabIndex = 2;
            this.info_title.Text = "Information";
            this.info_title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // info_text
            // 
            this.info_text.AutoSize = true;
            this.info_text.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.info_text.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.info_text.Location = new System.Drawing.Point(7, 39);
            this.info_text.Name = "info_text";
            this.info_text.Size = new System.Drawing.Size(230, 74);
            this.info_text.TabIndex = 0;
            this.info_text.Text = "Hakem: ruler_id \nHokm: hokm_card";
            this.info_text.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // score_text
            // 
            this.score_text.AutoSize = true;
            this.score_text.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.score_text.Location = new System.Drawing.Point(19, 39);
            this.score_text.Name = "score_text";
            this.score_text.Size = new System.Drawing.Size(116, 74);
            this.score_text.TabIndex = 2;
            this.score_text.Text = "4+2: p1 \n3+1: p2";
            this.score_text.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // scores_title
            // 
            this.scores_title.AutoSize = true;
            this.scores_title.Font = new System.Drawing.Font("Segoe UI", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.scores_title.Location = new System.Drawing.Point(68, -1);
            this.scores_title.Name = "scores_title";
            this.scores_title.Size = new System.Drawing.Size(114, 40);
            this.scores_title.TabIndex = 1;
            this.scores_title.Text = "Results";
            this.scores_title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // score_panel
            // 
            this.score_panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(234)))), ((int)(((byte)(216)))));
            this.score_panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.score_panel.Controls.Add(this.scores_title);
            this.score_panel.Controls.Add(this.score_text);
            this.score_panel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.score_panel.Location = new System.Drawing.Point(887, 30);
            this.score_panel.Name = "score_panel";
            this.score_panel.Size = new System.Drawing.Size(250, 130);
            this.score_panel.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Segoe UI", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.button1.Location = new System.Drawing.Point(956, 664);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(181, 148);
            this.button1.TabIndex = 5;
            this.button1.Text = "EndRound";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // winning_panel
            // 
            this.winning_panel.Controls.Add(this.round_title);
            this.winning_panel.Controls.Add(this.winner_label);
            this.winning_panel.Location = new System.Drawing.Point(272, 213);
            this.winning_panel.Name = "winning_panel";
            this.winning_panel.Size = new System.Drawing.Size(626, 445);
            this.winning_panel.TabIndex = 6;
            // 
            // round_title
            // 
            this.round_title.AutoSize = true;
            this.round_title.Font = new System.Drawing.Font("Segoe UI", 45F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.round_title.Location = new System.Drawing.Point(80, 16);
            this.round_title.Name = "round_title";
            this.round_title.Size = new System.Drawing.Size(470, 81);
            this.round_title.TabIndex = 2;
            this.round_title.Text = "End of Round n";
            this.round_title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // winner_label
            // 
            this.winner_label.AutoSize = true;
            this.winner_label.Font = new System.Drawing.Font("Segoe UI", 40F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.winner_label.Location = new System.Drawing.Point(109, 179);
            this.winner_label.Name = "winner_label";
            this.winner_label.Size = new System.Drawing.Size(424, 72);
            this.winner_label.TabIndex = 0;
            this.winner_label.Text = "Winner: client_id";
            // 
            // p_id_1
            // 
            this.p_id_1.AutoSize = true;
            this.p_id_1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.p_id_1.Location = new System.Drawing.Point(505, 9);
            this.p_id_1.Name = "p_id_1";
            this.p_id_1.Size = new System.Drawing.Size(90, 37);
            this.p_id_1.TabIndex = 7;
            this.p_id_1.Text = "label1";
            // 
            // p_id_0
            // 
            this.p_id_0.AutoSize = true;
            this.p_id_0.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.p_id_0.Location = new System.Drawing.Point(538, 825);
            this.p_id_0.Name = "p_id_0";
            this.p_id_0.Size = new System.Drawing.Size(90, 37);
            this.p_id_0.TabIndex = 8;
            this.p_id_0.Text = "label1";
            // 
            // p_id_2
            // 
            this.p_id_2.AutoSize = true;
            this.p_id_2.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.p_id_2.Location = new System.Drawing.Point(12, 419);
            this.p_id_2.Name = "p_id_2";
            this.p_id_2.Size = new System.Drawing.Size(90, 37);
            this.p_id_2.TabIndex = 9;
            this.p_id_2.Text = "label1";
            // 
            // p_id_3
            // 
            this.p_id_3.AutoSize = true;
            this.p_id_3.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.p_id_3.Location = new System.Drawing.Point(1086, 419);
            this.p_id_3.Name = "p_id_3";
            this.p_id_3.Size = new System.Drawing.Size(90, 37);
            this.p_id_3.TabIndex = 10;
            this.p_id_3.Text = "label1";
            // 
            // ending_panel
            // 
            this.ending_panel.Controls.Add(this.ending_winner);
            this.ending_panel.Controls.Add(this.ending_title);
            this.ending_panel.Controls.Add(this.game_again_but);
            this.ending_panel.Controls.Add(this.exit_but);
            this.ending_panel.Location = new System.Drawing.Point(429, 49);
            this.ending_panel.Name = "ending_panel";
            this.ending_panel.Size = new System.Drawing.Size(255, 160);
            this.ending_panel.TabIndex = 11;
            // 
            // ending_winner
            // 
            this.ending_winner.AutoSize = true;
            this.ending_winner.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ending_winner.Location = new System.Drawing.Point(54, 67);
            this.ending_winner.Name = "ending_winner";
            this.ending_winner.Size = new System.Drawing.Size(128, 28);
            this.ending_winner.TabIndex = 4;
            this.ending_winner.Text = "Winner: team";
            // 
            // ending_title
            // 
            this.ending_title.AutoSize = true;
            this.ending_title.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.ending_title.Location = new System.Drawing.Point(54, 0);
            this.ending_title.Name = "ending_title";
            this.ending_title.Size = new System.Drawing.Size(145, 37);
            this.ending_title.TabIndex = 3;
            this.ending_title.Text = "Game End";
            this.ending_title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // game_again_but
            // 
            this.game_again_but.Location = new System.Drawing.Point(130, 115);
            this.game_again_but.Name = "game_again_but";
            this.game_again_but.Size = new System.Drawing.Size(122, 42);
            this.game_again_but.TabIndex = 1;
            this.game_again_but.Text = "Another Game";
            this.game_again_but.UseVisualStyleBackColor = true;
            // 
            // exit_but
            // 
            this.exit_but.Location = new System.Drawing.Point(3, 115);
            this.exit_but.Name = "exit_but";
            this.exit_but.Size = new System.Drawing.Size(122, 42);
            this.exit_but.TabIndex = 0;
            this.exit_but.Text = "Exit Game";
            this.exit_but.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Segoe UI", 30F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.button2.Location = new System.Drawing.Point(21, 674);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(160, 148);
            this.button2.TabIndex = 12;
            this.button2.Text = "preset round";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.button3.Location = new System.Drawing.Point(94, 467);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(172, 148);
            this.button3.TabIndex = 13;
            this.button3.Text = "cmd round";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("Segoe UI", 37F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.button4.Location = new System.Drawing.Point(904, 467);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(166, 148);
            this.button4.TabIndex = 14;
            this.button4.Text = "end game";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // GameClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(1171, 871);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.ending_panel);
            this.Controls.Add(this.p_id_3);
            this.Controls.Add(this.p_id_2);
            this.Controls.Add(this.p_id_0);
            this.Controls.Add(this.p_id_1);
            this.Controls.Add(this.winning_panel);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.info_panel);
            this.Controls.Add(this.score_panel);
            this.Name = "GameClient";
            this.Text = "ODEA Client";
            this.Load += new System.EventHandler(this.GameClient_Load);
            this.info_panel.ResumeLayout(false);
            this.info_panel.PerformLayout();
            this.score_panel.ResumeLayout(false);
            this.score_panel.PerformLayout();
            this.winning_panel.ResumeLayout(false);
            this.winning_panel.PerformLayout();
            this.ending_panel.ResumeLayout(false);
            this.ending_panel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel info_panel;
        private System.Windows.Forms.Label info_text;
        private System.Windows.Forms.Label score_text;
        private System.Windows.Forms.Label scores_title;
        private System.Windows.Forms.Panel score_panel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label info_title;
        private System.Windows.Forms.Panel winning_panel;
        public System.Windows.Forms.Label winner_label;
        private System.Windows.Forms.Label p_id_1;
        private System.Windows.Forms.Label p_id_0;
        private System.Windows.Forms.Label p_id_2;
        private System.Windows.Forms.Label p_id_3;
        private System.Windows.Forms.Panel ending_panel;
        private System.Windows.Forms.Label round_title;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button game_again_but;
        private System.Windows.Forms.Button exit_but;
        public System.Windows.Forms.Label ending_winner;
        private System.Windows.Forms.Label ending_title;
    }
}