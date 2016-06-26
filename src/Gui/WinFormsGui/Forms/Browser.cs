using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using KryBot.Core;
using KryBot.Gui.WinFormsGui.Properties;

namespace KryBot.Gui.WinFormsGui.Forms
{
    public partial class Browser : Form
    {
        private readonly Bot _bot;
        private readonly string _endPage;
        private readonly string _phpSessId;
        private readonly string _startPage;
        private readonly string _title;
        private ChromiumWebBrowser _browser;

        public Browser(Bot bot, string startPage, string endPage, string title, string phpSessId)
        {
            _bot = bot;
            _startPage = startPage;
            _endPage = endPage;
            _title = title;
            _phpSessId = phpSessId;
            InitializeComponent();
            InitializeBrowserControl();
        }

        private void InitializeBrowserControl()
        {
            if (!Cef.IsInitialized)
            {
                var cefSettings = new CefSettings
                {
                    UserAgent = CefTools.GetUserAgent()
                };
                Cef.Initialize(cefSettings);
            }

            _browser = new ChromiumWebBrowser(_startPage)
            {
                Dock = DockStyle.Fill
            };
            browserPanel.Controls.Add(_browser);

            _browser.LoadingStateChanged += BrowserOnLoadingStateChanged;
        }

        private async void NewBrowser_Load(object sender, EventArgs e)
        {
            Text = _title;
            Icon = Resources.KryBotPresent_256b;
            toolStripStatusLabelChromium.Text =
                $"Chromium: {Cef.ChromiumVersion} Cef: {Cef.CefVersion} CefSharp: {Cef.CefSharpVersion}";

            if (_phpSessId != "")
            {
                await Cef.GetGlobalCookieManager().SetCookieAsync(Links.SteamTrade, new Cookie
                {
                    Domain = "steamtrade.info",
                    Name = "PHPSESSID",
                    Value = _phpSessId
                });
            }
        }

        private void BrowserOnLoadingStateChanged(object sender,
            LoadingStateChangedEventArgs loadingStateChangedEventArgs)
        {
            if (!loadingStateChangedEventArgs.IsLoading)
            {
                loadStatusLabel.Image = Resources.refresh;

                if (_browser.Address.Contains(_endPage) ||
                    _browser.Address.Contains("http://steamcommunity.com/profiles/"))
                {
                    if (_endPage == "http://steamcommunity.com/id/")
                    {
                        SteamAuth();
                        Exit();
                    }
                }

                if (_browser.Address == _endPage || _browser.Address == "https://www.steamgifts.com/register")
                {
                    if (_endPage.Contains("http://gameminer.net/?lang="))
                    {
                        GameMinerAuth();
                        Exit();
                    }

                    if (_endPage == Links.SteamGifts)
                    {
                        SteamGiftsAuth();
                        Exit();
                    }

                    if (_endPage == Links.SteamCompanion)
                    {
                        SteamCompanionAuth();
                        Exit();
                    }

                    if (_endPage == Links.UseGamble)
                    {
                        UseGambleAuth();
                        Exit();
                    }

                    if (_endPage == Links.SteamTrade)
                    {
                        SteamTradeAuth();
                        Exit();
                    }

                    if (_endPage == Links.PlayBlink)
                    {
                        PlayBlinkAuth();
                        Exit();
                    }

                    if (_endPage == Links.GameAways)
                    {
                        GameAwaysAuth();
                        Exit();
                    }

                    if (_endPage == Links.InventoryGifts)
                    {
                        InventoryGiftsAuth();
                        Exit();
                    }
                }
            }
            else
            {
                if (loadStatusLabel.Image != Resources.refresh)
                {
                    loadStatusLabel.Image = Resources.load;
                }
            }
        }

        private async void InventoryGiftsAuth()
        {
            foreach (var cookie in await GetCookies(Links.InventoryGifts))
            {
                if (cookie.Name == "PHPSESSID")
                {
                    _bot.InventoryGifts.Cookies.Phpsessid = cookie.Value;
                }

                if (cookie.Name == "hash")
                {
                    _bot.InventoryGifts.Cookies.Hash = cookie.Value;
                }

                if (cookie.Name == "steamid")
                {
                    _bot.InventoryGifts.Cookies.Steamid = cookie.Value;
                }
            }
            _bot.InventoryGifts.Enabled = true;
        }

        private async void GameAwaysAuth()
        {
            foreach (var cookie in await GetCookies(Links.GameAways))
            {
                if (cookie.Name == ".AspNet.ApplicationCookie")
                {
                    _bot.GameAways.Cookies.AspNetApplicationCookie = cookie.Value;
                }
            }
            _bot.GameAways.Enabled = true;
        }

        private async void PlayBlinkAuth()
        {
            foreach (var cookie in await GetCookies(Links.PlayBlink))
            {
                if (cookie.Name == "PHPSESSID")
                {
                    _bot.PlayBlink.Cookies.PhpSessId = cookie.Value;
                }
            }
            _bot.PlayBlink.Enabled = true;
        }

        private async void SteamTradeAuth()
        {
            foreach (var cookie in await GetCookies(Links.SteamTrade))
            {
                if (cookie.Name == "PHPSESSID")
                {
                    _bot.SteamTrade.Cookies.PhpSessId = cookie.Value;
                }

                if (cookie.Name == "dle_user_id")
                {
                    _bot.SteamTrade.Cookies.DleUserId = cookie.Value;
                }

                if (cookie.Name == "dle_password")
                {
                    _bot.SteamTrade.Cookies.DlePassword = cookie.Value;
                }

                if (cookie.Name == "passhash")
                {
                    _bot.SteamTrade.Cookies.PassHash = cookie.Value;
                }
            }
            _bot.SteamTrade.Enabled = true;
        }

        private async void UseGambleAuth()
        {
            foreach (var cookie in await GetCookies(Links.UseGamble))
            {
                if (cookie.Name == "PHPSESSID")
                {
                    _bot.UseGamble.Cookies.PhpSessId = cookie.Value;
                }
            }
            _bot.UseGamble.Enabled = true;
        }

        private async void SteamCompanionAuth()
        {
            foreach (var cookie in await GetCookies(Links.SteamCompanion))
            {
                if (cookie.Name == "PHPSESSID")
                {
                    _bot.SteamCompanion.Cookies.PhpSessId = cookie.Value;
                }

                if (cookie.Name == "userc")
                {
                    _bot.SteamCompanion.Cookies.UserC = cookie.Value;
                }

                if (cookie.Name == "userid")
                {
                    _bot.SteamCompanion.Cookies.UserId = cookie.Value;
                }

                if (cookie.Name == "usert")
                {
                    _bot.SteamCompanion.Cookies.UserT = cookie.Value;
                }
            }
            _bot.SteamCompanion.Enabled = true;
        }

        private async void SteamGiftsAuth()
        {
            foreach (var cookie in await GetCookies(Links.SteamGifts))
            {
                if (cookie.Name == "PHPSESSID")
                {
                    _bot.SteamGifts.Cookies.PhpSessId = cookie.Value;
                }
            }
            _bot.SteamGifts.Enabled = true;
        }

        private async void GameMinerAuth()
        {
            foreach (var cookie in await GetCookies(Links.GameMiner))
            {
                if (cookie.Name == "token")
                {
                    _bot.GameMiner.Cookies.Token = cookie.Value;
                }

                if (cookie.Name == "_xsrf")
                {
                    _bot.GameMiner.Cookies.Xsrf = cookie.Value;
                }
            }

            _bot.GameMiner.Enabled = true;
        }

        private async void SteamAuth()
        {
            var allCookies = await GetCookies(Links.Steam);

            foreach (var cookie in allCookies)
            {
                if (cookie.Domain == "steamcommunity.com")
                {
                    if (cookie.Name == "sessionid")
                    {
                        _bot.Steam.Cookies.Sessid = cookie.Value;
                    }

                    if (cookie.Name == "steamLogin")
                    {
                        _bot.Steam.Cookies.Login = cookie.Value;
                    }

                    if (cookie.Name == "steamLoginSecure")
                    {
                        _bot.Steam.Cookies.LoginSecure = cookie.Value;
                    }

                    if (cookie.Name.Contains("steamMachineAuth"))
                    {
                        _bot.Steam.Cookies.MachineAuth = cookie.Value;
                    }
                }
            }

            _bot.Steam.Enabled = true;
        }

        private async Task<List<Cookie>> GetCookies(string url)
        {
            return await Cef.GetGlobalCookieManager().VisitUrlCookiesAsync(url, true);
        }

        private void Exit()
        {
            if (IsHandleCreated)
            {
                Invoke((MethodInvoker) Close);
            }
        }

        private void loadStatusLabel_Click(object sender, EventArgs e)
        {
            if (!_browser.IsLoading)
            {
                _browser.Reload();
            }
        }
    }
}