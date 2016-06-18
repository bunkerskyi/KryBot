using System;
using System.Collections.Generic;
using System.Net;
using HtmlAgilityPack;
using KryBot.Core.Cookies;
using KryBot.Core.Giveaways;
using RestSharp;

namespace KryBot.Core.Sites
{
	public class GameAways
	{
		public GameAways()
		{
			Cookies = new GameAwaysCookie();
			Giveaways = new List<GameAwaysGiveaway>();
		}

		public bool Enabled { get; set; }
		public int Points { get; set; }
		public int JoinPointsLimit { get; set; } = 10000;
		public int PointsReserv { get; set; }
		public GameAwaysCookie Cookies { get; set; }
		public List<GameAwaysGiveaway> Giveaways { get; set; }

		public Log CheckLogin()
		{
			return new Log("");
		}

		#region Parse

		public static Log SteamGetProfile(Bot bot, bool echo)
		{
			var response = Web.Get(Links.GameAways, bot.Steam.Cookies.Generate());

			if (response.RestResponse.Content != "")
			{
				var htmlDoc = new HtmlDocument();
				htmlDoc.LoadHtml(response.RestResponse.Content);

				var login = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='username']");
				if (login == null)
				{
					return Messages.ParseProfileFailed("GameAways");
				}

				bot.Steam.ProfileLink = login.Attributes["href"].Value;
				return Messages.ParseProfile("GameAways", login.InnerText);
			}
			return Messages.ParseProfileFailed("GameAways");
		}

		#endregion

		#region Generate

		public CookieContainer GenerateCookies(Bot bot)
		{
			var cookie = new CookieContainer();
			var target = new Uri(Links.GameAways);

			if (bot.Steam.Cookies.Sessid != null)
			{
				cookie.Add(new Cookie("ASP.NET_SessionId", bot.GameAways.Cookies.SessionId) {Domain = target.Host});
			}

			return cookie;
		}

		public List<Parameter> JoinPostDatan(string id)
		{
			var list = new List<Parameter>();

			var idParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "_",
				Value = id
			};
			list.Add(idParam);

			return list;
		}

		#endregion
	}
}