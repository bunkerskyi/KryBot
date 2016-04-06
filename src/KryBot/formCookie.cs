using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using KryBot.Properties;

namespace KryBot
{
    public partial class FormCookie : Form
    {
        private readonly Classes.Bot _bot;
        private readonly List<string> _names;
        private readonly string _site;

        public FormCookie(string site, List<string> names, Classes.Bot bot)
        {
            _site = site;
            _names = names;
            _bot = bot;
            InitializeComponent();
        }

        private void formCookie_Load(object sender, EventArgs e)
        {
            Text = _site;
            Icon = Icon.FromHandle(Resources.locked1.GetHicon());

            foreach (var name in _names)
            {
                var label = new Label
                {
                    Location = new Point(6, Controls.Count == 0 ? 6 : Controls[Controls.Count - 1].Location.Y + 26),
                    Text = name.Split(':')[0] + ':',
                    Width = 68
                };
                Controls.Add(label);

                var textbox = new TextBox
                {
                    Location =
                        new Point(10 + label.Size.Width,
                            Controls.Count == 1 ? 3 : Controls[Controls.Count - 2].Location.Y + 26),
                    Name = name.Split(':')[0],
                    Text = name.Split(':')[1],
                    Width = 194
                };
                Controls.Add(textbox);
            }

            var button = new Button
            {
                Location = new Point(6, Controls[Controls.Count - 1].Location.Y + 28),
                Width = 272,
                Text = @"Save"
            };
            button.Click += ButtonOnClick;
            Controls.Add(button);

            Height = Controls[Controls.Count - 1].Location.Y + 68;
        }

        private void ButtonOnClick(object sender, EventArgs eventArgs)
        {
            if (_site == "GameMiner")
            {
                _bot.GameMinerToken = (Controls["token"] as TextBox)?.Text;
                _bot.GameMinerxsrf = (Controls["_xsrf"] as TextBox)?.Text;
                _bot.UserAgent = (Controls["UserAgent"] as TextBox)?.Text;
                _bot.GameMinerEnabled = true;
                Tools.SaveProfile(_bot, "");
                ShowMessage();
                Close();
            }

            if (_site == "SteamGifts")
            {
                _bot.SteamGiftsPhpSessId = (Controls["PHPSESSID"] as TextBox)?.Text;
                _bot.SteamGiftsEnabled = true;
                Tools.SaveProfile(_bot, "");
                ShowMessage();
                Close();
            }

            if (_site == "SteamCompanion")
            {
                _bot.SteamCompanionPhpSessId = (Controls["PHPSESSID"] as TextBox)?.Text;
                _bot.SteamCompanionUserId = (Controls["userid"] as TextBox)?.Text;
                _bot.SteamCompanionUserC = (Controls["userc"] as TextBox)?.Text;
                _bot.SteamCompanionUserT = (Controls["usert"] as TextBox)?.Text;
                _bot.SteamCompanionEnabled = true;
                Tools.SaveProfile(_bot, "");
                ShowMessage();
                Close();
            }

            if (_site == "SteamPortal")
            {
                _bot.SteamPortalPhpSessId = (Controls["PHPSESSID"] as TextBox)?.Text;
                _bot.SteamPortalEnabled = true;
                Tools.SaveProfile(_bot, "");
                ShowMessage();
                Close();
            }

            if (_site == "SteamTrade")
            {
                _bot.SteamTradePhpSessId = (Controls["PHPSESSID"] as TextBox)?.Text;
                _bot.SteamTradeDlePassword = (Controls["dle_password"] as TextBox)?.Text;
                _bot.SteamTradeDleUserId = (Controls["dle_user_id"] as TextBox)?.Text;
                _bot.SteamTradePassHash = (Controls["passhash"] as TextBox)?.Text;
                _bot.SteamTradeEnabled = true;
                Tools.SaveProfile(_bot, "");
                ShowMessage();
                Close();
            }

            if (_site == "Steam")
            {
                _bot.SteamLogin = (Controls["steamLogin"] as TextBox)?.Text;
                _bot.SteamSessid = (Controls["sessionid"] as TextBox)?.Text;
                _bot.SteamEnabled = true;
                Tools.SaveProfile(_bot, "");
                ShowMessage();
                Close();
            }
        }

        private void ShowMessage()
        {
            MessageBox.Show(@"Для применения измененией нужен перезапуск приложения.", @"Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}