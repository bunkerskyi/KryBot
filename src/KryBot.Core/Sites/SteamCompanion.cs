using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using KryBot.CommonResources.lang;
using KryBot.Core.Cookies;
using KryBot.Core.Giveaways;
using RestSharp;

namespace KryBot.Core.Sites
{
	public class SteamCompanion
	{
		public SteamCompanion()
		{
			Cookies = new SteamCompanionCookie();
			Giveaways = new List<SteamCompanionGiveaway>();
			WishlistGiveaways = new List<SteamCompanionGiveaway>();
		}

		public bool Enabled { get; set; }
		public bool Regular { get; set; } = true;
		public bool WishList { get; set; }
		public bool Contributors { get; set; }
		public bool Group { get; set; }
		public bool AutoJoin { get; set; }
		private string ProfileLink { get; set; }
		public int Points { get; set; }
		public int JoinPointLimit { get; set; } = 1500;
		public int PointsReserv { get; set; }
		public SteamCompanionCookie Cookies { get; set; }
		public List<SteamCompanionGiveaway> Giveaways { get; set; }
		public List<SteamCompanionGiveaway> WishlistGiveaways { get; set; }

		public void Logout()
		{
			Cookies = new SteamCompanionCookie();
			Enabled = false;
		}

		#region JoinGivaway

		private Log JoinGiveaway(SteamCompanionGiveaway giveaway, string steamCookie, Steam steam)
		{
			Thread.Sleep(400);
			var data = GetJoinData(giveaway, steamCookie, steam);
			if (data != null && data.Success)
			{
				if (giveaway.Code != null)
				{
					var list = new List<HttpHeader>();
					var header = new HttpHeader
					{
						Name = "X-Requested-With",
						Value = "XMLHttpRequest"
					};
					list.Add(header);

					var response = Web.Post(Links.SteamCompanionJoin,
						Generate.PostData_SteamCompanion(giveaway.Code), list,
						Cookies.Generate());

					if (response.RestResponse.Content.Split('"')[3].Split('"')[0] == "Success")
					{
						Points = int.Parse(response.RestResponse.Content.Split(':')[2].Split(',')[0]);
						return Messages.GiveawayJoined("SteamCompanion", giveaway.Name, giveaway.Price,
							int.Parse(response.RestResponse.Content.Split(':')[2].Split(',')[0]));
					}

					return Messages.GiveawayNotJoined("SteamCompanion", giveaway.Name, response.RestResponse.Content);
				}
			}
			return data;
		}

		public async Task<Log> JoinGiveawayAsync(int index, string steamCookie, Steam steam)
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = JoinGiveaway(Giveaways[index], steamCookie, steam);
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		#endregion

		#region Parse

		private Log GetProfile()
		{
			var response = Web.Get(Links.SteamCompanion, Cookies.Generate());
			if (response.RestResponse.Content != string.Empty)
			{
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(response.RestResponse.Content);

				var points = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='points']");
				var profileLink = htmlDoc.DocumentNode.SelectSingleNode("//ul[@class='right']/li[1]/a[1]");
				if (points != null && profileLink != null)
				{
					Points = int.Parse(points.InnerText);
					ProfileLink = profileLink.Attributes["href"].Value;
					return Messages.ParseProfile("SteamCompanion", Points,
						profileLink.Attributes["href"].Value.Split('/')[4]);
				}
			}
			return Messages.ParseProfileFailed("SteamCompanion");
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
			var response = Web.Get(Links.SteamCompanionWon, Cookies.Generate());
			if (response.RestResponse.Content != string.Empty)
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
						return Messages.GiveawayHaveWon("SteamCompanion", nodes.Count, Links.SteamCompanionWon);
					}
				}
			}
			return null;
		}

		public async Task<Log> WonParseAsync()
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = WonParse();
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		private Log LoadGiveaways()
		{
			var content = string.Empty;
			Giveaways?.Clear();
			WishlistGiveaways?.Clear();

			if (WishList)
			{
				content += LoadGiveawaysByUrl(
					$"{Links.SteamCompanionSearch}?wishlist=true",
					strings.ParseLoadGiveaways_WishListGiveAwaysIn,
					WishlistGiveaways);
			}

			if (Contributors)
			{
				content += LoadGiveawaysByUrl(
					$"{Links.SteamCompanionSearch}?type=contributor",
					strings.ParseLoadGiveaways__ContributorsIn,
					Giveaways);
			}

			if (Group)
			{
				content += LoadGiveawaysByUrl(
					$"{Links.SteamCompanionSearch}?type=group",
					strings.ParseLoadGiveaways_GroupGiveAwaysIn,
					Giveaways);
			}

			if (Regular)
			{
				content += LoadGiveawaysByUrl(
					$"{Links.SteamCompanionSearch}?type=public",
					strings.ParseLoadGiveaways_RegularGiveawaysIn,
					Giveaways);
			}

			if (Giveaways?.Count == 0 && WishlistGiveaways?.Count == 0)
			{
				return Messages.ParseGiveawaysEmpty(content, "SteamCompanion");
			}

			return Messages.ParseGiveawaysFoundMatchGiveaways(content, "SteamCompanion",
				(Giveaways?.Count + WishlistGiveaways?.Count).ToString());
		}

		public async Task<Log> LoadGiveawaysAsync()
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = LoadGiveaways();
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		private string LoadGiveawaysByUrl(string url, string message, List<SteamCompanionGiveaway> giveawaysList)
		{
			var count = 0;
			var pages = 1;
			for (var i = 0; i < pages; i++)
			{
				var response = Web.Get(
					$"{url}{(i > 0 ? $"&page={i + 1}" : string.Empty)}", Cookies.Generate());

				if (response.RestResponse.Content != string.Empty)
				{
					var htmlDoc = new HtmlDocument();
					htmlDoc.LoadHtml(response.RestResponse.Content);

					var pageNode = htmlDoc.DocumentNode.SelectSingleNode("//li[@class='arrow']/a[1]");
					if (pageNode != null)
					{
						pages = int.Parse(Regex.Match(pageNode.Attributes["href"].Value, @"\d+").Value);
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
						AddGiveaways(nodes, giveawaysList);
					}
				}
			}
			return
				$"{Messages.GetDateTime()} {{SteamCompanion}} {strings.ParseLoadGiveaways_Found} {count} {message} {pages} {strings.ParseLoadGiveaways_Pages}\n";
		}

		private void AddGiveaways(HtmlNodeCollection nodes, List<SteamCompanionGiveaway> giveaways)
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
						var scGiveaway = new SteamCompanionGiveaway
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

						if (scGiveaway.Price <= Points &&
						    scGiveaway.Price <= JoinPointLimit)
						{
							giveaways?.Add(scGiveaway);
						}
					}
				}
			}
		}

		private Log GetJoinData(SteamCompanionGiveaway scGiveaway, string steamCookie, Steam steam)
		{
			var response = Web.Get(scGiveaway.Link, Cookies.Generate());

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
					if (AutoJoin)
					{
						var trueGroupUrl = Web.Get(group.Attributes["href"].Value, Cookies.Generate());

						return steam.JoinGroup(trueGroupUrl.RestResponse.ResponseUri.AbsoluteUri,
							Generate.PostData_SteamGroupJoin(steamCookie));
					}
					var error =
						htmlDoc.DocumentNode.SelectSingleNode("//a[@class='notification group-join regular-button qa']");
					return
						new Log(
							$"{Messages.GetDateTime()} {{SteamCompanion}} {strings.GiveawayJoined_Join} \"{scGiveaway.Name}\" {strings.GiveawayNotJoined_NotConfirmed} " +
							$"{strings.GiveawayNotJoined_YouMustEnteredToGroup} {{{(error == null ? "Error" : error.Attributes["href"].Value)}}}",
							Color.Yellow, false, true);
				}

				var exception =
					htmlDoc.DocumentNode.SelectSingleNode("//a[@class='notification regular-button']").InnerText;

				if (exception != null)
				{
					return Messages.GiveawayNotJoined("SteamCompanion", scGiveaway.Name, exception);
				}
			}
			return Messages.GiveawayNotJoined("SteamCompanion", scGiveaway.Name, "Content is empty");
		}

		#endregion

		#region Sync

		private Log SyncAccount()
		{
			var response = Web.Get("https://steamcompanion.com//settings/resync&success=true", Cookies.Generate());
			if (response.RestResponse.Content != string.Empty)
			{
				return new Log($"{Messages.GetDateTime()} {{SteamCompanion}} Sync success!", Color.Green,
					true, true);
			}
			return new Log($"{Messages.GetDateTime()} {{SteamCompanion}} Sync failed", Color.Red, false, true);
		}

		public async Task<Log> SyncAccountAsync()
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = SyncAccount();
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		#endregion
	}
}