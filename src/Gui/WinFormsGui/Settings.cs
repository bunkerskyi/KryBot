using KryBot.Core;
using KryBot.Core.Helpers;

namespace KryBot.Gui.WinFormsGui
{
	public class Settings
	{
		// НЕ ДЕЛАТЬ ПОЛЯ ПРИВАТНЫМИ
		public bool IsLogActive { get; set; }
		public string Lang { get; set; }
		public bool Autorun { get; set; }
		public bool ShowWonTip { get; set; }
		public bool ShowFarmTip { get; set; }
		public int LogHeight { get; set; } = 247;
		public int LogWidth { get; set; } = 500;
		// НЕ ДЕЛАТЬ ПОЛЯ ПРИВАТНЫМИ

		public static Settings Load()
		{
			return FileHelper.SafelyLoad<Settings>(FilePaths.Settings);
		}

		public void Save()
		{
			FileHelper.Save(this, FilePaths.Settings);
		}
	}
}