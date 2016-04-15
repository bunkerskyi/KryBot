using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using KryBot.lang;
using KryBot.Properties;
using Newtonsoft.Json;

namespace KryBot
{
    class Updater
    {
        public static async Task<Classes.Log> CheckForUpdates()
        {
            var json = await Web.GetVersionInGitHubAsync(Settings.Default.GitHubRepoReleaseUrl);

            if (json != "")
            {
                Classes.GitHubRelease release;
                try
                {
                    release = JsonConvert.DeserializeObject<Classes.GitHubRelease>(json);
                }
                catch (JsonReaderException)
                {
                    return Tools.ConstructLog("", Color.Red, false, true);
                }

                if (release.tag_name != null && VersionCompare(Application.ProductVersion, release.tag_name))
                {
                    return Tools.ConstructLog($"Доступно обновление {release.tag_name}", Color.Green, true, true);
                }
            }
            return Tools.ConstructLog("", Color.Red, false, true);
        }

        private static bool VersionCompare(string sClient, string sServer)
        {
            var vClient = new Version(sClient);
            var vServer = new Version(sServer);
            var compare = vClient.CompareTo(vServer);

            if (compare == -1)
            {
                return true;
            }
            return false;
        }

        public static async Task<Classes.Log> Update()
        {
            if (File.Exists("KryBot.exe.old"))
            {
                try
                {
                    File.Delete("KryBot.exe.old");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return Tools.ConstructLog(strings.Updater_Update_UpdateFailed, Color.Red, false, true);
                }
            }

            if (File.Exists("KryBot.exe.new"))
            {
                try
                {
                    File.Delete("KryBot.exe.new");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return Tools.ConstructLog(strings.Updater_Update_UpdateFailed, Color.Red, false, true);
                }
            }

            var json = await Web.GetVersionInGitHubAsync(Settings.Default.GitHubRepoReleaseUrl);

            Classes.GitHubRelease release;
            try
            {
                release = JsonConvert.DeserializeObject<Classes.GitHubRelease>(json);
            }
            catch (JsonReaderException)
            {
                return Tools.ConstructLog(strings.Updater_Update_UpdateFailed, Color.Red, false, true);
            }

            Classes.GitHunReleaseAssets binaryAsset = null;
            foreach (var asset in release.assets)
            {                                                       
                if (asset.name == "KryBot.exe")
                {
                    binaryAsset = asset;
                    break;
                }
            }

            if (binaryAsset == null)
            {
                return Tools.ConstructLog(strings.Updater_Update_UpdateFailed, Color.Red, false, true);
            }

            var stream = new WebClient().OpenRead(binaryAsset.browser_download_url);
            if (stream == null)
            {
                return Tools.ConstructLog(strings.Updater_Update_UpdateFailed, Color.Red, false, true);
            }

            try
            {
                using (FileStream fileStream = File.Open("KryBot.exe.new", FileMode.Create))
                {
                    await stream.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return Tools.ConstructLog(strings.Updater_Update_UpdateFailed, Color.Red, false, true);
            }

            try
            {
                File.Move("KryBot.exe", "KryBot.exe.old");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);

                try
                {
                    File.Delete("KryBot.exe.new");
                }
                catch
                {
                    // ignored
                }

                return Tools.ConstructLog(strings.Updater_Update_UpdateFailed, Color.Red, false, true);
            }

            try
            {
                File.Move("KryBot.exe.new", "KryBot.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);

                try
                {
                    File.Move("KryBot.exe.old", "KryBot.exe");
                    File.Delete("KryBot.exe.new");
                    File.Delete("KryBot.exe.old");
                }
                catch
                {
                    // ignored
                }

                return Tools.ConstructLog(strings.Updater_Update_UpdateFailed, Color.Red, false, true);
            }

            return Tools.ConstructLog(strings.Updater_Update_UpdateDone, Color.Green, true, true);
        }
    }
}
