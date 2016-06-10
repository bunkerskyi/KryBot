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
		public int Points { get; set; }
		public int Level { get; set; }
		public int JoinPointLimit { get; set; } = 300;
		public int PointsReserv { get; set; }
		public int MinLevel { get; set; }
		public SteamGiftsCookie Cookies { get; set; }
		public List<SteamGiftsGiveaway> Giveaways { get; set; }
		public List<SteamGiftsGiveaway> WishlistGiveaways { get; set; }

		public void Logout()
		{
			Cookies = new SteamGiftsCookie();
			Enabled = false;
		}

		private class JsonResponseJoin
		{
			public string Type { get; set; }
			public int Points { get; set; }
		}

		private class JsonResponseSyncAccount
		{
			public string Type { get; set; }
			public string Msg { get; set; }
		}

		#region JoinGiveaway

		private Log JoinGiveaway(SteamGiftsGiveaway giveaway, string userAgent)
		{
			Thread.Sleep(400);
			giveaway = GetJoinData(giveaway, userAgent);

			if (giveaway.Token != null)
			{
				var response = Web.Post($"{Links.SteamGifts}ajax.php",
					Generate.PostData_SteamGifts(giveaway.Token, giveaway.Code, "entry_insert"),
					new List<HttpHeader>(),
					Cookies.Generate(), userAgent);

				if (response.RestResponse.Content != null)
				{
					var jsonresponse =
						JsonConvert.DeserializeObject<JsonResponseJoin>(response.RestResponse.Content);
					if (jsonresponse.Type == "success")
					{
						Points = jsonresponse.Points;
						return Messages.GiveawayJoined("SteamGifts", giveaway.Name, giveaway.Price,
							jsonresponse.Points,
							giveaway.Level);
					}

					var jresponse =
						JsonConvert.DeserializeObject<GameMiner.JsonResponseError>(response.RestResponse.Content);
					return Messages.GiveawayNotJoined("SteamGifts", giveaway.Name, jresponse.Error?.Message);
				}
			}
			return null;
		}

		public async Task<Log> JoinGiveawayAsync(int index, string userAgent)
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = JoinGiveaway(Giveaways[index], userAgent);
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		#endregion

		#region Parse

		private Log GetProfile(string userAgent)
		{
			var response = Web.Get(Links.SteamGifts, new List<Parameter>(),
				Cookies.Generate(), new List<HttpHeader>(), userAgent);

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
					return Messages.ParseProfile("SteamGifts", Points, Level,
						username.Attributes["href"].Value.Split('/')[2]);
				}

				var error = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='notification notification--warning']");
				if (error != null)
				{
					Enabled = false;
					return
						new Log(
							$"{Messages.GetDateTime()} {{SteamGifts}} {strings.AccountNotActive} {{{error.InnerText}}}",
							Color.Red, false, true);
				}
			}
			return Messages.ParseProfileFailed("SteamGifts");
		}

		public async Task<Log> GetProfileAsync(string userAgent)
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = GetProfile(userAgent);
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		private Log WonParse(string userAgent)
		{
			var response = Web.Get($"{Links.SteamGifts}giveaways/won", new List<Parameter>(),
				Cookies.Generate(), new List<HttpHeader>(), userAgent);

			if (response.RestResponse.Content != string.Empty)
			{
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(response.RestResponse.Content);

				var nodes =
					htmlDoc.DocumentNode.SelectSingleNode("//a[@title='Giveaways Won']//div[@class='nav__notification']");

				if (nodes != null)
				{
					return Messages.GiveawayHaveWon("SteamGifts", int.Parse(nodes.InnerText),
						$"{Links.SteamGifts}giveaways/won");
				}
			}
			return null;
		}

		public async Task<Log> WonParseAsync(string userAgent)
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = WonParse(userAgent);
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		private Log LoadGiveaways(List<SteamGiftsGiveaway> giveaways,
			List<SteamGiftsGiveaway> wishlistGiveaways, Blacklist blackList, string userAgent)
		{
			var content = "";
			giveaways?.Clear();

			if (WishList)
			{
				content += LoadWishListGiveaways(userAgent);
			}

			if (Group)
			{
				content += LoadGroupGiveaways(userAgent);
			}

			if (Regular)
			{
				var response = Web.Get($"{Links.SteamGifts}giveaways/search?page=1",
					new List<Parameter>(), Cookies.Generate(),
					new List<HttpHeader>(), userAgent);

				if (response.RestResponse.Content != string.Empty)
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
							response = Web.Get($"{Links.SteamGifts}giveaways/search?page={i + 1}",
								new List<Parameter>(), Cookies.Generate(),
								new List<HttpHeader>(), userAgent);

							if (response.RestResponse.Content != string.Empty)
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
								AddGiveaways(nodes);
							}
						}
					}
				}
			}

			if (giveaways?.Count == 0 && wishlistGiveaways.Count == 0)
			{
				return
					new Log(
						$"{content}{Messages.GetDateTime()} {{SteamGifts}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: 0",
						Color.White, true, true);
			}

			Tools.RemoveBlacklistedGames(giveaways, blackList);

			return
				new Log(
					$"{content}{Messages.GetDateTime()} {{SteamGifts}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: {giveaways?.Count + wishlistGiveaways.Count}",
					Color.White, true, true);
		}

		public async Task<Log> LoadGiveawaysAsync(
			List<SteamGiftsGiveaway> giveaways, List<SteamGiftsGiveaway> wishlistGiveaways,
			Blacklist blackList, string userAgent)
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = LoadGiveaways(giveaways, wishlistGiveaways, blackList, userAgent);
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		private string LoadWishListGiveaways(string userAgent)
		{
			var nodesCount = 0;
			var pages = 1;

			var response = Web.Get($"{Links.SteamGifts}giveaways/search?type=wishlist",
				new List<Parameter>(), Cookies.Generate(), new List<HttpHeader>(), userAgent);

			if (response.RestResponse.Content != string.Empty)
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
						AddGiveaways(nodes);
					}
				}
				else
				{
					var nodes =
						htmlDoc.DocumentNode.SelectNodes(
							"//div[@class='widget-container']//div[2]//div[3]//div[@class='giveaway__row-outer-wrap']//div[@class='giveaway__row-inner-wrap']");
					nodesCount += nodes?.Count ?? 0;
					AddGiveaways(nodes);
				}
			}
			return
				$"{Messages.GetDateTime()} {{SteamGifts}} {strings.ParseLoadGiveaways_FoundGiveAwaysInWishList}: {nodesCount}\n";
		}

		private string LoadGroupGiveaways(string userAgent)
		{
			var nodesCount = 0;

			var response = Web.Get($"{Links.SteamGifts}giveaways/search?type=group",
				new List<Parameter>(), Cookies.Generate(), new List<HttpHeader>(), userAgent);

			if (response.RestResponse.Content != string.Empty)
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
						AddGiveaways(nodes);
					}
				}
				else
				{
					var nodes =
						htmlDoc.DocumentNode.SelectNodes(
							"//div[@class='widget-container']//div[2]//div[3]//div[@class='giveaway__row-outer-wrap']//div[@class='giveaway__row-inner-wrap']");
					AddGiveaways(nodes);
				}
			}
			return $"{Messages.GetDateTime()} {{SteamGifts}} {strings.ParseLoadGiveaways_FoundGiveAwaysInGroup}: {nodesCount}\n";
		}

		private void AddGiveaways(HtmlNodeCollection nodes)
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
						var sgGiveaway = new SteamGiftsGiveaway
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

						if (sgGiveaway.Price <= Points &&
						    sgGiveaway.Price <= JoinPointLimit &&
						    sgGiveaway.Level >= MinLevel)
						{
							Giveaways?.Add(sgGiveaway);
						}
					}
				}
			}
		}

		private SteamGiftsGiveaway GetJoinData(SteamGiftsGiveaway sgGiveaway, string userAgent)
		{
			var response = Web.Get($"{Links.SteamGifts}{sgGiveaway.Link}",
				new List<Parameter>(), Cookies.Generate(), new List<HttpHeader>(), userAgent);

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

		#endregion

		#region Sync

		private Log SyncAccount(string userAgent)
		{
			var xsrf = Web.Get("https://www.steamgifts.com/account/profile/sync",
				new List<Parameter>(), Cookies.Generate(), new List<HttpHeader>(), "");

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

					var response = Web.Post($"{Links.SteamGifts}ajax.php",
						Generate.PostData_SteamGifts(xsrfToken.Attributes["value"].Value, "", "sync"), headers,
						Cookies.Generate(), userAgent);
					if (response != null)
					{
						var result =
							JsonConvert.DeserializeObject<JsonResponseSyncAccount>(
								response.RestResponse.Content);
						if (result.Type == "success")
						{
							return new Log($"{Messages.GetDateTime()} {{SteamGifts}} {result.Msg}",
								Color.Green,
								true, true);
						}
						return new Log($"{Messages.GetDateTime()} {{SteamGifts}} {result.Msg}", Color.Red,
							false,
							true);
					}
				}
			}
			return null;
		}

		public async Task<Log> SyncAccountAsync(string userAgent)
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = SyncAccount(userAgent);
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		#endregion
	}
}