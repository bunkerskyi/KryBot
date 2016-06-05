using System.Collections.Generic;
using KryBot.Functional.Giveaways;

namespace KryBot.Functional.Sites
{
	public class PlayBlink
	{
		public PlayBlink()
		{
			Cookies = new PbCookies();
			Giveaways = new List<PlayBlinkGiveaway>();
		}

		public bool Enabled { get; set; }
		public int Points { get; set; }
		public int Level { get; set; }
		public int MaxJoinValue { get; set; } = 50;
		public int PointReserv { get; set; } = 0;
		public PbCookies Cookies { get; set; }
		public List<PlayBlinkGiveaway> Giveaways { get; set; }

		public class PbCookies
		{
			public string PhpSessId { get; set; }
		}


		public void Logout()
		{
			Cookies = new PbCookies();
			Enabled = false;
		}
	}
}