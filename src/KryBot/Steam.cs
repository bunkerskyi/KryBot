using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RestSharp;

namespace KryBot
{
    public class Steam
    {
        public Steam()
        {
            Cookies = new SteamCookies();
        }

        public bool Enabled { get; set; }
        public string ProfileLink { get; private set; }
        public SteamCookies Cookies { get; private set; }

        public void Logout()
        {
            Cookies = new SteamCookies();
            Enabled = false;
        }

        public class SteamCookies
        {
            public string Sessid { get; set; }
            public string Login { get; set; }
            public string RememberLogin { get; set; }
        }

        #region Parse

        public async Task<Log> GetProfile()
        {
            var response = await Web.GetAsync("http://steamcommunity.com/", "", new List<Parameter>(), GenerateCookies(), new List<HttpHeader>());

            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var login = htmlDoc.DocumentNode.SelectSingleNode(
                        "//a[contains(@class, 'user_avatar') and contains(@class, 'playerAvatar')]");
                if (login == null)
                {
                    return Messages.ParseProfileFailed("Steam");
                }

                ProfileLink = login.Attributes["href"].Value;
                return Messages.ParseProfile("Steam", login.Attributes["href"].Value.Split('/')[4]);
            }
            return Messages.ParseProfileFailed("Steam");
        }

        public async Task<Classes.ProfileGamesList> GetUserGames()
        {
            var responseXmlProfile = await Web.GetAsync($"{ProfileLink}games?tab=all&xml=1");
            var serializer = new XmlSerializer(typeof(Classes.ProfileGamesList));
            TextReader reader = new StringReader(responseXmlProfile.RestResponse.Content);
            var games = (Classes.ProfileGamesList)serializer.Deserialize(reader);
            return games;
        }

        public string GetGameName(string appId)
        {
            var responseJsonDetail = Web.Get($"http://store.steampowered.com/api/appdetails?appids={appId}", "",
                        new List<Parameter>(),
                        new CookieContainer(), new List<HttpHeader>(), "");

            if (responseJsonDetail.RestResponse.Content == "null" || responseJsonDetail.RestResponse.Content == "")
            {
                return null;
            }

            var json = responseJsonDetail.RestResponse.Content.Replace($"{{\"{appId}\":", "");
            var gameDetail = JsonConvert.DeserializeObject<Classes.GameDetail>(json.Substring(0, json.Length - 1));
            return gameDetail.success ? gameDetail.data.name : null;
        }

        #endregion

        #region Generate

        public CookieContainer GenerateCookies()
        {
            var cookie = new CookieContainer();
            var target = new Uri("http://steamcommunity.com/");

            if (Cookies.Sessid != null)
            {
                cookie.Add(new Cookie("sessionid", Cookies.Sessid) {Domain = target.Host});
            }

            if (Cookies.Login != null)
            {
                cookie.Add(new Cookie("steamLogin", Cookies.Login) {Domain = target.Host});
            }

            if (Cookies.RememberLogin != null)
            {
                cookie.Add(new Cookie("steamRememberLogin", Cookies.RememberLogin) {Domain = target.Host});
            }

            return cookie;
        }

        #endregion
    }
}