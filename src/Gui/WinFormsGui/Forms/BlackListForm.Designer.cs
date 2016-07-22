using System.Drawing;
using KryBot.CommonResources.Localization;
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.delToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFromИзToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.профильSteamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.listView = new System.Windows.Forms.ListView();
            this.steamGiftsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToolStripMenuItem,
            this.delToolStripMenuItem,
            this.loadFromИзToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(284, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // addToolStripMenuItem
            // 
            this.addToolStripMenuItem.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.plus;
            this.addToolStripMenuItem.Name = "addToolStripMenuItem";
            this.addToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.addToolStripMenuItem.Text = "Add";
            this.addToolStripMenuItem.Click += new System.EventHandler(this.добавитьToolStripMenuItem_Click);
            // 
            // delToolStripMenuItem
            // 
            this.delToolStripMenuItem.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.close1;
            this.delToolStripMenuItem.Name = "delToolStripMenuItem";
            this.delToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.delToolStripMenuItem.Text = "Delete";
            this.delToolStripMenuItem.Click += new System.EventHandler(this.удалитьToolStripMenuItem1_Click);
            // 
            // loadFromИзToolStripMenuItem
            // 
            this.loadFromИзToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.профильSteamToolStripMenuItem,
            this.steamGiftsToolStripMenuItem});
            this.loadFromИзToolStripMenuItem.Name = "loadFromИзToolStripMenuItem";
            this.loadFromИзToolStripMenuItem.Size = new System.Drawing.Size(83, 20);
            this.loadFromИзToolStripMenuItem.Text = "Load from...";
            // 
            // профильSteamToolStripMenuItem
            // 
            this.профильSteamToolStripMenuItem.Name = "профильSteamToolStripMenuItem";
            this.профильSteamToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.профильSteamToolStripMenuItem.Text = "Steam profile";
            this.профильSteamToolStripMenuItem.Click += new System.EventHandler(this.профильSteamToolStripMenuItem_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 271);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(284, 22);
            this.statusStrip.TabIndex = 5;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(52, 17);
            this.toolStripStatusLabel.Text = "Count: 0";
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
            // steamGiftsToolStripMenuItem
            // 
            this.steamGiftsToolStripMenuItem.Name = "steamGiftsToolStripMenuItem";
            this.steamGiftsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.steamGiftsToolStripMenuItem.Text = "SteamGifts";
            this.steamGiftsToolStripMenuItem.Click += new System.EventHandler(this.steamGiftsToolStripMenuItem_Click_1);
            // 
            // FormBlackList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 293);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormBlackList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Blacklist";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormBlackList_FormClosing);
            this.Load += new System.EventHandler(this.formBlackList_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem delToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem loadFromИзToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem профильSteamToolStripMenuItem;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
		private System.Windows.Forms.ListView listView;

		private void Localization()
		{
			addToolStripMenuItem.Text = strings.BlacklistForm_Add;
			delToolStripMenuItem.Text = strings.BlacklistForm_Del;
			loadFromИзToolStripMenuItem.Text = strings.BlacklistForm_LoadFrom;
            профильSteamToolStripMenuItem.Text = strings.BlacklistForm_SteamAccount;
            toolStripStatusLabel.Text = $"{strings.Count}: 0";
			Text = strings.Blacklist;
            Icon = Icon.FromHandle(Resources.blocked.GetHicon());
        }

        private System.Windows.Forms.ToolStripMenuItem steamGiftsToolStripMenuItem;
    }
}