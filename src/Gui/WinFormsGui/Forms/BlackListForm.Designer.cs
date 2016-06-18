using System.Drawing;
using KryBot.CommonResources.lang;
using KryBot.Gui.WinFormsGui.Properties;

namespace KryBot.Gui.WinFormsGui.Forms
{
	partial class FormBlackList
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
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.delToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.loadFromИзToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.профильSteamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.listView = new System.Windows.Forms.ListView();
			this.menuStrip1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.delToolStripMenuItem1,
            this.loadFromИзToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(284, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// addToolStripMenuItem
			// 
			this.addToolStripMenuItem.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.plus;
			this.addToolStripMenuItem.Name = "addToolStripMenuItem";
			this.addToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
			this.addToolStripMenuItem.Text = strings.BlacklistForm_Add;
			this.addToolStripMenuItem.Click += new System.EventHandler(this.добавитьToolStripMenuItem_Click);
			// 
			// delToolStripMenuItem1
			// 
			this.delToolStripMenuItem1.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.close1;
			this.delToolStripMenuItem1.Name = "delToolStripMenuItem1";
			this.delToolStripMenuItem1.Size = new System.Drawing.Size(79, 20);
			this.delToolStripMenuItem1.Text = strings.BlacklistForm_Del;
			this.delToolStripMenuItem1.Click += new System.EventHandler(this.удалитьToolStripMenuItem1_Click);
			// 
			// loadFromИзToolStripMenuItem
			// 
			this.loadFromИзToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.профильSteamToolStripMenuItem});
			this.loadFromИзToolStripMenuItem.Name = "loadFromИзToolStripMenuItem";
			this.loadFromИзToolStripMenuItem.Size = new System.Drawing.Size(95, 20);
			this.loadFromИзToolStripMenuItem.Text = strings.BlacklistForm_LoadFrom;
			// 
			// профильSteamToolStripMenuItem
			// 
			this.профильSteamToolStripMenuItem.Name = "профильSteamToolStripMenuItem";
			this.профильSteamToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
			this.профильSteamToolStripMenuItem.Text = strings.BlacklistForm_SteamAccount;
			this.профильSteamToolStripMenuItem.Click += new System.EventHandler(this.профильSteamToolStripMenuItem_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
			this.statusStrip1.Location = new System.Drawing.Point(0, 271);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(284, 22);
			this.statusStrip1.TabIndex = 5;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabel
			// 
			this.toolStripStatusLabel.Name = "toolStripStatusLabel";
			this.toolStripStatusLabel.Size = new System.Drawing.Size(84, 17);
			this.toolStripStatusLabel.Text = $"{strings.Count}: 0";
			// 
			// listView
			// 
			this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView.FullRowSelect = true;
			this.listView.GridLines = true;
			this.listView.Location = new System.Drawing.Point(0, 24);
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(284, 247);
			this.listView.TabIndex = 6;
			this.listView.UseCompatibleStateImageBehavior = false;
			this.listView.View = System.Windows.Forms.View.Details;
			this.listView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
			// 
			// FormBlackList
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.Icon = Icon.FromHandle(Resources.blocked.GetHicon());
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 293);
			this.Controls.Add(this.listView);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormBlackList";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = strings.Blacklist;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormBlackList_FormClosing);
			this.Load += new System.EventHandler(this.formBlackList_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem delToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem loadFromИзToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem профильSteamToolStripMenuItem;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
		private System.Windows.Forms.ListView listView;
	}
}