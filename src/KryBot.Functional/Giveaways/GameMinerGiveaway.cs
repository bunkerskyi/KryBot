namespace KryBot.Functional.Giveaways
{
	public class GameMinerGiveaway : BaseGiveaway
	{
		public int Page { get; set; }

		public string Region { get; set; }

		public string Id { get; set; }

		public bool IsSandbox { get; set; }

		public bool IsRegular { get; set; }

		public bool IsGolden { get; set; }
	}
}
