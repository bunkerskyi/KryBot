using System.Linq;
using System.Net;
using KryBot.Core.Helpers;

namespace KryBot.Core
{
	public static class Tools
	{
		public static Bot LoadProfile()
		{
			var bot = new Bot();

			FileHelper.Load(ref bot, FilePaths.Profile);

			return bot;
		}

		public static string GetSessCookieInresponse(CookieContainer cookies, string domain, string cookieName)
		{
			if (cookies?.Count > 0)
			{
				var list = CookieHelper.CookieContainer_ToList(cookies);

				return
					(from cookie in list where cookie.Name == cookieName && cookie.Domain == domain select cookie.Value)
						.FirstOrDefault();
			}
			return null;
		}

		//	myShortcut.WorkingDirectory = MediaTypeNames.Application.StartupPath;
		//	myShortcut.IconLocation = shortcutTarget + ",0";
		//	myShortcut.TargetPath = shortcutTarget;
		//		(WshShortcut) myShell.CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
		//	WshShortcut myShortcut =
		//	WshShell myShell = new WshShell();
		//	string shortcutTarget = Path.Combine(MediaTypeNames.Application.StartupPath, "KryBot.exe");
		//{
		//public static void CreateShortcut()

		// TODO Перенести
		//	myShortcut.Save();
		//}
	}
}