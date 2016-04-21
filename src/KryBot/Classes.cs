using System.Collections.Generic;
using System.Net;
using System.Xml.Serialization;
using RestSharp;

namespace KryBot
{
    public class Classes
    {
        public class Response
        {
            public CookieContainer Cookies { get; set; }
            public IRestResponse RestResponse { get; set; }
        }

        public class Settings
        {
            public bool LogActive { get; set; }
            public string Lang { get; set; }
            public bool Sort { get; set; }
            public bool SortToMore { get; set; }
            public bool SortToLess { get; set; }
            public bool Timer { get; set; }
            public bool Autorun { get; set; }
            public bool ShowWonTip { get; set; }
            public bool ShowFarmTip { get; set; }
            public bool FullLog { get; set; }
            public bool WishlistSort { get; set; }
            public int TimerInterval { get; set; }
            public int TimerLoops { get; set; }
        }

        #region SteamXmlUserId64

        [XmlRoot(ElementName = "profile")]
        public class Profile
        {
            [XmlElement(ElementName = "steamID64")]
            public string SteamID64 { get; set; }
        }

        #endregion

        #region GitHub

        public class GitHubRelease
        {
            public string tag_name { get; set; }
            public bool prerelease { get; set; }
            public List<GitHunReleaseAssets> assets { get; set; }
            public string body { get; set; }
        }

        public class GitHunReleaseAssets
        {
            public string name { get; set; }
            public int size { get; set; }
            public string browser_download_url { get; set; }
        }

        #endregion

        #region SteamXMLUserGames

        [XmlRoot(ElementName = "game")]
        public class ProfileGame
        {
            [XmlElement(ElementName = "appID")]
            public string AppID { get; set; }

            [XmlElement(ElementName = "name")]
            public string Name { get; set; }
        }

        [XmlRoot(ElementName = "games")]
        public class ProfileGames
        {
            [XmlElement(ElementName = "game")]
            public List<ProfileGame> Game { get; set; }
        }

        [XmlRoot(ElementName = "gamesList")]
        public class ProfileGamesList
        {
            [XmlElement(ElementName = "games")]
            public ProfileGames Games { get; set; }
        }

        #endregion

        #region SteamJsonGameDetail

        public class GameDetail
        {
            public bool success { get; set; }
            public GameDetailData data { get; set; }
        }

        public class GameDetailData
        {
            public string name { get; set; }
        }

        #endregion
    }
}