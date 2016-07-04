using System;
using System.Collections.Generic;
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

                DeleteTempFiles();
                var release = await GetGitHubRelease();

                foreach (var asset in release.Assets)
                {
                    if(asset.Name == FilePaths.KryBotArchive)
                    {
                        archive = asset;
                    }
                }   

                if (archive == null)
                {
                    return new Log(strings.Updater_Update_UpdateFailed, Color.Red, false);
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

                foreach (var entry in ZipFile.OpenRead(FilePaths.KryBotArchive).Entries)
                {
                    if (File.Exists(entry.FullName))
                    {
                        entry.ExtractToFile($"{entry.FullName}.new");
                        File.Move(entry.FullName, $"{entry.FullName}.old");
                        filesToMove.Add(entry.FullName);
                    }
                    else
                    {
                        entry.ExtractToFile(entry.FullName);
                    }
                }

                if (filesToMove.Count > 0)
                {
                    foreach (var file in filesToMove)
                    {
                        File.Move($"{file}.new", file); 
                    }
                }

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