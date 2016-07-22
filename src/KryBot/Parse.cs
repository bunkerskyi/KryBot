using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HtmlAgilityPack;
using KryBot.lang;
using Newtonsoft.Json;
using RestSharp;
using static KryBot.Messages;

namespace KryBot
{
    public static class Parse
    {
        #region Steam

        private static Log SteamGetProfile(Bot bot)
        {
            var response = Web.Get("http://steamcommunity.com/", "", new List<Parameter>(),
                Generate.Cookies_Steam(bot), new List<HttpHeader>());

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var login =
                    htmlDoc.DocumentNode.SelectSingleNode(
                        "//a[contains(@class, 'user_avatar') and contains(@class, 'playerAvatar')]");
                if (login == null)
                {
                    //ProfileLoaded();
                    return ParseProfileFailed("Steam");
                }

                bot.Steam.ProfileLink = login.Attributes["href"].Value;
                return ParseProfile("Steam", login.Attributes["href"].Value.Split('/')[4]);
            }
            return ParseProfileFailed("Steam");
        }

        public static async Task<Log> SteamGetProfileAsync(Bot bot)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = SteamGetProfile(bot);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static async Task<Classes.ProfileGamesList> SteamGetUserGames(string profileLink)
        {
            var responseXmlProfile =
                await
                    Web.GetAsync($"{profileLink}games?tab=all&xml=1", "", new List<Parameter>(), new CookieContainer(),
                        new List<HttpHeader>(), "");
            var serializer = new XmlSerializer(typeof(Classes.ProfileGamesList));
            TextReader reader = new StringReader(responseXmlProfile.RestResponse.Content);
            var games = (Classes.ProfileGamesList) serializer.Deserialize(reader);
            return games;
        }

        public static async Task<string> SteamGetGameName(string appId)
        {
            var responseJsonDetail =
                await
                    Web.GetAsync($"http://store.steampowered.com/api/appdetails?appids={appId}", "",
                        new List<Parameter>(),
                        new CookieContainer(), new List<HttpHeader>(), "");

            if (responseJsonDetail.RestResponse.Content == "null" || responseJsonDetail.RestResponse.Content == "")
            {
                return null;
            }

            var json = responseJsonDetail.RestResponse.Content.Replace($"{{\"{appId}\":", "");
            var gameDetail = JsonConvert.DeserializeObject<Classes.GameDetail>(json.Substring(0, json.Length - 1));
            return gameDetail.success ? gameDetail.data.name : null;
        }

        #endregion

        #region GameMiner

        private static Log GameMinerGetProfile(Bot bot)
        {
            var response = Web.Get("http://gameminer.net/", "", new List<Parameter>(),
                Generate.Cookies_GameMiner(bot), new List<HttpHeader>(), bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var coal = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='user__coal']");
                var level = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='g-level-icon']");
                var username = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='dashboard__user-name']");

                if (coal != null && level != null && username != null)
                {
                    bot.GameMiner.Coal = int.Parse(coal.InnerText);
                    bot.GameMiner.Level = int.Parse(level.InnerText);

                    ProfileLoaded();
                    return ParseProfile("GameMiner", bot.GameMiner.Coal, bot.GameMiner.Level,
                        username.InnerText.Trim().Replace("\n1", ""));
                }
            }
            ProfileLoaded();
            return ParseProfileFailed("GameMiner");
        }

        public static async Task<Log> GameMinerGetProfileAsync(Bot bot)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = GameMinerGetProfile(bot);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        private static Log GameMinerWonParse(Bot bot)
        {
            var response = Web.Get("http://gameminer.net/", "giveaways/won", new List<Parameter>(),
                Generate.Cookies_GameMiner(bot), new List<HttpHeader>(), bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var nodes =
                    htmlDoc.DocumentNode.SelectNodes(
                        "//tbody[@class='giveaways__giveaways']//td[@class='valign-middle m-table-state-finished']");

                if (nodes != null)
                {
                    for (var i = 0; i < nodes.Count; i++)
                    {
                        if (!nodes[i].InnerText.Contains("требует подтверждения") &&
                            !nodes[i].InnerText.Contains("to be confirmed"))
                        {
                            nodes.Remove(nodes[i]);
                            i--;
                        }
                    }

                    if (nodes.Count > 0)
                    {
                        return GiveawayHaveWon("GameMiner", nodes.Count, "http://gameminer.net/giveaways/won");
                    }
                }
            }
            return null;
        }

        public static async Task<Log> GameMinerWonParseAsync(Bot bot)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = GameMinerWonParse(bot);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        private static Log GameMinerLoadGiveaways(Bot bot, List<GameMiner.GmGiveaway> giveaways,
            Blacklist blackList)
        {
            var content = "";
            giveaways?.Clear();
            var pages = 0;

            if (bot.GameMiner.FreeGolden)
            {
                var goldenFreesResponse = Web.Get("http://gameminer.net",
                    "/api/giveaways/golden?page=1&count=10&q=&type=regular&enter_price=on&sortby=finish&order=asc&filter_entered=on",
                    new List<Parameter>(),
                    Generate.Cookies_GameMiner(bot),
                    new List<HttpHeader>(), bot.UserAgent);

                if (goldenFreesResponse.RestResponse.Content != "")
                {
                    var goldenFreeJsonResponse =
                        JsonConvert.DeserializeObject<GameMiner.JsonRootObject>(
                            goldenFreesResponse.RestResponse.Content);
                    GameMinerAddGiveaways(goldenFreeJsonResponse, bot, giveaways);

                    content +=
                        $"{GetDateTime()} {{GameMiner}} {strings.ParseLoadGiveaways_Found} {goldenFreeJsonResponse.total} " +
                        $"{strings.ParseLoadGiveaways_FreeGoldenGiveawaysIn} {goldenFreeJsonResponse.last_page} {strings.ParseLoadGiveaways_Pages}\n";

                    pages = goldenFreeJsonResponse.last_page;

                    if (pages > 1)
                    {
                        for (var i = 1; i < pages + 1; i++)
                        {
                            goldenFreesResponse = Web.Get("http://gameminer.net/",
                                $"/api/giveaways/golden?page={i + 1}&count=10&q=&type=regular&enter_price=on&sortby=finish&order=asc&filter_entered=on",
                                new List<Parameter>(),
                                Generate.Cookies_GameMiner(bot),
                                new List<HttpHeader>(),
                                bot.UserAgent);
                            goldenFreeJsonResponse =
                                JsonConvert.DeserializeObject<GameMiner.JsonRootObject>(
                                    goldenFreesResponse.RestResponse.Content);

                            GameMinerAddGiveaways(goldenFreeJsonResponse, bot, giveaways);
                        }
                    }
                }
            }

            if (bot.GameMiner.Regular)
            {
                var regularResponse = Web.Get("http://gameminer.net",
                    $"/api/giveaways/coal?page=1&count=10&q=&type={(bot.GameMiner.OnlyGifts ? "regular" : "any")}&enter_price=on&sortby=finish&order=asc&filter_entered=on",
                    new List<Parameter>(),
                    Generate.Cookies_GameMiner(bot),
                    new List<HttpHeader>(), bot.UserAgent);

                var regularJsonResponse =
                    JsonConvert.DeserializeObject<GameMiner.JsonRootObject>(
                        regularResponse.RestResponse.Content);
                GameMinerAddGiveaways(regularJsonResponse, bot, giveaways);
                content +=
                    $"{GetDateTime()} {{GameMiner}} {strings.ParseLoadGiveaways_Found} {regularJsonResponse.total} " +
                    $"{strings.ParseLoadGiveaways_RegularGiveawaysIn} {regularJsonResponse.last_page} {strings.ParseLoadGiveaways_Pages}\n";

                pages = regularJsonResponse.last_page;

                if (pages > 1)
                {
                    for (var i = 1; i < pages + 1; i++)
                    {
                        var regularGiftsResponse = Web.Get("http://gameminer.net/",
                            $"/api/giveaways/coal?page={i + 1}&count=10&q=&type={(bot.GameMiner.OnlyGifts ? "regular" : "any")}&enter_price=on&sortby=finish&order=asc&filter_entered=on",
                            new List<Parameter>(),
                            Generate.Cookies_GameMiner(bot),
                            new List<HttpHeader>(), bot.UserAgent);

                        var regularGiftJsonResponse =
                            JsonConvert.DeserializeObject<GameMiner.JsonRootObject>(
                                regularGiftsResponse.RestResponse.Content);

                        GameMinerAddGiveaways(regularGiftJsonResponse, bot, giveaways);
                    }
                }
            }

            if (bot.GameMiner.Sandbox)
            {
                var sandboxGiftsResponse = Web.Get("http://gameminer.net",
                    $"/api/giveaways/sandbox?page=1&count=10&q=&type={(bot.GameMiner.OnlyGifts ? "regular" : "any")}&enter_price=on&sortby=finish&order=asc&filter_entered=on",
                    new List<Parameter>(),
                    Generate.Cookies_GameMiner(bot),
                    new List<HttpHeader>(), bot.UserAgent);

                if (sandboxGiftsResponse.RestResponse.Content != "")
                {
                    var sandboxGiftJsonResponse =
                        JsonConvert.DeserializeObject<GameMiner.JsonRootObject>(
                            sandboxGiftsResponse.RestResponse.Content);
                    GameMinerAddGiveaways(sandboxGiftJsonResponse, bot, giveaways);

                    content +=
                        $"{GetDateTime()} {{GameMiner}} {strings.ParseLoadGiveaways_Found} {sandboxGiftJsonResponse.total} " +
                        $"{strings.ParseLoadGiveaways_RegularGiveawaysIn} {sandboxGiftJsonResponse.last_page} {strings.ParseLoadGiveaways_Pages}\n";

                    pages = sandboxGiftJsonResponse.last_page;
                }

                if (pages > 1)
                {
                    for (var i = 1; i < pages + 1; i++)
                    {
                        var regularGiftsResponse = Web.Get("http://gameminer.net/",
                            $"/api/giveaways/sandbox?page={i + 1}&count=10&q=&type={(bot.GameMiner.OnlyGifts ? "regular" : "any")}&enter_price=on&sortby=finish&order=asc&filter_entered=on",
                            new List<Parameter>(),
                            Generate.Cookies_GameMiner(bot),
                            new List<HttpHeader>(), bot.UserAgent);

                        if (regularGiftsResponse.RestResponse.Content != "")
                        {
                            var regularGiftJsonResponse =
                                JsonConvert.DeserializeObject<GameMiner.JsonRootObject>(
                                    regularGiftsResponse.RestResponse.Content);

                            GameMinerAddGiveaways(regularGiftJsonResponse, bot, giveaways);
                        }
                    }
                }
            }

            if (giveaways == null)
            {
                return
                    new Log(
                        $"{content}{GetDateTime()} {{GameMiner}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: 0",
                        Color.White, true, true);
            }

            if (blackList.Items != null)
            {
                for (var i = 0; i < giveaways.Count; i++)
                {
                    foreach (var item in blackList.Items)
                    {
                        if (giveaways[i].StoreId == item.Id)
                        {
                            giveaways.Remove(giveaways[i]);
                            i--;
                            break;
                        }
                    }
                }
            }

            return
                new Log(
                    $"{content}{GetDateTime()} {{GameMiner}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: {giveaways.Count}",
                    Color.White, true, true);
        }

        public static async Task<Log> GameMinerLoadGiveawaysAsync(Bot bot,
            List<GameMiner.GmGiveaway> giveaways, Blacklist blackList)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = GameMinerLoadGiveaways(bot, giveaways, blackList);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        private static void GameMinerAddGiveaways(GameMiner.JsonRootObject json, Bot bot,
            List<GameMiner.GmGiveaway> giveaways)
        {
            if (json != null)
                foreach (var giveaway in json.giveaways)
                {
                    var lot = new GameMiner.GmGiveaway();
                    if (giveaway.golden && giveaway.price != 0)
                    {
                        break;
                    }

                    if (lot.Price > bot.GameMiner.Coal || lot.Price > bot.GameMiner.JoinCoalLimit)
                    {
                        break;
                    }

                    lot.Name = giveaway.game.name;
                    lot.Id = giveaway.code;
                    lot.IsRegular = giveaway.sandbox == null;
                    lot.IsSandbox = giveaway.sandbox != null;
                    lot.IsGolden = giveaway.golden;
                    lot.Page = json.page;
                    lot.Price = giveaway.price;

                    if (giveaway.regionlock_type_id != null)
                    {
                        lot.Region = giveaway.regionlock_type_id;
                    }

                    if (giveaway.game.url != "javascript:void(0);")
                    {
                        lot.StoreId = giveaway.game.url.Split('/')[4];
                    }
                    else
                    {
                        break;
                    }

                    giveaways?.Add(lot);
                }
        }

        #endregion

        #region SteamGifts

        private static Log SteamGiftsGetProfile(Bot bot)
        {
            var response = Web.Get("https://www.steamgifts.com/", "", new List<Parameter>(),
                Generate.Cookies_SteamGifts(bot), new List<HttpHeader>(), bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var points = htmlDoc.DocumentNode.SelectSingleNode("//a[@href='/account']/span[1]");
                var level = htmlDoc.DocumentNode.SelectSingleNode("//a[@href='/account']/span[2]");
                var username = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='nav__avatar-outer-wrap']");

                if (points != null && level != null && username != null)
                {
                    bot.SteamGifts.Points = int.Parse(points.InnerText);
                    bot.SteamGifts.Level = int.Parse(level.InnerText.Split(' ')[1]);
                    return ParseProfile("SteamGifts", bot.SteamGifts.Points, bot.SteamGifts.Level,
                        username.Attributes["href"].Value.Split('/')[2]);
                }

                var error = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='notification notification--warning']");
                if (error != null)
                {
                    bot.SteamGifts.Enabled = false;
                    bot.GameMiner.Cookies.Xsrf = "";
                    bot.Save();
                    return
                        new Log(
                            $"{GetDateTime()} {{SteamGifts}} {strings.AccountNotActive} {{{error.InnerText}}}",
                            Color.Red, false, true);
                }
            }
            return ParseProfileFailed("SteamGifts");
        }

        public static async Task<Log> SteamGiftsGetProfileAsync(Bot bot)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = SteamGiftsGetProfile(bot);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        private static Log SteamGiftsWonParse(Bot bot)
        {
            var response = Web.Get("https://www.steamgifts.com/", "", new List<Parameter>(),
                Generate.Cookies_SteamGifts(bot), new List<HttpHeader>(), bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var nodes =
                    htmlDoc.DocumentNode.SelectSingleNode("//a[@title='Giveaways Won']//div[@class='nav__notification']");

                if (nodes != null)
                {
                    return GiveawayHaveWon("SteamGifts", int.Parse(nodes.InnerText),
                        "https://www.steamgifts.com/giveaways/won");
                }
            }
            return null;
        }

        public static async Task<Log> SteamGiftsWonParseAsync(Bot bot)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = SteamGiftsWonParse(bot);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        private static Log SteamGiftsLoadGiveaways(Bot bot, List<SteamGifts.SgGiveaway> giveaways,
            List<SteamGifts.SgGiveaway> wishlistGiveaways, Blacklist blackList)
        {
            var content = "";
            giveaways?.Clear();

            if (bot.SteamGifts.WishList)
            {
                content += SteamGiftsLoadWishListGiveaways(bot, wishlistGiveaways);
            }

            if (bot.SteamGifts.Group)
            {
                content += SteamGiftsLoadGroupGiveaways(bot, giveaways);
            }

            if (bot.SteamGifts.Region)
            {
                content += SteamGiftsLoadRegionGiveaways(bot, giveaways);
            }

            if (bot.SteamGifts.MinNumberCopies)
            {
                content += SteamGiftsLoadMinNumberCopiesGiveaways(bot, giveaways);
            }

            if (bot.SteamGifts.Regular)
            {
                var response = Web.Get("https://www.steamgifts.com", "/giveaways/search?page=1",
                    new List<Parameter>(), Generate.Cookies_SteamGifts(bot),
                    new List<HttpHeader>(), bot.UserAgent);

                if (response.RestResponse.Content != "")
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(response.RestResponse.Content);

                    var pages = 1;
                    var pageNodeCouner = htmlDoc.DocumentNode.SelectNodes("//div[@class='pagination__navigation']/a");
                    if (pageNodeCouner != null)
                    {
                        var pageNode =
                            htmlDoc.DocumentNode.SelectSingleNode(
                                $"//div[@class='pagination__navigation']/a[{pageNodeCouner.Count - 1}]");
                        if (pageNode != null)
                        {
                            pages = int.Parse(pageNode.Attributes["data-page-number"].Value);
                        }
                    }

                    if (pages != 1)
                    {
                        for (var i = 1; i < pages + 1; i++)
                        {
                            response = Web.Get("https://www.steamgifts.com", "/giveaways/search?page=" + (i + 1),
                                new List<Parameter>(), Generate.Cookies_SteamGifts(bot),
                                new List<HttpHeader>(), bot.UserAgent);

                            if (response.RestResponse.Content != "")
                            {
                                htmlDoc = new HtmlDocument();
                                htmlDoc.LoadHtml(response.RestResponse.Content);

                                var pageNodeCounter =
                                    htmlDoc.DocumentNode.SelectNodes("//div[@class='pagination__navigation']/a");
                                if (pageNodeCounter != null)
                                {
                                    var pageNode =
                                        htmlDoc.DocumentNode.SelectSingleNode(
                                            $"//div[@class='pagination__navigation']/a[{pageNodeCounter.Count - 1}]");
                                    if (pageNode != null)
                                    {
                                        pages = int.Parse(pageNode.Attributes["data-page-number"].Value);
                                    }
                                }

                                var nodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='widget-container']" +
                                                                             "//div[2]//div[3]//div[@class='giveaway__row-outer-wrap']//div[@class='giveaway__row-inner-wrap']");
                                SteamGiftsAddGiveaways(nodes, bot, giveaways);
                            }
                        }
                    }
                }
            }

            if (giveaways?.Count == 0 && wishlistGiveaways.Count == 0)
            {
                return
                    new Log(
                        $"{content}{GetDateTime()} {{SteamGifts}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: 0",
                        Color.White, true, true);
            }

            if (blackList.Items != null)
            {
                for (var i = 0; i < giveaways?.Count; i++)
                {
                    foreach (var item in blackList.Items)
                    {
                        if (giveaways[i].StoreId == item.Id)
                        {
                            giveaways.Remove(giveaways[i]);
                            i--;
                        }
                    }
                }
            }

            return
                new Log(
                    $"{content}{GetDateTime()} {{SteamGifts}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: {giveaways?.Count + wishlistGiveaways.Count}",
                    Color.White, true, true);
        }

        public static async Task<Log> SteamGiftsLoadGiveawaysAsync(Bot bot,
            List<SteamGifts.SgGiveaway> giveaways, List<SteamGifts.SgGiveaway> wishlistGiveaways,
            Blacklist blackList)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = SteamGiftsLoadGiveaways(bot, giveaways, wishlistGiveaways, blackList);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        private static string SteamGiftsLoadWishListGiveaways(Bot bot, List<SteamGifts.SgGiveaway> giveaways)
        {
            var nodesCount = 0;
            var pages = 1;

            var response = Web.Get("https://www.steamgifts.com", "/giveaways/search?type=wishlist",
                new List<Parameter>(), Generate.Cookies_SteamGifts(bot), new List<HttpHeader>(), bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var pageNodeCounter = htmlDoc.DocumentNode.SelectNodes("//div[@class='pagination__navigation']/a");
                if (pageNodeCounter != null)
                {
                    var pageNode =
                        htmlDoc.DocumentNode.SelectSingleNode(
                            $"//div[@class='pagination__navigation']/a[{pageNodeCounter.Count - 1}]");
                    if (pageNode != null)
                    {
                        pages = int.Parse(pageNode.Attributes["data-page-number"].Value);
                    }
                }

                if (pages != 1)
                {
                    for (var i = 1; i < pages + 1; i++)
                    {
                        var nodes =
                            htmlDoc.DocumentNode.SelectNodes(
                                "//div[@class='widget-container']//div[2]//div[3]//div[@class='giveaway__row-outer-wrap']//div[@class='giveaway__row-inner-wrap']");

                        pageNodeCounter = htmlDoc.DocumentNode.SelectNodes("//div[@class='pagination__navigation']/a");
                        var pageNode =
                            htmlDoc.DocumentNode.SelectSingleNode(
                                $"//div[@class='pagination__navigation']/a[{pageNodeCounter.Count - 1}]");
                        if (pageNode != null)
                        {
                            pages = int.Parse(pageNode.Attributes["data-page-number"].Value);
                        }

                        nodesCount += nodes?.Count ?? 0;
                        SteamGiftsAddGiveaways(nodes, bot, giveaways);
                    }
                }
                else
                {
                    var nodes =
                        htmlDoc.DocumentNode.SelectNodes(
                            "//div[@class='widget-container']//div[2]//div[3]//div[@class='giveaway__row-outer-wrap']//div[@class='giveaway__row-inner-wrap']");
                    nodesCount += nodes?.Count ?? 0;
                    SteamGiftsAddGiveaways(nodes, bot, giveaways);
                }
            }
            return
                $"{GetDateTime()} {{SteamGifts}} {strings.ParseLoadGiveaways_FoundGiveAwaysInWishList}: {nodesCount}\n";
        }

        private static string SteamGiftsLoadGroupGiveaways(Bot bot, List<SteamGifts.SgGiveaway> giveaways)
        {
            var nodesCount = 0;

            var response = Web.Get("https://www.steamgifts.com", "/giveaways/search?type=group",
                new List<Parameter>(), Generate.Cookies_SteamGifts(bot), new List<HttpHeader>(), bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var pages = 1;

                var pageNodeCounter = htmlDoc.DocumentNode.SelectNodes("//div[@class='pagination__navigation']/a");
                if (pageNodeCounter != null)
                {
                    var pageNode =
                        htmlDoc.DocumentNode.SelectSingleNode(
                            $"//div[@class='pagination__navigation']/a[{pageNodeCounter.Count - 1}]");
                    if (pageNode != null)
                    {
                        pages = int.Parse(pageNode.Attributes["data-page-number"].Value);
                    }
                }

                if (pages != 1)
                {
                    for (var i = 1; i < pages + 1; i++)
                    {
                        var nodes =
                            htmlDoc.DocumentNode.SelectNodes(
                                "//div[@class='widget-container']//div[2]//div[3]//div[@class='giveaway__row-outer-wrap']//div[@class='giveaway__row-inner-wrap']");


                        pageNodeCounter = htmlDoc.DocumentNode.SelectNodes("//div[@class='pagination__navigation']/a");
                        if (pageNodeCounter != null)
                        {
                            var pageNode =
                                htmlDoc.DocumentNode.SelectSingleNode(
                                    $"//div[@class='pagination__navigation']/a[{pageNodeCounter.Count - 1}]");
                            if (pageNode != null)
                            {
                                pages = int.Parse(pageNode.Attributes["data-page-number"].Value);
                            }
                        }

                        nodesCount += nodes?.Count ?? 0;
                        SteamGiftsAddGiveaways(nodes, bot, giveaways);
                    }
                }
                else
                {
                    var nodes =
                        htmlDoc.DocumentNode.SelectNodes(
                            "//div[@class='widget-container']//div[2]//div[3]//div[@class='giveaway__row-outer-wrap']//div[@class='giveaway__row-inner-wrap']");
                    SteamGiftsAddGiveaways(nodes, bot, giveaways);
                }
            }
            return $"{GetDateTime()} {{SteamGifts}} {strings.ParseLoadGiveaways_FoundGiveAwaysInGroup}: {nodesCount}\n";
        }

        private static string SteamGiftsLoadRegionGiveaways(Bot bot, List<SteamGifts.SgGiveaway> giveaways)
        {
            var nodesCount = 0;

            var response = Web.Get("https://www.steamgifts.com", "/giveaways/search?region_restricted=true",
                new List<Parameter>(), Generate.Cookies_SteamGifts(bot), new List<HttpHeader>(), bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var pages = 1;

                var pageNodeCounter = htmlDoc.DocumentNode.SelectNodes("//div[@class='pagination__navigation']/a");
                if (pageNodeCounter != null)
                {
                    var pageNode =
                        htmlDoc.DocumentNode.SelectSingleNode(
                            $"//div[@class='pagination__navigation']/a[{pageNodeCounter.Count - 1}]");
                    if (pageNode != null)
                    {
                        pages = int.Parse(pageNode.Attributes["data-page-number"].Value);
                    }
                }

                if (pages != 1)
                {
                    for (var i = 1; i < pages + 1; i++)
                    {
                        var nodes =
                            htmlDoc.DocumentNode.SelectNodes(
                                "//div[@class='widget-container']//div[2]//div[3]//div[@class='giveaway__row-outer-wrap']//div[@class='giveaway__row-inner-wrap']");


                        pageNodeCounter = htmlDoc.DocumentNode.SelectNodes("//div[@class='pagination__navigation']/a");
                        if (pageNodeCounter != null)
                        {
                            var pageNode =
                                htmlDoc.DocumentNode.SelectSingleNode(
                                    $"//div[@class='pagination__navigation']/a[{pageNodeCounter.Count - 1}]");
                            if (pageNode != null)
                            {
                                pages = int.Parse(pageNode.Attributes["data-page-number"].Value);
                            }
                        }

                        nodesCount += nodes?.Count ?? 0;
                        SteamGiftsAddGiveaways(nodes, bot, giveaways);
                    }
                }
                else
                {
                    var nodes =
                        htmlDoc.DocumentNode.SelectNodes(
                            "//div[@class='widget-container']//div[2]//div[3]//div[@class='giveaway__row-outer-wrap']//div[@class='giveaway__row-inner-wrap']");
                    SteamGiftsAddGiveaways(nodes, bot, giveaways);
                }
            }
            return $"{GetDateTime()} {{SteamGifts}} {strings.ParseLoadGiveaways_FoundGiveAwaysInRegion}: {nodesCount}\n";
        }

        private static string SteamGiftsLoadMinNumberCopiesGiveaways(Bot bot, List<SteamGifts.SgGiveaway> giveaways)
        {
            var nodesCount = 0;

            var response = Web.Get("https://www.steamgifts.com", "/giveaways/search?copy_min=" + bot.SteamGifts.NumberCopies,
                new List<Parameter>(), Generate.Cookies_SteamGifts(bot), new List<HttpHeader>(), bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var pages = 1;

                var pageNodeCounter = htmlDoc.DocumentNode.SelectNodes("//div[@class='pagination__navigation']/a");
                if (pageNodeCounter != null)
                {
                    var pageNode =
                        htmlDoc.DocumentNode.SelectSingleNode(
                            $"//div[@class='pagination__navigation']/a[{pageNodeCounter.Count - 1}]");
                    if (pageNode != null)
                    {
                        pages = int.Parse(pageNode.Attributes["data-page-number"].Value);
                    }
                }

                if (pages != 1)
                {
                    for (var i = 1; i < pages + 1; i++)
                    {
                        var nodes =
                            htmlDoc.DocumentNode.SelectNodes(
                                "//div[@class='widget-container']//div[2]//div[3]//div[@class='giveaway__row-outer-wrap']//div[@class='giveaway__row-inner-wrap']");


                        pageNodeCounter = htmlDoc.DocumentNode.SelectNodes("//div[@class='pagination__navigation']/a");
                        if (pageNodeCounter != null)
                        {
                            var pageNode =
                                htmlDoc.DocumentNode.SelectSingleNode(
                                    $"//div[@class='pagination__navigation']/a[{pageNodeCounter.Count - 1}]");
                            if (pageNode != null)
                            {
                                pages = int.Parse(pageNode.Attributes["data-page-number"].Value);
                            }
                        }

                        nodesCount += nodes?.Count ?? 0;
                        SteamGiftsAddGiveaways(nodes, bot, giveaways);
                    }
                }
                else
                {
                    var nodes =
                        htmlDoc.DocumentNode.SelectNodes(
                            "//div[@class='widget-container']//div[2]//div[3]//div[@class='giveaway__row-outer-wrap']//div[@class='giveaway__row-inner-wrap']");
                    SteamGiftsAddGiveaways(nodes, bot, giveaways);
                }
            }
            return $"{GetDateTime()} {{SteamGifts}} {strings.ParseLoadGiveaways_FoundGiveAwaysInMinNumberCopies} {bot.SteamGifts.NumberCopies}: {nodesCount}\n";
        }

        private static void SteamGiftsAddGiveaways(HtmlNodeCollection nodes, Bot bot,
            List<SteamGifts.SgGiveaway> giveaways)
        {
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var name = node.SelectSingleNode(".//a[@class='giveaway__heading__name']");
                    var link = node.SelectSingleNode(".//a[@class='giveaway__heading__name']");
                    var storeId = node.SelectSingleNode(".//a[@class='giveaway__icon']");
                    if (name != null && link != null && storeId != null)
                    {
                        var sgGiveaway = new SteamGifts.SgGiveaway
                        {
                            Name = name.InnerText,
                            Link = link.Attributes["href"].Value,
                            StoreId = storeId.Attributes["href"].Value.Split('/')[4]
                        };
                        sgGiveaway.Code = sgGiveaway.Link.Split('/')[2];

                        foreach (var price in node.SelectNodes(".//span[@class='giveaway__heading__thin']"))
                        {
                            if (!price.InnerText.Contains("Copies"))
                            {
                                sgGiveaway.Price = int.Parse(price.InnerText.Split('(')[1].Split('P')[0]);
                            }
                        }

                        var level = node.SelectSingleNode(".//div[@title='Contributor Level']");
                        sgGiveaway.Level = level == null ? 0 : int.Parse(level.InnerText.Split(' ')[1].Replace("+", ""));

                        var region = node.SelectSingleNode(".//a[@title='Region Restricted']");
                        if (region != null)
                        {
                            sgGiveaway.Region = region.Attributes["href"].Value.Split('/')[2];
                        }

                        if (sgGiveaway.Price <= bot.SteamGifts.Points &&
                            sgGiveaway.Price <= bot.SteamGifts.JoinPointLimit &&
                            sgGiveaway.Level >= bot.SteamGifts.MinLevel)
                        {
                            giveaways?.Add(sgGiveaway);
                        }
                    }
                }
            }
        }

        public static SteamGifts.SgGiveaway SteamGiftsGetJoinData(SteamGifts.SgGiveaway sgGiveaway, Bot bot)
        {
            var response = Web.Get("https://www.steamgifts.com", sgGiveaway.Link,
                new List<Parameter>(), Generate.Cookies_SteamGifts(bot), new List<HttpHeader>(), bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var code = htmlDoc.DocumentNode.SelectSingleNode("//input[@name='code']");
                var token = htmlDoc.DocumentNode.SelectSingleNode("//input[@name='xsrf_token']");
                if (code != null && token != null)
                {
                    sgGiveaway.Code = code.Attributes["value"].Value;
                    sgGiveaway.Token = token.Attributes["value"].Value;
                }
            }
            return sgGiveaway;
        }

        #endregion

        #region SteamCompanion 

        private static Log SteamCompanionGetProfile(Bot bot)
        {
            var response = Web.Get("https://steamcompanion.com", "/", new List<Parameter>(),
                Generate.Cookies_SteamCompanion(bot),
                new List<HttpHeader>());
            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var points = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='points']");
                var profileLink = htmlDoc.DocumentNode.SelectSingleNode("//ul[@class='right']/li[1]/a[1]");
                if (points != null && profileLink != null)
                {
                    bot.SteamCompanion.Points = int.Parse(points.InnerText);
                    bot.SteamCompanion.ProfileLink = profileLink.Attributes["href"].Value;
                    return ParseProfile("SteamCompanion", bot.SteamCompanion.Points,
                        profileLink.Attributes["href"].Value.Split('/')[4]);
                }
            }
            return ParseProfileFailed("SteamCompanion");
        }

        public static async Task<Log> SteamCompanionGetProfileAsync(Bot bot)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = SteamCompanionGetProfile(bot);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        private static Log SteamCompanionWonParse(Bot bot)
        {
            var response = Web.Get("https://steamcompanion.com/", "gifts/won", new List<Parameter>(),
                Generate.Cookies_SteamCompanion(bot), new List<HttpHeader>());
            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var nodes = htmlDoc.DocumentNode.SelectNodes("//table[@id='created_giveaway']/tbody/tr");

                if (nodes != null)
                {
                    for (var i = 0; i < nodes.Count; i++)
                    {
                        var test = nodes[i].SelectSingleNode(".//input[@checked]");
                        if (test != null)
                        {
                            nodes.Remove(nodes[i]);
                            i--;
                        }
                    }

                    if (nodes.Count > 0)
                    {
                        return GiveawayHaveWon("SteamCompanion", nodes.Count, "https://steamcompanion.com/gifts/won");
                    }
                }
            }
            return null;
        }

        public static async Task<Log> SteamCompanionWonParseAsync(Bot bot)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = SteamCompanionWonParse(bot);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        private static Log SteamCompanionLoadGiveaways(Bot bot, List<SteamCompanion.ScGiveaway> giveaways,
            List<SteamCompanion.ScGiveaway> wishlistGiveaways)
        {
            var content = "";
            giveaways?.Clear();

            if (bot.SteamCompanion.WishList)
            {
                content += SteamCompanionLoadWishListGiveaways(bot, wishlistGiveaways);
            }

            if (bot.SteamCompanion.Contributors)
            {
                content += SteamCompanionLoadContributorsGiveaways(bot, giveaways);
            }

            if (bot.SteamCompanion.Group)
            {
                content += SteamCompanionLoadGroupGiveaways(bot, giveaways);
            }

            if (bot.SteamCompanion.Regular)
            {
                var pages = 1;
                for (var i = 0; i < pages; i++)
                {
                    var response = Web.Get("https://steamcompanion.com",
                        i == 0 ? "/gifts/search/?type=public" : "/gifts/search.php?page=" + (i + 1) + "&type=public",
                        new List<Parameter>(),
                        Generate.Cookies_SteamCompanion(bot), new List<HttpHeader>());

                    if (response.RestResponse.Content != null)
                    {
                        var htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(response.RestResponse.Content);

                        var pageNode = htmlDoc.DocumentNode.SelectSingleNode("//li[@class='arrow']/a[1]");
                        if (pageNode != null)
                        {
                            try
                            {
                                pages = int.Parse(pageNode.Attributes["href"].Value.Split('=')[1].Split('&')[0]);
                            }
                            catch (FormatException)
                            {
                                pages = int.Parse(pageNode.Attributes["href"].Value.Split('=')[2]);
                            }
                        }

                        var nodes = htmlDoc.DocumentNode.SelectNodes("//section[@class='col-2-3']/div");
                        if (nodes != null)
                        {
                            for (var j = 0; j < nodes.Count; j++)
                            {
                                if (nodes[j].Attributes["style"] != null &&
                                    nodes[j].Attributes["style"].Value == "opacity: 0.5;")
                                {
                                    nodes.Remove(nodes[j]);
                                    j--;
                                }
                            }

                            if (nodes.Count > 0)
                            {
                                SteamCompanionAddGiveaways(nodes, bot, giveaways);
                            }
                        }
                    }
                }
            }

            if (giveaways?.Count == 0 && wishlistGiveaways.Count == 0)
            {
                return
                    new Log(
                        $"{content}{GetDateTime()} {{SteamCompanion}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: 0",
                        Color.White, true, true);
            }

            return
                new Log(
                    $"{content}{GetDateTime()} {{SteamCompanion}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: {giveaways?.Count + wishlistGiveaways.Count}",
                    Color.White, true, true);
        }

        public static async Task<Log> SteamCompanionLoadGiveawaysAsync(Bot bot,
            List<SteamCompanion.ScGiveaway> giveaways, List<SteamCompanion.ScGiveaway> wishlistGiveaways)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = SteamCompanionLoadGiveaways(bot, giveaways, wishlistGiveaways);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        private static string SteamCompanionLoadWishListGiveaways(Bot bot,
            List<SteamCompanion.ScGiveaway> giveaways)
        {
            var count = 0;
            var pages = 1;
            for (var i = 0; i < pages; i++)
            {
                var response = Web.Get("https://steamcompanion.com",
                    i == 0 ? "/gifts/search/?wishlist=true" : "/gifts/search/?wishlist=true&page=" + (i + 1),
                    new List<Parameter>(),
                    Generate.Cookies_SteamCompanion(bot), new List<HttpHeader>());

                if (response.RestResponse.Content != null)
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(response.RestResponse.Content);

                    var pageNode = htmlDoc.DocumentNode.SelectSingleNode("//li[@class='arrow']/a[1]");
                    if (pageNode != null)
                    {
                        pages = int.Parse(pageNode.Attributes["href"].Value.Split('=')[2]);
                    }

                    var nodes = htmlDoc.DocumentNode.SelectNodes("//section[@class='col-2-3']/div");
                    if (nodes != null)
                    {
                        for (var j = 0; j < nodes.Count; j++)
                        {
                            if (nodes[j].Attributes["style"] != null &&
                                nodes[j].Attributes["style"].Value == "opacity: 0.5;")
                            {
                                nodes.Remove(nodes[j]);
                                j--;
                            }
                        }
                        count += nodes.Count;
                        SteamCompanionAddGiveaways(nodes, bot, giveaways);
                    }
                }
            }
            return
                $"{GetDateTime()} {{SteamCompanion}} {strings.ParseLoadGiveaways_Found} {(giveaways.Count == 0 ? 0 : count)} {strings.ParseLoadGiveaways_WishListGiveAwaysIn} {pages} {strings.ParseLoadGiveaways_Pages}\n";
        }

        private static string SteamCompanionLoadContributorsGiveaways(Bot bot,
            List<SteamCompanion.ScGiveaway> giveaways)
        {
            var count = 0;
            var pages = 1;
            for (var i = 0; i < pages; i++)
            {
                var response = Web.Get("https://steamcompanion.com",
                    i == 0 ? "/gifts/search/?type=contributor" : "/gifts/search/?type=contributor&page=" + (i + 1),
                    new List<Parameter>(),
                    Generate.Cookies_SteamCompanion(bot), new List<HttpHeader>());

                if (response.RestResponse.Content != null)
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(response.RestResponse.Content);

                    var pageNode = htmlDoc.DocumentNode.SelectSingleNode("//li[@class='arrow']/a[1]");
                    if (pageNode != null)
                    {
                        pages = int.Parse(pageNode.Attributes["href"].Value.Split('=')[1]);
                    }

                    var nodes = htmlDoc.DocumentNode.SelectNodes("//section[@class='col-2-3']/div");
                    if (nodes != null)
                    {
                        for (var j = 0; j < nodes.Count; j++)
                        {
                            if (nodes[j].Attributes["style"] != null &&
                                nodes[j].Attributes["style"].Value == "opacity: 0.5;")
                            {
                                nodes.Remove(nodes[j]);
                                j--;
                            }
                        }
                        count += nodes.Count;
                        SteamCompanionAddGiveaways(nodes, bot, giveaways);
                    }
                }
            }
            return
                $"{GetDateTime()} {{SteamCompanion}} {strings.ParseLoadGiveaways_Found} {(giveaways.Count == 0 ? 0 : count)} {strings.ParseLoadGiveaways__ContributorsIn} {pages} {strings.ParseLoadGiveaways_Pages}\n";
        }

        private static string SteamCompanionLoadGroupGiveaways(Bot bot, List<SteamCompanion.ScGiveaway> giveaways)
        {
            var count = 0;
            var pages = 1;
            for (var i = 0; i < pages; i++)
            {
                var response = Web.Get("https://steamcompanion.com",
                    i == 0 ? "/gifts/search/?type=group" : "/gifts/search/?type=group&page=" + (i + 1),
                    new List<Parameter>(),
                    Generate.Cookies_SteamCompanion(bot), new List<HttpHeader>());

                if (response.RestResponse.Content != null)
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(response.RestResponse.Content);

                    var pageNode = htmlDoc.DocumentNode.SelectSingleNode("//li[@class='arrow']/a[1]");
                    if (pageNode != null)
                    {
                        try
                        {
                            pages = int.Parse(pageNode.Attributes["href"].Value.Split('=')[1].Split('&')[0]);
                        }
                        catch (FormatException)
                        {
                            pages = int.Parse(pageNode.Attributes["href"].Value.Split('=')[2].Split('&')[0]);
                        }
                    }

                    var nodes = htmlDoc.DocumentNode.SelectNodes("//section[@class='col-2-3']/div");
                    if (nodes != null)
                    {
                        for (var j = 0; j < nodes.Count; j++)
                        {
                            if (nodes[j].Attributes["style"] != null &&
                                nodes[j].Attributes["style"].Value == "opacity: 0.5;")
                            {
                                nodes.Remove(nodes[j]);
                                j--;
                            }
                        }
                        count += nodes.Count;
                        if (nodes.Count > 0)
                        {
                            SteamCompanionAddGiveaways(nodes, bot, giveaways);
                        }
                    }
                }
            }

            return
                $"{GetDateTime()} {{SteamCompanion}} {strings.ParseLoadGiveaways_Found} {count} {strings.ParseLoadGiveaways_GroupGiveAwaysIn} {pages} " +
                $"{strings.ParseLoadGiveaways_Pages} \n";
        }

        private static void SteamCompanionAddGiveaways(HtmlNodeCollection nodes, Bot bot,
            List<SteamCompanion.ScGiveaway> giveaways)
        {
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var name = node.SelectNodes(".//p[@class='game-name']/a/span").Count > 1
                        ? node.SelectSingleNode(".//p[@class='game-name']/a/span[2]")
                        : node.SelectSingleNode(".//p[@class='game-name']/a/span[1]");
                    var price = node.SelectSingleNode(".//p[@class='game-name']/a");

                    if (price != null && name != null)
                    {
                        var scGiveaway = new SteamCompanion.ScGiveaway
                        {
                            Name = name.InnerText,
                            Price = int.Parse(price.InnerText.Replace("p)", "").Split('(')[
                                node.SelectSingleNode(".//p[@class='game-name']")
                                    .InnerText.Replace("p)", "")
                                    .Split('(')
                                    .Length - 1]),
                            Link = node.SelectSingleNode(".//p[@class='game-name']/a").Attributes["href"].Value
                        };

                        var region = node.SelectSingleNode(".//span[@class='icon-region']");
                        if (region != null)
                        {
                            scGiveaway.Region = true;
                        }

                        if (scGiveaway.Price <= bot.SteamCompanion.Points &&
                            scGiveaway.Price <= bot.SteamCompanion.JoinPointLimit)
                        {
                            giveaways?.Add(scGiveaway);
                        }
                    }
                }
            }
        }

        public static Log SteamCompanionGetJoinData(SteamCompanion.ScGiveaway scGiveaway, Bot bot)
        {
            var response = Web.Get(scGiveaway.Link, "",
                new List<Parameter>(), Generate.Cookies_SteamCompanion(bot),
                new List<HttpHeader>());

            if (response.RestResponse.Content != null)
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var storId = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='banner large-5 columns']");
                var code = htmlDoc.DocumentNode.SelectSingleNode($"//div[@data-points='{scGiveaway.Price}']");
                var giftId = htmlDoc.DocumentNode.SelectSingleNode("//input[@name='giftID']");
                if (storId != null && code != null && giftId != null)
                {
                    scGiveaway.StoreId = storId.Attributes["href"].Value.Split('/')[4];
                    scGiveaway.Code = code.Attributes["data-hashid"].Value;
                    return new Log("", Color.White, true, false);
                }

                var group =
                    htmlDoc.DocumentNode.SelectSingleNode("//a[@class='notification group-join regular-button qa']");
                if (group != null)
                {
                    if (bot.SteamCompanion.AutoJoin)
                    {
                        var trueGroupUrl = Web.Get(group.Attributes["href"].Value, "", new List<Parameter>(),
                            Generate.Cookies_Steam(bot),
                            new List<HttpHeader>());

                        return Web.SteamJoinGroup(trueGroupUrl.RestResponse.ResponseUri.AbsoluteUri, "",
                            Generate.PostData_SteamGroupJoin(bot.Steam.Cookies.Sessid),
                            Generate.Cookies_Steam(bot), new List<HttpHeader>());
                    }
                    var error =
                        htmlDoc.DocumentNode.SelectSingleNode("//a[@class='notification group-join regular-button qa']");
                    return
                        new Log(
                            $"{GetDateTime()} {{SteamCompanion}} {strings.GiveawayJoined_Join} \"{scGiveaway.Name}\" {strings.GiveawayNotJoined_NotConfirmed} " +
                            $"{strings.GiveawayNotJoined_YouMustEnteredToGroup} {{{(error == null ? "Error" : error.Attributes["href"].Value)}}}",
                            Color.Yellow, false, true);
                }

                var exception =
                    htmlDoc.DocumentNode.SelectSingleNode("//a[@class='notification regular-button']").InnerText;

                if (exception != null)
                {
                    return GiveawayNotJoined("SteamCompanion", scGiveaway.Name, exception);
                }
            }
            return GiveawayNotJoined("SteamCompanion", scGiveaway.Name, "Content is empty");
        }

        #endregion

        #region UseGamble

        private static Log UseGambleGetProfile(Bot bot)
        {
            var response = Web.Get("http://usegamble.com/", "", new List<Parameter>(),
                Generate.Cookies_UseGamble(bot),
                new List<HttpHeader>());

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var points = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='my_coins']");
                var profileLink = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='mp-wrap']/a[1]");
                if (points != null && profileLink != null)
                {
                    bot.UseGamble.Points = int.Parse(points.InnerText);
                    bot.UseGamble.ProfileLink = "http://usegamble.com" + profileLink.Attributes["href"].Value;
                    return ParseProfile("UseGamble", bot.UseGamble.Points, profileLink.InnerText);
                }
            }
            return ParseProfileFailed("UseGamble");
        }

        public static async Task<Log> UseGambleGetProfileAsync(Bot bot)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = UseGambleGetProfile(bot);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        private static Log UsegambleWonParse(Bot bot)
        {
            var response = Web.Get("http://usegamble.com/", "profile/logs", new List<Parameter>(),
                Generate.Cookies_UseGamble(bot), new List<HttpHeader>());

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var nodes = htmlDoc.DocumentNode.SelectNodes("//tr[@class='gray']");
                if (nodes != null)
                {
                    for (var i = 0; i < nodes.Count; i++)
                    {
                        var content = nodes[i].SelectSingleNode("//tr/td[2]").InnerText;
                        if (!content.Contains("you've won the Giveaway"))
                        {
                            nodes.Remove(nodes[i]);
                            i--;
                        }
                    }
                    return GiveawayHaveWon("UseGamble", nodes.Count, "http://usegamble.com/profile/logs");
                }
            }
            return null;
        }

        public static async Task<Log> UseGambleWonParsAsync(Bot bot)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = UsegambleWonParse(bot);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        private static Log UseGambleLoadGiveaways(Bot bot, List<UseGamble.UgGiveaway> giveaways,
            Blacklist blackList)
        {
            giveaways?.Clear();

            var pages = 1;

            for (var i = 0; i < pages; i++)
            {
                if (pages != 1)
                {
                    var headerList = new List<HttpHeader>();
                    var header = new HttpHeader
                    {
                        Name = "X-Requested-With",
                        Value = "XMLHttpRequest"
                    };
                    headerList.Add(header);

                    var jsonresponse = Web.Post("http://usegamble.com/", "page/ga_page",
                        Generate.PageData_UseGamble(i + 1), headerList,
                        Generate.Cookies_UseGamble(bot));
                    if (jsonresponse.RestResponse.Content != "")
                    {
                        var data = jsonresponse.RestResponse.Content.Replace("\\", "");
                        var htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(data);

                        var nodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='giveaway_container']");
                        UseGambleAddGiveaways(nodes, bot, giveaways);
                    }
                }
                else
                {
                    var response = Web.Get("http://usegamble.com/", "page",
                        new List<Parameter>(), Generate.Cookies_UseGamble(bot),
                        new List<HttpHeader>());

                    if (response.RestResponse.Content != "")
                    {
                        var htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(response.RestResponse.Content);

                        var count =
                            htmlDoc.DocumentNode.SelectNodes("//div[@class='nPagin']//div[@class='pagin']/span");
                        if (count != null)
                        {
                            pages = int.Parse(htmlDoc.DocumentNode.
                                SelectSingleNode($"//div[@class='nPagin']//div[@class='pagin']/span[{count.Count - 1}]")
                                .InnerText);
                        }

                        var nodes =
                            htmlDoc.DocumentNode.SelectNodes("//div[@id='normal']/div[@class='giveaway_container']");
                        UseGambleAddGiveaways(nodes, bot, giveaways);
                    }
                }
            }

            if (giveaways == null)
            {
                return
                    new Log(
                        $"{GetDateTime()} {{UseGamble}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: 0",
                        Color.White, true, true);
            }

            if (blackList.Items != null)
            {
                for (var i = 0; i < giveaways.Count; i++)
                {
                    if (blackList.Items.Any(item => giveaways[i].StoreId == item.Id))
                    {
                        giveaways.Remove(giveaways[i]);
                        i--;
                    }
                }
            }

            return
                new Log(
                    $"{GetDateTime()} {{UseGamble}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: {giveaways.Count}",
                    Color.White, true, true);
        }

        public static async Task<Log> UseGambleLoadGiveawaysAsync(Bot bot,
            List<UseGamble.UgGiveaway> giveaways, Blacklist blackList)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = UseGambleLoadGiveaways(bot, giveaways, blackList);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        private static void UseGambleAddGiveaways(HtmlNodeCollection nodes, Bot bot,
            List<UseGamble.UgGiveaway> giveaways)
        {
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var name = node.SelectSingleNode(".//div[@class='giveaway_name']");
                    var storeId = node.SelectSingleNode(".//a[@class='steam-icon']");
                    if (name != null && storeId != null)
                    {
                        var spGiveaway = new UseGamble.UgGiveaway
                        {
                            Name = name.InnerText,
                            StoreId = storeId.Attributes["href"].Value.Split('/')[4]
                        };

                        var price = node.SelectSingleNode(".//span[@class='coin-white-icon']");
                        var code = node.SelectSingleNode(".//div[@class='ga_join_btn ga_coin_join']");
                        if (price != null && code != null)
                        {
                            spGiveaway.Price = int.Parse(price.InnerText);
                            spGiveaway.Code = code.Attributes["onclick"].Value.Split('\'')[5].Replace("ga:", "");

                            var iconsBlock = node.SelectSingleNode(".//div[@class='giveaway_iconbar']");
                            var icons = iconsBlock?.SelectNodes(".//span");
                            if (icons != null)
                            {
                                foreach (var icon in icons)
                                {
                                    if (icon.Attributes["class"].Value.Contains("region"))
                                    {
                                        spGiveaway.Region = icon.Attributes["class"].Value.Split('-')[1];
                                    }
                                }
                            }

                            if (spGiveaway.Price <= bot.UseGamble.Points &&
                                spGiveaway.Price <= bot.UseGamble.MaxJoinValue)
                            {
                                giveaways?.Add(spGiveaway);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region SteamTrade

        private static Log SteamTradeGetProfile(Bot bot)
        {
            var response = Web.Get("http://steamtrade.info/", "", new List<Parameter>(),
                Generate.Cookies_SteamTrade(bot),
                new List<HttpHeader>());

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var test = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='topm1']");
                if (test != null)
                {
                    return ParseProfile("SteamTrade", test.InnerText);
                }
            }
            return ParseProfileFailed("SteamTrade");
        }

        public static async Task<Log> SteamTradeGetProfileAsync(Bot bot)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = SteamTradeGetProfile(bot);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        private static Log SteamTradeLoadGiveaways(Bot bot, List<SteamTrade.StGiveaway> giveaways,
            Blacklist blackList)
        {
            giveaways?.Clear();

            var response = Web.Get("http://steamtrade.info/", "awards/", new List<Parameter>(),
                Generate.Cookies_SteamTrade(bot),
                new List<HttpHeader>());

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var nodes = htmlDoc.DocumentNode.SelectNodes("//tbody[@bgcolor='#F3F5F7']/tr");
                SteamTradeAddGiveaways(nodes, giveaways);

                if (giveaways == null)
                {
                    return
                        new Log(
                            $"{GetDateTime()} {{SteamTrade}} {strings.ParseLoadGiveaways_FoundMatchGiveaways} : 0",
                            Color.White, true, true);
                }

                if (blackList != null)
                {
                    for (var i = 0; i < giveaways.Count; i++)
                    {
                        if (blackList.Items.Any(item => giveaways[i].StoreId == item.Id))
                        {
                            giveaways.Remove(giveaways[i]);
                            i--;
                        }
                    }
                }

                return
                    new Log(
                        $"{GetDateTime()} {{SteamTrade}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: {giveaways.Count}",
                        Color.White, true, true);
            }
            return null;
        }

        public static async Task<Log> SteamTradeLoadGiveawaysAsync(Bot bot,
            List<SteamTrade.StGiveaway> giveaways, Blacklist blackList)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = SteamTradeLoadGiveaways(bot, giveaways, blackList);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        private static void SteamTradeAddGiveaways(HtmlNodeCollection nodes, List<SteamTrade.StGiveaway> giveaways)
        {
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    if (node.SelectSingleNode(".//span[@class='status1']") == null)
                    {
                        var name = node.SelectSingleNode(".//td[1]/a[2]");
                        var link = node.SelectSingleNode(".//td[1]/a[2]");
                        var storeId = node.SelectSingleNode(".//td[1]/a[1]");
                        if (name != null && link != null && storeId != null)
                        {
                            var spGiveaway = new SteamTrade.StGiveaway
                            {
                                Name = node.SelectSingleNode(".//td[1]/a[2]").InnerText,
                                Link = node.SelectSingleNode(".//td[1]/a[2]").Attributes["href"].Value,
                                StoreId = node.SelectSingleNode(".//td[1]/a[1]").Attributes["href"].Value.Split('/')[4]
                            };
                            giveaways?.Add(spGiveaway);
                        }
                    }
                }
            }
        }

        public static SteamTrade.StGiveaway SteamTradeGetJoinData(SteamTrade.StGiveaway stGiveaway, Bot bot)
        {
            var response = Web.Get("http://steamtrade.info", stGiveaway.Link,
                new List<Parameter>(), Generate.Cookies_SteamTrade(bot),
                new List<HttpHeader>());

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var linkJoin = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='inv_join']");
                if (linkJoin != null)
                {
                    stGiveaway.LinkJoin = linkJoin.Attributes["href"].Value.Trim();
                }
            }
            return stGiveaway;
        }

        #endregion

        #region PlayBlink

        private static Log PlayBlinkGetProfile(Bot bot)
        {
            var response = Web.Get("http://playblink.com/", "", new List<Parameter>(),
                Generate.Cookies_PlayBlink(bot), new List<HttpHeader>());

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var points = htmlDoc.DocumentNode.SelectSingleNode("//td[@id='points']");
                var level = htmlDoc.DocumentNode.SelectSingleNode("//a[@title='Your contribution level']/b");
                var username = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='usr_link']");
                if (points != null && level != null && username != null)
                {
                    bot.PlayBlink.Points = int.Parse(points.InnerText.Split('P')[0].Split('\n')[1].Trim());
                    bot.PlayBlink.Level = int.Parse(level.InnerText);

                    ProfileLoaded();
                    return ParseProfile("PlayBlink", bot.PlayBlink.Points, bot.PlayBlink.Level, username.InnerText);
                }
            }
            return ParseProfileFailed("PlayBlink");
        }

        public static async Task<Log> PlayBlinkGetProfileAsync(Bot bot)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = PlayBlinkGetProfile(bot);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        private static Log PlayBlinkLoadGiveaways(Bot bot, List<PlayBlink.PbGiveaway> giveaways,
            Blacklist blackList)
        {
            giveaways?.Clear();

            var response = Web.Get("http://playblink.com/", "", new List<Parameter>(),
                Generate.Cookies_PlayBlink(bot),
                new List<HttpHeader>());

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var nodes = htmlDoc.DocumentNode.SelectNodes("//div[@id='games_free']/div[@class='game_box']");
                if (nodes != null)
                {
                    PlayBlinkAddGiveaways(nodes, giveaways);
                }

                if (giveaways == null)
                {
                    return
                        new Log(
                            $"{GetDateTime()} {{PlayBlink}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: 0",
                            Color.White, true, true);
                }

                if (blackList.Items != null)
                {
                    for (var i = 0; i < giveaways.Count; i++)
                    {
                        if (blackList.Items.Any(item => giveaways[i].StoreId == item.Id))
                        {
                            giveaways.Remove(giveaways[i]);
                            i--;
                        }
                    }
                }
            }
            return
                new Log(
                    $"{GetDateTime()} {{PlayBlink}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: {giveaways?.Count}",
                    Color.White, true, true);
        }

        public static async Task<Log> PlayBlinkLoadGiveawaysAsync(Bot bot,
            List<PlayBlink.PbGiveaway> giveaways, Blacklist blackList)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = PlayBlinkLoadGiveaways(bot, giveaways, blackList);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        private static void PlayBlinkAddGiveaways(HtmlNodeCollection nodes, List<PlayBlink.PbGiveaway> giveaways)
        {
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    if (node.SelectSingleNode(".//a[@class='button grey']") == null)
                    {
                        var level = node.SelectSingleNode(".//div[@class='min_level tooltip']");
                        var name = node.SelectSingleNode(".//div[@class='name']/div");
                        var storeId = node.SelectSingleNode("//div[@class='description']/a");
                        var id = node.SelectSingleNode(".//a[@class='blink button blue']");
                        if (level != null && name != null && storeId != null && id != null)
                        {
                            var pbGiveaway = new PlayBlink.PbGiveaway
                            {
                                Level = int.Parse(level.InnerText.Replace("L", "")),
                                Name = name.InnerText,
                                StoreId = storeId.Attributes["href"].Value.Split('/')[4],
                                Id = id.Attributes["id"].Value.Replace("blink_", "")
                            };

                            var price =
                                node.SelectSingleNode(".//div[@class='stats']/table/tr[3]/td/div[2]") ??
                                node.SelectSingleNode(".//div[@class='stats']/table/tr[3]");

                            pbGiveaway.Price =
                                int.Parse(price.InnerText.Replace("Point(s)", "").Replace("Entrance Fee:", "").Trim());
                            giveaways?.Add(pbGiveaway);
                        }
                    }
                }
            }
        }

        #endregion
    }
}