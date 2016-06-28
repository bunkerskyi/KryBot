using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using KryBot.CommonResources.Localization;
using KryBot.Core;
using KryBot.Core.Helpers;

namespace KryBot.Gui.WinFormsGui.Forms
{
    public partial class FormSettings : Form
    {
        private readonly Bot _bot;
        private readonly Settings _settings;
        private bool _userAutorun;

        public FormSettings(Bot bot, Settings settings)
        {
            _bot = bot;
            _settings = settings;
            InitializeComponent();
            Localization();
        }

        private void formSettings_Load(object sender, EventArgs e)
        {
            Design();
        }

        private void Design()
        {
            LoadLags();

            cbSortBy.Text = cbSortBy.Items[0].ToString();

            if (_bot.Timer)
            {
                cbTimerEnable.Checked = true;
                gbTimerSettings.Enabled = true;
                tbTimerInterval.Text = (_bot.TimerInterval/60000).ToString();
                tbTimerLoops.Text = _bot.TimerLoops.ToString();
            }
            else
            {
                cbTimerEnable.Checked = false;
                gbTimerSettings.Enabled = false;
                tbTimerInterval.Text = (_bot.TimerInterval/60000).ToString();
                tbTimerLoops.Text = _bot.TimerLoops.ToString();
            }

            cbSort.Checked = _bot.Sort;
            if (_bot.SortToMore)
            {
                cbSortBy.Text = strings.SettingsForm_FirstExpensive;
            }

            if (_bot.SortToLess)
            {
                cbSortBy.Text = strings.SettingsForm_FirstCheap;
            }

            cbGMRegular.Checked = _bot.GameMiner.Regular;
            chGMSandbox.Checked = _bot.GameMiner.Sandbox;
            cbGMGolden.Checked = _bot.GameMiner.FreeGolden;
            cbGMRegional.Checked = _bot.GameMiner.NoRegion;
            tbGMMaxValue.Text = _bot.GameMiner.JoinPointsLimit.ToString();
            tbGMReserv.Text = _bot.GameMiner.PointsReserv.ToString();

            cbSGWishlist.Checked = _bot.SteamGifts.WishList;
            cbSGGroup.Checked = _bot.SteamGifts.Group;
            cbSGRegular.Checked = _bot.SteamGifts.Regular;
            cbSGMinLevel.Checked = _bot.SteamGifts.SortLevel;
            cbSGSortTeLessLevel.Checked = _bot.SteamGifts.SortToLessLevel;
            numSGLevel.Enabled = _bot.SteamGifts.SortLevel;
            numSGLevel.Value = _bot.SteamGifts.MinLevel;
            tbSGMaxValue.Text = _bot.SteamGifts.JoinPointLimit.ToString();
            tbSGReserv.Text = _bot.SteamGifts.PointsReserv.ToString();

            cbSCWishlist.Checked = _bot.SteamCompanion.WishList;
            cbSCContributors.Checked = _bot.SteamCompanion.Contributors;
            cbSCGroup.Checked = _bot.SteamCompanion.Group;
            cbSCRegular.Checked = _bot.SteamCompanion.Regular;
            cbSCAutojoin.Enabled = _bot.Steam.Enabled;
            cbSCAutojoin.Enabled = cbSCGroup.Checked;
            cbSCAutojoin.Checked = _bot.SteamCompanion.AutoJoin;
            tbSCMaxValue.Text = _bot.SteamCompanion.JoinPointLimit.ToString();
            tbSCReserv.Text = _bot.SteamCompanion.PointsReserv.ToString();

            tbPBMaxValue.Text = _bot.PlayBlink.MaxJoinValue.ToString();
            tbPBReserv.Text = _bot.PlayBlink.PointReserv.ToString();

            tbSPMaxValue.Text = _bot.UseGamble.MaxJoinValue.ToString();
            tbSPReserv.Text = _bot.UseGamble.PointsReserv.ToString();

            tbGAMaxBet.Text = _bot.GameAways.JoinPointsLimit.ToString();
            tbGAReserv.Text = _bot.GameAways.PointsReserv.ToString();

            cbIGSteamGiveaways.Checked = _bot.InventoryGifts.SteamGifts;
            cbIGSteamItems.Checked = _bot.InventoryGifts.SteamItems;
            cbIGTF2.Checked = _bot.InventoryGifts.Tf2Items;
            cbIGCSGO.Checked = _bot.InventoryGifts.CsGoitem;
            cbIGDota.Checked = _bot.InventoryGifts.DotaItems;
            tbIGReserv.Text = _bot.InventoryGifts.PointsReserv.ToString();
            tbIGMaxValue.Text = _bot.InventoryGifts.JoinPointsLimit.ToString();

            cbAutorun.Checked = _settings.Autorun;
            cbWonTip.Checked = _settings.ShowWonTip;
            cbFarmTip.Checked = _settings.ShowFarmTip;
            cbWishlistSort.Checked = _bot.WishlistSort;
        }

        private void btbGMCookies_Click(object sender, EventArgs e)
        {
            var names = new List<string>
            {
                "token:" + _bot.GameMiner.Cookies.Token,
                "_xsrf:" + _bot.GameMiner.Cookies.Xsrf,
                "UserAgent:" + _bot.GameMiner.UserAgent
            };

            var form = new FormCookie("GameMiner", names, _bot);
            form.ShowDialog();
        }

        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Close();
        }

        private void SaveSettings()
        {
            _settings.Lang = List.GetShortLang(cbLang.Text);

            if (cbTimerEnable.Checked && int.Parse(tbTimerInterval.Text) == 0)
            {
                MessageBox.Show(strings.SettingsForm_IntervalNotZero, strings.Error, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                tbTimerInterval.Text = _bot.TimerInterval.ToString();
                return;
            }

            _bot.Sort = cbSort.Checked;
            _bot.SortToMore = cbSortBy.Text == strings.SettingsForm_FirstExpensive;
            _bot.SortToLess = cbSortBy.Text == strings.SettingsForm_FirstCheap;

            _bot.GameMiner.Regular = cbGMRegular.Checked;
            _bot.GameMiner.Sandbox = chGMSandbox.Checked;
            _bot.GameMiner.FreeGolden = cbGMGolden.Checked;
            _bot.GameMiner.NoRegion = cbGMRegional.Checked;
            _bot.GameMiner.JoinPointsLimit = int.Parse(tbGMMaxValue.Text);
            _bot.GameMiner.PointsReserv = int.Parse(tbGMReserv.Text);

            _bot.SteamGifts.WishList = cbSGWishlist.Checked;
            _bot.SteamGifts.Group = cbSGGroup.Checked;
            _bot.SteamGifts.Regular = cbSGRegular.Checked;
            _bot.SteamGifts.JoinPointLimit = int.Parse(tbSGMaxValue.Text);
            _bot.SteamGifts.PointsReserv = int.Parse(tbSGReserv.Text);
            _bot.SteamGifts.MinLevel = int.Parse(numSGLevel.Value.ToString(CultureInfo.InvariantCulture));
            _bot.SteamGifts.SortLevel = cbSGMinLevel.Checked;
            _bot.SteamGifts.SortToLessLevel = cbSGSortTeLessLevel.Checked;

            _bot.SteamCompanion.WishList = cbSCWishlist.Checked;
            _bot.SteamCompanion.Contributors = cbSCContributors.Checked;
            _bot.SteamCompanion.Group = cbSCGroup.Checked;
            _bot.SteamCompanion.Regular = cbSCRegular.Checked;
            _bot.SteamCompanion.AutoJoin = cbSCAutojoin.Checked;
            _bot.SteamCompanion.JoinPointLimit = int.Parse(tbSCMaxValue.Text);
            _bot.SteamCompanion.PointsReserv = int.Parse(tbSCReserv.Text);

            _bot.UseGamble.MaxJoinValue = int.Parse(tbSPMaxValue.Text);
            _bot.UseGamble.PointsReserv = int.Parse(tbSPReserv.Text);

            _bot.PlayBlink.MaxJoinValue = int.Parse(tbPBMaxValue.Text);
            _bot.PlayBlink.PointReserv = int.Parse(tbPBReserv.Text);

            _bot.GameAways.JoinPointsLimit = int.Parse(tbGAMaxBet.Text);
            _bot.GameAways.PointsReserv = int.Parse(tbGAReserv.Text);

            _bot.InventoryGifts.SteamGifts = cbIGSteamGiveaways.Checked;
            _bot.InventoryGifts.SteamItems = cbIGSteamItems.Checked;
            _bot.InventoryGifts.Tf2Items = cbIGTF2.Checked;
            _bot.InventoryGifts.CsGoitem = cbIGCSGO.Checked;
            _bot.InventoryGifts.DotaItems = cbIGDota.Checked;
            _bot.InventoryGifts.JoinPointsLimit = int.Parse(tbIGMaxValue.Text);
            _bot.InventoryGifts.PointsReserv = int.Parse(tbIGReserv.Text);

            _bot.Timer = cbTimerEnable.Checked;
            _bot.TimerInterval = int.Parse(tbTimerInterval.Text)*60000;
            _bot.TimerLoops = int.Parse(tbTimerLoops.Text);

            _settings.Autorun = cbAutorun.Checked;
            _settings.ShowWonTip = cbWonTip.Checked;
            _settings.ShowFarmTip = cbFarmTip.Checked;
            _bot.WishlistSort = cbWishlistSort.Checked;

            _bot.Save();
            _settings.Save();
        }

        private void btnSGCookies_Click(object sender, EventArgs e)
        {
            var names = new List<string>
            {
                "PHPSESSID:" + _bot.SteamGifts.Cookies.PhpSessId,
                "UserAgent:" + _bot.SteamGifts.UserAgent
            };

            var form = new FormCookie("SteamGifts", names, _bot);
            form.ShowDialog();
        }

        private void btnSCCookies_Click(object sender, EventArgs e)
        {
            var names = new List<string>
            {
                "PHPSESSID:" + _bot.SteamCompanion.Cookies.PhpSessId,
                "userc:" + _bot.SteamCompanion.Cookies.UserC,
                "userid:" + _bot.SteamCompanion.Cookies.UserId,
                "usert:" + _bot.SteamCompanion.Cookies.UserT
            };

            var form = new FormCookie("SteamCompanion", names, _bot);
            form.ShowDialog();
        }

        private void btnSPCookies_Click(object sender, EventArgs e)
        {
            var names = new List<string> {"PHPSESSID:" + _bot.UseGamble.Cookies.PhpSessId};

            var form = new FormCookie("UseGamble", names, _bot);
            form.ShowDialog();
        }

        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            _bot.Save();
        }

        private void EventKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != (char) Keys.Back))
                e.Handled = true;
        }

        private void cbAutorun_CheckedChanged(object sender, EventArgs e)
        {
            if (_userAutorun)
            {
                if (cbAutorun.Checked)
                {
                    if (!AutorunHelper.SetAutorun(Application.ExecutablePath))
                    {
                        _userAutorun = false;
                        cbAutorun.Checked = false;
                    }
                }
                else
                {
                    if (!AutorunHelper.DeleteAutorun())
                    {
                        _userAutorun = false;
                        cbAutorun.Checked = true;
                    }
                }
            }
            else
            {
                _userAutorun = true;
            }
        }

        private void cbSCGroup_CheckedChanged(object sender, EventArgs e)
        {
            cbSCAutojoin.Enabled = cbSCGroup.Checked;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            numSGLevel.Enabled = cbSGMinLevel.Checked;
        }

        private void btnPBCookies_Click(object sender, EventArgs e)
        {
            var names = new List<string> {"PHPSESSID:" + _bot.PlayBlink.Cookies.PhpSessId};

            var form = new FormCookie("PlayBlink", names, _bot);
            form.ShowDialog();
        }

        private void cbTimerEnable_CheckedChanged(object sender, EventArgs e)
        {
            gbTimerSettings.Enabled = cbTimerEnable.Checked;
        }

        private void LoadLags()
        {
            var array = List.GetFullLangs();
            foreach (var item in array)
            {
                cbLang.Items.Add(item);
            }
            cbLang.Text = List.GetFullLang(_settings.Lang);
        }

        private void btnShortcut_Click(object sender, EventArgs e)
        {
            ShortcutHelper.Create();
        }

        private void btnIGCookie_Click(object sender, EventArgs e)
        {
            var cookies = new List<string>
            {
                "PHPSESSID:" + _bot.InventoryGifts.Cookies.Phpsessid,
                "hash:" + _bot.InventoryGifts.Cookies.Hash,
                "steamid:" + _bot.InventoryGifts.Cookies.Steamid
            };

            new FormCookie("InventoryGifts", cookies, _bot).ShowDialog();
        }

        private void btnGACookies_Click(object sender, EventArgs e)
        {
            var cookies = new List<string>
            {
                ".AspNet.ApplicationCookie:" + _bot.GameAways.Cookies.AspNetApplicationCookie
            };

            new FormCookie("GameAways", cookies, _bot).ShowDialog();
        }
    }
}