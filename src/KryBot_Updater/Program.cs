using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using KryBot_Updater.lang;
using Newtonsoft.Json;

namespace KryBot_Updater
{
    internal class Program
    {
        public static Classes.GitHubRelease Release = new Classes.GitHubRelease();
        public static int UpdatePosition;
        public static string Lang = "en-EN";
        public static string RootPath;

        private static void Main(string[] args)
        {
            Console.Title = Application.ProductName + @" [" + Application.ProductVersion + @"]";

            Lang = args[1].Replace('_', '-');
            RootPath = args[2];

            if (RootPath == null)
            {
                Application.Exit();
            }

            if (Lang != null && Lang != "en-EN")
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Lang);
            }

            var version = GetRemoteVersion();
            if (version != null)
            {
                if (Release.assets.Count != 0)
                {
                    foreach (var asset in Release.assets)
                    {
                        if (asset.name.Contains("Portable"))
                        {
                            UpdatePosition = Release.assets.IndexOf(asset);
                        }
                    }

                    if (args.Length != 0)
                    {
                        Console.WriteLine(strings.LocalVersion + @": " + args[0]);
                        Console.WriteLine(strings.RemoteVersion + @": " + version);
                        if (int.Parse(args[0].Replace(".", "")) <= int.Parse(version.Replace(".", "")))
                        {
                            GetRemoteFiles(Release.assets[UpdatePosition].browser_download_url);
                            if (Unzip())
                            {
                                Console.WriteLine(strings.Start + @" KryBot.exe...");
                                Process.Start("KryBot.exe");
                                Console.WriteLine(@"KryBot.exe " + strings.Started);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(strings.RemoteVersion + @": " + version);
                        GetRemoteFiles(Release.assets[UpdatePosition].browser_download_url);
                        if (Unzip())
                        {
                            Console.WriteLine(strings.Start + @" KryBot.exe...");
                            Process.Start(RootPath + "\\KryBot.exe");
                            Console.WriteLine(@"KryBot.exe " + strings.Started);
                        }
                    }
                }
                Console.WriteLine(strings.Close);
                Application.Exit();
            }
        }

        private static string GetRemoteVersion()
        {
            var json = Web.Get("https://api.github.com/repos/KriBetko/KryBot/releases/latest");

            Release = JsonConvert.DeserializeObject<Classes.GitHubRelease>(json);

            if (!Release.prerelease)
            {
                return Release.tag_name;
            }

            return null;
        }

        private static void GetRemoteFiles(string url)
        {
            var remoteUri = url;
            var fileName = "KryBot_Portable.zip";
            var myWebClient = new WebClient();
            var myStringWebResource = remoteUri;
            Console.WriteLine(strings.DownloadingFile + @" " + fileName + @" " + strings.From + @" " +
                              myStringWebResource + @"...");
            myWebClient.DownloadFile(myStringWebResource, fileName);
            Console.WriteLine(strings.SuccessfullyDownloadedFile + @" " + fileName);
        }

        private static bool Unzip()
        {
            var filesToExtract = new List<string>();
            var filesToUpdate = Directory.GetFiles(RootPath);

            Console.WriteLine(strings.UnzipingFile + @" KryBot_Portable.zip...");

            using (var zip = ZipFile.OpenRead("KryBot_Portable.zip"))
            {
                filesToExtract.AddRange(zip.Entries.Select(e => e.Name));
            }

            foreach (var fileToUpdate in filesToUpdate)
            {
                foreach (var fileToExtract in filesToExtract)
                {
                    if (fileToUpdate == RootPath + "\\" + fileToExtract)
                    {
                        try
                        {
                            File.Delete(fileToUpdate);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            MessageBox.Show(strings.Error_UpdateClosePogramNeeded, strings.Error,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                }
            }
            ZipFile.ExtractToDirectory("KryBot_Portable.zip", RootPath);
            File.Delete("KryBot_Portable.zip");

            Console.WriteLine(strings.UnzipeComplete);
            return true;
        }
    }
}