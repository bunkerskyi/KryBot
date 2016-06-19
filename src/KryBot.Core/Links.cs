namespace KryBot.Core
{
	public static class Links
	{
		public const string VkGroup = "https://vk.com/krybot";

		#region GameAways

		public const string GameAways = "http://www.gameaways.com/";

		public const string GameAwaysAuth =
			"https://steamcommunity.com/openid/login?openid.ns=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0&openid.mode=checkid_setup&openid.claimed_id=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.identity=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.return_to=http%3A%2F%2Fwww.gameaways.com%2FAccount%2FExternalLoginCallback%3FReturnUrl%3D%252FHome%252FIndex%26authenticationProvider%3DSteam&openid.realm=http%3A%2F%2Fwww.gameaways.com";

		public static readonly string GameAwaysJoin = $"{GameAways}Giveaways/Enter/";

		#endregion

		#region GameMiner

		public const string GameMiner = "http://gameminer.net/";

		public static readonly string GameMinerWon = $"{GameMiner}giveaways/won";

		public static readonly string GameMinerGoldenGiveaways =
			$"{GameMiner}api/giveaways/golden?count=10&q=&type=regular&enter_price=on&sortby=finish&order=asc&filter_entered=on";

		public static readonly string GameMinerRegularGiveaways =
			$"{GameMiner}api/giveaways/coal?count=10&q=&enter_price=on&sortby=finish&order=asc&filter_entered=on";

		public static readonly string GameMinerSandboxGiveaways =
			$"{GameMiner}api/giveaways/sandbox?count=10&q=&enter_price=on&sortby=finish&order=asc&filter_entered=on";

		public static readonly string GameMinerSync = $"{GameMiner}account/sync";

		#endregion

		#region SteamGifts

		public const string SteamGifts = "https://www.steamgifts.com/";

		public static readonly string SteamGiftsAjax = $"{SteamGifts}ajax.php";

		public static readonly string SteamGiftsWon = $"{SteamGifts}giveaways/won";

		public static readonly string SteamGiftsSearch = $"{SteamGifts}giveaways/search";

		public static readonly string SteamGiftsSync = $"{SteamGifts}account/profile/sync";

		#endregion

		#region SteamCompanion

		public const string SteamCompanion = "https://steamcompanion.com/";

		public static readonly string SteamCompanionJoin = $"{SteamCompanion}gifts/steamcompanion.php";

		public static readonly string SteamCompanionWon = $"{SteamCompanion}gifts/won";

		public static readonly string SteamCompanionSearch = $"{SteamCompanion}gifts/search/";

		#endregion

		#region UseGamble

		public const string UseGamble = "http://usegamble.com/";

		public static readonly string UseGambleJoin = $"{UseGamble}page/join";

		public static readonly string UseGambleWon = $"{UseGamble}profile/logs";

		public static readonly string UseGambleGiveaways = $"{UseGamble}page";

		public static readonly string UseGambleGaPage = $"{UseGamble}page/ga_page";

		#endregion

		#region SteamTrade

		public const string SteamTrade = "http://steamtrade.info/";

		public static readonly string SteamTradeLogin = $"{SteamTrade}reg.php?login";

		public static readonly string SteamTradeWon = $"{SteamTrade}awards/";

		#endregion

		#region PlayBlink

		public const string PlayBlink = "http://playblink.com/";

		public static readonly string PlayBlinkJoin = $"{PlayBlink}?do=blink&captcha=1";

		#endregion

		#region Steam

		public const string Steam = "http://steamcommunity.com/";

		public const string SteamGroup = "https://steamcommunity.com/groups/krybot";

		public const string SteamGameInfo = "http://store.steampowered.com/api/appdetails?appids=";

		public const string SteamTradeUrl = "https://steamcommunity.com/tradeoffer/new/?partner=107171644&token=eSC3IOi7";

		#endregion

		#region GitHub

		public const string GitHubRepo = "https://github.com/KriBetko/KryBot";
		public const string GitHubLatestReliase = "https://api.github.com/repos/KriBetko/KryBot/releases/latest";

		#endregion
	}
}