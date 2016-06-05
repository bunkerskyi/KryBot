using System.Collections.Generic;
using KryBot.Functional.Cookies;
using KryBot.Functional.Giveaways;

namespace KryBot.Functional.Sites
{
	public class PlayBlink
	{
		public PlayBlink()
		{
			Cookies = new PlayBlinkCookie();
			Giveaways = new List<PlayBlinkGiveaway>();
		}

		public bool Enabled { get; set; }
		public int Points { get; set; }
		public int Level { get; set; }
		public int MaxJoinValue { get; set; } = 50;
		public int PointReserv { get; set; } = 0;
		public PlayBlinkCookie Cookies { get; set; }
		public List<PlayBlinkGiveaway> Giveaways { get; set; }

		public void Logout()
		{
			Cookies = new PlayBlinkCookie();
			Enabled = false;
		}
	}
}