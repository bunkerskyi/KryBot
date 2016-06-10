using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using KryBot.CommonResources.lang;
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
		public SteamTradeCookie Cookies { get; private set; }
		public List<SteamTradeGiveaway> Giveaways { get; set; }

		public void Logout()
		{
			Cookies = new SteamTradeCookie();
			Enabled = false;
		}

		#region JoinGiveaway

		private Log JoinGiveaway(SteamTradeGiveaway giveaway)
		{
			Thread.Sleep(400);
			giveaway = GetJoinData(giveaway);
			if(giveaway.LinkJoin != null)
			{
				var response = Web.Get("http://steamtrade.info", new List<Parameter>(),
					Cookies.Generate(),
					new List<HttpHeader>(), giveaway.LinkJoin);
				if(response.RestResponse.StatusCode == HttpStatusCode.OK)
				{
					return Messages.GiveawayJoined("SteamTrade", giveaway.Name.Trim(), 0, 0, 0);
				}
				return Messages.GiveawayNotJoined("SteamTrade", giveaway.Name,
					response.RestResponse.StatusCode.ToString());
			}
			return null;
		}

		public async Task<Log> JoinGiveawayAsync(int index)
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = JoinGiveaway(Giveaways[index]);
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		#endregion

		#region Parse

		private Log SteamTradeGetProfile()
		{
			var response = Web.Get(Links.SteamTrade, new List<Parameter>(),
				Cookies.Generate(),
				new List<HttpHeader>(), string.Empty);

			if(response.RestResponse.Content != string.Empty)
			{
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(response.RestResponse.Content);

				var test = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='topm1']");
				if(test != null)
				{
					return Messages.ParseProfile("SteamTrade", test.InnerText);
				}
			}
			return Messages.ParseProfileFailed("SteamTrade");
		}

		public async Task<Log> SteamTradeGetProfileAsync()
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = SteamTradeGetProfile();
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		private Log SteamTradeLoadGiveaways(Blacklist blackList)
		{
			Giveaways?.Clear();

			var response = Web.Get(Links.SteamTrade, new List<Parameter>(),
				Cookies.Generate(),
				new List<HttpHeader>(), "awards/");

			if(response.RestResponse.Content != String.Empty)
			{
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(response.RestResponse.Content);

				var nodes = htmlDoc.DocumentNode.SelectNodes("//tbody[@bgcolor='#F3F5F7']/tr");
				SteamTradeAddGiveaways(nodes);

				if(Giveaways == null)
				{
					return
						new Log(
							$"{Messages.GetDateTime()} {{SteamTrade}} {strings.ParseLoadGiveaways_FoundMatchGiveaways} : 0",
							Color.White, true, true);
				}

				if(blackList != null)
				{
					for(var i = 0; i < Giveaways.Count; i++)
					{
						if(blackList.Items.Any(item => Giveaways[i].StoreId == item.Id))
						{
							Giveaways.Remove(Giveaways[i]);
							i--;
						}
					}
				}

				return
					new Log(
						$"{Messages.GetDateTime()} {{SteamTrade}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: {Giveaways.Count}",
						Color.White, true, true);
			}
			return null;
		}

		public async Task<Log> SteamTradeLoadGiveawaysAsync(Blacklist blackList)
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = SteamTradeLoadGiveaways(blackList);
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		private void SteamTradeAddGiveaways(HtmlNodeCollection nodes)
		{
			if(nodes != null)
			{
				foreach(var node in nodes)
				{
					if(node.SelectSingleNode(".//span[@class='status1']") == null)
					{
						var name = node.SelectSingleNode(".//td[1]/a[2]");
						var link = node.SelectSingleNode(".//td[1]/a[2]");
						var storeId = node.SelectSingleNode(".//td[1]/a[1]");
						if(name != null && link != null && storeId != null)
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
			var response = Web.Get(Links.SteamTrade,
				new List<Parameter>(), Cookies.Generate(),
				new List<HttpHeader>(), stGiveaway.Link);

			if(response.RestResponse.Content != String.Empty)
			{
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(response.RestResponse.Content);

				var linkJoin = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='inv_join']");
				if(linkJoin != null)
				{
					stGiveaway.LinkJoin = linkJoin.Attributes["href"].Value.Trim();
				}
			}
			return stGiveaway;
		}

		#endregion
	}
}