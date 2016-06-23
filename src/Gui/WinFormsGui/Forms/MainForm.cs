using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using KryBot.CommonResources.Localization;
using KryBot.Core;
using KryBot.Core.Helpers;
using KryBot.Gui.WinFormsGui.Properties;

namespace KryBot.Gui.WinFormsGui.Forms
{
	public partial class FormMain : Form
	{
		public delegate void SubscribesContainer();

		private static Bot _bot = new Bot();
		private static Settings _settings;


		private readonly Timer _timer = new Timer();
		private readonly Timer _timerTickCount = new Timer();
		private bool _farming;
		private int _interval;
		private bool _logActive;
		private int _loopsLeft;

		public Log LogBuffer;

		public FormMain()
		{
			_settings = Settings.Load();
			GetLocalization();
			InitializeComponent();
			Localization();
		}

		public event SubscribesContainer LogHide;
		public event SubscribesContainer LogUnHide;
		public event SubscribesContainer FormChangeLocation;
		public event SubscribesContainer LoadProfilesInfo;

		private void логToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_logActive)
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

			LoadProfilesInfo += ShowProfileInfo;
			_logActive = _settings.IsLogActive;
			Design();

			var version = await Updater.CheckForUpdates();
			LogMessage.Instance.AddMessage(version);

			if (version.Success)
			{
				var dr = MessageBox.Show($"{version.Content.Replace("\n", "")}. Обновить?", @"Обновление",
					MessageBoxButtons.YesNo, MessageBoxIcon.Information);
				if (dr == DialogResult.Yes)
				{
					LogMessage.Instance.AddMessage(new Log($"{Messages.GetDateTime()} Обновление...", Color.White, true));

					toolStripProgressBar1.Style = ProgressBarStyle.Marquee;
					toolStripProgressBar1.Visible = true;

					var log = await Updater.Update();
					LogMessage.Instance.AddMessage(log);

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
				cbGMEnable.Checked = _bot.GameMiner.Enabled;
				cbSGEnable.Checked = _bot.SteamGifts.Enabled;
				cbSCEnable.Checked = _bot.SteamCompanion.Enabled;
				cbUGEnable.Checked = _bot.UseGamble.Enabled;
				cbSTEnable.Checked = _bot.SteamTrade.Enabled;
				cbPBEnabled.Checked = _bot.SteamTrade.Enabled;
				cbGAEnabled.Checked = _bot.GameAways.Enabled;
				btnStart.Enabled = await LoginCheck();

				if (_bot.Steam.Enabled)
				{
					await
						_bot.Steam.Join("http://steamcommunity.com/groups/krybot");
				}
			}
			else
			{
				btnGMLogin.Visible = true;
				btnSGLogin.Visible = true;
				btnSCLogin.Visible = true;
				btnUGLogin.Visible = true;
				btnSTLogin.Visible = true;
				btnPBLogin.Visible = true;
				btnSteamLogin.Visible = true;
				btnGALogin.Visible = true;
				btnStart.Enabled = false;
			}
		}

		private void FormMain_LocationChanged(object sender, EventArgs e)
		{
			FormChangeLocation?.Invoke();
		}

		private async void btnStart_Click(object sender, EventArgs e)
		{
			if (_bot.Timer)
			{
				if (!_farming)
				{
					_loopsLeft = _bot.TimerLoops;
					_timer.Interval = _bot.TimerInterval;
					_interval = _timer.Interval;
					_timerTickCount.Interval = 1000;
					_timer.Tick += timer_Tick;
					_timerTickCount.Tick += TimerTickCountOnTick;
					_timer.Start();
					_timerTickCount.Start();
					btnStart.Text =
						$"{strings.FormMain_btnStart_Click_Stop} ({TimeSpan.FromMilliseconds(_timer.Interval)})";
					if (_settings.ShowFarmTip)
					{
						ShowBaloolTip(
							$"{strings.FormMain_btnStart_Click_FarmBeginWithInterval} {_interval/60000} {strings.FormMain_btnStart_Click_M}",
							5000, ToolTipIcon.Info);
					}
					await DoFarm();
					if (_settings.ShowFarmTip)
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

					if (_settings.ShowFarmTip)
					{
						ShowBaloolTip(strings.FormMain_btnStart_Click_FarmBegin, 5000, ToolTipIcon.Info);
					}

					await DoFarm();

					if (_settings.ShowFarmTip)
					{
						ShowBaloolTip(strings.FormMain_btnStart_Click_FarmFinish, 5000, ToolTipIcon.Info);
					}

					btnStart.Text = strings.FormMain_btnStart_Click_Старт;
					btnStart.Enabled = true;
				}
				else
				{
					LogMessage.Instance.AddMessage(new Log($"{Messages.GetDateTime()} {strings.FormMain_btnStart_Click_FarmSkip}",
						Color.Red, false));
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
			btnStart.Enabled = false;
			btnGMLogin.Visible = false;
			btnSGLogin.Visible = false;
			btnSCLogin.Visible = false;
			btnUGLogin.Visible = false;
			btnSTLogin.Visible = false;
			btnPBLogin.Visible = false;
			btnSteamLogin.Visible = false;
			btnGMExit.Visible = false;
			btnSGExit.Visible = false;
			btnSCExit.Visible = false;
			btnUGExit.Visible = false;
			btnSTExit.Visible = false;
			btnPBExit.Visible = false;
			btnSteamExit.Visible = false;

			toolStripProgressBar1.Visible = false;
			toolStripStatusLabel1.Image = null;
			toolStripStatusLabel1.Text = "";

			pbGMReload.Visible = false;
			pbSGReload.Visible = false;
			pbSCReload.Visible = false;
			pbUGReload.Visible = false;
			pbSTreload.Visible = false;
			pbPBRefresh.Visible = false;

			if (_logActive)
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
				_bot = FileHelper.Load<Bot>(FilePaths.Profile);
				if (_bot == null)
				{
					var message = Messages.FileLoadFailed("profile.xml");
					message.Content += "\n";
					LogMessage.Instance.AddMessage(message);

					MessageBox.Show(message.Content.Split(']')[1], strings.Error, MessageBoxButtons.OK,
						MessageBoxIcon.Error);
					return false;
				}
				LogMessage.Instance.AddMessage(Messages.ProfileLoaded());
				LogMessage.Instance.AddMessage(Messages.GamesInBlacklist(_bot.Blacklist?.Items.Count ?? 0));
				return true;
			}

			_bot = new Bot();
			return false;
		}

		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			_settings.IsLogActive = _logActive;
			_bot.Save();
			_settings.Save();
			Properties.Settings.Default.Save();
		}

		private void OpenLog()
		{
			логToolStripMenuItem.Text = $"{strings.Log} <<";
			var form = new FormLog(Location.X + Width - 15, Location.Y, _settings) {Owner = this};

			LogHide += form.FormHide;
			LogUnHide += form.FormUnHide;
			FormChangeLocation += form.FormChangeLocation;

			LogBuffer = Messages.Start();
			form.Show();
			_logActive = true;
		}

		private void UnHideLog()
		{
			LogUnHide?.Invoke();
			логToolStripMenuItem.Text = $"{strings.Log} <<";
			_logActive = true;
		}

		private void HideLog()
		{
			LogHide?.Invoke();
			логToolStripMenuItem.Text = $"{strings.Log} >>";
			_logActive = false;
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
			LogMessage.Instance.AddMessage(Messages.DoFarm_Start());

			toolStripProgressBar1.Value = 0;
			toolStripProgressBar1.Maximum = 7;
			toolStripProgressBar1.Visible = true;
			toolStripStatusLabel1.Image = Resources.load;
			toolStripStatusLabel1.Text = strings.FormMain_DoFarm_Farn;

			var stopWatch = new Stopwatch();
			stopWatch.Start();

			if (_bot.GameMiner.Enabled)
			{
				var profile = await _bot.GameMiner.CheckLogin(_settings.Lang);
				LogMessage.Instance.AddMessage(profile);
				LoadProfilesInfo?.Invoke();

				if (profile.Success)
				{
					var won = await _bot.GameMiner.CheckWon(_settings.Lang);
					if (won != null)
					{
						LogMessage.Instance.AddMessage(won);
						if (_settings.ShowWonTip)
						{
							ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info);
						}
					}

					await _bot.GameMiner.Join(_bot.Blacklist, _bot.Sort, _bot.SortToMore, _settings.Lang);
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

			if (_bot.SteamGifts.Enabled)
			{
				var profile = await _bot.SteamGifts.CheckLogin();
				LogMessage.Instance.AddMessage(profile);
				LoadProfilesInfo?.Invoke();

				if (profile.Success)
				{
					var won = await _bot.SteamGifts.CheckWon();
					if (won != null)
					{
						LogMessage.Instance.AddMessage(won);
						if (_settings.ShowWonTip)
						{
							ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info);
						}
					}

					await _bot.SteamGifts.Join(_bot.Blacklist, _bot.Sort, _bot.SortToMore);
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

			if (_bot.SteamCompanion.Enabled)
			{
				var profile = await _bot.SteamCompanion.CheckLogin();
				LogMessage.Instance.AddMessage(profile);
				LoadProfilesInfo?.Invoke();

				if (profile.Success)
				{
					var won = await _bot.SteamCompanion.CheckWon();
					if (won != null)
					{
						LogMessage.Instance.AddMessage(won);
						if (_settings.ShowWonTip)
						{
							ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info);
						}
					}

					await _bot.SteamCompanion.Join(_bot.Steam, _bot.Sort, _bot.SortToMore);
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
			toolStripProgressBar1.Value++;

			if (_bot.UseGamble.Enabled)
			{
				var profile = await _bot.UseGamble.CheckLogin();
				LogMessage.Instance.AddMessage(profile);
				LoadProfilesInfo?.Invoke();

				if (profile.Success)
				{
					var won = await _bot.UseGamble.CheckWon();
					if (won != null)
					{
						LogMessage.Instance.AddMessage(won);
						if (_settings.ShowWonTip)
						{
							ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info);
						}
					}

					await _bot.UseGamble.Join(_bot.Sort, _bot.SortToMore, _bot.Blacklist);
				}
				else
				{
					BlockTabpage(tabPageUG, false);
					btnUGLogin.Enabled = true;
					btnUGLogin.Visible = true;
					linkLabelUG.Enabled = true;
					lblUGStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
				}
			}
			toolStripProgressBar1.Value++;

			if (_bot.SteamTrade.Enabled)
			{
				var profile = await _bot.SteamTrade.CheckLogin();
				LogMessage.Instance.AddMessage(profile);
				LoadProfilesInfo?.Invoke();

				if (profile.Success)
				{
					await _bot.SteamTrade.Join(_bot.Blacklist);
				}
				else
				{
					BlockTabpage(tabPageST, false);
					btnSTLogin.Enabled = true;
					btnSTLogin.Visible = true;
					linkLabelST.Enabled = true;
					lblSTStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
				}
			}
			toolStripProgressBar1.Value++;

			if (_bot.PlayBlink.Enabled)
			{
				var profile = await _bot.PlayBlink.CheckLogin();
				LogMessage.Instance.AddMessage(profile);
				LoadProfilesInfo?.Invoke();

				if (profile.Success)
				{
					await _bot.PlayBlink.Join(_bot.Sort, _bot.SortToMore, _bot.Blacklist);
				}
				else
				{
					BlockTabpage(tabPagePB, false);
					btnPBLogin.Enabled = true;
					btnPBLogin.Visible = true;
					linkLabelPB.Enabled = true;
					lblPBStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
				}
			}
			toolStripProgressBar1.Value++;

			if (_bot.GameAways.Enabled)
			{
				var profile = await _bot.GameAways.CheckLogin();
				LogMessage.Instance.AddMessage(profile);
				LoadProfilesInfo?.Invoke();

				if (profile.Success)
				{
					await _bot.GameAways.Join(_bot.Blacklist, _bot.Sort, _bot.SortToMore);
				}
				else
				{
					BlockTabpage(tabPageGA, false);
					btnGALogin.Enabled = true;
					btnGALogin.Visible = true;
					linkLabelGA.Enabled = true;
					lblGAStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
				}
			}
			toolStripProgressBar1.Value++;

			stopWatch.Stop();
			var ts = stopWatch.Elapsed;
			string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}";
			LogMessage.Instance.AddMessage(Messages.DoFarm_Finish(elapsedTime));
			LoadProfilesInfo?.Invoke();

			if (_loopsLeft > 0)
			{
				LogMessage.Instance.AddMessage(
					new Log($"{Messages.GetDateTime()} {strings.FormMain_timer_Tick_LoopsLeft}: {_loopsLeft - 1}",
						Color.White, true));
				_loopsLeft += -1;
			}

			_bot.ClearGiveawayList();

			toolStripProgressBar1.Visible = false;
			toolStripStatusLabel1.Image = null;
			toolStripStatusLabel1.Text = strings.StatusBar_End;
			_farming = false;
			btnStart.Enabled = true;
			Properties.Settings.Default.Save();

			return true;
		}

		private void ShowProfileInfo()
		{
			lblGMCoal.Text = $"{strings.Points}: {_bot.GameMiner.Points}";
			lblGMLevel.Text = $"{strings.Level}: {_bot.GameMiner.Level}";

			lblSGPoints.Text = $"{strings.Points}: {_bot.SteamGifts.Points}";
			lblSGLevel.Text = $"{strings.Level}: {_bot.SteamGifts.Level}";

			lblSCPoints.Text = $"{strings.Points}: {_bot.SteamCompanion.Points}";
			lblSCLevel.Text = $"{strings.Level}: -";

			lblUGPoints.Text = $"{strings.Points}: {_bot.UseGamble.Points}";
			lblUGLevel.Text = $"{strings.Level}: -";

			lblSTPoints.Text = $"{strings.Points}: -";
			lblSTLevel.Text = $"{strings.Level}: -";

			lblPBPoints.Text = $"{strings.Points}: {_bot.PlayBlink.Points}";
			lblPBLevel.Text = $"{strings.Level}: {_bot.PlayBlink.Level}";

			lblGAPoints.Text = $"{strings.Points}: {_bot.GameAways.Points}";
			lblGALevel.Text = $"{strings.Level}: -";
		}

		private async Task<bool> LoginCheck()
		{
			toolStripProgressBar1.Value = 0;
			toolStripProgressBar1.Maximum = 8;
			toolStripProgressBar1.Visible = true;
			toolStripStatusLabel1.Image = Resources.load;
			toolStripStatusLabel1.Text = strings.TryLogin;
			var login = false;

			if (_bot.GameMiner.Enabled)
			{
				if (await CheckLoginGm())
				{
					var won = await _bot.GameMiner.CheckWon(_settings.Lang);
					if (won != null)
					{
						LogMessage.Instance.AddMessage(won);
						if (_settings.ShowWonTip)
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

			if (_bot.SteamGifts.Enabled)
			{
				if (await CheckLoginSg())
				{
					var won = await _bot.SteamGifts.CheckWon();
					if (won != null && won.Content != "\n")
					{
						LogMessage.Instance.AddMessage(won);
						if (_settings.ShowWonTip)
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

			if (_bot.SteamCompanion.Enabled)
			{
				if (await CheckLoginSc())
				{
					var won = await _bot.SteamCompanion.CheckWon();
					if (won != null && won.Content != "\n")
					{
						LogMessage.Instance.AddMessage(won);
						if (_settings.ShowWonTip)
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

			if (_bot.UseGamble.Enabled)
			{
				if (await CheckLoginSp())
				{
					var won = await _bot.UseGamble.CheckWon();
					if (won != null)
					{
						LogMessage.Instance.AddMessage(won);
						if (_settings.ShowWonTip)
						{
							ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info);
						}
					}

					login = true;
					btnUGLogin.Enabled = false;
					btnUGLogin.Visible = false;
					lblUGStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
					pbUGReload.Visible = true;
					btnUGExit.Visible = true;
				}
				else
				{
					BlockTabpage(tabPageUG, false);
					btnUGLogin.Enabled = true;
					btnUGLogin.Visible = true;
					lblUGStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
				}
			}
			else
			{
				BlockTabpage(tabPageUG, false);
				btnUGLogin.Enabled = true;
				btnUGLogin.Visible = true;
			}
			toolStripProgressBar1.Value++;

			if (_bot.SteamTrade.Enabled)
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

			if (_bot.PlayBlink.Enabled)
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

			if (_bot.Steam.Enabled)
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

			if (_bot.GameAways.Enabled)
			{
				if (await CheckLoginGa())
				{
					LoadProfilesInfo?.Invoke();
					login = true;
					btnGALogin.Enabled = false;
					btnGALogin.Visible = false;
					lblGAStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
					btnGAExit.Visible = true;
				}
				else
				{
					BlockTabpage(tabPageGA, false);
					btnGALogin.Enabled = true;
					btnGALogin.Visible = true;
					lblGAStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
				}
			}
			else
			{
				BlockTabpage(tabPageGA, false);
				btnGALogin.Enabled = true;
				btnGALogin.Visible = true;
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
			var first = await Web.GetAsync(Links.SteamTrade, new CookieContainer());
			var getLoginHref = await _bot.SteamTrade.DoAuth(first.Cookies);
			var location = getLoginHref.RestResponse.ResponseUri.ToString();
			var cookie = CookieHelper.GetSessCookieInresponse(getLoginHref.Cookies, "steamtrade.info", "PHPSESSID");

			BrowserStart(location, Links.SteamTrade, "SteamTrade - Login", cookie);
			_bot.Save();

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
				_bot.SteamTrade.Enabled = true;
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

		private static void BrowserStart(string startPage, string endPage, string title, string phpSessId)
		{
			Form form = new Browser(_bot, startPage, endPage, title, phpSessId);
			form.Height = Screen.PrimaryScreen.Bounds.Height/2;
			form.Width = Screen.PrimaryScreen.Bounds.Width/2;
			form.Name = "KryBot - CefBrowser";
			form.ShowDialog();
		}

		private async void btnSPLogin_Click(object sender, EventArgs e)
		{
			btnUGLogin.Enabled = false;
			BrowserStart($"{Links.UseGamble}page/steam", Links.UseGamble, "UseGamble - Login", "");
			_bot.Save();

			toolStripStatusLabel1.Image = Resources.load;
			toolStripStatusLabel1.Text = strings.StatusBar_Login;
			var login = await CheckLoginSp();
			if (login)
			{
				var won = await _bot.UseGamble.CheckWon();
				if (won != null && won.Content != "\n")
				{
					LogMessage.Instance.AddMessage(won);
				}

				BlockTabpage(tabPageUG, true);
				btnUGLogin.Enabled = false;
				btnUGLogin.Visible = false;
				lblUGStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
				LoadProfilesInfo?.Invoke();
				btnStart.Enabled = true;
				pbUGReload.Visible = true;
				btnUGExit.Visible = true;
				btnUGExit.Enabled = true;
				cbUGEnable.Checked = true;
				_bot.UseGamble.Enabled = true;
			}
			else
			{
				lblUGStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
				BlockTabpage(tabPageUG, false);
				btnUGLogin.Enabled = true;
				btnUGLogin.Visible = true;
			}
			toolStripStatusLabel1.Image = null;
			toolStripStatusLabel1.Text = strings.StatusBar_End;
		}

		private async void btnSCLogin_Click(object sender, EventArgs e)
		{
			btnSCLogin.Enabled = false;
			BrowserStart($"{Links.SteamCompanion}login", Links.SteamCompanion, "SteamCompanion - Login", "");
			_bot.Save();

			toolStripStatusLabel1.Image = Resources.load;
			toolStripStatusLabel1.Text = strings.StatusBar_Login;
			var login = await CheckLoginSc();
			if (login)
			{
				var won = await _bot.SteamCompanion.CheckWon();
				if (won != null && won.Content != "\n")
				{
					LogMessage.Instance.AddMessage(won);
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
				_bot.SteamCompanion.Enabled = true;
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

			if (string.IsNullOrEmpty(_bot.SteamGifts.UserAgent))
			{
				_bot.SteamGifts.UserAgent = CefTools.GetUserAgent();
			}
			_bot.Save();

			toolStripStatusLabel1.Image = Resources.load;
			toolStripStatusLabel1.Text = strings.StatusBar_Login;
			var login = await CheckLoginSg();
			if (login)
			{
				var won = await _bot.SteamGifts.CheckWon();
				if (won != null && won.Content != "\n")
				{
					LogMessage.Instance.AddMessage(won);
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
				_bot.SteamGifts.Enabled = true;
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
			             _settings.Lang + @"&agree=True",
				"http://gameminer.net/?lang=" + _settings.Lang, "GameMiner - Login", "");

			if (string.IsNullOrEmpty(_bot.GameMiner.UserAgent))
			{
				_bot.GameMiner.UserAgent = CefTools.GetUserAgent();
			}
			_bot.Save();

			toolStripStatusLabel1.Image = Resources.load;
			toolStripStatusLabel1.Text = strings.StatusBar_Login;
			var login = await CheckLoginGm();
			if (login)
			{
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
				_bot.GameMiner.Enabled = true;
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
			var login = await _bot.GameMiner.CheckLogin(_settings.Lang);
			LogMessage.Instance.AddMessage(login);
			return login.Success;
		}

		private async Task<bool> CheckLoginSg()
		{
			Message_TryLogin("SteamGifts");
			var login = await _bot.SteamGifts.CheckLogin();
			LogMessage.Instance.AddMessage(login);
			return login.Success;
		}

		private async Task<bool> CheckLoginSc()
		{
			Message_TryLogin("SteamCompanion");
			var login = await _bot.SteamCompanion.CheckLogin();
			LogMessage.Instance.AddMessage(login);
			return login.Success;
		}

		private async Task<bool> CheckLoginSp()
		{
			Message_TryLogin("UseGamble");
			var login = await _bot.UseGamble.CheckLogin();
			LogMessage.Instance.AddMessage(login);
			return login.Success;
		}

		private async Task<bool> CheckLoginSt()
		{
			Message_TryLogin("SteamTrade");
			var login = await _bot.SteamTrade.CheckLogin();
			LogMessage.Instance.AddMessage(login);
			return login.Success;
		}

		private async Task<bool> CheckLoginSteam()
		{
			Message_TryLogin("Steam");
			var login = await _bot.Steam.CheckLogin();
			LogMessage.Instance.AddMessage(login);
			return login.Success;
		}

		private async Task<bool> CheckLoginPb()
		{
			Message_TryLogin("PlayBlink");
			var login = await _bot.PlayBlink.CheckLogin();
			LogMessage.Instance.AddMessage(login);
			return login.Success;
		}

		private async Task<bool> CheckLoginGa()
		{
			Message_TryLogin("GameAways");
			var login = await _bot.GameAways.CheckLogin();
			LogMessage.Instance.AddMessage(login);
			return login.Success;
		}

		private async void pbGMReload_Click(object sender, EventArgs e)
		{
			btnGMExit.Enabled = false;
			pbGMReload.Image = Resources.load;
			SetStatusPanel($"{strings.MainForm_UpdateInfoAbout} GameMiner", Resources.load);

			if (await CheckLoginGm())
			{
				LoadProfilesInfo?.Invoke();

				var async = await _bot.GameMiner.Sync(_settings.Lang);
				if (async != null)
				{
					LogMessage.Instance.AddMessage(async);
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
			SetStatusPanel($"{strings.MainForm_UpdateInfoAbout} SteamGifts", Resources.load);

			if (await CheckLoginSg())
			{
				LoadProfilesInfo?.Invoke();
				var won = await _bot.SteamGifts.CheckWon();
				if (won != null)
				{
					LogMessage.Instance.AddMessage(won);
				}

				var async = await _bot.SteamGifts.SyncAccount();
				if (async != null)
				{
					LogMessage.Instance.AddMessage(async);
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
			SetStatusPanel($"{strings.MainForm_UpdateInfoAbout} SteamCompanion", Resources.load);

			if (await CheckLoginSc())
			{
				LoadProfilesInfo?.Invoke();
				var won = await _bot.SteamCompanion.CheckWon();
				if (won != null)
				{
					LogMessage.Instance.AddMessage(won);
				}
				btnSCLogin.Enabled = false;
				btnSCLogin.Visible = false;
				lblSCStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
				BlockTabpage(tabPageSC, true);

				var async = await _bot.SteamCompanion.Sync();
				if (async != null)
				{
					LogMessage.Instance.AddMessage(async);
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
			pbUGReload.Image = Resources.load;
			SetStatusPanel($"{strings.MainForm_UpdateInfoAbout} UseGamble", Resources.load);

			if (await CheckLoginSp())
			{
				LoadProfilesInfo?.Invoke();
				btnUGLogin.Enabled = false;
				btnUGLogin.Visible = false;
				lblUGStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
				BlockTabpage(tabPageUG, true);
			}
			else
			{
				BlockTabpage(tabPageUG, false);
				btnUGLogin.Enabled = true;
				btnUGLogin.Visible = true;
				lblUGStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
			}

			pbUGReload.Image = Resources.refresh;
			SetStatusPanel(strings.Finish, null);
			btnGMExit.Enabled = true;
		}

		private async void pbSTreload_Click(object sender, EventArgs e)
		{
			btnGMExit.Enabled = false;
			pbSTreload.Image = Resources.load;
			SetStatusPanel($"{strings.MainForm_UpdateInfoAbout} SteamTrade", Resources.load);

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
		    if (WindowState == FormWindowState.Minimized)
		    {
		        Show();
		        WindowState = FormWindowState.Normal;
		        if (_logActive)
		        {
		            LogUnHide?.Invoke();
		        }
		    }
		    else
		    {
                Hide();
                WindowState = FormWindowState.Minimized;
                if (_logActive)
                {
                    LogHide?.Invoke();
                }
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
			Process.Start(Links.UseGamble);
		}

		private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(Links.SteamTrade);
		}

		private void оПрограммеToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			var form = new FormAbout();
			form.ShowDialog();
		}

		private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_bot.Save())
			{
				LogMessage.Instance.AddMessage(new Log($"{Messages.GetDateTime()} Настройки сохранены в profile.xml", Color.White,
					true));
			}

			Properties.Settings.Default.Save();
		}

		private void сохранитьКакToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var saveFileDialog1 = new SaveFileDialog
			{
				Filter = @"XML|*.xml",
				Title = strings.FileDialog_SaveProfile
			};
			saveFileDialog1.ShowDialog();

			if (saveFileDialog1.FileName != "")
			{
				if (_bot.Save(saveFileDialog1.FileName))
				{
					LogMessage.Instance.AddMessage(
						new Log($"{Messages.GetDateTime()} Файл сохранен по пути {saveFileDialog1.FileName}",
							Color.White, true));
				}
				else
				{
					LogMessage.Instance.AddMessage(new Log($"{Messages.GetDateTime()} Файл НЕ сохранен", Color.Red, true));
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
				_bot.SteamTrade.Enabled = false;
			}
			else
			{
				BlockTabpage(tabPageST, true);
				_bot.SteamTrade.Enabled = true;
			}
		}

		private void checkBox3_CheckedChanged(object sender, EventArgs e)
		{
			if (lblSGStatus.Enabled)
			{
				BlockTabpage(tabPageSG, false);
				cbSGEnable.Enabled = true;
				_bot.SteamGifts.Enabled = false;
			}
			else
			{
				BlockTabpage(tabPageSG, true);
				_bot.SteamGifts.Enabled = true;
			}
		}

		private void cbGMEnable_CheckedChanged(object sender, EventArgs e)
		{
			if (lblGMStatus.Enabled)
			{
				BlockTabpage(tabPageGM, false);
				cbGMEnable.Enabled = true;
				_bot.GameMiner.Enabled = false;
			}
			else
			{
				BlockTabpage(tabPageGM, true);
				_bot.GameMiner.Enabled = true;
			}
		}

		private void cbSPEnable_CheckedChanged(object sender, EventArgs e)
		{
			if (lblUGStatus.Enabled)
			{
				BlockTabpage(tabPageUG, false);
				cbUGEnable.Enabled = true;
				_bot.UseGamble.Enabled = false;
			}
			else
			{
				BlockTabpage(tabPageUG, true);
				_bot.UseGamble.Enabled = true;
			}
		}

		private void cbSCEnable_CheckedChanged(object sender, EventArgs e)
		{
			if (lblSCStatus.Enabled)
			{
				BlockTabpage(tabPageSC, false);
				cbSCEnable.Enabled = true;
				_bot.SteamCompanion.Enabled = false;
			}
			else
			{
				BlockTabpage(tabPageSC, true);
				_bot.SteamCompanion.Enabled = true;
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
			_bot.Save();

			toolStripStatusLabel1.Image = Resources.load;
			toolStripStatusLabel1.Text = strings.StatusBar_Login;
			var login = await CheckLoginSteam();
			if (login)
			{
				await _bot.Steam.Join("http://steamcommunity.com/groups/krybot");

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
				if (_logActive)
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

			if (_logActive)
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
			_bot.Steam.Logout();
			BlockTabpage(tabPageSteam, false);
			btnSteamLogin.Visible = true;
			btnSteamLogin.Enabled = true;
			btnSteamExit.Visible = false;
			_bot.Save();
		}

		private void btnSTExit_Click(object sender, EventArgs e)
		{
			_bot.SteamTrade.Logout();
			BlockTabpage(tabPageST, false);
			btnSTLogin.Visible = true;
			btnSTExit.Visible = false;
			btnSTLogin.Enabled = true;
			_bot.Save();
		}

		private void btnSPExit_Click(object sender, EventArgs e)
		{
			_bot.UseGamble.Logout();
			BlockTabpage(tabPageUG, false);
			btnUGLogin.Visible = true;
			btnUGLogin.Enabled = true;
			btnUGExit.Visible = false;
			_bot.Save();
		}

		private void btnSCExit_Click(object sender, EventArgs e)
		{
			_bot.SteamCompanion.Logout();
			BlockTabpage(tabPageSC, false);
			btnSCLogin.Visible = true;
			btnSCLogin.Enabled = true;
			btnSCExit.Visible = false;
			_bot.Save();
		}

		private void btnSGExit_Click(object sender, EventArgs e)
		{
			_bot.SteamGifts.Logout();
			BlockTabpage(tabPageSG, false);
			btnSGLogin.Visible = true;
			btnSGLogin.Enabled = true;
			btnSGExit.Visible = false;
			_bot.Save();
		}

		private void btnGMExit_Click(object sender, EventArgs e)
		{
			_bot.GameMiner.Logout();
			BlockTabpage(tabPageGM, false);
			btnGMLogin.Enabled = true;
			btnGMLogin.Visible = true;
			btnGMExit.Visible = false;
			_bot.Save();
		}

		private void вПапкуСБотомToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Process.Start("explorer", Environment.CurrentDirectory);
		}

		private void черныйСписокToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var form = new FormBlackList(_bot);
			form.ShowDialog();
			LogMessage.Instance.AddMessage(Messages.GamesInBlacklist(_bot.Blacklist?.Items.Count ?? 0));
		}

		private void настройкиToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			var form = new FormSettings(_bot, _settings);
			form.ShowDialog();
			if (CultureInfo.CurrentCulture.Name != _settings.Lang)
			{
				SetLocalization();
				MessageBox.Show(strings.CookieForm_MustRestart, strings.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private async void btnPBLogin_Click(object sender, EventArgs e)
		{
			btnPBLogin.Enabled = false;
			BrowserStart("http://playblink.com/?do=login&act=signin", Links.PlayBlink, "PlayBlink - Login", "");
			_bot.Save();

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
				_bot.PlayBlink.Enabled = true;
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
			_bot.PlayBlink.Logout();
			BlockTabpage(tabPagePB, false);
			btnPBLogin.Enabled = true;
			btnPBLogin.Visible = true;
			btnPBExit.Visible = false;
			_bot.Save();
		}

		private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(Links.PlayBlink);
		}

		private async void pbPBRefresh_Click(object sender, EventArgs e)
		{
			btnPBExit.Enabled = false;
			pbPBRefresh.Image = Resources.load;
			SetStatusPanel($"{strings.MainForm_UpdateInfoAbout} PlayBlink", Resources.load);

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

		private void Message_TryLogin(string site)
		{
			LogMessage.Instance.AddMessage(new Log($"{Messages.GetDateTime()} {{{site}}} {strings.TryLogin}", Color.White, true));
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
				_bot.PlayBlink.Enabled = false;
			}
			else
			{
				BlockTabpage(tabPagePB, true);
				_bot.PlayBlink.Enabled = true;
			}
		}

		private async void buttonLoginGA_Click(object sender, EventArgs e)
		{
			btnGALogin.Enabled = false;
			BrowserStart(Links.GameAwaysAuth, Links.GameAways, "GameAways - Login", "");
			_bot.Save();

			toolStripStatusLabel1.Image = Resources.load;
			toolStripStatusLabel1.Text = strings.StatusBar_Login;
			var login = await CheckLoginGa();
			if (login)
			{
				BlockTabpage(tabPageGA, true);
				btnGALogin.Enabled = false;
				btnGALogin.Visible = false;
				lblGAStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
				LoadProfilesInfo?.Invoke();
				btnStart.Enabled = true;
				pbGARefresh.Visible = true;
				btnGAExit.Visible = true;
				btnGAExit.Enabled = true;
				cbGAEnabled.Checked = true;
				_bot.GameAways.Enabled = true;
			}
			else
			{
				lblGAStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
				BlockTabpage(tabPageGA, false);
				btnGALogin.Enabled = true;
				btnGALogin.Visible = true;
			}
			toolStripStatusLabel1.Image = null;
			toolStripStatusLabel1.Text = strings.StatusBar_End;
		}

		private void buttonExitGA_Click(object sender, EventArgs e)
		{
		}

		private void linkLabelGA_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(Links.GameAways);
		}

		private void cbGAEnabled_CheckedChanged(object sender, EventArgs e)
		{
			if (lblGAStatus.Enabled)
			{
				BlockTabpage(tabPageGA, false);
				cbGAEnabled.Enabled = true;
				_bot.GameAways.Enabled = false;
			}
			else
			{
				BlockTabpage(tabPageGA, true);
				_bot.GameAways.Enabled = true;
			}
		}

		private void GetLocalization()
		{
			if (string.IsNullOrEmpty(_settings.Lang))
			{
				string lang;
				switch (CultureInfo.CurrentCulture.Name)
				{
					case "ru-RU":
						lang = "ru-RU";
						break;
					case "uk-UA":
						lang = "ru-RU";
						break;
					default:
						lang = "en-EN";
						break;
				}
				_settings.Lang = lang;
				_settings.Save();
			}

			SetLocalization();
		}

		private void SetLocalization()
		{
			CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(_settings.Lang);
			CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo(_settings.Lang);
		}
	}
}