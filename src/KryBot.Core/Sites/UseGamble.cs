using System.Collections.Generic;
using KryBot.Core.Cookies;
using KryBot.Core.Giveaways;

namespace KryBot.Core.Sites
{
	public class UseGamble
	{
		public UseGamble()
		{
			Cookies = new UseGambleCookie();
			Giveaways = new List<UseGambleGiveaway>();
		}

		public bool Enabled { get; set; }
		public string ProfileLink { get; set; }
		public int Points { get; set; }
		public int MaxJoinValue { get; set; } = 30;
		public int PointsReserv { get; set; }
		public UseGambleCookie Cookies { get; set; }
		public List<UseGambleGiveaway> Giveaways { get; set; }

		public void Logout()
		{
			Cookies = new UseGambleCookie();
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