using System;
using System.Net;

namespace KryBot.Core.Cookies
{
	public class GameMinerCookie
	{
		public string Token { get; set; }

		public string Xsrf { get; set; }

		public CookieContainer Generate(string lang)
		{
			var cookie = new CookieContainer();
			var target = new Uri(Links.GameMiner);

			if (Token != null)
			{
				cookie.Add(new Cookie("token", Token) {Domain = target.Host});
			}

			if (Xsrf != null)
			{
				cookie.Add(new Cookie("_xsrf", Xsrf) {Domain = target.Host});
			}

			if (lang != null)
			{
				cookie.Add(new Cookie("lang", lang) {Domain = target.Host});
			}

			return cookie;
		}
	}
}