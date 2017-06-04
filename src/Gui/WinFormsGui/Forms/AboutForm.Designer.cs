/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
*/
using System.Drawing;
using System.Windows.Forms;
using KryBot.CommonResources.Localization;
using KryBot.Core;
using KryBot.Gui.WinFormsGui.Properties;

namespace KryBot.Gui.WinFormsGui.Forms
{
	partial class FormAbout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAbout));
            this.labelVersion = new System.Windows.Forms.Label();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.linkLabelGitHub = new System.Windows.Forms.LinkLabel();
            this.linkLabelSteam = new System.Windows.Forms.LinkLabel();
            this.linkLabelVK = new System.Windows.Forms.LinkLabel();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(12, 9);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(80, 13);
            this.labelVersion.TabIndex = 0;
            this.labelVersion.Text = "KryBot - 1.0.0.0";
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.linkLabelGitHub);
            this.groupBox.Controls.Add(this.linkLabelSteam);
            this.groupBox.Controls.Add(this.linkLabelVK);
            this.groupBox.Location = new System.Drawing.Point(12, 25);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(260, 67);
            this.groupBox.TabIndex = 1;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Social links";
            // 
            // linkLabelGitHub
            // 
            this.linkLabelGitHub.AutoSize = true;
            this.linkLabelGitHub.Location = new System.Drawing.Point(6, 42);
            this.linkLabelGitHub.Name = "linkLabelGitHub";
            this.linkLabelGitHub.Size = new System.Drawing.Size(176, 13);
            this.linkLabelGitHub.TabIndex = 2;
            this.linkLabelGitHub.TabStop = true;
            this.linkLabelGitHub.Text = "https://github.com/KriBetko/KryBot";
            this.linkLabelGitHub.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel3_LinkClicked);
            // 
            // linkLabelSteam
            // 
            this.linkLabelSteam.AutoSize = true;
            this.linkLabelSteam.Location = new System.Drawing.Point(6, 29);
            this.linkLabelSteam.Name = "linkLabelSteam";
            this.linkLabelSteam.Size = new System.Drawing.Size(215, 13);
            this.linkLabelSteam.TabIndex = 1;
            this.linkLabelSteam.TabStop = true;
            this.linkLabelSteam.Text = "https://steamcommunity.com/groups/krybot";
            this.linkLabelSteam.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // linkLabelVK
            // 
            this.linkLabelVK.AutoSize = true;
            this.linkLabelVK.Location = new System.Drawing.Point(6, 16);
            this.linkLabelVK.Name = "linkLabelVK";
            this.linkLabelVK.Size = new System.Drawing.Size(112, 13);
            this.linkLabelVK.TabIndex = 0;
            this.linkLabelVK.TabStop = true;
            this.linkLabelVK.Text = "https://vk.com/krybot";
            this.linkLabelVK.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // FormAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 103);
            this.Controls.Add(this.groupBox);
            this.Controls.Add(this.labelVersion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAbout";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About program";
            this.groupBox.ResumeLayout(false);
            this.groupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.GroupBox groupBox;
		private System.Windows.Forms.LinkLabel linkLabelVK;
		private System.Windows.Forms.LinkLabel linkLabelGitHub;
		private System.Windows.Forms.LinkLabel linkLabelSteam;

		private void Localization()
		{
			labelVersion.Text = $"KryBot - {Application.ProductVersion}";
			groupBox.Text = strings.AboutForm_groupBox1;
			Text = strings.AboutProgram;
            Icon = Icon.FromHandle(Resources.info.GetHicon());
        }
	}
}