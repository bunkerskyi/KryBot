using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Exceptionless.Json;
using KryBot.CommonResources.lang;
using KryBot.Core.Properties;
using KryBot.Core.Serializable.GitHub;

namespace KryBot.Core
{
	public static class Updater
	{
		public static async Task<Log> CheckForUpdates()
		{
			var json = await Web.GetVersionInGitHubAsync(Settings.Default.GitHubRepoReleaseUrl);

			if (json != "")
			{
				GitHubRelease release;
				try
				{
					release = JsonConvert.DeserializeObject<GitHubRelease>(json);
				}
				catch (JsonReaderException)
				{
					return new Log("Updater: JsonReaderException", Color.Red, false);
				}

				if (release.tag_name != null && VersionCompare(Application.ProductVersion, release.tag_name))
				{
					return new Log($"Доступно обновление {release.tag_name}", Color.Green, true);
				}
				return new Log($"Актуальная версия {release.tag_name}", Color.White, false);
			}
			return new Log("Updater: Json is Empty", Color.Red, false);
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

		public static async Task<Log> Update()
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
					return new Log(strings.Updater_Update_UpdateFailed, Color.Red, false);
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
					return new Log(strings.Updater_Update_UpdateFailed, Color.Red, false);
				}
			}

			var json = await Web.GetVersionInGitHubAsync(Settings.Default.GitHubRepoReleaseUrl);

			GitHubRelease release;
			try
			{
				release = JsonConvert.DeserializeObject<GitHubRelease>(json);
			}
			catch (JsonReaderException)
			{
				return new Log(strings.Updater_Update_UpdateFailed, Color.Red, false);
			}

			GitHunReleaseAssets binaryAsset = null;
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
				return new Log(strings.Updater_Update_UpdateFailed, Color.Red, false);
			}

			var stream = new WebClient().OpenRead(binaryAsset.browser_download_url);
			if (stream == null)
			{
				return new Log(strings.Updater_Update_UpdateFailed, Color.Red, false);
			}

			try
			{
				using (var fileStream = File.Open("KryBot.exe.new", FileMode.Create))
				{
					await stream.CopyToAsync(fileStream);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return new Log(strings.Updater_Update_UpdateFailed, Color.Red, false);
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
					File.Delete("KryBot.exe.old");
				}
				catch
				{
					// ignored
				}

				return new Log(strings.Updater_Update_UpdateFailed, Color.Red, false);
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

				return new Log(strings.Updater_Update_UpdateFailed, Color.Red, false);
			}

			return new Log(strings.Updater_Update_UpdateDone, Color.Green, true);
		}
	}
}