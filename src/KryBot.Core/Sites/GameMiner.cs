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
using KryBot.Core.Json.GameMiner;
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
		public bool NoRegion { get; set; }
		public GameMinerCookie Cookies { get; set; }
		public List<GameMinerGiveaway> Giveaways { get; set; }
		public string UserAgent { get; set; }

		public void Logout()
		{
			Cookies = new GameMinerCookie();
			Enabled = false;
		}

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

		#region JoinGivaway

		private Log JoinGiveaway(GameMinerGiveaway giveaway)
		{
			Thread.Sleep(400);

			var response =
				Web.Post(
					$"{Links.GameMiner}giveaway/enter/{giveaway.Id}?{(giveaway.IsSandbox ? "sandbox" : "coal")}_page={giveaway.Page}",
					GenerateJoinData(), Cookies.Generate(), UserAgent);

			if (response.RestResponse.Content != string.Empty)
			{
				try
				{
					var jsonresponse =
						JsonConvert.DeserializeObject<JsonResponse>(response.RestResponse.Content);
					if (jsonresponse.Status == "ok")
					{
						Coal = jsonresponse.Coal;
						return Messages.GiveawayJoined("GameMiner", giveaway.Name, giveaway.Price, jsonresponse.Coal);
					}
					var jresponse =
						JsonConvert.DeserializeObject<JsonResponseError>(response.RestResponse.Content);
					return Messages.GiveawayNotJoined("GameMiner", giveaway.Name, jresponse.Error.Message);
				}
				catch (JsonReaderException)
				{
					return Messages.GiveawayNotJoined("GameMiner", giveaway.Name, response.RestResponse.Content);
				}
			}
			return Messages.GiveawayNotJoined("GameMiner", giveaway.Name, "Content is empty");
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

		private List<Parameter> GenerateJoinData()
		{
			var list = new List<Parameter>();

			var xsrfParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "_xsrf",
				Value = Cookies.Xsrf
			};
			list.Add(xsrfParam);

			var jsonParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "json",
				Value = "true"
			};
			list.Add(jsonParam);

			return list;
		}

		#endregion

		#region Parse

		private Log GetProfile()
		{
			var response = Web.Get(Links.GameMiner, Cookies.Generate(), UserAgent);

			if (response.RestResponse.Content != string.Empty)
			{
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(response.RestResponse.Content);

				var coal = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='user__coal']");
				var level = htmlDoc.DocumentNode.SelectSingleNode("//span[@class='g-level-icon']");
				var username = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='dashboard__user-name']");

				if (coal != null && level != null && username != null)
				{
					Coal = int.Parse(coal.InnerText);
					Level = int.Parse(level.InnerText);

					return Messages.ParseProfile("GameMiner", Coal, Level,
						username.InnerText.Trim().Replace("\n1", ""));
				}
			}
			return Messages.ParseProfileFailed("GameMiner");
		}

		public async Task<Log> CheckLogin()
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
			var response = Web.Get(Links.GameMinerWon, Cookies.Generate(), UserAgent);

			if (response.RestResponse.Content != "")
			{
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(response.RestResponse.Content);

				var nodes =
					htmlDoc.DocumentNode.SelectNodes(
						"//tbody[@class='giveaways__giveaways']//td[@class='valign-middle m-table-state-finished']");

				if (nodes != null)
				{
					for (var i = 0; i < nodes.Count; i++)
					{
						if (!nodes[i].InnerText.Contains("требует подтверждения") &&
						    !nodes[i].InnerText.Contains("to be confirmed"))
						{
							nodes.Remove(nodes[i]);
							i--;
						}
					}

					if (nodes.Count > 0)
					{
						return Messages.GiveawayHaveWon("GameMiner", nodes.Count, Links.GameMinerWon);
					}
				}
			}
			return null;
		}

		public async Task<Log> CheckWon()
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
			var content = string.Empty;

			Giveaways?.Clear();

			if (FreeGolden)
			{
				content += LoadGiveawaysByUrl(Links.GameMinerGoldenGiveaways, strings.ParseLoadGiveaways_FreeGoldenGiveawaysIn);
			}

			if (Regular)
			{
				content += LoadGiveawaysByUrl(Links.GameMinerRegularGiveaways, strings.ParseLoadGiveaways_RegularGiveawaysIn);
			}

			if (Sandbox)
			{
				content += LoadGiveawaysByUrl(Links.GameMinerSandboxGiveaways, strings.ParseLoadGiveaways_SandboxGiveawaysIn);
			}

			if (Giveaways == null)
			{
				return Messages.ParseGiveawaysEmpty(content, "GameMiner");
			}

			Tools.RemoveBlacklistedGames(Giveaways, blackList);

			return Messages.ParseGiveawaysFoundMatchGiveaways(content, "GameMiner", Giveaways.Count.ToString());
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

		private string LoadGiveawaysByUrl(string url, string message)
		{
			var response = Web.Get(url, Cookies.Generate(), UserAgent);

			if (response.RestResponse.Content != string.Empty)
			{
				var jsonResponse = JsonConvert.DeserializeObject<JsonRootObject>(response.RestResponse.Content);
				AddGiveaways(jsonResponse);

				if (jsonResponse.last_page > 1)
				{
					for (var i = 1; i < jsonResponse.last_page + 1; i++)
					{
						response = Web.Get($"{url}&page={i + 1}", Cookies.Generate(), UserAgent);
						jsonResponse = JsonConvert.DeserializeObject<JsonRootObject>(response.RestResponse.Content);
						AddGiveaways(jsonResponse);
					}
				}

				return $"{Messages.GetDateTime()} {{GameMiner}} {strings.ParseLoadGiveaways_Found} {jsonResponse.Total} " +
				       $"{message} {jsonResponse.last_page} {strings.ParseLoadGiveaways_Pages}\n";
			}

			return string.Empty;
		}

		#endregion

		#region Sync

		private Log SyncAccount()
		{
			var response = Web.Post("http://gameminer.net/account/sync",
				GenerateSyncData(), Cookies.Generate(), UserAgent);

			if (response.RestResponse.Content != "")
			{
				if (response.RestResponse.StatusCode == HttpStatusCode.OK)
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

		public async Task<Log> Sync()
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = SyncAccount();
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		private List<Parameter> GenerateSyncData()
		{
			var list = new List<Parameter>();

			var xsrfParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "_xsrf",
				Value = Cookies.Xsrf
			};
			list.Add(xsrfParam);

			return list;
		}

		#endregion
	}
}