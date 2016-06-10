using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
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
	public class GameMiner
	{
		public GameMiner()
		{
			Cookies = new GameMinerCookie();
			Giveaways = new List<GameMinerGiveaway>();
		}

		public bool Enabled { get; set; }
		public int Coal { get; set; }
		public int Level { get; set; }
		public int JoinCoalLimit { get; set; } = 50;
		public int CoalReserv { get; set; }
		public bool Sandbox { get; set; } = true;
		public bool Regular { get; set; } = true;
		public bool FreeGolden { get; set; } = true;
		public bool OnlyGifts { get; set; }
		public bool NoRegion { get; set; }
		public GameMinerCookie Cookies { get; set; }
		public List<GameMinerGiveaway> Giveaways { get; set; }

		public void Logout()
		{
			Cookies = new GameMinerCookie();
			Enabled = false;
		}

		#region JoinGivaway

		private Log JoinGiveaway(GameMinerGiveaway giveaway, string userAgent)
		{
			Thread.Sleep(400);

			var response = Web.Post($"{Links.GameMiner}giveaway/enter/{giveaway.Id}?{(giveaway.IsSandbox ? "sandbox" : "coal")}_page={giveaway.Page}",
					Generate.PostData_GameMiner(Cookies.Xsrf), new List<HttpHeader>(),
					Cookies.Generate(), userAgent);

			if(response.RestResponse.Content != String.Empty)
			{
				try
				{
					var jsonresponse =
						JsonConvert.DeserializeObject<JsonResponse>(response.RestResponse.Content);
					if(jsonresponse.Status == "ok")
					{
						Coal = jsonresponse.Coal;
						return Messages.GiveawayJoined("GameMiner", giveaway.Name, giveaway.Price, jsonresponse.Coal,
							0);
					}
					var jresponse =
						JsonConvert.DeserializeObject<JsonResponseError>(response.RestResponse.Content);
					return Messages.GiveawayNotJoined("GameMiner", giveaway.Name, jresponse.Error.Message);
				}
				catch(JsonReaderException)
				{
					return Messages.GiveawayNotJoined("GameMiner", giveaway.Name, response.RestResponse.Content);
				}
			}
			return Messages.GiveawayNotJoined("GameMiner", giveaway.Name, "Content is empty");
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
			var response = Web.Get(Links.GameMiner, new List<Parameter>(),
				Cookies.Generate(), new List<HttpHeader>(), userAgent);

			if(response.RestResponse.Content != String.Empty)
			{
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(response.RestResponse.Content);

				var coal = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='user__coal']");
				var level = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='g-level-icon']");
				var username = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='dashboard__user-name']");

				if(coal != null && level != null && username != null)
				{
					Coal = Int32.Parse(coal.InnerText);
					Level = Int32.Parse(level.InnerText);

					return Messages.ParseProfile("GameMiner", Coal, Level,
						username.InnerText.Trim().Replace("\n1", ""));
				}
			}
			return Messages.ParseProfileFailed("GameMiner");
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
			var response = Web.Get($"{Links.GameMiner}giveaways/won", new List<Parameter>(),
				Cookies.Generate(), new List<HttpHeader>(), userAgent);

			if(response.RestResponse.Content != "")
			{
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(response.RestResponse.Content);

				var nodes =
					htmlDoc.DocumentNode.SelectNodes(
						"//tbody[@class='giveaways__giveaways']//td[@class='valign-middle m-table-state-finished']");

				if(nodes != null)
				{
					for(var i = 0; i < nodes.Count; i++)
					{
						if(!nodes[i].InnerText.Contains("требует подтверждения") &&
							!nodes[i].InnerText.Contains("to be confirmed"))
						{
							nodes.Remove(nodes[i]);
							i--;
						}
					}

					if(nodes.Count > 0)
					{
						return Messages.GiveawayHaveWon("GameMiner", nodes.Count, "http://gameminer.net/giveaways/won");
					}
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

		private Log LoadGiveaways(Blacklist blackList, string userAgent)
		{
			var content = String.Empty;
			Giveaways?.Clear();
			var pages = 0;

			if(FreeGolden)
			{
				var goldenFreesResponse = Web.Get($"{Links.GameMiner}" +
												  "api/giveaways/golden?page=1&count=10&q=&type=regular&enter_price=on&sortby=finish&order=asc&filter_entered=on",
					new List<Parameter>(),
					Cookies.Generate(),
					new List<HttpHeader>(), userAgent);

				if(goldenFreesResponse.RestResponse.Content != "")
				{
					var goldenFreeJsonResponse =
						JsonConvert.DeserializeObject<JsonRootObject>(
							goldenFreesResponse.RestResponse.Content);
					AddGiveaways(goldenFreeJsonResponse);

					content +=
						$"{Messages.GetDateTime()} {{GameMiner}} {strings.ParseLoadGiveaways_Found} {goldenFreeJsonResponse.Total} " +
						$"{strings.ParseLoadGiveaways_FreeGoldenGiveawaysIn} {goldenFreeJsonResponse.LastPage} {strings.ParseLoadGiveaways_Pages}\n";

					pages = goldenFreeJsonResponse.LastPage;

					if(pages > 1)
					{
						for(var i = 1; i < pages + 1; i++)
						{
							goldenFreesResponse = Web.Get($"{Links.GameMiner}" +
														  $"/api/giveaways/golden?page={i + 1}&count=10&q=&type=regular&enter_price=on&sortby=finish&order=asc&filter_entered=on",
								new List<Parameter>(),
								Cookies.Generate(),
								new List<HttpHeader>(),
								userAgent);
							goldenFreeJsonResponse =
								JsonConvert.DeserializeObject<JsonRootObject>(
									goldenFreesResponse.RestResponse.Content);

							AddGiveaways(goldenFreeJsonResponse);
						}
					}
				}
			}

			if(Regular)
			{
				var regularResponse = Web.Get($"{Links.GameMiner}" +
											  $"api/giveaways/coal?page=1&count=10&q=&type={(OnlyGifts ? "regular" : "any")}&enter_price=on&sortby=finish&order=asc&filter_entered=on",
					new List<Parameter>(),
					Cookies.Generate(),
					new List<HttpHeader>(), userAgent);

				var regularJsonResponse =
					JsonConvert.DeserializeObject<JsonRootObject>(
						regularResponse.RestResponse.Content);
				AddGiveaways(regularJsonResponse);
				content +=
					$"{Messages.GetDateTime()} {{GameMiner}} {strings.ParseLoadGiveaways_Found} {regularJsonResponse.Total} " +
					$"{strings.ParseLoadGiveaways_RegularGiveawaysIn} {regularJsonResponse.LastPage} {strings.ParseLoadGiveaways_Pages}\n";

				pages = regularJsonResponse.LastPage;

				if(pages > 1)
				{
					for(var i = 1; i < pages + 1; i++)
					{
						var regularGiftsResponse = Web.Get($"{Links.GameMiner}" +
														   $"api/giveaways/coal?page={i + 1}&count=10&q=&type={(OnlyGifts ? "regular" : "any")}&enter_price=on&sortby=finish&order=asc&filter_entered=on",
							new List<Parameter>(),
							Cookies.Generate(),
							new List<HttpHeader>(), userAgent);

						var regularGiftJsonResponse =
							JsonConvert.DeserializeObject<JsonRootObject>(
								regularGiftsResponse.RestResponse.Content);

						AddGiveaways(regularGiftJsonResponse);
					}
				}
			}

			if(Sandbox)
			{
				var sandboxGiftsResponse = Web.Get($"{Links.GameMiner}" +
												   $"api/giveaways/sandbox?page=1&count=10&q=&type={(OnlyGifts ? "regular" : "any")}&enter_price=on&sortby=finish&order=asc&filter_entered=on",
					new List<Parameter>(),
					Cookies.Generate(),
					new List<HttpHeader>(), userAgent);

				if(sandboxGiftsResponse.RestResponse.Content != "")
				{
					var sandboxGiftJsonResponse =
						JsonConvert.DeserializeObject<JsonRootObject>(
							sandboxGiftsResponse.RestResponse.Content);
					AddGiveaways(sandboxGiftJsonResponse);

					content +=
						$"{Messages.GetDateTime()} {{GameMiner}} {strings.ParseLoadGiveaways_Found} {sandboxGiftJsonResponse.Total} " +
						$"{strings.ParseLoadGiveaways_RegularGiveawaysIn} {sandboxGiftJsonResponse.LastPage} {strings.ParseLoadGiveaways_Pages}\n";

					pages = sandboxGiftJsonResponse.LastPage;
				}

				if(pages > 1)
				{
					for(var i = 1; i < pages + 1; i++)
					{
						var regularGiftsResponse = Web.Get($"{Links.GameMiner}" +
														   $"api/giveaways/sandbox?page={i + 1}&count=10&q=&type={(OnlyGifts ? "regular" : "any")}&enter_price=on&sortby=finish&order=asc&filter_entered=on",
							new List<Parameter>(),
							Cookies.Generate(),
							new List<HttpHeader>(), userAgent);

						if(regularGiftsResponse.RestResponse.Content != "")
						{
							var regularGiftJsonResponse =
								JsonConvert.DeserializeObject<JsonRootObject>(
									regularGiftsResponse.RestResponse.Content);

							AddGiveaways(regularGiftJsonResponse);
						}
					}
				}
			}

			if(Giveaways == null)
			{
				return
					new Log(
						$"{content}{Messages.GetDateTime()} {{GameMiner}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: 0",
						Color.White, true, true);
			}

			Tools.RemoveBlacklistedGames(Giveaways, blackList);

			return
				new Log(
					$"{content}{Messages.GetDateTime()} {{GameMiner}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: {Giveaways.Count}",
					Color.White, true, true);
		}

		public async Task<Log> LoadGiveawaysAsync(Blacklist blackList, string userAgent)
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = LoadGiveaways(blackList, userAgent);
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		#endregion

		#region Sync

		private Log SyncAccount(string userAgent)
		{
			var response = Web.Post("http://gameminer.net/account/sync",
				Generate.SyncPostData_GameMiner(Cookies.Xsrf), new List<HttpHeader>(),
				Cookies.Generate(), userAgent);

			if(response.RestResponse.Content != "")
			{
				if(response.RestResponse.StatusCode == HttpStatusCode.OK)
				{
					return new Log($"{Messages.GetDateTime()} {{GameMiner}} Sync success", Color.Green,
						true, true);
				}
				return new Log($"{Messages.GetDateTime()} {{GameMiner}} Sync failed", Color.Red, false,
					true);
			}
			return
				new Log($"{Messages.GetDateTime()} {{GameMiner}} {strings.ParseProfile_LoginOrServerError}",
					Color.Red, false,
					true);
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

		private void AddGiveaways(JsonRootObject json)
		{
			if (json != null)
			{
				foreach (var giveaway in json.Giveaways)
				{
					var lot = new GameMinerGiveaway();
					if (giveaway.Golden && giveaway.Price != 0)
					{
						break;
					}

					if (lot.Price > Coal || lot.Price > JoinCoalLimit)
					{
						break;
					}

					lot.Name = giveaway.Game.Name;
					lot.Id = giveaway.Code;
					lot.IsRegular = giveaway.Sandbox == null;
					lot.IsSandbox = giveaway.Sandbox != null;
					lot.IsGolden = giveaway.Golden;
					lot.Page = json.Page;
					lot.Price = giveaway.Price;

					if (giveaway.RegionlockTypeId != null)
					{
						lot.Region = giveaway.RegionlockTypeId;
					}

					if (giveaway.Game.Url != "javascript:void(0);")
					{
						lot.StoreId = giveaway.Game.Url.Split('/')[4];
					}
					else
					{
						break;
					}

					Giveaways?.Add(lot);
				}
			}
		}

		public class JsonProperties
		{
			public string RankColor { get; set; }
			public string Hat { get; set; }
			public string Rank { get; set; }
			public string NickColor { get; set; }
		}

		public class JsonAccount
		{
			public string Name { get; set; }
			public int Level { get; set; }
			public int Id { get; set; }
			public string Avatar { get; set; }
			public bool Active { get; set; }
			public string RoleId { get; set; }
			public JsonProperties Properties { get; set; }
		}

		public class JsonGame
		{
			public int Updated { get; set; }
			public string Name { get; set; }
			public string Img { get; set; }
			public string Url { get; set; }
			public string Image { get; set; }
			public object SteamApp { get; set; }
			public string GameAppTypeId { get; set; }
			public string GameClassId { get; set; }
			public string ForeignId { get; set; }
			public int Id { get; set; }
		}

		public class JsonAward
		{
			public int Res { get; set; }
			public int Exp { get; set; }
		}

		public class JsonGiveaway
		{
			public string State { get; set; }
			public bool Golden { get; set; }
			public JsonAccount Account { get; set; }
			public string Code { get; set; }
			public object GiftAsKey { get; set; }
			public int Created { get; set; }
			public bool IsMember { get; set; }
			public int Price { get; set; }
			public object Winner { get; set; }
			public object Sandbox { get; set; }
			public int KeyStat { get; set; }
			public bool ConstraintCheck { get; set; }
			public JsonGame Game { get; set; }
			public int Finish { get; set; }
			public JsonAward Award { get; set; }
			public string RegionlockTypeId { get; set; }
			public List<object> Regionlocks { get; set; }
			public int Entries { get; set; }
			public string GiveawayTypeId { get; set; }
			public object Entered { get; set; }
			public List<object> Constraints { get; set; }
		}

		private class JsonRootObject
		{
			public int Count { get; set; }
			public int ServerTime { get; set; }
			public List<JsonGiveaway> Giveaways { get; set; }
			public int LastPage { get; set; }
			public int Total { get; set; }
			public int Page { get; set; }
		}

		private class JsonResponse
		{
			public string Status { get; set; }
			public int Coal { get; set; }
			public int Gold { get; set; }
		}

		public class JsonResponseErrorDetail
		{
			public string Message { get; set; }
			public string Code { get; set; }
		}

		public class JsonResponseError
		{
			public JsonResponseErrorDetail Error { get; set; }
		}
	}
}