using System.Collections.Generic;

namespace KryBot.Core.Serializable.GameMiner
{
	public class JsonRootObject
	{
		public List<JsonGiveaway> Giveaways { get; set; }
		public int last_page { get; set; }
		public int Total { get; set; }
		public int Page { get; set; }
	}
}