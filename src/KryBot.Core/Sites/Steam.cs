using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Exceptionless.Json;
using HtmlAgilityPack;
using KryBot.Core.Cookies;
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
		public SteamCookie Cookies { get; private set; }

		public void Logout()
		{
			Cookies = new SteamCookie();
			Enabled = false;
		}

		#region JoinGroup

		public Log JoinGroup(string url, List<Parameter> parameters)
		{
			var response = Web.Post(url, parameters, new List<HttpHeader>(), Cookies.Generate());
			if(response.RestResponse.Content != "")
			{
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(response.RestResponse.Content);

				var node = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='btn_blue_white_innerfade btn_medium']");
				if(node != null)
				{
					return Messages.GroupJoined(url);
				}

				var error = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='error_ctn']");
				if(error != null && error.InnerText.Contains("You are already a member of this group."))
				{
					return Messages.GroupAlreadyMember(url);
				}
			}
			return Messages.GroupNotJoinde(url);
		}

		public async Task<Log> JoinGroupAsync(string url, List<Parameter> parameters)
		{
			var task = new TaskCompletionSource<Log>();
			await Task.Run(() =>
			{
				var result = JoinGroup(url, parameters);
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		#endregion

		#region Parse

		private Log GetProfile()
		{
			var response = Web.Get(Links.Steam, new List<Parameter>(),
				Cookies.Generate(), new List<HttpHeader>(), String.Empty);

			if(response.RestResponse.Content != String.Empty)
			{
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(response.RestResponse.Content);

				var login =
					htmlDoc.DocumentNode.SelectSingleNode(
						"//a[contains(@class, 'user_avatar') and contains(@class, 'playerAvatar')]");
				if(login == null)
				{
					//ProfileLoaded();
					return Messages.ParseProfileFailed("Steam");
				}

				ProfileLink = login.Attributes["href"].Value;
				return Messages.ParseProfile("Steam", login.Attributes["href"].Value.Split('/')[4]);
			}
			return Messages.ParseProfileFailed("Steam");
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

		public async Task<Classes.ProfileGamesList> GetUserGames()
		{
			var responseXmlProfile =
				await
					Web.GetAsync($"{ProfileLink}games?tab=all&xml=1", new List<Parameter>(), new CookieContainer(),
						new List<HttpHeader>(), "");
			var serializer = new XmlSerializer(typeof(Classes.ProfileGamesList));
			TextReader reader = new StringReader(responseXmlProfile.RestResponse.Content);
			var games = (Classes.ProfileGamesList)serializer.Deserialize(reader);
			return games;
		}

		public async Task<string> GetGameName(string appId)
		{
			var responseJsonDetail =
				await
					Web.GetAsync($"http://store.steampowered.com/api/appdetails?appids={appId}",
						new List<Parameter>(),
						new CookieContainer(), new List<HttpHeader>(), "");

			if(responseJsonDetail.RestResponse.Content == "null" || responseJsonDetail.RestResponse.Content == "")
			{
				return null;
			}

			var json = responseJsonDetail.RestResponse.Content.Replace($"{{\"{appId}\":", "");
			var gameDetail = JsonConvert.DeserializeObject<Classes.GameDetail>(json.Substring(0, json.Length - 1));
			return gameDetail.success ? gameDetail.data.name : null;
		}

		#endregion
	}
}