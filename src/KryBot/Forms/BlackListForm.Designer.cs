namespace KryBot.Forms
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
			this.добавитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.удалитьToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.загрузитьИзToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
			this.добавитьToolStripMenuItem,
			this.удалитьToolStripMenuItem1,
			this.загрузитьИзToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(284, 24);
			this.menuStrip1.TabIndex = 1;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// добавитьToolStripMenuItem
			// 
			this.добавитьToolStripMenuItem.Image = global::KryBot.Properties.Resources.plus;
			this.добавитьToolStripMenuItem.Name = "добавитьToolStripMenuItem";
			this.добавитьToolStripMenuItem.Size = new System.Drawing.Size(87, 20);
			this.добавитьToolStripMenuItem.Text = "Добавить";
			this.добавитьToolStripMenuItem.Click += new System.EventHandler(this.добавитьToolStripMenuItem_Click);
			// 
			// удалитьToolStripMenuItem1
			// 
			this.удалитьToolStripMenuItem1.Image = global::KryBot.Properties.Resources.close1;
			this.удалитьToolStripMenuItem1.Name = "удалитьToolStripMenuItem1";
			this.удалитьToolStripMenuItem1.Size = new System.Drawing.Size(79, 20);
			this.удалитьToolStripMenuItem1.Text = "Удалить";
			this.удалитьToolStripMenuItem1.Click += new System.EventHandler(this.удалитьToolStripMenuItem1_Click);
			// 
			// загрузитьИзToolStripMenuItem
			// 
			this.загрузитьИзToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.профильSteamToolStripMenuItem});
			this.загрузитьИзToolStripMenuItem.Name = "загрузитьИзToolStripMenuItem";
			this.загрузитьИзToolStripMenuItem.Size = new System.Drawing.Size(95, 20);
			this.загрузитьИзToolStripMenuItem.Text = "Добавить из...";
			// 
			// профильSteamToolStripMenuItem
			// 
			this.профильSteamToolStripMenuItem.Name = "профильSteamToolStripMenuItem";
			this.профильSteamToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
			this.профильSteamToolStripMenuItem.Text = "Профиль Steam";
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
			this.toolStripStatusLabel.Text = "Количество: 0";
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
			this.Text = "formBlackList";
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
		private System.Windows.Forms.ToolStripMenuItem добавитьToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem удалитьToolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem загрузитьИзToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem профильSteamToolStripMenuItem;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
		private System.Windows.Forms.ListView listView;
	}
}