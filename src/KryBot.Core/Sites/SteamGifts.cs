using System.Collections.Generic;
using KryBot.Core.Cookies;
using KryBot.Core.Giveaways;

namespace KryBot.Core.Sites
{
	public class SteamGifts
	{
		public SteamGifts()
		{
			Cookies = new SteamGiftsCookie();
			Giveaways = new List<SteamGiftsGiveaway>();
			WishlistGiveaways = new List<SteamGiftsGiveaway>();
		}

		public bool Enabled { get; set; }
		public bool Group { get; set; }
		public bool Regular { get; set; } = true;
		public bool WishList { get; set; }
		public bool SortLevel { get; set; }
		public bool SortToLessLevel { get; set; }
		public int Points { get; set; }
		public int Level { get; set; }
		public int JoinPointLimit { get; set; } = 300;
		public int PointsReserv { get; set; }
		public int MinLevel { get; set; }
		public SteamGiftsCookie Cookies { get; set; }
		public List<SteamGiftsGiveaway> Giveaways { get; set; }
		public List<SteamGiftsGiveaway> WishlistGiveaways { get; set; }

		public void Logout()
		{
			Cookies = new SteamGiftsCookie();
			Enabled = false;
		}

		public class JsonResponseJoin
		{
			public string Type { get; set; }
			public string EntryCount { get; set; }
			public int Points { get; set; }
		}

		public class JsonResponseSyncAccount
		{
			public string Type { get; set; }
			public string Msg { get; set; }
		}
	}
}