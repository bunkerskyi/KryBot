using System.Collections.Generic;
using KryBot.Functional.Giveaways;

namespace KryBot.Functional.Sites
{
	public class SteamCompanion
	{
		public SteamCompanion()
		{
			Cookies = new ScCookies();
			Giveaways = new List<SteamCompanionGiveaway>();
			WishlistGiveaways = new List<SteamCompanionGiveaway>();
		}

		public bool Enabled { get; set; }
		public bool Regular { get; set; } = true;
		public bool WishList { get; set; }
		public bool Contributors { get; set; }
		public bool Group { get; set; }
		public bool AutoJoin { get; set; }
		public string ProfileLink { get; set; }
		public int Points { get; set; }
		public int JoinPointLimit { get; set; } = 1500;
		public int PointsReserv { get; set; }
		public ScCookies Cookies { get; set; }
		public List<SteamCompanionGiveaway> Giveaways { get; set; }
		public List<SteamCompanionGiveaway> WishlistGiveaways { get; set; }

		public class ScCookies
		{
			public string PhpSessId { get; set; }
			public string UserId { get; set; }
			public string UserC { get; set; }
			public string UserT { get; set; }
		}

		public void Logout()
		{
			Cookies = new ScCookies();
			Enabled = false;
		}
	}
}