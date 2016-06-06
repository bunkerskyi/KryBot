using System;
using System.Net;

namespace KryBot.Core.Cookies
{
	public class SteamCompanionCookie : BaseCookie
	{
		public string UserId { get; set; }

		public string UserC { get; set; }

		public string UserT { get; set; }

		public CookieContainer Generate()
		{
			var cookie = new CookieContainer();
			var target = new Uri(Links.SteamCompanion);

			if(PhpSessId != null)
				cookie.Add(new Cookie("PHPSESSID", PhpSessId) { Domain = target.Host });

			if(UserC != null)
				cookie.Add(new Cookie("userc", UserC) { Domain = target.Host });

			if(UserId != null)
				cookie.Add(new Cookie("userid", UserId) { Domain = target.Host });

			if(UserT != null)
				cookie.Add(new Cookie("usert", UserT) { Domain = target.Host });

			return cookie;
		}
	}
}