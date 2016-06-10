using System;
using System.Net;

namespace KryBot.Core.Cookies
{
	public class PlayBlinkCookie : BaseCookie
	{
		public CookieContainer Generate(int level)
		{
			var cookie = new CookieContainer();
			var target = new Uri(Links.PlayBlink);

			if (PhpSessId != null)
			{
				cookie.Add(new Cookie("PHPSESSID", PhpSessId) {Domain = target.Host});
			}

			cookie.Add(new Cookie("entry", "1") {Domain = target.Host});
			cookie.Add(new Cookie("level", level.ToString()) {Domain = target.Host});
			cookie.Add(new Cookie("order", "2") {Domain = target.Host});
			cookie.Add(new Cookie("ppage", "100") {Domain = target.Host});

			return cookie;
		}
	}
}