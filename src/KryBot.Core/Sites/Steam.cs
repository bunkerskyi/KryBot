using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Exceptionless.Json;
using HtmlAgilityPack;
using KryBot.Core.Cookies;
using KryBot.Core.Serializable.Steam;
using RestSharp;

namespace KryBot.Core.Sites
{
	public class Steam
	{
		public Steam()
		{
			Cookies = new SteamCookie();
		}

		public bool Enabled { get; set; }
		public string ProfileLink { get; set; }
		public SteamCookie Cookies { get; set; }

		public void Logout()
		{
			Cookies = new SteamCookie();
			Enabled = false;
		}

		#region JoinGroup

		public async Task<Log> Join(string url)
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var response = Web.Post(url, GenerateJoinParams(), Cookies.Generate());
				if (response.RestResponse.Content != string.Empty)
				{
					var htmlDoc = new HtmlDocument();
					htmlDoc.LoadHtml(response.RestResponse.Content);

					var node = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='btn_blue_white_innerfade btn_medium']");
					if (node != null)
					{
						task.SetResult(Messages.GroupJoined(url));
					}
					else
					{
						var error = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='error_ctn']");
						if (error != null && error.InnerText.Contains("You are already a member of this group."))
						{
							task.SetResult(Messages.GroupAlreadyMember(url));
						}
						else
						{
							task.SetResult(Messages.GroupNotJoinde(url));
						}
					}
				}
				else
				{
					task.SetResult(Messages.GroupNotJoinde(url));
				}
			});
			return task.Task.Result;
		}

		private List<Parameter> GenerateJoinParams()
		{
			var list = new List<Parameter>();

			var actionParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "action",
				Value = "join"
			};
			list.Add(actionParam);

			var sessidParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "sessionID",
				Value = Cookies.Sessid
			};
			list.Add(sessidParam);

			return list;
		}

		#endregion

		#region Parse

		public async Task<Log> CheckLogin()
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var response = Web.Get(Links.Steam, Cookies.Generate());

				if (response.RestResponse.Content != string.Empty)
				{
					var htmlDoc = new HtmlDocument();
					htmlDoc.LoadHtml(response.RestResponse.Content);

					var login =
						htmlDoc.DocumentNode.SelectSingleNode(
							"//a[contains(@class, 'user_avatar') and contains(@class, 'playerAvatar')]");
					if (login == null)
					{
						task.SetResult(Messages.ParseProfileFailed("Steam"));
					}
					else
					{
						ProfileLink = login.Attributes["href"].Value;
						task.SetResult(Messages.ParseProfile("Steam", login.Attributes["href"].Value.Split('/')[4]));
					}
				}
				else
				{
					task.SetResult(Messages.ParseProfileFailed("Steam"));
				}
			});
			return task.Task.Result;
		}

		public async Task<ProfileGamesList> GetUserGames()
		{
			var responseXmlProfile = await Web.GetAsync($"{ProfileLink}games?tab=all&xml=1");
			var serializer = new XmlSerializer(typeof(ProfileGamesList));
			TextReader reader = new StringReader(responseXmlProfile.RestResponse.Content);
			var games = (ProfileGamesList) serializer.Deserialize(reader);
			return games;
		}

		public async Task<string> GetGameName(string appId)
		{
			var responseJsonDetail = await Web.GetAsync($"{Links.SteamGameInfo}{appId}");

			if (responseJsonDetail.RestResponse.Content == "null" || responseJsonDetail.RestResponse.Content == "")
			{
				return null;
			}

			var json = responseJsonDetail.RestResponse.Content.Replace($"{{\"{appId}\":", "");
			var gameDetail = JsonConvert.DeserializeObject<GameDetail>(json.Substring(0, json.Length - 1));
			return gameDetail.success ? gameDetail.data.name : null;
		}

		#endregion
	}
}