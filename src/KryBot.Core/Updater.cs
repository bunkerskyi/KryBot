using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using KryBot.CommonResources.Localization;
using KryBot.Core.Helpers;
using KryBot.Core.Serializable.GitHub;
using Newtonsoft.Json;

namespace KryBot.Core
{
    public static class Updater
    {
        /// <summary>
        ///     Check application updates from GitHub repo.
        /// </summary>
        public static async Task<Log> CheckForUpdates()
        {
            GitHubRelease release;
            try
            {
                release = await GetGitHubRelease();
            }
            catch (JsonReaderException)
            {
                return new Log("Updater: JsonReaderException", Color.Red, false);
            }

            if (release.Tag != null &&
                CompareVersion(Application.ProductVersion, release.Tag))
            {
                return new Log($"{strings.Updater_HaveUpdate} {release.Tag}", Color.Green, true);
            }
            return new Log($"{strings.Updater_CurrentVersion} {release.Tag}", Color.White, false);
        }

        /// <summary>
        ///     Update application.
        /// </summary>
        public static async Task<Log> Update()
        {
            try
            {
                GitHunReleaseAssets archive = null;
                GitHunReleaseAssets singleFile = null;

                DeleteTempFiles();
                var release = await GetGitHubRelease();

                foreach (var asset in release.Assets)
                {
                    if (asset.Name == FilePaths.KryBot)
                    {
                        archive = asset;
                    }
                    else if(asset.Name == FilePaths.KryBotArchive)
                    {
                        singleFile = asset;
                    }
                }   

                if (archive == null && singleFile == null)
                {
                    return new Log(strings.Updater_Update_UpdateFailed, Color.Red, false);
                }

                if (singleFile != null)
                {
                    return await UpdatefromFile(singleFile);
                }
                else
                {
                    return await UpdateFromArchive(archive);
                }
            }
            catch (JsonReaderException)
            {
                return new Log(strings.Updater_Update_UpdateFailed, Color.Red, false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                DeleteTempFiles();
                return new Log(strings.Updater_Update_UpdateFailed, Color.Red, false);
            }
        }

        private static async Task<Log> UpdatefromFile(GitHunReleaseAssets asset)
        {
            var stream = new WebClient().OpenRead(asset.DownloadUrl);
            if (stream != null)
            {
                using (var fileStream = File.Open(FilePaths.KryBotNew, FileMode.Create))
                {
                    await stream.CopyToAsync(fileStream);
                }

                File.Move(FilePaths.KryBot, FilePaths.KryBotOld);

                try
                {
                    File.Move(FilePaths.KryBotNew, FilePaths.KryBot);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    File.Move(FilePaths.KryBotOld, FilePaths.KryBot);
                    DeleteTempFiles();
                    return new Log(strings.Updater_Update_UpdateFailed, Color.Red, false);
                }
                return new Log(strings.Updater_Update_UpdateDone, Color.Green, true);
            }
            return new Log(strings.Updater_Update_UpdateFailed, Color.Red, true);
        }

        private static async Task<Log> UpdateFromArchive(GitHunReleaseAssets asset)
        {
            List<string> filesToMove = new List<string>();
            var stream = new WebClient().OpenRead(asset.DownloadUrl);
            if (stream != null)
            {
                using (var fileStream = File.Open(FilePaths.KryBotArchive, FileMode.Create))
                {
                    await stream.CopyToAsync(fileStream);
                }

                Debug.WriteLine("Exstract strat");
                foreach (var entry in ZipFile.OpenRead(FilePaths.KryBotArchive).Entries)
                {
                    if (File.Exists(entry.FullName))
                    {
                        Debug.WriteLine($"Exstract: {entry.FullName}.new");
                        entry.ExtractToFile($"{entry.FullName}.new");
                        Debug.WriteLine($"Move: {entry.FullName} to {entry.FullName}.old");
                        File.Move(entry.FullName, $"{entry.FullName}.old");
                        filesToMove.Add(entry.FullName);
                    }
                    else
                    {
                        Debug.WriteLine($"Exstract: {entry.FullName}");
                        entry.ExtractToFile(entry.FullName);
                    }
                }
                Debug.WriteLine("Exstract end");

                Debug.WriteLine("Move start");
                if (filesToMove.Count > 0)
                {
                    foreach (var file in filesToMove)
                    {
                        Debug.WriteLine($"Move: {file}.new to {file}");
                        File.Move($"{file}.new", file); 
                    }
                }
                Debug.WriteLine("Move end");
                return new Log(strings.Updater_Update_UpdateDone, Color.Green, true);

            }
            return new Log(strings.Updater_Update_UpdateFailed, Color.Red, true);
        }

        /// <summary>
        ///     Compare two strings as verions.
        /// </summary>
        private static bool CompareVersion(string sClient, string sServer)
        {
            var vClient = new Version(sClient);
            var vServer = new Version(sServer);
            var compare = vClient.CompareTo(vServer);

            return compare == -1;
        }

        private static void DeleteTempFiles()
        {
            foreach (var directory in Directory.GetDirectories(Environment.CurrentDirectory))
            {
                foreach (var file in Directory.GetFiles(directory))
                {
                    if (file.Contains(".new") || file.Contains(".old"))
                    {
                        FileHelper.SafelyDelete(file);
                    }
                }

                foreach (var file in Directory.GetFiles(Environment.CurrentDirectory))
                {
                    if (file.Contains(".new") || file.Contains(".old"))
                    {
                        FileHelper.SafelyDelete(file);
                    }
                }
            }
        }

        private static async Task<GitHubRelease> GetGitHubRelease()
        {
            var json = await Web.GetVersionInGitHubAsync(Links.GitHubLatestReliase);
            return JsonConvert.DeserializeObject<GitHubRelease>(json);
        }
    }
}