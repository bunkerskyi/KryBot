using System.Windows.Forms;
using KryBot.CommonResources.Localization;
using KryBot.Gui.WinFormsGui.Properties;

namespace KryBot.Gui.WinFormsGui.Forms
{
	partial class FormMain
	{
		/// <summary>
		/// Обязательная переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsКакToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.blacklistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.informationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.donateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageSteam = new System.Windows.Forms.TabPage();
            this.btnSteamExit = new System.Windows.Forms.Button();
            this.btnSteamLogin = new System.Windows.Forms.Button();
            this.linkLabelSteam = new System.Windows.Forms.LinkLabel();
            this.lblSteamStatus = new System.Windows.Forms.Label();
            this.tabPageGM = new System.Windows.Forms.TabPage();
            this.btnGMExit = new System.Windows.Forms.Button();
            this.cbGMEnable = new System.Windows.Forms.CheckBox();
            this.linkLabelGM = new System.Windows.Forms.LinkLabel();
            this.pbGMReload = new System.Windows.Forms.PictureBox();
            this.btnGMLogin = new System.Windows.Forms.Button();
            this.lblGMStatus = new System.Windows.Forms.Label();
            this.lblGMCoal = new System.Windows.Forms.Label();
            this.lblGMLevel = new System.Windows.Forms.Label();
            this.tabPageSG = new System.Windows.Forms.TabPage();
            this.btnSGExit = new System.Windows.Forms.Button();
            this.cbSGEnable = new System.Windows.Forms.CheckBox();
            this.linkLabelSG = new System.Windows.Forms.LinkLabel();
            this.pbSGReload = new System.Windows.Forms.PictureBox();
            this.lblSGStatus = new System.Windows.Forms.Label();
            this.lblSGPoints = new System.Windows.Forms.Label();
            this.lblSGLevel = new System.Windows.Forms.Label();
            this.btnSGLogin = new System.Windows.Forms.Button();
            this.tabPageSC = new System.Windows.Forms.TabPage();
            this.btnSCExit = new System.Windows.Forms.Button();
            this.cbSCEnable = new System.Windows.Forms.CheckBox();
            this.linkLabelSC = new System.Windows.Forms.LinkLabel();
            this.pbSCReload = new System.Windows.Forms.PictureBox();
            this.lblSCStatus = new System.Windows.Forms.Label();
            this.lblSCPoints = new System.Windows.Forms.Label();
            this.lblSCLevel = new System.Windows.Forms.Label();
            this.btnSCLogin = new System.Windows.Forms.Button();
            this.tabPageSP = new System.Windows.Forms.TabPage();
            this.btnSPExit = new System.Windows.Forms.Button();
            this.cbSPEnable = new System.Windows.Forms.CheckBox();
            this.linkLabelSP = new System.Windows.Forms.LinkLabel();
            this.pbUSPeload = new System.Windows.Forms.PictureBox();
            this.lblSPStatus = new System.Windows.Forms.Label();
            this.lblSPPoints = new System.Windows.Forms.Label();
            this.lblSPLevel = new System.Windows.Forms.Label();
            this.btnSPLogin = new System.Windows.Forms.Button();
            this.tabPageST = new System.Windows.Forms.TabPage();
            this.btnSTExit = new System.Windows.Forms.Button();
            this.cbSTEnable = new System.Windows.Forms.CheckBox();
            this.linkLabelST = new System.Windows.Forms.LinkLabel();
            this.pbSTreload = new System.Windows.Forms.PictureBox();
            this.lblSTStatus = new System.Windows.Forms.Label();
            this.lblSTPoints = new System.Windows.Forms.Label();
            this.lblSTLevel = new System.Windows.Forms.Label();
            this.btnSTLogin = new System.Windows.Forms.Button();
            this.tabPagePB = new System.Windows.Forms.TabPage();
            this.btnPBExit = new System.Windows.Forms.Button();
            this.btnPBLogin = new System.Windows.Forms.Button();
            this.pbPBRefresh = new System.Windows.Forms.PictureBox();
            this.cbPBEnabled = new System.Windows.Forms.CheckBox();
            this.linkLabelPB = new System.Windows.Forms.LinkLabel();
            this.lblPBStatus = new System.Windows.Forms.Label();
            this.lblPBPoints = new System.Windows.Forms.Label();
            this.lblPBLevel = new System.Windows.Forms.Label();
            this.tabPageGA = new System.Windows.Forms.TabPage();
            this.pbGARefresh = new System.Windows.Forms.PictureBox();
            this.cbGAEnabled = new System.Windows.Forms.CheckBox();
            this.btnGAExit = new System.Windows.Forms.Button();
            this.btnGALogin = new System.Windows.Forms.Button();
            this.linkLabelGA = new System.Windows.Forms.LinkLabel();
            this.lblGAPoints = new System.Windows.Forms.Label();
            this.lblGALevel = new System.Windows.Forms.Label();
            this.lblGAStatus = new System.Windows.Forms.Label();
            this.tabPageIG = new System.Windows.Forms.TabPage();
            this.btnIGLogout = new System.Windows.Forms.Button();
            this.cbIGEnabled = new System.Windows.Forms.CheckBox();
            this.linkLabelIG = new System.Windows.Forms.LinkLabel();
            this.pbIGRefresh = new System.Windows.Forms.PictureBox();
            this.btnIGLogin = new System.Windows.Forms.Button();
            this.lblIGStatus = new System.Windows.Forms.Label();
            this.lblIGPoints = new System.Windows.Forms.Label();
            this.lblIGLevel = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.toolStripMenuItem_Main = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem_Show = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Farm = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabPageSteam.SuspendLayout();
            this.tabPageGM.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGMReload)).BeginInit();
            this.tabPageSG.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSGReload)).BeginInit();
            this.tabPageSC.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSCReload)).BeginInit();
            this.tabPageSP.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbUSPeload)).BeginInit();
            this.tabPageST.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSTreload)).BeginInit();
            this.tabPagePB.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPBRefresh)).BeginInit();
            this.tabPageGA.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGARefresh)).BeginInit();
            this.tabPageIG.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIGRefresh)).BeginInit();
            this.toolStripMenuItem_Main.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel,
            this.toolStripProgressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 205);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(350, 22);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(96, 17);
            this.toolStripStatusLabel.Text = "Loading profile...";
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.informationToolStripMenuItem,
            this.logToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(350, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFolderToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsКакToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.file;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openFolderToolStripMenuItem
            // 
            this.openFolderToolStripMenuItem.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.folder;
            this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            this.openFolderToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.openFolderToolStripMenuItem.Text = "Open folder with bot";
            this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.вПапкуСБотомToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.file;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.сохранитьToolStripMenuItem_Click);
            // 
            // saveAsКакToolStripMenuItem
            // 
            this.saveAsКакToolStripMenuItem.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.file;
            this.saveAsКакToolStripMenuItem.Name = "saveAsКакToolStripMenuItem";
            this.saveAsКакToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.saveAsКакToolStripMenuItem.Text = "Save as...";
            this.saveAsКакToolStripMenuItem.Click += new System.EventHandler(this.сохранитьКакToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.exit;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(184, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.загрузитьToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.blacklistToolStripMenuItem});
            this.toolsToolStripMenuItem.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.settings;
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.settings;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.настройкиToolStripMenuItem1_Click);
            // 
            // blacklistToolStripMenuItem
            // 
            this.blacklistToolStripMenuItem.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.blocked;
            this.blacklistToolStripMenuItem.Name = "blacklistToolStripMenuItem";
            this.blacklistToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.blacklistToolStripMenuItem.Text = "Blacklist";
            this.blacklistToolStripMenuItem.Click += new System.EventHandler(this.черныйСписокToolStripMenuItem_Click);
            // 
            // informationToolStripMenuItem
            // 
            this.informationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.donateToolStripMenuItem});
            this.informationToolStripMenuItem.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.info;
            this.informationToolStripMenuItem.Name = "informationToolStripMenuItem";
            this.informationToolStripMenuItem.Size = new System.Drawing.Size(98, 20);
            this.informationToolStripMenuItem.Text = "Information";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.info;
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.aboutToolStripMenuItem.Text = "About program";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.оПрограммеToolStripMenuItem1_Click);
            // 
            // donateToolStripMenuItem
            // 
            this.donateToolStripMenuItem.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.wallet;
            this.donateToolStripMenuItem.Name = "donateToolStripMenuItem";
            this.donateToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.donateToolStripMenuItem.Text = "Donate";
            this.donateToolStripMenuItem.Click += new System.EventHandler(this.донатToolStripMenuItem_Click);
            // 
            // logToolStripMenuItem
            // 
            this.logToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.logToolStripMenuItem.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.log1;
            this.logToolStripMenuItem.Name = "logToolStripMenuItem";
            this.logToolStripMenuItem.Size = new System.Drawing.Size(74, 20);
            this.logToolStripMenuItem.Text = "Log <<";
            this.logToolStripMenuItem.Click += new System.EventHandler(this.логToolStripMenuItem_Click);
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageSteam);
            this.tabControl.Controls.Add(this.tabPageGM);
            this.tabControl.Controls.Add(this.tabPageSG);
            this.tabControl.Controls.Add(this.tabPageSC);
            this.tabControl.Controls.Add(this.tabPageSP);
            this.tabControl.Controls.Add(this.tabPageST);
            this.tabControl.Controls.Add(this.tabPagePB);
            this.tabControl.Controls.Add(this.tabPageGA);
            this.tabControl.Controls.Add(this.tabPageIG);
            this.tabControl.Location = new System.Drawing.Point(4, 27);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(342, 105);
            this.tabControl.TabIndex = 4;
            // 
            // tabPageSteam
            // 
            this.tabPageSteam.Controls.Add(this.btnSteamExit);
            this.tabPageSteam.Controls.Add(this.btnSteamLogin);
            this.tabPageSteam.Controls.Add(this.linkLabelSteam);
            this.tabPageSteam.Controls.Add(this.lblSteamStatus);
            this.tabPageSteam.Location = new System.Drawing.Point(4, 40);
            this.tabPageSteam.Name = "tabPageSteam";
            this.tabPageSteam.Size = new System.Drawing.Size(334, 61);
            this.tabPageSteam.TabIndex = 5;
            this.tabPageSteam.Text = "Steam";
            this.tabPageSteam.UseVisualStyleBackColor = true;
            // 
            // btnSteamExit
            // 
            this.btnSteamExit.Location = new System.Drawing.Point(134, 19);
            this.btnSteamExit.Name = "btnSteamExit";
            this.btnSteamExit.Size = new System.Drawing.Size(75, 23);
            this.btnSteamExit.TabIndex = 29;
            this.btnSteamExit.Text = "Logout";
            this.btnSteamExit.UseVisualStyleBackColor = true;
            this.btnSteamExit.Click += new System.EventHandler(this.btnSteamExit_Click);
            // 
            // btnSteamLogin
            // 
            this.btnSteamLogin.Location = new System.Drawing.Point(134, 19);
            this.btnSteamLogin.Name = "btnSteamLogin";
            this.btnSteamLogin.Size = new System.Drawing.Size(75, 23);
            this.btnSteamLogin.TabIndex = 12;
            this.btnSteamLogin.Text = "Login";
            this.btnSteamLogin.UseVisualStyleBackColor = true;
            this.btnSteamLogin.Click += new System.EventHandler(this.btnSteamLogin_Click);
            // 
            // linkLabelSteam
            // 
            this.linkLabelSteam.AutoSize = true;
            this.linkLabelSteam.Location = new System.Drawing.Point(6, 42);
            this.linkLabelSteam.Name = "linkLabelSteam";
            this.linkLabelSteam.Size = new System.Drawing.Size(59, 13);
            this.linkLabelSteam.TabIndex = 11;
            this.linkLabelSteam.TabStop = true;
            this.linkLabelSteam.Text = "To website";
            this.linkLabelSteam.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel6_LinkClicked);
            // 
            // lblSteamStatus
            // 
            this.lblSteamStatus.AutoSize = true;
            this.lblSteamStatus.Location = new System.Drawing.Point(6, 3);
            this.lblSteamStatus.Name = "lblSteamStatus";
            this.lblSteamStatus.Size = new System.Drawing.Size(112, 13);
            this.lblSteamStatus.TabIndex = 10;
            this.lblSteamStatus.Text = "Status: Not authorized";
            // 
            // tabPageGM
            // 
            this.tabPageGM.Controls.Add(this.btnGMExit);
            this.tabPageGM.Controls.Add(this.cbGMEnable);
            this.tabPageGM.Controls.Add(this.linkLabelGM);
            this.tabPageGM.Controls.Add(this.pbGMReload);
            this.tabPageGM.Controls.Add(this.btnGMLogin);
            this.tabPageGM.Controls.Add(this.lblGMStatus);
            this.tabPageGM.Controls.Add(this.lblGMCoal);
            this.tabPageGM.Controls.Add(this.lblGMLevel);
            this.tabPageGM.Location = new System.Drawing.Point(4, 40);
            this.tabPageGM.Name = "tabPageGM";
            this.tabPageGM.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGM.Size = new System.Drawing.Size(334, 61);
            this.tabPageGM.TabIndex = 0;
            this.tabPageGM.Text = "GM";
            this.tabPageGM.UseVisualStyleBackColor = true;
            // 
            // btnGMExit
            // 
            this.btnGMExit.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGMExit.Location = new System.Drawing.Point(134, 19);
            this.btnGMExit.Name = "btnGMExit";
            this.btnGMExit.Size = new System.Drawing.Size(75, 23);
            this.btnGMExit.TabIndex = 27;
            this.btnGMExit.Text = "Logout";
            this.btnGMExit.UseVisualStyleBackColor = true;
            this.btnGMExit.Click += new System.EventHandler(this.btnGMExit_Click);
            // 
            // cbGMEnable
            // 
            this.cbGMEnable.AutoSize = true;
            this.cbGMEnable.Checked = true;
            this.cbGMEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbGMEnable.Location = new System.Drawing.Point(315, 3);
            this.cbGMEnable.Name = "cbGMEnable";
            this.cbGMEnable.Size = new System.Drawing.Size(15, 14);
            this.cbGMEnable.TabIndex = 26;
            this.cbGMEnable.UseVisualStyleBackColor = true;
            this.cbGMEnable.CheckedChanged += new System.EventHandler(this.cbGMEnable_CheckedChanged);
            // 
            // linkLabelGM
            // 
            this.linkLabelGM.AutoSize = true;
            this.linkLabelGM.Location = new System.Drawing.Point(6, 42);
            this.linkLabelGM.Name = "linkLabelGM";
            this.linkLabelGM.Size = new System.Drawing.Size(59, 13);
            this.linkLabelGM.TabIndex = 7;
            this.linkLabelGM.TabStop = true;
            this.linkLabelGM.Text = "To website";
            this.linkLabelGM.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // pbGMReload
            // 
            this.pbGMReload.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.refresh;
            this.pbGMReload.Location = new System.Drawing.Point(314, 42);
            this.pbGMReload.Name = "pbGMReload";
            this.pbGMReload.Size = new System.Drawing.Size(16, 16);
            this.pbGMReload.TabIndex = 6;
            this.pbGMReload.TabStop = false;
            this.pbGMReload.Click += new System.EventHandler(this.pbGMReload_Click);
            // 
            // btnGMLogin
            // 
            this.btnGMLogin.Location = new System.Drawing.Point(134, 19);
            this.btnGMLogin.Name = "btnGMLogin";
            this.btnGMLogin.Size = new System.Drawing.Size(75, 23);
            this.btnGMLogin.TabIndex = 5;
            this.btnGMLogin.Text = "Login";
            this.btnGMLogin.UseVisualStyleBackColor = true;
            this.btnGMLogin.Click += new System.EventHandler(this.btnGMLogin_Click);
            // 
            // lblGMStatus
            // 
            this.lblGMStatus.AutoSize = true;
            this.lblGMStatus.Location = new System.Drawing.Point(6, 3);
            this.lblGMStatus.Name = "lblGMStatus";
            this.lblGMStatus.Size = new System.Drawing.Size(112, 13);
            this.lblGMStatus.TabIndex = 3;
            this.lblGMStatus.Text = "Status: Not authorized";
            // 
            // lblGMCoal
            // 
            this.lblGMCoal.AutoSize = true;
            this.lblGMCoal.Location = new System.Drawing.Point(6, 29);
            this.lblGMCoal.Name = "lblGMCoal";
            this.lblGMCoal.Size = new System.Drawing.Size(39, 13);
            this.lblGMCoal.TabIndex = 2;
            this.lblGMCoal.Text = "Points:";
            // 
            // lblGMLevel
            // 
            this.lblGMLevel.AutoSize = true;
            this.lblGMLevel.Location = new System.Drawing.Point(6, 16);
            this.lblGMLevel.Name = "lblGMLevel";
            this.lblGMLevel.Size = new System.Drawing.Size(36, 13);
            this.lblGMLevel.TabIndex = 1;
            this.lblGMLevel.Text = "Level:";
            // 
            // tabPageSG
            // 
            this.tabPageSG.Controls.Add(this.btnSGExit);
            this.tabPageSG.Controls.Add(this.cbSGEnable);
            this.tabPageSG.Controls.Add(this.linkLabelSG);
            this.tabPageSG.Controls.Add(this.pbSGReload);
            this.tabPageSG.Controls.Add(this.lblSGStatus);
            this.tabPageSG.Controls.Add(this.lblSGPoints);
            this.tabPageSG.Controls.Add(this.lblSGLevel);
            this.tabPageSG.Controls.Add(this.btnSGLogin);
            this.tabPageSG.Location = new System.Drawing.Point(4, 40);
            this.tabPageSG.Name = "tabPageSG";
            this.tabPageSG.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSG.Size = new System.Drawing.Size(334, 61);
            this.tabPageSG.TabIndex = 1;
            this.tabPageSG.Text = "SG";
            this.tabPageSG.UseVisualStyleBackColor = true;
            // 
            // btnSGExit
            // 
            this.btnSGExit.Location = new System.Drawing.Point(134, 19);
            this.btnSGExit.Name = "btnSGExit";
            this.btnSGExit.Size = new System.Drawing.Size(75, 23);
            this.btnSGExit.TabIndex = 28;
            this.btnSGExit.Text = "Logout";
            this.btnSGExit.UseVisualStyleBackColor = true;
            this.btnSGExit.Click += new System.EventHandler(this.btnSGExit_Click);
            // 
            // cbSGEnable
            // 
            this.cbSGEnable.AutoSize = true;
            this.cbSGEnable.Checked = true;
            this.cbSGEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSGEnable.Location = new System.Drawing.Point(315, 3);
            this.cbSGEnable.Name = "cbSGEnable";
            this.cbSGEnable.Size = new System.Drawing.Size(15, 14);
            this.cbSGEnable.TabIndex = 26;
            this.cbSGEnable.UseVisualStyleBackColor = true;
            this.cbSGEnable.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // linkLabelSG
            // 
            this.linkLabelSG.AutoSize = true;
            this.linkLabelSG.Location = new System.Drawing.Point(6, 42);
            this.linkLabelSG.Name = "linkLabelSG";
            this.linkLabelSG.Size = new System.Drawing.Size(59, 13);
            this.linkLabelSG.TabIndex = 12;
            this.linkLabelSG.TabStop = true;
            this.linkLabelSG.Text = "To website";
            this.linkLabelSG.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // pbSGReload
            // 
            this.pbSGReload.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.refresh;
            this.pbSGReload.Location = new System.Drawing.Point(314, 42);
            this.pbSGReload.Name = "pbSGReload";
            this.pbSGReload.Size = new System.Drawing.Size(16, 16);
            this.pbSGReload.TabIndex = 11;
            this.pbSGReload.TabStop = false;
            this.pbSGReload.Click += new System.EventHandler(this.pbSGReload_Click);
            // 
            // lblSGStatus
            // 
            this.lblSGStatus.AutoSize = true;
            this.lblSGStatus.Location = new System.Drawing.Point(6, 3);
            this.lblSGStatus.Name = "lblSGStatus";
            this.lblSGStatus.Size = new System.Drawing.Size(112, 13);
            this.lblSGStatus.TabIndex = 9;
            this.lblSGStatus.Text = "Status: Not authorized";
            // 
            // lblSGPoints
            // 
            this.lblSGPoints.AutoSize = true;
            this.lblSGPoints.Location = new System.Drawing.Point(6, 29);
            this.lblSGPoints.Name = "lblSGPoints";
            this.lblSGPoints.Size = new System.Drawing.Size(39, 13);
            this.lblSGPoints.TabIndex = 8;
            this.lblSGPoints.Text = "Points:";
            // 
            // lblSGLevel
            // 
            this.lblSGLevel.AutoSize = true;
            this.lblSGLevel.Location = new System.Drawing.Point(6, 16);
            this.lblSGLevel.Name = "lblSGLevel";
            this.lblSGLevel.Size = new System.Drawing.Size(36, 13);
            this.lblSGLevel.TabIndex = 7;
            this.lblSGLevel.Text = "Level:";
            // 
            // btnSGLogin
            // 
            this.btnSGLogin.Location = new System.Drawing.Point(134, 19);
            this.btnSGLogin.Name = "btnSGLogin";
            this.btnSGLogin.Size = new System.Drawing.Size(75, 23);
            this.btnSGLogin.TabIndex = 6;
            this.btnSGLogin.Text = "Login";
            this.btnSGLogin.UseVisualStyleBackColor = true;
            this.btnSGLogin.Click += new System.EventHandler(this.btnSGLogin_Click);
            // 
            // tabPageSC
            // 
            this.tabPageSC.Controls.Add(this.btnSCExit);
            this.tabPageSC.Controls.Add(this.cbSCEnable);
            this.tabPageSC.Controls.Add(this.linkLabelSC);
            this.tabPageSC.Controls.Add(this.pbSCReload);
            this.tabPageSC.Controls.Add(this.lblSCStatus);
            this.tabPageSC.Controls.Add(this.lblSCPoints);
            this.tabPageSC.Controls.Add(this.lblSCLevel);
            this.tabPageSC.Controls.Add(this.btnSCLogin);
            this.tabPageSC.Location = new System.Drawing.Point(4, 40);
            this.tabPageSC.Name = "tabPageSC";
            this.tabPageSC.Size = new System.Drawing.Size(334, 61);
            this.tabPageSC.TabIndex = 2;
            this.tabPageSC.Text = "SC";
            this.tabPageSC.UseVisualStyleBackColor = true;
            // 
            // btnSCExit
            // 
            this.btnSCExit.Location = new System.Drawing.Point(134, 19);
            this.btnSCExit.Name = "btnSCExit";
            this.btnSCExit.Size = new System.Drawing.Size(75, 23);
            this.btnSCExit.TabIndex = 29;
            this.btnSCExit.Text = "Logout";
            this.btnSCExit.UseVisualStyleBackColor = true;
            this.btnSCExit.Click += new System.EventHandler(this.btnSCExit_Click);
            // 
            // cbSCEnable
            // 
            this.cbSCEnable.AutoSize = true;
            this.cbSCEnable.Checked = true;
            this.cbSCEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSCEnable.Location = new System.Drawing.Point(315, 3);
            this.cbSCEnable.Name = "cbSCEnable";
            this.cbSCEnable.Size = new System.Drawing.Size(15, 14);
            this.cbSCEnable.TabIndex = 26;
            this.cbSCEnable.UseVisualStyleBackColor = true;
            this.cbSCEnable.CheckedChanged += new System.EventHandler(this.cbSCEnable_CheckedChanged);
            // 
            // linkLabelSC
            // 
            this.linkLabelSC.AutoSize = true;
            this.linkLabelSC.Location = new System.Drawing.Point(6, 42);
            this.linkLabelSC.Name = "linkLabelSC";
            this.linkLabelSC.Size = new System.Drawing.Size(59, 13);
            this.linkLabelSC.TabIndex = 16;
            this.linkLabelSC.TabStop = true;
            this.linkLabelSC.Text = "To website";
            this.linkLabelSC.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel3_LinkClicked);
            // 
            // pbSCReload
            // 
            this.pbSCReload.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.refresh;
            this.pbSCReload.Location = new System.Drawing.Point(314, 42);
            this.pbSCReload.Name = "pbSCReload";
            this.pbSCReload.Size = new System.Drawing.Size(16, 16);
            this.pbSCReload.TabIndex = 15;
            this.pbSCReload.TabStop = false;
            this.pbSCReload.Click += new System.EventHandler(this.pbSCReload_Click);
            // 
            // lblSCStatus
            // 
            this.lblSCStatus.AutoSize = true;
            this.lblSCStatus.Location = new System.Drawing.Point(6, 3);
            this.lblSCStatus.Name = "lblSCStatus";
            this.lblSCStatus.Size = new System.Drawing.Size(112, 13);
            this.lblSCStatus.TabIndex = 13;
            this.lblSCStatus.Text = "Status: Not authorized";
            // 
            // lblSCPoints
            // 
            this.lblSCPoints.AutoSize = true;
            this.lblSCPoints.Location = new System.Drawing.Point(6, 29);
            this.lblSCPoints.Name = "lblSCPoints";
            this.lblSCPoints.Size = new System.Drawing.Size(39, 13);
            this.lblSCPoints.TabIndex = 12;
            this.lblSCPoints.Text = "Points:";
            // 
            // lblSCLevel
            // 
            this.lblSCLevel.AutoSize = true;
            this.lblSCLevel.Location = new System.Drawing.Point(6, 16);
            this.lblSCLevel.Name = "lblSCLevel";
            this.lblSCLevel.Size = new System.Drawing.Size(36, 13);
            this.lblSCLevel.TabIndex = 11;
            this.lblSCLevel.Text = "Level:";
            // 
            // btnSCLogin
            // 
            this.btnSCLogin.Location = new System.Drawing.Point(134, 19);
            this.btnSCLogin.Name = "btnSCLogin";
            this.btnSCLogin.Size = new System.Drawing.Size(75, 23);
            this.btnSCLogin.TabIndex = 7;
            this.btnSCLogin.Text = "Login";
            this.btnSCLogin.UseVisualStyleBackColor = true;
            this.btnSCLogin.Click += new System.EventHandler(this.btnSCLogin_Click);
            // 
            // tabPageSP
            // 
            this.tabPageSP.Controls.Add(this.btnSPExit);
            this.tabPageSP.Controls.Add(this.cbSPEnable);
            this.tabPageSP.Controls.Add(this.linkLabelSP);
            this.tabPageSP.Controls.Add(this.pbUSPeload);
            this.tabPageSP.Controls.Add(this.lblSPStatus);
            this.tabPageSP.Controls.Add(this.lblSPPoints);
            this.tabPageSP.Controls.Add(this.lblSPLevel);
            this.tabPageSP.Controls.Add(this.btnSPLogin);
            this.tabPageSP.Location = new System.Drawing.Point(4, 40);
            this.tabPageSP.Name = "tabPageSP";
            this.tabPageSP.Size = new System.Drawing.Size(334, 61);
            this.tabPageSP.TabIndex = 4;
            this.tabPageSP.Text = "SP";
            this.tabPageSP.UseVisualStyleBackColor = true;
            // 
            // btnSPExit
            // 
            this.btnSPExit.Location = new System.Drawing.Point(134, 19);
            this.btnSPExit.Name = "btnSPExit";
            this.btnSPExit.Size = new System.Drawing.Size(75, 23);
            this.btnSPExit.TabIndex = 29;
            this.btnSPExit.Text = "Logout";
            this.btnSPExit.UseVisualStyleBackColor = true;
            this.btnSPExit.Click += new System.EventHandler(this.btnSPExit_Click);
            // 
            // cbSPEnable
            // 
            this.cbSPEnable.AutoSize = true;
            this.cbSPEnable.Checked = true;
            this.cbSPEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSPEnable.Location = new System.Drawing.Point(315, 3);
            this.cbSPEnable.Name = "cbSPEnable";
            this.cbSPEnable.Size = new System.Drawing.Size(15, 14);
            this.cbSPEnable.TabIndex = 26;
            this.cbSPEnable.UseVisualStyleBackColor = true;
            this.cbSPEnable.CheckedChanged += new System.EventHandler(this.cbSPEnable_CheckedChanged);
            // 
            // linkLabelSP
            // 
            this.linkLabelSP.AutoSize = true;
            this.linkLabelSP.Location = new System.Drawing.Point(6, 42);
            this.linkLabelSP.Name = "linkLabelSP";
            this.linkLabelSP.Size = new System.Drawing.Size(59, 13);
            this.linkLabelSP.TabIndex = 20;
            this.linkLabelSP.TabStop = true;
            this.linkLabelSP.Text = "To website";
            this.linkLabelSP.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel4_LinkClicked);
            // 
            // pbUSPeload
            // 
            this.pbUSPeload.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.refresh;
            this.pbUSPeload.Location = new System.Drawing.Point(314, 42);
            this.pbUSPeload.Name = "pbUSPeload";
            this.pbUSPeload.Size = new System.Drawing.Size(16, 16);
            this.pbUSPeload.TabIndex = 19;
            this.pbUSPeload.TabStop = false;
            this.pbUSPeload.Click += new System.EventHandler(this.pbSPReload_Click);
            // 
            // lblSPStatus
            // 
            this.lblSPStatus.AutoSize = true;
            this.lblSPStatus.Location = new System.Drawing.Point(6, 3);
            this.lblSPStatus.Name = "lblSPStatus";
            this.lblSPStatus.Size = new System.Drawing.Size(112, 13);
            this.lblSPStatus.TabIndex = 17;
            this.lblSPStatus.Text = "Status: Not authorized";
            // 
            // lblSPPoints
            // 
            this.lblSPPoints.AutoSize = true;
            this.lblSPPoints.Location = new System.Drawing.Point(6, 29);
            this.lblSPPoints.Name = "lblSPPoints";
            this.lblSPPoints.Size = new System.Drawing.Size(39, 13);
            this.lblSPPoints.TabIndex = 16;
            this.lblSPPoints.Text = "Points:";
            // 
            // lblSPLevel
            // 
            this.lblSPLevel.AutoSize = true;
            this.lblSPLevel.Location = new System.Drawing.Point(6, 16);
            this.lblSPLevel.Name = "lblSPLevel";
            this.lblSPLevel.Size = new System.Drawing.Size(36, 13);
            this.lblSPLevel.TabIndex = 15;
            this.lblSPLevel.Text = "Level:";
            // 
            // btnSPLogin
            // 
            this.btnSPLogin.Location = new System.Drawing.Point(134, 19);
            this.btnSPLogin.Name = "btnSPLogin";
            this.btnSPLogin.Size = new System.Drawing.Size(75, 23);
            this.btnSPLogin.TabIndex = 9;
            this.btnSPLogin.Text = "Login";
            this.btnSPLogin.UseVisualStyleBackColor = true;
            this.btnSPLogin.Click += new System.EventHandler(this.btnSPLogin_Click);
            // 
            // tabPageST
            // 
            this.tabPageST.Controls.Add(this.btnSTExit);
            this.tabPageST.Controls.Add(this.cbSTEnable);
            this.tabPageST.Controls.Add(this.linkLabelST);
            this.tabPageST.Controls.Add(this.pbSTreload);
            this.tabPageST.Controls.Add(this.lblSTStatus);
            this.tabPageST.Controls.Add(this.lblSTPoints);
            this.tabPageST.Controls.Add(this.lblSTLevel);
            this.tabPageST.Controls.Add(this.btnSTLogin);
            this.tabPageST.Location = new System.Drawing.Point(4, 40);
            this.tabPageST.Name = "tabPageST";
            this.tabPageST.Size = new System.Drawing.Size(334, 61);
            this.tabPageST.TabIndex = 3;
            this.tabPageST.Text = "ST";
            this.tabPageST.UseVisualStyleBackColor = true;
            // 
            // btnSTExit
            // 
            this.btnSTExit.Location = new System.Drawing.Point(134, 19);
            this.btnSTExit.Name = "btnSTExit";
            this.btnSTExit.Size = new System.Drawing.Size(75, 23);
            this.btnSTExit.TabIndex = 29;
            this.btnSTExit.Text = "Logout";
            this.btnSTExit.UseVisualStyleBackColor = true;
            this.btnSTExit.Click += new System.EventHandler(this.btnSTExit_Click);
            // 
            // cbSTEnable
            // 
            this.cbSTEnable.AutoSize = true;
            this.cbSTEnable.Checked = true;
            this.cbSTEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSTEnable.Location = new System.Drawing.Point(315, 3);
            this.cbSTEnable.Name = "cbSTEnable";
            this.cbSTEnable.Size = new System.Drawing.Size(15, 14);
            this.cbSTEnable.TabIndex = 25;
            this.cbSTEnable.UseVisualStyleBackColor = true;
            this.cbSTEnable.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // linkLabelST
            // 
            this.linkLabelST.AutoSize = true;
            this.linkLabelST.Location = new System.Drawing.Point(6, 42);
            this.linkLabelST.Name = "linkLabelST";
            this.linkLabelST.Size = new System.Drawing.Size(59, 13);
            this.linkLabelST.TabIndex = 24;
            this.linkLabelST.TabStop = true;
            this.linkLabelST.Text = "To website";
            this.linkLabelST.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel5_LinkClicked);
            // 
            // pbSTreload
            // 
            this.pbSTreload.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.refresh;
            this.pbSTreload.Location = new System.Drawing.Point(314, 42);
            this.pbSTreload.Name = "pbSTreload";
            this.pbSTreload.Size = new System.Drawing.Size(16, 16);
            this.pbSTreload.TabIndex = 23;
            this.pbSTreload.TabStop = false;
            this.pbSTreload.Click += new System.EventHandler(this.pbSTreload_Click);
            // 
            // lblSTStatus
            // 
            this.lblSTStatus.AutoSize = true;
            this.lblSTStatus.Location = new System.Drawing.Point(6, 3);
            this.lblSTStatus.Name = "lblSTStatus";
            this.lblSTStatus.Size = new System.Drawing.Size(112, 13);
            this.lblSTStatus.TabIndex = 21;
            this.lblSTStatus.Text = "Status: Not authorized";
            // 
            // lblSTPoints
            // 
            this.lblSTPoints.AutoSize = true;
            this.lblSTPoints.Location = new System.Drawing.Point(6, 29);
            this.lblSTPoints.Name = "lblSTPoints";
            this.lblSTPoints.Size = new System.Drawing.Size(39, 13);
            this.lblSTPoints.TabIndex = 20;
            this.lblSTPoints.Text = "Points:";
            // 
            // lblSTLevel
            // 
            this.lblSTLevel.AutoSize = true;
            this.lblSTLevel.Location = new System.Drawing.Point(6, 16);
            this.lblSTLevel.Name = "lblSTLevel";
            this.lblSTLevel.Size = new System.Drawing.Size(36, 13);
            this.lblSTLevel.TabIndex = 19;
            this.lblSTLevel.Text = "Level:";
            // 
            // btnSTLogin
            // 
            this.btnSTLogin.Location = new System.Drawing.Point(134, 19);
            this.btnSTLogin.Name = "btnSTLogin";
            this.btnSTLogin.Size = new System.Drawing.Size(75, 23);
            this.btnSTLogin.TabIndex = 8;
            this.btnSTLogin.Text = "Login";
            this.btnSTLogin.UseVisualStyleBackColor = true;
            this.btnSTLogin.Click += new System.EventHandler(this.btnSTLogin_Click);
            // 
            // tabPagePB
            // 
            this.tabPagePB.Controls.Add(this.btnPBExit);
            this.tabPagePB.Controls.Add(this.btnPBLogin);
            this.tabPagePB.Controls.Add(this.pbPBRefresh);
            this.tabPagePB.Controls.Add(this.cbPBEnabled);
            this.tabPagePB.Controls.Add(this.linkLabelPB);
            this.tabPagePB.Controls.Add(this.lblPBStatus);
            this.tabPagePB.Controls.Add(this.lblPBPoints);
            this.tabPagePB.Controls.Add(this.lblPBLevel);
            this.tabPagePB.Location = new System.Drawing.Point(4, 40);
            this.tabPagePB.Name = "tabPagePB";
            this.tabPagePB.Size = new System.Drawing.Size(334, 61);
            this.tabPagePB.TabIndex = 6;
            this.tabPagePB.Text = "PB";
            this.tabPagePB.UseVisualStyleBackColor = true;
            // 
            // btnPBExit
            // 
            this.btnPBExit.Location = new System.Drawing.Point(134, 19);
            this.btnPBExit.Name = "btnPBExit";
            this.btnPBExit.Size = new System.Drawing.Size(75, 23);
            this.btnPBExit.TabIndex = 32;
            this.btnPBExit.Text = "Logout";
            this.btnPBExit.UseVisualStyleBackColor = true;
            this.btnPBExit.Click += new System.EventHandler(this.btnPBExit_Click);
            // 
            // btnPBLogin
            // 
            this.btnPBLogin.Location = new System.Drawing.Point(134, 19);
            this.btnPBLogin.Name = "btnPBLogin";
            this.btnPBLogin.Size = new System.Drawing.Size(75, 23);
            this.btnPBLogin.TabIndex = 31;
            this.btnPBLogin.Text = "Login";
            this.btnPBLogin.UseVisualStyleBackColor = true;
            this.btnPBLogin.Click += new System.EventHandler(this.btnPBLogin_Click);
            // 
            // pbPBRefresh
            // 
            this.pbPBRefresh.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.refresh;
            this.pbPBRefresh.Location = new System.Drawing.Point(314, 42);
            this.pbPBRefresh.Name = "pbPBRefresh";
            this.pbPBRefresh.Size = new System.Drawing.Size(16, 16);
            this.pbPBRefresh.TabIndex = 30;
            this.pbPBRefresh.TabStop = false;
            this.pbPBRefresh.Click += new System.EventHandler(this.pbPBRefresh_Click);
            // 
            // cbPBEnabled
            // 
            this.cbPBEnabled.AutoSize = true;
            this.cbPBEnabled.Checked = true;
            this.cbPBEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbPBEnabled.Location = new System.Drawing.Point(315, 3);
            this.cbPBEnabled.Name = "cbPBEnabled";
            this.cbPBEnabled.Size = new System.Drawing.Size(15, 14);
            this.cbPBEnabled.TabIndex = 29;
            this.cbPBEnabled.UseVisualStyleBackColor = true;
            this.cbPBEnabled.CheckedChanged += new System.EventHandler(this.cbPBEnabled_CheckedChanged);
            // 
            // linkLabelPB
            // 
            this.linkLabelPB.AutoSize = true;
            this.linkLabelPB.Location = new System.Drawing.Point(6, 42);
            this.linkLabelPB.Name = "linkLabelPB";
            this.linkLabelPB.Size = new System.Drawing.Size(59, 13);
            this.linkLabelPB.TabIndex = 28;
            this.linkLabelPB.TabStop = true;
            this.linkLabelPB.Text = "To website";
            this.linkLabelPB.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel7_LinkClicked);
            // 
            // lblPBStatus
            // 
            this.lblPBStatus.AutoSize = true;
            this.lblPBStatus.Location = new System.Drawing.Point(6, 3);
            this.lblPBStatus.Name = "lblPBStatus";
            this.lblPBStatus.Size = new System.Drawing.Size(112, 13);
            this.lblPBStatus.TabIndex = 27;
            this.lblPBStatus.Text = "Status: Not authorized";
            // 
            // lblPBPoints
            // 
            this.lblPBPoints.AutoSize = true;
            this.lblPBPoints.Location = new System.Drawing.Point(6, 29);
            this.lblPBPoints.Name = "lblPBPoints";
            this.lblPBPoints.Size = new System.Drawing.Size(39, 13);
            this.lblPBPoints.TabIndex = 26;
            this.lblPBPoints.Text = "Points:";
            // 
            // lblPBLevel
            // 
            this.lblPBLevel.AutoSize = true;
            this.lblPBLevel.Location = new System.Drawing.Point(6, 16);
            this.lblPBLevel.Name = "lblPBLevel";
            this.lblPBLevel.Size = new System.Drawing.Size(36, 13);
            this.lblPBLevel.TabIndex = 25;
            this.lblPBLevel.Text = "Level:";
            // 
            // tabPageGA
            // 
            this.tabPageGA.Controls.Add(this.pbGARefresh);
            this.tabPageGA.Controls.Add(this.cbGAEnabled);
            this.tabPageGA.Controls.Add(this.btnGAExit);
            this.tabPageGA.Controls.Add(this.btnGALogin);
            this.tabPageGA.Controls.Add(this.linkLabelGA);
            this.tabPageGA.Controls.Add(this.lblGAPoints);
            this.tabPageGA.Controls.Add(this.lblGALevel);
            this.tabPageGA.Controls.Add(this.lblGAStatus);
            this.tabPageGA.Location = new System.Drawing.Point(4, 40);
            this.tabPageGA.Name = "tabPageGA";
            this.tabPageGA.Size = new System.Drawing.Size(334, 61);
            this.tabPageGA.TabIndex = 7;
            this.tabPageGA.Text = "GA";
            this.tabPageGA.UseVisualStyleBackColor = true;
            // 
            // pbGARefresh
            // 
            this.pbGARefresh.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.refresh;
            this.pbGARefresh.Location = new System.Drawing.Point(314, 42);
            this.pbGARefresh.Name = "pbGARefresh";
            this.pbGARefresh.Size = new System.Drawing.Size(16, 16);
            this.pbGARefresh.TabIndex = 35;
            this.pbGARefresh.TabStop = false;
            // 
            // cbGAEnabled
            // 
            this.cbGAEnabled.AutoSize = true;
            this.cbGAEnabled.Checked = true;
            this.cbGAEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbGAEnabled.Location = new System.Drawing.Point(315, 3);
            this.cbGAEnabled.Name = "cbGAEnabled";
            this.cbGAEnabled.Size = new System.Drawing.Size(15, 14);
            this.cbGAEnabled.TabIndex = 34;
            this.cbGAEnabled.UseVisualStyleBackColor = true;
            this.cbGAEnabled.CheckedChanged += new System.EventHandler(this.cbGAEnabled_CheckedChanged);
            // 
            // btnGAExit
            // 
            this.btnGAExit.Location = new System.Drawing.Point(134, 19);
            this.btnGAExit.Name = "btnGAExit";
            this.btnGAExit.Size = new System.Drawing.Size(75, 23);
            this.btnGAExit.TabIndex = 33;
            this.btnGAExit.Text = "Logout";
            this.btnGAExit.UseVisualStyleBackColor = true;
            this.btnGAExit.Click += new System.EventHandler(this.buttonExitGA_Click);
            // 
            // btnGALogin
            // 
            this.btnGALogin.Location = new System.Drawing.Point(134, 19);
            this.btnGALogin.Name = "btnGALogin";
            this.btnGALogin.Size = new System.Drawing.Size(75, 23);
            this.btnGALogin.TabIndex = 32;
            this.btnGALogin.Text = "Login";
            this.btnGALogin.UseVisualStyleBackColor = true;
            this.btnGALogin.Click += new System.EventHandler(this.buttonLoginGA_Click);
            // 
            // linkLabelGA
            // 
            this.linkLabelGA.AutoSize = true;
            this.linkLabelGA.Location = new System.Drawing.Point(6, 42);
            this.linkLabelGA.Name = "linkLabelGA";
            this.linkLabelGA.Size = new System.Drawing.Size(59, 13);
            this.linkLabelGA.TabIndex = 31;
            this.linkLabelGA.TabStop = true;
            this.linkLabelGA.Text = "To website";
            this.linkLabelGA.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelGA_LinkClicked);
            // 
            // lblGAPoints
            // 
            this.lblGAPoints.AutoSize = true;
            this.lblGAPoints.Location = new System.Drawing.Point(6, 29);
            this.lblGAPoints.Name = "lblGAPoints";
            this.lblGAPoints.Size = new System.Drawing.Size(39, 13);
            this.lblGAPoints.TabIndex = 30;
            this.lblGAPoints.Text = "Points:";
            // 
            // lblGALevel
            // 
            this.lblGALevel.AutoSize = true;
            this.lblGALevel.Location = new System.Drawing.Point(6, 16);
            this.lblGALevel.Name = "lblGALevel";
            this.lblGALevel.Size = new System.Drawing.Size(36, 13);
            this.lblGALevel.TabIndex = 29;
            this.lblGALevel.Text = "Level:";
            // 
            // lblGAStatus
            // 
            this.lblGAStatus.AutoSize = true;
            this.lblGAStatus.Location = new System.Drawing.Point(6, 3);
            this.lblGAStatus.Name = "lblGAStatus";
            this.lblGAStatus.Size = new System.Drawing.Size(112, 13);
            this.lblGAStatus.TabIndex = 28;
            this.lblGAStatus.Text = "Status: Not authorized";
            // 
            // tabPageIG
            // 
            this.tabPageIG.Controls.Add(this.btnIGLogout);
            this.tabPageIG.Controls.Add(this.cbIGEnabled);
            this.tabPageIG.Controls.Add(this.linkLabelIG);
            this.tabPageIG.Controls.Add(this.pbIGRefresh);
            this.tabPageIG.Controls.Add(this.btnIGLogin);
            this.tabPageIG.Controls.Add(this.lblIGStatus);
            this.tabPageIG.Controls.Add(this.lblIGPoints);
            this.tabPageIG.Controls.Add(this.lblIGLevel);
            this.tabPageIG.Location = new System.Drawing.Point(4, 40);
            this.tabPageIG.Name = "tabPageIG";
            this.tabPageIG.Size = new System.Drawing.Size(334, 61);
            this.tabPageIG.TabIndex = 8;
            this.tabPageIG.Text = "IG";
            this.tabPageIG.UseVisualStyleBackColor = true;
            // 
            // btnIGLogout
            // 
            this.btnIGLogout.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnIGLogout.Location = new System.Drawing.Point(134, 19);
            this.btnIGLogout.Name = "btnIGLogout";
            this.btnIGLogout.Size = new System.Drawing.Size(75, 23);
            this.btnIGLogout.TabIndex = 35;
            this.btnIGLogout.Text = "Logout";
            this.btnIGLogout.UseVisualStyleBackColor = true;
            this.btnIGLogout.Click += new System.EventHandler(this.btnIGLogout_Click);
            // 
            // cbIGEnabled
            // 
            this.cbIGEnabled.AutoSize = true;
            this.cbIGEnabled.Checked = true;
            this.cbIGEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIGEnabled.Location = new System.Drawing.Point(315, 3);
            this.cbIGEnabled.Name = "cbIGEnabled";
            this.cbIGEnabled.Size = new System.Drawing.Size(15, 14);
            this.cbIGEnabled.TabIndex = 34;
            this.cbIGEnabled.UseVisualStyleBackColor = true;
            // 
            // linkLabelIG
            // 
            this.linkLabelIG.AutoSize = true;
            this.linkLabelIG.Location = new System.Drawing.Point(6, 42);
            this.linkLabelIG.Name = "linkLabelIG";
            this.linkLabelIG.Size = new System.Drawing.Size(59, 13);
            this.linkLabelIG.TabIndex = 33;
            this.linkLabelIG.TabStop = true;
            this.linkLabelIG.Text = "To website";
            this.linkLabelIG.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelIG_LinkClicked);
            // 
            // pbIGRefresh
            // 
            this.pbIGRefresh.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.refresh;
            this.pbIGRefresh.Location = new System.Drawing.Point(314, 42);
            this.pbIGRefresh.Name = "pbIGRefresh";
            this.pbIGRefresh.Size = new System.Drawing.Size(16, 16);
            this.pbIGRefresh.TabIndex = 32;
            this.pbIGRefresh.TabStop = false;
            // 
            // btnIGLogin
            // 
            this.btnIGLogin.Location = new System.Drawing.Point(134, 19);
            this.btnIGLogin.Name = "btnIGLogin";
            this.btnIGLogin.Size = new System.Drawing.Size(75, 23);
            this.btnIGLogin.TabIndex = 31;
            this.btnIGLogin.Text = "Login";
            this.btnIGLogin.UseVisualStyleBackColor = true;
            this.btnIGLogin.Click += new System.EventHandler(this.btnIGLogin_Click);
            // 
            // lblIGStatus
            // 
            this.lblIGStatus.AutoSize = true;
            this.lblIGStatus.Location = new System.Drawing.Point(6, 3);
            this.lblIGStatus.Name = "lblIGStatus";
            this.lblIGStatus.Size = new System.Drawing.Size(112, 13);
            this.lblIGStatus.TabIndex = 30;
            this.lblIGStatus.Text = "Status: Not authorized";
            // 
            // lblIGPoints
            // 
            this.lblIGPoints.AutoSize = true;
            this.lblIGPoints.Location = new System.Drawing.Point(6, 29);
            this.lblIGPoints.Name = "lblIGPoints";
            this.lblIGPoints.Size = new System.Drawing.Size(39, 13);
            this.lblIGPoints.TabIndex = 29;
            this.lblIGPoints.Text = "Points:";
            // 
            // lblIGLevel
            // 
            this.lblIGLevel.AutoSize = true;
            this.lblIGLevel.Location = new System.Drawing.Point(6, 16);
            this.lblIGLevel.Name = "lblIGLevel";
            this.lblIGLevel.Size = new System.Drawing.Size(36, 13);
            this.lblIGLevel.TabIndex = 28;
            this.lblIGLevel.Text = "Level:";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(4, 134);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(342, 67);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.toolStripMenuItem_Main;
            this.notifyIcon.Icon = global::KryBot.Gui.WinFormsGui.Properties.Resources.KryBotPresent_256b;
            this.notifyIcon.Text = "KryBot";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // toolStripMenuItem_Main
            // 
            this.toolStripMenuItem_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem_Show,
            this.toolStripMenuItem_Farm,
            this.toolStripMenuItem_Exit});
            this.toolStripMenuItem_Main.Name = "toolStripMenuItem_Main";
            this.toolStripMenuItem_Main.Size = new System.Drawing.Size(113, 70);
            // 
            // toolStripMenuItem_Show
            // 
            this.toolStripMenuItem_Show.Name = "toolStripMenuItem_Show";
            this.toolStripMenuItem_Show.Size = new System.Drawing.Size(112, 22);
            this.toolStripMenuItem_Show.Text = "Show";
            this.toolStripMenuItem_Show.Click += new System.EventHandler(this.toolStripMenuItem_Show_Click);
            // 
            // toolStripMenuItem_Farm
            // 
            this.toolStripMenuItem_Farm.Name = "toolStripMenuItem_Farm";
            this.toolStripMenuItem_Farm.Size = new System.Drawing.Size(112, 22);
            this.toolStripMenuItem_Farm.Text = "Farm";
            this.toolStripMenuItem_Farm.Click += new System.EventHandler(this.toolStripMenuItem_Farm_Click);
            // 
            // toolStripMenuItem_Exit
            // 
            this.toolStripMenuItem_Exit.Name = "toolStripMenuItem_Exit";
            this.toolStripMenuItem_Exit.Size = new System.Drawing.Size(112, 22);
            this.toolStripMenuItem_Exit.Text = "Logout";
            this.toolStripMenuItem_Exit.Click += new System.EventHandler(this.toolStripMenuItem_Exit_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(350, 227);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = global::KryBot.Gui.WinFormsGui.Properties.Resources.KryBotPresent_256b;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "KryBot";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.LocationChanged += new System.EventHandler(this.FormMain_LocationChanged);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.tabPageSteam.ResumeLayout(false);
            this.tabPageSteam.PerformLayout();
            this.tabPageGM.ResumeLayout(false);
            this.tabPageGM.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGMReload)).EndInit();
            this.tabPageSG.ResumeLayout(false);
            this.tabPageSG.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSGReload)).EndInit();
            this.tabPageSC.ResumeLayout(false);
            this.tabPageSC.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSCReload)).EndInit();
            this.tabPageSP.ResumeLayout(false);
            this.tabPageSP.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbUSPeload)).EndInit();
            this.tabPageST.ResumeLayout(false);
            this.tabPageST.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSTreload)).EndInit();
            this.tabPagePB.ResumeLayout(false);
            this.tabPagePB.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPBRefresh)).EndInit();
            this.tabPageGA.ResumeLayout(false);
            this.tabPageGA.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGARefresh)).EndInit();
            this.tabPageIG.ResumeLayout(false);
            this.tabPageIG.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIGRefresh)).EndInit();
            this.toolStripMenuItem_Main.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
		private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabPageGM;
		private System.Windows.Forms.TabPage tabPageSG;
		private System.Windows.Forms.TabPage tabPageSC;
		private System.Windows.Forms.TabPage tabPageST;
		private System.Windows.Forms.TabPage tabPageSP;
		private System.Windows.Forms.ToolStripMenuItem logToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem informationToolStripMenuItem;
		private System.Windows.Forms.Label lblGMStatus;
		private System.Windows.Forms.Label lblGMCoal;
		private System.Windows.Forms.Label lblGMLevel;
		private System.Windows.Forms.Button btnStart;
		private System.Windows.Forms.Button btnGMLogin;
		private System.Windows.Forms.Button btnSGLogin;
		private System.Windows.Forms.Button btnSCLogin;
		private System.Windows.Forms.Button btnSTLogin;
		private System.Windows.Forms.Button btnSPLogin;
		private System.Windows.Forms.Label lblSGStatus;
		private System.Windows.Forms.Label lblSGPoints;
		private System.Windows.Forms.Label lblSGLevel;
		private System.Windows.Forms.Label lblSCStatus;
		private System.Windows.Forms.Label lblSCPoints;
		private System.Windows.Forms.Label lblSCLevel;
		private System.Windows.Forms.Label lblSPStatus;
		private System.Windows.Forms.Label lblSPPoints;
		private System.Windows.Forms.Label lblSPLevel;
		private System.Windows.Forms.Label lblSTStatus;
		private System.Windows.Forms.Label lblSTPoints;
		private System.Windows.Forms.Label lblSTLevel;
		private System.Windows.Forms.PictureBox pbGMReload;
		private System.Windows.Forms.PictureBox pbSGReload;
		private System.Windows.Forms.PictureBox pbSCReload;
		private System.Windows.Forms.PictureBox pbUSPeload;
		private System.Windows.Forms.PictureBox pbSTreload;
		private System.Windows.Forms.NotifyIcon notifyIcon;
		private System.Windows.Forms.LinkLabel linkLabelGM;
		private System.Windows.Forms.LinkLabel linkLabelSG;
		private System.Windows.Forms.LinkLabel linkLabelSC;
		private System.Windows.Forms.LinkLabel linkLabelSP;
		private System.Windows.Forms.LinkLabel linkLabelST;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsКакToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.CheckBox cbSTEnable;
		private System.Windows.Forms.CheckBox cbGMEnable;
		private System.Windows.Forms.CheckBox cbSGEnable;
		private System.Windows.Forms.CheckBox cbSCEnable;
		private System.Windows.Forms.CheckBox cbSPEnable;
		private System.Windows.Forms.TabPage tabPageSteam;
		private System.Windows.Forms.LinkLabel linkLabelSteam;
		private System.Windows.Forms.Label lblSteamStatus;
		private System.Windows.Forms.Button btnSteamLogin;
		private System.Windows.Forms.ContextMenuStrip toolStripMenuItem_Main;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Show;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Farm;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem_Exit;
		private System.Windows.Forms.ToolStripMenuItem donateToolStripMenuItem;
		private System.Windows.Forms.Button btnGMExit;
		private System.Windows.Forms.Button btnSGExit;
		private System.Windows.Forms.Button btnSCExit;
		private System.Windows.Forms.Button btnSPExit;
		private System.Windows.Forms.Button btnSTExit;
		private System.Windows.Forms.Button btnSteamExit;
		private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem blacklistToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
		private System.Windows.Forms.TabPage tabPagePB;
		private System.Windows.Forms.Button btnPBExit;
		private System.Windows.Forms.Button btnPBLogin;
		private System.Windows.Forms.PictureBox pbPBRefresh;
		private System.Windows.Forms.CheckBox cbPBEnabled;
		private System.Windows.Forms.LinkLabel linkLabelPB;
		private System.Windows.Forms.Label lblPBStatus;
		private System.Windows.Forms.Label lblPBPoints;
		private System.Windows.Forms.Label lblPBLevel;
		private System.Windows.Forms.TabPage tabPageGA;
		private System.Windows.Forms.PictureBox pbGARefresh;
		private System.Windows.Forms.CheckBox cbGAEnabled;
		private System.Windows.Forms.Button btnGAExit;
		private System.Windows.Forms.Button btnGALogin;
		private System.Windows.Forms.LinkLabel linkLabelGA;
		private System.Windows.Forms.Label lblGAPoints;
		private System.Windows.Forms.Label lblGALevel;
		private System.Windows.Forms.Label lblGAStatus;

		private void Localization()
		{
			toolStripStatusLabel.Text = strings.ProfileLoading;
			fileToolStripMenuItem.Text = strings.File;
			openFolderToolStripMenuItem.Text = strings.OpenBotFolder;
			saveToolStripMenuItem.Text = strings.Save;
			saveAsКакToolStripMenuItem.Text = strings.SaveAs;
			exitToolStripMenuItem.Text = strings.Exit;
			toolsToolStripMenuItem.Text = strings.Tools;
			settingsToolStripMenuItem.Text = strings.Settings;
			blacklistToolStripMenuItem.Text = strings.Blacklist;
			informationToolStripMenuItem.Text = strings.Information;
			aboutToolStripMenuItem.Text = strings.AboutProgram;
			donateToolStripMenuItem.Text = strings.MainForm_Donate;
			logToolStripMenuItem.Text = $"{strings.Log} <<";
			btnGMExit.Text = strings.Logout;
			linkLabelGM.Text = strings.LinkLabel_OnSite;
			btnGMLogin.Text = strings.Login;
			lblGMStatus.Text = $"{strings.Status}: {strings.Status_NotLogined}";
			lblGMCoal.Text = $"{strings.Points}: ";
			lblGMLevel.Text = $"{strings.Level}: ";
			btnSGExit.Text = strings.Logout;
			linkLabelSG.Text = strings.LinkLabel_OnSite;
			lblSGStatus.Text = $"{strings.Status}: {strings.Status_NotLogined}";
			lblSGPoints.Text = $"{strings.Points}: ";
			lblSGLevel.Text = $"{strings.Level}: ";
			btnSGLogin.Text = strings.Login;
			btnSCExit.Text = strings.Logout;
			linkLabelSC.Text = strings.LinkLabel_OnSite;
			lblSCStatus.Text = $"{strings.Status}: {strings.Status_NotLogined}";
			lblSCPoints.Text = $"{strings.Points}: ";
			lblSCLevel.Text = $"{strings.Level}: ";
			btnSCLogin.Text = strings.Login;
			btnSPExit.Text = strings.Logout;
			linkLabelSP.Text = strings.LinkLabel_OnSite;
			lblSPStatus.Text = $"{strings.Status}: {strings.Status_NotLogined}";
			lblSPPoints.Text = $"{strings.Points}: ";
			lblSPLevel.Text = $"{strings.Level}: ";
			btnSPLogin.Text = strings.Login;
			btnSTExit.Text = strings.Logout;
			linkLabelST.Text = strings.LinkLabel_OnSite;
			lblSTStatus.Text = $"{strings.Status}: {strings.Status_NotLogined}";
			lblSTPoints.Text = $"{strings.Points}: ";
			lblSTLevel.Text = $"{strings.Level}: ";
			btnSTLogin.Text = strings.Login;
			btnPBExit.Text = strings.Logout;
			btnPBLogin.Text = strings.Login;
			linkLabelPB.Text = strings.LinkLabel_OnSite;
			lblPBStatus.Text = $"{strings.Status}: {strings.Status_NotLogined}";
			lblPBPoints.Text = $"{strings.Points}: ";
			lblPBLevel.Text = $"{strings.Level}: ";
			btnGAExit.Text = strings.Logout;
			btnGALogin.Text = strings.Login;
			linkLabelGA.Text = strings.LinkLabel_OnSite;
			lblGAPoints.Text = $"{strings.Points}: ";
			lblGALevel.Text = $"{strings.Level}: ";
			lblGAStatus.Text = $"{ strings.Status}: { strings.Status_NotLogined}";
			btnSteamExit.Text = strings.Logout;
			btnSteamLogin.Text = strings.Login;
			linkLabelSteam.Text = strings.LinkLabel_OnSite;
			lblSteamStatus.Text = $"{strings.Status}: {strings.Status_NotLogined}";
			btnStart.Text = strings.Start;
			toolStripMenuItem_Show.Text = strings.Show;
			toolStripMenuItem_Farm.Text = strings.Farm;
			toolStripMenuItem_Exit.Text = strings.Exit;
			Text = $"{Application.ProductName} [{Application.ProductVersion}]";
            btnIGLogout.Text = strings.Logout;
		    btnIGLogin.Text = strings.Login;
            lblIGPoints.Text = $"{strings.Points}: ";
            lblIGLevel.Text = $"{strings.Level}: ";
            lblIGStatus.Text = $"{ strings.Status}: { strings.Status_NotLogined}";
            linkLabelIG.Text = strings.LinkLabel_OnSite;
        }

        private TabPage tabPageIG;
        private Button btnIGLogout;
        private CheckBox cbIGEnabled;
        private LinkLabel linkLabelIG;
        private PictureBox pbIGRefresh;
        private Button btnIGLogin;
        private Label lblIGStatus;
        private Label lblIGPoints;
        private Label lblIGLevel;
    }
}

