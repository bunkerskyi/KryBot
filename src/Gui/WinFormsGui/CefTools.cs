using System.Collections.Generic;
using System.Net;
using CefSharp;
using KryBot.Core.Helpers;
using Cookie = CefSharp.Cookie;

namespace KryBot.Gui.WinFormsGui
{
    public static class CefTools
    {
        public static string GetUserAgent()
        {
            return
                $"Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/{Cef.ChromiumVersion} Safari/537.36";
        }

        public static List<Cookie> CookieContainerToCefCookie(CookieContainer container)
        {
            var cefCookies = new List<Cookie>();
            foreach (var cookie in CookieHelper.CookieContainer_ToList(container))
            {
                cefCookies.Add(new Cookie
                {
                    Domain = cookie.Domain,
                    Name = cookie.Name,
                    Value = cookie.Value
                });
            }
            return cefCookies;
        }
    }
}