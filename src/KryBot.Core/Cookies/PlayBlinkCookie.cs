using System;
using System.Net;

namespace KryBot.Core.Cookies
{
    public class PlayBlinkCookie : BaseCookie
    {
        public CookieContainer Generate()
        {
            var cookie = new CookieContainer();
            var target = new Uri(Links.PlayBlink);

            if (PhpSessId != null)
            {
                cookie.Add(new Cookie("PHPSESSID", PhpSessId) {Domain = target.Host});
            }

            return cookie;
        }
    }
}