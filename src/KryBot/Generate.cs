using System;
using System.Collections.Generic;
using System.Net;
using RestSharp;

namespace KryBot
{
    internal static class Generate
    {
        // Steam //
        public static CookieContainer Cookies_Steam(Bot bot)
        {
            var cookie = new CookieContainer();
            var target = new Uri("http://steamcommunity.com/");

            if (bot.Steam.Cookies.Sessid != null)
                cookie.Add(new Cookie("sessionid", bot.Steam.Cookies.Sessid) {Domain = target.Host});

            if (bot.Steam.Cookies.Login != null)
                cookie.Add(new Cookie("steamLogin", bot.Steam.Cookies.Login) {Domain = target.Host});

            if (bot.Steam.Cookies.RememberLogin != null)
                cookie.Add(new Cookie("steamRememberLogin", bot.Steam.Cookies.RememberLogin) {Domain = target.Host});

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
        public static CookieContainer Cookies_GameMiner(Bot bot)
        {
            var cookie = new CookieContainer();
            var target = new Uri("http://gameminer.net/");
            if (bot.GameMiner.Cookies.Token != null)
                cookie.Add(new Cookie("token", bot.GameMiner.Cookies.Token) {Domain = target.Host});

            if (bot.GameMiner.Cookies.Xsrf != null)
                cookie.Add(new Cookie("_xsrf", bot.GameMiner.Cookies.Xsrf) {Domain = target.Host});

            if (Properties.Settings.Default.Lang != null)
                cookie.Add(new Cookie("lang", Properties.Settings.Default.Lang) {Domain = target.Host});
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

        public static List<Parameter> SyncPostData_GameMiner(string xsrf)
        {
            var list = new List<Parameter>();

            var xsrfParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "_xsrf",
                Value = xsrf
            };
            list.Add(xsrfParam);

            return list;
        }

        // GameMiner //

        // SteamGifts //
        public static CookieContainer Cookies_SteamGifts(Bot bot)
        {
            var cookie = new CookieContainer();
            var target = new Uri("https://www.steamgifts.com/");

            if (bot.SteamGifts.Cookies.PhpSessId != null)
            {
                cookie.Add(new Cookie("PHPSESSID", bot.SteamGifts.Cookies.PhpSessId) {Domain = target.Host});
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
        public static CookieContainer Cookies_SteamCompanion(Bot bot)
        {
            var cookie = new CookieContainer();
            var target = new Uri("https://steamcompanion.com/");

            if (bot.SteamCompanion.Cookies.PhpSessId != null)
                cookie.Add(new Cookie("PHPSESSID", bot.SteamCompanion.Cookies.PhpSessId) {Domain = target.Host});

            if (bot.SteamCompanion.Cookies.UserC != null)
                cookie.Add(new Cookie("userc", bot.SteamCompanion.Cookies.UserC) {Domain = target.Host});

            if (bot.SteamCompanion.Cookies.UserId != null)
                cookie.Add(new Cookie("userid", bot.SteamCompanion.Cookies.UserId) {Domain = target.Host});

            if (bot.SteamCompanion.Cookies.UserT != null)
                cookie.Add(new Cookie("usert", bot.SteamCompanion.Cookies.UserT) {Domain = target.Host});

            return cookie;
        }

        public static List<Parameter> PostData_SteamCompanion(string hashId)
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
                Name = "hashID",
                Value = hashId
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

        // UseGamble //
        public static CookieContainer Cookies_UseGamble(Bot bot)
        {
            var cookie = new CookieContainer();
            var target = new Uri("http://usegamble.com/");

            if (bot.UseGamble.Cookies.PhpSessId != null)
                cookie.Add(new Cookie("PHPSESSID", bot.UseGamble.Cookies.PhpSessId) {Domain = target.Host});

            return cookie;
        }

        public static List<Parameter> PostData_UseGamble(string ga)
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

        public static List<Parameter> PageData_UseGamble(int page)
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

        // UseGamble //

        // SteamTrade //
        public static CookieContainer Cookies_SteamTrade(Bot bot)
        {
            var cookie = new CookieContainer();
            var target = new Uri("http://steamtrade.info/");

            if (bot.SteamTrade.Cookies.PhpSessId != null)
                cookie.Add(new Cookie("PHPSESSID", bot.SteamTrade.Cookies.PhpSessId) {Domain = target.Host});

            if (bot.SteamTrade.Cookies.DleUserId != null)
                cookie.Add(new Cookie("dle_user_id", bot.SteamTrade.Cookies.DleUserId) {Domain = target.Host});

            if (bot.SteamTrade.Cookies.DlePassword != null)
                cookie.Add(new Cookie("dle_password", bot.SteamTrade.Cookies.DlePassword) {Domain = target.Host});

            if (bot.SteamTrade.Cookies.PassHash != null)
                cookie.Add(new Cookie("passhash", bot.SteamTrade.Cookies.PassHash) {Domain = target.Host});

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

        // PlayBlink //
        public static CookieContainer Cookies_PlayBlink(Bot bot)
        {
            var cookie = new CookieContainer();
            var target = new Uri("http://playblink.com/");

            if (bot.PlayBlink.Cookies.PhpSessId != null)
            {
                cookie.Add(new Cookie("PHPSESSID", bot.PlayBlink.Cookies.PhpSessId) {Domain = target.Host});
            }

            cookie.Add(new Cookie("entry", "1") {Domain = target.Host});
            cookie.Add(new Cookie("level", (bot.PlayBlink.Level-1).ToString()) {Domain = target.Host});
            cookie.Add(new Cookie("order", "2") {Domain = target.Host});
            cookie.Add(new Cookie("ppage", "100") {Domain = target.Host});

            return cookie;
        }

        public static List<Parameter> PostData_PlayBlink(string game)
        {
            var list = new List<Parameter>();

            var doParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "do",
                Value = "blink"
            };
            list.Add(doParam);

            var gameParam = new Parameter
            {
                Type = ParameterType.GetOrPost,
                Name = "game",
                Value = game
            };
            list.Add(gameParam);

            return list;
        }

        // PlayBlink //
    }
}