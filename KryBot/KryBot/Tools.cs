using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Xml.Serialization;
using KryBot.lang;
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

        public static void CheckDlls()
        {
            if (!File.Exists(Application.StartupPath + "//HtmlAgilityPack.dll"))
            {
                CopyResource("KryBot.Resources.HtmlAgilityPack.dll", Application.StartupPath + "/HtmlAgilityPack.dll");
            }
            if (!File.Exists(Application.StartupPath + "//Newtonsoft.Json.dll"))
            {
                CopyResource("KryBot.Resources.Newtonsoft.Json.dll", Application.StartupPath + "/Newtonsoft.Json.dll");
            }
            if (!File.Exists(Application.StartupPath + "//RestSharp.dll"))
            {
                CopyResource("KryBot.Resources.RestSharp.dll", Application.StartupPath + "/RestSharp.dll");
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
            //var js =
            //    @"<script type='text/javascript'>function getUserAgent(){document.write(navigator.userAgent)}</script>";

            //var wb = new WebBrowser {Url = new Uri("about:blank")};
            //if (wb.Document != null)
            //{
            //    wb.Document.Write(js);
            //    wb.Document.InvokeScript("getUserAgent");
            //}

            //return wb.DocumentText.Substring(js.Length);
            return "User-Agent: Mozilla/5.0 (Windows NT 6.2; WOW64; Trident/7.0; rv:11.0) like Gecko";
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

                return (from cookie in list where cookie.Name == cookieName && cookie.Domain == domain select cookie.Value)
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

        public static bool VersionCompare(string sClient, string sServer)
        {
            Version vClient = new Version(sClient);
            Version vServer = new Version(sServer);
            var compare = vClient.CompareTo(vServer);

            if (compare == -1)
            {
                return true;
            }
            return false;
        }

        public static bool SaveSettings()
        {
            Classes.Settings sets = new Classes.Settings
            {
                Lang = Properties.Settings.Default.Lang,
                LogActive = Properties.Settings.Default.LogActive,
                FullLog = Properties.Settings.Default.FullLog,
                Timer = Properties.Settings.Default.Timer,
                TimerInterval = Properties.Settings.Default.TimerInterval,
                TimerLoops = Properties.Settings.Default.TimerLoops,
                SortToLess = Properties.Settings.Default.SortToLess,
                SortToMore = Properties.Settings.Default.SortToMore,
                Sort = Properties.Settings.Default.Sort,
                ShowFarmTip = Properties.Settings.Default.ShowFarmTip,
                ShowWonTip = Properties.Settings.Default.ShowWonTip,
                Autorun = Properties.Settings.Default.Autorun,
                WishlistSort = Properties.Settings.Default.WishlistNotSort
            };

            try
            {
                using (var fs = new FileStream("settings.xml", FileMode.Create, FileAccess.Write))
                {
                    var serializer = new XmlSerializer(typeof(Classes.Settings));
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
                var serializer = new XmlSerializer(typeof(Classes.Settings));
                var reader = new StreamReader("settings.xml");
                var sets = (Classes.Settings)serializer.Deserialize(reader);
                reader.Close();

                Properties.Settings.Default.Autorun = sets.Autorun;
                Properties.Settings.Default.Sort = sets.Sort;
                Properties.Settings.Default.SortToLess = sets.SortToLess;
                Properties.Settings.Default.SortToMore = sets.SortToMore;
                Properties.Settings.Default.Timer = sets.Timer;
                Properties.Settings.Default.TimerInterval = sets.TimerInterval;
                Properties.Settings.Default.TimerLoops = sets.TimerLoops;
                Properties.Settings.Default.LogActive = sets.LogActive;
                Properties.Settings.Default.FullLog = sets.FullLog;
                Properties.Settings.Default.WishlistNotSort = sets.WishlistSort;
                Properties.Settings.Default.Lang = sets.Lang;
                Properties.Settings.Default.ShowFarmTip = sets.ShowFarmTip;
                Properties.Settings.Default.ShowWonTip = sets.ShowWonTip;
                Properties.Settings.Default.Save();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string[] LoadBlackList()
        {
            if (File.Exists("blacklist.txt"))
            {
                try
                {
                    return File.ReadAllLines("blacklist.txt");
                            
                }
                catch (IOException ex)
                {
                    MessageBox.Show(strings.Error, ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            return null;
        }

        public static bool CheckMd5(string localPath, string embeddedPath)
        {
            byte[] localFile;
            byte[] embeddedFile;

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(localPath))
                {
                    localFile = md5.ComputeHash(stream);
                }

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedPath))
                {
                    embeddedFile = md5.ComputeHash(stream);
                }
            }

            if (localFile == embeddedFile)
            {
                return true;
            }
            return false;
        }
    }
}