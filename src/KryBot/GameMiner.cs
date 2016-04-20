using System.Collections.Generic;

namespace KryBot
{
    public class GameMiner
    {
        public GameMiner()
        {
            Cookies = new GmCookies();    
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

        public class GmCookies
        {
            public string Token { get; set; }
            public string Xsrf { get; set; }
        }

        public class JsonProperties
        {
            public string RankColor { get; set; }
            public string Hat { get; set; }
            public string Rank { get; set; }
            public string NickColor { get; set; }
        }

        public class JsonAccount
        {
            public string Name { get; set; }
            public int Level { get; set; }
            public int Id { get; set; }
            public string Avatar { get; set; }
            public bool Active { get; set; }
            public string RoleId { get; set; }
            public JsonProperties Properties { get; set; }
        }

        public class JsonGame
        {
            public int Updated { get; set; }
            public string Name { get; set; }
            public string Img { get; set; }
            public string Url { get; set; }
            public string Image { get; set; }
            public object SteamApp { get; set; }
            public string GameAppTypeId { get; set; }
            public string GameClassId { get; set; }
            public string ForeignId { get; set; }
            public int Id { get; set; }
        }

        public class JsonAward
        {
            public int Res { get; set; }
            public int Exp { get; set; }
        }

        public class JsonGiveaway
        {
            public string State { get; set; }
            public bool Golden { get; set; }
            public JsonAccount Account { get; set; }
            public string Code { get; set; }
            public object GiftAsKey { get; set; }
            public int Created { get; set; }
            public bool IsMember { get; set; }
            public int Price { get; set; }
            public object Winner { get; set; }
            public object Sandbox { get; set; }
            public int KeyStat { get; set; }
            public bool ConstraintCheck { get; set; }
            public JsonGame Game { get; set; }
            public int Finish { get; set; }
            public JsonAward Award { get; set; }
            public string RegionlockTypeId { get; set; }
            public List<object> Regionlocks { get; set; }
            public int Entries { get; set; }
            public string GiveawayTypeId { get; set; }
            public object Entered { get; set; }
            public List<object> Constraints { get; set; }
        }

        public class JsonRootObject
        {
            public int Count { get; set; }
            public int ServerTime { get; set; }
            public List<JsonGiveaway> Giveaways { get; set; }
            public int LastPage { get; set; }
            public int Total { get; set; }
            public int Page { get; set; }
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