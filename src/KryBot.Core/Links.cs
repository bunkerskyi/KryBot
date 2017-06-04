/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
*/
// ReSharper disable UnusedMember.Global
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

        public const string SteamGiftsBlackList = "https://www.steamgifts.com/account/settings/giveaways/filters";

        #endregion

        #region SteamCompanion

        public const string SteamCompanion = "https://steamcompanion.com/";

        public static readonly string SteamCompanionJoin = $"{SteamCompanion}gifts/steamcompanion.php";

        public static readonly string SteamCompanionWon = $"{SteamCompanion}gifts/won";

        public static readonly string SteamCompanionSearch = $"{SteamCompanion}gifts/search/";

        public static readonly string SteamCompanionSync = $"{SteamCompanion}settings/resync&success=true";

        #endregion

        #region SteamPortal

        public const string SteamPortal = "http://steamportal.net/";

        public static readonly string SteamPortalJoin = $"{SteamPortal}page/join";

        public static readonly string SteamPortalWon = $"{SteamPortal}profile/logs";

        public static readonly string SteamPortalGiveaways = $"{SteamPortal}page";

        public static readonly string SteamPortalGaPage = $"{SteamPortal}page/ga_page";

        #endregion

        #region SteamTrade

        public const string SteamTrade = "http://steamtrade.info/";

        public static readonly string SteamTradeLogin = $"{SteamTrade}reg.php?login";

        public static readonly string SteamTradeWon = $"{SteamTrade}awards/";

        #endregion

        #region PlayBlink

        public const string PlayBlink = "http://playblink.com/";

        public const string PlayBlinkAuth =
            "https://steamcommunity.com/openid/login?openid.ns=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0&openid.mode=checkid_setup&openid.return_to=http%3A%2F%2Fplayblink.com%2F%3Fdo%3Dlogin%26act%3Dsignin&openid.realm=http%3A%2F%2Fplayblink.com%2F&openid.ns.sreg=http%3A%2F%2Fopenid.net%2Fextensions%2Fsreg%2F1.1&openid.claimed_id=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.identity=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select";

        public static readonly string PlayBlinkJoin = $"{PlayBlink}?do=blink&captcha=1";

        #endregion

        #region Steam

        public const string Steam = "http://steamcommunity.com/";

        public const string SteamGroup = "https://steamcommunity.com/groups/krybot";

        public const string SteamGameInfo = "http://store.steampowered.com/api/appdetails?appids=";

        public const string SteamTradeUrl =
            "https://steamcommunity.com/tradeoffer/new/?partner=107171644&token=eSC3IOi7";

        public static string SteamUserGames(string userProfile)
        {
            return $"{userProfile}games?tab=all&xml=1";
        }

        #endregion

        #region GitHub

        public const string GitHubRepo = "https://github.com/KriBetko/KryBot";
        public const string GitHubLatestReliase = "https://api.github.com/repos/KriBetko/KryBot/releases/latest";

        #endregion

        #region InventoryGifts

        public const string InventoryGifts = "https://inventorygifts.com/";

        public const string InventoryGiftsAuth =
            "https://steamcommunity.com/openid/login?openid.ns=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0&openid.mode=checkid_setup&openid.return_to=https%3A%2F%2Finventorygifts.com&openid.realm=https%3A%2F%2Finventorygifts.com&openid.identity=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select&openid.claimed_id=http%3A%2F%2Fspecs.openid.net%2Fauth%2F2.0%2Fidentifier_select";

        public static readonly string InventoryGiftsSearch = $"{InventoryGifts}section.php";

        public static readonly string InvenrotyGiftsSync = $"{InventoryGifts}settings/dc_resync.php";

        public static readonly string InventoryGiftsJoin = $"{InventoryGifts}giveaway_threads/view_thread.php";

        #endregion
    }
}