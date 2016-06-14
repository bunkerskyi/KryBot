using System;
using System.Net;

namespace KryBot.Core.Cookies
{
	public class SteamCookie
	{
		public string Sessid { get; set; }

		public string Login { get; set; }

		public string LoginSecure { get; set; }

		public string MachineAuth { get; set; }

		public CookieContainer Generate()
		{
			var cookie = new CookieContainer();
			var target = new Uri(Links.Steam);

			if (Sessid != null)
			{
				cookie.Add(new Cookie("sessionid", Sessid) {Domain = target.Host});
			}

			if (Login != null)
			{
				cookie.Add(new Cookie("steamLogin", Login) {Domain = target.Host});
			}

			if (LoginSecure != null)
			{
				cookie.Add(new Cookie("steamLoginSecure", LoginSecure) {Domain = target.Host});
			}

			if(MachineAuth != null)
			{
				cookie.Add(new Cookie($"{LoginSecure?.Split('%')[0]}steamMachineAuth", MachineAuth) { Domain = target.Host });
			}

			return cookie;
		}
	}
}