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