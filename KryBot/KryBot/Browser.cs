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
        private readonly Classes.Bot _bot;
        private readonly CookieContainer _cookies;
        private readonly string _endPage;
        private readonly string _phpSessId;
        private readonly string _startPage;
        private readonly string _title;

        public Browser(Classes.Bot bot, string startPage, string endPage, string title, string phpSessId,
            CookieContainer cookies)
        {
            _bot = bot;
            _startPage = startPage;
            _endPage = endPage;
            _title = title;
            _phpSessId = phpSessId;
            _cookies = cookies;
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

            if (_cookies != null && _cookies.Count > 0)
            {
                //var cookies = Tools.CookieContainer_ToList(_cookies);
                //foreach (var cookie in cookies)
                //{
                //    InternetSetCookie("http://steamcommunity.com/", cookie.Name, cookie.Value);
                //}
            }

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
                    webBrowser.Url.AbsoluteUri == "http://www.steamgifts.com/register")
                {
                    if (_endPage == "http://gameminer.net/?lang=ru_RU" || _endPage == "http://gameminer.net/?lang=en_US")
                    {
                        GameMinerAuth();
                    }

                    if (_endPage == "http://www.steamgifts.com/")
                    {
                        SteamGiftsAuth();
                    }

                    if (_endPage == "https://steamcompanion.com/")
                    {
                        SteamCompanionAuth();
                    }

                    if (_endPage == "http://steamportal.net/")
                    {
                        SteamPortalAuth();
                    }

                    if (_endPage == "http://www.gameaways.com/")
                    {
                        GameAwaysAuth();
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
            catch (ObjectDisposedException){}
        }

        private void SteamAuth()
        {
            var container = GetUriCookieContainer(webBrowser.Url);
            var cookies = container.GetCookies(webBrowser.Url);
            foreach (Cookie cookie in cookies)
            {
                if (cookie.Name == "sessionid")
                {
                    _bot.SteamSessid = cookie.Value;
                }

                if (cookie.Name == "steamLogin")
                {
                    _bot.SteamLogin = cookie.Value;
                }

                if (cookie.Name == "steamRememberLogin")
                {
                    _bot.SteamRememberLogin = cookie.Value;
                }
            }
            _bot.SteamEnabled = true;
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
                    _bot.GameMinerToken = cookie.Value;
                }

                if (cookie.Name == "_xsrf")
                {
                    _bot.GameMinerxsrf = cookie.Value;
                }
            }
            _bot.GameMinerEnabled = true;
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
                    _bot.SteamGiftsPhpSessId = cookie.Value;
                }
            }
            _bot.SteamGiftsEnabled = true;
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
                    _bot.SteamCompanionPhpSessId = cookie.Value;
                }

                if (cookie.Name == "userc")
                {
                    _bot.SteamCompanionUserC = cookie.Value;
                }

                if (cookie.Name == "userid")
                {
                    _bot.SteamCompanionUserId = cookie.Value;
                }

                if (cookie.Name == "usert")
                {
                    _bot.SteamCompanionUserT = cookie.Value;
                }
            }
            _bot.SteamCompanionEnabled = true;
            webBrowser.Dispose();
            Close();
        }

        private void SteamPortalAuth()
        {
            var container = GetUriCookieContainer(webBrowser.Url);
            var cookies = container.GetCookies(webBrowser.Url);
            foreach (Cookie cookie in cookies)
            {
                if (cookie.Name == "PHPSESSID")
                {
                    _bot.SteamPortalPhpSessId = cookie.Value;
                }
            }
            _bot.SteamPortalEnabled = true;
            webBrowser.Dispose();
            Close();
        }

        private void GameAwaysAuth()
        {
            var container = GetUriCookieContainer(webBrowser.Url);
            var cookies = container.GetCookies(webBrowser.Url);
            foreach (Cookie cookie in cookies)
            {
                if (cookie.Name == "ASP.NET_SessionId")
                {
                    _bot.GameAwaysPhpSessId = cookie.Value;
                }
            }
            _bot.GameAwaysEnabled = true;
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
                    _bot.SteamTradePhpSessId = cookie.Value;
                }

                if (cookie.Name == "dle_user_id")
                {
                    _bot.SteamTradeDleUserId = cookie.Value;
                }

                if (cookie.Name == "dle_password")
                {
                    _bot.SteamTradeDlePassword = cookie.Value;
                }

                if (cookie.Name == "passhash")
                {
                    _bot.SteamTradePassHash = cookie.Value;
                }
            }
            _bot.SteamTradeEnabled = true;
            webBrowser.Dispose();
            Close();
        }

        private void PlayBlinkAuth()
        {
            if (webBrowser.DocumentText != "")
            {
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(webBrowser.DocumentText);

                var node = htmlDoc.DocumentNode.SelectSingleNode("//a[@title='Your contribution level']");
                if (node != null)
                {
                    var container = GetUriCookieContainer(webBrowser.Url);
                    var cookies = container.GetCookies(webBrowser.Url);
                    foreach (Cookie cookie in cookies)
                    {
                        if (cookie.Name == "PHPSESSID")
                        {
                            _bot.PlayBlinkPhpSessId = cookie.Value;
                        }
                    }
                    _bot.PlayBlinkEnabled = true;
                    webBrowser.Dispose();
                    Close();
                }
            }
        }

        public static CookieContainer GetUriCookieContainer(Uri uri)
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