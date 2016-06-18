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
		// НЕ ДЕЛАТЬ ПОЛЯ ПРИВАТНЫМИ

		public void Load()
		{
			var settings = FileHelper.SafelyLoad<Settings>(FilePaths.Settings);

			Properties.Settings.Default.Autorun = settings.Autorun;
			Properties.Settings.Default.LogActive = settings.IsLogActive;
			Properties.Settings.Default.Lang = settings.Lang;
			Properties.Settings.Default.ShowFarmTip = settings.ShowFarmTip;
			Properties.Settings.Default.ShowWonTip = settings.ShowWonTip;
			Properties.Settings.Default.Save();
		}

		public void Save()
		{
			var settings = new Settings
			{
				Lang = Properties.Settings.Default.Lang,
				IsLogActive = Properties.Settings.Default.LogActive,
				ShowFarmTip = Properties.Settings.Default.ShowFarmTip,
				ShowWonTip = Properties.Settings.Default.ShowWonTip,
				Autorun = Properties.Settings.Default.Autorun
			};

			FileHelper.Save(settings, FilePaths.Settings);
		}
	}
}