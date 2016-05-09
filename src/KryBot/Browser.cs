using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using KryBot.Properties;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace KryBot
{
    public partial class Browser : Form
    {
        private const int InternetCookieHttponly = 0x2000;
        private readonly Bot _bot;
        private readonly string _endPage;
        private readonly string _phpSessId;
        private readonly string _startPage;
        private readonly string _title;

        public Browser(Bot bot, string startPage, string endPage, string title, string phpSessId)
        {
            _bot = bot;
            _startPage = startPage;
            _endPage = endPage;
            _title = title;
            _phpSessId = phpSessId;
            InitializeComponent();
        }

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool InternetSetCookie(string lpszUrlName, string lbszCookieName, string lpszCookieData);

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetGetCookieEx(
            string url,
            string cookieName,
            StringBuilder cookieData,
            ref int size,
            int dwFlags,
            IntPtr lpReserved);

        private void Browser_Load(object sender, EventArgs e)
        {
            Text = _title;
            Icon = Resources.KryBotPresent_256b;
            toolStripStatusLabelIE.Text = $"IE: {webBrowser.Version}";
            webBrowser.ScriptErrorsSuppressed = true;
            webBrowser.DocumentCompleted += wb_DocumentCompleted;

            if (_phpSessId != "")
            {
                InternetSetCookie("http://steamtrade.info/", "PHPSESSID", _phpSessId);
            }
            toolStripStatusLabelURL.Text = @"URL: " + _startPage;
            webBrowser.Navigate(_startPage);
        }

        private void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            toolStripStatusLabelLoad.Image = null;
            toolStripStatusLabelLoad.Text = @"Завершено";
            toolStripStatusLabelURL.Text = @"URL: " + webBrowser.Document?.Url?.AbsoluteUri;

            try
            {
                if (webBrowser.Url.AbsoluteUri.Contains(_endPage) ||
                    webBrowser.Url.AbsoluteUri.Contains("http://steamcommunity.com/profiles/"))
                {
                    if (_endPage == "http://steamcommunity.com/id/")
                    {
                        SteamAuth();
                    }
                }

                if (webBrowser.Url.AbsoluteUri == _endPage ||
                    webBrowser.Url.AbsoluteUri == "https://www.steamgifts.com/register")
                {
                    if (_endPage.Contains("http://gameminer.net/?lang="))
                    {
                        GameMinerAuth();
                    }

                    if (_endPage == "https://www.steamgifts.com/")
                    {
                        SteamGiftsAuth();
                    }

                    if (_endPage == "https://steamcompanion.com/")
                    {
                        SteamCompanionAuth();
                    }

                    if (_endPage == "http://usegamble.com/")
                    {
                        UseGamblelAuth();
                    }

                    if (_endPage == "http://steamtrade.info/")
                    {
                        SteamTradeAuth();
                    }

                    if (_endPage == "http://playblink.com/")
                    {
                        PlayBlinkAuth();
                    }
                }
            }
            catch (ObjectDisposedException)
            {
            }
        }

        private void SteamAuth()
        {
            var container = GetUriCookieContainer(webBrowser.Url);
            var cookies = container.GetCookies(webBrowser.Url);
            foreach (Cookie cookie in cookies)
            {
                if (cookie.Name == "sessionid")
                {
                    _bot.Steam.Cookies.Sessid = cookie.Value;
                }

                if (cookie.Name == "steamLogin")
                {
                    _bot.Steam.Cookies.Login = cookie.Value;
                }

                if (cookie.Name == "steamRememberLogin")
                {
                    _bot.Steam.Cookies.RememberLogin = cookie.Value;
                }
            }
            _bot.Steam.Enabled = true;
            webBrowser.Dispose();
            Close();
        }

        private void GameMinerAuth()
        {
            var container = GetUriCookieContainer(webBrowser.Url);
            var cookies = container.GetCookies(webBrowser.Url);
            foreach (Cookie cookie in cookies)
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
            webBrowser.Dispose();
            Close();
        }

        private void SteamGiftsAuth()
        {
            var container = GetUriCookieContainer(webBrowser.Url);
            var cookies = container.GetCookies(webBrowser.Url);
            foreach (Cookie cookie in cookies)
            {
                if (cookie.Name == "PHPSESSID")
                {
                    _bot.SteamGifts.Cookies.PhpSessId = cookie.Value;
                }
            }
            _bot.SteamGifts.Enabled = true;
            webBrowser.Dispose();
            Close();
        }

        private void SteamCompanionAuth()
        {
            var container = GetUriCookieContainer(webBrowser.Url);
            var cookies = container.GetCookies(webBrowser.Url);
            foreach (Cookie cookie in cookies)
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
            webBrowser.Dispose();
            Close();
        }

        private void UseGamblelAuth()
        {
            var container = GetUriCookieContainer(webBrowser.Url);
            var cookies = container.GetCookies(webBrowser.Url);
            foreach (Cookie cookie in cookies)
            {
                if (cookie.Name == "PHPSESSID")
                {
                    _bot.UseGamble.Cookies.PhpSessId = cookie.Value;
                }
            }
            _bot.UseGamble.Enabled = true;
            webBrowser.Dispose();
            Close();
        }

        private void SteamTradeAuth()
        {
            var container = GetUriCookieContainer(webBrowser.Url);
            var cookies = container.GetCookies(webBrowser.Url);
            foreach (Cookie cookie in cookies)
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
            webBrowser.Dispose();
            Close();
        }

        private void PlayBlinkAuth()
        {
            if (webBrowser.DocumentText != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(webBrowser.DocumentText);

                var node = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='usr_link']");
                if (node != null)
                {
                    var container = GetUriCookieContainer(webBrowser.Url);
                    var cookies = container.GetCookies(webBrowser.Url);
                    foreach (Cookie cookie in cookies)
                    {
                        if (cookie.Name == "PHPSESSID")
                        {
                            _bot.PlayBlink.Cookies.PhpSessId = cookie.Value;
                        }
                    }
                    _bot.PlayBlink.Enabled = true;
                    webBrowser.Dispose();
                    Close();
                }
            }
        }

        private static CookieContainer GetUriCookieContainer(Uri uri)
        {
            // First, create a null cookie container
            CookieContainer cookies = null;

            // Determine the size of the cookie
            var datasize = 8192*16;
            var cookieData = new StringBuilder(datasize);

            // Call InternetGetCookieEx from wininet.dll
            if (
                !InternetGetCookieEx(uri.ToString(), null, cookieData, ref datasize, InternetCookieHttponly, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;
                // Allocate stringbuilder large enough to hold the cookie
                cookieData = new StringBuilder(datasize);
                if (!InternetGetCookieEx(
                    uri.ToString(),
                    null, cookieData,
                    ref datasize,
                    InternetCookieHttponly,
                    IntPtr.Zero))
                    return null;
            }

            // If the cookie contains data, add it to the cookie container
            if (cookieData.Length > 0)
            {
                cookies = new CookieContainer();
                cookies.SetCookies(uri, cookieData.ToString().Replace(';', ','));
            }

            // Return the cookie container
            return cookies;
        }

        private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            toolStripStatusLabelLoad.Image = Resources.load;
            toolStripStatusLabelLoad.Text = @"Загрузка...";
            toolStripStatusLabelURL.Text = @"URL: " + webBrowser.Url?.AbsoluteUri;
        }

        private void webBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            toolStripStatusLabelLoad.Image = null;
            toolStripStatusLabelURL.Text = @"URL: " + webBrowser.Url?.AbsoluteUri;
        }
    }
}