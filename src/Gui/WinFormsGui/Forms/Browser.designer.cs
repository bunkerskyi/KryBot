namespace KryBot.Gui.WinFormsGui.Forms
{
	partial class Browser
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
			if(disposing && (components != null))
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
            this.components = new System.ComponentModel.Container();
            this.browserPanel = new System.Windows.Forms.Panel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.loadStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabelChromium = new System.Windows.Forms.ToolStripStatusLabel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.browserPanel.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // browserPanel
            // 
            this.browserPanel.Controls.Add(this.statusStrip);
            this.browserPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browserPanel.Location = new System.Drawing.Point(0, 0);
            this.browserPanel.Name = "browserPanel";
            this.browserPanel.Size = new System.Drawing.Size(284, 261);
            this.browserPanel.TabIndex = 0;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadStatusLabel,
            this.toolStripStatusLabelChromium});
            this.statusStrip.Location = new System.Drawing.Point(0, 239);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(284, 22);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // loadStatusLabel
            // 
            this.loadStatusLabel.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.load;
            this.loadStatusLabel.Name = "loadStatusLabel";
            this.loadStatusLabel.Size = new System.Drawing.Size(16, 17);
            this.loadStatusLabel.Click += new System.EventHandler(this.loadStatusLabel_Click);
            // 
            // toolStripStatusLabelChromium
            // 
            this.toolStripStatusLabelChromium.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabelChromium.Name = "toolStripStatusLabelChromium";
            this.toolStripStatusLabelChromium.Size = new System.Drawing.Size(68, 17);
            this.toolStripStatusLabelChromium.Text = "Chromium:";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // Browser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.browserPanel);
            this.Name = "Browser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NewBrowser";
            this.Load += new System.EventHandler(this.NewBrowser_Load);
            this.browserPanel.ResumeLayout(false);
            this.browserPanel.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel browserPanel;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelChromium;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolStripStatusLabel loadStatusLabel;
	}
}