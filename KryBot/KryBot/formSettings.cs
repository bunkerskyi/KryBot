using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using KryBot.Properties;

namespace KryBot
{
    public partial class FormSettings : Form
    {
        private readonly Classes.Bot _bot;
        private bool _userAutorun;

        public FormSettings(Classes.Bot bot)
        {
            _bot = bot;
            InitializeComponent();
        }

        private void formSettings_Load(object sender, EventArgs e)
        {
            Design();
        }

        private void Design()
        {
            Text = @"Настройки";
            Icon = Icon.FromHandle(Resources.settings.GetHicon());

            cbSortBy.Text = cbSortBy.Items[0].ToString();

            if (Settings.Default.Timer)
            {
                cbTimerEnable.Checked = true;
                gbTimerSettings.Enabled = true;
                tbTimerInterval.Text = (Settings.Default.TimerInterval/60000).ToString();
                tbTimerLoops.Text = Settings.Default.TimerLoops.ToString();
            }
            else
            {
                cbTimerEnable.Checked = false;
                gbTimerSettings.Enabled = false;
                tbTimerInterval.Text = (Settings.Default.TimerInterval/60000).ToString();
                tbTimerLoops.Text = Settings.Default.TimerLoops.ToString();
            }

            cbSort.Checked = Settings.Default.Sort;
            if (Settings.Default.SortToMore)
            {
                cbSortBy.Text = @"дорогие";
            }

            if (Settings.Default.SortToLess)
            {
                cbSortBy.Text = @"дешевые";
            }

            cbGMRegular.Checked = _bot.GameMinerRegular;
            chGMSandbox.Checked = _bot.GameMinerSandbox;
            cbGMGolden.Checked = _bot.GameMinerFreeGolden;
            cbGBOnlyGifts.Checked = _bot.GameMinerOnlyGifts;
            cbGMRegional.Checked = _bot.GameMinerNoRegion;
            tbGMMaxValue.Text = _bot.GameMinerJoinCoalLimit.ToString();
            tbGMReserv.Text = _bot.GameMinerCoalReserv.ToString();

            cbSGWishlist.Checked = _bot.SteamGiftsWishList;
            cbSGGroup.Checked = _bot.SteamGiftsGroup;
            cbSGRegular.Checked = _bot.SteamGiftsRegular;
            cbSGMinLevel.Checked = _bot.SteamGiftsSortLevel;
            cbSGSortTeLessLevel.Checked = _bot.SteamGiftsSortToLessLevel;
            numSGLevel.Enabled = _bot.SteamGiftsSortLevel;
            numSGLevel.Value = _bot.SteamGiftsMinLevel;
            tbSGMaxValue.Text = _bot.SteamGiftsJoinPointLimit.ToString();
            tbSGReserv.Text = _bot.SteamGiftsPointsReserv.ToString();

            cbSCWishlist.Checked = _bot.SteamCompanionWishList;
            cbSCGroup.Checked = _bot.SteamCompanionGroup;
            cbSCRegular.Checked = _bot.SteamCompanionRegular;
            cbSCAutojoin.Enabled = _bot.SteamEnabled;
            cbSCAutojoin.Enabled = cbSCGroup.Checked;
            cbSCAutojoin.Checked = _bot.SteamCompanionAutoJoin;
            tbSCMaxValue.Text = _bot.SteamCompanionJoinPointLimit.ToString();
            tbSCReserv.Text = _bot.SteamCompanionPointsReserv.ToString();

            tbSPMaxValue.Text = _bot.SteamPortalMaxJoinValue.ToString();
            tbSPReserv.Text = _bot.SteamPortalPointsReserv.ToString();

            cbAutorun.Checked = Settings.Default.Autorun;
            cbWonTip.Checked = Settings.Default.ShowWonTip;
            cbFarmTip.Checked = Settings.Default.ShowFarmTip;
            cbFullLog.Checked = Settings.Default.FullLog;
        }

        private void btbGMCookies_Click(object sender, EventArgs e)
        {
            var names = new List<string> {"token:" + _bot.GameMinerToken, "_xsrf:" + _bot.GameMinerxsrf, "UserAgent:" + _bot.UserAgent};

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
            Settings.Default.Sort = cbSort.Checked;
            Settings.Default.SortToMore = cbSortBy.Text == @"дорогие";
            Settings.Default.SortToLess = cbSortBy.Text == @"дешевые";

            _bot.GameMinerRegular = cbGMRegular.Checked;
            _bot.GameMinerSandbox = chGMSandbox.Checked;
            _bot.GameMinerFreeGolden = cbGMGolden.Checked;
            _bot.GameMinerOnlyGifts = cbGBOnlyGifts.Checked;
            _bot.GameMinerNoRegion = cbGMRegional.Checked;
            _bot.GameMinerJoinCoalLimit = int.Parse(tbGMMaxValue.Text);
            _bot.GameMinerCoalReserv = int.Parse(tbGMReserv.Text);

            _bot.SteamGiftsWishList = cbSGWishlist.Checked;
            _bot.SteamGiftsGroup = cbSGGroup.Checked;
            _bot.SteamGiftsRegular = cbSGRegular.Checked;
            _bot.SteamGiftsJoinPointLimit = int.Parse(tbSGMaxValue.Text);
            _bot.SteamGiftsPointsReserv = int.Parse(tbSGReserv.Text);
            _bot.SteamGiftsMinLevel = int.Parse(numSGLevel.Value.ToString(CultureInfo.InvariantCulture));
            _bot.SteamGiftsSortLevel = cbSGMinLevel.Checked;
            _bot.SteamGiftsSortToLessLevel = cbSGSortTeLessLevel.Checked;

            _bot.SteamCompanionWishList = cbSCWishlist.Checked;
            _bot.SteamCompanionGroup = cbSCGroup.Checked;
            _bot.SteamCompanionRegular = cbSCRegular.Checked;
            _bot.SteamCompanionAutoJoin = cbSCAutojoin.Checked;
            _bot.SteamCompanionJoinPointLimit = int.Parse(tbSCMaxValue.Text);
            _bot.SteamCompanionPointsReserv = int.Parse(tbSCReserv.Text);

            _bot.SteamPortalMaxJoinValue = int.Parse(tbSPMaxValue.Text);
            _bot.SteamPortalPointsReserv = int.Parse(tbSPReserv.Text);

            Settings.Default.Timer = cbTimerEnable.Checked;
            Settings.Default.TimerInterval = int.Parse(tbTimerInterval.Text)*60000;
            Settings.Default.TimerLoops = int.Parse(tbTimerLoops.Text);

            Settings.Default.Autorun = cbAutorun.Checked;                               
            Settings.Default.ShowWonTip = cbWonTip.Checked;
            Settings.Default.ShowFarmTip = cbFarmTip.Checked;
            Settings.Default.FullLog = cbFullLog.Checked;

            Tools.SaveProfile(_bot, "");
            Settings.Default.Save();
        }

        private void btnSGCookies_Click(object sender, EventArgs e)
        {
            var names = new List<string> {"PHPSESSID:" + _bot.SteamGiftsPhpSessId};

            var form = new FormCookie("SteamGifts", names, _bot);
            form.ShowDialog();
        }

        private void btnSCCookies_Click(object sender, EventArgs e)
        {
            var names = new List<string>
            {
                "PHPSESSID:" + _bot.SteamCompanionPhpSessId,
                "userc:" + _bot.SteamCompanionUserC,
                "userid:" + _bot.SteamCompanionUserId,
                "usert:" + _bot.SteamCompanionUserT
            };

            var form = new FormCookie("SteamCompanion", names, _bot);
            form.ShowDialog();
        }

        private void btnSPCookies_Click(object sender, EventArgs e)
        {
            var names = new List<string> {"PHPSESSID:" + _bot.SteamPortalPhpSessId};

            var form = new FormCookie("SteamPortal", names, _bot);
            form.ShowDialog();
        }

        private void btnSTCookies_Click(object sender, EventArgs e)
        {
            var names = new List<string>
            {
                "PHPSESSID:" + _bot.SteamTradePhpSessId,
                "dle_user_id:" + _bot.SteamTradeDleUserId,
                "dle_password:" + _bot.SteamTradeDlePassword,
                "passhash:" + _bot.SteamTradePassHash
            };

            var form = new FormCookie("SteamTrade", names, _bot);
            form.ShowDialog();
        }

        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            Tools.SaveProfile(_bot, "");
        }

        private void tbGMMaxValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != (char) Keys.Back))
                e.Handled = true;
        }

        private void tbGMReserv_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != (char) Keys.Back))
                e.Handled = true;
        }

        private void tbSGMaxValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != (char) Keys.Back))
                e.Handled = true;
        }

        private void tbSGReserv_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != (char) Keys.Back))
                e.Handled = true;
        }

        private void tbSCMaxValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != (char) Keys.Back))
                e.Handled = true;
        }

        private void tbSCReserv_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != (char) Keys.Back))
                e.Handled = true;
        }

        private void tbSPMaxValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != (char) Keys.Back))
                e.Handled = true;
        }

        private void tbSPReserv_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != (char) Keys.Back))
                e.Handled = true;
        }

        private void cbTimerEnable_CheckedChanged(object sender, EventArgs e)
        {
            gbTimerSettings.Enabled = cbTimerEnable.Checked;
        }

        private void tbTimerInterval_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != (char) Keys.Back))
                e.Handled = true;
        }

        private void tbTimerLoops_KeyPress(object sender, KeyPressEventArgs e)
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
                    if (!Tools.SetAutorun())
                    {
                        _userAutorun = false;
                        cbAutorun.Checked = false;
                    }
                }
                else
                {
                    if (!Tools.DeleteAutorun())
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

        private void button1_Click(object sender, EventArgs e)
        {
            var names = new List<string>
            {
                "steamLogin:" + _bot.SteamLogin,
                "sessionid:" + _bot.SteamSessid
            };

            var form = new FormCookie("Steam", names, _bot);
            form.ShowDialog();
        }

        private void btnDeleteCookies_Click(object sender, EventArgs e)
        {
            try
            {
                System.IO.Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Cookies), true);
                MessageBox.Show(@"Для применения изменений нужен перезапуск приложения", @"Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                // ignored
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
    }
}