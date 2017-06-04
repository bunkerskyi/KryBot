/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
*/
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