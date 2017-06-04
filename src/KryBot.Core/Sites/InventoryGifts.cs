/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
*/
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using KryBot.CommonResources.Localization;
using KryBot.Core.Cookies;
using KryBot.Core.Giveaways;
using RestSharp;

namespace KryBot.Core.Sites
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "ConvertIfStatementToConditionalTernaryExpression")]
    public class InventoryGifts
    {
        public InventoryGifts()
        {
            Cookies = new InventoryGiftsCookie();
            Giveaways = new List<InventoryGiftsGiveaway>();
        }

        public bool Enabled { get; set; }
        public int Points { get; set; }
        public int Level { get; set; }
        public bool SteamGifts { get; set; } = true;
        public bool SteamItems { get; set; } = true;
        public bool Tf2Items { get; set; } = true;
        public bool CsGoitem { get; set; } = true;
        public bool DotaItems { get; set; } = true;
        public int JoinPointsLimit { get; set; } = 20000;
        public int PointsReserv { get; set; }
        public InventoryGiftsCookie Cookies { get; set; }
        public List<InventoryGiftsGiveaway> Giveaways { get; set; }

        public void Logout()
        {
            Cookies = new InventoryGiftsCookie();
            Enabled = false;
        }

        #region Parse

        public async Task<Log> CheckLogin()
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var response = Web.Get(Links.InventoryGifts, Cookies.Generate());

                if (response.RestResponse.Content != string.Empty)
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(response.RestResponse.Content);

                    var points = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='navbar']/ul[2]/li[2]");
                    var username = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='navbar']/ul[2]/a");

                    if (points != null && username != null)
                    {
                        Points =
                            int.Parse(
                                points.InnerText.Split(':')[1].Split('/')[0].Replace(",", "").Replace("P", "").Trim());
                        Level = int.Parse(points.InnerText.Split(':')[2].Trim());

                        task.SetResult(Messages.ParseProfile("InvetoryGifts", Points,
                            username.Attributes["href"].Value.Split('/')[4]));
                    }
                    else
                    {
                        task.SetResult(Messages.ParseProfileFailed("InventoryGifts"));
                    }
                }
                else
                {
                    task.SetResult(Messages.ParseProfileFailed("InventoryGifts"));
                }
            });
            return task.Task.Result;
        }

        private async Task<Log> LoadGiveawaysAsync(Blacklist blacklist)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var content = string.Empty;
                Giveaways?.Clear();

                if (SteamGifts)
                {
                    content += LoadGiveawaysByUrl(
                        $"{Links.InventoryGiftsSearch}?id=2",
                        strings.ParseLoadGiveaways_SteamGiveawaysIn, "text1");
                }

                if (SteamItems)
                {
                    content += LoadGiveawaysByUrl(
                        $"{Links.InventoryGiftsSearch}?id=6",
                        strings.ParseLoadGiveaways_SteamItemsIn, "text1-5");
                }

                if (Tf2Items)
                {
                    content += LoadGiveawaysByUrl(
                        $"{Links.InventoryGiftsSearch}?id=3",
                        strings.ParseLoadGiveaways_TF2GiveawaysIn, "text1-8");
                }

                if (CsGoitem)
                {
                    content += LoadGiveawaysByUrl(
                        $"{Links.InventoryGiftsSearch}?id=4",
                        strings.ParseLoadGiveaways_CSGOGiveawaysIn, "text1-5_csgo");
                }

                if (DotaItems)
                {
                    content += LoadGiveawaysByUrl(
                        $"{Links.InventoryGiftsSearch}?id=5",
                        strings.ParseLoadGiveaways_DotaGiveawaysIn, "text1");
                }

                if (Giveaways?.Count == 0)
                {
                    task.SetResult(Messages.ParseGiveawaysEmpty(content, "InventoryGifts"));
                }
                else
                {
                    blacklist.RemoveGames(Giveaways);
                    task.SetResult(Messages.ParseGiveawaysFoundMatchGiveaways(content, "InventoryGifts",
                        Giveaways?.Count.ToString()));
                }
            });
            return task.Task.Result;
        }

        private string LoadGiveawaysByUrl(string url, string message, string nodeClass)
        {
            var count = 0;
            var pages = 1;


            for (var i = 0; i < pages; i++)
            {
                var response = Web.Get($"{url}&pn={i + 1}", Cookies.Generate());

                if (response.RestResponse.Content != string.Empty)
                {
                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(response.RestResponse.Content);

                    var pageNodeCounter =
                        htmlDoc.DocumentNode.SelectSingleNode("//div[@class='pagination_margin_right']/a[3]");
                    if (pageNodeCounter != null)
                    {
                        pages = int.Parse(pageNodeCounter.Attributes["href"].Value.Split('=')[2]);
                    }

                    var nodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='box']/div//div[@class='thread']");
                    if (nodes != null)
                    {
                        count += nodes.Count;
                        AddGiveaways(nodes, nodeClass);
                    }
                }
            }

            return
                $"{Messages.GetDateTime()} {{InventoryGifts}} {strings.ParseLoadGiveaways_Found} {count} {message} {pages} {strings.ParseLoadGiveaways_Pages}\n";
        }

        private void AddGiveaways(HtmlNodeCollection nodes, string nodeClass)
        {
            foreach (var node in nodes)
            {
                var entered =
                    node.SelectSingleNode(
                        ".//i[contains(@class, 'fa') and contains(@class, 'fa-check')]");
                if (entered == null)
                {
                    var id = node.SelectSingleNode($".//div[@class='{nodeClass}']/a");
                    if (id != null)
                    {
                        var name = id.SelectSingleNode(".//span");
                        var price = id.SelectSingleNode(".//span[2]");
                        if (name != null && price != null)
                        {
                            var giveaway = new InventoryGiftsGiveaway
                            {
                                Name = name.InnerText.Trim(),
                                Id = id.Attributes["href"].Value.Split('/')[3].Split('/')[0]
                            };

                            if (price.InnerText.Contains("(CD-Key)"))
                            {
                                giveaway.Price = int.Parse(price.InnerText.Split('(')[2].Split('p')[0].Trim());
                            }
                            else
                            {
                                giveaway.Price = int.Parse(price.InnerText.Split('(')[1].Split('p')[0].Trim());
                            }

                            if (giveaway.Price <= Points && giveaway.Price <= JoinPointsLimit)
                            {
                                Giveaways?.Add(giveaway);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Join

        public async Task Join(Blacklist blacklist)
        {
            LogMessage.Instance.AddMessage(await LoadGiveawaysAsync(blacklist));

            if (Giveaways?.Count > 0)
            {
                foreach (var giveaway in Giveaways)
                {
                    if (giveaway.Price <= Points && PointsReserv <= Points - giveaway.Price)
                    {
                        LogMessage.Instance.AddMessage(await JoinGiveaway(giveaway));
                    }
                }
            }
        }

        private async Task<Log> JoinGiveaway(InventoryGiftsGiveaway giveaway)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                Thread.Sleep(400);

                var response = Web.Post($"{Links.InventoryGiftsJoin}?id={giveaway.Id}",
                    GenerateJoinData(giveaway.Id), Cookies.Generate());

                if (response.RestResponse.Content.Contains("Remove Entry"))
                {
                    Points = Points - giveaway.Price;
                    task.SetResult(Messages.GiveawayJoined("InventoryGifts", giveaway.Name, giveaway.Price, Points));
                }
                else
                {
                    task.SetResult(Messages.GiveawayNotJoined("InventoryGifts", giveaway.Name,
                        response.RestResponse.Content));
                }
            });
            return task.Task.Result;
        }

        private List<Parameter> GenerateJoinData(string id)
        {
            var list = new List<Parameter>();

            var xsrfParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "alpha",
                Value = "1"
            };
            list.Add(xsrfParam);

            var codeParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "member_id",
                Value = Cookies.Steamid
            };
            list.Add(codeParam);

            var tokenParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "giveaway_type",
                Value = "1"
            };
            list.Add(tokenParam);

            var doParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "giveaway_id",
                Value = id
            };
            list.Add(doParam);

            return list;
        }

        #endregion
    }
}