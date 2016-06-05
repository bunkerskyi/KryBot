using System.Collections.Generic;
using KryBot.Functional.Giveaways;

namespace KryBot.Functional.Sites
{
	public class SteamPortal
	{
		public SteamPortal()
		{
			Cookies = new SpCookies();
			Giveaways = new List<SteamPortalGiveaway>();
		}

		public bool Enabled { get; set; }
		public string ProfileLink { get; set; }
		public int Points { get; set; }
		public int MaxJoinValue { get; set; } = 30;
		public int PointsReserv { get; set; }
		public SpCookies Cookies { get; set; }
		public List<SteamPortalGiveaway> Giveaways { get; set; }

		public class SpCookies
		{
			public string PhpSessId { get; set; }
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

		public void Logout()
		{
			Cookies = new SpCookies();
			Enabled = false;
		}
	}
}