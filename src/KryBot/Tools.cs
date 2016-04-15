using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using System.Windows.Forms;
using System.Xml.Serialization;
using KryBot.lang;
using KryBot.Properties;
using Microsoft.Win32;
using RestSharp;

namespace KryBot
{
    public class Tools
    {
        public static Classes.Bot LoadProfile(string path)
        {
            try
            {
                var serializer = new XmlSerializer(typeof (Classes.Bot));
                var reader = new StreamReader(path == "" ? "profile.xml" : path);
                var bot = (Classes.Bot) serializer.Deserialize(reader);
                reader.Close();
                return bot;
            }
            catch (Exception)
            {
                var bot = new Classes.Bot();
                return bot;
            }
        }

        public static void CopyResource(string resourceName, string file)
        {
            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (resource == null)
                {
                    return;
                }
                using (Stream output = File.OpenWrite(file))
                {
                    resource.CopyTo(output);
                }
            }
        }

        public static bool SaveProfile(Classes.Bot bot, string path)
        {
            try
            {
                using (var fs = new FileStream(path == "" ? "profile.xml" : path, FileMode.Create, FileAccess.Write))
                {
                    var serializer = new XmlSerializer(typeof (Classes.Bot));
                    serializer.Serialize(fs, bot);
                }
                return true;
            }
            catch (Exception)
            {
                // ignored todo сделать оповещение о фейле
                return false;
            }
        }

        public static string UserAgent()
        {
            var js =
                @"<script type='text/javascript'>function getUserAgent(){document.write(navigator.userAgent)}</script>";

            var wb = new WebBrowser {Url = new Uri("about:blank")};
            if (wb.Document != null)
            {
                wb.Document.Write(js);
                wb.Document.InvokeScript("getUserAgent");
            }

            return wb.DocumentText.Substring(js.Length);
        }

        public static Classes.Log ConstructLog(string content, Color color, bool success, bool echo)
        {
            var log = new Classes.Log
            {
                Content = content + "\n",
                Color = color,
                Success = success,
                Echo = echo
            };
            return log;
        }

        public static string GetLocationInresponse(IRestResponse response)
        {
            return
                (from header in response.Headers where header.Name == "Location" select header.Value.ToString())
                    .FirstOrDefault();
        }

        public static string GetSessCookieInresponse(CookieContainer cookies, string domain, string cookieName)
        {
            if (cookies.Count > 0)
            {
                var list = CookieContainer_ToList(cookies);

                return
                    (from cookie in list where cookie.Name == cookieName && cookie.Domain == domain select cookie.Value)
                        .FirstOrDefault();
            }
            return null;
        }

        public static List<Cookie> CookieContainer_ToList(CookieContainer container)
        {
            var cookies = new List<Cookie>();

            var table = (Hashtable) container.GetType().InvokeMember("m_domainTable",
                BindingFlags.NonPublic |
                BindingFlags.GetField |
                BindingFlags.Instance,
                null,
                container,
                new object[] {});

            foreach (var key in table.Keys)
            {
                Uri uri;

                var domain = key as string;

                if (domain == null)
                    continue;

                if (domain.StartsWith("."))
                    domain = domain.Substring(1);

                var address = $"http://{domain}/";

                if (Uri.TryCreate(address, UriKind.RelativeOrAbsolute, out uri) == false)
                    continue;

                foreach (Cookie cookie in container.GetCookies(uri))
                {
                    if (cookies.Contains(cookie) == false)
                    {
                        cookies.Add(cookie);
                    }
                }
                return cookies;
            }
            return null;
        }

        public static bool SetAutorun()
        {
            try
            {
                var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", true);
                key?.SetValue("KryBot", Application.ExecutablePath);
                return true;
            }
            catch (SecurityException)
            {
                MessageBox.Show(
                    @"Для выполнения этого действия приложение должно быть запущено с правами администратора",
                    @"Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(
                    @"При записи в реестр произошла ошибка. Запись в реестр разрешена администратором?",
                    strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool DeleteAutorun()
        {
            try
            {
                var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", true);
                key?.DeleteValue("KryBot");
                return true;
            }
            catch (SecurityException)
            {
                MessageBox.Show(
                    @"Для выполнения этого действия приложение должно быть запущено с правами администратора",
                    @"Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(
                    @"При записи в реестр произошла ошибка. Запись в реестр разрешена администратором?",
                    strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        public static bool SaveSettings()
        {
            var sets = new Classes.Settings
            {
                Lang = Settings.Default.Lang,
                LogActive = Settings.Default.LogActive,
                FullLog = Settings.Default.FullLog,
                Timer = Settings.Default.Timer,
                TimerInterval = Settings.Default.TimerInterval,
                TimerLoops = Settings.Default.TimerLoops,
                SortToLess = Settings.Default.SortToLess,
                SortToMore = Settings.Default.SortToMore,
                Sort = Settings.Default.Sort,
                ShowFarmTip = Settings.Default.ShowFarmTip,
                ShowWonTip = Settings.Default.ShowWonTip,
                Autorun = Settings.Default.Autorun,
                WishlistSort = Settings.Default.WishlistNotSort
            };

            try
            {
                using (var fs = new FileStream("settings.xml", FileMode.Create, FileAccess.Write))
                {
                    var serializer = new XmlSerializer(typeof (Classes.Settings));
                    serializer.Serialize(fs, sets);
                }
                return true;
            }
            catch (Exception)
            {
                // ignored todo сделать оповещение о фейле
                return false;
            }
        }

        public static bool LoadSettings()
        {
            try
            {
                var serializer = new XmlSerializer(typeof (Classes.Settings));
                var reader = new StreamReader("settings.xml");
                var sets = (Classes.Settings) serializer.Deserialize(reader);
                reader.Close();

                Settings.Default.Autorun = sets.Autorun;
                Settings.Default.Sort = sets.Sort;
                Settings.Default.SortToLess = sets.SortToLess;
                Settings.Default.SortToMore = sets.SortToMore;
                Settings.Default.Timer = sets.Timer;
                Settings.Default.TimerInterval = sets.TimerInterval;
                Settings.Default.TimerLoops = sets.TimerLoops;
                Settings.Default.LogActive = sets.LogActive;
                Settings.Default.FullLog = sets.FullLog;
                Settings.Default.WishlistNotSort = sets.WishlistSort;
                Settings.Default.Lang = sets.Lang;
                Settings.Default.ShowFarmTip = sets.ShowFarmTip;
                Settings.Default.ShowWonTip = sets.ShowWonTip;
                Settings.Default.Save();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Classes.Blacklist LoadBlackList()
        {
            if (File.Exists("blacklist.xml"))
            {
                try
                {
                    using (var reader = new StreamReader("blacklist.xml"))
                    {
                        var serializer = new XmlSerializer(typeof (Classes.Blacklist));
                        var blacklist = (Classes.Blacklist) serializer.Deserialize(reader);
                        return blacklist;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(@"Ошибка", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new Classes.Blacklist();
                }
            }
            return new Classes.Blacklist();
        }
    }
}