namespace KryBot
{
    partial class FormStatistic
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
            this.gbSession = new System.Windows.Forms.GroupBox();
            this.lblSessionLoops = new System.Windows.Forms.Label();
            this.lblSessionJoins = new System.Windows.Forms.Label();
            this.gbTotal = new System.Windows.Forms.GroupBox();
            this.lblTotalLoops = new System.Windows.Forms.Label();
            this.lblTotalJoins = new System.Windows.Forms.Label();
            this.gbSession.SuspendLayout();
            this.gbTotal.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbSession
            // 
            this.gbSession.Controls.Add(this.lblSessionLoops);
            this.gbSession.Controls.Add(this.lblSessionJoins);
            this.gbSession.Location = new System.Drawing.Point(12, 12);
            this.gbSession.Name = "gbSession";
            this.gbSession.Size = new System.Drawing.Size(260, 50);
            this.gbSession.TabIndex = 0;
            this.gbSession.TabStop = false;
            this.gbSession.Text = "За сессию";
            // 
            // lblSessionLoops
            // 
            this.lblSessionLoops.AutoSize = true;
            this.lblSessionLoops.Location = new System.Drawing.Point(6, 29);
            this.lblSessionLoops.Name = "lblSessionLoops";
            this.lblSessionLoops.Size = new System.Drawing.Size(105, 13);
            this.lblSessionLoops.TabIndex = 1;
            this.lblSessionLoops.Text = "Циклов фарминга: ";
            // 
            // lblSessionJoins
            // 
            this.lblSessionJoins.AutoSize = true;
            this.lblSessionJoins.Location = new System.Drawing.Point(6, 16);
            this.lblSessionJoins.Name = "lblSessionJoins";
            this.lblSessionJoins.Size = new System.Drawing.Size(125, 13);
            this.lblSessionJoins.TabIndex = 0;
            this.lblSessionJoins.Text = "Вступлений в раздачи: ";
            // 
            // gbTotal
            // 
            this.gbTotal.Controls.Add(this.lblTotalLoops);
            this.gbTotal.Controls.Add(this.lblTotalJoins);
            this.gbTotal.Location = new System.Drawing.Point(12, 68);
            this.gbTotal.Name = "gbTotal";
            this.gbTotal.Size = new System.Drawing.Size(260, 50);
            this.gbTotal.TabIndex = 2;
            this.gbTotal.TabStop = false;
            this.gbTotal.Text = "За все время";
            // 
            // lblTotalLoops
            // 
            this.lblTotalLoops.AutoSize = true;
            this.lblTotalLoops.Location = new System.Drawing.Point(6, 29);
            this.lblTotalLoops.Name = "lblTotalLoops";
            this.lblTotalLoops.Size = new System.Drawing.Size(105, 13);
            this.lblTotalLoops.TabIndex = 1;
            this.lblTotalLoops.Text = "Циклов фарминга: ";
            // 
            // lblTotalJoins
            // 
            this.lblTotalJoins.AutoSize = true;
            this.lblTotalJoins.Location = new System.Drawing.Point(6, 16);
            this.lblTotalJoins.Name = "lblTotalJoins";
            this.lblTotalJoins.Size = new System.Drawing.Size(125, 13);
            this.lblTotalJoins.TabIndex = 0;
            this.lblTotalJoins.Text = "Вступлений в раздачи: ";
            // 
            // FormStatistic
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 129);
            this.Controls.Add(this.gbTotal);
            this.Controls.Add(this.gbSession);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormStatistic";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "formStatistic";
            this.Load += new System.EventHandler(this.formStatistic_Load);
            this.gbSession.ResumeLayout(false);
            this.gbSession.PerformLayout();
            this.gbTotal.ResumeLayout(false);
            this.gbTotal.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbSession;
        private System.Windows.Forms.Label lblSessionLoops;
        private System.Windows.Forms.Label lblSessionJoins;
        private System.Windows.Forms.GroupBox gbTotal;
        private System.Windows.Forms.Label lblTotalLoops;
        private System.Windows.Forms.Label lblTotalJoins;
    }
}