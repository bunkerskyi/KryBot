using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using KryBot.CommonResources.Localization;
using KryBot.Core.Cookies;
using KryBot.Core.Giveaways;
using KryBot.Core.Serializable.SteamGifts;
using Newtonsoft.Json;
using RestSharp;

namespace KryBot.Core.Sites
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class SteamGifts
    {
        public SteamGifts()
        {
            Cookies = new SteamGiftsCookie();
            Giveaways = new List<SteamGiftsGiveaway>();
            WishlistGiveaways = new List<SteamGiftsGiveaway>();
        }

        public bool Enabled { get; set; }
        public bool Group { get; set; }
        public bool Regular { get; set; } = true;
        public bool WishList { get; set; }
        public bool SortLevel { get; set; }
        public bool SortToLessLevel { get; set; }
        public bool SortToMoreCopies { get; set; }
        public int Points { get; set; }
        public int Level { get; set; }
        public int JoinPointLimit { get; set; } = 300;
        public int PointsReserv { get; set; }
        public int MinLevel { get; set; }
        public SteamGiftsCookie Cookies { get; set; }
        public List<SteamGiftsGiveaway> Giveaways { get; set; }
        public List<SteamGiftsGiveaway> WishlistGiveaways { get; set; }
        public string UserAgent { get; set; }

        public void Logout()
        {
            Cookies = new SteamGiftsCookie();
            Enabled = false;
        }

        #region Sync

        public async Task<Log> SyncAccount()
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var xsrf = Web.Get(Links.SteamGiftsSync, Cookies.Generate());

                if (xsrf.RestResponse.Content != null)
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(xsrf.RestResponse.Content);

                    var xsrfToken = htmlDoc.DocumentNode.SelectSingleNode("//input[@name='xsrf_token']");
                    if (xsrfToken != null)
                    {
                        var headers = new List<HttpHeader>();
                        var header = new HttpHeader
                        {
                            Name = "X-Requested-With",
                            Value = "XMLHttpRequest"
                        };
                        headers.Add(header);

                        var response = Web.Post(Links.SteamGiftsAjax,
                            GenerateJoinParams(xsrfToken.Attributes["value"].Value, "", "sync"), headers,
                            Cookies.Generate(), UserAgent);
                        if (response != null)
                        {
                            var result =
                                JsonConvert.DeserializeObject<JsonResponseSyncAccount>(response.RestResponse.Content);
                            if (result.Type == "success")
                            {
                                task.SetResult(new Log($"{Messages.GetDateTime()} {{SteamGifts}} {result.Msg}",
                                    Color.Green,
                                    true));
                            }
                            task.SetResult(new Log($"{Messages.GetDateTime()} {{SteamGifts}} {result.Msg}", Color.Red,
                                false));
                        }
                    }
                }
                task.SetResult(null);
            });
            return task.Task.Result;
        }

        #endregion

        #region JoinGiveaway

        private async Task<Log> JoinGiveaway(SteamGiftsGiveaway giveaway)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(async () =>
            {
                Thread.Sleep(400);
                giveaway = GetJoinData(giveaway);

                if (giveaway.Token != null)
                {
                    var response = Web.Post(Links.SteamGiftsAjax,
                        GenerateJoinParams(giveaway.Token, giveaway.Code, "entry_insert"),
                        Cookies.Generate(), UserAgent);

                    if (response.RestResponse.Content != null)
                    {
                        var jsonresponse =
                            JsonConvert.DeserializeObject<JsonResponseJoin>(response.RestResponse.Content);
                        if (jsonresponse.Type == "success")
                        {
                            Points = jsonresponse.Points;
                            task.SetResult(Messages.GiveawayJoined("SteamGifts", giveaway.Name, giveaway.Price,
                                jsonresponse.Points,
                                giveaway.Level));
                        }
                        else
                        {
                            var jresponse =
                                JsonConvert.DeserializeObject<JsonResponseError>(response.RestResponse.Content);
                            task.SetResult(Messages.GiveawayNotJoined("SteamGifts", giveaway.Name,
                                jresponse.Error?.Message));
                        }
                    }
                }
                else
                {
                    task.SetResult(Messages.GiveawayNotJoined("SteamGifts", giveaway.Name,
                        await GetFailedDetail(giveaway.Link)));
                }
            });
            return task.Task.Result;
        }

        public async Task Join(Blacklist blacklist, bool sort, bool sortToMore, bool wishlistNotSort)
        {
            LogMessage.Instance.AddMessage(await LoadGiveawaysAsync(blacklist));

            if (Giveaways?.Count > 0)
            {
                if (sort)
                {
                    if (sortToMore)
                    {
                        Giveaways.Sort((a, b) => b.Price.CompareTo(a.Price));
                    }
                    else
                    {
                        Giveaways.Sort((a, b) => a.Price.CompareTo(b.Price));
                    }
                }

                if (SortToLessLevel)
                {
                    Giveaways.Sort((a, b) => b.Level.CompareTo(a.Level));
                }
                else if(SortToMoreCopies)
                {
                    Giveaways.Sort((a, b) => a.Copies.CompareTo(b.Copies));
                }

                foreach (var giveaway in Giveaways)
                {
                    if (giveaway.Price <= Points && PointsReserv <= Points - giveaway.Price)
                    {
                        LogMessage.Instance.AddMessage(await JoinGiveaway(giveaway));
                    }
                }
            }

            if (WishlistGiveaways?.Count > 0)
            {
                if (sort)
                {
                    if (sortToMore)
                    {
                        if (!wishlistNotSort)
                        {
                            WishlistGiveaways.Sort((a, b) => b.Price.CompareTo(a.Price));
                        }
                    }
                    else
                    {
                        if (!wishlistNotSort)
                        {
                            WishlistGiveaways.Sort((a, b) => a.Price.CompareTo(b.Price));
                        }
                    }
                }

                if (SortToLessLevel)
                {
                    if (!wishlistNotSort)
                    {
                        WishlistGiveaways.Sort((a, b) => b.Level.CompareTo(a.Level));
                    }
                }

                foreach (var giveaway in WishlistGiveaways)
                {
                    if (giveaway.Price <= Points && PointsReserv <= Points - giveaway.Price)
                    {
                        LogMessage.Instance.AddMessage(await JoinGiveaway(giveaway));
                    }
                }
            }
        }

        private static List<Parameter> GenerateJoinParams(string xsrfToken, string code, string action)
        {
            var list = new List<Parameter>();

            var xsrfParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "xsrf_token",
                Value = xsrfToken
            };
            list.Add(xsrfParam);

            var doParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "do",
                Value = action
            };
            list.Add(doParam);

            if (code != "")
            {
                var codeParam = new Parameter
                {
                    Type = ParameterType.GetOrPost,
                    Name = "code",
                    Value = code
                };
                list.Add(codeParam);
            }

            return list;
        }

        #endregion

        #region Parse

        public async Task<Log> CheckLogin()
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var response = Web.Get(Links.SteamGifts, Cookies.Generate(), UserAgent);

                if (response.RestResponse.Content != string.Empty)
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(response.RestResponse.Content);

                    var points = htmlDoc.DocumentNode.SelectSingleNode("//a[@href='/account']/span[1]");
                    var level = htmlDoc.DocumentNode.SelectSingleNode("//a[@href='/account']/span[2]");
                    var username = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='nav__avatar-outer-wrap']");

                    if (points != null && level != null && username != null)
                    {
                        Points = int.Parse(points.InnerText);
                        Level = int.Parse(level.InnerText.Split(' ')[1]);
                        task.SetResult(Messages.ParseProfile("SteamGifts", Points, Level,
                            username.Attributes["href"].Value.Split('/')[2]));
                    }

                    var error =
                        htmlDoc.DocumentNode.SelectSingleNode("//div[@class='notification notification--warning']");
                    if (error != null)
                    {
                        Enabled = false;
                        task.SetResult(Messages.ParseProfileFailed("SteamGifts", error.InnerText));
                    }
                }
                else
                {
                    task.SetResult(Messages.ParseProfileFailed("SteamGifts"));
                }
            });
            return task.Task.Result;
        }

        public async Task<Log> CheckWon()
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var response = Web.Get(Links.SteamGiftsWon, Cookies.Generate(), UserAgent);

                if (response.RestResponse.Content != string.Empty)
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(response.RestResponse.Content);

                    var nodes =
                        htmlDoc.DocumentNode.SelectSingleNode(
                            "//a[@title='Giveaways Won']//div[@class='nav__notification']");

                    task.SetResult(nodes != null
                        ? Messages.GiveawayHaveWon("SteamGifts", int.Parse(nodes.InnerText), Links.SteamGiftsWon)
                        : null);
                }
                else
                {
                    task.SetResult(null);
                }
            });
            return task.Task.Result;
        }

        private async Task<Log> LoadGiveawaysAsync(Blacklist blackList)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var content = string.Empty;
                Giveaways?.Clear();
                WishlistGiveaways?.Clear();

                if (WishList)
                {
                    content += LoadGiveawaysByUrl(
                        $"{Links.SteamGiftsSearch}?type=wishlist",
                        strings.ParseLoadGiveaways_WishListGiveAwaysIn,
                        WishlistGiveaways);
                }

                if (Group)
                {
                    content += LoadGiveawaysByUrl(
                        $"{Links.SteamGiftsSearch}?type=group",
                        strings.ParseLoadGiveaways_GroupGiveAwaysIn,
                        Giveaways);
                }

                if (Regular)
                {
                    content += LoadGiveawaysByUrl(
                        $"{Links.SteamGiftsSearch}",
                        strings.ParseLoadGiveaways_RegularGiveawaysIn,
                        Giveaways);
                }

                if (Giveaways?.Count == 0 && WishlistGiveaways?.Count == 0)
                {
                    task.SetResult(Messages.ParseGiveawaysEmpty(content, "SteamGifts"));
                }
                else
                {
                    blackList.RemoveGames(Giveaways);
                    blackList.RemoveGames(WishlistGiveaways);
                    task.SetResult(Messages.ParseGiveawaysFoundMatchGiveaways(content, "SteamGifts",
                        (Giveaways?.Count + WishlistGiveaways?.Count).ToString()));
                }
            });
            return task.Task.Result;
        }

        private string LoadGiveawaysByUrl(string url, string message, List<SteamGiftsGiveaway> giveawaysList)
        {
            var count = 0;
            var pages = 1;

            for (var i = 0; i < pages; i++)
            {
                var response = Web.Get(
                    $"{url}{(i > 0 ? $"?page={i + 1}" : string.Empty)}",
                    Cookies.Generate(), UserAgent);

                if (response.RestResponse.Content != string.Empty)
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(response.RestResponse.Content);

                    var pageNodeCounter = htmlDoc.DocumentNode.SelectNodes("//div[@class='pagination__navigation']/a");
                    if (pageNodeCounter != null)
                    {
                        pages =
                            int.Parse(pageNodeCounter[pageNodeCounter.Count - 1].Attributes["data-page-number"].Value);
                    }

                    var nodes =
                        htmlDoc.DocumentNode.SelectNodes(
                            "//div[@class='widget-container']//div[2]//div[3]//div[@class='giveaway__row-outer-wrap']//div[@class='giveaway__row-inner-wrap']");
                    count += AddGiveaways(nodes, giveawaysList);
                }
            }

            return
                $"{Messages.GetDateTime()} {{SteamGifts}} {strings.ParseLoadGiveaways_Found} {count} {message} {pages} {strings.ParseLoadGiveaways_Pages}\n";
        }

        private int AddGiveaways(HtmlNodeCollection nodes, List<SteamGiftsGiveaway> giveawaysList)
        {
            var count = 0;

            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var name = node.SelectSingleNode(".//a[@class='giveaway__heading__name']");
                    var link = node.SelectSingleNode(".//a[@class='giveaway__heading__name']");
                    var storeId = node.SelectSingleNode(".//a[@class='giveaway__icon']");
                    if (name != null && link != null && storeId != null)
                    {
                        var sgGiveaway = new SteamGiftsGiveaway
                        {
                            Name = name.InnerText,
                            Link = link.Attributes["href"].Value,
                            StoreId = storeId.Attributes["href"].Value.Split('/')[4]
                        };
                        sgGiveaway.Code = sgGiveaway.Link.Split('/')[2];

                        foreach (var price in node.SelectNodes(".//span[@class='giveaway__heading__thin']"))
                        {
                            if(price.InnerText.Contains("Copies"))
                            {
                                sgGiveaway.Copies = int.Parse(price.InnerText.Replace(" Copies)", "").Split('(')[1]);
                            }
                            else
                            {
                                sgGiveaway.Price = int.Parse(price.InnerText.Split('(')[1].Split('P')[0]);
                            }
                        }

                        var level = node.SelectSingleNode(".//div[@title='Contributor Level']");
                        sgGiveaway.Level = level == null ? 0 : int.Parse(level.InnerText.Split(' ')[1].Trim('+'));

                        var region = node.SelectSingleNode(".//a[@title='Region Restricted']");
                        if (region != null)
                        {
                            sgGiveaway.Region = region.Attributes["href"].Value.Split('/')[2];
                        }

                        if (sgGiveaway.Price <= Points &&
                            sgGiveaway.Price <= JoinPointLimit &&
                            sgGiveaway.Level <= MinLevel)
                        {
                            giveawaysList?.Add(sgGiveaway);
                            count++;
                        }
                    }
                }
            }

            return count;
        }

        private SteamGiftsGiveaway GetJoinData(SteamGiftsGiveaway sgGiveaway)
        {
            var response = Web.Get($"{Links.SteamGifts}{sgGiveaway.Link}", Cookies.Generate(), UserAgent);

            if (response.RestResponse.Content != string.Empty)
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

        private async Task<string> GetFailedDetail(string url)
        {
            var task = new TaskCompletionSource<string>();
            await Task.Run(() =>
            {
                var response = Web.Get($"{Links.SteamGifts}{url}", Cookies.Generate(), UserAgent);

                if (response.RestResponse.Content != string.Empty)
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(response.RestResponse.Content);

                    var errorNode =
                        htmlDoc.DocumentNode.SelectSingleNode(
                            "//div[contains(@class, 'sidebar__error') and contains(@class, 'is-disabled')]");

                    task.SetResult(errorNode?.InnerText.Trim() ?? "Failed to get Token");
                }
                else
                {
                    task.SetResult("Failed to get Token");
                }
            });
            return task.Task.Result;
        }

        #endregion

        public async Task<Dictionary<int, string>> GetBlaclList()
        {
            var task = new TaskCompletionSource<Dictionary<int, string>>();
            await Task.Run(() =>
            {
                var response = Web.Get(Links.SteamGiftsBlackList, Cookies.Generate(), UserAgent);

                if (response.RestResponse.Content != string.Empty)
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(response.RestResponse.Content);

                    var nodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='table__row-outer-wrap']");

                    Dictionary<int, string> gameList = new Dictionary<int, string>();

                    if (nodes.Count > 0)
                    {
                        foreach (var game in nodes)
                        {
                            var name = game.SelectSingleNode(".//p[@class='table__column__heading']");
                            var id = game.SelectSingleNode(".//a[@class='table__column__secondary-link']");

                            if (name != null && id != null)
                            {
                                gameList.Add(int.Parse(id.InnerText.Split('/')[4].Split('/')[0]), name.InnerText);
                            }
                        }
                    }

                    task.SetResult(gameList);
                }
                else
                {
                    task.SetResult(null);
                }
            });
            return task.Task.Result;
        }
    }
}