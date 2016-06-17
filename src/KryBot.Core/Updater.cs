using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Exceptionless.Json;
using KryBot.CommonResources.lang;
using KryBot.Core.Helpers;
using KryBot.Core.Serializable.GitHub;

namespace KryBot.Core
{
	public static class Updater
	{
		/// <summary>
		/// Check application updates from GitHub repo. 
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

			if (release.tag_name != null &&
				CompareVersion(Application.ProductVersion, release.tag_name))
			{
				return new Log($"Доступно обновление {release.tag_name}", Color.Green, true);
			}
			return new Log($"Актуальная версия {release.tag_name}", Color.White, false);
		}

		/// <summary>
		/// Update application.
		/// </summary>
		public static async Task<Log> Update()
		{
			try
			{
				DeleteTempFiles();
				var release = await GetGitHubRelease();
				var binaryAsset = release.assets.FirstOrDefault(asset => asset.name == FilePaths.KryBot);
				if (binaryAsset == null)
				{
					return new Log(strings.Updater_Update_UpdateFailed, Color.Red, false);
				}
				var stream = new WebClient().OpenRead(binaryAsset.browser_download_url);
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
		/// Compare two strings as verions.
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