using System.Collections.Generic;
using KryBot.Core.Cookies;
using KryBot.Core.Giveaways;

namespace KryBot.Core.Sites
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