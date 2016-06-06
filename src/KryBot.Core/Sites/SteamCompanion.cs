using System.Collections.Generic;
using KryBot.Core.Cookies;
using KryBot.Core.Giveaways;

namespace KryBot.Core.Sites
{
	public class SteamCompanion
	{
		public SteamCompanion()
		{
			Cookies = new SteamCompanionCookie();
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
		public SteamCompanionCookie Cookies { get; set; }
		public List<SteamCompanionGiveaway> Giveaways { get; set; }
		public List<SteamCompanionGiveaway> WishlistGiveaways { get; set; }

		public void Logout()
		{
			Cookies = new SteamCompanionCookie();
			Enabled = false;
		}
	}
}