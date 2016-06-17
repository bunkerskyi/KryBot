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
using KryBot.Core.Properties;
using KryBot.Core.Serializable.GameMiner;
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
		public int Points { get; set; }
		public int Level { get; set; }
		public int JoinPointsLimit { get; set; } = 50;
		public int PointsReserv { get; set; }
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

					if (lot.Price > Points || lot.Price > JoinPointsLimit)
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

		private async Task<Log> JoinGiveaway(GameMinerGiveaway giveaway)
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
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
						var jsonresponse = JsonConvert.DeserializeObject<JsonResponse>(response.RestResponse.Content);
						if (jsonresponse.Status == "ok")
						{
							Points = jsonresponse.Coal;
							task.SetResult(Messages.GiveawayJoined("GameMiner", giveaway.Name, giveaway.Price, jsonresponse.Coal));
						}
						else
						{
							var jresponse = JsonConvert.DeserializeObject<JsonResponseError>(response.RestResponse.Content);
							task.SetResult(Messages.GiveawayNotJoined("GameMiner", giveaway.Name, jresponse.Error.Message));
						}
					}
					catch (JsonReaderException)
					{
						task.SetResult(Messages.GiveawayNotJoined("GameMiner", giveaway.Name, response.RestResponse.Content));
					}
				}
				else
				{
					task.SetResult(Messages.GiveawayNotJoined("GameMiner", giveaway.Name, "Content is empty"));
				}
			});
			return task.Task.Result;
		}

		public async Task Join(Blacklist blacklist)
		{
			LogMessage.Instance.AddMessage(await LoadGiveaways(blacklist));

			if (Giveaways?.Count > 0)
			{
				if (Settings.Default.Sort)
				{
					if (Settings.Default.SortToMore)
					{
						Giveaways.Sort((a, b) => b.Price.CompareTo(a.Price));
					}
					else
					{
						Giveaways.Sort((a, b) => a.Price.CompareTo(b.Price));
					}
				}

				foreach (var giveaway in Giveaways)
				{
					if (giveaway.Price <= Points && PointsReserv <= Points - giveaway.Price)
					{
						LogMessage.Instance.AddMessage(await JoinGiveaway(giveaway));
					}
				}
			}
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

		public async Task<Log> CheckLogin()
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
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
						Points = int.Parse(coal.InnerText);
						Level = int.Parse(level.InnerText);

						task.SetResult(Messages.ParseProfile("GameMiner", Points, Level, username.InnerText.Trim().Replace("\n1", "")));
					}
					else
					{
						task.SetResult(Messages.ParseProfileFailed("GameMiner"));
					}
				}
				else
				{
					task.SetResult(Messages.ParseProfileFailed("GameMiner"));
				}
			});
			return task.Task.Result;
		}

		public async Task<Log> CheckWon()
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var response = Web.Get(Links.GameMinerWon, Cookies.Generate(), UserAgent);
				if (response.RestResponse.Content != string.Empty)
				{
					var htmlDoc = new HtmlDocument();
					htmlDoc.LoadHtml(response.RestResponse.Content);

					var nodes = htmlDoc.DocumentNode.SelectNodes(
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
							task.SetResult(Messages.GiveawayHaveWon("GameMiner", nodes.Count, Links.GameMinerWon));
						}
					}
				}
				task.SetResult(null);
			});
			return task.Task.Result;
		}

		private async Task<Log> LoadGiveaways(Blacklist blackList)
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
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
					task.SetResult(Messages.ParseGiveawaysEmpty(content, "GameMiner"));
				}
				else
				{
					blackList.RemoveGames(Giveaways);
					task.SetResult(Messages.ParseGiveawaysFoundMatchGiveaways(content, "GameMiner", Giveaways.Count.ToString()));
				}
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

		public async Task<Log> Sync() //TODO Move log response in Messages
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var response = Web.Post(Links.GameMinerSync, GenerateSyncData(), Cookies.Generate(), UserAgent);
				if (response.RestResponse.Content != "")
				{
					task.SetResult(response.RestResponse.StatusCode == HttpStatusCode.OK
						? new Log($"{Messages.GetDateTime()} {{GameMiner}} Sync success", Color.Green, true)
						: new Log($"{Messages.GetDateTime()} {{GameMiner}} Sync failed", Color.Red, false));
				}
				else
				{
					task.SetResult(Messages.ParseProfileFailed("GameMiner"));
				}
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