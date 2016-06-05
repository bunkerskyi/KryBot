using System.Collections.Generic;
using KryBot.Functional.Giveaways;

namespace KryBot.Functional.Sites
{
	public class SteamTrade
	{
		public SteamTrade()
		{
			Cookies = new StCookies();
			Giveaways = new List<SteamTradeGiveaway>();
		}

		public bool Enabled { get; set; }
		public StCookies Cookies { get; set; }
		public List<SteamTradeGiveaway> Giveaways { get; set; }

		public class StCookies
		{
			public string PhpSessId { get; set; }
			public string DleUserId { get; set; }
			public string DlePassword { get; set; }
			public string PassHash { get; set; }
		}

		public void Logout()
		{
			Cookies = new StCookies();
			Enabled = false;
		}
	}
}