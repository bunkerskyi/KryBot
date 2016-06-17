using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace KryBot.Core.Helpers
{
	public  static class CookieHelper
	{
		public static List<Cookie> CookieContainer_ToList(CookieContainer container)
		{
			var cookies = new List<Cookie>();

			var table = (Hashtable)container.GetType().InvokeMember("m_domainTable",
				BindingFlags.NonPublic |
				BindingFlags.GetField |
				BindingFlags.Instance,
				null,
				container,
				new object[] { });

			foreach(var key in table.Keys)
			{
				Uri uri;

				var domain = key as string;

				if(domain == null)
					continue;

				if(domain.StartsWith("."))
					domain = domain.Substring(1);

				var address = $"http://{domain}/";

				if(Uri.TryCreate(address, UriKind.RelativeOrAbsolute, out uri) == false)
					continue;

				foreach(Cookie cookie in container.GetCookies(uri))
				{
					if(cookies.Contains(cookie) == false)
					{
						cookies.Add(cookie);
					}
				}
				return cookies;
			}
			return null;
		}

		public static string GetSessCookieInresponse(CookieContainer cookies, string domain, string cookieName)
		{
			if(cookies?.Count > 0)
			{
				var list = CookieContainer_ToList(cookies);

				return
					(from cookie in list where cookie.Name == cookieName && cookie.Domain == domain select cookie.Value)
						.FirstOrDefault();
			}
			return null;
		}
	}
}
