using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using KryBot_Updater.lang;
using Newtonsoft.Json;

namespace KryBot_Updater
{
    class Program
    {
        public static Classes.GitHubRelease Release = new Classes.GitHubRelease();
        public static int UpdatePosition;
        public static string Lang = "ru-RU";
        public static bool RunInTheProgram;

        static void Main(string[] args)
        {
            Console.Title = Application.ProductName + @" [" + Application.ProductVersion + @"]";

            foreach (var arg in args)
            {
                if (arg == "RunInTheProgram")
                {
                    RunInTheProgram = true;
                }

                if (arg == "ru-RU" || arg == "en-EN")
                {
                    Lang = arg;
                }
            }

            if (Lang != null || Lang != "en-EN")
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Lang);
            }

            string version = GetRemoteVersion();
            if (version != null)
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
                        GetRemoteFiles(version, Release.assets[UpdatePosition].browser_download_url);
                        if (Unzip(version))
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
                    GetRemoteFiles(version, Release.assets[UpdatePosition].browser_download_url);
                    if (Unzip(version))
                    {
                        Console.WriteLine(strings.Start + @" KryBot.exe...");
                        Process.Start("KryBot.exe");
                        Console.WriteLine(@"KryBot.exe " + strings.Started);
                    }
                }
            }
            Console.WriteLine(strings.Close);
            Console.ReadKey();
            Application.Exit();
        }

        private static string GetRemoteVersion()
        {
            try
            {
                string json = Web.Get("https://api.github.com/repos/KriBetko/KryBot/releases/latest");

                Release = JsonConvert.DeserializeObject<Classes.GitHubRelease>(json);

                if (!Release.prerelease)
                {
                    return Release.tag_name;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return null;
        }

        private static void GetRemoteFiles(string version, string url)
        {
            try
            {
                string remoteUri = url;
                string fileName = "KryBot_Portable_." + version + ".zip";
                WebClient myWebClient = new WebClient();
                var myStringWebResource = remoteUri;
                Console.WriteLine(strings.DownloadingFile + @" " + fileName + @" " + strings.From + @" " + myStringWebResource + @"...");
                myWebClient.DownloadFile(myStringWebResource, fileName);
                Console.WriteLine(strings.SuccessfullyDownloadedFile + @" " + fileName);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static bool Unzip(string version)
        {
            Console.WriteLine(strings.UnzipingFile + @" KryBot_Portable_." + version + @".zip...");
            try
            {
                string[] files = Directory.GetFiles(Environment.CurrentDirectory);
                foreach (var file in files)
                {
                    if (file == Environment.CurrentDirectory + "\\KryBot.exe")
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            MessageBox.Show(strings.Error_UpdateClosePogramNeeded, strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }    
                }
                System.IO.Compression.ZipFile.ExtractToDirectory("KryBot_Portable_." + version + ".zip", Environment.CurrentDirectory);
                File.Delete("KryBot_Portable_." + version + ".zip");
            }
            catch (NullReferenceException e)
            {
                MessageBox.Show(e.Message, strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            Console.WriteLine(strings.UnzipeComplete);
            return true;
        }
    }
}
