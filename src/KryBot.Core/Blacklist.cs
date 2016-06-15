using System.Collections.Generic;

namespace KryBot.Core
{
	public class Blacklist
	{
		public Blacklist()
		{
			Items = new List<BlacklistItem>();
		}

		public List<BlacklistItem> Items { get; }

		public class BlacklistItem
		{
			public string Id { get; set; }
			public string Name { get; set; }
		}
	}
}