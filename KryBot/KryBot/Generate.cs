using System;
using System.Collections.Generic;
using System.Net;
using RestSharp;

namespace KryBot
{
    internal class Generate
    {
        // Steam //
        public static CookieContainer Cookies_Steam(string sessid, string login)
        {
            var cookie = new CookieContainer();
            var target = new Uri("http://steamcommunity.com/");
            try
            {
                cookie.Add(new Cookie("sessionid", sessid) {Domain = target.Host});
                cookie.Add(new Cookie("steamLogin", login) {Domain = target.Host});
            }
            catch (CookieException)
            {
                return null;
            }
            return cookie;
        }

        public static List<Parameter> PostData_SteamGroupJoin(string sessid)
        {
            var list = new List<Parameter>();

            var actionParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "action",
                Value = "join"
            };
            list.Add(actionParam);

            var sessidParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "sessionID",
                Value = sessid
            };
            list.Add(sessidParam);

            return list;
        }

        // Steam //

        // GameMiner //
        public static CookieContainer Cookies_GameMiner(string token, string xsrf, string lang)
        {
            var cookie = new CookieContainer();
            var target = new Uri("http://gameminer.net/");
            try
            {
                cookie.Add(new Cookie("token", token) {Domain = target.Host});
                cookie.Add(new Cookie("_xsrf", xsrf) {Domain = target.Host});
                cookie.Add(new Cookie("lang", lang) {Domain = target.Host});
            }
            catch (Exception)
            {
                return null;
            }
            return cookie;
        }

        public static List<Parameter> PostData_GameMiner(string xsrf)
        {
            var list = new List<Parameter>();

            var xsrfParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "_xsrf",
                Value = xsrf
            };
            list.Add(xsrfParam);

            var jsonParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "json",
                Value = "true"
            };
            list.Add(jsonParam);

            return list;
        }

        // GameMiner //

        // SteamGifts //
        public static CookieContainer Cookies_SteamGifts(string phpSessId)
        {
            var cookie = new CookieContainer();
            var target = new Uri("http://www.steamgifts.com/");
            try
            {
                cookie.Add(new Cookie("PHPSESSID", phpSessId) {Domain = target.Host});
            }
            catch (Exception)
            {
                return null;
            }
            return cookie;
        }

        public static List<Parameter> PostData_SteamGifts(string xsrfToken, string code, string action)
        {
            var list = new List<Parameter>();

            var xsrfParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "xsrf_token",
                Value = xsrfToken
            };
            list.Add(xsrfParam);

            var doParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "do",
                Value = action
            };
            list.Add(doParam);

            if (code != "")
            {
                var codeParam = new Parameter
                {
                    Type = ParameterType.GetOrPost,
                    Name = "code",
                    Value = code
                };
                list.Add(codeParam);
            }

            return list;
        }

        // SteamGifts //

        // SteamCompanion //
        public static CookieContainer Cookies_SteamCompanion(string phpSessId, string userc, string userid, string usert)
        {
            var cookie = new CookieContainer();
            var target = new Uri("https://steamcompanion.com/");
            try
            {
                cookie.Add(new Cookie("PHPSESSID", phpSessId) {Domain = target.Host});
                cookie.Add(new Cookie("userc", userc) {Domain = target.Host});
                cookie.Add(new Cookie("userid", userid) {Domain = target.Host});
                cookie.Add(new Cookie("usert", usert) {Domain = target.Host});
            }
            catch (Exception)
            {
                return null;
            }
            return cookie;
        }

        public static List<Parameter> PostData_SteamCompanion(string giftId)
        {
            var list = new List<Parameter>();

            var xsrfParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "script",
                Value = "enter"
            };
            list.Add(xsrfParam);

            var codeParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "giftID",
                Value = giftId
            };
            list.Add(codeParam);

            var tokenParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "token",
                Value = ""
            };
            list.Add(tokenParam);

            var doParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "action",
                Value = "enter_giveaway"
            };
            list.Add(doParam);

            return list;
        }

        // SteamCompanion //

        // SteamPortal //
        public static CookieContainer Cookies_SteamPortal(string phpSessId)
        {
            var cookie = new CookieContainer();
            var target = new Uri("http://steamportal.net/");
            try
            {
                cookie.Add(new Cookie("PHPSESSID", phpSessId) {Domain = target.Host});
            }
            catch (Exception)
            {
                return null;
            }
            return cookie;
        }

        public static List<Parameter> PostData_SteamPortal(string ga)
        {
            var list = new List<Parameter>();

            var gaParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "ga",
                Value = ga
            };
            list.Add(gaParam);

            return list;
        }

        public static List<Parameter> PageData_SteamPortal(int page)
        {
            var list = new List<Parameter>();

            var typeParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "type",
                Value = 1
            };
            list.Add(typeParam);

            var pageParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "page",
                Value = page
            };
            list.Add(pageParam);

            return list;
        }

        // Steamportal //

        // GameAways //
        public static CookieContainer Cookies_GameAways(string phpSessId)
        {
            var cookie = new CookieContainer();
            var target = new Uri("http://www.gameaways.com/");
            try
            {
                cookie.Add(new Cookie("ASP.NET_SessionId", phpSessId) {Domain = target.Host});
            }
            catch (Exception)
            {
                return null;
            }
            return cookie;
        }

        public static List<Parameter> GetData_GameAways(string id)
        {
            var list = new List<Parameter>();

            var idParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "_",
                Value = id
            };
            list.Add(idParam);

            return list;
        }

        public static List<Parameter> AuthData_GameAways(string token)
        {
            var list = new List<Parameter>();

            var tokenParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "__RequestVerificationToken",
                Value = token
            };
            list.Add(tokenParam);

            var providerParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "provider",
                Value = "Staem"
            };
            list.Add(providerParam);

            return list;
        }

        // GameAways //

        // SteamTrade //
        public static CookieContainer Cookies_SteamTrade(string phpSessId, string dleUserId, string dlePassword,
            string passHash)
        {
            var cookie = new CookieContainer();
            var target = new Uri("http://steamtrade.info/");
            try
            {
                cookie.Add(new Cookie("PHPSESSID", phpSessId) {Domain = target.Host});
                cookie.Add(new Cookie("dle_user_id", dleUserId) {Domain = target.Host});
                cookie.Add(new Cookie("dle_password", dlePassword) {Domain = target.Host});
                cookie.Add(new Cookie("passhash", passHash) {Domain = target.Host});
            }
            catch (CookieException)
            {
                return null;
            }
            return cookie;
        }

        public static List<Parameter> LoginData_SteamTrade()
        {
            var rnd = new Random();
            var list = new List<Parameter>();

            var xParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "x",
                Value = rnd.Next(0, 190)
            };
            list.Add(xParam);

            var yParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "y",
                Value = rnd.Next(0, 23)
            };
            list.Add(yParam);

            return list;
        }

        // Steamtrade //
    }
}