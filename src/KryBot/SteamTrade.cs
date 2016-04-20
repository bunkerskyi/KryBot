namespace KryBot
{
    public class SteamTrade
    {
        public SteamTrade()
        {
            Cookies = new StCookies();    
        }

        public bool Enabled { get; set; }
        public StCookies Cookies { get; set; }

        public class StCookies
        {
            public string PhpSessId { get; set; }
            public string DleUserId { get; set; }
            public string DlePassword { get; set; }
            public string PassHash { get; set; }
        }

        public class StGiveaway
        {
            public string Name { get; set; }
            public string StoreId { get; set; }
            public string LinkJoin { get; set; }
            public string Link { get; set; }
        }
    }
}