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
using static KryBot.Tools;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace KryBot
{                                                                                                                                                     
    public class Parse
    {
        #region Steam
        public static Classes.Log SteamGetProfile(Classes.Bot bot, bool echo)
        {
            var response = Web.Get("http://steamcommunity.com/", "", new List<Parameter>(),
                Generate.Cookies_Steam(bot), new List<HttpHeader>(),
                bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var login = htmlDoc.DocumentNode.SelectSingleNode("//a[contains(@class, 'user_avatar') and contains(@class, 'playerAvatar')]");
                if (login == null)
                {
                    ProfileLoaded();
                    return ParseProfile("Steam", false);
                }

                bot.SteamProfileLink = login.Attributes["href"].Value;
                return ParseProfile("Steam", true);
            }
            return ParseProfile("Steam", false);
        }

        public static async Task<Classes.Log> SteamGetProfileAsync(Classes.Bot bot, bool echo)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = SteamGetProfile(bot, echo);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static async Task<List<string>> SteamGetUserGames(string profileLink)
        {
            var responseXmlProfile = await Web.GetAsync($"{profileLink}?xml=1", "", new List<Parameter>(), new CookieContainer(), new List<HttpHeader>(), "");
            var serializer = new XmlSerializer(typeof(Classes.Profile));
            TextReader reader = new StringReader(responseXmlProfile.RestResponse.Content);
            var userId64 = (Classes.Profile)serializer.Deserialize(reader);
            var responseJsonGames = await Web.GetAsync($"http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=5A5D4A6B747FC6020B4E898710665F29&steamid={userId64.SteamID64}&format=json", 
                "", new List<Parameter>(), new CookieContainer(), new List<HttpHeader>(), "");
            if (!responseJsonGames.RestResponse.Content.Contains("Internal Server Error"))
            {
                var json = JsonConvert.DeserializeObject<Classes.OwnedGames>(responseJsonGames.RestResponse.Content);
                return json.response.games.Select(game => game.appid).ToList();
            }
            return new List<string>();
        } 
        #endregion

        #region GameMiner
        public static Classes.Log GameMinerGetProfile(Classes.Bot bot, bool echo)
        {
            var response = Web.Get("http://gameminer.net/", "", new List<Parameter>(),
                Generate.Cookies_GameMiner(bot), new List<HttpHeader>(),
                bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var coal = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='user__coal']");
                var level = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='g-level-icon']");

                if (coal != null && level != null)
                {
                    bot.GameMinerCoal = int.Parse(coal.InnerText);
                    bot.GameMinerLevel = int.Parse(level.InnerText);

                    ProfileLoaded();
                    return ParseProfile("GameMiner", bot.GameMinerCoal, bot.GameMinerLevel);
                }

                //var error = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='notice-top label label-error label-big']");
                //if (error != null)
                //{
                //    bot.GameMinerEnabled = false;
                //    bot.GameMinerxsrf = "";
                //    bot.GameMinerToken = "";
                //    SaveSettings(bot, "");
                //    return
                //    ConstructLog($"{GetDateTime()} {{GameMiner}} {strings.AccountNotActive}", Color.Red, false, true);
                //}
            }
            ProfileLoaded();
            return ParseProfile("GameMiner", false);
        }

        public static async Task<Classes.Log> GameMinerGetProfileAsync(Classes.Bot bot, bool echo)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = GameMinerGetProfile(bot, echo);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Log GameMinerWonParse(Classes.Bot bot)
        {
            var response = Web.Get("http://gameminer.net/", "giveaways/won", new List<Parameter>(),
                Generate.Cookies_GameMiner(bot), new List<HttpHeader>(),
                bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var nodes =
                    htmlDoc.DocumentNode.SelectNodes(
                        "//tbody[@class='giveaways__giveaways']//td[@class='valign-middle m-table-state-finished']");

                if (nodes != null)
                {
                    for (int i = 0; i < nodes.Count; i++)
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

        public static async Task<Classes.Log> GameMinerWonParseAsync(Classes.Bot bot)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = GameMinerWonParse(bot);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Log GameMinerLoadGiveaways(Classes.Bot bot, List<GameMiner.GmGiveaway> giveaways,
            string[] blackList)
        {
            var content = "";
            giveaways?.Clear();
            int pages = 0;

            if (bot.GameMinerFreeGolden)
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
                        $"{GetDateTime()} {{GameMiner}} {strings.ParseLoadGiveaways_Found} {goldenFreeJsonResponse.Total} " +
                        $"{strings.ParseLoadGiveaways_FreeGoldenGiveawaysIn} {goldenFreeJsonResponse.Last_Page} {strings.ParseLoadGiveaways_Pages}\n";

                    pages = goldenFreeJsonResponse.Last_Page;

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

            if (bot.GameMinerRegular)
            {
                var regularResponse = Web.Get("http://gameminer.net",
                    $"/api/giveaways/coal?page=1&count=10&q=&type={(bot.GameMinerOnlyGifts ? "regular" : "any")}&enter_price=on&sortby=finish&order=asc&filter_entered=on",
                    new List<Parameter>(),
                    Generate.Cookies_GameMiner(bot),
                    new List<HttpHeader>(), bot.UserAgent);

                var regularJsonResponse =
                    JsonConvert.DeserializeObject<GameMiner.JsonRootObject>(
                        regularResponse.RestResponse.Content);
                GameMinerAddGiveaways(regularJsonResponse, bot, giveaways);
                content +=
                    $"{GetDateTime()} {{GameMiner}} {strings.ParseLoadGiveaways_Found} {regularJsonResponse.Total} " +
                    $"{strings.ParseLoadGiveaways_RegularGiveawaysIn} {regularJsonResponse.Last_Page} {strings.ParseLoadGiveaways_Pages}\n";

                pages = regularJsonResponse.Last_Page;

                if (pages > 1)
                {
                    for (var i = 1; i < pages + 1; i++)
                    {
                        var regularGiftsResponse = Web.Get("http://gameminer.net/",
                            $"/api/giveaways/coal?page={i + 1}&count=10&q=&type={(bot.GameMinerOnlyGifts ? "regular" : "any")}&enter_price=on&sortby=finish&order=asc&filter_entered=on",
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

            if (bot.GameMinerSandbox)
            {
                var sandboxGiftsResponse = Web.Get("http://gameminer.net",
                    $"/api/giveaways/sandbox?page=1&count=10&q=&type={(bot.GameMinerOnlyGifts ? "regular" : "any")}&enter_price=on&sortby=finish&order=asc&filter_entered=on",
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
                        $"{GetDateTime()} {{GameMiner}} {strings.ParseLoadGiveaways_Found} {sandboxGiftJsonResponse.Total} " +
                        $"{strings.ParseLoadGiveaways_RegularGiveawaysIn} {sandboxGiftJsonResponse.Last_Page} {strings.ParseLoadGiveaways_Pages}\n";

                    pages = sandboxGiftJsonResponse.Last_Page;
                }

                if (pages > 1)
                {
                    for (var i = 1; i < pages + 1; i++)
                    {
                        var regularGiftsResponse = Web.Get("http://gameminer.net/",
                            $"/api/giveaways/sandbox?page={i + 1}&count=10&q=&type={(bot.GameMinerOnlyGifts ? "regular" : "any")}&enter_price=on&sortby=finish&order=asc&filter_entered=on",
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
                    ConstructLog(
                        $"{content}{GetDateTime()} {{GameMiner}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: 0",
                        Color.White, true, true);
            }

            if (blackList != null)
            {
                for (int i = 0; i < giveaways.Count; i++)
                {
                    foreach (var id in blackList)
                    {
                        if (giveaways[i].StoreId == id)
                        {
                            giveaways.Remove(giveaways[i]);
                            i--;
                            break;
                        }
                    }
                }
            }

            return
                ConstructLog(
                    $"{content}{GetDateTime()} {{GameMiner}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: {giveaways.Count}",
                    Color.White, true, true);
        }

        public static async Task<Classes.Log> GameMinerLoadGiveawaysAsync(Classes.Bot bot,
            List<GameMiner.GmGiveaway> giveaways, string[] blackList)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = GameMinerLoadGiveaways(bot, giveaways, blackList);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        private static void GameMinerAddGiveaways(GameMiner.JsonRootObject json, Classes.Bot bot,
            List<GameMiner.GmGiveaway> giveaways)
        {
            if (json != null)
                foreach (var giveaway in json.Giveaways)
                {
                    var lot = new GameMiner.GmGiveaway();
                    if (giveaway.Golden && giveaway.Price != 0)
                    {
                        break;
                    }

                    if (lot.Price > bot.GameMinerCoal || lot.Price > bot.GameMinerJoinCoalLimit)
                    {
                        break;
                    }

                    lot.Name = giveaway.Game.Name;
                    lot.Id = giveaway.Code;
                    lot.IsRegular = giveaway.Sandbox == null;
                    lot.IsSandbox = giveaway.Sandbox != null;
                    lot.IsGolden = giveaway.Golden;
                    lot.Page = json.Page;
                    lot.Price = giveaway.Price;

                    if (giveaway.regionlock_type_id != null)
                    {
                        lot.Region = giveaway.regionlock_type_id;
                    }

                    if (giveaway.Game.Url != "javascript:void(0);")
                    {
                        lot.StoreId = giveaway.Game.Url.Split('/')[4];
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
        public static Classes.Log SteamGiftsGetProfile(Classes.Bot bot, bool echo)
        {
            var response = Web.Get("http://www.steamgifts.com/", "", new List<Parameter>(),
                Generate.Cookies_SteamGifts(bot), new List<HttpHeader>(), bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var points = htmlDoc.DocumentNode.SelectSingleNode("//a[@href='/account']/span[1]");
                var level = htmlDoc.DocumentNode.SelectSingleNode("//a[@href='/account']/span[2]");

                if (points != null && level != null)
                {
                    bot.SteamGiftsPoint = int.Parse(points.InnerText);
                    bot.SteamGiftsLevel = int.Parse(level.InnerText.Split(' ')[1]);
                    return ParseProfile("SteamGifts", bot.SteamGiftsPoint, bot.SteamGiftsLevel);
                }

                var error = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='notification notification--warning']");
                if (error != null)
                {
                    bot.SteamGiftsEnabled = false;
                    bot.GameMinerxsrf = "";
                    SaveProfile(bot, "");
                    return ConstructLog($"{GetDateTime()} {{SteamGifts}} {strings.AccountNotActive} {{{error.InnerText}}}", Color.Red, false, true);
                }
            }
            return ParseProfile("SteamGifts", false);
        }

        public static async Task<Classes.Log> SteamGiftsGetProfileAsync(Classes.Bot bot, bool echo)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = SteamGiftsGetProfile(bot, echo);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Log SteamGiftsWonParse(Classes.Bot bot)
        {
            var response = Web.Get("http://www.steamgifts.com/", "", new List<Parameter>(),
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
                        "http://www.steamgifts.com/giveaways/won");
                }
            }
            return null;
        }

        public static async Task<Classes.Log> SteamGiftsWonParseAsync(Classes.Bot bot)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = SteamGiftsWonParse(bot);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Log SteamGiftsLoadGiveaways(Classes.Bot bot, List<SteamGifts.SgGiveaway> giveaways,
            List<SteamGifts.SgGiveaway> wishlistGiveaways, string[] blackList)
        {
            var content = "";
            giveaways?.Clear();

            if (bot.SteamGiftsWishList)
            {
                content += SteamGiftsLoadWishListGiveaways(bot, wishlistGiveaways);
            }

            if (bot.SteamGiftsGroup)
            {
                content += SteamGiftsLoadGroupGiveaways(bot, giveaways);
            }

            if (bot.SteamGiftsRegular)
            {
                var response = Web.Get("http://www.steamgifts.com", "/giveaways/search?page=1",
                    new List<Parameter>(), Generate.Cookies_SteamGifts(bot),
                    new List<HttpHeader>(),
                    bot.UserAgent);

                if (response.RestResponse.Content != "")
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(response.RestResponse.Content);

                    int pages = 1;
                    var pageNodeCouner = htmlDoc.DocumentNode.SelectNodes("//div[@class='pagination__navigation']/a");
                    if (pageNodeCouner != null)
                    {
                        var pageNode =
                            htmlDoc.DocumentNode.SelectSingleNode($"//div[@class='pagination__navigation']/a[{pageNodeCouner.Count - 1}]");
                        if (pageNode != null)
                        {
                            pages = int.Parse(pageNode.Attributes["data-page-number"].Value);
                        }
                    }

                    if (pages != 1)
                    {
                        for (var i = 1; i < pages + 1; i++)
                        {
                            response = Web.Get("http://www.steamgifts.com", "/giveaways/search?page=" + (i + 1),
                                new List<Parameter>(), Generate.Cookies_SteamGifts(bot),
                                new List<HttpHeader>(),
                                bot.UserAgent);

                            if (response.RestResponse.Content != "")
                            {
                                htmlDoc = new HtmlDocument();
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
                    ConstructLog(
                        $"{content}{GetDateTime()} {{SteamGifts}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: 0",
                        Color.White, true, true);
            }

            if (blackList != null)
            {
                for (int i = 0; i < giveaways?.Count; i++)
                {
                    foreach (var id in blackList)
                    {
                        if (giveaways[i].StoreId == id)
                        {
                            giveaways.Remove(giveaways[i]);
                            i--;
                            break;
                        }
                    }
                }
            }

            return
                ConstructLog(
                    $"{content}{GetDateTime()} {{SteamGifts}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: {giveaways?.Count + wishlistGiveaways.Count}",
                    Color.White, true, true);
        }

        public static async Task<Classes.Log> SteamGiftsLoadGiveawaysAsync(Classes.Bot bot,
            List<SteamGifts.SgGiveaway> giveaways, List<SteamGifts.SgGiveaway> wishlistGiveaways, string[] blackList)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = SteamGiftsLoadGiveaways(bot, giveaways, wishlistGiveaways, blackList);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static string SteamGiftsLoadWishListGiveaways(Classes.Bot bot, List<SteamGifts.SgGiveaway> giveaways)
        {
            var nodesCount = 0;
            int pages = 1;

            var response = Web.Get("http://www.steamgifts.com", "/giveaways/search?type=wishlist",
                new List<Parameter>(), Generate.Cookies_SteamGifts(bot), new List<HttpHeader>(),
                bot.UserAgent);

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
                        var pageNode = htmlDoc.DocumentNode.SelectSingleNode($"//div[@class='pagination__navigation']/a[{pageNodeCounter.Count - 1}]");
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
            return $"{GetDateTime()} {{SteamGifts}} {strings.ParseLoadGiveaways_FoundGiveAwaysInWishList}: {nodesCount}\n";
        }

        public static string SteamGiftsLoadGroupGiveaways(Classes.Bot bot, List<SteamGifts.SgGiveaway> giveaways)
        {
            var nodesCount = 0;

            var response = Web.Get("http://www.steamgifts.com", "/giveaways/search?type=group",
                new List<Parameter>(), Generate.Cookies_SteamGifts(bot), new List<HttpHeader>(),
                bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                int pages = 1;

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

        private static void SteamGiftsAddGiveaways(HtmlNodeCollection nodes, Classes.Bot bot,
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

                        if (sgGiveaway.Price <= bot.SteamGiftsPoint &&
                            sgGiveaway.Price <= bot.SteamGiftsJoinPointLimit &&
                            sgGiveaway.Level >= bot.SteamGiftsMinLevel)
                        {
                            giveaways?.Add(sgGiveaway);
                        }
                    }
                }
            }
        }

        public static SteamGifts.SgGiveaway SteamGiftsGetJoinData(SteamGifts.SgGiveaway giveaway, Classes.Bot bot)
        {
            var response = Web.Get("http://www.steamgifts.com", giveaway.Link,
                new List<Parameter>(), Generate.Cookies_SteamGifts(bot), new List<HttpHeader>(), bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var code = htmlDoc.DocumentNode.SelectSingleNode("//input[@name='code']");
                var token = htmlDoc.DocumentNode.SelectSingleNode("//input[@name='xsrf_token']");
                if (code != null && token != null)
                {
                    giveaway.Code = code.Attributes["value"].Value;
                    giveaway.Token = token.Attributes["value"].Value;
                }
            }
            return giveaway;
        }
        #endregion

        #region SteamCompanion 
        public static Classes.Log SteamCompanionGetProfile(Classes.Bot bot, bool echo)
        {
            var response = Web.Get("https://steamcompanion.com", "/", new List<Parameter>(),
                Generate.Cookies_SteamCompanion(bot),
                new List<HttpHeader>(), bot.UserAgent);
            if (response.RestResponse.Content != "")
            {

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var points = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='points']");
                var profileLink = htmlDoc.DocumentNode.SelectSingleNode("//ul[@class='right']/li[1]/a[1]");
                if (points != null && profileLink != null)
                {
                    bot.SteamCompanionPoint = int.Parse(points.InnerText);
                    bot.SteamCompanionProfileLink = profileLink.Attributes["href"].Value;
                    return ParseProfile("SteamCompanion", bot.SteamCompanionPoint, -1);
                }
            }
            return ParseProfile("SteamCompanion", false);
        }

        public static async Task<Classes.Log> SteamCompanionGetProfileAsync(Classes.Bot bot, bool echo)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = SteamCompanionGetProfile(bot, echo);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Log SteamCompanionWonParse(Classes.Bot bot)
        {
            var response = Web.Get("https://steamcompanion.com/", "gifts/won", new List<Parameter>(),
                Generate.Cookies_SteamCompanion(bot), new List<HttpHeader>(),
                bot.UserAgent);
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

        public static async Task<Classes.Log> SteamCompanionWonParseAsync(Classes.Bot bot)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = SteamCompanionWonParse(bot);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Log SteamCompanionLoadGiveaways(Classes.Bot bot, List<SteamCompanion.ScGiveaway> giveaways,
            List<SteamCompanion.ScGiveaway> wishlistGiveaways, string[] blackList)
        {
            var content = "";
            giveaways?.Clear();

            if (bot.SteamCompanionWishList)
            {
                content += SteamCompanionLoadWishListGiveaways(bot, wishlistGiveaways);
            }

            if (bot.SteamCompanionGroup)
            {
                content += SteamCompanionLoadGroupGiveaways(bot, giveaways);
            }

            if (bot.SteamCompanionRegular)
            {
                var pages = 1;
                for (var i = 0; i < pages; i++)
                {
                    var response = Web.Get("https://steamcompanion.com",
                        i == 0 ? "/gifts/search/?type=public" : "/gifts/search.php?page=" + (i + 1) + "&type=public",
                        new List<Parameter>(),
                        Generate.Cookies_SteamCompanion(bot), new List<HttpHeader>(), bot.UserAgent);

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

                        var nodes = htmlDoc.DocumentNode.SelectNodes("//section[@class='col-2-3']/a");
                        if (nodes != null)
                        {
                            for (var j = 0; j < nodes.Count; j++)
                            {
                                if (nodes[j].Attributes["style"] != null && nodes[j].Attributes["style"].Value == "opacity: 0.5;")
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
                    ConstructLog(
                        $"{content}{GetDateTime()} {{SteamCompanion}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: 0",
                        Color.White, true, true);
            }

            //if (blackList != null)
            //{
            //    for (int i = 0; i < giveaways.Count; i++)
            //    {
            //        foreach (var id in blackList)
            //        {
            //            if (giveaways[i].StoreId == id)
            //            {
            //                giveaways.Remove(giveaways[i]);
            //                i--;
            //                break;
            //            }
            //        }
            //    }
            //}

            return
                ConstructLog(
                    $"{content}{GetDateTime()} {{SteamCompanion}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: {giveaways?.Count + wishlistGiveaways.Count}",
                    Color.White, true, true);
        }

        public static async Task<Classes.Log> SteamCompanionLoadGiveawaysAsync(Classes.Bot bot,
            List<SteamCompanion.ScGiveaway> giveaways, List<SteamCompanion.ScGiveaway> wishlistGiveaways, string[] blackList)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = SteamCompanionLoadGiveaways(bot, giveaways, wishlistGiveaways, blackList);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static string SteamCompanionLoadWishListGiveaways(Classes.Bot bot,
            List<SteamCompanion.ScGiveaway> giveaways)
        {
            var count = 0;
            var pages = 1;
            for (var i = 0; i < pages; i++)
            {
                var response = Web.Get("https://steamcompanion.com",
                    i == 0 ? "/gifts/search/?wishlist=true" : "/gifts/search/?wishlist=true&page=" + (i + 1),
                    new List<Parameter>(),
                    Generate.Cookies_SteamCompanion(bot), new List<HttpHeader>(), bot.UserAgent);

                if (response.RestResponse.Content != null)
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(response.RestResponse.Content);

                    var pageNode = htmlDoc.DocumentNode.SelectSingleNode("//li[@class='arrow']/a[1]");
                    if (pageNode != null)
                    {
                        pages = int.Parse(pageNode.Attributes["href"].Value.Split('=')[1]);
                    }

                    var nodes = htmlDoc.DocumentNode.SelectNodes("//section[@class='col-2-3']/a");
                    if (nodes != null)
                    {
                        for (var j = 0; j < nodes.Count; j++)
                        {
                            if (nodes[j].Attributes["style"] != null && nodes[j].Attributes["style"].Value == "opacity: 0.5;")
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
            return $"{GetDateTime()} {{SteamCompanion}} {strings.ParseLoadGiveaways_Found} {(giveaways.Count == 0 ? 0 : count)} {strings.ParseLoadGiveaways_WishListGiveAwaysIn} {pages} {strings.ParseLoadGiveaways_Pages}\n";
        }

        public static string SteamCompanionLoadGroupGiveaways(Classes.Bot bot, List<SteamCompanion.ScGiveaway> giveaways)
        {
            var count = 0;
            var pages = 1;
            for (var i = 0; i < pages; i++)
            {
                var response = Web.Get("https://steamcompanion.com",
                    i == 0 ? "/gifts/search/?type=group" : "/gifts/search/?type=group&page=" + (i + 1),
                    new List<Parameter>(),
                    Generate.Cookies_SteamCompanion(bot), new List<HttpHeader>(), bot.UserAgent);

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

                    var nodes = htmlDoc.DocumentNode.SelectNodes("//section[@class='col-2-3']/a");
                    if (nodes != null)
                    {
                        for (var j = 0; j < nodes.Count; j++)
                        {
                            if (nodes[j].Attributes["style"] != null && nodes[j].Attributes["style"].Value == "opacity: 0.5;")
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

            return $"{GetDateTime()} {{SteamCompanion}} {strings.ParseLoadGiveaways_Found} {count} {strings.ParseLoadGiveaways_GroupGiveAwaysIn} {pages} " +
                   $"{strings.ParseLoadGiveaways_Pages} \n";
        }

        private static void SteamCompanionAddGiveaways(HtmlNodeCollection nodes, Classes.Bot bot,
            List<SteamCompanion.ScGiveaway> giveaways)
        {
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var name = node.SelectNodes(".//p[@class='game-name']/span").Count > 1
                        ? node.SelectSingleNode(".//p[@class='game-name']/span[2]")
                        : node.SelectSingleNode(".//p[@class='game-name']/span[1]");
                    var price = node.SelectSingleNode(".//p[@class='game-name']");

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
                            Link = node.Attributes["href"].Value
                        };

                        var region = node.SelectSingleNode(".//span[@class='icon-region']");
                        if (region != null)
                        {
                            scGiveaway.Region = true;
                        }

                        if (scGiveaway.Price <= bot.SteamCompanionPoint &&
                            scGiveaway.Price <= bot.SteamCompanionJoinPointLimit)
                        {
                            giveaways?.Add(scGiveaway);
                        }
                    }
                }
            }
        }

        public static Classes.Log SteamCompanionGetJoinData(SteamCompanion.ScGiveaway giveaway, Classes.Bot bot)
        {
            var response = Web.Get(giveaway.Link, "",
                new List<Parameter>(), Generate.Cookies_SteamCompanion(bot),
                new List<HttpHeader>(), bot.UserAgent);

            if (response.RestResponse.Content != null)
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var storId = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='banner large-5 columns']");
                var code = htmlDoc.DocumentNode.SelectSingleNode($"//div[@data-points='{giveaway.Price}']");
                var giftId = htmlDoc.DocumentNode.SelectSingleNode("//input[@name='giftID']");
                if (storId != null && code != null && giftId != null)
                {
                    giveaway.StoreId = storId.Attributes["href"].Value.Split('/')[4];
                    giveaway.Code = code.Attributes["data-hashid"].Value;
                    return ConstructLog("", Color.AliceBlue, true, false);
                }

                var group =
                    htmlDoc.DocumentNode.SelectSingleNode("//a[@class='notification group-join regular-button qa']");
                if (@group != null)
                {
                    if (bot.SteamCompanionAutoJoin)
                    {
                        var trueGroupUrl = Web.Get(@group.Attributes["href"].Value, "", new List<Parameter>(),
                            Generate.Cookies_Steam(bot),
                            new List<HttpHeader>(), bot.UserAgent);

                        return Web.SteamJoinGroup(trueGroupUrl.RestResponse.ResponseUri.AbsoluteUri, "",
                            Generate.PostData_SteamGroupJoin(bot.SteamSessid),
                            Generate.Cookies_Steam(bot), new List<HttpHeader>(), bot.UserAgent);
                    }
                    var error =
                        htmlDoc.DocumentNode.SelectSingleNode("//a[@class='notification group-join regular-button qa']");
                    return
                        ConstructLog(
                            $"{GetDateTime()} {{SteamCompanion}} {strings.GiveawayJoined_Join} \"{giveaway.Name}\" {strings.GiveawayNotJoined_NotConfirmed} " +
                            $"{strings.GiveawayNotJoined_YouMustEnteredToGroup} {{{(error == null ? "Error" : error.Attributes["href"].Value)}}}",
                            Color.Yellow, false, true);
                }

                var exception =
                    htmlDoc.DocumentNode.SelectSingleNode("//a[@class='notification regular-button']").InnerText;

                if (exception != null)
                {
                    return GiveawayNotJoined("SteamCompanion", giveaway.Name, exception);
                }
            }
            return GiveawayNotJoined("SteamCompanion", giveaway.Name, "Content is empty");
        }
        #endregion

        #region SteamPortal
        public static Classes.Log SteamPortalGetProfile(Classes.Bot bot, bool echo)
        {
            var response = Web.Get("http://steamportal.net/", "", new List<Parameter>(),
                Generate.Cookies_SteamPortal(bot),
                new List<HttpHeader>(), bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var points = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='coin-icon my_coins']");
                var profileLink = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='my_profile']/a[1]");
                if (points != null && profileLink != null)
                {
                    bot.SteamPortalPoints = int.Parse(points.InnerText);
                    bot.SteamPortalProfileLink = "http://steamportal.net" + profileLink.Attributes["href"].Value;
                    return ParseProfile("SteamPortal", bot.SteamPortalPoints, -1);
                }
            }
            return ParseProfile("SteamPortal", false);
        }

        public static async Task<Classes.Log> SteamPortalGetProfileAsync(Classes.Bot bot, bool echo)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = SteamPortalGetProfile(bot, echo);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Log SteamPortalWonParse(Classes.Bot bot)
        {
            var response = Web.Get("http://steamportal.net/", "profile/logs", new List<Parameter>(),
                Generate.Cookies_SteamPortal(bot), new List<HttpHeader>(), bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var nodes = htmlDoc.DocumentNode.SelectNodes("//tr[@class='gray']");
                if (nodes != null)
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        var content = nodes[i].SelectSingleNode("//tr/td[2]").InnerText;
                        if (!content.Contains("you've won the Giveaway"))
                        {
                            nodes.Remove(nodes[i]);
                            i--;
                        }
                    }
                    return GiveawayHaveWon("SteamPortal", nodes.Count, "http://steamportal.net/profile/logs");
                }
            }
            return null;
        }

        public static async Task<Classes.Log> SteamPortalWonParsAsync(Classes.Bot bot)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = SteamPortalWonParse(bot);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Log SteamPortalLoadGiveaways(Classes.Bot bot, List<SteamPortal.SpGiveaway> giveaways,
            string[] blackList)
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

                    var jsonresponse = Web.Post("http://steamportal.net/", "page/ga_page",
                        Generate.PageData_SteamPortal(i + 1), headerList,
                        Generate.Cookies_SteamPortal(bot), bot.UserAgent);
                    if (jsonresponse.RestResponse.Content != "")
                    {
                        var data = jsonresponse.RestResponse.Content.Replace("\\", "");
                        var htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(data);

                        var nodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='giveaway_container']");
                        SteamPortalAddGiveaways(nodes, bot, giveaways);
                    }
                }
                else
                {
                    var response = Web.Get("http://steamportal.net/", "",
                        new List<Parameter>(), Generate.Cookies_SteamPortal(bot),
                        new List<HttpHeader>(),
                        bot.UserAgent);

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

                        var nodes = htmlDoc.DocumentNode.SelectNodes("//div[@id='normal']/div[@class='giveaway_container']");
                        SteamPortalAddGiveaways(nodes, bot, giveaways);
                    }
                }
            }

            if (giveaways == null)
            {
                return
                    ConstructLog(
                        $"{GetDateTime()} {{SteamPortal}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: 0",
                        Color.White, true, true);
            }

            if (blackList != null)
            {
                for (int i = 0; i < giveaways.Count; i++)
                {
                    foreach (var id in blackList)
                    {
                        if (giveaways[i].StoreId == id)
                        {
                            giveaways.Remove(giveaways[i]);
                            i--;
                            break;
                        }
                    }
                }
            }

            return
                ConstructLog(
                    $"{GetDateTime()} {{SteamPortal}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: {giveaways.Count}",
                    Color.White, true, true);
        }

        public static async Task<Classes.Log> SteamPortalLoadGiveawaysAsync(Classes.Bot bot,
            List<SteamPortal.SpGiveaway> giveaways, string[] blackList)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = SteamPortalLoadGiveaways(bot, giveaways, blackList);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        private static void SteamPortalAddGiveaways(HtmlNodeCollection nodes, Classes.Bot bot,
            List<SteamPortal.SpGiveaway> giveaways)
        {
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var name = node.SelectSingleNode(".//div[@class='giveaway_name']");
                    var storeId = node.SelectSingleNode(".//a[@class='steam-icon']");
                    if (name != null && storeId != null)
                    {
                        var spGiveaway = new SteamPortal.SpGiveaway
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

                            if (spGiveaway.Price <= bot.SteamPortalPoints &&
                                spGiveaway.Price <= bot.SteamPortalMaxJoinValue)
                            {
                                giveaways?.Add(spGiveaway);
                            }
                        }
                    }

                }
            }
        }
        #endregion

        #region GameAways
        public static bool GameAwaysProfile(Classes.Bot bot, bool echo)
        {

            var response = Web.Get("http://www.gameaways.com/", "", new List<Parameter>(),
                Generate.Cookies_GameAways(bot),
                new List<HttpHeader>(), bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                bot.GameAwaysPoints =
                    int.Parse(htmlDoc.DocumentNode.SelectSingleNode("//span[@class='ga-points']").InnerText);
                bot.GameAwaysProfileLink = "http://www.gameaways.com" +
                                           htmlDoc.DocumentNode.SelectSingleNode("//li[@class='user-menu-item'][2]/a[1]")
                                               .Attributes["href"].Value;
                if (echo)
                {
                    //Console.WriteLine(GetDateTime() + GetBotName(bot.Name) + @"{GameAways} " +
                    //                  strings.ParseProfile_Points + @": " + bot.GameAwaysPoints);
                }
                return true;
            }
            return false;
        }
        #endregion

        #region SteamTrade
        public static Classes.Log SteamTradeGetProfile(Classes.Bot bot, bool echo)
        {
            var response = Web.Get("http://steamtrade.info/", "", new List<Parameter>(),
                Generate.Cookies_SteamTrade(bot),
                new List<HttpHeader>(), bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var test = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='topm1']").Attributes["href"].Value;
                if (test != null)
                {
                    return ParseProfile("SteamTrade", true);
                }
            }
            return ParseProfile("SteamTrade", false);
        }

        public static async Task<Classes.Log> SteamTradeGetProfileAsync(Classes.Bot bot, bool echo)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                    var result = SteamTradeGetProfile(bot, echo);
                    task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Log SteamTradeLoadGiveaways(Classes.Bot bot, List<SteamTrade.StGiveaway> giveaways,
            string[] blackList)
        {
            giveaways?.Clear();

            var response = Web.Get("http://steamtrade.info/", "awards/", new List<Parameter>(),
                Generate.Cookies_SteamTrade(bot),
                new List<HttpHeader>(), bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var nodes = htmlDoc.DocumentNode.SelectNodes("//tbody[@bgcolor='#F3F5F7']/tr");
                SteamTradeAddGiveaways(nodes, giveaways);

                if (giveaways == null)
                {
                    return
                        ConstructLog(
                            $"{GetDateTime()} {{SteamTrade}} {strings.ParseLoadGiveaways_FoundMatchGiveaways} : 0",
                            Color.White, true, true);
                }

                if (blackList != null)
                {
                    for (int i = 0; i < giveaways.Count; i++)
                    {
                        foreach (var id in blackList)
                        {
                            if (giveaways[i].StoreId == id)
                            {
                                giveaways.Remove(giveaways[i]);
                                i--;
                                break;
                            }
                        }
                    }
                }

                return
                    ConstructLog(
                        $"{GetDateTime()} {{SteamTrade}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: {giveaways.Count}",
                        Color.White, true, true);
            }
            return null;
        }

        public static async Task<Classes.Log> SteamTradeLoadGiveawaysAsync(Classes.Bot bot,
            List<SteamTrade.StGiveaway> giveaways, string[] blackList)
        {
            var task = new TaskCompletionSource<Classes.Log>();
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

        public static SteamTrade.StGiveaway SteamTradeGetJoinData(SteamTrade.StGiveaway giveaway, Classes.Bot bot)
        {
            var response = Web.Get("http://steamtrade.info", giveaway.Link,
                new List<Parameter>(), Generate.Cookies_SteamTrade(bot),
                new List<HttpHeader>(), bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var linkJoin = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='inv_join']");
                if (linkJoin != null)
                {
                    giveaway.LinkJoin = linkJoin.Attributes["href"].Value.Trim();
                }
            }
            return giveaway;
        }
        #endregion

        #region PlayBlink
        public static Classes.Log PlayBlinkGetProfile(Classes.Bot bot, bool echo)
        {
            var response = Web.Get("http://playblink.com/", "", new List<Parameter>(),
                Generate.Cookies_PlayBlink(bot), new List<HttpHeader>(),
                bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var points = htmlDoc.DocumentNode.SelectSingleNode("//td[@id='points']");
                var level = htmlDoc.DocumentNode.SelectSingleNode("//a[@title='Your contribution level']/b");
                if (points != null && level != null)
                {
                    bot.PlayBlinkPoints = int.Parse(points.InnerText.Split('P')[0].Split('\n')[1].Trim());
                    bot.PlayBlinkLevel =int.Parse(level.InnerText);

                    ProfileLoaded();
                    return ParseProfile("PlayBlink", bot.PlayBlinkPoints, bot.PlayBlinkLevel);
                }
            }
            return ParseProfile("PlayBlink", false);
        }

        public static async Task<Classes.Log> PlayBlinkGetProfileAsync(Classes.Bot bot, bool echo)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = PlayBlinkGetProfile(bot, echo);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Log PlayBlinkLoadGiveaways(Classes.Bot bot, List<PlayBlink.PbGiveaway> giveaways,
            string[] blackList)
        {
            giveaways?.Clear();

            var response = Web.Get("http://playblink.com/", "", new List<Parameter>(),
                Generate.Cookies_PlayBlink(bot),
                new List<HttpHeader>(), bot.UserAgent);

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
                        ConstructLog(
                            $"{GetDateTime()} {{PlayBlink}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: 0",
                            Color.White, true, true);
                }

                if (blackList != null)
                {
                    for (int i = 0; i < giveaways.Count; i++)
                    {
                        foreach (var id in blackList)
                        {
                            if (giveaways[i].StoreId == id)
                            {
                                giveaways.Remove(giveaways[i]);
                                i--;
                                break;
                            }
                        }
                    }
                }
            }
            return
                ConstructLog(
                    $"{GetDateTime()} {{PlayBlink}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: {giveaways?.Count}",
                    Color.White, true, true);
        }

        public static async Task<Classes.Log> PlayBlinkLoadGiveawaysAsync(Classes.Bot bot,
            List<PlayBlink.PbGiveaway> giveaways, string[] blackList)
        {
            var task = new TaskCompletionSource<Classes.Log>();
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
                                Id = id.Attributes["id"].Value.Replace("blink_", ""),
                            };

                            var price = node.SelectSingleNode(".//div[@class='stats']/table/tr[3]/td/div[2]");
                            if (price == null)
                            {
                                price = node.SelectSingleNode(".//div[@class='stats']/table/tr[3]");
                            }
                            pbGiveaway.Price = int.Parse(price.InnerText.Replace("Point(s)", "").Replace("Entrance Fee:", "").Trim());
                            giveaways?.Add(pbGiveaway);
                        }
                    }
                }
            }
        }
        #endregion
    }
}