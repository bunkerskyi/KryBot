using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using KryBot.lang;
using KryBot.Properties;

namespace KryBot
{
    public partial class FormSettings : Form
    {
        private readonly Bot _bot;
        private bool _userAutorun;

        public FormSettings(Bot bot)
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

            if (Properties.Settings.Default.Timer)
            {
                cbTimerEnable.Checked = true;
                gbTimerSettings.Enabled = true;
                tbTimerInterval.Text = (Properties.Settings.Default.TimerInterval/60000).ToString();
                tbTimerLoops.Text = Properties.Settings.Default.TimerLoops.ToString();
            }
            else
            {
                cbTimerEnable.Checked = false;
                gbTimerSettings.Enabled = false;
                tbTimerInterval.Text = (Properties.Settings.Default.TimerInterval/60000).ToString();
                tbTimerLoops.Text = Properties.Settings.Default.TimerLoops.ToString();
            }

            cbSort.Checked = Properties.Settings.Default.Sort;
            if (Properties.Settings.Default.SortToMore)
            {
                cbSortBy.Text = @"дорогие";
            }

            if (Properties.Settings.Default.SortToLess)
            {
                cbSortBy.Text = @"дешевые";
            }

            cbGMRegular.Checked = _bot.GameMiner.Regular;
            chGMSandbox.Checked = _bot.GameMiner.Sandbox;
            cbGMGolden.Checked = _bot.GameMiner.FreeGolden;
            cbGBOnlyGifts.Checked = _bot.GameMiner.OnlyGifts;
            cbGMRegional.Checked = _bot.GameMiner.NoRegion;
            tbGMMaxValue.Text = _bot.GameMiner.JoinCoalLimit.ToString();
            tbGMReserv.Text = _bot.GameMiner.CoalReserv.ToString();

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

            tbUGMaxValue.Text = _bot.UseGamble.MaxJoinValue.ToString();
            tbUGReserv.Text = _bot.UseGamble.PointsReserv.ToString();

            cbAutorun.Checked = Properties.Settings.Default.Autorun;
            cbWonTip.Checked = Properties.Settings.Default.ShowWonTip;
            cbFarmTip.Checked = Properties.Settings.Default.ShowFarmTip;
            cbFullLog.Checked = Properties.Settings.Default.FullLog;
            cbWishlistSort.Checked = Properties.Settings.Default.WishlistNotSort;
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
            if (cbTimerEnable.Checked && int.Parse(tbTimerInterval.Text) == 0)
            {
                MessageBox.Show(@"Интервал таймера не может равняться 0", strings.Error, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                tbTimerInterval.Text = Properties.Settings.Default.TimerInterval.ToString();
                return;
            }

            Properties.Settings.Default.Sort = cbSort.Checked;
            Properties.Settings.Default.SortToMore = cbSortBy.Text == @"дорогие";
            Properties.Settings.Default.SortToLess = cbSortBy.Text == @"дешевые";

            _bot.GameMiner.Regular = cbGMRegular.Checked;
            _bot.GameMiner.Sandbox = chGMSandbox.Checked;
            _bot.GameMiner.FreeGolden = cbGMGolden.Checked;
            _bot.GameMiner.OnlyGifts = cbGBOnlyGifts.Checked;
            _bot.GameMiner.NoRegion = cbGMRegional.Checked;
            _bot.GameMiner.JoinCoalLimit = int.Parse(tbGMMaxValue.Text);
            _bot.GameMiner.CoalReserv = int.Parse(tbGMReserv.Text);

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

            _bot.UseGamble.MaxJoinValue = int.Parse(tbUGMaxValue.Text);
            _bot.UseGamble.PointsReserv = int.Parse(tbUGReserv.Text);

            _bot.PlayBlink.MaxJoinValue = int.Parse(tbPBMaxValue.Text);
            _bot.PlayBlink.PointReserv = int.Parse(tbPBReserv.Text);

            Properties.Settings.Default.Timer = cbTimerEnable.Checked;
            Properties.Settings.Default.TimerInterval = int.Parse(tbTimerInterval.Text)*60000;
            Properties.Settings.Default.TimerLoops = int.Parse(tbTimerLoops.Text);

            Properties.Settings.Default.Autorun = cbAutorun.Checked;
            Properties.Settings.Default.ShowWonTip = cbWonTip.Checked;
            Properties.Settings.Default.ShowFarmTip = cbFarmTip.Checked;
            Properties.Settings.Default.FullLog = cbFullLog.Checked;
            Properties.Settings.Default.WishlistNotSort = cbWishlistSort.Checked;

            _bot.Save();
            Properties.Settings.Default.Save();
        }

        private void btnSGCookies_Click(object sender, EventArgs e)
        {
            var names = new List<string> {"PHPSESSID:" + _bot.SteamGifts.Cookies.PhpSessId};

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

        private void btnUGCookies_Click(object sender, EventArgs e)
        {
            var names = new List<string> {"PHPSESSID:" + _bot.UseGamble.Cookies.PhpSessId};

            var form = new FormCookie("UseGamble", names, _bot);
            form.ShowDialog();
        }

        private void btnSTCookies_Click(object sender, EventArgs e)
        {
            var names = new List<string>
            {
                "PHPSESSID:" + _bot.SteamTrade.Cookies.PhpSessId,
                "dle_user_id:" + _bot.SteamTrade.Cookies.DleUserId,
                "dle_password:" + _bot.SteamTrade.Cookies.DlePassword,
                "passhash:" + _bot.SteamTrade.Cookies.PassHash
            };

            var form = new FormCookie("SteamTrade", names, _bot);
            form.ShowDialog();
        }

        private void FormSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            _bot.Save();
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

        private void tbUGMaxValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != (char) Keys.Back))
                e.Handled = true;
        }

        private void tbUGReserv_KeyPress(object sender, KeyPressEventArgs e)
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
                "steamLogin:" + _bot.Steam.Cookies.Login,
                "sessionid:" + _bot.Steam.Cookies.Sessid,
                "steamRememberLogin:" + _bot.Steam.Cookies.RememberLogin
            };

            var form = new FormCookie("Steam", names, _bot);
            form.ShowDialog();
        }

        private void btnDeleteCookies_Click(object sender, EventArgs e)
        {
            try
            {
                Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Cookies), true);
                MessageBox.Show(@"Для применения изменений нужен перезапуск приложения", @"Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void btnPBCookies_Click(object sender, EventArgs e)
        {
            var names = new List<string> {"PHPSESSID:" + _bot.PlayBlink.Cookies.PhpSessId};

            var form = new FormCookie("PlayBlink", names, _bot);
            form.ShowDialog();
        }

        private void tbPBMaxValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != (char) Keys.Back))
                e.Handled = true;
        }

        private void tbPBReserv_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && (e.KeyChar != (char) Keys.Back))
                e.Handled = true;
        }
    }
}