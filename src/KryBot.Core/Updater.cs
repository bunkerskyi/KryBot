/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
*/
using System;
using System.Drawing;
using System.IO;
using System.Linq;
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
                DeleteTempFiles();
                var release = await GetGitHubRelease();
                var binaryAsset = release.Assets.FirstOrDefault(asset => asset.Name == FilePaths.KryBotArchive);
                if (binaryAsset == null)
                {
                    return new Log(strings.Updater_Update_UpdateFailed, Color.Red, false);
                }
                var stream = new WebClient().OpenRead(binaryAsset.DownloadUrl);
                if (stream == null)
                {
                    return new Log(strings.Updater_Update_UpdateFailed, Color.Red, false);
                }
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

            return new Log(strings.Updater_Update_UpdateDone, Color.Green, true);
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
            FileHelper.SafelyDelete(FilePaths.KryBotOld);
            FileHelper.SafelyDelete(FilePaths.KryBotNew);
        }

        private static async Task<GitHubRelease> GetGitHubRelease()
        {
            var json = await Web.GetVersionInGitHubAsync(Links.GitHubLatestReliase);
            return JsonConvert.DeserializeObject<GitHubRelease>(json);
        }
    }
}