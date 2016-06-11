namespace KryBot.Core.Json.GameMiner
{
	public class JsonGiveaway
	{
		public bool Golden { get; set; }
		public string Code { get; set; }
		public int Price { get; set; }
		public object Sandbox { get; set; }
		public JsonGame Game { get; set; }
		public string RegionlockTypeId { get; set; }
	}
}
