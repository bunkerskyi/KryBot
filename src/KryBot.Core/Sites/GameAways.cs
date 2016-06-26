using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using KryBot.Core.Cookies;
using KryBot.Core.Giveaways;
using KryBot.Core.Serializable.GameAways;
using Newtonsoft.Json;
using RestSharp;

namespace KryBot.Core.Sites
{
    public class GameAways
    {
        public GameAways()
        {
            Cookies = new GameAwaysCookie();
            Giveaways = new List<GameAwaysGiveaway>();
        }

        public bool Enabled { get; set; }
        public int Points { get; set; }
        public int JoinPointsLimit { get; set; }
        public int PointsReserv { get; set; }
        public GameAwaysCookie Cookies { get; set; }
        public List<GameAwaysGiveaway> Giveaways { get; set; }

        public void Logout()
        {
            Cookies = new GameAwaysCookie();
            Enabled = false;
        }

        #region Generate

        private List<Parameter> GenerateJoinParams()
        {
            var list = new List<Parameter>();

            var idParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "_",
                Value = "229"
            };
            list.Add(idParam);

            return list;
        }

        #endregion

        #region Join

        public async Task Join(Blacklist blacklist, bool sort, bool sortToMore)
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

                foreach (var giveaway in Giveaways)
                {
                    if (giveaway.Price <= Points && PointsReserv <= Points - giveaway.Price)
                    {
                        LogMessage.Instance.AddMessage(await JoinGiveaway(giveaway));
                    }
                }
            }
        }

        private async Task<Log> JoinGiveaway(GameAwaysGiveaway giveaway)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                Thread.Sleep(400);

                var response = Web.Post($"{Links.GameAwaysJoin}{giveaway.Id}",
                    GenerateJoinParams(), Cookies.Generate());

                if (!string.IsNullOrEmpty(response.RestResponse.Content))
                {
                    var jsonresponse = JsonConvert.DeserializeObject<JsonJoinResponse>(response.RestResponse.Content);
                    if (jsonresponse != null)
                    {
                        Points = jsonresponse.Balance;
                        task.SetResult(Messages.GiveawayJoined("GameAways", giveaway.Name, giveaway.Price,
                            jsonresponse.Balance));
                    }
                    else
                    {
                        task.SetResult(Messages.GiveawayNotJoined("GameAways", giveaway.Name, "Oooops, some wrong..."));
                    }
                }
            });
            return task.Task.Result;
        }

        #endregion

        #region Parse

        public async Task<Log> CheckLogin()
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var response = Web.Get(Links.GameAways, Cookies.Generate());

                if (response.RestResponse.Content != string.Empty)
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(response.RestResponse.Content);

                    var points = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='ga-points']");
                    var username = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='username']");

                    if (points != null && username != null)
                    {
                        Points = int.Parse(points.InnerText.Replace(" GP", ""));

                        task.SetResult(Messages.ParseProfile("GameAways", Points, username.InnerText));
                    }
                    else
                    {
                        task.SetResult(Messages.ParseProfileFailed("GameAways"));
                    }
                }
                else
                {
                    task.SetResult(Messages.ParseProfileFailed("GameAways"));
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

                var pages = 1;


                for (var i = 0; i < pages; i++)
                {
                    var response = Web.Get($"{Links.GameAways}{(i > 0 ? $"?page={i + 1}" : string.Empty)}",
                        Cookies.Generate());

                    if (response.RestResponse.Content != string.Empty)
                    {
                        var htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(response.RestResponse.Content);

                        var pageNodeCounter = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='float-right next']");
                        if (pageNodeCounter != null)
                        {
                            pages = int.Parse(pageNodeCounter.Attributes["href"].Value.Split('=')[1]);
                        }

                        var nodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='giveaway ']");
                        AddGiveaways(nodes, Giveaways);
                    }
                }

                if (Giveaways?.Count == 0)
                {
                    task.SetResult(Messages.ParseGiveawaysEmpty(content, "GameAways"));
                }
                else
                {
                    blackList.RemoveGames(Giveaways);
                    task.SetResult(Messages.ParseGiveawaysFoundMatchGiveaways(content, "GameAways",
                        Giveaways?.Count.ToString()));
                }
            });
            return task.Task.Result;
        }

        private void AddGiveaways(HtmlNodeCollection nodes, List<GameAwaysGiveaway> giveawaysList)
        {
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    var name = node.SelectSingleNode(".//a[@class='game-title']");
                    var storeId = node.SelectSingleNode(".//div[@class='ga-tag-container']/a[1]");
                    var price = node.SelectSingleNode(".//a[@class='entry-fee']");
                    var group =
                        node.SelectSingleNode(
                            ".//span[contains(@class, 'ga-list-menu-icon') and contains(@class, 'ga-list-group')]");
                    if (name != null && storeId != null && price != null && group == null)
                    {
                        var gaGiveaway = new GameAwaysGiveaway
                        {
                            Name = name.InnerText,
                            Id = node.Attributes["id"].Value,
                            StoreId = storeId.Attributes["href"].Value.Split('/')[4],
                            Price = int.Parse(price.InnerText.Replace(" GP", ""))
                        };

                        if (gaGiveaway.Price <= Points && gaGiveaway.Price <= JoinPointsLimit)
                        {
                            giveawaysList?.Add(gaGiveaway);
                        }
                    }
                }
            }
        }

        #endregion
    }
}