using System;
using System.Net;

namespace KryBot.Core.Cookies
{
	public class SteamTradeCookie : BaseCookie
	{
		public string DleUserId { get; set; }

		public string DlePassword { get; set; }

		public string PassHash { get; set; }

		public CookieContainer Generate()
		{
			var cookie = new CookieContainer();
			var target = new Uri(Links.SteamTrade);

			if (PhpSessId != null)
				cookie.Add(new Cookie("PHPSESSID", PhpSessId) {Domain = target.Host});

			if (DleUserId != null)
				cookie.Add(new Cookie("dle_user_id", DleUserId) {Domain = target.Host});

			if (DlePassword != null)
				cookie.Add(new Cookie("dle_password", DlePassword) {Domain = target.Host});

			if (PassHash != null)
				cookie.Add(new Cookie("passhash", PassHash) {Domain = target.Host});

			return cookie;
		}
	}
}