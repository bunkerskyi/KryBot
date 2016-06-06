using System;
using System.Collections.Generic;
using System.Net;
using KryBot.Core.Properties;
using RestSharp;

namespace KryBot.Core
{
	public static class Generate
	{
		// Steam //
		public static List<Parameter> PostData_SteamGroupJoin(string sessid)
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
				Value = sessid
			};
			list.Add(sessidParam);

			return list;
		}

		// Steam //

		// GameMiner //
		public static List<Parameter> PostData_GameMiner(string xsrf)
		{
			var list = new List<Parameter>();

			var xsrfParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "_xsrf",
				Value = xsrf
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

		public static List<Parameter> SyncPostData_GameMiner(string xsrf)
		{
			var list = new List<Parameter>();

			var xsrfParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "_xsrf",
				Value = xsrf
			};
			list.Add(xsrfParam);

			return list;
		}

		// GameMiner //

		// SteamGifts //
		public static List<Parameter> PostData_SteamGifts(string xsrfToken, string code, string action)
		{
			var list = new List<Parameter>();

			var xsrfParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "xsrf_token",
				Value = xsrfToken
			};
			list.Add(xsrfParam);

			var doParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "do",
				Value = action
			};
			list.Add(doParam);

			if (code != "")
			{
				var codeParam = new Parameter
				{
					Type = ParameterType.GetOrPost,
					Name = "code",
					Value = code
				};
				list.Add(codeParam);
			}

			return list;
		}

		// SteamGifts //

		// SteamCompanion //
		public static List<Parameter> PostData_SteamCompanion(string hashId)
		{
			var list = new List<Parameter>();

			var xsrfParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "script",
				Value = "enter"
			};
			list.Add(xsrfParam);

			var codeParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "hashID",
				Value = hashId
			};
			list.Add(codeParam);

			var tokenParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "token",
				Value = ""
			};
			list.Add(tokenParam);

			var doParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "action",
				Value = "enter_giveaway"
			};
			list.Add(doParam);

			return list;
		}

		// SteamCompanion //

		// UseGamble //
		public static List<Parameter> PostData_UseGamble(string ga)
		{
			var list = new List<Parameter>();

			var gaParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "ga",
				Value = ga
			};
			list.Add(gaParam);

			return list;
		}

		public static List<Parameter> PageData_UseGamble(int page)
		{
			var list = new List<Parameter>();

			var typeParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "type",
				Value = 1
			};
			list.Add(typeParam);

			var pageParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "page",
				Value = page
			};
			list.Add(pageParam);

			//TODO Добавить параметр AjaxToken, находится на странице 

			return list;
		}

		// UseGamble //

		// SteamTrade //
		public static List<Parameter> LoginData_SteamTrade()
		{
			var rnd = new Random();
			var list = new List<Parameter>();

			var xParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "x",
				Value = rnd.Next(0, 190)
			};
			list.Add(xParam);

			var yParam = new Parameter
			{
				Type = ParameterType.GetOrPost,
				Name = "y",
				Value = rnd.Next(0, 23)
			};
			list.Add(yParam);

			return list;
		}

		// Steamtrade //

		// PlayBlink //
		public static List<Parameter> PostData_PlayBlink(string game)
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

		// PlayBlink //
	}
}