namespace KryBot.Core
{
	public static class Links
	{
		#region GameMiner

		public static readonly string GameMinerWon = $"{GameMiner}giveaways/won";
		public static readonly string GameMinerGoldenGiveaways = $"{GameMiner}api/giveaways/golden?count=10&q=&type=regular&enter_price=on&sortby=finish&order=asc&filter_entered=on";
		public static readonly string GameMinerRegularGiveaways = $"{GameMiner}api/giveaways/coal?count=10&q=&enter_price=on&sortby=finish&order=asc&filter_entered=on";
		public static readonly string GameMinerSandboxGiveaways = $"{GameMiner}api/giveaways/sandbox?count=10&q=&enter_price=on&sortby=finish&order=asc&filter_entered=on";

		#endregion

		public const string Steam = "http://steamcommunity.com/";
		public const string SteamGifts = "https://www.steamgifts.com/";
		public const string GameMiner = "http://gameminer.net/";
		public const string SteamTrade = "http://steamtrade.info/";
		public const string PlayBlink = "http://playblink.com/";
		public const string UseGamble = "http://usegamble.com/";
		public const string SteamCompanion = "https://steamcompanion.com/";
		public const string GameAways = "http://www.gameaways.com/";
	}
}