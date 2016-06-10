using System;
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

		private Log JoinGiveaway(PlayBlinkGiveaway pbGiveaway)
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

				var response = Web.Post($"{Links.PlayBlink}?do=blink&game={pbGiveaway.Id}&captcha=1",
					Generate.PostData_PlayBlink(pbGiveaway.Id), list,
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
							return Messages.GiveawayJoined("PlayBlink", pbGiveaway.Name, pbGiveaway.Price, Points, 0);
						}

						var error = htmldoc.DocumentNode.SelectSingleNode("//div[@class='msgbox error']");
						if(error != null)
						{
							return Messages.GiveawayNotJoined("PlayBlink", pbGiveaway.Name, error.InnerText);
						}

						var captcha = htmldoc.DocumentNode.SelectSingleNode("//div[@class='flash_rules']");
						if(captcha != null)
						{
							return Messages.GiveawayNotJoined("PlayBlink", pbGiveaway.Name, "Captcha");
						}
					}
					return Messages.GiveawayNotJoined("PlayBlink", pbGiveaway.Name, "Error");
				}
				return Messages.GiveawayNotJoined("PlayBlink", pbGiveaway.Name, "Error");
			}
			return null;
		}

		public async Task<Log> JoinGiveawayAsync(PlayBlinkGiveaway pbGiveaway)
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = JoinGiveaway(pbGiveaway);
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		#endregion

		#region Parse

		private Log PlayBlinkGetProfile()
		{
			var response = Web.Get(Links.PlayBlink, new List<Parameter>(),
				Cookies.Generate(Level), new List<HttpHeader>(), String.Empty);

			if(response.RestResponse.Content != String.Empty)
			{
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(response.RestResponse.Content);

				var points = htmlDoc.DocumentNode.SelectSingleNode("//td[@id='points']");
				var level = htmlDoc.DocumentNode.SelectSingleNode("//a[@title='Your contribution level']/b");
				var username = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='usr_link']");
				if(points != null && level != null && username != null)
				{
					Points = Int32.Parse(points.InnerText.Split('P')[0].Split('\n')[1].Trim());
					Level = Int32.Parse(level.InnerText);

					Messages.ProfileLoaded();
					return Messages.ParseProfile("PlayBlink", Points, Level, username.InnerText);
				}
			}
			return Messages.ParseProfileFailed("PlayBlink");
		}

		public async Task<Log> PlayBlinkGetProfileAsync()
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = PlayBlinkGetProfile();
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		private Log PlayBlinkLoadGiveaways(Blacklist blackList)
		{
			Giveaways?.Clear();

			var response = Web.Get(Links.PlayBlink, new List<Parameter>(),
				Cookies.Generate(Level),
				new List<HttpHeader>(), String.Empty);

			if(response.RestResponse.Content != String.Empty)
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
					return
						new Log(
							$"{Messages.GetDateTime()} {{PlayBlink}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: 0",
							Color.White, true, true);
				}

				Tools.RemoveBlacklistedGames(Giveaways, blackList);
			}
			return
				new Log(
					$"{Messages.GetDateTime()} {{PlayBlink}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: {Giveaways?.Count}",
					Color.White, true, true);
		}

		public async Task<Log> PlayBlinkLoadGiveawaysAsync(Blacklist blackList)
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = PlayBlinkLoadGiveaways(blackList);
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		private void PlayBlinkAddGiveaways(HtmlNodeCollection nodes)
		{
			if(nodes != null)
			{
				foreach(var node in nodes)
				{
					if(node.SelectSingleNode(".//a[@class='button grey']") == null)
					{
						var level = node.SelectSingleNode(".//div[@class='min_level tooltip']");
						var name = node.SelectSingleNode(".//div[@class='name']/div");
						var storeId = node.SelectSingleNode("//div[@class='description']/a");
						var id = node.SelectSingleNode(".//a[@class='blink button blue']");
						if(level != null && name != null && storeId != null && id != null)
						{
							var pbGiveaway = new PlayBlinkGiveaway
							{
								Level = Int32.Parse(level.InnerText.Replace("L", "")),
								Name = name.InnerText,
								StoreId = storeId.Attributes["href"].Value.Split('/')[4],
								Id = id.Attributes["id"].Value.Replace("blink_", "")
							};

							var price =
								node.SelectSingleNode(".//div[@class='stats']/table/tr[3]/td/div[2]") ??
								node.SelectSingleNode(".//div[@class='stats']/table/tr[3]");

							pbGiveaway.Price =
								Int32.Parse(price.InnerText.Replace("Point(s)", "").Replace("Entrance Fee:", "").Trim());
							Giveaways?.Add(pbGiveaway);
						}
					}
				}
			}
		}

		#endregion
	}
}