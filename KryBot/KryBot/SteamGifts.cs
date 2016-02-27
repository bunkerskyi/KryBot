
namespace KryBot
{
    public class SteamGifts
    {
        public class SgGiveaway
        {
            public string Name { get; set; }
            public int Price { get; set; }
            public int Level { get; set; }
            public string Code { get; set; }
            public string Token { get; set; }
            public string Link { get; set; }
        }

        public class JsonResponseJoin
        {
            public string Type { get; set; }
            public string EntryCount { get; set; }
            public int Points { get; set; }
        }

        public class JsonResponseSyncAccount
        {
            public string type { get; set; }
            public string msg { get; set; }
        }
    }
}