using System.Collections.Generic;
using System.Drawing;
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
	public class PlayBlink
	{
		public PlayBlink()
		{
			Cookies = new PlayBlinkCookie();
			Giveaways = new List<PlayBlinkGiveaway>();
		}

		public bool Enabled { get; set; }
		public int Points { get; set; }
		public int Level { get; set; }
		public int MaxJoinValue { get; set; } = 50;
		public int PointReserv { get; set; }
		public PlayBlinkCookie Cookies { get; set; }
		public List<PlayBlinkGiveaway> Giveaways { get; set; }

		public void Logout()
		{
			Cookies = new PlayBlinkCookie();
			Enabled = false;
		}

		#region JoinGiveaway

		public async Task<Log> JoinGiveawayAsync(PlayBlinkGiveaway pbGiveaway)
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				Thread.Sleep(1000);
				if(pbGiveaway.Id != null)
				{
					var list = new List<HttpHeader>();
					var header = new HttpHeader
					{
						Name = "X-Requested-With",
						Value = "XMLHttpRequest"
					};
					list.Add(header);

					var response = Web.Post($"{Links.PlayBlinkJoin}&game={pbGiveaway.Id}",
						GenerateJoinData(pbGiveaway.Id), list,
						Cookies.Generate(Level));

					if(response.RestResponse.StatusCode == HttpStatusCode.OK)
					{
						if(response.RestResponse.Content != "")
						{
							var htmldoc = new HtmlDocument();
							htmldoc.LoadHtml(response.RestResponse.Content);

							Points = Points - pbGiveaway.Price;

							var message = htmldoc.DocumentNode.SelectSingleNode("//div[@class='msgbox success']");
							if(message != null)
							{
								task.SetResult(Messages.GiveawayJoined("PlayBlink", pbGiveaway.Name, pbGiveaway.Price, Points));
							}
							else
							{
								var error = htmldoc.DocumentNode.SelectSingleNode("//div[@class='msgbox error']");
								if(error != null)
								{
									task.SetResult(Messages.GiveawayNotJoined("PlayBlink", pbGiveaway.Name, error.InnerText));
								}
								else
								{
									var captcha = htmldoc.DocumentNode.SelectSingleNode("//div[@class='flash_rules']");
									if(captcha != null)
									{
										task.SetResult(Messages.GiveawayNotJoined("PlayBlink", pbGiveaway.Name, "Captcha"));
									}
								}
							}			
						}
						else
						{
							task.SetResult(Messages.GiveawayNotJoined("PlayBlink", pbGiveaway.Name, "Error"));
						}
					}
					else
					{
						task.SetResult(Messages.GiveawayNotJoined("PlayBlink", pbGiveaway.Name, "Error"));
					}
				}
				else
				{
					task.SetResult(null);	
				}
			});
			return task.Task.Result;
		}

		private static List<Parameter> GenerateJoinData(string game)
		{
			var list = new List<Parameter>();

			var doParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "do",
				Value = "blink"
			};
			list.Add(doParam);

			var gameParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "game",
				Value = game
			};
			list.Add(gameParam);

			return list;
		}

		#endregion

		#region Parse

		public async Task<Log> CheckProfile()
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var response = Web.Get(Links.PlayBlink, Cookies.Generate(Level));
				if(response.RestResponse.Content != string.Empty)
				{
					var htmlDoc = new HtmlDocument();
					htmlDoc.LoadHtml(response.RestResponse.Content);

					var points = htmlDoc.DocumentNode.SelectSingleNode("//td[@id='points']");
					var level = htmlDoc.DocumentNode.SelectSingleNode("//a[@title='Your contribution level']/b");
					var username = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='usr_link']");
					if(points != null && level != null && username != null)
					{
						Points = int.Parse(points.InnerText.Split('P')[0].Split('\n')[1].Trim());
						Level = int.Parse(level.InnerText);

						Messages.ProfileLoaded();
						task.SetResult(Messages.ParseProfile("PlayBlink", Points, Level, username.InnerText));
					}
				}
				else
				{
					task.SetResult(Messages.ParseProfileFailed("PlayBlink"));
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

				var response = Web.Get(Links.PlayBlink, Cookies.Generate(Level));

				if(response.RestResponse.Content != string.Empty)
				{
					var htmlDoc = new HtmlDocument();
					htmlDoc.LoadHtml(response.RestResponse.Content);

					var nodes = htmlDoc.DocumentNode.SelectNodes("//div[@id='games_free']/div[@class='game_box']");
					if(nodes != null)
					{
						PlayBlinkAddGiveaways(nodes);
					}

					if(Giveaways == null)
					{
						task.SetResult(Messages.ParseGiveawaysEmpty("PlayBlink"));
					}

					Tools.RemoveBlacklistedGames(Giveaways, blackList);
				}
				else
				{
					task.SetResult(Messages.ParseGiveawaysFoundMatchGiveaways("PlayBlink", Giveaways?.Count.ToString()));	
				}
			});
			return task.Task.Result;
		}

		private void PlayBlinkAddGiveaways(HtmlNodeCollection nodes)
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
							var pbGiveaway = new PlayBlinkGiveaway
							{
								Level = int.Parse(level.InnerText.Replace("L", "")),
								Name = name.InnerText,
								StoreId = storeId.Attributes["href"].Value.Split('/')[4],
								Id = id.Attributes["id"].Value.Replace("blink_", "")
							};

							var price =
								node.SelectSingleNode(".//div[@class='stats']/table/tr[3]/td/div[2]") ??
								node.SelectSingleNode(".//div[@class='stats']/table/tr[3]");

							pbGiveaway.Price =
								int.Parse(price.InnerText.Replace("Point(s)", "").Replace("Entrance Fee:", "").Trim());
							Giveaways?.Add(pbGiveaway);
						}
					}
				}
			}
		}

		#endregion
	}
}