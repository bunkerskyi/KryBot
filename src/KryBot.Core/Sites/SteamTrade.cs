using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using KryBot.Core.Cookies;
using KryBot.Core.Giveaways;
using RestSharp;

namespace KryBot.Core.Sites
{
	public class SteamTrade
	{
		public SteamTrade()
		{
			Cookies = new SteamTradeCookie();
			Giveaways = new List<SteamTradeGiveaway>();
		}

		public bool Enabled { get; set; }
		public SteamTradeCookie Cookies { get; set; }
		public List<SteamTradeGiveaway> Giveaways { get; set; }

		public void Logout()
		{
			Cookies = new SteamTradeCookie();
			Enabled = false;
		}

		public async Task<Web.Response> DoAuth(CookieContainer cookies)
		{
			return await Web.PostAsync(Links.SteamTradeLogin, GenerateJoinData(), cookies);
		}

		#region JoinGiveaway

		public async Task<Log> Join(SteamTradeGiveaway giveaway)
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				Thread.Sleep(400);
				giveaway = GetJoinData(giveaway);
				if (giveaway.LinkJoin != null)
				{
					var response = Web.Get($"{Links.SteamTrade}{giveaway.LinkJoin}", Cookies.Generate());
					if (response.RestResponse.StatusCode == HttpStatusCode.OK)
					{
						task.SetResult(Messages.GiveawayJoined("SteamTrade", giveaway.Name.Trim(), 0, 0));
					}
					else
					{
						task.SetResult(Messages.GiveawayNotJoined("SteamTrade", giveaway.Name,
							response.RestResponse.StatusCode.ToString()));
					}
				}
				else
				{
					task.SetResult(null);
				}
			});
			return task.Task.Result;
		}

		private static List<Parameter> GenerateJoinData()
		{
			var rnd = new Random();
			var list = new List<Parameter>();

			var xParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "x",
				Value = rnd.Next(0, 190)
			};
			list.Add(xParam);

			var yParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "y",
				Value = rnd.Next(0, 23)
			};
			list.Add(yParam);

			return list;
		}

		#endregion

		#region Parse

		public async Task<Log> CheckLogin()
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var response = Web.Get(Links.SteamTrade, Cookies.Generate());
				if (response.RestResponse.Content != string.Empty)
				{
					var htmlDoc = new HtmlDocument();
					htmlDoc.LoadHtml(response.RestResponse.Content);

					var test = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='topm1']");
					if (test != null)
					{
						task.SetResult(Messages.ParseProfile("SteamTrade", test.InnerText));
					}
				}
				else
				{
					task.SetResult(Messages.ParseProfileFailed("SteamTrade"));
				}
			});
			return task.Task.Result;
		}

		public async Task<Log> LoadGiveawaysAsync(Blacklist blackList)
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				Giveaways?.Clear();

				var response = Web.Get(Links.SteamTradeWon, Cookies.Generate());

				if (response.RestResponse.Content != string.Empty)
				{
					var htmlDoc = new HtmlDocument();
					htmlDoc.LoadHtml(response.RestResponse.Content);

					var nodes = htmlDoc.DocumentNode.SelectNodes("//tbody[@bgcolor='#F3F5F7']/tr");
					AddGiveaways(nodes);

					if (Giveaways == null)
					{
						task.SetResult(Messages.ParseGiveawaysEmpty("SteamTrade"));
					}
					else
					{
						if (blackList != null)
						{
							for (var i = 0; i < Giveaways.Count; i++)
							{
								if (blackList.Items.Any(item => Giveaways[i].StoreId == item.Id))
								{
									Giveaways.Remove(Giveaways[i]);
									i--;
								}
							}
						}
						task.SetResult(Messages.ParseGiveawaysFoundMatchGiveaways("SteamTrade", Giveaways.Count.ToString()));
					}
				}
				else
				{
					task.SetResult(null);
				}
			});
			return task.Task.Result;
		}

		private void AddGiveaways(HtmlNodeCollection nodes)
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
							var spGiveaway = new SteamTradeGiveaway
							{
								Name = node.SelectSingleNode(".//td[1]/a[2]").InnerText,
								Link = node.SelectSingleNode(".//td[1]/a[2]").Attributes["href"].Value,
								StoreId = node.SelectSingleNode(".//td[1]/a[1]").Attributes["href"].Value.Split('/')[4]
							};
							Giveaways?.Add(spGiveaway);
						}
					}
				}
			}
		}

		private SteamTradeGiveaway GetJoinData(SteamTradeGiveaway stGiveaway)
		{
			var response = Web.Get($"{Links.SteamTrade}{stGiveaway.Link}", Cookies.Generate());

			if (response.RestResponse.Content != string.Empty)
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
	}
}