namespace KryBot.Functional.Giveaways
{
	public class GameAwaysGiveaway : BaseGiveaway
	{
		public string Id { get; set; }

		public bool IsGroup { get; set; }

		public int Price { get; set; }
	}
}