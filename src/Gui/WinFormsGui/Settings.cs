using System.Diagnostics.CodeAnalysis;
using KryBot.Core;
using KryBot.Core.Helpers;

namespace KryBot.Gui.WinFormsGui
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	public class Settings
	{
		// НЕ ДЕЛАТЬ ПОЛЯ ПРИВАТНЫМИ
		public bool IsLogActive { get; set; }

		public string Lang { get; set; }

		public bool Sort { get; set; }

		public bool SortToMore { get; set; }

		public bool SortToLess { get; set; }

		public bool Timer { get; set; }

		public bool Autorun { get; set; }

		public bool ShowWonTip { get; set; }

		public bool ShowFarmTip { get; set; }

		public bool WishlistSort { get; set; }

		public int TimerInterval { get; set; }

		public int TimerLoops { get; set; }
		// НЕ ДЕЛАТЬ ПОЛЯ ПРИВАТНЫМИ

		public void Load()
		{
			var settings = FileHelper.SafelyLoad<Settings>(FilePaths.Settings);

			Properties.Settings.Default.Autorun = settings.Autorun;
			Properties.Settings.Default.Sort = settings.Sort;
			Properties.Settings.Default.SortToLess = settings.SortToLess;
			Properties.Settings.Default.SortToMore = settings.SortToMore;
			Properties.Settings.Default.Timer = settings.Timer;
			Properties.Settings.Default.TimerInterval = settings.TimerInterval;
			Properties.Settings.Default.TimerLoops = settings.TimerLoops;
			Properties.Settings.Default.LogActive = settings.IsLogActive;
			Properties.Settings.Default.WishlistNotSort = settings.WishlistSort;
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
				Timer = Properties.Settings.Default.Timer,
				TimerInterval = Properties.Settings.Default.TimerInterval,
				TimerLoops = Properties.Settings.Default.TimerLoops,
				SortToLess = Properties.Settings.Default.SortToLess,
				SortToMore = Properties.Settings.Default.SortToMore,
				Sort = Properties.Settings.Default.Sort,
				ShowFarmTip = Properties.Settings.Default.ShowFarmTip,
				ShowWonTip = Properties.Settings.Default.ShowWonTip,
				Autorun = Properties.Settings.Default.Autorun,
				WishlistSort = Properties.Settings.Default.WishlistNotSort
			};

			FileHelper.Save(settings, FilePaths.Settings);
		}
	}
}