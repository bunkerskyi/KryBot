using System.Drawing;
using KryBot.CommonResources.Localization;
using KryBot.Gui.WinFormsGui.Properties;

namespace KryBot.Gui.WinFormsGui.Forms
{
	partial class FormSettings
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageCommun = new System.Windows.Forms.TabPage();
            this.btnShortcut = new System.Windows.Forms.Button();
            this.gbLang = new System.Windows.Forms.GroupBox();
            this.cbLang = new System.Windows.Forms.ComboBox();
            this.cbWishlistSort = new System.Windows.Forms.CheckBox();
            this.cbFarmTip = new System.Windows.Forms.CheckBox();
            this.cbWonTip = new System.Windows.Forms.CheckBox();
            this.cbAutorun = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.gbTimerSettings = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tbTimerLoops = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbTimerInterval = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbTimerEnable = new System.Windows.Forms.CheckBox();
            this.cbSortBy = new System.Windows.Forms.ComboBox();
            this.cbSort = new System.Windows.Forms.CheckBox();
            this.tabPageGM = new System.Windows.Forms.TabPage();
            this.btbGMCookies = new System.Windows.Forms.Button();
            this.cbGMRegional = new System.Windows.Forms.CheckBox();
            this.cbGMGolden = new System.Windows.Forms.CheckBox();
            this.chGMSandbox = new System.Windows.Forms.CheckBox();
            this.cbGMRegular = new System.Windows.Forms.CheckBox();
            this.tbGMReserv = new System.Windows.Forms.TextBox();
            this.tbGMMaxValue = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageSG = new System.Windows.Forms.TabPage();
            this.cbSGSortTeLessLevel = new System.Windows.Forms.CheckBox();
            this.numSGLevel = new System.Windows.Forms.NumericUpDown();
            this.cbSGMinLevel = new System.Windows.Forms.CheckBox();
            this.cbSGRegular = new System.Windows.Forms.CheckBox();
            this.btnSGCookies = new System.Windows.Forms.Button();
            this.tbSGReserv = new System.Windows.Forms.TextBox();
            this.tbSGMaxValue = new System.Windows.Forms.TextBox();
            this.lblSGReserv = new System.Windows.Forms.Label();
            this.lblSGMaxValue = new System.Windows.Forms.Label();
            this.cbSGGroup = new System.Windows.Forms.CheckBox();
            this.cbSGWishlist = new System.Windows.Forms.CheckBox();
            this.tabPageSC = new System.Windows.Forms.TabPage();
            this.cbSCContributors = new System.Windows.Forms.CheckBox();
            this.cbSCRegular = new System.Windows.Forms.CheckBox();
            this.cbSCAutojoin = new System.Windows.Forms.CheckBox();
            this.btnSCCookies = new System.Windows.Forms.Button();
            this.tbSCReserv = new System.Windows.Forms.TextBox();
            this.tbSCMaxValue = new System.Windows.Forms.TextBox();
            this.lblSCReserv = new System.Windows.Forms.Label();
            this.lblSCMaxValue = new System.Windows.Forms.Label();
            this.cbSCGroup = new System.Windows.Forms.CheckBox();
            this.cbSCWishlist = new System.Windows.Forms.CheckBox();
            this.tabPageUG = new System.Windows.Forms.TabPage();
            this.btnSPCookies = new System.Windows.Forms.Button();
            this.tbSPReserv = new System.Windows.Forms.TextBox();
            this.tbSPMaxValue = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tabPagePB = new System.Windows.Forms.TabPage();
            this.tbPBReserv = new System.Windows.Forms.TextBox();
            this.tbPBMaxValue = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.btnPBCookies = new System.Windows.Forms.Button();
            this.tabPageGA = new System.Windows.Forms.TabPage();
            this.tbGAReserv = new System.Windows.Forms.TextBox();
            this.tbGAMaxBet = new System.Windows.Forms.TextBox();
            this.lblGAReserv = new System.Windows.Forms.Label();
            this.lblGAMaxBet = new System.Windows.Forms.Label();
            this.btnGACookies = new System.Windows.Forms.Button();
            this.tabPageIG = new System.Windows.Forms.TabPage();
            this.tbIGReserv = new System.Windows.Forms.TextBox();
            this.lblIGReserv = new System.Windows.Forms.Label();
            this.tbIGMaxValue = new System.Windows.Forms.TextBox();
            this.lblIGMaxValue = new System.Windows.Forms.Label();
            this.cbIGDota = new System.Windows.Forms.CheckBox();
            this.cbIGCSGO = new System.Windows.Forms.CheckBox();
            this.cbIGTF2 = new System.Windows.Forms.CheckBox();
            this.cbIGSteamItems = new System.Windows.Forms.CheckBox();
            this.cbIGSteamGiveaways = new System.Windows.Forms.CheckBox();
            this.btnIGCookie = new System.Windows.Forms.Button();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl.SuspendLayout();
            this.tabPageCommun.SuspendLayout();
            this.gbLang.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbTimerSettings.SuspendLayout();
            this.tabPageGM.SuspendLayout();
            this.tabPageSG.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSGLevel)).BeginInit();
            this.tabPageSC.SuspendLayout();
            this.tabPageUG.SuspendLayout();
            this.tabPagePB.SuspendLayout();
            this.tabPageGA.SuspendLayout();
            this.tabPageIG.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageCommun);
            this.tabControl.Controls.Add(this.tabPageGM);
            this.tabControl.Controls.Add(this.tabPageSG);
            this.tabControl.Controls.Add(this.tabPageSC);
            this.tabControl.Controls.Add(this.tabPageUG);
            this.tabControl.Controls.Add(this.tabPagePB);
            this.tabControl.Controls.Add(this.tabPageGA);
            this.tabControl.Controls.Add(this.tabPageIG);
            this.tabControl.Location = new System.Drawing.Point(0, 27);
            this.tabControl.Multiline = true;
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(317, 238);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageCommun
            // 
            this.tabPageCommun.Controls.Add(this.btnShortcut);
            this.tabPageCommun.Controls.Add(this.gbLang);
            this.tabPageCommun.Controls.Add(this.cbWishlistSort);
            this.tabPageCommun.Controls.Add(this.cbFarmTip);
            this.tabPageCommun.Controls.Add(this.cbWonTip);
            this.tabPageCommun.Controls.Add(this.cbAutorun);
            this.tabPageCommun.Controls.Add(this.groupBox1);
            this.tabPageCommun.Controls.Add(this.cbSortBy);
            this.tabPageCommun.Controls.Add(this.cbSort);
            this.tabPageCommun.Location = new System.Drawing.Point(4, 40);
            this.tabPageCommun.Name = "tabPageCommun";
            this.tabPageCommun.Size = new System.Drawing.Size(309, 215);
            this.tabPageCommun.TabIndex = 5;
            this.tabPageCommun.Text = "Commun";
            this.tabPageCommun.UseVisualStyleBackColor = true;
            // 
            // btnShortcut
            // 
            this.btnShortcut.Location = new System.Drawing.Point(160, 126);
            this.btnShortcut.Name = "btnShortcut";
            this.btnShortcut.Size = new System.Drawing.Size(141, 23);
            this.btnShortcut.TabIndex = 10;
            this.btnShortcut.Text = "Create shortcut";
            this.btnShortcut.UseVisualStyleBackColor = true;
            this.btnShortcut.Click += new System.EventHandler(this.btnShortcut_Click);
            // 
            // gbLang
            // 
            this.gbLang.Controls.Add(this.cbLang);
            this.gbLang.Location = new System.Drawing.Point(0, 115);
            this.gbLang.Name = "gbLang";
            this.gbLang.Size = new System.Drawing.Size(154, 40);
            this.gbLang.TabIndex = 9;
            this.gbLang.TabStop = false;
            this.gbLang.Text = "Language";
            // 
            // cbLang
            // 
            this.cbLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLang.FormattingEnabled = true;
            this.cbLang.Location = new System.Drawing.Point(6, 13);
            this.cbLang.Name = "cbLang";
            this.cbLang.Size = new System.Drawing.Size(142, 21);
            this.cbLang.TabIndex = 0;
            // 
            // cbWishlistSort
            // 
            this.cbWishlistSort.Location = new System.Drawing.Point(160, 97);
            this.cbWishlistSort.Name = "cbWishlistSort";
            this.cbWishlistSort.Size = new System.Drawing.Size(144, 30);
            this.cbWishlistSort.TabIndex = 8;
            this.cbWishlistSort.Text = "Do not use sorting to wishlist";
            this.cbWishlistSort.UseVisualStyleBackColor = true;
            // 
            // cbFarmTip
            // 
            this.cbFarmTip.AutoSize = true;
            this.cbFarmTip.Location = new System.Drawing.Point(160, 74);
            this.cbFarmTip.Name = "cbFarmTip";
            this.cbFarmTip.Size = new System.Drawing.Size(132, 17);
            this.cbFarmTip.TabIndex = 5;
            this.cbFarmTip.Text = "Notification about farm";
            this.cbFarmTip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbFarmTip.UseVisualStyleBackColor = true;
            // 
            // cbWonTip
            // 
            this.cbWonTip.AutoSize = true;
            this.cbWonTip.Location = new System.Drawing.Point(160, 51);
            this.cbWonTip.Name = "cbWonTip";
            this.cbWonTip.Size = new System.Drawing.Size(130, 17);
            this.cbWonTip.TabIndex = 4;
            this.cbWonTip.Text = "Notificaions about win";
            this.cbWonTip.UseVisualStyleBackColor = true;
            // 
            // cbAutorun
            // 
            this.cbAutorun.AutoSize = true;
            this.cbAutorun.Location = new System.Drawing.Point(160, 28);
            this.cbAutorun.Name = "cbAutorun";
            this.cbAutorun.Size = new System.Drawing.Size(63, 17);
            this.cbAutorun.TabIndex = 3;
            this.cbAutorun.Text = "Autorun";
            this.cbAutorun.UseVisualStyleBackColor = true;
            this.cbAutorun.CheckedChanged += new System.EventHandler(this.cbAutorun_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.gbTimerSettings);
            this.groupBox1.Controls.Add(this.cbTimerEnable);
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(154, 114);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Timer";
            // 
            // gbTimerSettings
            // 
            this.gbTimerSettings.Controls.Add(this.label8);
            this.gbTimerSettings.Controls.Add(this.label7);
            this.gbTimerSettings.Controls.Add(this.tbTimerLoops);
            this.gbTimerSettings.Controls.Add(this.label6);
            this.gbTimerSettings.Controls.Add(this.tbTimerInterval);
            this.gbTimerSettings.Controls.Add(this.label5);
            this.gbTimerSettings.Location = new System.Drawing.Point(0, 42);
            this.gbTimerSettings.Name = "gbTimerSettings";
            this.gbTimerSettings.Size = new System.Drawing.Size(154, 72);
            this.gbTimerSettings.TabIndex = 1;
            this.gbTimerSettings.TabStop = false;
            this.gbTimerSettings.Text = "Settings";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 42);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "Cycles:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(128, 46);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(0, 13);
            this.label7.TabIndex = 4;
            // 
            // tbTimerLoops
            // 
            this.tbTimerLoops.Location = new System.Drawing.Point(68, 39);
            this.tbTimerLoops.Name = "tbTimerLoops";
            this.tbTimerLoops.Size = new System.Drawing.Size(56, 20);
            this.tbTimerLoops.TabIndex = 3;
            this.tbTimerLoops.Text = "0";
            this.tbTimerLoops.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EventKeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(128, 20);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(15, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "m";
            // 
            // tbTimerInterval
            // 
            this.tbTimerInterval.Location = new System.Drawing.Point(68, 13);
            this.tbTimerInterval.Name = "tbTimerInterval";
            this.tbTimerInterval.Size = new System.Drawing.Size(56, 20);
            this.tbTimerInterval.TabIndex = 1;
            this.tbTimerInterval.Text = "5";
            this.tbTimerInterval.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EventKeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Interval:";
            // 
            // cbTimerEnable
            // 
            this.cbTimerEnable.AutoSize = true;
            this.cbTimerEnable.Location = new System.Drawing.Point(6, 19);
            this.cbTimerEnable.Name = "cbTimerEnable";
            this.cbTimerEnable.Size = new System.Drawing.Size(65, 17);
            this.cbTimerEnable.TabIndex = 0;
            this.cbTimerEnable.Text = "Enabled";
            this.cbTimerEnable.UseVisualStyleBackColor = true;
            this.cbTimerEnable.CheckedChanged += new System.EventHandler(this.cbTimerEnable_CheckedChanged);
            // 
            // cbSortBy
            // 
            this.cbSortBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSortBy.FormattingEnabled = true;
            this.cbSortBy.ItemHeight = 13;
            this.cbSortBy.Location = new System.Drawing.Point(225, 3);
            this.cbSortBy.Name = "cbSortBy";
            this.cbSortBy.Size = new System.Drawing.Size(79, 21);
            this.cbSortBy.TabIndex = 1;
            // 
            // cbSort
            // 
            this.cbSort.AutoSize = true;
            this.cbSort.Location = new System.Drawing.Point(160, 5);
            this.cbSort.Name = "cbSort";
            this.cbSort.Size = new System.Drawing.Size(45, 17);
            this.cbSort.TabIndex = 0;
            this.cbSort.Text = "First";
            this.cbSort.UseVisualStyleBackColor = true;
            // 
            // tabPageGM
            // 
            this.tabPageGM.Controls.Add(this.btbGMCookies);
            this.tabPageGM.Controls.Add(this.cbGMRegional);
            this.tabPageGM.Controls.Add(this.cbGMGolden);
            this.tabPageGM.Controls.Add(this.chGMSandbox);
            this.tabPageGM.Controls.Add(this.cbGMRegular);
            this.tabPageGM.Controls.Add(this.tbGMReserv);
            this.tabPageGM.Controls.Add(this.tbGMMaxValue);
            this.tabPageGM.Controls.Add(this.label2);
            this.tabPageGM.Controls.Add(this.label1);
            this.tabPageGM.Location = new System.Drawing.Point(4, 40);
            this.tabPageGM.Name = "tabPageGM";
            this.tabPageGM.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGM.Size = new System.Drawing.Size(309, 215);
            this.tabPageGM.TabIndex = 0;
            this.tabPageGM.Text = "GM";
            this.tabPageGM.UseVisualStyleBackColor = true;
            // 
            // btbGMCookies
            // 
            this.btbGMCookies.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.locked1;
            this.btbGMCookies.Location = new System.Drawing.Point(280, 6);
            this.btbGMCookies.Name = "btbGMCookies";
            this.btbGMCookies.Size = new System.Drawing.Size(23, 23);
            this.btbGMCookies.TabIndex = 9;
            this.btbGMCookies.UseVisualStyleBackColor = true;
            this.btbGMCookies.Click += new System.EventHandler(this.btbGMCookies_Click);
            // 
            // cbGMRegional
            // 
            this.cbGMRegional.AutoSize = true;
            this.cbGMRegional.Location = new System.Drawing.Point(6, 75);
            this.cbGMRegional.Name = "cbGMRegional";
            this.cbGMRegional.Size = new System.Drawing.Size(133, 17);
            this.cbGMRegional.TabIndex = 8;
            this.cbGMRegional.Text = "No regional restrictions";
            this.cbGMRegional.UseVisualStyleBackColor = true;
            // 
            // cbGMGolden
            // 
            this.cbGMGolden.AutoSize = true;
            this.cbGMGolden.Location = new System.Drawing.Point(6, 52);
            this.cbGMGolden.Name = "cbGMGolden";
            this.cbGMGolden.Size = new System.Drawing.Size(135, 17);
            this.cbGMGolden.TabIndex = 6;
            this.cbGMGolden.Text = "Free golden giveaways";
            this.cbGMGolden.UseVisualStyleBackColor = true;
            // 
            // chGMSandbox
            // 
            this.chGMSandbox.AutoSize = true;
            this.chGMSandbox.Location = new System.Drawing.Point(6, 29);
            this.chGMSandbox.Name = "chGMSandbox";
            this.chGMSandbox.Size = new System.Drawing.Size(121, 17);
            this.chGMSandbox.TabIndex = 5;
            this.chGMSandbox.Text = "Sandbox giveaways";
            this.chGMSandbox.UseVisualStyleBackColor = true;
            // 
            // cbGMRegular
            // 
            this.cbGMRegular.AutoSize = true;
            this.cbGMRegular.Location = new System.Drawing.Point(6, 6);
            this.cbGMRegular.Name = "cbGMRegular";
            this.cbGMRegular.Size = new System.Drawing.Size(116, 17);
            this.cbGMRegular.TabIndex = 4;
            this.cbGMRegular.Text = "Regular giveaways";
            this.cbGMRegular.UseVisualStyleBackColor = true;
            // 
            // tbGMReserv
            // 
            this.tbGMReserv.Location = new System.Drawing.Point(254, 127);
            this.tbGMReserv.Name = "tbGMReserv";
            this.tbGMReserv.Size = new System.Drawing.Size(30, 20);
            this.tbGMReserv.TabIndex = 3;
            this.tbGMReserv.Text = "0";
            this.tbGMReserv.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EventKeyPress);
            // 
            // tbGMMaxValue
            // 
            this.tbGMMaxValue.Location = new System.Drawing.Point(134, 127);
            this.tbGMMaxValue.Name = "tbGMMaxValue";
            this.tbGMMaxValue.Size = new System.Drawing.Size(30, 20);
            this.tbGMMaxValue.TabIndex = 2;
            this.tbGMMaxValue.Text = "50";
            this.tbGMMaxValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EventKeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(170, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Reserve points:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 125);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Maximum bet:";
            // 
            // tabPageSG
            // 
            this.tabPageSG.Controls.Add(this.cbSGSortTeLessLevel);
            this.tabPageSG.Controls.Add(this.numSGLevel);
            this.tabPageSG.Controls.Add(this.cbSGMinLevel);
            this.tabPageSG.Controls.Add(this.cbSGRegular);
            this.tabPageSG.Controls.Add(this.btnSGCookies);
            this.tabPageSG.Controls.Add(this.tbSGReserv);
            this.tabPageSG.Controls.Add(this.tbSGMaxValue);
            this.tabPageSG.Controls.Add(this.lblSGReserv);
            this.tabPageSG.Controls.Add(this.lblSGMaxValue);
            this.tabPageSG.Controls.Add(this.cbSGGroup);
            this.tabPageSG.Controls.Add(this.cbSGWishlist);
            this.tabPageSG.Location = new System.Drawing.Point(4, 40);
            this.tabPageSG.Name = "tabPageSG";
            this.tabPageSG.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSG.Size = new System.Drawing.Size(309, 215);
            this.tabPageSG.TabIndex = 1;
            this.tabPageSG.Text = "SG";
            this.tabPageSG.UseVisualStyleBackColor = true;
            // 
            // tbMinNumberCopies
            // 
            this.tbMinNumberCopies.Location = new System.Drawing.Point(203, 170);
            this.tbMinNumberCopies.Name = "tbMinNumberCopies";
            this.tbMinNumberCopies.Size = new System.Drawing.Size(30, 20);
            this.tbMinNumberCopies.TabIndex = 17;
            this.tbMinNumberCopies.Text = "5";
            this.tbMinNumberCopies.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbMinNumberCopies_KeyPress);
            // 
            // cbSGRegionLocked
            // 
            this.cbSGRegionLocked.AutoSize = true;
            this.cbSGRegionLocked.Location = new System.Drawing.Point(6, 149);
            this.cbSGRegionLocked.Name = "cbSGRegionLocked";
            this.cbSGRegionLocked.Size = new System.Drawing.Size(242, 17);
            this.cbSGRegionLocked.TabIndex = 15;
            this.cbSGRegionLocked.Text = "Region restricted giveaways";
            this.cbSGRegionLocked.UseVisualStyleBackColor = true;
            this.cbSGRegionLocked.CheckedChanged += new System.EventHandler(this.cbSGRegionLocked_CheckedChanged);
            // 
            // cbSGMinNumberCopies
            // 
            this.cbSGMinNumberCopies.AutoSize = true;
            this.cbSGMinNumberCopies.Location = new System.Drawing.Point(6, 172);
            this.cbSGMinNumberCopies.Name = "cbSGMinNumberCopies";
            this.cbSGMinNumberCopies.Size = new System.Drawing.Size(194, 17);
            this.cbSGMinNumberCopies.TabIndex = 16;
            this.cbSGMinNumberCopies.Text = "Minimal number of copies:";
            this.cbSGMinNumberCopies.UseVisualStyleBackColor = true;
            this.cbSGMinNumberCopies.CheckedChanged += new System.EventHandler(this.cbSGMinNumberCopies_CheckedChanged);
            // 
            // cbSGSortTeLessLevel
            // 
            this.cbSGSortTeLessLevel.AutoSize = true;
            this.cbSGSortTeLessLevel.Location = new System.Drawing.Point(6, 98);
            this.cbSGSortTeLessLevel.Name = "cbSGSortTeLessLevel";
            this.cbSGSortTeLessLevel.Size = new System.Drawing.Size(190, 17);
            this.cbSGSortTeLessLevel.TabIndex = 14;
            this.cbSGSortTeLessLevel.Text = "First giveaways of the highest level";
            this.cbSGSortTeLessLevel.UseVisualStyleBackColor = true;
            // 
            // numSGLevel
            // 
            this.numSGLevel.Location = new System.Drawing.Point(183, 74);
            this.numSGLevel.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numSGLevel.Name = "numSGLevel";
            this.numSGLevel.Size = new System.Drawing.Size(27, 20);
            this.numSGLevel.TabIndex = 13;
            // 
            // cbSGMinLevel
            // 
            this.cbSGMinLevel.AutoSize = true;
            this.cbSGMinLevel.Location = new System.Drawing.Point(6, 75);
            this.cbSGMinLevel.Name = "cbSGMinLevel";
            this.cbSGMinLevel.Size = new System.Drawing.Size(178, 17);
            this.cbSGMinLevel.TabIndex = 12;
            this.cbSGMinLevel.Text = "The minimum level of giveaways";
            this.cbSGMinLevel.UseVisualStyleBackColor = true;
            this.cbSGMinLevel.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // cbSGRegular
            // 
            this.cbSGRegular.AutoSize = true;
            this.cbSGRegular.Location = new System.Drawing.Point(6, 6);
            this.cbSGRegular.Name = "cbSGRegular";
            this.cbSGRegular.Size = new System.Drawing.Size(116, 17);
            this.cbSGRegular.TabIndex = 11;
            this.cbSGRegular.Text = "Regular giveaways";
            this.cbSGRegular.UseVisualStyleBackColor = true;
            // 
            // btnSGCookies
            // 
            this.btnSGCookies.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.locked1;
            this.btnSGCookies.Location = new System.Drawing.Point(280, 6);
            this.btnSGCookies.Name = "btnSGCookies";
            this.btnSGCookies.Size = new System.Drawing.Size(23, 23);
            this.btnSGCookies.TabIndex = 10;
            this.btnSGCookies.UseVisualStyleBackColor = true;
            this.btnSGCookies.Click += new System.EventHandler(this.btnSGCookies_Click);
            // 
            // tbSGReserv
            // 
            this.tbSGReserv.Location = new System.Drawing.Point(254, 127);
            this.tbSGReserv.Name = "tbSGReserv";
            this.tbSGReserv.Size = new System.Drawing.Size(30, 20);
            this.tbSGReserv.TabIndex = 7;
            this.tbSGReserv.Text = "0";
            this.tbSGReserv.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EventKeyPress);
            // 
            // tbSGMaxValue
            // 
            this.tbSGMaxValue.Location = new System.Drawing.Point(134, 127);
            this.tbSGMaxValue.Name = "tbSGMaxValue";
            this.tbSGMaxValue.Size = new System.Drawing.Size(30, 20);
            this.tbSGMaxValue.TabIndex = 6;
            this.tbSGMaxValue.Text = "300";
            this.tbSGMaxValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EventKeyPress);
            // 
            // lblSGReserv
            // 
            this.lblSGReserv.AutoSize = true;
            this.lblSGReserv.Location = new System.Drawing.Point(170, 130);
            this.lblSGReserv.Name = "lblSGReserv";
            this.lblSGReserv.Size = new System.Drawing.Size(81, 13);
            this.lblSGReserv.TabIndex = 5;
            this.lblSGReserv.Text = "Reserve points:";
            // 
            // lblSGMaxValue
            // 
            this.lblSGMaxValue.AutoSize = true;
            this.lblSGMaxValue.Location = new System.Drawing.Point(3, 130);
            this.lblSGMaxValue.Name = "lblSGMaxValue";
            this.lblSGMaxValue.Size = new System.Drawing.Size(72, 13);
            this.lblSGMaxValue.TabIndex = 4;
            this.lblSGMaxValue.Text = "Maximum bet:";
            // 
            // cbSGGroup
            // 
            this.cbSGGroup.AutoSize = true;
            this.cbSGGroup.Location = new System.Drawing.Point(6, 52);
            this.cbSGGroup.Name = "cbSGGroup";
            this.cbSGGroup.Size = new System.Drawing.Size(108, 17);
            this.cbSGGroup.TabIndex = 1;
            this.cbSGGroup.Text = "Group giveaways";
            this.cbSGGroup.UseVisualStyleBackColor = true;
            // 
            // cbSGWishlist
            // 
            this.cbSGWishlist.AutoSize = true;
            this.cbSGWishlist.Location = new System.Drawing.Point(6, 29);
            this.cbSGWishlist.Name = "cbSGWishlist";
            this.cbSGWishlist.Size = new System.Drawing.Size(62, 17);
            this.cbSGWishlist.TabIndex = 0;
            this.cbSGWishlist.Text = "Wishlist";
            this.cbSGWishlist.UseVisualStyleBackColor = true;
            // 
            // tabPageSC
            // 
            this.tabPageSC.Controls.Add(this.cbSCContributors);
            this.tabPageSC.Controls.Add(this.cbSCRegular);
            this.tabPageSC.Controls.Add(this.cbSCAutojoin);
            this.tabPageSC.Controls.Add(this.btnSCCookies);
            this.tabPageSC.Controls.Add(this.tbSCReserv);
            this.tabPageSC.Controls.Add(this.tbSCMaxValue);
            this.tabPageSC.Controls.Add(this.lblSCReserv);
            this.tabPageSC.Controls.Add(this.lblSCMaxValue);
            this.tabPageSC.Controls.Add(this.cbSCGroup);
            this.tabPageSC.Controls.Add(this.cbSCWishlist);
            this.tabPageSC.Location = new System.Drawing.Point(4, 40);
            this.tabPageSC.Name = "tabPageSC";
            this.tabPageSC.Size = new System.Drawing.Size(309, 215);
            this.tabPageSC.TabIndex = 2;
            this.tabPageSC.Text = "SC";
            this.tabPageSC.UseVisualStyleBackColor = true;
            // 
            // cbSCContributors
            // 
            this.cbSCContributors.AutoSize = true;
            this.cbSCContributors.Location = new System.Drawing.Point(6, 52);
            this.cbSCContributors.Name = "cbSCContributors";
            this.cbSCContributors.Size = new System.Drawing.Size(135, 17);
            this.cbSCContributors.TabIndex = 17;
            this.cbSCContributors.Text = "Contributors giveaways";
            this.cbSCContributors.UseVisualStyleBackColor = true;
            // 
            // cbSCRegular
            // 
            this.cbSCRegular.AutoSize = true;
            this.cbSCRegular.Location = new System.Drawing.Point(6, 6);
            this.cbSCRegular.Name = "cbSCRegular";
            this.cbSCRegular.Size = new System.Drawing.Size(116, 17);
            this.cbSCRegular.TabIndex = 16;
            this.cbSCRegular.Text = "Regular giveaways";
            this.cbSCRegular.UseVisualStyleBackColor = true;
            // 
            // cbSCAutojoin
            // 
            this.cbSCAutojoin.AutoSize = true;
            this.cbSCAutojoin.Location = new System.Drawing.Point(6, 98);
            this.cbSCAutojoin.Name = "cbSCAutojoin";
            this.cbSCAutojoin.Size = new System.Drawing.Size(303, 17);
            this.cbSCAutojoin.TabIndex = 15;
            this.cbSCAutojoin.Text = "Automatic entry into group (required authorization in Steam)";
            this.cbSCAutojoin.UseVisualStyleBackColor = true;
            // 
            // btnSCCookies
            // 
            this.btnSCCookies.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.locked1;
            this.btnSCCookies.Location = new System.Drawing.Point(280, 6);
            this.btnSCCookies.Name = "btnSCCookies";
            this.btnSCCookies.Size = new System.Drawing.Size(23, 23);
            this.btnSCCookies.TabIndex = 14;
            this.btnSCCookies.UseVisualStyleBackColor = true;
            this.btnSCCookies.Click += new System.EventHandler(this.btnSCCookies_Click);
            // 
            // tbSCReserv
            // 
            this.tbSCReserv.Location = new System.Drawing.Point(254, 127);
            this.tbSCReserv.Name = "tbSCReserv";
            this.tbSCReserv.Size = new System.Drawing.Size(30, 20);
            this.tbSCReserv.TabIndex = 13;
            this.tbSCReserv.Text = "0";
            this.tbSCReserv.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EventKeyPress);
            // 
            // tbSCMaxValue
            // 
            this.tbSCMaxValue.Location = new System.Drawing.Point(134, 127);
            this.tbSCMaxValue.Name = "tbSCMaxValue";
            this.tbSCMaxValue.Size = new System.Drawing.Size(30, 20);
            this.tbSCMaxValue.TabIndex = 12;
            this.tbSCMaxValue.Text = "1500";
            this.tbSCMaxValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EventKeyPress);
            // 
            // lblSCReserv
            // 
            this.lblSCReserv.AutoSize = true;
            this.lblSCReserv.Location = new System.Drawing.Point(170, 130);
            this.lblSCReserv.Name = "lblSCReserv";
            this.lblSCReserv.Size = new System.Drawing.Size(81, 13);
            this.lblSCReserv.TabIndex = 11;
            this.lblSCReserv.Text = "Reserve points:";
            // 
            // lblSCMaxValue
            // 
            this.lblSCMaxValue.AutoSize = true;
            this.lblSCMaxValue.Location = new System.Drawing.Point(3, 130);
            this.lblSCMaxValue.Name = "lblSCMaxValue";
            this.lblSCMaxValue.Size = new System.Drawing.Size(72, 13);
            this.lblSCMaxValue.TabIndex = 10;
            this.lblSCMaxValue.Text = "Maximum bet:";
            // 
            // cbSCGroup
            // 
            this.cbSCGroup.AutoSize = true;
            this.cbSCGroup.Location = new System.Drawing.Point(6, 75);
            this.cbSCGroup.Name = "cbSCGroup";
            this.cbSCGroup.Size = new System.Drawing.Size(108, 17);
            this.cbSCGroup.TabIndex = 9;
            this.cbSCGroup.Text = "Group giveaways";
            this.cbSCGroup.UseVisualStyleBackColor = true;
            this.cbSCGroup.CheckedChanged += new System.EventHandler(this.cbSCGroup_CheckedChanged);
            // 
            // cbSCWishlist
            // 
            this.cbSCWishlist.AutoSize = true;
            this.cbSCWishlist.Location = new System.Drawing.Point(6, 29);
            this.cbSCWishlist.Name = "cbSCWishlist";
            this.cbSCWishlist.Size = new System.Drawing.Size(62, 17);
            this.cbSCWishlist.TabIndex = 8;
            this.cbSCWishlist.Text = "Wishlist";
            this.cbSCWishlist.UseVisualStyleBackColor = true;
            // 
            // tabPageUG
            // 
            this.tabPageUG.Controls.Add(this.btnSPCookies);
            this.tabPageUG.Controls.Add(this.tbSPReserv);
            this.tabPageUG.Controls.Add(this.tbSPMaxValue);
            this.tabPageUG.Controls.Add(this.label3);
            this.tabPageUG.Controls.Add(this.label4);
            this.tabPageUG.Location = new System.Drawing.Point(4, 40);
            this.tabPageUG.Name = "tabPageUG";
            this.tabPageUG.Size = new System.Drawing.Size(309, 215);
            this.tabPageUG.TabIndex = 4;
            this.tabPageUG.Text = "UG";
            this.tabPageUG.UseVisualStyleBackColor = true;
            // 
            // btnSPCookies
            // 
            this.btnSPCookies.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.locked1;
            this.btnSPCookies.Location = new System.Drawing.Point(280, 6);
            this.btnSPCookies.Name = "btnSPCookies";
            this.btnSPCookies.Size = new System.Drawing.Size(23, 23);
            this.btnSPCookies.TabIndex = 18;
            this.btnSPCookies.UseVisualStyleBackColor = true;
            this.btnSPCookies.Click += new System.EventHandler(this.btnUGCookies_Click);
            // 
            // tbSPReserv
            // 
            this.tbSPReserv.Location = new System.Drawing.Point(254, 127);
            this.tbSPReserv.Name = "tbSPReserv";
            this.tbSPReserv.Size = new System.Drawing.Size(30, 20);
            this.tbSPReserv.TabIndex = 17;
            this.tbSPReserv.Text = "0";
            this.tbSPReserv.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EventKeyPress);
            // 
            // tbSPMaxValue
            // 
            this.tbSPMaxValue.Location = new System.Drawing.Point(134, 127);
            this.tbSPMaxValue.Name = "tbSPMaxValue";
            this.tbSPMaxValue.Size = new System.Drawing.Size(30, 20);
            this.tbSPMaxValue.TabIndex = 16;
            this.tbSPMaxValue.Text = "30";
            this.tbSPMaxValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EventKeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(170, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Reserve points:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Maximum bet:";
            // 
            // tabPagePB
            // 
            this.tabPagePB.Controls.Add(this.tbPBReserv);
            this.tabPagePB.Controls.Add(this.tbPBMaxValue);
            this.tabPagePB.Controls.Add(this.label9);
            this.tabPagePB.Controls.Add(this.label10);
            this.tabPagePB.Controls.Add(this.btnPBCookies);
            this.tabPagePB.Location = new System.Drawing.Point(4, 40);
            this.tabPagePB.Name = "tabPagePB";
            this.tabPagePB.Size = new System.Drawing.Size(309, 215);
            this.tabPagePB.TabIndex = 7;
            this.tabPagePB.Text = "PB";
            this.tabPagePB.UseVisualStyleBackColor = true;
            // 
            // tbPBReserv
            // 
            this.tbPBReserv.Location = new System.Drawing.Point(254, 127);
            this.tbPBReserv.Name = "tbPBReserv";
            this.tbPBReserv.Size = new System.Drawing.Size(30, 20);
            this.tbPBReserv.TabIndex = 24;
            this.tbPBReserv.Text = "0";
            this.tbPBReserv.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EventKeyPress);
            // 
            // tbPBMaxValue
            // 
            this.tbPBMaxValue.Location = new System.Drawing.Point(134, 127);
            this.tbPBMaxValue.Name = "tbPBMaxValue";
            this.tbPBMaxValue.Size = new System.Drawing.Size(30, 20);
            this.tbPBMaxValue.TabIndex = 23;
            this.tbPBMaxValue.Text = "30";
            this.tbPBMaxValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EventKeyPress);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(170, 130);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(81, 13);
            this.label9.TabIndex = 22;
            this.label9.Text = "Reserve points:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(3, 130);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 13);
            this.label10.TabIndex = 21;
            this.label10.Text = "Maximum bet:";
            // 
            // btnPBCookies
            // 
            this.btnPBCookies.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.locked1;
            this.btnPBCookies.Location = new System.Drawing.Point(280, 6);
            this.btnPBCookies.Name = "btnPBCookies";
            this.btnPBCookies.Size = new System.Drawing.Size(23, 23);
            this.btnPBCookies.TabIndex = 20;
            this.btnPBCookies.UseVisualStyleBackColor = true;
            this.btnPBCookies.Click += new System.EventHandler(this.btnPBCookies_Click);
            // 
            // tabPageGA
            // 
            this.tabPageGA.Controls.Add(this.tbGAReserv);
            this.tabPageGA.Controls.Add(this.tbGAMaxBet);
            this.tabPageGA.Controls.Add(this.lblGAReserv);
            this.tabPageGA.Controls.Add(this.lblGAMaxBet);
            this.tabPageGA.Controls.Add(this.btnGACookies);
            this.tabPageGA.Location = new System.Drawing.Point(4, 40);
            this.tabPageGA.Name = "tabPageGA";
            this.tabPageGA.Size = new System.Drawing.Size(309, 215);
            this.tabPageGA.TabIndex = 9;
            this.tabPageGA.Text = "GA";
            this.tabPageGA.UseVisualStyleBackColor = true;
            // 
            // tbGAReserv
            // 
            this.tbGAReserv.Location = new System.Drawing.Point(254, 127);
            this.tbGAReserv.Name = "tbGAReserv";
            this.tbGAReserv.Size = new System.Drawing.Size(30, 20);
            this.tbGAReserv.TabIndex = 29;
            this.tbGAReserv.Text = "0";
            this.tbGAReserv.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EventKeyPress);
            // 
            // tbGAMaxBet
            // 
            this.tbGAMaxBet.Location = new System.Drawing.Point(134, 127);
            this.tbGAMaxBet.Name = "tbGAMaxBet";
            this.tbGAMaxBet.Size = new System.Drawing.Size(30, 20);
            this.tbGAMaxBet.TabIndex = 28;
            this.tbGAMaxBet.Text = "30";
            // 
            // lblGAReserv
            // 
            this.lblGAReserv.AutoSize = true;
            this.lblGAReserv.Location = new System.Drawing.Point(170, 130);
            this.lblGAReserv.Name = "lblGAReserv";
            this.lblGAReserv.Size = new System.Drawing.Size(81, 13);
            this.lblGAReserv.TabIndex = 27;
            this.lblGAReserv.Text = "Reserve points:";
            // 
            // lblGAMaxBet
            // 
            this.lblGAMaxBet.AutoSize = true;
            this.lblGAMaxBet.Location = new System.Drawing.Point(3, 130);
            this.lblGAMaxBet.Name = "lblGAMaxBet";
            this.lblGAMaxBet.Size = new System.Drawing.Size(72, 13);
            this.lblGAMaxBet.TabIndex = 26;
            this.lblGAMaxBet.Text = "Maximum bet:";
            // 
            // btnGACookies
            // 
            this.btnGACookies.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.locked1;
            this.btnGACookies.Location = new System.Drawing.Point(280, 6);
            this.btnGACookies.Name = "btnGACookies";
            this.btnGACookies.Size = new System.Drawing.Size(23, 23);
            this.btnGACookies.TabIndex = 25;
            this.btnGACookies.UseVisualStyleBackColor = true;
            this.btnGACookies.Click += new System.EventHandler(this.btnGACookies_Click);
            // 
            // tabPageIG
            // 
            this.tabPageIG.Controls.Add(this.tbIGReserv);
            this.tabPageIG.Controls.Add(this.lblIGReserv);
            this.tabPageIG.Controls.Add(this.tbIGMaxValue);
            this.tabPageIG.Controls.Add(this.lblIGMaxValue);
            this.tabPageIG.Controls.Add(this.cbIGDota);
            this.tabPageIG.Controls.Add(this.cbIGCSGO);
            this.tabPageIG.Controls.Add(this.cbIGTF2);
            this.tabPageIG.Controls.Add(this.cbIGSteamItems);
            this.tabPageIG.Controls.Add(this.cbIGSteamGiveaways);
            this.tabPageIG.Controls.Add(this.btnIGCookie);
            this.tabPageIG.Location = new System.Drawing.Point(4, 40);
            this.tabPageIG.Name = "tabPageIG";
            this.tabPageIG.Size = new System.Drawing.Size(309, 215);
            this.tabPageIG.TabIndex = 8;
            this.tabPageIG.Text = "IG";
            this.tabPageIG.UseVisualStyleBackColor = true;
            // 
            // tbIGReserv
            // 
            this.tbIGReserv.Location = new System.Drawing.Point(254, 127);
            this.tbIGReserv.Name = "tbIGReserv";
            this.tbIGReserv.Size = new System.Drawing.Size(30, 20);
            this.tbIGReserv.TabIndex = 30;
            this.tbIGReserv.Text = "0";
            this.tbIGReserv.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EventKeyPress);
            // 
            // lblIGReserv
            // 
            this.lblIGReserv.AutoSize = true;
            this.lblIGReserv.Location = new System.Drawing.Point(170, 130);
            this.lblIGReserv.Name = "lblIGReserv";
            this.lblIGReserv.Size = new System.Drawing.Size(81, 13);
            this.lblIGReserv.TabIndex = 29;
            this.lblIGReserv.Text = "Reserve points:";
            // 
            // tbIGMaxValue
            // 
            this.tbIGMaxValue.Location = new System.Drawing.Point(134, 127);
            this.tbIGMaxValue.Name = "tbIGMaxValue";
            this.tbIGMaxValue.Size = new System.Drawing.Size(30, 20);
            this.tbIGMaxValue.TabIndex = 28;
            this.tbIGMaxValue.Text = "20000";
            this.tbIGMaxValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EventKeyPress);
            // 
            // lblIGMaxValue
            // 
            this.lblIGMaxValue.AutoSize = true;
            this.lblIGMaxValue.Location = new System.Drawing.Point(3, 130);
            this.lblIGMaxValue.Name = "lblIGMaxValue";
            this.lblIGMaxValue.Size = new System.Drawing.Size(81, 13);
            this.lblIGMaxValue.TabIndex = 27;
            this.lblIGMaxValue.Text = "Reserve points:";
            // 
            // cbIGDota
            // 
            this.cbIGDota.AutoSize = true;
            this.cbIGDota.Location = new System.Drawing.Point(6, 98);
            this.cbIGDota.Name = "cbIGDota";
            this.cbIGDota.Size = new System.Drawing.Size(85, 17);
            this.cbIGDota.TabIndex = 26;
            this.cbIGDota.Text = "Dota 2 items";
            this.cbIGDota.UseVisualStyleBackColor = true;
            // 
            // cbIGCSGO
            // 
            this.cbIGCSGO.AutoSize = true;
            this.cbIGCSGO.Location = new System.Drawing.Point(6, 75);
            this.cbIGCSGO.Name = "cbIGCSGO";
            this.cbIGCSGO.Size = new System.Drawing.Size(86, 17);
            this.cbIGCSGO.TabIndex = 25;
            this.cbIGCSGO.Text = "CS:GO items";
            this.cbIGCSGO.UseVisualStyleBackColor = true;
            // 
            // cbIGTF2
            // 
            this.cbIGTF2.AutoSize = true;
            this.cbIGTF2.Location = new System.Drawing.Point(6, 52);
            this.cbIGTF2.Name = "cbIGTF2";
            this.cbIGTF2.Size = new System.Drawing.Size(72, 17);
            this.cbIGTF2.TabIndex = 24;
            this.cbIGTF2.Text = "TF2 items";
            this.cbIGTF2.UseVisualStyleBackColor = true;
            // 
            // cbIGSteamItems
            // 
            this.cbIGSteamItems.AutoSize = true;
            this.cbIGSteamItems.Location = new System.Drawing.Point(6, 29);
            this.cbIGSteamItems.Name = "cbIGSteamItems";
            this.cbIGSteamItems.Size = new System.Drawing.Size(83, 17);
            this.cbIGSteamItems.TabIndex = 23;
            this.cbIGSteamItems.Text = "Steam items";
            this.cbIGSteamItems.UseVisualStyleBackColor = true;
            // 
            // cbIGSteamGiveaways
            // 
            this.cbIGSteamGiveaways.AutoSize = true;
            this.cbIGSteamGiveaways.Location = new System.Drawing.Point(6, 6);
            this.cbIGSteamGiveaways.Name = "cbIGSteamGiveaways";
            this.cbIGSteamGiveaways.Size = new System.Drawing.Size(109, 17);
            this.cbIGSteamGiveaways.TabIndex = 22;
            this.cbIGSteamGiveaways.Text = "Steam giveaways";
            this.cbIGSteamGiveaways.UseVisualStyleBackColor = true;
            // 
            // btnIGCookie
            // 
            this.btnIGCookie.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.locked1;
            this.btnIGCookie.Location = new System.Drawing.Point(280, 6);
            this.btnIGCookie.Name = "btnIGCookie";
            this.btnIGCookie.Size = new System.Drawing.Size(23, 23);
            this.btnIGCookie.TabIndex = 21;
            this.btnIGCookie.UseVisualStyleBackColor = true;
            this.btnIGCookie.Click += new System.EventHandler(this.btnIGCookie_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(317, 24);
            this.menuStrip.TabIndex = 1;
            this.menuStrip.Text = "menuStrip1";
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = global::KryBot.Gui.WinFormsGui.Properties.Resources.check;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(59, 20);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.сохранитьToolStripMenuItem_Click);
            // 
            // FormSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(317, 265);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSettings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSettings_FormClosing);
            this.Load += new System.EventHandler(this.formSettings_Load);
            this.tabControl.ResumeLayout(false);
            this.tabPageCommun.ResumeLayout(false);
            this.tabPageCommun.PerformLayout();
            this.gbLang.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbTimerSettings.ResumeLayout(false);
            this.gbTimerSettings.PerformLayout();
            this.tabPageGM.ResumeLayout(false);
            this.tabPageGM.PerformLayout();
            this.tabPageSG.ResumeLayout(false);
            this.tabPageSG.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSGLevel)).EndInit();
            this.tabPageSC.ResumeLayout(false);
            this.tabPageSC.PerformLayout();
            this.tabPageUG.ResumeLayout(false);
            this.tabPageUG.PerformLayout();
            this.tabPagePB.ResumeLayout(false);
            this.tabPagePB.PerformLayout();
            this.tabPageGA.ResumeLayout(false);
            this.tabPageGA.PerformLayout();
            this.tabPageIG.ResumeLayout(false);
            this.tabPageIG.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage tabPageGM;
		private System.Windows.Forms.TabPage tabPageSG;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.TabPage tabPageSC;
		private System.Windows.Forms.TabPage tabPageUG;
		private System.Windows.Forms.TabPage tabPageCommun;
		private System.Windows.Forms.ComboBox cbSortBy;
		private System.Windows.Forms.CheckBox cbSort;
		private System.Windows.Forms.CheckBox cbGMRegional;
		private System.Windows.Forms.CheckBox cbGMGolden;
		private System.Windows.Forms.CheckBox chGMSandbox;
		private System.Windows.Forms.CheckBox cbGMRegular;
		private System.Windows.Forms.TextBox tbGMReserv;
		private System.Windows.Forms.TextBox tbGMMaxValue;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbSGReserv;
		private System.Windows.Forms.TextBox tbSGMaxValue;
		private System.Windows.Forms.Label lblSGReserv;
		private System.Windows.Forms.Label lblSGMaxValue;
		private System.Windows.Forms.CheckBox cbSGGroup;
		private System.Windows.Forms.CheckBox cbSGWishlist;
		private System.Windows.Forms.TextBox tbSCReserv;
		private System.Windows.Forms.TextBox tbSCMaxValue;
		private System.Windows.Forms.Label lblSCReserv;
		private System.Windows.Forms.Label lblSCMaxValue;
		private System.Windows.Forms.CheckBox cbSCGroup;
		private System.Windows.Forms.CheckBox cbSCWishlist;
		private System.Windows.Forms.TextBox tbSPReserv;
		private System.Windows.Forms.TextBox tbSPMaxValue;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btbGMCookies;
		private System.Windows.Forms.Button btnSGCookies;
		private System.Windows.Forms.Button btnSCCookies;
		private System.Windows.Forms.Button btnSPCookies;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox cbTimerEnable;
		private System.Windows.Forms.GroupBox gbTimerSettings;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox tbTimerLoops;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox tbTimerInterval;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.CheckBox cbAutorun;
		private System.Windows.Forms.CheckBox cbSCAutojoin;
		private System.Windows.Forms.CheckBox cbWonTip;
		private System.Windows.Forms.CheckBox cbFarmTip;
		private System.Windows.Forms.CheckBox cbSGRegular;
		private System.Windows.Forms.CheckBox cbSCRegular;
		private System.Windows.Forms.NumericUpDown numSGLevel;
		private System.Windows.Forms.CheckBox cbSGMinLevel;
		private System.Windows.Forms.CheckBox cbSGSortTeLessLevel;
		private System.Windows.Forms.TabPage tabPagePB;
		private System.Windows.Forms.Button btnPBCookies;
		private System.Windows.Forms.TextBox tbPBReserv;
		private System.Windows.Forms.TextBox tbPBMaxValue;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.CheckBox cbWishlistSort;
		private System.Windows.Forms.CheckBox cbSCContributors;
        private System.Windows.Forms.CheckBox cbSGRegionLocked;
        private System.Windows.Forms.CheckBox cbSGMinNumberCopies;
        private System.Windows.Forms.TextBox tbMinNumberCopies;
        private System.Windows.Forms.GroupBox gbLang;
		private System.Windows.Forms.ComboBox cbLang;

		private void Localization()
		{
			tabPageCommun.Text = strings.SettingsForm_Commun;
			gbLang.Text = strings.Language;
			cbWishlistSort.Text = strings.SettingsForm_cbWishlistSort;
			cbFarmTip.Text = strings.SettingsForm_cbFarmTip;
			cbWonTip.Text = strings.SettingsForm_cbWonTip;
			cbAutorun.Text = strings.SettingsForm_cbAutorun;
			groupBox1.Text = strings.SettingsForm_groupBox1;
			gbTimerSettings.Text = strings.SettingsForm_gbTimerSettings;
			label8.Text = strings.SettingsForm_label8;
			label7.Text = strings.SettingsForm_label7;
			label6.Text = strings.SettingsForm_label6;
			label5.Text = strings.SettingsForm_label5;
			cbTimerEnable.Text = strings.SettingsForm_cbTimerEnable;
			cbSort.Text = strings.SettingsForm_cbSort;
			cbGMRegional.Text = strings.SettingsForm_cbGMRegional;
			cbGMGolden.Text = strings.SettingsForm_cbGMGolden;
			chGMSandbox.Text = strings.SettingsForm_chGMSandbox;
			cbGMRegular.Text = strings.SettingsForm_RegularGiveaways;
			label2.Text = strings.SettingsForm_PointsReserv;
			label1.Text = strings.SettingsFform_MaxValue;
			cbSGSortTeLessLevel.Text = strings.SettingsForm_cbSGSortTeLessLevel;
			cbSGMinLevel.Text = strings.SettingsForm_cbSGMinLevel;
			cbSGRegular.Text = strings.SettingsForm_RegularGiveaways;
		    cbSGGroup.Text = strings.SettingsForm_GroupGiveaways;
			lblSGReserv.Text = strings.SettingsForm_PointsReserv;
			lblSGMaxValue.Text = strings.SettingsFform_MaxValue;
			cbSGWishlist.Text = strings.SettingsForm_WishlistGiveaways;
			cbSCContributors.Text = strings.SettingsForm_cbSCContributors;
			cbSCRegular.Text = strings.SettingsForm_RegularGiveaways;
			cbSCAutojoin.Text = strings.SettingsForm_cbSCAutojoin;
			lblSCReserv.Text = strings.SettingsForm_PointsReserv;
			lblSCMaxValue.Text = strings.SettingsFform_MaxValue;
			cbSCGroup.Text = strings.SettingsForm_GroupGiveaways;
			cbSCWishlist.Text = strings.SettingsForm_WishlistGiveaways;
			label3.Text = strings.SettingsForm_PointsReserv;
			label4.Text = strings.SettingsFform_MaxValue;
			label9.Text = strings.SettingsForm_PointsReserv;
			label10.Text = strings.SettingsFform_MaxValue;
			saveToolStripMenuItem.Text = strings.Save;
			Text = strings.Settings;
			Icon = Icon.FromHandle(Resources.settings.GetHicon());
		    cbIGCSGO.Text = strings.SettingsForm_cbIGCSGO;
		    cbIGTF2.Text = strings.SettingsForm_cbIGTF2;
		    cbIGDota.Text = strings.SettingsForm_cbIGDota;
		    cbIGSteamGiveaways.Text = strings.SettingsForm_cbIGSteamGiveaways;
		    cbIGSteamItems.Text = strings.SettingsForm_cbIGSteamItems;
		    lblIGMaxValue.Text = strings.SettingsFform_MaxValue;
		    lblIGReserv.Text = strings.SettingsForm_PointsReserv;
		    lblGAMaxBet.Text = strings.SettingsFform_MaxValue;
		    lblGAReserv.Text = strings.SettingsForm_PointsReserv;
		    btnShortcut.Text = strings.Settngs_CreateShortcut;
		}

        private System.Windows.Forms.Button btnShortcut;
        private System.Windows.Forms.TabPage tabPageIG;
        private System.Windows.Forms.Button btnIGCookie;
        private System.Windows.Forms.CheckBox cbIGDota;
        private System.Windows.Forms.CheckBox cbIGCSGO;
        private System.Windows.Forms.CheckBox cbIGTF2;
        private System.Windows.Forms.CheckBox cbIGSteamItems;
        private System.Windows.Forms.CheckBox cbIGSteamGiveaways;
        private System.Windows.Forms.Label lblIGReserv;
        private System.Windows.Forms.TextBox tbIGMaxValue;
        private System.Windows.Forms.Label lblIGMaxValue;
        private System.Windows.Forms.TextBox tbIGReserv;
        private System.Windows.Forms.TabPage tabPageGA;
        private System.Windows.Forms.TextBox tbGAReserv;
        private System.Windows.Forms.TextBox tbGAMaxBet;
        private System.Windows.Forms.Label lblGAReserv;
        private System.Windows.Forms.Label lblGAMaxBet;
        private System.Windows.Forms.Button btnGACookies;
    }
}