using System.Collections.Generic;
using System.Linq;
using KryBot.Core.Giveaways;

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

		/// <summary>
		///     Remove all blacklisted games from list of <paramref name="giveaways" />.
		/// </summary>
		/// <param name="giveaways"> List of Giweways. </param>
		public void RemoveGames<T>(IList<T> giveaways) where T : BaseGiveaway
		{
			if(Items == null) return;
			for(var i = 0; i < giveaways.Count; i++)
			{
				if(Items.Any(item => giveaways[i].StoreId == item.Id))
				{
					giveaways.Remove(giveaways[i]);
					i--;
				}
			}
		}
	}
}