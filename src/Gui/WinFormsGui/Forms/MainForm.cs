using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using KryBot.CommonResources.lang;
using KryBot.Core;
using KryBot.Core.Giveaways;
using KryBot.Gui.WinFormsGui.Properties;

namespace KryBot.Gui.WinFormsGui.Forms
{
	public partial class FormMain : Form
	{
		public delegate void SubscribesContainer();

		private static Bot _bot = new Bot();
		private static Blacklist _blackList;


		private readonly Timer _timer = new Timer();
		private readonly Timer _timerTickCount = new Timer();
		private bool _farming;
		private int _interval;
		private bool _logActive;
		private int _loopsLeft;

		public Log LogBuffer;

		public FormMain()
		{
			InitializeComponent();
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

			new Settings().Load();
			LoadProfilesInfo += ShowProfileInfo;
			_logActive = Properties.Settings.Default.LogActive;
			Design();
			_blackList = Tools.LoadBlackList();

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
				cbSPEnable.Checked = _bot.UseGamble.Enabled;
				cbSTEnable.Checked = _bot.SteamTrade.Enabled;
				cbPBEnabled.Checked = _bot.SteamTrade.Enabled;
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
					LogMessage.Instance.AddMessage(new Log($"{Messages.GetDateTime()} {strings.FormMain_btnStart_Click_FarmSkip}", Color.Red, false));
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
				_bot = Tools.LoadProfile();
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
				return true;
			}

			_bot = new Bot();
			return false;
		}

		private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
		{
			Properties.Settings.Default.LogActive = _logActive;
			_bot.Save();
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
			toolStripProgressBar1.Maximum = 5;
			toolStripProgressBar1.Visible = true;
			toolStripStatusLabel1.Image = Resources.load;
			toolStripStatusLabel1.Text = strings.FormMain_DoFarm_Farn;

			var stopWatch = new Stopwatch();
			stopWatch.Start();

			if(_bot.GameMiner.Enabled)
			{
				var profile = await _bot.GameMiner.CheckLogin();
				LogMessage.Instance.AddMessage(profile);
				LoadProfilesInfo?.Invoke();

				if(profile.Success)
				{
					var won = await _bot.GameMiner.CheckWon();
					if(won != null)
					{
						LogMessage.Instance.AddMessage(won);
						if(Properties.Settings.Default.ShowWonTip)
						{
							ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info);
						}
					}

					await _bot.GameMiner.Join(_blackList);
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
						if (Properties.Settings.Default.ShowWonTip)
						{
							ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info);
						}
					}

					await _bot.GameMiner.Join(_blackList);
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
						if (Properties.Settings.Default.ShowWonTip)
						{
							ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info);
						}
					}

					var giveaways =
						await _bot.SteamCompanion.LoadGiveawaysAsync();
					if (giveaways != null && giveaways.Content != "\n")
					{
						LogMessage.Instance.AddMessage(giveaways);
					}

					if (_bot.SteamCompanion.WishlistGiveaways.Count > 0)
					{
						if (Properties.Settings.Default.Sort)
						{
							if (Properties.Settings.Default.SortToMore)
							{
								if (!Properties.Settings.Default.WishlistNotSort)
								{
									_bot.SteamCompanion.WishlistGiveaways.Sort((a, b) => b.Price.CompareTo(a.Price));
								}
							}
							else
							{
								if (!Properties.Settings.Default.WishlistNotSort)
								{
									_bot.SteamCompanion.WishlistGiveaways.Sort((a, b) => a.Price.CompareTo(b.Price));
								}
							}
						}

						await JoinGiveaways(_bot.SteamCompanion.WishlistGiveaways, true);
					}

					if (_bot.SteamCompanion.Giveaways.Count > 0)
					{
						if (Properties.Settings.Default.Sort)
						{
							if (Properties.Settings.Default.SortToMore)
							{
								_bot.SteamCompanion.Giveaways.Sort((a, b) => b.Price.CompareTo(a.Price));
								if (!Properties.Settings.Default.WishlistNotSort)
								{
									_bot.SteamCompanion.WishlistGiveaways.Sort((a, b) => b.Price.CompareTo(a.Price));
								}
							}
							else
							{
								_bot.SteamCompanion.Giveaways.Sort((a, b) => a.Price.CompareTo(b.Price));
								if (!Properties.Settings.Default.WishlistNotSort)
								{
									_bot.SteamCompanion.WishlistGiveaways.Sort((a, b) => a.Price.CompareTo(b.Price));
								}
							}
						}
					}

					await JoinGiveaways(_bot.SteamCompanion.Giveaways, false);

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
					linkLabel3.Enabled = true;
					lblSCStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
				}
			}

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
						if (Properties.Settings.Default.ShowWonTip)
						{
							ShowBaloolTip(won.Content.Split(']')[1], 5000, ToolTipIcon.Info);
						}
					}


					var giveaways =
						await _bot.UseGamble.LoadGiveawaysAsync(_blackList);
					if (giveaways != null && giveaways.Content != "\n")
					{
						LogMessage.Instance.AddMessage(giveaways);
					}

					if (_bot.UseGamble.Giveaways?.Count > 0)
					{
						if (Properties.Settings.Default.Sort)
						{
							if (Properties.Settings.Default.SortToMore)
							{
								_bot.UseGamble.Giveaways.Sort((a, b) => b.Price.CompareTo(a.Price));
							}
							else
							{
								_bot.UseGamble.Giveaways.Sort((a, b) => a.Price.CompareTo(b.Price));
							}
						}

						await JoinGiveaways(_bot.UseGamble.Giveaways);
					}
				}
				else
				{
					BlockTabpage(tabPageUG, false);
					btnSPLogin.Enabled = true;
					btnSPLogin.Visible = true;
					linkLabel4.Enabled = true;
					lblSPStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
				}
			}

			if (_bot.SteamTrade.Enabled)
			{
				var profile = await _bot.SteamTrade.CheckLogin();
				LogMessage.Instance.AddMessage(profile);
				LoadProfilesInfo?.Invoke();
				if (profile.Success)
				{
					var giveaways = await _bot.SteamTrade.LoadGiveawaysAsync(_blackList);
					if (giveaways != null && giveaways.Content != "\n")
					{
						LogMessage.Instance.AddMessage(giveaways);
					}

					if (_bot.SteamTrade.Giveaways?.Count > 0)
					{
						await JoinGiveaways(_bot.SteamTrade.Giveaways);
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

			if (_bot.PlayBlink.Enabled)
			{
				var profile = await _bot.PlayBlink.PlayBlinkGetProfileAsync();
				LogMessage.Instance.AddMessage(profile);
				LoadProfilesInfo?.Invoke();

				if (profile.Success)
				{
					if (_bot.PlayBlink.Points > 0)
					{
						var giveaways = await _bot.PlayBlink.PlayBlinkLoadGiveawaysAsync(_blackList);
						if (giveaways != null && giveaways.Content != "\n")
						{
							LogMessage.Instance.AddMessage(giveaways);
						}

						if (_bot.PlayBlink.Giveaways?.Count > 0)
						{
							if (Properties.Settings.Default.Sort)
							{
								if (Properties.Settings.Default.SortToMore)
								{
									_bot.PlayBlink.Giveaways.Sort((a, b) => b.Price.CompareTo(a.Price));
								}
								else
								{
									_bot.PlayBlink.Giveaways.Sort((a, b) => a.Price.CompareTo(b.Price));
								}
							}

							await JoinGiveaways(_bot.PlayBlink.Giveaways);
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
			LogMessage.Instance.AddMessage(Messages.DoFarm_Finish(elapsedTime));
			LoadProfilesInfo?.Invoke();

			if (_loopsLeft > 0)
			{
				LogMessage.Instance.AddMessage(new Log($"{Messages.GetDateTime()} {strings.FormMain_timer_Tick_LoopsLeft}: {_loopsLeft - 1}",
					Color.White, true));
				_loopsLeft += -1;
			}

			_bot.ClearGiveawayList();

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
			lblGMCoal.Text = $"{strings.Coal}: {_bot.GameMiner.Coal}";
			lblGMLevel.Text = $"{strings.Level}: {_bot.GameMiner.Level}";

			lblSGPoints.Text = $"{strings.Points}: {_bot.SteamGifts.Points}";
			lblSGLevel.Text = $"{strings.Level}: {_bot.SteamGifts.Level}";

			lblSCPoints.Text = $"{strings.Points}: {_bot.SteamCompanion.Points}";
			lblSCLevel.Text = $"{strings.Level}: -";

			lblSPPoints.Text = $"{strings.Points}: {_bot.UseGamble.Points}";
			lblSPLevel.Text = $"{strings.Level}: -";

			lblSTPoints.Text = $"{strings.Points}: -";
			lblSTLevel.Text = $"{strings.Level}: -";

			lblPBPoints.Text = $"{strings.Points}: {_bot.PlayBlink.Points}";
			lblPBLevel.Text = $"{strings.Level}: {_bot.PlayBlink.Level}";
		}

		private async Task<bool> LoginCheck()
		{
			toolStripProgressBar1.Value = 0;
			toolStripProgressBar1.Maximum = 7;
			toolStripProgressBar1.Visible = true;
			toolStripStatusLabel1.Image = Resources.load;
			toolStripStatusLabel1.Text = strings.TryLogin;
			var login = false;

			if(_bot.GameMiner.Enabled)
			{
				if(await CheckLoginGm())
				{
					var won = await _bot.GameMiner.CheckWon();
					if(won != null)
					{
						LogMessage.Instance.AddMessage(won);
						if(Properties.Settings.Default.ShowWonTip)
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

			if (_bot.SteamCompanion.Enabled)
			{
				if (await CheckLoginSc())
				{
					var won = await _bot.SteamCompanion.CheckWon();
					if (won != null && won.Content != "\n")
					{
						LogMessage.Instance.AddMessage(won);
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

			if (_bot.UseGamble.Enabled)
			{
				if (await CheckLoginSp())
				{
					var won = await _bot.UseGamble.CheckWon();
					if (won != null)
					{
						LogMessage.Instance.AddMessage(won);
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
					BlockTabpage(tabPageUG, false);
					btnSPLogin.Enabled = true;
					btnSPLogin.Visible = true;
					lblSPStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
				}
			}
			else
			{
				BlockTabpage(tabPageUG, false);
				btnSPLogin.Enabled = true;
				btnSPLogin.Visible = true;
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
			var cookie = Tools.GetSessCookieInresponse(getLoginHref.Cookies, "steamtrade.info", "PHPSESSID");

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
			form.Height = Screen.PrimaryScreen.Bounds.Height / 2;
			form.Width = Screen.PrimaryScreen.Bounds.Width / 2;
			form.Name = "KryBot - CefBrowser";
			form.ShowDialog();
		}

		private async void btnSPLogin_Click(object sender, EventArgs e)
		{
			btnSPLogin.Enabled = false;
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
				btnSPLogin.Enabled = false;
				btnSPLogin.Visible = false;
				lblSPStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
				LoadProfilesInfo?.Invoke();
				btnStart.Enabled = true;
				pbSPReload.Visible = true;
				btnSPExit.Visible = true;
				btnSPExit.Enabled = true;
				cbSPEnable.Checked = true;
				_bot.UseGamble.Enabled = true;
			}
			else
			{
				lblSPStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginFaild}";
				BlockTabpage(tabPageUG, false);
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
			             Properties.Settings.Default.Lang + @"&agree=True",
				"http://gameminer.net/?lang=" + Properties.Settings.Default.Lang, "GameMiner - Login", "");

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
			var login = await _bot.GameMiner.CheckLogin();
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
			var login = await _bot.PlayBlink.PlayBlinkGetProfileAsync();
			LogMessage.Instance.AddMessage(login);
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

				var async = await _bot.GameMiner.Sync();
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
			SetStatusPanel("Обновление информации о SteamGifts", Resources.load);

			if (await CheckLoginSg())
			{
				LoadProfilesInfo?.Invoke();
				var won = await _bot.SteamGifts.CheckWon();
				if (won != null)
				{
					LogMessage.Instance.AddMessage(won);
				}

				var async = await _bot.SteamGifts.Sync();
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
			SetStatusPanel("Обновление информации о SteamCompanion", Resources.load);

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
			pbSPReload.Image = Resources.load;
			SetStatusPanel("Обновление информации о UseGamble", Resources.load);

			if (await CheckLoginSp())
			{
				LoadProfilesInfo?.Invoke();
				btnSPLogin.Enabled = false;
				btnSPLogin.Visible = false;
				lblSPStatus.Text = $"{strings.FormMain_Label_Status}: {strings.LoginSuccess}";
				BlockTabpage(tabPageUG, true);
			}
			else
			{
				BlockTabpage(tabPageUG, false);
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
			if (_logActive)
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
			Process.Start(Links.UseGamble);
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
			if (_bot.Save())
			{
				LogMessage.Instance.AddMessage(new Log(Messages.GetDateTime() + " Настройки сохранены в profile.xml", Color.White, true));
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
				if (_bot.Save(saveFileDialog1.FileName))
				{
					LogMessage.Instance.AddMessage(new Log(Messages.GetDateTime() + " Файл сохранен по пути " + saveFileDialog1.FileName,
						Color.White, true));
				}
				else
				{
					LogMessage.Instance.AddMessage(new Log(Messages.GetDateTime() + " Файл НЕ сохранен", Color.Red, true));
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
			if (lblSPStatus.Enabled)
			{
				BlockTabpage(tabPageUG, false);
				cbSPEnable.Enabled = true;
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
				notifyIcon.Icon = Resources.KryBotPresent_256b;
				notifyIcon.Visible = true;
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
			btnSPLogin.Visible = true;
			btnSPLogin.Enabled = true;
			btnSPExit.Visible = false;
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
			_blackList = Tools.LoadBlackList();
		}

		private void настройкиToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			var form = new FormSettings(_bot);
			form.ShowDialog();
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

		private async Task<bool> JoinGiveaways(List<SteamCompanionGiveaway> giveaways, bool wishlist)
		{
			foreach (var giveaway in giveaways)
			{
				if (wishlist)
				{
					if (giveaway.Price <= _bot.SteamCompanion.Points)
					{
						var data =
							await _bot.SteamCompanion.Join(giveaways.IndexOf(giveaway), _bot.Steam);
						if (data != null && data.Content != "\n")
						{
							LogMessage.Instance.AddMessage(data);
						}
					}
				}
				else
				{
					if (giveaway.Price <= _bot.SteamCompanion.Points &&
					    _bot.SteamCompanion.PointsReserv <= _bot.SteamCompanion.Points - giveaway.Price)
					{
						var data =
							await _bot.SteamCompanion.Join(giveaways.IndexOf(giveaway), _bot.Steam);
						if (data != null && data.Content != "\n")
						{
							LogMessage.Instance.AddMessage(data);
						}
					}
				}
			}
			return true;
		}

		private async Task<bool> JoinGiveaways(List<UseGambleGiveaway> giveaways)
		{
			foreach (var giveaway in giveaways)
			{
				if (giveaway.Price <= _bot.UseGamble.Points &&
				    _bot.UseGamble.PointsReserv <= _bot.UseGamble.Points - giveaway.Price)
				{
					var data = await _bot.UseGamble.Join(giveaways.IndexOf(giveaway));
					if (data != null && data.Content != "\n")
					{
						LogMessage.Instance.AddMessage(data);
					}
				}
			}
			return true;
		}

		private async Task<bool> JoinGiveaways(List<SteamTradeGiveaway> giveaways)
		{
			foreach (var giveaway in giveaways)
			{
				var data = await _bot.SteamTrade.Join(giveaways.IndexOf(giveaway));
				if (data != null && data.Content != "\n")
				{
					LogMessage.Instance.AddMessage(data);
				}
			}
			return true;
		}

		private async Task<bool> JoinGiveaways(List<PlayBlinkGiveaway> giveaways)
		{
			foreach (var giveaway in giveaways)
			{
				if (giveaway.Price <= _bot.PlayBlink.MaxJoinValue && giveaway.Price <= _bot.PlayBlink.Points)
				{
					if (_bot.PlayBlink.PointReserv <= _bot.PlayBlink.Points - giveaway.Price || giveaway.Price == 0)
					{
						var data = await _bot.PlayBlink.JoinGiveawayAsync(giveaway);
						if (data != null && data.Content != "\n")
						{
							LogMessage.Instance.AddMessage(data);
						}
					}
				}
			}
			return true;
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
	}
}