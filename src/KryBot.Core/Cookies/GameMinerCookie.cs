using System;
using System.Net;
using KryBot.Core.Properties;

namespace KryBot.Core.Cookies
{
	public class GameMinerCookie
	{
		public string Token { get; set; }

		public string Xsrf { get; set; }

		public CookieContainer Generate()
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

			if (Settings.Default.Lang != null)
			{
				cookie.Add(new Cookie("lang", Settings.Default.Lang) {Domain = target.Host});
			}

			return cookie;
		}
	}
}