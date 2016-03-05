using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using KryBot.lang;
using KryBot.Properties;
using Newtonsoft.Json;
using RestSharp;

namespace KryBot
{
    public partial class FormMain : Form
    {
        public delegate void SubscribesContainer();

        public static Classes.Bot Bot = new Classes.Bot();
        public static List<GameMiner.GmGiveaway> GmGiveaways = new List<GameMiner.GmGiveaway>();
        public static List<SteamGifts.SgGiveaway> SgGiveaways = new List<SteamGifts.SgGiveaway>();
        public static List<SteamCompanion.ScGiveaway> ScGiveaways = new List<SteamCompanion.ScGiveaway>();
        public static List<SteamPortal.SpGiveaway> SpGiveaways = new List<SteamPortal.SpGiveaway>();
        public static List<SteamTrade.StGiveaway> StGiveaways = new List<SteamTrade.StGiveaway>();
        public static List<PlayBlink.PbGiveaway> PbGiveaways = new List<PlayBlink.PbGiveaway>();
        public static string[] BlackList;

        public static bool Hided;

        private readonly Timer _timer = new Timer();
        private readonly Timer _timerTickCount = new Timer();
        private bool _farming;
        private int _interval;
        public bool LogActive;

        public Classes.Log LogBuffer;

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
            if (Settings.Default.FirstExecute)
            {
                var dr = MessageBox.Show(strings.Licens, @"KryBot- Соглашение", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information);
                if (dr == DialogResult.No)
                {
                    Application.Exit();
                }
                else if (dr == DialogResult.Yes)
                {
                    Settings.Default.FirstExecute = false;
                }
            }

            Tools.CheckDlls();
            Tools.LoadSettings();
            LoadProfilesInfo += ShowProfileInfo;
            LogActive = Settings.Default.LogActive;
            Design();
            BlackList = Tools.LoadBlackList();

            if (LoadProfile())
            {
                await CheckForUpdates();
                cbGMEnable.Checked = Bot.GameMinerEnabled;
                cbSGEnable.Checked = Bot.SteamGiftsEnabled;
                cbSCEnable.Checked = Bot.SteamCompanionEnabled;
                cbSPEnable.Checked = Bot.SteamPortalEnabled;
                cbSTEnable.Checked = Bot.SteamTradeEnabled;
                btnStart.Enabled = await LoginCheck();
            }
            else
            {
                await CheckForUpdates();
                btnGMLogin.Visible = true;
                btnSGLogin.Visible = true;
                btnSCLogin.Visible = true;
                btnSPLogin.Visible = true;
                btnSTLogin.Visible = true;
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
            if (Settings.Default.Timer)
            {
                if (!_farming)
                {
                    if (Settings.Default.TimerLoops > 0)
                    {
                        for (var i = 0; i < Settings.Default.TimerLoops; i++)
                        {
                            _timer.Interval = Settings.Default.TimerInterval;
                            _interval = _timer.Interval;
                            _timerTickCount.Interval = 1000;
                            _timer.Tick += timer_Tick;
                            _timerTickCount.Tick += TimerTickCountOnTick;
                            _timer.Start();
                            _timerTickCount.Start();
                            btnStart.Text = @"Стоп (" + TimeSpan.FromMilliseconds(_timer.Interval) + @")";
                            if (Settings.Default.ShowFarmTip)
                            {
                                ShowBaloolTip(
                                    "Фарм запущен с интервалом " + _interval/60000 + "м (" + (Settings.Default.TimerLoops - i) +
                                    " циклов осталось)", 5000, ToolTipIcon.Info, "");
                            }
                            await DoFarm();
                            if (Settings.Default.ShowFarmTip)
                            {
                                ShowBaloolTip("Фарм завершен", 5000, ToolTipIcon.Info, "");
                            }
                        }
                    }
                    else if (Settings.Default.TimerLoops == 0)
                    {
                        _timer.Interval = Settings.Default.TimerInterval;
                        _interval = _timer.Interval;
                        _timerTickCount.Interval = 1000;
                        _timer.Tick += timer_Tick;
                        _timerTickCount.Tick += TimerTickCountOnTick;
                        _timer.Start();
                        _timerTickCount.Start();
                        btnStart.Text = @"Стоп (" + TimeSpan.FromMilliseconds(_timer.Interval) + @")";
                        if (Settings.Default.ShowFarmTip)
                        {
                            ShowBaloolTip("Фарм запущен с интервалом " + _interval/60000 + "м", 5000, ToolTipIcon.Info,
                                "");
                        }
                        await DoFarm();
                        if (Settings.Default.ShowFarmTip)
                        {
                            ShowBaloolTip("Фарм завершен", 5000, ToolTipIcon.Info, "");
                        }
                    }
                }
                else
                {
                    _timer.Stop();
                    _timerTickCount.Stop();
                    btnStart.Text = @"Старт";
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
                    btnStart.Text = @"Выполняется...";
                    if (Settings.Default.ShowFarmTip)
                    {
                        ShowBaloolTip("Фарм запущен...", 5000, ToolTipIcon.Info, "");
                    }
                    await DoFarm();
                    if (Settings.Default.ShowFarmTip)
                    {
                        ShowBaloolTip("Фарм завершен", 5000, ToolTipIcon.Info, "");
                    }
                    btnStart.Text = @"Старт";
                    btnStart.Enabled = true;
                }
                else
                {
                    LogBuffer =
                        Tools.ConstructLog(
                            Messages.GetDateTime() + "Пропуск попытки фарма. Предыдущий цикл фарма еше не окончен.",
                            Color.Red, false, true);
                    LogChanged?.Invoke();
                    btnStart.Text = @"Старт";
                }
            }
        }

        private void TimerTickCountOnTick(object sender, EventArgs eventArgs)
        {
            _interval -= 1000;
            btnStart.Text = @"Стоп (" + TimeSpan.FromMilliseconds(_interval) + @")";
        }

        private void Design()
        {
            Text = Application.ProductName + @" [" + Application.ProductVersion + @"]";
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
                    LogBuffer = message;
                    LogChanged?.Invoke();

                    MessageBox.Show(message.Content.Split(']')[1], @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                LogBuffer = Messages.ProfileLoaded();
                LogChanged?.Invoke();
                return true;
            }
            return false;
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings.Default.LogActive = LogActive;
            Tools.SaveProfile(Bot, "");
            Tools.SaveSettings();
            Settings.Default.JoinsPerSession = 0;
            Settings.Default.JoinsLoops = 0;
            Settings.Default.Save();
        }

        private void OpenLog()
        {
            логToolStripMenuItem.Text = @"Лог <<";
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
            логToolStripMenuItem.Text = @"Лог <<";
            LogActive = true;
        }

        private void HideLog()
        {
            LogHide?.Invoke();
            логToolStripMenuItem.Text = @"Лог >>";
            LogActive = false;
        }

        private async void timer_Tick(object sender, EventArgs e)
        {
            _interval = _timer.Interval;
            if (!_farming)
            {
                await DoFarm();
            }
        }

        private async Task<bool> DoFarm()
        {
            _farming = true;
            LogBuffer = Messages.DoFarm_Start();
            LogChanged?.Invoke();

            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = 5;
            toolStripProgressBar1.Visible = true;
            toolStripStatusLabel1.Image = Resources.load;
            toolStripStatusLabel1.Text = @"Фарм";

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            if (Bot.GameMinerEnabled)
            {
                var profile = await Parse.GameMinerGetProfileAsync(Bot, true);
                if (profile.Echo)
                {
                    LogBuffer = profile;
                    LogChanged?.Invoke();
                }
                LoadProfilesInfo?.Invoke();

                if (profile.Success)
                {
                    var won = await Parse.GameMinerWonParseAsync(Bot);
                    if (won != null && won.Content != "\n")
                    {
                        LogBuffer = won;
                        LogChanged?.Invoke();
                        if (Settings.Default.ShowWonTip)
                        {
                            ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info,
                                "http://gameminer.net/giveaways/won");
                        }
                    }

                    var giveaways = await Parse.GameMinerLoadGiveawaysAsync(Bot, GmGiveaways, BlackList);
                    if (giveaways != null && giveaways.Content != "\n")
                    {
                        LogBuffer = giveaways;
                        LogChanged?.Invoke();
                    }

                    if (GmGiveaways?.Count > 0)
                    { 
                        if (Settings.Default.Sort)
                        {
                            if (Settings.Default.SortToMore)
                            {
                                GmGiveaways.Sort((a, b) => b.Price.CompareTo(a.Price));
                            }
                            else
                            {
                                GmGiveaways.Sort((a, b) => a.Price.CompareTo(b.Price));
                            }
                        }
                        foreach (var giveaway in GmGiveaways)
                        {
                            if (giveaway.Price <= Bot.GameMinerJoinCoalLimit && giveaway.Price <= Bot.GameMinerCoal)
                            {
                                if (Bot.GameMinerCoalReserv <= Bot.GameMinerCoal - giveaway.Price ||
                                    giveaway.Price == 0)
                                {
                                    var data = await Web.GameMinerJoinGiveawayAsync(Bot, giveaway);
                                    if (data != null && data.Content != "\n")
                                    {
                                        if (Settings.Default.FullLog)
                                        {
                                            LogBuffer = data;
                                            LogChanged?.Invoke();
                                        }
                                        else
                                        {
                                            if (data.Color != Color.Yellow && data.Color != Color.Red)
                                            {
                                                LogBuffer = data;
                                                LogChanged?.Invoke();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    BlockTabpage(tabPageGM, false);
                    btnGMLogin.Enabled = true;
                    btnGMLogin.Visible = true;
                    linkLabel1.Enabled = true;
                    lblGMStatus.Text = @"Статус: " + strings.LoginFaild;
                }
            }
            toolStripProgressBar1.Value++;

            if (Bot.SteamGiftsEnabled)
            {
                var profile = await Parse.SteamGiftsGetProfileAsync(Bot, true);
                if (profile.Echo)
                {
                    LogBuffer = profile;
                    LogChanged?.Invoke();
                }
                LoadProfilesInfo?.Invoke();

                if (profile.Success)
                {
                    var won = await Parse.SteamGiftsWonParseAsync(Bot);
                    if (won != null && won.Content != "\n")
                    {
                        LogBuffer = won;
                        LogChanged?.Invoke();
                        if (Settings.Default.ShowWonTip)
                        {
                            ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info,
                                "http://www.steamgifts.com/giveaways/won");
                        }
                    }

                    if (Bot.SteamGiftsPoint > 0)
                    {
                        var giveaways = await Parse.SteamGiftsLoadGiveawaysAsync(Bot, SgGiveaways, BlackList);
                        if (giveaways != null && giveaways.Content != "\n")
                        {
                            LogBuffer = giveaways;
                            LogChanged?.Invoke();
                        }

                        if (SgGiveaways?.Count > 0)
                        {
                            if (Settings.Default.Sort)
                            {
                                if (Settings.Default.SortToMore)
                                {
                                    SgGiveaways.Sort((a, b) => b.Price.CompareTo(a.Price));
                                }
                                else
                                {
                                    SgGiveaways.Sort((a, b) => a.Price.CompareTo(b.Price));
                                }
                            }

                            if (Bot.SteamGiftsSortToLessLevel)
                            {
                                SgGiveaways.Sort((a, b) => b.Level.CompareTo(a.Level));
                            }

                            foreach (var giveaway in SgGiveaways)
                            {
                                if (giveaway.Price <= Bot.SteamGiftsPoint &&
                                    Bot.SteamGiftsPointsReserv <= Bot.SteamGiftsPoint - giveaway.Price)
                                {
                                    var data = await Web.SteamGiftsJoinGiveawayAsync(Bot, giveaway);
                                    if (data != null && data.Content != "\n")
                                    {
                                        if (Settings.Default.FullLog)
                                        {
                                            LogBuffer = data;
                                            LogChanged?.Invoke();
                                        }
                                        else
                                        {
                                            if (data.Color != Color.Yellow && data.Color != Color.Red)
                                            {
                                                LogBuffer = data;
                                                LogChanged?.Invoke();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    BlockTabpage(tabPageSG, false);
                    btnSGLogin.Enabled = true;
                    btnSGLogin.Visible = true;
                    linkLabel2.Enabled = true;
                    lblSGStatus.Text = @"Статус: " + strings.LoginFaild;
                }
            }
            toolStripProgressBar1.Value++;

            if (Bot.SteamCompanionEnabled)
            {
                var profile = await Parse.SteamCompanionGetProfileAsync(Bot, true);
                if (profile.Echo)
                {
                    LogBuffer = profile;
                    LogChanged?.Invoke();
                }
                LoadProfilesInfo?.Invoke();
                if (profile.Success)
                {
                    var won = await Parse.SteamCompanionWonParseAsync(Bot);
                    if (won != null)
                    {
                        LogBuffer = won;
                        LogChanged?.Invoke();
                        if (Settings.Default.ShowWonTip)
                        {
                            ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info,
                                "https://steamcompanion.com/gifts/won");
                        }
                    }

                    if (Bot.SteamCompanionPoint > 0)
                    {
                        var giveaways = await Parse.SteamCompanionLoadGiveawaysAsync(Bot, ScGiveaways, BlackList);
                        if (giveaways != null && giveaways.Content != "\n")
                        {
                            LogBuffer = giveaways;
                            LogChanged?.Invoke();
                        }

                        if (ScGiveaways?.Count > 0)
                        {
                            if (Settings.Default.Sort)
                            {
                                if (Settings.Default.SortToMore)
                                {
                                    ScGiveaways.Sort((a, b) => b.Price.CompareTo(a.Price));
                                }
                                else
                                {
                                    ScGiveaways.Sort((a, b) => a.Price.CompareTo(b.Price));
                                }
                            }
                            foreach (var giveaway in ScGiveaways)
                            {
                                if (giveaway.Price <= Bot.SteamCompanionPoint &&
                                    Bot.SteamCompanionPointsReserv <= Bot.SteamCompanionPoint - giveaway.Price)
                                {
                                    var data = await Web.SteamCompanionJoinGiveawayAsync(Bot, giveaway);
                                    if (data != null && data.Content != "\n")
                                    {
                                        if (Settings.Default.FullLog)
                                        {
                                            LogBuffer = data;
                                            LogChanged?.Invoke();
                                        }
                                        else
                                        {
                                            if (data.Color != Color.Yellow && data.Color != Color.Red)
                                            {
                                                LogBuffer = data;
                                                LogChanged?.Invoke();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    BlockTabpage(tabPageSC, false);
                    btnSCLogin.Enabled = true;
                    btnSCLogin.Visible = true;
                    linkLabel3.Enabled = true;
                    lblSCStatus.Text = @"Статус: " + strings.LoginFaild;
                }
            }

            if (Bot.SteamPortalEnabled)
            {
                var profile = await Parse.SteamPortalGetProfileAsync(Bot, true);
                if (profile.Echo)
                {
                    LogBuffer = profile;
                    LogChanged?.Invoke();
                }
                LoadProfilesInfo?.Invoke();
                if (profile.Success)
                {
                    var won = await Parse.SteamPortalWonParsAsync(Bot);
                    if (won != null)
                    {
                        LogBuffer = won;
                        LogChanged?.Invoke();
                        if (Settings.Default.ShowWonTip)
                        {
                            ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info,
                                "http://steamportal.net/profile/logs");
                        }
                    }

                    if (Bot.SteamPortalPoints > 0)
                    {
                        var giveaways = await Parse.SteamPortalLoadGiveawaysAsync(Bot, SpGiveaways, BlackList);
                        if (giveaways != null && giveaways.Content != "\n")
                        {
                            LogBuffer = giveaways;
                            LogChanged?.Invoke();
                        }

                        if (SpGiveaways?.Count > 0)
                        {
                            if (Settings.Default.Sort)
                            {
                                if (Settings.Default.SortToMore)
                                {
                                    SpGiveaways.Sort((a, b) => b.Price.CompareTo(a.Price));
                                }
                                else
                                {
                                    SpGiveaways.Sort((a, b) => a.Price.CompareTo(b.Price));
                                }
                            }
                            foreach (var giveaway in SpGiveaways)
                            {
                                if (giveaway.Price <= Bot.SteamPortalPoints &&
                                    Bot.SteamPortalPointsReserv <= Bot.SteamPortalPoints - giveaway.Price)
                                {
                                    var data = await Web.SteamPortalJoinGiveawayAsync(Bot, giveaway);
                                    if (data != null && data.Content != "\n")
                                    {
                                        if (Settings.Default.FullLog)
                                        {
                                            LogBuffer = data;
                                            LogChanged?.Invoke();
                                        }
                                        else
                                        {
                                            if (data.Color != Color.Yellow && data.Color != Color.Red)
                                            {
                                                LogBuffer = data;
                                                LogChanged?.Invoke();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    BlockTabpage(tabPageSP, false);
                    btnSPLogin.Enabled = true;
                    btnSPLogin.Visible = true;
                    linkLabel4.Enabled = true;
                    lblSPStatus.Text = @"Статус: " + strings.LoginFaild;
                }
            }

            if (Bot.SteamTradeEnabled)
            {
                var profile = await Parse.SteamTradeGetProfileAsync(Bot, true);
                if (profile.Echo)
                {
                    LogBuffer = profile;
                    LogChanged?.Invoke();
                }
                LoadProfilesInfo?.Invoke();
                if (profile.Success)
                {
                    //var won = await Parse.SteamPortalWonParsAsync(Bot);
                    //if (won != null)
                    //{
                    //    LogBuffer = won;
                    //    LogChanged?.Invoke();
                    //}

                    var giveaways = await Parse.SteamTradeLoadGiveawaysAsync(Bot, StGiveaways, BlackList);
                    if (giveaways != null && giveaways.Content != "\n")
                    {
                        LogBuffer = giveaways;
                        LogChanged?.Invoke();
                    }

                    if (StGiveaways?.Count > 0)
                    {
                        foreach (var giveaway in StGiveaways)
                        {
                            var data = await Web.SteamTradeJoinGiveawayAsync(Bot, giveaway);
                            if (data != null && data.Content != "\n")
                            {
                                if (Settings.Default.FullLog)
                                {
                                    LogBuffer = data;
                                    LogChanged?.Invoke();
                                }
                                else
                                {
                                    if (data.Color != Color.Yellow && data.Color != Color.Red)
                                    {
                                        LogBuffer = data;
                                        LogChanged?.Invoke();
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    BlockTabpage(tabPageSteam, false);
                    btnSteamLogin.Enabled = true;
                    btnSteamLogin.Visible = true;
                    linkLabel6.Enabled = true;
                    lblSteamStatus.Text = @"Статус: " + strings.LoginFaild;
                }
            }

            if (Bot.PlayBlinkEnabled)
            {
                var profile = await Parse.PlayBlinkGetProfileAsync(Bot, true);
                if (profile.Echo)
                {
                    LogBuffer = profile;
                    LogChanged?.Invoke();
                }
                LoadProfilesInfo?.Invoke();

                if (profile.Success)
                {
                    //var won = await Parse.PlayBlinkWonParseAsync(Bot);
                    //if (won != null && won.Content != "\n")
                    //{
                    //    LogBuffer = won;
                    //    LogChanged?.Invoke();
                    //    if (Settings.Default.ShowWonTip)
                    //    {
                    //        ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info,
                    //            "http://playblink.com/");
                    //    }
                    //}

                    if(Bot.PlayBlinkPoints > 0)
                    { 
                    var giveaways = await Parse.PlayBlinkLoadGiveawaysAsync(Bot, PbGiveaways, BlackList);
                    if (giveaways != null && giveaways.Content != "\n")
                    {
                        LogBuffer = giveaways;
                        LogChanged?.Invoke();
                    }

                        if (PbGiveaways?.Count > 0)
                        {
                            if (Settings.Default.Sort)
                            {
                                if (Settings.Default.SortToMore)
                                {
                                    PbGiveaways.Sort((a, b) => b.Price.CompareTo(a.Price));
                                }
                                else
                                {
                                    PbGiveaways.Sort((a, b) => a.Price.CompareTo(b.Price));
                                }
                            }
                            foreach (var giveaway in PbGiveaways)
                            {
                                if (giveaway.Price <= Bot.PlayBlinkMaxJoinValue && giveaway.Price <= Bot.PlayBlinkPoints)
                                {
                                    if (Bot.PlayBlinkPointReserv <= Bot.PlayBlinkPoints - giveaway.Price ||
                                        giveaway.Price == 0)
                                    {
                                        var data = await Web.PlayBlinkJoinGiveawayAsync(Bot, giveaway);
                                        if (data != null && data.Content != "\n")
                                        {
                                            if (Settings.Default.FullLog)
                                            {
                                                LogBuffer = data;
                                                LogChanged?.Invoke();
                                            }
                                            else
                                            {
                                                if (data.Color != Color.Yellow && data.Color != Color.Red)
                                                {
                                                    LogBuffer = data;
                                                    LogChanged?.Invoke();
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
                        }
                    }
                }
                else
                {
                    BlockTabpage(tabPagePB, false);
                    btnPBLogin.Enabled = true;
                    btnPBLogin.Visible = true;
                    linkLabel7.Enabled = true;
                    lblPBStatus.Text = @"Статус: " + strings.LoginFaild;
                }
            }
            toolStripProgressBar1.Value++;

            stopWatch.Stop();
            var ts = stopWatch.Elapsed;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds/10:00}";
            LogBuffer = Messages.DoFarm_Finish(elapsedTime);
            LogChanged?.Invoke();
            LoadProfilesInfo?.Invoke();

            toolStripProgressBar1.Visible = false;
            toolStripStatusLabel1.Image = null;
            toolStripStatusLabel1.Text = @"Завершено";
            _farming = false;
            btnStart.Enabled = true;

            Settings.Default.JoinsLoops += 1;
            Settings.Default.JoinsLoopsTotal += 1;
            Settings.Default.Save();

            return true;
        }

        private void ShowProfileInfo()
        {
            lblGMCoal.Text = @"Угля: " + Bot.GameMinerCoal;
            lblGMLevel.Text = @"Уровень: " + Bot.GameMinerLevel;

            lblSGPoints.Text = @"Points: " + Bot.SteamGiftsPoint;
            lblSGLevel.Text = @"Уровень: " + Bot.SteamGiftsLevel;

            lblSCPoints.Text = @"Points: " + Bot.SteamCompanionPoint;
            lblSCLevel.Text = @"Уровень: -";

            lblSPPoints.Text = @"Points: " + Bot.SteamPortalPoints;
            lblSPLevel.Text = @"Уровень: -";

            lblSTPoints.Text = @"Points: -";
            lblSTLevel.Text = @"Уровень: -";

            lblPBPoints.Text = @"Points: " + Bot.PlayBlinkPoints;
            lblPBLevel.Text = @"Уровень: " + Bot.PlayBlinkLevel;
        }

        private async Task<bool> LoginCheck()
        {
            toolStripProgressBar1.Value = 0;
            toolStripProgressBar1.Maximum = 7;
            toolStripProgressBar1.Visible = true;
            toolStripStatusLabel1.Image = Resources.load;
            toolStripStatusLabel1.Text = @"Авторизация";
            var login = false;

            if (Bot.GameMinerEnabled)
            {
                var gm = await Parse.GameMinerGetProfileAsync(Bot, Bot.GameMinerEnabled);
                if (gm.Echo)
                {
                    LogBuffer = gm;
                    LogChanged?.Invoke();
                }
                LoadProfilesInfo?.Invoke();

                if (gm.Success)
                {
                    var won = await Parse.GameMinerWonParseAsync(Bot);
                    if (won != null && won.Content != "\n")
                    {
                        LogBuffer = won;
                        LogChanged?.Invoke();
                        if (Settings.Default.ShowWonTip)
                        {
                            ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info,
                                "http://gameminer.net/giveaways/won");
                        }
                    }

                    LoadProfilesInfo?.Invoke();

                    login = true;
                    btnGMLogin.Enabled = false;
                    btnGMLogin.Visible = false;
                    lblGMStatus.Text = @"Статус: " + strings.LoginSuccess;
                    LoadProfilesInfo?.Invoke();
                    pbGMReload.Visible = true;
                    btnGMExit.Visible = true;
                }
                else
                {
                    BlockTabpage(tabPageGM, false);
                    btnGMLogin.Enabled = true;
                    btnGMLogin.Visible = true;
                    linkLabel1.Enabled = true;
                    lblGMStatus.Text = @"Статус: " + strings.LoginFaild;
                }
            }
            else
            {
                BlockTabpage(tabPageGM, false);
                btnGMLogin.Enabled = true;
                btnGMLogin.Visible = true;
                linkLabel1.Enabled = true;
            }
            toolStripProgressBar1.Value++;

            if (Bot.SteamGiftsEnabled)
            {
                var sg = await Parse.SteamGiftsGetProfileAsync(Bot, Bot.SteamGiftsEnabled);
                if (sg.Echo)
                {
                    LogBuffer = sg;
                    LogChanged?.Invoke();
                }
                LoadProfilesInfo?.Invoke();

                if (sg.Success)
                {
                    var won = await Parse.SteamGiftsWonParseAsync(Bot);
                    if (won != null && won.Content != "\n")
                    {
                        LogBuffer = won;
                        LogChanged?.Invoke();
                        if (Settings.Default.ShowWonTip)
                        {
                            ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info,
                                "http://www.steamgifts.com/giveaways/won");
                        }
                    }
                    LoadProfilesInfo?.Invoke();

                    login = true;
                    btnSGLogin.Enabled = false;
                    btnSGLogin.Visible = false;
                    lblSGStatus.Text = @"Статус: " + strings.LoginSuccess;
                    pbSGReload.Visible = true;
                    btnSGExit.Visible = true;
                }
                else
                {
                    BlockTabpage(tabPageSG, false);
                    btnSGLogin.Enabled = true;
                    btnSGLogin.Visible = true;
                    linkLabel2.Enabled = true;
                    lblSGStatus.Text = @"Статус: " + strings.LoginFaild;
                }
            }
            else
            {
                BlockTabpage(tabPageSG, false);
                btnSGLogin.Enabled = true;
                btnSGLogin.Visible = true;
                linkLabel2.Enabled = true;
            }
            toolStripProgressBar1.Value++;

            if (Bot.SteamCompanionEnabled)
            {
                var sc = await Parse.SteamCompanionGetProfileAsync(Bot, Bot.SteamCompanionEnabled);
                if (sc.Echo)
                {
                    LogBuffer = sc;
                    LogChanged?.Invoke();
                }
                LoadProfilesInfo?.Invoke();

                if (sc.Success)
                {
                    var won = await Parse.SteamCompanionWonParseAsync(Bot);
                    if (won != null && won.Content != "\n")
                    {
                        LogBuffer = won;
                        LogChanged?.Invoke();
                        if (Settings.Default.ShowWonTip)
                        {
                            ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info,
                                "https://steamcompanion.com/gifts/won");
                        }
                    }
                    LoadProfilesInfo?.Invoke();

                    login = true;
                    btnSCLogin.Enabled = false;
                    btnSCLogin.Visible = false;
                    lblSCStatus.Text = @"Статус: " + strings.LoginSuccess;
                    pbSCReload.Visible = true;
                    btnSCExit.Visible = true;
                }
                else
                {
                    BlockTabpage(tabPageSC, false);
                    btnSCLogin.Enabled = true;
                    btnSCLogin.Visible = true;
                    linkLabel3.Enabled = true;
                    lblSCStatus.Text = @"Статус: " + strings.LoginFaild;
                }
            }
            else
            {
                BlockTabpage(tabPageSC, false);
                btnSCLogin.Enabled = true;
                btnSCLogin.Visible = true;
                linkLabel3.Enabled = true;
            }
            toolStripProgressBar1.Value++;

            if (Bot.SteamPortalEnabled)
            {
                var sp = await Parse.SteamPortalGetProfileAsync(Bot, Bot.SteamPortalEnabled);
                if (sp.Echo)
                {
                    LogBuffer = sp;
                    LogChanged?.Invoke();
                }
                LoadProfilesInfo?.Invoke();

                if (sp.Success)
                {
                    var won = await Parse.SteamPortalWonParsAsync(Bot);
                    if (won != null)
                    {
                        LogBuffer = won;
                        LogChanged?.Invoke();
                        if (Settings.Default.ShowWonTip)
                        {
                            ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info,
                                "http://steamportal.net/profile/logs");
                        }
                    }

                    login = true;
                    btnSPLogin.Enabled = false;
                    btnSPLogin.Visible = false;
                    lblSPStatus.Text = @"Статус: " + strings.LoginSuccess;
                    pbSPReload.Visible = true;
                    btnSPExit.Visible = true;
                }
                else
                {
                    BlockTabpage(tabPageSP, false);
                    btnSPLogin.Enabled = true;
                    btnSPLogin.Visible = true;
                    linkLabel4.Enabled = true;
                    lblSPStatus.Text = @"Статус: " + strings.LoginFaild;
                }
            }
            else
            {
                BlockTabpage(tabPageSP, false);
                btnSPLogin.Enabled = true;
                btnSPLogin.Visible = true;
                linkLabel4.Enabled = true;
            }
            toolStripProgressBar1.Value++;

            if (Bot.SteamTradeEnabled)
            {
                var st = await Parse.SteamTradeGetProfileAsync(Bot, Bot.SteamTradeEnabled);
                if (st.Echo)
                {
                    LogBuffer = st;
                    LogChanged?.Invoke();
                }
                LoadProfilesInfo?.Invoke();

                if (st.Success)
                {
                    login = true;
                    btnSTLogin.Enabled = false;
                    btnSTLogin.Visible = false;
                    lblSTStatus.Text = @"Статус: " + strings.LoginSuccess;
                    pbSTreload.Visible = true;
                    btnSTExit.Visible = true;
                }
                else
                {
                    BlockTabpage(tabPageST, false);
                    btnSTLogin.Enabled = true;
                    btnSTLogin.Visible = true;
                    linkLabel5.Enabled = true;
                    lblSTStatus.Text = @"Статус: " + strings.LoginFaild;
                }
            }
            else
            {
                BlockTabpage(tabPageST, false);
                btnSTLogin.Enabled = true;
                btnSTLogin.Visible = true;
                linkLabel5.Enabled = true;
            }
            toolStripProgressBar1.Value++;

            if (Bot.PlayBlinkEnabled)
            {
                var pb = await Parse.PlayBlinkGetProfileAsync(Bot, Bot.PlayBlinkEnabled);
                if (pb.Echo)
                {
                    LogBuffer = pb;
                    LogChanged?.Invoke();
                }
                LoadProfilesInfo?.Invoke();

                if (pb.Success)
                {
                    //var won = await Parse.PlayBlinkWonParseAsync(Bot);
                    //if (won != null && won.Content != "\n")
                    //{
                    //    LogBuffer = won;
                    //    LogChanged?.Invoke();
                    //    if (Settings.Default.ShowWonTip)
                    //    {
                    //        ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info,
                    //            "http://playblink.com/");
                    //    }
                    //}

                    LoadProfilesInfo?.Invoke();

                    login = true;
                    btnPBLogin.Enabled = false;
                    btnPBLogin.Visible = false;
                    lblPBStatus.Text = @"Статус: " + strings.LoginSuccess;
                    LoadProfilesInfo?.Invoke();
                    pbPBRefresh.Visible = true;
                    btnPBExit.Visible = true;
                }
                else
                {
                    BlockTabpage(tabPagePB, false);
                    btnPBLogin.Enabled = true;
                    btnPBLogin.Visible = true;
                    linkLabel7.Enabled = true;
                    lblPBStatus.Text = @"Статус: " + strings.LoginFaild;
                }
            }
            else
            {
                BlockTabpage(tabPagePB, false);
                btnPBLogin.Enabled = true;
                btnPBLogin.Visible = true;
                linkLabel7.Enabled = true;
            }
            toolStripProgressBar1.Value++;

            if (Bot.SteamEnabled)
            {
                var steam = await Parse.SteamGetProfileAsync(Bot, Bot.SteamEnabled);
                if (steam.Echo)
                {
                    LogBuffer = steam;
                    LogChanged?.Invoke();
                }
                LoadProfilesInfo?.Invoke();

                if (steam.Success)
                {
                    btnSteamLogin.Enabled = false;
                    btnSteamLogin.Visible = false;
                    lblSteamStatus.Text = @"Статус: " + strings.LoginSuccess;
                    btnSteamExit.Visible = true;
                }
                else
                {
                    BlockTabpage(tabPageSteam, false);
                    btnSteamLogin.Enabled = true;
                    btnSteamLogin.Visible = true;
                    linkLabel6.Enabled = true;
                    lblSteamStatus.Text = @"Статус: " + strings.LoginFaild;
                }
            }
            else
            {
                BlockTabpage(tabPageSteam, false);
                btnSteamLogin.Enabled = true;
                btnSteamLogin.Visible = true;
                linkLabel6.Enabled = true;
            }
            toolStripProgressBar1.Value++;

            toolStripProgressBar1.Visible = false;
            toolStripStatusLabel1.Image = null;
            toolStripStatusLabel1.Text = @"Завершено";
            return login;
        }

        private void BlockTabpage(TabPage tabPage, bool state)
        {
            foreach (Control control in tabPage.Controls)
            {
                if (control == linkLabel1 || control == linkLabel2 || control == linkLabel3 || control == linkLabel4 ||
                    control == linkLabel5)
                {
                    control.Enabled = true;
                }
                else
                {
                    control.Enabled = state;
                }
            }
        }

        private async void btnSTLogin_Click(object sender, EventArgs e)
        {
            btnSTLogin.Enabled = false;
            var first = Web.Get("http://steamtrade.info", "", new List<Parameter>(), new CookieContainer(),
                new List<HttpHeader>(), Bot.UserAgent);
            var getLoginHref = Web.SteamTradeDoAuth("http://steamtrade.info/", "reg.php?login",
                Generate.LoginData_SteamTrade(), first.Cookies, new List<HttpHeader>(), Bot.UserAgent, "");
            var location = Tools.GetLocationInresponse(getLoginHref.RestResponse);
            var cookie = Tools.GetSessCookieInresponse(getLoginHref.Cookies, "steamtrade.info", "PHPSESSID");

            BrowserStart(location, "http://steamtrade.info/", "SteamTrade - Login", cookie,
                Bot.SteamEnabled ? Generate.Cookies_Steam(Bot) : new CookieContainer());
            Tools.SaveProfile(Bot, "");

            toolStripStatusLabel1.Image = Resources.load;
            toolStripStatusLabel1.Text = @"Авторизация";
            var login = await CheckLoginSt();
            if (login)
            {                                
                //var won = await Parse.GameMinerSteamTradeWonParseAsync(Bot);
                //if (won != null && won.Content != "\n")
                //{
                //    LogBuffer = won;
                //    LogChanged?.Invoke();
                //}
                BlockTabpage(tabPageST, true);
                btnSTLogin.Enabled = false;
                btnSTLogin.Visible = false;
                lblSTStatus.Text = @"Статус: " + strings.LoginSuccess;
                LoadProfilesInfo?.Invoke();
                btnStart.Enabled = true;
                pbSTreload.Visible = true;
                btnSTExit.Visible = true;
                btnSTExit.Enabled = true;
                cbSTEnable.Checked = true;
                Bot.SteamTradeEnabled = true;

                LogBuffer = Tools.ConstructLog(Messages.GetDateTime() + "{SteamTrade} " + strings.LoginSuccess,
                    Color.Green, true, true);
                LogChanged?.Invoke();

                var st = await Parse.SteamTradeGetProfileAsync(Bot, true);
                LogBuffer = st;
                LogChanged?.Invoke();
                LoadProfilesInfo?.Invoke();

            }
            else
            {
                lblSTStatus.Text = @"Статус: " + strings.LoginFaild;
                BlockTabpage(tabPageST, false);
                btnSTLogin.Enabled = true;
                btnSTLogin.Visible = true;
            }
            toolStripStatusLabel1.Image = null;
            toolStripStatusLabel1.Text = @"Завершено";
        }

        public static void BrowserStart(string startPage, string endPage, string title, string phpSessId,
            CookieContainer cookies)
        {
            Form form = new Browser(Bot, startPage, endPage, title, phpSessId, cookies);
            form.Height = Screen.PrimaryScreen.Bounds.Height/2;
            form.Width = Screen.PrimaryScreen.Bounds.Width/2;
            form.Name = "Browser";
            form.ShowDialog();
        }

        private async void btnSPLogin_Click(object sender, EventArgs e)
        {
            btnSPLogin.Enabled = false;
            BrowserStart("http://steamportal.net/page/steam", "http://steamportal.net/", "SteamPortal - Login", "",
                Bot.SteamEnabled ? Generate.Cookies_Steam(Bot) : new CookieContainer());
            Tools.SaveProfile(Bot, "");

            toolStripStatusLabel1.Image = Resources.load;
            toolStripStatusLabel1.Text = @"Авторизация";
            var login = await CheckLoginSp();
            if (login)
            {
                var won = await Parse.SteamPortalWonParsAsync(Bot);
                if (won != null && won.Content != "\n")
                {
                    LogBuffer = won;
                    LogChanged?.Invoke();
                }

                BlockTabpage(tabPageSP, true);
                btnSPLogin.Enabled = false;
                btnSPLogin.Visible = false;
                lblSPStatus.Text = @"Статус: " + strings.LoginSuccess;
                LoadProfilesInfo?.Invoke();
                btnStart.Enabled = true;
                pbSPReload.Visible = true;
                btnSPExit.Visible = true;
                btnSPExit.Enabled = true;
                cbSPEnable.Checked = true;
                Bot.SteamPortalEnabled = true;

                LogBuffer = Tools.ConstructLog(Messages.GetDateTime() + "{SteamPortal} " + strings.LoginSuccess,
                    Color.Green, true, true);
                LogChanged?.Invoke();

                var sp = await Parse.SteamPortalGetProfileAsync(Bot, true);
                LogBuffer = sp;
                LogChanged?.Invoke();
                LoadProfilesInfo?.Invoke();
            }
            else
            {
                lblSPStatus.Text = @"Статус: " + strings.LoginFaild;
                BlockTabpage(tabPageSP, false);
                btnSPLogin.Enabled = true;
                btnSPLogin.Visible = true;
            }
            toolStripStatusLabel1.Image = null;
            toolStripStatusLabel1.Text = @"Завершено";
        }

        private async void btnSCLogin_Click(object sender, EventArgs e)
        {
            btnSCLogin.Enabled = false;
            BrowserStart("https://steamcompanion.com/login", "https://steamcompanion.com/", "SteamCompanion - Login", "",
                Bot.SteamEnabled ? Generate.Cookies_Steam(Bot) : new CookieContainer());
            Tools.SaveProfile(Bot, "");

            toolStripStatusLabel1.Image = Resources.load;
            toolStripStatusLabel1.Text = @"Авторизация";
            var login = await CheckLoginSc();
            if (login)
            {
                var won = await Parse.SteamCompanionWonParseAsync(Bot);
                if (won != null && won.Content != "\n")
                {
                    LogBuffer = won;
                    LogChanged?.Invoke();
                }
                BlockTabpage(tabPageSC, true);
                btnSCLogin.Enabled = false;
                btnSCLogin.Visible = false;
                lblSCStatus.Text = @"Статус: " + strings.LoginSuccess;
                LoadProfilesInfo?.Invoke();
                btnStart.Enabled = true;
                pbSCReload.Visible = true;
                btnSCExit.Visible = true;
                btnSCExit.Enabled = true;
                cbSCEnable.Checked = true;
                Bot.SteamCompanionEnabled = true;

                LogBuffer = Tools.ConstructLog(Messages.GetDateTime() + "{SteamCompanion} " + strings.LoginSuccess,
                    Color.Green, true, true);
                LogChanged?.Invoke();

                var sc = await Parse.SteamCompanionGetProfileAsync(Bot, true);
                LogBuffer = sc;
                LogChanged?.Invoke();
                LoadProfilesInfo?.Invoke();
            }
            else
            {
                lblSCStatus.Text = @"Статус: " + strings.LoginFaild;
                BlockTabpage(tabPageSC, false);
                btnSCLogin.Enabled = true;
                btnSCLogin.Visible = true;
            }
            toolStripStatusLabel1.Image = null;
            toolStripStatusLabel1.Text = @"Завершено";
        }

        private async void btnSGLogin_Click(object sender, EventArgs e)
        {
            btnSGLogin.Enabled = false;
            BrowserStart("http://www.steamgifts.com/?login", "http://www.steamgifts.com/", "SteamGifts - Login", "",
                Bot.SteamEnabled ? Generate.Cookies_Steam(Bot) : new CookieContainer());
            Tools.SaveProfile(Bot, "");

            toolStripStatusLabel1.Image = Resources.load;
            toolStripStatusLabel1.Text = @"Авторизация";
            var login = await CheckLoginSg();
            if (login)
            {
                var won = await Parse.SteamGiftsWonParseAsync(Bot);
                if (won != null && won.Content != "\n")
                {
                    LogBuffer = won;
                    LogChanged?.Invoke();
                }
                BlockTabpage(tabPageSG, true);
                btnSGLogin.Enabled = false;
                btnSGLogin.Visible = false;
                lblSGStatus.Text = @"Статус: " + strings.LoginSuccess;
                LoadProfilesInfo?.Invoke();
                btnStart.Enabled = true;
                pbSGReload.Visible = true;
                btnSGExit.Visible = true;
                btnSGExit.Enabled = true;
                cbSGEnable.Checked = true;
                Bot.SteamGiftsEnabled = true;

                LogBuffer = Tools.ConstructLog(Messages.GetDateTime() + "{SteamGifts} " + strings.LoginSuccess,
                    Color.Green, true, true);
                LogChanged?.Invoke();

                var sg = await Parse.SteamGiftsGetProfileAsync(Bot, true);
                LogBuffer = sg;
                LogChanged?.Invoke();
                LoadProfilesInfo?.Invoke();
            }
            else
            {
                lblSGStatus.Text = @"Статус: " + strings.LoginFaild;
                BlockTabpage(tabPageSG, false);
                btnSGLogin.Enabled = true;
                btnSGLogin.Visible = true;
            }
            toolStripStatusLabel1.Image = null;
            toolStripStatusLabel1.Text = @"Завершено";
        }

        private async void btnGMLogin_Click(object sender, EventArgs e)
        {
            btnGMLogin.Enabled = false;
            BrowserStart("http://gameminer.net/login/steam?backurl=http%3A%2F%2Fgameminer.net%2F%3Flang%3D" + Settings.Default.Lang + @"&agree=True",
                "http://gameminer.net/?lang=" + Settings.Default.Lang, "GameMiner - Login", "",
                Bot.SteamEnabled ? Generate.Cookies_Steam(Bot) : new CookieContainer());
            if (string.IsNullOrEmpty(Bot.UserAgent))
            {
                Bot.UserAgent = Tools.UserAgent();
            }
            Tools.SaveProfile(Bot, "");

            toolStripStatusLabel1.Image = Resources.load;
            toolStripStatusLabel1.Text = @"Авторизация";
            var login = await CheckLoginGm();
            if (login)
            {
                var won = await Parse.GameMinerWonParseAsync(Bot);
                if (won != null && won.Content != "\n")
                {
                    LogBuffer = won;
                    LogChanged?.Invoke();
                }
                BlockTabpage(tabPageGM, true);
                btnGMLogin.Enabled = false;
                btnGMLogin.Visible = false;
                lblGMStatus.Text = @"Статус: " + strings.LoginSuccess;
                LoadProfilesInfo?.Invoke();
                btnStart.Enabled = true;
                pbGMReload.Visible = true;
                btnGMExit.Visible = true;
                btnGMExit.Enabled = true;
                cbGMEnable.Checked = true;
                Bot.GameMinerEnabled = true;

                LogBuffer = Tools.ConstructLog(Messages.GetDateTime() + "{GameMiner} " + strings.LoginSuccess,
                    Color.Green, true, true);
                LogChanged?.Invoke();

                var gm = await Parse.GameMinerGetProfileAsync(Bot, Bot.GameMinerEnabled);
                LogBuffer = gm;
                LogChanged?.Invoke();
                LoadProfilesInfo?.Invoke();
            }
            else
            {
                lblGMStatus.Text = @"Статус: " + strings.LoginFaild;
                BlockTabpage(tabPageGM, false);
                btnGMLogin.Enabled = true;
                btnGMLogin.Visible = true;
            }
            toolStripStatusLabel1.Image = null;
            toolStripStatusLabel1.Text = @"Завершено";
        }

        private async Task<bool> CheckLoginGm()
        {
            var login = await Parse.GameMinerGetProfileAsync(Bot, false);
            if (login.Success)
            {
                return true;
            }
            else
            {
                LogBuffer = login;
                LogChanged?.Invoke();
            }
            return false;
        }

        private async Task<bool> CheckLoginSg()
        {
            var login = await Parse.SteamGiftsGetProfileAsync(Bot, false);
            if (login.Success)
            {
                return true;
            }
            else
            {
                LogBuffer = login;
                LogChanged?.Invoke();
            }
            return false;
        }

        private async Task<bool> CheckLoginSc()
        {
            var login = await Parse.SteamCompanionGetProfileAsync(Bot, false);
            if (login.Success)
            {
                return true;
            }
            return false;
        }

        private async Task<bool> CheckLoginSp()
        {
            var login = await Parse.SteamPortalGetProfileAsync(Bot, false);
            if (login.Success)
            {
                return true;
            }
            return false;
        }

        private async Task<bool> CheckLoginSt()
        {
            var login = await Parse.SteamTradeGetProfileAsync(Bot, false);
            if (login.Success)
            {
                return true;
            }
            return false;
        }

        private async Task<bool> CheckLoginSteam()
        {
            var login = await Parse.SteamGetProfileAsync(Bot, false);
            if (login.Success)
            {
                return true;
            }
            return false;
        }

        private async Task<bool> CheckLoginPb()
        {
            var login = await Parse.SteamGetProfileAsync(Bot, false);
            if (login.Success)
            {
                return true;
            }
            return false;
        }

        private async void pbGMReload_Click(object sender, EventArgs e)
        {
            btnGMExit.Enabled = false;
            pbGMReload.Image = Resources.load;
            toolStripStatusLabel1.Text = @"Обновление информации о GameMiner";
            toolStripStatusLabel1.Image = Resources.load;

            var profile = await Parse.GameMinerGetProfileAsync(Bot, false);
            if (profile.Echo)
            {
                LogBuffer = profile;
                LogChanged?.Invoke();
            }
            LoadProfilesInfo?.Invoke();

            if (profile.Success)
            {
                var won = await Parse.GameMinerWonParseAsync(Bot);
                if (won != null && won.Content != "\n")
                {
                    LogBuffer = won;
                    LogChanged?.Invoke();
                }

                var async = await Web.GameMinerSyncAccountAsync(Bot);
                if (async != null)
                {
                    LogBuffer = async;
                    LogChanged?.Invoke();
                }

                btnGMLogin.Enabled = false;
                btnGMLogin.Visible = false;
                lblGMStatus.Text = @"Статус: " + strings.LoginSuccess;
                LoadProfilesInfo?.Invoke();
                BlockTabpage(tabPageGM, true);
            }
            else
            {
                BlockTabpage(tabPageGM, false);
                btnGMLogin.Enabled = true;
                btnGMLogin.Visible = true;
                lblGMStatus.Text = @"Статус: " + strings.LoginFaild;
            }

            toolStripStatusLabel1.Image = null;
            pbGMReload.Image = Resources.refresh;
            toolStripStatusLabel1.Text = @"Завершено";
            btnGMExit.Enabled = true;
        }

        private async void pbSGReload_Click(object sender, EventArgs e)
        {
            btnGMExit.Enabled = false;
            pbSGReload.Image = Resources.load;
            toolStripStatusLabel1.Text = @"Обновление информации о SteamGifts";
            toolStripStatusLabel1.Image = Resources.load;

            var profile = await Parse.SteamGiftsGetProfileAsync(Bot, false);
            if (profile.Echo)
            {
                LogBuffer = profile;
                LogChanged?.Invoke();
            }
            LoadProfilesInfo?.Invoke();

            if (profile.Success)
            {
                var won = await Parse.SteamGiftsWonParseAsync(Bot);
                if (won != null)
                {
                    LogBuffer = won;
                    LogChanged?.Invoke();
                }

                var async = await Web.SteamGiftsSyncAccountAsync(Bot);
                if (async != null)
                {
                    LogBuffer = async;
                    LogChanged?.Invoke();
                }

                btnSGLogin.Enabled = false;
                btnSGLogin.Visible = false;
                lblSGStatus.Text = @"Статус: " + strings.LoginSuccess;
                BlockTabpage(tabPageSG, true);
            }
            else
            {
                BlockTabpage(tabPageSG, false);
                btnSGLogin.Enabled = true;
                btnSGLogin.Visible = true;
                lblSGStatus.Text = @"Статус: " + strings.LoginFaild;
            }

            toolStripStatusLabel1.Image = null;
            pbSGReload.Image = Resources.refresh;
            toolStripStatusLabel1.Text = @"Завершено";
            btnGMExit.Enabled = true;
        }

        private async void pbSCReload_Click(object sender, EventArgs e)
        {
            btnGMExit.Enabled = false;
            pbSCReload.Image = Resources.load;
            toolStripStatusLabel1.Text = @"Обновление информации о SteamCompanion";
            toolStripStatusLabel1.Image = Resources.load;

            var profile = await Parse.SteamCompanionGetProfileAsync(Bot, false);
            if (profile.Echo)
            {
                LogBuffer = profile;
                LogChanged?.Invoke();
            }
            LoadProfilesInfo?.Invoke();

            if (profile.Success)
            {
                var won = await Parse.SteamCompanionWonParseAsync(Bot);
                if (won != null)
                {
                    LogBuffer = won;
                    LogChanged?.Invoke();
                }
                btnSCLogin.Enabled = false;
                btnSCLogin.Visible = false;
                lblSCStatus.Text = @"Статус: " + strings.LoginSuccess;
                BlockTabpage(tabPageSC, true);

                var async = await Web.SteamCompanionSyncAccountAsync(Bot);
                if (async != null)
                {
                    LogBuffer = async;
                    LogChanged?.Invoke();
                }

            }
            else
            {
                BlockTabpage(tabPageSC, false);
                btnSCLogin.Enabled = true;
                btnSCLogin.Visible = true;
                lblSCStatus.Text = @"Статус: " + strings.LoginFaild;
            }

            toolStripStatusLabel1.Image = null;
            pbSCReload.Image = Resources.refresh;
            toolStripStatusLabel1.Text = @"Завершено";
            btnGMExit.Enabled = true;
        }

        private async void pbSPReload_Click(object sender, EventArgs e)
        {
            btnGMExit.Enabled = false;
            pbSPReload.Image = Resources.load;
            toolStripStatusLabel1.Text = @"Обновление информации о SteamPortal";
            toolStripStatusLabel1.Image = Resources.load;

            var profile = await Parse.SteamPortalGetProfileAsync(Bot, false);
            if (profile.Echo)
            {
                LogBuffer = profile;
                LogChanged?.Invoke();
            }
            LoadProfilesInfo?.Invoke();

            if (profile.Success)
            {
                btnSPLogin.Enabled = false;
                btnSPLogin.Visible = false;
                lblSPStatus.Text = @"Статус: " + strings.LoginSuccess;
                BlockTabpage(tabPageSP, true);
            }
            else
            {
                BlockTabpage(tabPageSP, false);
                btnSPLogin.Enabled = true;
                btnSPLogin.Visible = true;
                lblSPStatus.Text = @"Статус: " + strings.LoginFaild;
            }

            toolStripStatusLabel1.Image = null;
            pbSPReload.Image = Resources.refresh;
            toolStripStatusLabel1.Text = @"Завершено";
            btnGMExit.Enabled = true;
        }

        private async void pbSTreload_Click(object sender, EventArgs e)
        {
            btnGMExit.Enabled = false;
            pbSTreload.Image = Resources.load;
            toolStripStatusLabel1.Text = @"Обновление информации о SteamTrade";
            toolStripStatusLabel1.Image = Resources.load;

            var profile = await Parse.SteamTradeGetProfileAsync(Bot, false);
            if (profile.Echo)
            {
                LogBuffer = profile;
                LogChanged?.Invoke();
            }
            LoadProfilesInfo?.Invoke();

            if (profile.Success)
            {
                btnSTLogin.Enabled = false;
                btnSTLogin.Visible = false;
                lblSTStatus.Text = @"Статус: " + strings.LoginSuccess;
                BlockTabpage(tabPageST, true);
            }
            else
            {
                BlockTabpage(tabPageST, false);
                btnSTLogin.Enabled = true;
                btnSTLogin.Visible = true;
                lblSTStatus.Text = @"Статус: " + strings.LoginFaild;
            }

            toolStripStatusLabel1.Image = null;
            pbSTreload.Image = Resources.refresh;
            toolStripStatusLabel1.Text = @"Завершено";
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
            Process.Start("http://gameminer.net/");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.steamgifts.com/");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://steamcompanion.com/");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://steamportal.net/");
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://steamtrade.info/");
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
            if (Tools.SaveProfile(Bot, ""))
            {
                LogBuffer = Tools.ConstructLog(Messages.GetDateTime() + "Настройки сохранены в profile.xml", Color.White,
                    true, true);
                LogChanged?.Invoke();
            }

            Settings.Default.Save();
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
                if (Tools.SaveProfile(Bot, saveFileDialog1.FileName))
                {
                    LogBuffer = Tools.ConstructLog("Файл сохранен по пути " + saveFileDialog1.FileName, Color.Green,
                        true, true);
                    LogChanged?.Invoke();
                }
                else
                {
                    LogBuffer = Tools.ConstructLog("Файл НЕ сохранен", Color.Red, true, true);
                    LogChanged?.Invoke();
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
                Bot.SteamTradeEnabled = false;
            }
            else
            {
                BlockTabpage(tabPageST, true);
                Bot.SteamTradeEnabled = true;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (lblSGStatus.Enabled)
            {
                BlockTabpage(tabPageSG, false);
                cbSGEnable.Enabled = true;
                Bot.SteamGiftsEnabled = false;
            }
            else
            {
                BlockTabpage(tabPageSG, true);
                Bot.SteamGiftsEnabled = true;
            }
        }

        private void cbGMEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (lblGMStatus.Enabled)
            {
                BlockTabpage(tabPageGM, false);
                cbGMEnable.Enabled = true;
                Bot.GameMinerEnabled = false;
            }
            else
            {
                BlockTabpage(tabPageGM, true);
                Bot.GameMinerEnabled = true;
            }
        }

        private void cbSPEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (lblSPStatus.Enabled)
            {
                BlockTabpage(tabPageSP, false);
                cbSPEnable.Enabled = true;
                Bot.SteamPortalEnabled = false;
            }
            else
            {
                BlockTabpage(tabPageSP, true);
                Bot.SteamPortalEnabled = true;
            }
        }

        private void cbSCEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (lblSCStatus.Enabled)
            {
                BlockTabpage(tabPageSC, false);
                cbSCEnable.Enabled = true;
                Bot.SteamCompanionEnabled = false;
            }
            else
            {
                BlockTabpage(tabPageSC, true);
                Bot.SteamCompanionEnabled = true;
            }
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://steamcommunity.com/");
        }

        private async void btnSteamLogin_Click(object sender, EventArgs e)
        {
            BrowserStart("https://steamcommunity.com/login/home/?goto=0",
                "http://steamcommunity.com/id/", "Steam - Login", "", new CookieContainer());
            Tools.SaveProfile(Bot, "");

            toolStripStatusLabel1.Image = Resources.load;
            toolStripStatusLabel1.Text = @"Авторизация";
            var login = await CheckLoginSteam();
            if (login)
            {
                BlockTabpage(tabPageSteam, true);
                btnSteamLogin.Enabled = false;
                btnSteamLogin.Visible = false;

                lblSteamStatus.Text = @"Статус: " + strings.LoginSuccess;
                LoadProfilesInfo?.Invoke();

                LogBuffer = Tools.ConstructLog(Messages.GetDateTime() + "{Steam} " + strings.LoginSuccess, Color.Green,
                    true, true);
                LogChanged?.Invoke();

                var steam = await Parse.SteamGetProfileAsync(Bot, Bot.SteamEnabled);
                if (steam.Echo)
                {
                    LogBuffer = steam;
                    LogChanged?.Invoke();
                }
                LoadProfilesInfo?.Invoke();
            }
            else
            {
                lblSteamStatus.Text = @"Статус: " + strings.LoginFaild;
                BlockTabpage(tabPageSteam, false);
                btnSteamLogin.Enabled = true;
                btnSteamLogin.Visible = true;
            }
            toolStripStatusLabel1.Image = null;
            toolStripStatusLabel1.Text = @"Завершено";
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

        private void ShowBaloolTip(string content, int interval, ToolTipIcon icon, string url)
        {
            notifyIcon.BalloonTipIcon = icon;
            notifyIcon.BalloonTipText = content;
            notifyIcon.BalloonTipTitle = Application.ProductName;
            notifyIcon.Tag = url;
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

        private async Task<bool> CheckForUpdates()
        {
            try
            {
                string json = await Web.GetVersionInGitHubAsync(Settings.Default.GitHubRepoReleaseUrl);

                if(json != null)
                {
                    Classes.GitHubRelease release;
                    try
                    {
                        release = JsonConvert.DeserializeObject<Classes.GitHubRelease>(json);
                    }
                    catch (JsonReaderException)
                    {
                        return false;
                    }

                    if (release.tag_name != null && Tools.VersionCompare(Application.ProductVersion, release.tag_name))
                    {
                        LogBuffer =
                            Tools.ConstructLog(
                                $"Для скачивания доступна новая версия [{release.tag_name}] {Settings.Default.GitHubRepoUrl}",
                                Color.Green, true, true);
                        LogChanged?.Invoke();

                        DialogResult dr = MessageBox.Show($"Для скачивания доступна новая версия { release.tag_name}. Скачать?",
                            @"Обновление", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                        if (dr == DialogResult.Yes)
                        {
                            if (File.Exists("KryBot_Updater.exe"))
                            {
                                if (Tools.CheckMd5("KryBot_Updater.exe", "KryBot.Resources.KryBot_Updater.exe"))
                                {
                                    Process.Start("KryBot_Updater.exe", Application.ProductVersion);
                                    Application.Exit();
                                }
                                else
                                {
                                    Tools.CopyResource("KryBot.Resources.KryBot_Updater.exe", Application.StartupPath + "\\KryBot_Updater.exe");
                                    Process.Start("KryBot_Updater.exe", Application.ProductVersion);
                                    Application.Exit();
                                }
                            }
                            else
                            {
                                Tools.CopyResource("KryBot.Resources.KryBot_Updater.exe", Application.StartupPath + "\\KryBot_Updater.exe");
                                Process.Start("KryBot_Updater.exe", Application.ProductVersion);
                                Application.Exit();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + @"{" + ex.Source + @"} {" + ex.InnerException.Message + @"} {" + ex.StackTrace + @"}");
            }                                                                                                                 
            return true;                                                                                 
        }

        private void донатToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new FormDonate();
            form.ShowDialog();
        }

        private void btnSteamExit_Click(object sender, EventArgs e)
        {
            Bot.SteamSessid = "";
            Bot.SteamLogin = "";
            Bot.SteamEnabled = false;
            BlockTabpage(tabPageSteam, false);
            btnSteamLogin.Visible = true;
            btnSteamLogin.Enabled = true;
            btnSteamExit.Visible = false;
            Tools.SaveProfile(Bot, "");
        }

        private void btnSTExit_Click(object sender, EventArgs e)
        {
            Bot.SteamTradeDleUserId = "";
            Bot.SteamTradeDlePassword = "";
            Bot.SteamTradePassHash = "";
            Bot.SteamTradePhpSessId = "";
            Bot.SteamTradeEnabled = false;
            BlockTabpage(tabPageST, false);
            btnSTLogin.Visible = true;
            btnSTExit.Visible = false;
            btnSTLogin.Enabled = true;
            Tools.SaveProfile(Bot, "");
        }

        private void btnSPExit_Click(object sender, EventArgs e)
        {
            Bot.SteamPortalPhpSessId = "";
            Bot.SteamPortalEnabled = false;
            BlockTabpage(tabPageSP, false);
            btnSPLogin.Visible = true;
            btnSPLogin.Enabled = true;
            btnSPExit.Visible = false;
            Tools.SaveProfile(Bot, "");
        }

        private void btnSCExit_Click(object sender, EventArgs e)
        {
            Bot.SteamCompanionPhpSessId = "";
            Bot.SteamCompanionUserC = "";
            Bot.SteamCompanionUserId = "";
            Bot.SteamCompanionUserT = "";
            Bot.SteamCompanionEnabled = false;
            BlockTabpage(tabPageSC, false);
            btnSCLogin.Visible = true;
            btnSCLogin.Enabled = true;
            btnSCExit.Visible = false;
            Tools.SaveProfile(Bot, "");
        }

        private void btnSGExit_Click(object sender, EventArgs e)
        {
            Bot.SteamGiftsPhpSessId = "";
            Bot.SteamGiftsEnabled = false;
            BlockTabpage(tabPageSG, false);
            btnSGLogin.Visible = true;
            btnSGLogin.Enabled = true;
            btnSGExit.Visible = false;
            Tools.SaveProfile(Bot, "");
        }

        private void btnGMExit_Click(object sender, EventArgs e)
        {
            Bot.GameMinerToken = "";
            Bot.GameMinerxsrf = "";
            Bot.GameMinerEnabled = false;
            BlockTabpage(tabPageGM, false);
            btnGMLogin.Enabled = true;
            btnGMLogin.Visible = true;
            btnGMExit.Visible = false;
            Tools.SaveProfile(Bot, "");
        }

        private void вПапкуСБотомToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("explorer", Environment.CurrentDirectory);
        }

        private void черныйСписокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormBlackList form = new FormBlackList();
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
            BrowserStart("http://playblink.com/?do=login&act=signin",
                "http://playblink.com/", "PlayBlink - Login", "",
                Bot.SteamEnabled ? Generate.Cookies_Steam(Bot) : new CookieContainer());
            if (string.IsNullOrEmpty(Bot.UserAgent))
            {
                Bot.UserAgent = Tools.UserAgent();
            }
            Tools.SaveProfile(Bot, "");

            toolStripStatusLabel1.Image = Resources.load;
            toolStripStatusLabel1.Text = @"Авторизация";
            var login = await CheckLoginPb();
            if (login)
            {
                //var won = await Parse.PlayBlinkWonParseAsync(Bot);
                //if (won != null && won.Content != "\n")
                //{
                //    LogBuffer = won;
                //    LogChanged?.Invoke();
                //}
                BlockTabpage(tabPagePB, true);
                btnPBLogin.Enabled = false;
                btnPBLogin.Visible = false;
                lblPBStatus.Text = @"Статус: " + strings.LoginSuccess;
                LoadProfilesInfo?.Invoke();
                btnStart.Enabled = true;
                pbPBRefresh.Visible = true;
                btnPBExit.Visible = true;
                btnPBExit.Enabled = true;
                cbPBEnabled.Checked = true;
                Bot.PlayBlinkEnabled = true;

                LogBuffer = Tools.ConstructLog(Messages.GetDateTime() + "{PlayBlink} " + strings.LoginSuccess,
                    Color.Green, true, true);
                LogChanged?.Invoke();

                var pb = await Parse.PlayBlinkGetProfileAsync(Bot, Bot.GameMinerEnabled);
                LogBuffer = pb;
                LogChanged?.Invoke();
                LoadProfilesInfo?.Invoke();
            }
            else
            {
                lblPBStatus.Text = @"Статус: " + strings.LoginFaild;
                BlockTabpage(tabPagePB, false);
                btnPBLogin.Enabled = true;
                btnPBLogin.Visible = true;
            }
            toolStripStatusLabel1.Image = null;
            toolStripStatusLabel1.Text = @"Завершено";
        }

        private void btnPBExit_Click(object sender, EventArgs e)
        {
            Bot.PlayBlinkPhpSessId = "";
            Bot.PlayBlinkEnabled = false;
            BlockTabpage(tabPagePB, false);
            btnPBLogin.Enabled = true;
            btnPBLogin.Visible = true;
            btnPBExit.Visible = false;
            Tools.SaveProfile(Bot, "");
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://playblink.com/");
        }

        private async void pbPBRefresh_Click(object sender, EventArgs e)
        {
            btnPBExit.Enabled = false;
            pbPBRefresh.Image = Resources.load;
            toolStripStatusLabel1.Text = @"Обновление информации о PlayBlink";
            toolStripStatusLabel1.Image = Resources.load;

            var profile = await Parse.PlayBlinkGetProfileAsync(Bot, false);
            if (profile.Echo)
            {
                LogBuffer = profile;
                LogChanged?.Invoke();
            }
            LoadProfilesInfo?.Invoke();

            if (profile.Success)
            {
                btnPBLogin.Enabled = false;
                btnPBLogin.Visible = false;
                lblPBStatus.Text = @"Статус: " + strings.LoginSuccess;
                BlockTabpage(tabPagePB, true);
            }
            else
            {
                BlockTabpage(tabPagePB, false);
                btnPBLogin.Enabled = true;
                btnPBLogin.Visible = true;
                lblPBStatus.Text = @"Статус: " + strings.LoginFaild;
            }

            toolStripStatusLabel1.Image = null;
            pbPBRefresh.Image = Resources.refresh;
            toolStripStatusLabel1.Text = @"Завершено";
            btnPBExit.Enabled = true;
        }
    }
}