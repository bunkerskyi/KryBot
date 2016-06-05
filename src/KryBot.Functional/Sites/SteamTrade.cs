using System.Collections.Generic;
using KryBot.Functional.Cookies;
using KryBot.Functional.Giveaways;

namespace KryBot.Functional.Sites
{
	public class SteamTrade
	{
		public SteamTrade()
		{
			Cookies = new SteamTradeCookie();
			Giveaways = new List<SteamTradeGiveaway>();
		}

		public bool Enabled { get; set; }
		public SteamTradeCookie Cookies { get; set; }
		public List<SteamTradeGiveaway> Giveaways { get; set; }

		public void Logout()
		{
			Cookies = new SteamTradeCookie();
			Enabled = false;
		}
	}
}