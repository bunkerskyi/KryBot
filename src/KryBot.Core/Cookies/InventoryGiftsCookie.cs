using System;
using System.Net;

namespace KryBot.Core.Cookies
{
    public class InventoryGiftsCookie
    {
        public string Phpsessid { get; set; }

        public string Hash { get; set; }
           
        public string Steamid { get; set; }

        public CookieContainer Generate()
        {
            var cookie = new CookieContainer();
            var target = new Uri(Links.InventoryGifts);

            if (Phpsessid != null)
            {
                cookie.Add(new Cookie("PHPSESSID", Phpsessid) { Domain = target.Host });
            }

            if (Hash != null)
            {
                cookie.Add(new Cookie("hash", Hash) { Domain = target.Host });
            }

            if (Steamid != null)
            {
                cookie.Add(new Cookie("steamid", Steamid) { Domain = target.Host });
            }

            return cookie;
        }
    }
}
