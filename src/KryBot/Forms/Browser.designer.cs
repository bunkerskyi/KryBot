namespace KryBot.Forms
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
			this.webBrowser = new System.Windows.Forms.WebBrowser();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabelLoad = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabelURL = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabelIE = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// webBrowser
			// 
			this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webBrowser.Location = new System.Drawing.Point(0, 0);
			this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser.Name = "webBrowser";
			this.webBrowser.Size = new System.Drawing.Size(284, 261);
			this.webBrowser.TabIndex = 0;
			this.webBrowser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(this.webBrowser_Navigated);
			this.webBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webBrowser_Navigating);
			// 
			// statusStrip1
			// 
			this.statusStrip1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.toolStripStatusLabelLoad,
			this.toolStripStatusLabelURL,
			this.toolStripStatusLabelIE});
			this.statusStrip1.Location = new System.Drawing.Point(0, 239);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(284, 22);
			this.statusStrip1.TabIndex = 1;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabelLoad
			// 
			this.toolStripStatusLabelLoad.Image = global::KryBot.Properties.Resources.load;
			this.toolStripStatusLabelLoad.Name = "toolStripStatusLabelLoad";
			this.toolStripStatusLabelLoad.Size = new System.Drawing.Size(80, 17);
			this.toolStripStatusLabelLoad.Text = "Загрузка...";
			// 
			// toolStripStatusLabelURL
			// 
			this.toolStripStatusLabelURL.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripStatusLabelURL.Name = "toolStripStatusLabelURL";
			this.toolStripStatusLabelURL.Size = new System.Drawing.Size(28, 17);
			this.toolStripStatusLabelURL.Text = "URL";
			// 
			// toolStripStatusLabelIE
			// 
			this.toolStripStatusLabelIE.Name = "toolStripStatusLabelIE";
			this.toolStripStatusLabelIE.Size = new System.Drawing.Size(16, 17);
			this.toolStripStatusLabelIE.Text = "IE";
			// 
			// Browser
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 261);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.webBrowser);
			this.Name = "Browser";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Browser";
			this.Load += new System.EventHandler(this.Browser_Load);
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.WebBrowser webBrowser;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelLoad;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelURL;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelIE;
	}
}