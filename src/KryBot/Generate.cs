using System;
using System.Collections.Generic;
using System.Net;
using KryBot.Properties;
using RestSharp;

namespace KryBot
{
    internal class Generate
    {
        // Steam //
        public static CookieContainer Cookies_Steam(Classes.Bot bot)
        {
            var cookie = new CookieContainer();
            var target = new Uri("http://steamcommunity.com/");

            if (bot.SteamSessid != null)
                cookie.Add(new Cookie("sessionid", bot.SteamSessid) {Domain = target.Host});

            if (bot.SteamLogin != null)
                cookie.Add(new Cookie("steamLogin", bot.SteamLogin) {Domain = target.Host});

            if (bot.SteamRememberLogin != null)
                cookie.Add(new Cookie("steamRememberLogin", bot.SteamRememberLogin) {Domain = target.Host});

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
        public static CookieContainer Cookies_GameMiner(Classes.Bot bot)
        {
            var cookie = new CookieContainer();
            var target = new Uri("http://gameminer.net/");
            if (bot.GameMinerToken != null)
                cookie.Add(new Cookie("token", bot.GameMinerToken) {Domain = target.Host});

            if (bot.GameMinerxsrf != null)
                cookie.Add(new Cookie("_xsrf", bot.GameMinerxsrf) {Domain = target.Host});

            if (Settings.Default.Lang != null)
                cookie.Add(new Cookie("lang", Settings.Default.Lang) {Domain = target.Host});
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
        public static CookieContainer Cookies_SteamGifts(Classes.Bot bot)
        {
            var cookie = new CookieContainer();
            var target = new Uri("https://www.steamgifts.com/");

            if (bot.SteamGiftsPhpSessId != null)
                cookie.Add(new Cookie("PHPSESSID", bot.SteamGiftsPhpSessId) {Domain = target.Host});

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
        public static CookieContainer Cookies_SteamCompanion(Classes.Bot bot)
        {
            var cookie = new CookieContainer();
            var target = new Uri("https://steamcompanion.com/");

            if (bot.SteamCompanionPhpSessId != null)
                cookie.Add(new Cookie("PHPSESSID", bot.SteamCompanionPhpSessId) {Domain = target.Host});

            if (bot.SteamCompanionUserC != null)
                cookie.Add(new Cookie("userc", bot.SteamCompanionUserC) {Domain = target.Host});

            if (bot.SteamCompanionUserId != null)
                cookie.Add(new Cookie("userid", bot.SteamCompanionUserId) {Domain = target.Host});

            if (bot.SteamCompanionUserT != null)
                cookie.Add(new Cookie("usert", bot.SteamCompanionUserT) {Domain = target.Host});

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

        // SteamPortal //
        public static CookieContainer Cookies_SteamPortal(Classes.Bot bot)
        {
            var cookie = new CookieContainer();
            var target = new Uri("http://steamportal.net/");

            if (bot.SteamPortalPhpSessId != null)
                cookie.Add(new Cookie("PHPSESSID", bot.SteamPortalPhpSessId) {Domain = target.Host});

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
        public static CookieContainer Cookies_GameAways(Classes.Bot bot)
        {
            var cookie = new CookieContainer();
            var target = new Uri("http://www.gameaways.com/");

            if (bot.GameAwaysPhpSessId != null)
                cookie.Add(new Cookie("ASP.NET_SessionId", bot.GameAwaysPhpSessId) {Domain = target.Host});

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
        public static CookieContainer Cookies_SteamTrade(Classes.Bot bot)
        {
            var cookie = new CookieContainer();
            var target = new Uri("http://steamtrade.info/");

            if (bot.SteamTradePhpSessId != null)
                cookie.Add(new Cookie("PHPSESSID", bot.SteamTradePhpSessId) {Domain = target.Host});

            if (bot.SteamTradeDleUserId != null)
                cookie.Add(new Cookie("dle_user_id", bot.SteamTradeDleUserId) {Domain = target.Host});

            if (bot.SteamTradeDlePassword != null)
                cookie.Add(new Cookie("dle_password", bot.SteamTradeDlePassword) {Domain = target.Host});

            if (bot.SteamTradePassHash != null)
                cookie.Add(new Cookie("passhash", bot.SteamTradePassHash) {Domain = target.Host});

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
        public static CookieContainer Cookies_PlayBlink(Classes.Bot bot)
        {
            var cookie = new CookieContainer();
            var target = new Uri("http://playblink.com/");

            if (bot.PlayBlinkPhpSessId != null)
            {
                cookie.Add(new Cookie("PHPSESSID", bot.PlayBlinkPhpSessId) {Domain = target.Host});
            }

            cookie.Add(new Cookie("entry", "1") {Domain = target.Host});
            cookie.Add(new Cookie("level", bot.PlayBlinkLevel.ToString()) {Domain = target.Host});
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