using System.Collections.Generic;

namespace KryBot
{
    public class GameMiner
    {
        public GameMiner()
        {
            Cookies = new GmCookies();
            Giveaways = new List<GmGiveaway>();
        }

        public bool Enabled { get; set; }
        public int Coal { get; set; }
        public int Level { get; set; }
        public int JoinCoalLimit { get; set; } = 50;
        public int CoalReserv { get; set; }
        public bool Sandbox { get; set; } = true;
        public bool Regular { get; set; } = true;
        public bool FreeGolden { get; set; } = true;
        public bool OnlyGifts { get; set; }
        public bool NoRegion { get; set; }
        public GmCookies Cookies { get; set; }
        public List<GmGiveaway> Giveaways { get; set; }

        public void Logout()
        {
            Cookies = new GmCookies();
            Enabled = false;
        }

        public class GmCookies
        {
            public string Token { get; set; }
            public string Xsrf { get; set; }
        }

        public class Properties
        {
            public string hat { get; set; }
            public string nick_color { get; set; }
        }

        public class Account
        {
            public string name { get; set; }
            public int level { get; set; }
            public int id { get; set; }
            public string avatar { get; set; }
            public bool active { get; set; }
            public string role_id { get; set; }
            public Properties properties { get; set; }
        }

        public class Game
        {
            public int updated { get; set; }
            public string name { get; set; }
            public string img { get; set; }
            public string url { get; set; }
            public string image { get; set; }
            public object steam_app { get; set; }
            public string game_app_type_id { get; set; }
            public string game_class_id { get; set; }
            public string foreign_id { get; set; }
            public int id { get; set; }
        }

        public class Award
        {
            public int res { get; set; }
            public int exp { get; set; }
        }

        public class Giveaway
        {
            public string state { get; set; }
            public bool golden { get; set; }
            public Account account { get; set; }
            public string code { get; set; }
            public object gift_as_key { get; set; }
            public int created { get; set; }
            public bool is_member { get; set; }
            public int price { get; set; }
            public object winner { get; set; }
            public object sandbox { get; set; }
            public int? key_stat { get; set; }
            public bool constraint_check { get; set; }
            public Game game { get; set; }
            public int finish { get; set; }
            public Award award { get; set; }
            public string regionlock_type_id { get; set; }
            public List<object> regionlocks { get; set; }
            public int entries { get; set; }
            public string giveaway_type_id { get; set; }
            public object entered { get; set; }
            public List<object> constraints { get; set; }
        }

        public class JsonRootObject
        {
            public int count { get; set; }
            public int server_time { get; set; }
            public List<Giveaway> giveaways { get; set; }
            public int last_page { get; set; }
            public int total { get; set; }
            public int page { get; set; }
        }

        public class GmGiveaway
        {
            public string Name { get; set; }
            public string StoreId { get; set; }
            public int Price { get; set; }
            public int Page { get; set; }
            public string Region { get; set; }
            public string Id { get; set; }
            public bool IsSandbox { get; set; }
            public bool IsRegular { get; set; }
            public bool IsGolden { get; set; }
        }

        public class JsonResponse
        {
            public string Status { get; set; }
            public int Coal { get; set; }
            public int Gold { get; set; }
        }

        public class JsonResponseErrorDetail
        {
            public string Message { get; set; }
            public string Code { get; set; }
        }

        public class JsonResponseError
        {
            public JsonResponseErrorDetail Error { get; set; }
        }
    }
}