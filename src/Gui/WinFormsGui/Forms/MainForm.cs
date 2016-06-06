using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using KryBot.CommonResources.lang;
using KryBot.Functional;
using KryBot.Functional.Giveaways;
using KryBot.Functional.Sites;
using KryBot.Gui.WinFormsGui.Properties;
using RestSharp;

namespace KryBot.Gui.WinFormsGui.Forms
{
    public partial class FormMain : Form
    {
        public delegate void SubscribesContainer();

        public static Bot Bot = new Bot();
        public static Blacklist BlackList;

        public static bool Hided;

        private readonly Timer _timer = new Timer();
        private readonly Timer _timerTickCount = new Timer();
        private bool _farming;
        private int _interval;
        private int _loopsLeft;
        public bool LogActive;

        public Log LogBuffer;

        public FormMain()
        {
            InitializeComponent();
        }

        public event SubscribesContainer LogHide;
        public event SubscribesContainer LogUnHide;
        public event SubscribesContainer FormChangeLocation;
        public event SubscribesContainer LogChanged;
        public event SubscribesContainer LoadProfilesInfo;

        private void логToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LogActive)
            {
                HideLog();
            }
            else
            {
                UnHideLog();
            }
        }

        private async void FormMain_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.FirstExecute)
            {
                switch (MessageBox.Show(strings.Licens, strings.Agreement, MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information))
                {
                    case DialogResult.Yes:
                        Properties.Settings.Default.FirstExecute = false;
                        break;
                    case DialogResult.No:
                        Application.Exit();
                        break;
                }
            }

            if (Tools.CheckIeVersion(9))
            {
                switch (
                    MessageBox.Show(strings.IECheck, strings.Warning, MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Warning))
                {
                    case DialogResult.Yes:
                        Process.Start("http://windows.microsoft.com/ru-ru/internet-explorer/download-ie");
                        Application.Exit();
                        break;
                    case DialogResult.Cancel:
                        Application.Exit();
                        break;
                }
            }

            new Settings().Load();
            LoadProfilesInfo += ShowProfileInfo;
            LogActive = Properties.Settings.Default.LogActive;
            Design();
            BlackList = Tools.LoadBlackList();

            var version = await Updater.CheckForUpdates();
            WriteLog(version);

            if (version.Success)
            {
                var dr = MessageBox.Show($"{version.Content.Replace("\n", "")}. Обновить?", @"Обновление",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dr == DialogResult.Yes)
                {
                    WriteLog(new Log($"{Messages.GetDateTime()} Обновление...", Color.White, true, true));

                    toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
                    toolStripProgressBar1.Visible = true;

                    var log = await Updater.Update();
                    WriteLog(log);

                    if (log.Success)
                    {
                        Process.Start("KryBot.exe");
                        Application.Exit();
                    }

                    toolStripProgressBar1.Style = ProgressBarStyle.Blocks;
                    toolStripProgressBar1.Visible = false;
                }
            }

            if (LoadProfile())
            {
                cbGMEnable.Checked = Bot.GameMiner.Enabled;
                cbSGEnable.Checked = Bot.SteamGifts.Enabled;
                cbSCEnable.Checked = Bot.SteamCompanion.Enabled;
                cbSPEnable.Checked = Bot.SteamPortal.Enabled;
                cbSTEnable.Checked = Bot.SteamTrade.Enabled;
                cbPBEnabled.Checked = Bot.SteamTrade.Enabled;
                btnStart.Enabled = await LoginCheck();

                if (Bot.Steam.Enabled)
                {
                    await
                        Web.SteamJoinGroupAsync("http://steamcommunity.com/groups/krybot",
                            Generate.PostData_SteamGroupJoin(Bot.Steam.Cookies.Sessid), Generate.Cookies_Steam(Bot));
                }
            }
            else
            {
                btnGMLogin.Visible = true;
                btnSGLogin.Visible = true;
                btnSCLogin.Visible = true;
                btnSPLogin.Visible = true;
                btnSTLogin.Visible = true;
                btnPBLogin.Visible = true;
                btnSteamLogin.Visible = true;
                btnStart.Enabled = false;
            }
        }

        private void FormMain_LocationChanged(object sender, EventArgs e)
        {
            FormChangeLocation?.Invoke();
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.Timer)
            {
                if (!_farming)
                {
                    _loopsLeft = Properties.Settings.Default.TimerLoops;
                    _timer.Interval = Properties.Settings.Default.TimerInterval;
                    _interval = _timer.Interval;
                    _timerTickCount.Interval = 1000;
                    _timer.Tick += timer_Tick;
                    _timerTickCount.Tick += TimerTickCountOnTick;
                    _timer.Start();
                    _timerTickCount.Start();
                    btnStart.Text =
                        $"{strings.FormMain_btnStart_Click_Stop} ({TimeSpan.FromMilliseconds(_timer.Interval)})";
                    if (Properties.Settings.Default.ShowFarmTip)
                    {
                        ShowBaloolTip(
                            $"{strings.FormMain_btnStart_Click_FarmBeginWithInterval} {_interval/60000} {strings.FormMain_btnStart_Click_M}",
                            5000, ToolTipIcon.Info);
                    }
                    await DoFarm();
                    if (Properties.Settings.Default.ShowFarmTip)
                    {
                        ShowBaloolTip(strings.FormMain_btnStart_Click_FarmFinish, 5000, ToolTipIcon.Info);
                    }
                }
                else
                {
                    _timer.Stop();
                    _timerTickCount.Stop();
                    btnStart.Text = strings.FormMain_btnStart_Click_Старт;
                    btnStart.Enabled = false;
                }
            }
            else
            {
                if (_timer.Enabled)
                {
                    _timer.Stop();
                }

                if (_timerTickCount.Enabled)
                {
                    _timerTickCount.Stop();
                }

                if (!_farming)
                {
                    btnStart.Enabled = false;
                    btnStart.Text = strings.FormMain_btnStart_Click_TaskBegin;

                    if (Properties.Settings.Default.ShowFarmTip)
                    {
                        ShowBaloolTip(strings.FormMain_btnStart_Click_FarmBegin, 5000, ToolTipIcon.Info);
                    }

                    await DoFarm();

                    if (Properties.Settings.Default.ShowFarmTip)
                    {
                        ShowBaloolTip(strings.FormMain_btnStart_Click_FarmFinish, 5000, ToolTipIcon.Info);
                    }

                    btnStart.Text = strings.FormMain_btnStart_Click_Старт;
                    btnStart.Enabled = true;
                }
                else
                {
                    WriteLog(new Log($"{Messages.GetDateTime()} {strings.FormMain_btnStart_Click_FarmSkip}", Color.Red,
                        false, true));
                    btnStart.Text = strings.FormMain_btnStart_Click_Старт;
                }
            }
        }

        private void TimerTickCountOnTick(object sender, EventArgs eventArgs)
        {
            _interval += -1000;
            btnStart.Text = $"{strings.FormMain_btnStart_Click_Stop} ({TimeSpan.FromMilliseconds(_interval)})";
        }

        private void Design()
        {
            Text = $"{Application.ProductName} [{Application.ProductVersion}]";
            Icon = Resources.KryBotPresent_256b;

            btnStart.Enabled = false;
            btnGMLogin.Visible = false;
            btnSGLogin.Visible = false;
            btnSCLogin.Visible = false;
            btnSPLogin.Visible = false;
            btnSTLogin.Visible = false;
            btnPBLogin.Visible = false;
            btnSteamLogin.Visible = false;
            btnGMExit.Visible = false;
            btnSGExit.Visible = false;
            btnSCExit.Visible = false;
            btnSPExit.Visible = false;
            btnSTExit.Visible = false;
            btnPBExit.Visible = false;
            btnSteamExit.Visible = false;

            toolStripProgressBar1.Visible = false;
            toolStripStatusLabel1.Image = null;
            toolStripStatusLabel1.Text = "";

            pbGMReload.Visible = false;
            pbSGReload.Visible = false;
            pbSCReload.Visible = false;
            pbSPReload.Visible = false;
            pbSTreload.Visible = false;
            pbPBRefresh.Visible = false;

            if (LogActive)
            {
                OpenLog();
            }
            else
            {
                OpenLog();
                HideLog();
            }
        }

        private bool LoadProfile()
        {
            if (File.Exists("profile.xml"))
            {
                Bot = Tools.LoadProfile("");
                if (Bot == null)
                {
                    var message = Messages.FileLoadFailed("profile.xml");
                    message.Content += "\n";
                    WriteLog(message);

                    MessageBox.Show(message.Content.Split(']')[1], strings.Error, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }
                WriteLog(Messages.ProfileLoaded());
                return true;
            }

            Bot = new Bot();
            return false;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.LogActive = LogActive;
            Bot.Save();
            new Settings().Save();
            Properties.Settings.Default.JoinsPerSession = 0;
            Properties.Settings.Default.JoinsLoops = 0;
            Properties.Settings.Default.Save();
        }

        private void OpenLog()
        {
            логToolStripMenuItem.Text = $"{strings.Log} <<";
            var form = new FormLog(Location.X + Width - 15, Location.Y) {Owner = this};

            LogHide += form.FormHide;
            LogUnHide += form.FormUnHide;
            FormChangeLocation += form.FormChangeLocation;
            LogChanged += form.LogChanged;

            LogBuffer = Messages.Start();
            form.Show();
            LogActive = true;
        }

        private void UnHideLog()
        {
            LogUnHide?.Invoke();
            логToolStripMenuItem.Text = $"{strings.Log} <<";
            LogActive = true;
        }

        private void HideLog()
        {
            LogHide?.Invoke();
            логToolStripMenuItem.Text = $"{strings.Log} >>";
            LogActive = false;
        }

        private async void timer_Tick(object sender, EventArgs e)
        {
            _interval = _timer.Interval;
            if (!_farming)
            {
                if (_loopsLeft > 0)
                {
                    await DoFarm();
                    _loopsLeft += -1;
                }
                else
                {
                    await DoFarm();
                }
            }
        }

        private async Task<bool> DoFarm()
        {
            _farming = true;
            WriteLog(Messages.DoFarm_Start());

            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = 5;
            toolStripProgressBar1.Visible = true;
            toolStripStatusLabel1.Image = Resources.load;
            toolStripStatusLabel1.Text = strings.FormMain_DoFarm_Farn;

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            if (Bot.GameMiner.Enabled)
            {
                var profile = await Parse.GameMinerGetProfileAsync(Bot);
                if (profile.Echo)
                {
                    WriteLog(profile);
                }
                LoadProfilesInfo?.Invoke();

                if (profile.Success)
                {
                    var won = await Parse.GameMinerWonParseAsync(Bot);
                    if (won != null && won.Content != "\n")
                    {
                        WriteLog(won);
                        if (Properties.Settings.Default.ShowWonTip)
                        {
                            ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info);
                        }
                    }

                    var giveaways = await Parse.GameMinerLoadGiveawaysAsync(Bot, Bot.GameMiner.Giveaways, BlackList);
                    if (giveaways != null && giveaways.Content != "\n")
                    {
                        WriteLog(giveaways);
                    }

                    if (Bot.GameMiner.Giveaways?.Count > 0)
                    {
                        if (Properties.Settings.Default.Sort)
                        {
                            if (Properties.Settings.Default.SortToMore)
                            {
                                Bot.GameMiner.Giveaways.Sort((a, b) => b.Price.CompareTo(a.Price));
                            }
                            else
                            {
                                Bot.GameMiner.Giveaways.Sort((a, b) => a.Price.CompareTo(b.Price));
                            }
                        }

                        await JoinGiveaways(Bot.GameMiner.Giveaways);
                    }
                }
                else
                {
                    BlockTabpage(tabPageGM, false);
                    btnGMLogin.Enabled = true;
                    btnGMLogin.Visible = true;
                    linkLabel1.Enabled = true;
                    lblGMStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
                }
            }
            toolStripProgressBar1.Value++;

            if (Bot.SteamGifts.Enabled)
            {
                var profile = await Parse.SteamGiftsGetProfileAsync(Bot);
                if (profile.Echo)
                {
                    WriteLog(profile);
                }
                LoadProfilesInfo?.Invoke();

                if (profile.Success)
                {
                    var won = await Parse.SteamGiftsWonParseAsync(Bot);
                    if (won != null && won.Content != "\n")
                    {
                        WriteLog(won);
                        if (Properties.Settings.Default.ShowWonTip)
                        {
                            ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info);
                        }
                    }

                    if (Bot.SteamGifts.Points > 0)
                    {
                        var giveaways =
                            await
                                Parse.SteamGiftsLoadGiveawaysAsync(Bot, Bot.SteamGifts.Giveaways,
                                    Bot.SteamGifts.WishlistGiveaways, BlackList);
                        if (giveaways != null && giveaways.Content != "\n")
                        {
                            WriteLog(giveaways);
                        }

                        if (Bot.SteamGifts.WishlistGiveaways.Count > 0)
                        {
                            if (Properties.Settings.Default.Sort)
                            {
                                if (Properties.Settings.Default.SortToMore)
                                {
                                    if (!Properties.Settings.Default.WishlistNotSort)
                                    {
                                        Bot.SteamGifts.WishlistGiveaways.Sort((a, b) => b.Price.CompareTo(a.Price));
                                    }
                                }
                                else
                                {
                                    if (!Properties.Settings.Default.WishlistNotSort)
                                    {
                                        Bot.SteamGifts.WishlistGiveaways.Sort((a, b) => a.Price.CompareTo(b.Price));
                                    }
                                }
                            }

                            if (Bot.SteamGifts.SortToLessLevel)
                            {
                                if (!Properties.Settings.Default.WishlistNotSort)
                                {
                                    Bot.SteamGifts.WishlistGiveaways.Sort((a, b) => b.Level.CompareTo(a.Level));
                                }
                            }

                            await JoinGiveaways(Bot.SteamGifts.WishlistGiveaways, true);
                        }

                        if (Bot.SteamGifts.Giveaways.Count > 0)
                        {
                            if (Properties.Settings.Default.Sort)
                            {
                                if (Properties.Settings.Default.SortToMore)
                                {
                                    Bot.SteamGifts.Giveaways.Sort((a, b) => b.Price.CompareTo(a.Price));
                                }
                                else
                                {
                                    Bot.SteamGifts.Giveaways.Sort((a, b) => a.Price.CompareTo(b.Price));
                                }
                            }

                            if (Bot.SteamGifts.SortToLessLevel)
                            {
                                Bot.SteamGifts.Giveaways.Sort((a, b) => b.Level.CompareTo(a.Level));
                            }

                            await JoinGiveaways(Bot.SteamGifts.Giveaways, false);
                        }
                    }
                }
                else
                {
                    BlockTabpage(tabPageSG, false);
                    btnSGLogin.Enabled = true;
                    btnSGLogin.Visible = true;
                    linkLabel2.Enabled = true;
                    lblSGStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
                }
            }
            toolStripProgressBar1.Value++;

            if (Bot.SteamCompanion.Enabled)
            {
                var profile = await Parse.SteamCompanionGetProfileAsync(Bot);
                if (profile.Echo)
                {
                    WriteLog(profile);
                }
                LoadProfilesInfo?.Invoke();
                if (profile.Success)
                {
                    var won = await Parse.SteamCompanionWonParseAsync(Bot);
                    if (won != null)
                    {
                        WriteLog(won);
                        if (Properties.Settings.Default.ShowWonTip)
                        {
                            ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info);
                        }
                    }

                    var giveaways =
                        await
                            Parse.SteamCompanionLoadGiveawaysAsync(Bot, Bot.SteamCompanion.Giveaways,
                                Bot.SteamCompanion.WishlistGiveaways);
                    if (giveaways != null && giveaways.Content != "\n")
                    {
                        WriteLog(giveaways);
                    }

                    if (Bot.SteamCompanion.WishlistGiveaways.Count > 0)
                    {
                        if (Properties.Settings.Default.Sort)
                        {
                            if (Properties.Settings.Default.SortToMore)
                            {
                                if (!Properties.Settings.Default.WishlistNotSort)
                                {
                                    Bot.SteamCompanion.WishlistGiveaways.Sort((a, b) => b.Price.CompareTo(a.Price));
                                }
                            }
                            else
                            {
                                if (!Properties.Settings.Default.WishlistNotSort)
                                {
                                    Bot.SteamCompanion.WishlistGiveaways.Sort((a, b) => a.Price.CompareTo(b.Price));
                                }
                            }
                        }

                        await JoinGiveaways(Bot.SteamCompanion.WishlistGiveaways, true);
                    }

                    if (Bot.SteamCompanion.Giveaways.Count > 0)
                    {
                        if (Properties.Settings.Default.Sort)
                        {
                            if (Properties.Settings.Default.SortToMore)
                            {
                                Bot.SteamCompanion.Giveaways.Sort((a, b) => b.Price.CompareTo(a.Price));
                                if (!Properties.Settings.Default.WishlistNotSort)
                                {
                                    Bot.SteamCompanion.WishlistGiveaways.Sort((a, b) => b.Price.CompareTo(a.Price));
                                }
                            }
                            else
                            {
                                Bot.SteamCompanion.Giveaways.Sort((a, b) => a.Price.CompareTo(b.Price));
                                if (!Properties.Settings.Default.WishlistNotSort)
                                {
                                    Bot.SteamCompanion.WishlistGiveaways.Sort((a, b) => a.Price.CompareTo(b.Price));
                                }
                            }
                        }
                    }

                    await JoinGiveaways(Bot.SteamCompanion.Giveaways, false);

                    var async = await Web.SteamCompanionSyncAccountAsync(Bot);
                    if (async != null)
                    {
                        WriteLog(async);
                    }
                }
                else
                {
                    BlockTabpage(tabPageSC, false);
                    btnSCLogin.Enabled = true;
                    btnSCLogin.Visible = true;
                    linkLabel3.Enabled = true;
                    lblSCStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
                }
            }

            if (Bot.SteamPortal.Enabled)
            {
                var profile = await Parse.SteamPortalGetProfileAsync(Bot);
                if (profile.Echo)
                {
                    WriteLog(profile);
                }
                LoadProfilesInfo?.Invoke();
                if (profile.Success)
                {
                    var won = await Parse.SteamPortalWonParsAsync(Bot);
                    if (won != null)
                    {
                        WriteLog(won);
                        if (Properties.Settings.Default.ShowWonTip)
                        {
                            ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info);
                        }
                    }

                    if (Bot.SteamPortal.Points > 0)
                    {
                        var giveaways =
                            await Parse.SteamPortalLoadGiveawaysAsync(Bot, Bot.SteamPortal.Giveaways, BlackList);
                        if (giveaways != null && giveaways.Content != "\n")
                        {
                            WriteLog(giveaways);
                        }

                        if (Bot.SteamPortal.Giveaways?.Count > 0)
                        {
                            if (Properties.Settings.Default.Sort)
                            {
                                if (Properties.Settings.Default.SortToMore)
                                {
                                    Bot.SteamPortal.Giveaways.Sort((a, b) => b.Price.CompareTo(a.Price));
                                }
                                else
                                {
                                    Bot.SteamPortal.Giveaways.Sort((a, b) => a.Price.CompareTo(b.Price));
                                }
                            }

                            await JoinGiveaways(Bot.SteamPortal.Giveaways);
                        }
                    }
                }
                else
                {
                    BlockTabpage(tabPageSP, false);
                    btnSPLogin.Enabled = true;
                    btnSPLogin.Visible = true;
                    linkLabel4.Enabled = true;
                    lblSPStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
                }
            }

            if (Bot.SteamTrade.Enabled)
            {
                var profile = await Parse.SteamTradeGetProfileAsync(Bot);
                if (profile.Echo)
                {
                    WriteLog(profile);
                }
                LoadProfilesInfo?.Invoke();
                if (profile.Success)
                {
                    var giveaways = await Parse.SteamTradeLoadGiveawaysAsync(Bot, Bot.SteamTrade.Giveaways, BlackList);
                    if (giveaways != null && giveaways.Content != "\n")
                    {
                        WriteLog(giveaways);
                    }

                    if (Bot.SteamTrade.Giveaways?.Count > 0)
                    {
                        await JoinGiveaways(Bot.SteamTrade.Giveaways);
                    }
                }
                else
                {
                    BlockTabpage(tabPageSteam, false);
                    btnSteamLogin.Enabled = true;
                    btnSteamLogin.Visible = true;
                    linkLabel6.Enabled = true;
                    lblSteamStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
                }
            }

            if (Bot.PlayBlink.Enabled)
            {
                var profile = await Parse.PlayBlinkGetProfileAsync(Bot);
                if (profile.Echo)
                {
                    WriteLog(profile);
                }
                LoadProfilesInfo?.Invoke();

                if (profile.Success)
                {
                    if (Bot.PlayBlink.Points > 0)
                    {
                        var giveaways = await Parse.PlayBlinkLoadGiveawaysAsync(Bot, Bot.PlayBlink.Giveaways, BlackList);
                        if (giveaways != null && giveaways.Content != "\n")
                        {
                            WriteLog(giveaways);
                        }

                        if (Bot.PlayBlink.Giveaways?.Count > 0)
                        {
                            if (Properties.Settings.Default.Sort)
                            {
                                if (Properties.Settings.Default.SortToMore)
                                {
                                    Bot.PlayBlink.Giveaways.Sort((a, b) => b.Price.CompareTo(a.Price));
                                }
                                else
                                {
                                    Bot.PlayBlink.Giveaways.Sort((a, b) => a.Price.CompareTo(b.Price));
                                }
                            }

                            await JoinGiveaways(Bot.PlayBlink.Giveaways);
                        }
                    }
                }
                else
                {
                    BlockTabpage(tabPagePB, false);
                    btnPBLogin.Enabled = true;
                    btnPBLogin.Visible = true;
                    linkLabel7.Enabled = true;
                    lblPBStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
                }
            }
            toolStripProgressBar1.Value++;

            stopWatch.Stop();
            var ts = stopWatch.Elapsed;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
            WriteLog(Messages.DoFarm_Finish(elapsedTime));
            LoadProfilesInfo?.Invoke();

            if (_loopsLeft > 0)
            {
                WriteLog(new Log($"{Messages.GetDateTime()} {strings.FormMain_timer_Tick_LoopsLeft}: {_loopsLeft - 1}",
                    Color.White, true, true));
                _loopsLeft += -1;
            }

            Bot.ClearGiveawayList();

            toolStripProgressBar1.Visible = false;
            toolStripStatusLabel1.Image = null;
            toolStripStatusLabel1.Text = strings.StatusBar_End;
            _farming = false;
            btnStart.Enabled = true;

            Properties.Settings.Default.JoinsLoops += 1;
            Properties.Settings.Default.JoinsLoopsTotal += 1;
            Properties.Settings.Default.Save();

            return true;
        }

        private void ShowProfileInfo()
        {
            lblGMCoal.Text = $"{strings.Coal}: {Bot.GameMiner.Coal}";
            lblGMLevel.Text = $"{strings.Level}: {Bot.GameMiner.Level}";

            lblSGPoints.Text = $"{strings.Points}: {Bot.SteamGifts.Points}";
            lblSGLevel.Text = $"{strings.Level}: {Bot.SteamGifts.Level}";

            lblSCPoints.Text = $"{strings.Points}: {Bot.SteamCompanion.Points}";
            lblSCLevel.Text = $"{strings.Level}: -";

            lblSPPoints.Text = $"{strings.Points}: {Bot.SteamPortal.Points}";
            lblSPLevel.Text = $"{strings.Level}: -";

            lblSTPoints.Text = $"{strings.Points}: -";
            lblSTLevel.Text = $"{strings.Level}: -";

            lblPBPoints.Text = $"{strings.Points}: {Bot.PlayBlink.Points}";
            lblPBLevel.Text = $"{strings.Level}: {Bot.PlayBlink.Level}";
        }

        private async Task<bool> LoginCheck()
        {
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = 7;
            toolStripProgressBar1.Visible = true;
            toolStripStatusLabel1.Image = Resources.load;
            toolStripStatusLabel1.Text = strings.TryLogin;
            var login = false;

            if (Bot.GameMiner.Enabled)
            {
                if (await CheckLoginGm())
                {
                    var won = await Parse.GameMinerWonParseAsync(Bot);
                    if (won != null && won.Content != "\n")
                    {
                        WriteLog(won);
                        if (Properties.Settings.Default.ShowWonTip)
                        {
                            ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info);
                        }
                    }

                    LoadProfilesInfo?.Invoke();

                    login = true;
                    btnGMLogin.Enabled = false;
                    btnGMLogin.Visible = false;
                    lblGMStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
                    LoadProfilesInfo?.Invoke();
                    pbGMReload.Visible = true;
                    btnGMExit.Visible = true;
                }
                else
                {
                    BlockTabpage(tabPageGM, false);
                    btnGMLogin.Enabled = true;
                    btnGMLogin.Visible = true;
                    lblGMStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
                }
            }
            else
            {
                BlockTabpage(tabPageGM, false);
                btnGMLogin.Enabled = true;
                btnGMLogin.Visible = true;
            }
            toolStripProgressBar1.Value++;

            if (Bot.SteamGifts.Enabled)
            {
                if (await CheckLoginSg())
                {
                    var won = await Parse.SteamGiftsWonParseAsync(Bot);
                    if (won != null && won.Content != "\n")
                    {
                        WriteLog(won);
                        if (Properties.Settings.Default.ShowWonTip)
                        {
                            ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info);
                        }
                    }
                    LoadProfilesInfo?.Invoke();

                    login = true;
                    btnSGLogin.Enabled = false;
                    btnSGLogin.Visible = false;
                    lblSGStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
                    pbSGReload.Visible = true;
                    btnSGExit.Visible = true;
                }
                else
                {
                    BlockTabpage(tabPageSG, false);
                    btnSGLogin.Enabled = true;
                    btnSGLogin.Visible = true;
                    lblSGStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
                }
            }
            else
            {
                BlockTabpage(tabPageSG, false);
                btnSGLogin.Enabled = true;
                btnSGLogin.Visible = true;
            }
            toolStripProgressBar1.Value++;

            if (Bot.SteamCompanion.Enabled)
            {
                if (await CheckLoginSc())
                {
                    var won = await Parse.SteamCompanionWonParseAsync(Bot);
                    if (won != null && won.Content != "\n")
                    {
                        WriteLog(won);
                        if (Properties.Settings.Default.ShowWonTip)
                        {
                            ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info);
                        }
                    }
                    LoadProfilesInfo?.Invoke();

                    login = true;
                    btnSCLogin.Enabled = false;
                    btnSCLogin.Visible = false;
                    lblSCStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
                    pbSCReload.Visible = true;
                    btnSCExit.Visible = true;
                }
                else
                {
                    BlockTabpage(tabPageSC, false);
                    btnSCLogin.Enabled = true;
                    btnSCLogin.Visible = true;
                    lblSCStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
                }
            }
            else
            {
                BlockTabpage(tabPageSC, false);
                btnSCLogin.Enabled = true;
                btnSCLogin.Visible = true;
            }
            toolStripProgressBar1.Value++;

            if (Bot.SteamPortal.Enabled)
            {
                if (await CheckLoginSp())
                {
                    var won = await Parse.SteamPortalWonParsAsync(Bot);
                    if (won != null)
                    {
                        WriteLog(won);
                        if (Properties.Settings.Default.ShowWonTip)
                        {
                            ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info);
                        }
                    }

                    login = true;
                    btnSPLogin.Enabled = false;
                    btnSPLogin.Visible = false;
                    lblSPStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
                    pbSPReload.Visible = true;
                    btnSPExit.Visible = true;
                }
                else
                {
                    BlockTabpage(tabPageSP, false);
                    btnSPLogin.Enabled = true;
                    btnSPLogin.Visible = true;
                    lblSPStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
                }
            }
            else
            {
                BlockTabpage(tabPageSP, false);
                btnSPLogin.Enabled = true;
                btnSPLogin.Visible = true;
            }
            toolStripProgressBar1.Value++;

            if (Bot.SteamTrade.Enabled)
            {
                if (await CheckLoginSt())
                {
                    login = true;
                    btnSTLogin.Enabled = false;
                    btnSTLogin.Visible = false;
                    lblSTStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
                    pbSTreload.Visible = true;
                    btnSTExit.Visible = true;
                }
                else
                {
                    BlockTabpage(tabPageST, false);
                    btnSTLogin.Enabled = true;
                    btnSTLogin.Visible = true;
                    lblSTStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
                }
            }
            else
            {
                BlockTabpage(tabPageST, false);
                btnSTLogin.Enabled = true;
                btnSTLogin.Visible = true;
            }
            toolStripProgressBar1.Value++;

            if (Bot.PlayBlink.Enabled)
            {
                if (await CheckLoginPb())
                {
                    LoadProfilesInfo?.Invoke();

                    login = true;
                    btnPBLogin.Enabled = false;
                    btnPBLogin.Visible = false;
                    lblPBStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
                    LoadProfilesInfo?.Invoke();
                    pbPBRefresh.Visible = true;
                    btnPBExit.Visible = true;
                }
                else
                {
                    BlockTabpage(tabPagePB, false);
                    btnPBLogin.Enabled = true;
                    btnPBLogin.Visible = true;
                    lblPBStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
                }
            }
            else
            {
                BlockTabpage(tabPagePB, false);
                btnPBLogin.Enabled = true;
                btnPBLogin.Visible = true;
            }
            toolStripProgressBar1.Value++;

            if (Bot.Steam.Enabled)
            {
                if (await CheckLoginSteam())
                {
                    btnSteamLogin.Enabled = false;
                    btnSteamLogin.Visible = false;
                    lblSteamStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
                    btnSteamExit.Visible = true;
                }
                else
                {
                    BlockTabpage(tabPageSteam, false);
                    btnSteamLogin.Enabled = true;
                    btnSteamLogin.Visible = true;
                    lblSteamStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
                }
            }
            else
            {
                BlockTabpage(tabPageSteam, false);
                btnSteamLogin.Enabled = true;
                btnSteamLogin.Visible = true;
            }
            toolStripProgressBar1.Value++;

            toolStripProgressBar1.Visible = false;
            toolStripStatusLabel1.Image = null;
            toolStripStatusLabel1.Text = strings.StatusBar_End;
            return login;
        }

        private void BlockTabpage(TabPage tabPage, bool state)
        {
            foreach (Control control in tabPage.Controls)
            {
                control.Enabled = control.GetType().FullName == "System.Windows.Forms.LinkLabel" || state;
            }
        }

        private async void btnSTLogin_Click(object sender, EventArgs e)
        {
            btnSTLogin.Enabled = false;
            var first = Web.Get(Links.SteamTrade, new List<Parameter>(), new CookieContainer(),
                new List<HttpHeader>(), "");
            var getLoginHref = Web.SteamTradeDoAuth($"{Links.SteamTrade}reg.php?login",
                Generate.LoginData_SteamTrade(), first.Cookies, new List<HttpHeader>());
            var location = Tools.GetLocationInresponse(getLoginHref.RestResponse);
            var cookie = Tools.GetSessCookieInresponse(getLoginHref.Cookies, "steamtrade.info", "PHPSESSID");

            BrowserStart(location, Links.SteamTrade, "SteamTrade - Login", cookie);
            Bot.Save();

            toolStripStatusLabel1.Image = Resources.load;
            toolStripStatusLabel1.Text = strings.StatusBar_Login;
            var login = await CheckLoginSt();
            if (login)
            {
                BlockTabpage(tabPageST, true);
                btnSTLogin.Enabled = false;
                btnSTLogin.Visible = false;
                lblSTStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
                LoadProfilesInfo?.Invoke();
                btnStart.Enabled = true;
                pbSTreload.Visible = true;
                btnSTExit.Visible = true;
                btnSTExit.Enabled = true;
                cbSTEnable.Checked = true;
                Bot.SteamTrade.Enabled = true;
            }
            else
            {
                lblSTStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
                BlockTabpage(tabPageST, false);
                btnSTLogin.Enabled = true;
                btnSTLogin.Visible = true;
            }
            toolStripStatusLabel1.Image = null;
            toolStripStatusLabel1.Text = strings.StatusBar_End;
        }

        public static void BrowserStart(string startPage, string endPage, string title, string phpSessId)
        {
            Form form = new Browser(Bot, startPage, endPage, title, phpSessId);
            form.Height = Screen.PrimaryScreen.Bounds.Height/2;
            form.Width = Screen.PrimaryScreen.Bounds.Width/2;
            form.Name = "Browser";
            form.ShowDialog();
        }

        private async void btnSPLogin_Click(object sender, EventArgs e)
        {
            btnSPLogin.Enabled = false;
            BrowserStart("http://steamportal.net/page/steam", Links.SteamPortal, "SteamPortal - Login", "");
            Bot.Save();

            toolStripStatusLabel1.Image = Resources.load;
            toolStripStatusLabel1.Text = strings.StatusBar_Login;
            var login = await CheckLoginSp();
            if (login)
            {
                var won = await Parse.SteamPortalWonParsAsync(Bot);
                if (won != null && won.Content != "\n")
                {
                    WriteLog(won);
                }

                BlockTabpage(tabPageSP, true);
                btnSPLogin.Enabled = false;
                btnSPLogin.Visible = false;
                lblSPStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
                LoadProfilesInfo?.Invoke();
                btnStart.Enabled = true;
                pbSPReload.Visible = true;
                btnSPExit.Visible = true;
                btnSPExit.Enabled = true;
                cbSPEnable.Checked = true;
                Bot.SteamPortal.Enabled = true;
            }
            else
            {
                lblSPStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
                BlockTabpage(tabPageSP, false);
                btnSPLogin.Enabled = true;
                btnSPLogin.Visible = true;
            }
            toolStripStatusLabel1.Image = null;
            toolStripStatusLabel1.Text = strings.StatusBar_End;
        }

        private async void btnSCLogin_Click(object sender, EventArgs e)
        {
            btnSCLogin.Enabled = false;
            BrowserStart($"{Links.SteamCompanion}login", Links.SteamCompanion, "SteamCompanion - Login", "");
            Bot.Save();

            toolStripStatusLabel1.Image = Resources.load;
            toolStripStatusLabel1.Text = strings.StatusBar_Login;
            var login = await CheckLoginSc();
            if (login)
            {
                var won = await Parse.SteamCompanionWonParseAsync(Bot);
                if (won != null && won.Content != "\n")
                {
                    WriteLog(won);
                }

                BlockTabpage(tabPageSC, true);
                btnSCLogin.Enabled = false;
                btnSCLogin.Visible = false;
                lblSCStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
                LoadProfilesInfo?.Invoke();
                btnStart.Enabled = true;
                pbSCReload.Visible = true;
                btnSCExit.Visible = true;
                btnSCExit.Enabled = true;
                cbSCEnable.Checked = true;
                Bot.SteamCompanion.Enabled = true;
            }
            else
            {
                lblSCStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
                BlockTabpage(tabPageSC, false);
                btnSCLogin.Enabled = true;
                btnSCLogin.Visible = true;
            }
            toolStripStatusLabel1.Image = null;
            toolStripStatusLabel1.Text = strings.StatusBar_End;
        }

        private async void btnSGLogin_Click(object sender, EventArgs e)
        {
            btnSGLogin.Enabled = false;
            BrowserStart($"{Links.SteamGifts}?login", Links.SteamGifts, "SteamGifts - Login", "");

			if(string.IsNullOrEmpty(Bot.UserAgent))
			{
				Bot.UserAgent = Tools.UserAgent();
			}
			Bot.Save();

			toolStripStatusLabel1.Image = Resources.load;
            toolStripStatusLabel1.Text = strings.StatusBar_Login;
            var login = await CheckLoginSg();
            if (login)
            {
                var won = await Parse.SteamGiftsWonParseAsync(Bot);
                if (won != null && won.Content != "\n")
                {
                    WriteLog(won);
                }
                BlockTabpage(tabPageSG, true);
                btnSGLogin.Enabled = false;
                btnSGLogin.Visible = false;
                lblSGStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
                LoadProfilesInfo?.Invoke();
                btnStart.Enabled = true;
                pbSGReload.Visible = true;
                btnSGExit.Visible = true;
                btnSGExit.Enabled = true;
                cbSGEnable.Checked = true;
                Bot.SteamGifts.Enabled = true;
            }
            else
            {
                lblSGStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
                BlockTabpage(tabPageSG, false);
                btnSGLogin.Enabled = true;
                btnSGLogin.Visible = true;
            }
            toolStripStatusLabel1.Image = null;
            toolStripStatusLabel1.Text = strings.StatusBar_End;
        }

        private async void btnGMLogin_Click(object sender, EventArgs e)
        {
            btnGMLogin.Enabled = false;
            BrowserStart($"{Links.GameMiner}login/steam?backurl=http%3A%2F%2Fgameminer.net%2F%3Flang%3D" +
                Properties.Settings.Default.Lang + @"&agree=True",
                "http://gameminer.net/?lang=" + Properties.Settings.Default.Lang, "GameMiner - Login", "");

            if (string.IsNullOrEmpty(Bot.UserAgent))
            {
                Bot.UserAgent = Tools.UserAgent();
            }
            Bot.Save();

            toolStripStatusLabel1.Image = Resources.load;
            toolStripStatusLabel1.Text = strings.StatusBar_Login;
            var login = await CheckLoginGm();
            if (login)
            {
                var won = await Parse.GameMinerWonParseAsync(Bot);
                if (won != null && won.Content != "\n")
                {
                    WriteLog(won);
                }
                BlockTabpage(tabPageGM, true);
                btnGMLogin.Enabled = false;
                btnGMLogin.Visible = false;
                lblGMStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
                LoadProfilesInfo?.Invoke();
                btnStart.Enabled = true;
                pbGMReload.Visible = true;
                btnGMExit.Visible = true;
                btnGMExit.Enabled = true;
                cbGMEnable.Checked = true;
                Bot.GameMiner.Enabled = true;
            }
            else
            {
                lblGMStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
                BlockTabpage(tabPageGM, false);
                btnGMLogin.Enabled = true;
                btnGMLogin.Visible = true;
            }
            toolStripStatusLabel1.Image = null;
            toolStripStatusLabel1.Text = strings.StatusBar_End;
        }

        private async Task<bool> CheckLoginGm()
        {
            Message_TryLogin("GameMiner");
            var login = await Parse.GameMinerGetProfileAsync(Bot);
            WriteLog(login);
            return login.Success;
        }

        private async Task<bool> CheckLoginSg()
        {
            Message_TryLogin("SteamGifts");
            var login = await Parse.SteamGiftsGetProfileAsync(Bot);
            WriteLog(login);
            return login.Success;
        }

        private async Task<bool> CheckLoginSc()
        {
            Message_TryLogin("SteamCompanion");
            var login = await Parse.SteamCompanionGetProfileAsync(Bot);
            WriteLog(login);
            return login.Success;
        }

        private async Task<bool> CheckLoginSp()
        {
            Message_TryLogin("SteamPortal");
            var login = await Parse.SteamPortalGetProfileAsync(Bot);
            WriteLog(login);
            return login.Success;
        }

        private async Task<bool> CheckLoginSt()
        {
            Message_TryLogin("SteamTrade");
            var login = await Parse.SteamTradeGetProfileAsync(Bot);
            WriteLog(login);
            return login.Success;
        }

        private async Task<bool> CheckLoginSteam()
        {
            Message_TryLogin("Steam");
            var login = await Parse.SteamGetProfileAsync(Bot);
            WriteLog(login);
            return login.Success;
        }

        private async Task<bool> CheckLoginPb()
        {
            Message_TryLogin("PlayBlink");
            var login = await Parse.PlayBlinkGetProfileAsync(Bot);
            WriteLog(login);
            return login.Success;
        }

        private async void pbGMReload_Click(object sender, EventArgs e)
        {
            btnGMExit.Enabled = false;
            pbGMReload.Image = Resources.load;
            SetStatusPanel("Обновление информации о GameMiner", Resources.load);

            if (await CheckLoginGm())
            {
                LoadProfilesInfo?.Invoke();
                var won = await Parse.GameMinerWonParseAsync(Bot);
                if (won != null && won.Content != "\n")
                {
                    WriteLog(won);
                }

                var async = await Web.GameMinerSyncAccountAsync(Bot);
                if (async != null)
                {
                    WriteLog(async);
                }

                btnGMLogin.Enabled = false;
                btnGMLogin.Visible = false;
                lblGMStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
                LoadProfilesInfo?.Invoke();
                BlockTabpage(tabPageGM, true);
            }
            else
            {
                BlockTabpage(tabPageGM, false);
                btnGMLogin.Enabled = true;
                btnGMLogin.Visible = true;
                lblGMStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
            }

            SetStatusPanel(strings.Finish, null);
            pbGMReload.Image = Resources.refresh;
            btnGMExit.Enabled = true;
        }

        private async void pbSGReload_Click(object sender, EventArgs e)
        {
            btnGMExit.Enabled = false;
            pbSGReload.Image = Resources.load;
            SetStatusPanel("Обновление информации о SteamGifts", Resources.load);

            if (await CheckLoginSg())
            {
                LoadProfilesInfo?.Invoke();
                var won = await Parse.SteamGiftsWonParseAsync(Bot);
                if (won != null)
                {
                    WriteLog(won);
                }

                var async = await Web.SteamGiftsSyncAccountAsync(Bot);
                if (async != null)
                {
                    WriteLog(async);
                }

                btnSGLogin.Enabled = false;
                btnSGLogin.Visible = false;
                lblSGStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
                BlockTabpage(tabPageSG, true);
            }
            else
            {
                BlockTabpage(tabPageSG, false);
                btnSGLogin.Enabled = true;
                btnSGLogin.Visible = true;
                lblSGStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
            }

            SetStatusPanel(strings.Finish, null);
            pbSGReload.Image = Resources.refresh;
            btnGMExit.Enabled = true;
        }

        private async void pbSCReload_Click(object sender, EventArgs e)
        {
            btnGMExit.Enabled = false;
            pbSCReload.Image = Resources.load;
            SetStatusPanel("Обновление информации о SteamCompanion", Resources.load);

            if (await CheckLoginSc())
            {
                LoadProfilesInfo?.Invoke();
                var won = await Parse.SteamCompanionWonParseAsync(Bot);
                if (won != null)
                {
                    WriteLog(won);
                }
                btnSCLogin.Enabled = false;
                btnSCLogin.Visible = false;
                lblSCStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
                BlockTabpage(tabPageSC, true);

                var async = await Web.SteamCompanionSyncAccountAsync(Bot);
                if (async != null)
                {
                    WriteLog(async);
                }
            }
            else
            {
                BlockTabpage(tabPageSC, false);
                btnSCLogin.Enabled = true;
                btnSCLogin.Visible = true;
                lblSCStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
            }

            SetStatusPanel(strings.Finish, null);
            pbSCReload.Image = Resources.refresh;
            btnGMExit.Enabled = true;
        }

        private async void pbSPReload_Click(object sender, EventArgs e)
        {
            btnGMExit.Enabled = false;
            pbSPReload.Image = Resources.load;
            SetStatusPanel("Обновление информации о SteamPortal", Resources.load);

            if (await CheckLoginSp())
            {
                LoadProfilesInfo?.Invoke();
                btnSPLogin.Enabled = false;
                btnSPLogin.Visible = false;
                lblSPStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
                BlockTabpage(tabPageSP, true);
            }
            else
            {
                BlockTabpage(tabPageSP, false);
                btnSPLogin.Enabled = true;
                btnSPLogin.Visible = true;
                lblSPStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
            }

            pbSPReload.Image = Resources.refresh;
            SetStatusPanel(strings.Finish, null);
            btnGMExit.Enabled = true;
        }

        private async void pbSTreload_Click(object sender, EventArgs e)
        {
            btnGMExit.Enabled = false;
            pbSTreload.Image = Resources.load;
            SetStatusPanel("Обновление информации о SteamTrade", Resources.load);

            if (await CheckLoginSt())
            {
                LoadProfilesInfo?.Invoke();
                btnSTLogin.Enabled = false;
                btnSTLogin.Visible = false;
                lblSTStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
                BlockTabpage(tabPageST, true);
            }
            else
            {
                BlockTabpage(tabPageST, false);
                btnSTLogin.Enabled = true;
                btnSTLogin.Visible = true;
                lblSTStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
            }

            SetStatusPanel(strings.Finish, null);
            pbSTreload.Image = Resources.refresh;
            btnGMExit.Enabled = true;
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            notifyIcon.Visible = false;
            Show();
            WindowState = FormWindowState.Normal;
            Hided = false;
            if (LogActive)
            {
                LogUnHide?.Invoke();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Links.GameMiner);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Links.SteamGifts);
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Links.SteamCompanion);
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Links.SteamPortal);
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Links.SteamTrade);
        }

        private void статстикаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormStatistic();
            form.ShowDialog();
        }

        private void оПрограммеToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var form = new FormAbout();
            form.ShowDialog();
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Bot.Save())
            {
                WriteLog(new Log(Messages.GetDateTime() + " Настройки сохранены в profile.xml", Color.White, true, true));
            }

            Properties.Settings.Default.Save();
        }

        private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var saveFileDialog1 = new SaveFileDialog
            {
                Filter = @"XML|*.xml",
                Title = @"Сохранить профиль"
            };
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                if (Bot.Save(saveFileDialog1.FileName))
                {
                    WriteLog(new Log(Messages.GetDateTime() + " Файл сохранен по пути " + saveFileDialog1.FileName,
                        Color.White, true, true));
                }
                else
                {
                    WriteLog(new Log(Messages.GetDateTime() + " Файл НЕ сохранен", Color.Red, true, true));
                }
            }
        }

        private void загрузитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (lblSTStatus.Enabled)
            {
                BlockTabpage(tabPageST, false);
                cbSTEnable.Enabled = true;
                Bot.SteamTrade.Enabled = false;
            }
            else
            {
                BlockTabpage(tabPageST, true);
                Bot.SteamTrade.Enabled = true;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (lblSGStatus.Enabled)
            {
                BlockTabpage(tabPageSG, false);
                cbSGEnable.Enabled = true;
                Bot.SteamGifts.Enabled = false;
            }
            else
            {
                BlockTabpage(tabPageSG, true);
                Bot.SteamGifts.Enabled = true;
            }
        }

        private void cbGMEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (lblGMStatus.Enabled)
            {
                BlockTabpage(tabPageGM, false);
                cbGMEnable.Enabled = true;
                Bot.GameMiner.Enabled = false;
            }
            else
            {
                BlockTabpage(tabPageGM, true);
                Bot.GameMiner.Enabled = true;
            }
        }

        private void cbSPEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (lblSPStatus.Enabled)
            {
                BlockTabpage(tabPageSP, false);
                cbSPEnable.Enabled = true;
                Bot.SteamPortal.Enabled = false;
            }
            else
            {
                BlockTabpage(tabPageSP, true);
                Bot.SteamPortal.Enabled = true;
            }
        }

        private void cbSCEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (lblSCStatus.Enabled)
            {
                BlockTabpage(tabPageSC, false);
                cbSCEnable.Enabled = true;
                Bot.SteamCompanion.Enabled = false;
            }
            else
            {
                BlockTabpage(tabPageSC, true);
                Bot.SteamCompanion.Enabled = true;
            }
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Links.Steam);
        }

        private async void btnSteamLogin_Click(object sender, EventArgs e)
        {
            BrowserStart("https://steamcommunity.com/login/home/?goto=0", "http://steamcommunity.com/id/",
                "Steam - Login", "");
            Bot.Save();

            toolStripStatusLabel1.Image = Resources.load;
            toolStripStatusLabel1.Text = strings.StatusBar_Login;
            var login = await CheckLoginSteam();
            if (login)
            {
                await
                    Web.SteamJoinGroupAsync("http://steamcommunity.com/groups/krybot",
                        Generate.PostData_SteamGroupJoin(Bot.Steam.Cookies.Sessid), Generate.Cookies_Steam(Bot));

                BlockTabpage(tabPageSteam, true);
                btnSteamLogin.Enabled = false;
                btnSteamLogin.Visible = false;
                btnSteamExit.Enabled = true;
                btnSteamExit.Visible = true;

                lblSteamStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
                LoadProfilesInfo?.Invoke();
            }
            else
            {
                lblSteamStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
                BlockTabpage(tabPageSteam, false);
                btnSteamLogin.Enabled = true;
                btnSteamLogin.Visible = true;
            }
            toolStripStatusLabel1.Image = null;
            toolStripStatusLabel1.Text = strings.StatusBar_End;
        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                notifyIcon.Icon = Resources.KryBotPresent_256b;
                notifyIcon.Visible = true;
                Hided = true;
                if (LogActive)
                {
                    LogHide?.Invoke();
                }
                Hide();
            }
        }

        private void ShowBaloolTip(string content, int interval, ToolTipIcon icon)
        {
            notifyIcon.BalloonTipIcon = icon;
            notifyIcon.BalloonTipText = content;
            notifyIcon.BalloonTipTitle = Application.ProductName;
            notifyIcon.Tag = "";
            notifyIcon.BalloonTipClicked += NotifyIconOnBalloonTipClicked;
            notifyIcon.ShowBalloonTip(interval);
        }

        private void NotifyIconOnBalloonTipClicked(object sender, EventArgs eventArgs)
        {
            var tag = (string) notifyIcon.Tag;
            if (tag == "")
            {
                notifyIcon.Visible = false;
                Show();
                WindowState = FormWindowState.Normal;
                Hided = false;
            }
            else
            {
                Process.Start((string) notifyIcon.Tag);
            }
        }

        private void toolStripMenuItem_Show_Click(object sender, EventArgs e)
        {
            notifyIcon.Visible = false;
            Show();
            WindowState = FormWindowState.Normal;
            Hided = false;

            if (LogActive)
            {
                LogUnHide?.Invoke();
            }
        }

        private void toolStripMenuItem_Farm_Click(object sender, EventArgs e)
        {
            btnStart_Click(sender, e);
        }

        private void toolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void донатToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormDonate();
            form.ShowDialog();
        }

        private void btnSteamExit_Click(object sender, EventArgs e)
        {
            Bot.Steam.Logout();
            BlockTabpage(tabPageSteam, false);
            btnSteamLogin.Visible = true;
            btnSteamLogin.Enabled = true;
            btnSteamExit.Visible = false;
            Bot.Save();
        }

        private void btnSTExit_Click(object sender, EventArgs e)
        {
            Bot.SteamTrade.Logout();
            BlockTabpage(tabPageST, false);
            btnSTLogin.Visible = true;
            btnSTExit.Visible = false;
            btnSTLogin.Enabled = true;
            Bot.Save();
        }

        private void btnSPExit_Click(object sender, EventArgs e)
        {
            Bot.SteamPortal.Logout();
            BlockTabpage(tabPageSP, false);
            btnSPLogin.Visible = true;
            btnSPLogin.Enabled = true;
            btnSPExit.Visible = false;
            Bot.Save();
        }

        private void btnSCExit_Click(object sender, EventArgs e)
        {
            Bot.SteamCompanion.Logout();
            BlockTabpage(tabPageSC, false);
            btnSCLogin.Visible = true;
            btnSCLogin.Enabled = true;
            btnSCExit.Visible = false;
            Bot.Save();
        }

        private void btnSGExit_Click(object sender, EventArgs e)
        {
            Bot.SteamGifts.Logout();
            BlockTabpage(tabPageSG, false);
            btnSGLogin.Visible = true;
            btnSGLogin.Enabled = true;
            btnSGExit.Visible = false;
            Bot.Save();
        }

        private void btnGMExit_Click(object sender, EventArgs e)
        {
            Bot.GameMiner.Logout();
            BlockTabpage(tabPageGM, false);
            btnGMLogin.Enabled = true;
            btnGMLogin.Visible = true;
            btnGMExit.Visible = false;
            Bot.Save();
        }

        private void вПапкуСБотомToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", Environment.CurrentDirectory);
        }

        private void черныйСписокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormBlackList(Bot);
            form.ShowDialog();
            BlackList = Tools.LoadBlackList();
        }

        private void настройкиToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var form = new FormSettings(Bot);
            form.ShowDialog();
        }

        private async void btnPBLogin_Click(object sender, EventArgs e)
        {
            btnPBLogin.Enabled = false;
            BrowserStart("http://playblink.com/?do=login&act=signin", Links.PlayBlink, "PlayBlink - Login", "");
            Bot.Save();

            toolStripStatusLabel1.Image = Resources.load;
            toolStripStatusLabel1.Text = strings.StatusBar_Login;
            var login = await CheckLoginPb();
            if (login)
            {
                BlockTabpage(tabPagePB, true);
                btnPBLogin.Enabled = false;
                btnPBLogin.Visible = false;
                lblPBStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
                LoadProfilesInfo?.Invoke();
                btnStart.Enabled = true;
                pbPBRefresh.Visible = true;
                btnPBExit.Visible = true;
                btnPBExit.Enabled = true;
                cbPBEnabled.Checked = true;
                Bot.PlayBlink.Enabled = true;
            }
            else
            {
                lblPBStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
                BlockTabpage(tabPagePB, false);
                btnPBLogin.Enabled = true;
                btnPBLogin.Visible = true;
            }
            toolStripStatusLabel1.Image = null;
            toolStripStatusLabel1.Text = strings.StatusBar_End;
        }

        private void btnPBExit_Click(object sender, EventArgs e)
        {
            Bot.PlayBlink.Logout();
            BlockTabpage(tabPagePB, false);
            btnPBLogin.Enabled = true;
            btnPBLogin.Visible = true;
            btnPBExit.Visible = false;
            Bot.Save();
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Links.PlayBlink);
        }

        private async void pbPBRefresh_Click(object sender, EventArgs e)
        {
            btnPBExit.Enabled = false;
            pbPBRefresh.Image = Resources.load;
            SetStatusPanel("Обновление информации о PlayBlink", Resources.load);

            if (await CheckLoginPb())
            {
                LoadProfilesInfo?.Invoke();
                btnPBLogin.Enabled = false;
                btnPBLogin.Visible = false;
                lblPBStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
                BlockTabpage(tabPagePB, true);
            }
            else
            {
                BlockTabpage(tabPagePB, false);
                btnPBLogin.Enabled = true;
                btnPBLogin.Visible = true;
                lblPBStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
            }

            SetStatusPanel(strings.Finish, null);
            pbPBRefresh.Image = Resources.refresh;
            btnPBExit.Enabled = true;
        }

        private async Task<bool> JoinGiveaways(List<GameMinerGiveaway> giveaways)
        {
            foreach (var giveaway in giveaways)
            {
                if (giveaway.Price <= Bot.GameMiner.JoinCoalLimit && giveaway.Price <= Bot.GameMiner.Coal)
                {
                    if (Bot.GameMiner.CoalReserv <= Bot.GameMiner.Coal - giveaway.Price || giveaway.Price == 0)
                    {
                        var data = await Web.GameMinerJoinGiveawayAsync(Bot, giveaway);
                        if (data != null && data.Content != "\n")
                        {
                            if (Properties.Settings.Default.FullLog)
                            {
                                WriteLog(data);
                            }
                            else
                            {
                                if (data.Color != Color.Yellow && data.Color != Color.Red)
                                {
                                    WriteLog(data);
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        private async Task<bool> JoinGiveaways(List<SteamGiftsGiveaway> giveaways, bool wishlist)
        {
            foreach (var giveaway in giveaways)
            {
                if (wishlist)
                {
                    if (giveaway.Price <= Bot.SteamGifts.Points)
                    {
                        var data = await Web.SteamGiftsJoinGiveawayAsync(Bot, giveaway);
                        if (data != null && data.Content != "\n")
                        {
                            if (Properties.Settings.Default.FullLog)
                            {
                                WriteLog(data);
                            }
                            else
                            {
                                if (data.Color != Color.Yellow && data.Color != Color.Red)
                                {
                                    WriteLog(data);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (giveaway.Price <= Bot.SteamGifts.Points &&
                        Bot.SteamGifts.PointsReserv <= Bot.SteamGifts.Points - giveaway.Price)
                    {
                        var data = await Web.SteamGiftsJoinGiveawayAsync(Bot, giveaway);
                        if (data != null && data.Content != "\n")
                        {
                            if (Properties.Settings.Default.FullLog)
                            {
                                WriteLog(data);
                            }
                            else
                            {
                                if (data.Color != Color.Yellow && data.Color != Color.Red)
                                {
                                    WriteLog(data);
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        private async Task<bool> JoinGiveaways(List<SteamCompanionGiveaway> giveaways, bool wishlist)
        {
            foreach (var giveaway in giveaways)
            {
                if (wishlist)
                {
                    if (giveaway.Price <= Bot.SteamCompanion.Points)
                    {
                        var data = await Web.SteamCompanionJoinGiveawayAsync(Bot, giveaway);
                        if (data != null && data.Content != "\n")
                        {
                            if (Properties.Settings.Default.FullLog)
                            {
                                WriteLog(data);
                            }
                            else
                            {
                                if (data.Color != Color.Yellow && data.Color != Color.Red)
                                {
                                    WriteLog(data);
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (giveaway.Price <= Bot.SteamCompanion.Points &&
                        Bot.SteamCompanion.PointsReserv <= Bot.SteamCompanion.Points - giveaway.Price)
                    {
                        var data = await Web.SteamCompanionJoinGiveawayAsync(Bot, giveaway);
                        if (data != null && data.Content != "\n")
                        {
                            if (Properties.Settings.Default.FullLog)
                            {
                                WriteLog(data);
                            }
                            else
                            {
                                if (data.Color != Color.Yellow && data.Color != Color.Red)
                                {
                                    WriteLog(data);
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        private async Task<bool> JoinGiveaways(List<SteamPortalGiveaway> giveaways)
        {
            foreach (var giveaway in giveaways)
            {
                if (giveaway.Price <= Bot.SteamPortal.Points &&
                    Bot.SteamPortal.PointsReserv <= Bot.SteamPortal.Points - giveaway.Price)
                {
                    var data = await Web.SteamPortalJoinGiveawayAsync(Bot, giveaway);
                    if (data != null && data.Content != "\n")
                    {
                        if (Properties.Settings.Default.FullLog)
                        {
                            WriteLog(data);
                        }
                        else
                        {
                            if (data.Color != Color.Yellow && data.Color != Color.Red)
                            {
                                WriteLog(data);
                            }
                        }
                    }
                }
            }
            return true;
        }

        private async Task<bool> JoinGiveaways(List<SteamTradeGiveaway> giveaways)
        {
            foreach (var giveaway in giveaways)
            {
                var data = await Web.SteamTradeJoinGiveawayAsync(Bot, giveaway);
                if (data != null && data.Content != "\n")
                {
                    if (Properties.Settings.Default.FullLog)
                    {
                        WriteLog(data);
                    }
                    else
                    {
                        if (data.Color != Color.Yellow && data.Color != Color.Red)
                        {
                            WriteLog(data);
                        }
                    }
                }
            }
            return true;
        }

        private async Task<bool> JoinGiveaways(List<PlayBlinkGiveaway> giveaways)
        {
            foreach (var giveaway in giveaways)
            {
                if (giveaway.Price <= Bot.PlayBlink.MaxJoinValue && giveaway.Price <= Bot.PlayBlink.Points)
                {
                    if (Bot.PlayBlink.PointReserv <= Bot.PlayBlink.Points - giveaway.Price || giveaway.Price == 0)
                    {
                        var data = await Web.PlayBlinkJoinGiveawayAsync(Bot, giveaway);
                        if (data != null && data.Content != "\n")
                        {
                            if (Properties.Settings.Default.FullLog)
                            {
                                WriteLog(data);
                            }
                            else
                            {
                                if (data.Color != Color.Yellow && data.Color != Color.Red)
                                {
                                    WriteLog(data);
                                }
                            }

                            //if (data.Content.Contains("Captcha"))
                            //{
                            //    break;
                            //}
                        }
                    }
                }
            }
            return true;
        }

        private void Message_TryLogin(string site)
        {
            if (Properties.Settings.Default.FullLog)
            {
                WriteLog(new Log($"{Messages.GetDateTime()} {{{site}}} {strings.TryLogin}", Color.White, true, true));
            }
        }

        private void SetStatusPanel(string text, Image image)
        {
            toolStripStatusLabel1.Image = image;
            toolStripStatusLabel1.Text = text;
        }

        private void cbPBEnabled_CheckedChanged(object sender, EventArgs e)
        {
            if (lblPBStatus.Enabled)
            {
                BlockTabpage(tabPagePB, false);
                cbPBEnabled.Enabled = true;
                Bot.PlayBlink.Enabled = false;
            }
            else
            {
                BlockTabpage(tabPagePB, true);
                Bot.PlayBlink.Enabled = true;
            }
        }

        private void WriteLog(Log log)
        {
            LogBuffer = log;
            LogChanged?.Invoke();
        }
    }
}