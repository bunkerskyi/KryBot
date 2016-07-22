using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace KryBot.Core.Cookies
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class GameAwaysCookie
    {
        public string AspNetApplicationCookie { get; set; }

        public CookieContainer Generate()
        {
            var cookie = new CookieContainer();
            var target = new Uri(Links.GameAways);

            if (AspNetApplicationCookie != null)
            {
                cookie.Add(new Cookie(".AspNet.ApplicationCookie", AspNetApplicationCookie) {Domain = target.Host});
            }

            return cookie;
        }
    }
}