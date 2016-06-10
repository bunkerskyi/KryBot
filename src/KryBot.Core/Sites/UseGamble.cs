using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Exceptionless.Json;
using HtmlAgilityPack;
using KryBot.CommonResources.lang;
using KryBot.Core.Cookies;
using KryBot.Core.Giveaways;
using RestSharp;

namespace KryBot.Core.Sites
{
	public class UseGamble
	{
		public UseGamble()
		{
			Cookies = new UseGambleCookie();
			Giveaways = new List<UseGambleGiveaway>();
		}

		public bool Enabled { get; set; }
		private string ProfileLink { get; set; }
		public int Points { get; private set; }
		public int MaxJoinValue { get; set; } = 30;
		public int PointsReserv { get; set; }
		public UseGambleCookie Cookies { get; private set; }
		public List<UseGambleGiveaway> Giveaways { get; set; }

		public void Logout()
		{
			Cookies = new UseGambleCookie();
			Enabled = false;
		}

		#region JoinGiveaway
		private Log JoinGiveaway(UseGambleGiveaway giveaway)
		{
			Thread.Sleep(400);
			if(giveaway.Code != null)
			{
				var list = new List<HttpHeader>();
				var header = new HttpHeader
				{
					Name = "X-Requested-With",
					Value = "XMLHttpRequest"
				};
				list.Add(header);

				var response = Web.Post($"{Links.UseGamble}page/join",
					Generate.PostData_UseGamble(giveaway.Code), list,
					Cookies.Generate());
				var jresponse =
					JsonConvert.DeserializeObject<JsonJoin>(response.RestResponse.Content.Replace(".", ""));
				if(jresponse.Error == 0)
				{
					Points = jresponse.target_h.my_coins;
					return Messages.GiveawayJoined("UseGamble", giveaway.Name, giveaway.Price,
						jresponse.target_h.my_coins, 0);
				}
				return Messages.GiveawayNotJoined("UseGamble", giveaway.Name, "Error");
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

		private Log GetProfile()
		{
			var response = Web.Get(Links.UseGamble, new List<Parameter>(),
				Cookies.Generate(),
				new List<HttpHeader>(), String.Empty);

			if(response.RestResponse.Content != String.Empty)
			{
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(response.RestResponse.Content);

				var points = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='my_coins']");
				var profileLink = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='mp-wrap']/a[1]");
				if(points != null && profileLink != null)
				{
					Points = Int32.Parse(points.InnerText);
					ProfileLink = Links.UseGamble + profileLink.Attributes["href"].Value.Replace("/", "");
					return Messages.ParseProfile("UseGamble", Points, profileLink.InnerText);
				}
			}
			return Messages.ParseProfileFailed("UseGamble");
		}

		public async Task<Log> GetProfileAsync()
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = GetProfile();
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		private Log WonParse()
		{
			var response = Web.Get(Links.UseGamble, new List<Parameter>(),
				Cookies.Generate(), new List<HttpHeader>(), "profile/logs");

			if(response.RestResponse.Content != String.Empty)
			{
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(response.RestResponse.Content);

				var nodes = htmlDoc.DocumentNode.SelectNodes("//tr[@class='gray']");
				if(nodes != null)
				{
					for(var i = 0; i < nodes.Count; i++)
					{
						var content = nodes[i].SelectSingleNode("//tr/td[2]").InnerText;
						if(!content.Contains("you've won the Giveaway"))
						{
							nodes.Remove(nodes[i]);
							i--;
						}
					}
					return Messages.GiveawayHaveWon("UseGamble", nodes.Count, $"{Links.UseGamble}profile/logs");
				}
			}
			return null;
		}

		public async Task<Log> WonParsAsync()
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = WonParse();
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		private Log LoadGiveaways(Blacklist blackList)
		{
			Giveaways?.Clear();

			var pages = 1;

			for(var i = 0; i < pages; i++)
			{
				if(pages != 1)
				{
					var headerList = new List<HttpHeader>();
					var header = new HttpHeader
					{
						Name = "X-Requested-With",
						Value = "XMLHttpRequest"
					};
					headerList.Add(header);

					var jsonresponse = Web.Post($"{Links.UseGamble}page/ga_page",
						Generate.PageData_UseGamble(i + 1), headerList,
						Cookies.Generate());
					if(jsonresponse.RestResponse.Content != String.Empty)
					{
						var data = jsonresponse.RestResponse.Content.Replace("\\", "");
						var htmlDoc = new HtmlDocument();
						htmlDoc.LoadHtml(data);

						var nodes = htmlDoc.DocumentNode.SelectNodes("//div[@class='giveaway_container']");
						AddGiveaways(nodes);
					}
				}
				else
				{
					var response = Web.Get(Links.UseGamble,
						new List<Parameter>(), Cookies.Generate(),
						new List<HttpHeader>(), String.Empty);

					if(response.RestResponse.Content != String.Empty)
					{
						var htmlDoc = new HtmlDocument();
						htmlDoc.LoadHtml(response.RestResponse.Content);

						var count =
							htmlDoc.DocumentNode.SelectNodes("//div[@class='nPagin']//div[@class='pagin']/span");
						if(count != null)
						{
							pages = Int32.Parse(htmlDoc.DocumentNode.
								SelectSingleNode($"//div[@class='nPagin']//div[@class='pagin']/span[{count.Count - 1}]")
								.InnerText);
						}

						var nodes =
							htmlDoc.DocumentNode.SelectNodes("//div[@id='normal']/div[@class='giveaway_container']");
						AddGiveaways(nodes);
					}
				}
			}

			if(Giveaways == null)
			{
				return
					new Log(
						$"{Messages.GetDateTime()} {{UseGamble}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: 0",
						Color.White, true, true);
			}

			Tools.RemoveBlacklistedGames(Giveaways, blackList);

			return
				new Log(
					$"{Messages.GetDateTime()} {{UseGamble}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: {Giveaways.Count}",
					Color.White, true, true);
		}

		public async Task<Log> LoadGiveawaysAsync(Blacklist blackList)
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = LoadGiveaways(blackList);
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		private void AddGiveaways(HtmlNodeCollection nodes)
		{
			if(nodes != null)
			{
				foreach(var node in nodes)
				{
					var name = node.SelectSingleNode(".//div[@class='giveaway_name']");
					var storeId = node.SelectSingleNode(".//a[@class='steam-icon']");
					if(name != null && storeId != null)
					{
						var spGiveaway = new UseGambleGiveaway
						{
							Name = name.InnerText,
							StoreId = storeId.Attributes["href"].Value.Split('/')[4]
						};

						var price = node.SelectSingleNode(".//span[@class='coin-white-icon']");
						var code = node.SelectSingleNode(".//div[@class='ga_join_btn ga_coin_join']");
						if(price != null && code != null)
						{
							spGiveaway.Price = Int32.Parse(price.InnerText);
							spGiveaway.Code = code.Attributes["onclick"].Value.Split('\'')[5].Replace("ga:", "");

							var iconsBlock = node.SelectSingleNode(".//div[@class='giveaway_iconbar']");
							var icons = iconsBlock?.SelectNodes(".//span");
							if(icons != null)
							{
								foreach(var icon in icons)
								{
									if(icon.Attributes["class"].Value.Contains("region"))
									{
										spGiveaway.Region = icon.Attributes["class"].Value.Split('-')[1];
									}
								}
							}

							if(spGiveaway.Price <= Points &&
							   spGiveaway.Price <= MaxJoinValue)
							{
								Giveaways?.Add(spGiveaway);
							}
						}
					}
				}
			}
		}

		#endregion

		private class JsonJoin
		{
			public int Error { get; set; }
			public TargetH target_h { get; set; }
		}

		private class TargetH
		{
			public int my_coins { get; set; }
		}
	}
}