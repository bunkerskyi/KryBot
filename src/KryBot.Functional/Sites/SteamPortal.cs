using System.Collections.Generic;
using KryBot.Functional.Cookies;
using KryBot.Functional.Giveaways;

namespace KryBot.Functional.Sites
{
	public class SteamPortal
	{
		public SteamPortal()
		{
			Cookies = new SteamPortalCookie();
			Giveaways = new List<SteamPortalGiveaway>();
		}

		public bool Enabled { get; set; }
		public string ProfileLink { get; set; }
		public int Points { get; set; }
		public int MaxJoinValue { get; set; } = 30;
		public int PointsReserv { get; set; }
		public SteamPortalCookie Cookies { get; set; }
		public List<SteamPortalGiveaway> Giveaways { get; set; }

		public void Logout()
		{
			Cookies = new SteamPortalCookie();
			Enabled = false;
		}

		public class JsonJoin
		{
			public int Error { get; set; }
			public TargetH target_h { get; set; }
		}

		public class TargetH
		{
			public int my_coins { get; set; }
		}
	}
}