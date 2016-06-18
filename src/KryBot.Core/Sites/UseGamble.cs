using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Exceptionless.Json;
using HtmlAgilityPack;
using KryBot.Core.Cookies;
using KryBot.Core.Giveaways;
using KryBot.Core.Serializable.UseGamble;
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
		public int Points { get; set; }
		public int MaxJoinValue { get; set; } = 30;
		public int PointsReserv { get; set; }
		public UseGambleCookie Cookies { get; set; }
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
			if (giveaway.Code != null)
			{
				var list = new List<HttpHeader>();
				var header = new HttpHeader
				{
					Name = "X-Requested-With",
					Value = "XMLHttpRequest"
				};
				list.Add(header);

				var response = Web.Post(Links.UseGambleJoin,
					GenerateJoinData(giveaway.Code), list,
					Cookies.Generate());
				var jresponse =
					JsonConvert.DeserializeObject<JsonJoin>(response.RestResponse.Content.Replace(".", ""));
				if (jresponse.Error == 0)
				{
					Points = jresponse.target_h.my_coins;
					return Messages.GiveawayJoined("UseGamble", giveaway.Name, giveaway.Price,
						jresponse.target_h.my_coins);
				}
				return Messages.GiveawayNotJoined("UseGamble", giveaway.Name, "Error");
			}
			return null;
		}

		public async Task<Log> Join(int index)
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = JoinGiveaway(Giveaways[index]);
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		private static List<Parameter> GenerateJoinData(string ga)
		{
			var list = new List<Parameter>();

			var gaParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "ga",
				Value = ga
			};
			list.Add(gaParam);

			return list;
		}

		#endregion

		#region Parse

		public async Task<Log> CheckLogin()
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var response = Web.Get(Links.UseGamble, Cookies.Generate());

				if (response.RestResponse.Content != string.Empty)
				{
					var htmlDoc = new HtmlDocument();
					htmlDoc.LoadHtml(response.RestResponse.Content);

					var points = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='my_coins']");
					var profileLink = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='mp-wrap']/a[1]");
					if (points != null && profileLink != null)
					{
						Points = int.Parse(points.InnerText);
						task.SetResult(Messages.ParseProfile("UseGamble", Points, profileLink.InnerText));
					}
					else
					{
						task.SetResult(Messages.ParseProfileFailed("UseGamble"));
					}
				}
				else
				{
					task.SetResult(Messages.ParseProfileFailed("UseGamble"));
				}
			});
			return task.Task.Result;
		}

		public async Task<Log> CheckWon()
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var response = Web.Get(Links.UseGambleWon, Cookies.Generate());
				if (response.RestResponse.Content != string.Empty)
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
						task.SetResult(Messages.GiveawayHaveWon("UseGamble", nodes.Count, Links.UseGambleWon));
					}
					else
					{
						task.SetResult(null);
					}
				}
				else
				{
					task.SetResult(null);
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

						var jsonresponse = Web.Post(Links.UseGambleGaPage,
							GeneratePageData(i + 1), headerList,
							Cookies.Generate());
						if (jsonresponse.RestResponse.Content != string.Empty)
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
						var response = Web.Get(Links.UseGambleGiveaways, Cookies.Generate());

						if (response.RestResponse.Content != string.Empty)
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
							AddGiveaways(nodes);
						}
					}
				}

				if (Giveaways == null)
				{
					task.SetResult(Messages.ParseGiveawaysEmpty("UseGamble"));
				}
				else
				{
					blackList.RemoveGames(Giveaways);
					task.SetResult(Messages.ParseGiveawaysFoundMatchGiveaways("UseGamble", Giveaways.Count.ToString()));
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
					var name = node.SelectSingleNode(".//div[@class='giveaway_name']");
					var storeId = node.SelectSingleNode(".//a[@class='steam-icon']");
					if (name != null && storeId != null)
					{
						var spGiveaway = new UseGambleGiveaway
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

							if (spGiveaway.Price <= Points &&
							    spGiveaway.Price <= MaxJoinValue)
							{
								Giveaways?.Add(spGiveaway);
							}
						}
					}
				}
			}
		}

		private static List<Parameter> GeneratePageData(int page)
		{
			var list = new List<Parameter>();

			var typeParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "type",
				Value = 1
			};
			list.Add(typeParam);

			var pageParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "page",
				Value = page
			};
			list.Add(pageParam);

			//TODO Добавить параметр AjaxToken, находится на странице 

			return list;
		}

		#endregion
	}
}