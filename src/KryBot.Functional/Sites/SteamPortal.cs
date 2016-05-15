using System.Collections.Generic;

namespace KryBot.Functional.Sites
{
	public class SteamPortal
	{
		public SteamPortal()
		{
			Cookies = new SpCookies();
			Giveaways = new List<SpGiveaway>();
		}

		public bool Enabled { get; set; }
		public string ProfileLink { get; set; }
		public int Points { get; set; }
		public int MaxJoinValue { get; set; } = 30;
		public int PointsReserv { get; set; }
		public SpCookies Cookies { get; set; }
		public List<SpGiveaway> Giveaways { get; set; }

		public class SpCookies
		{
			public string PhpSessId { get; set; }
		}

		public class SpGiveaway
		{
			public string Name { get; set; }
			public string StoreId { get; set; }
			public int Price { get; set; }
			public string Code { get; set; }
			public string Region { get; set; }
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