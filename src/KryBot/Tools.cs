using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using System.Windows.Forms;
using System.Xml.Serialization;
using IWshRuntimeLibrary;
using KryBot.lang;
using Microsoft.Win32;
using RestSharp;
using File = System.IO.File;

namespace KryBot
{
    public static class Tools
    {
        public static Bot LoadProfile(string path)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(Bot));
                var reader = new StreamReader(path == "" ? "profile.xml" : path);
                var bot = (Bot) serializer.Deserialize(reader);
                reader.Close();
                return bot;
            }
            catch (Exception)
            {
                return new Bot();
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

        private static List<Cookie> CookieContainer_ToList(CookieContainer container)
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

        public static Blacklist LoadBlackList()
        {
            if (File.Exists("blacklist.xml"))
            {
                try
                {
                    using (var reader = new StreamReader("blacklist.xml"))
                    {
                        var serializer = new XmlSerializer(typeof(Blacklist));
                        var blacklist = (Blacklist) serializer.Deserialize(reader);
                        return blacklist;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(@"Ошибка", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new Blacklist();
                }
            }
            return new Blacklist();
        }

        private static string GetIeVersion()
        {
            return new WebBrowser().Version.ToString();
        }

        public static bool CheckIeVersion(int minIeVersion)
        {
            return int.Parse(GetIeVersion().Split('.')[0]) < minIeVersion;
        }

        public static void CreateShortcut()
        {
            var shortcutTarget = Path.Combine(Application.StartupPath, "KryBot.exe");
            var myShell = new WshShell();
            var myShortcut =
                (WshShortcut) myShell.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            myShortcut.TargetPath = shortcutTarget;
            myShortcut.IconLocation = shortcutTarget + ",0";
            myShortcut.WorkingDirectory = Application.StartupPath;
            myShortcut.Save();
        }
    }
}