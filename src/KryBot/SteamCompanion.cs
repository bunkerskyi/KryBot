using System.Collections.Generic;

namespace KryBot
{
    public class SteamCompanion
    {
        public SteamCompanion()
        {
            Cookies = new ScCookies();
            Giveaways = new List<ScGiveaway>();
            WishlistGiveaways = new List<ScGiveaway>();
        }

        public bool Enabled { get; set; }
        public bool Regular { get; set; } = true;
        public bool WishList { get; set; }
        public bool Contributors { get; set; }
        public bool Group { get; set; }
        public bool AutoJoin { get; set; }
        public string ProfileLink { get; set; }
        public int Points { get; set; }
        public int JoinPointLimit { get; set; } = 1500;
        public int PointsReserv { get; set; }
        public ScCookies Cookies { get; set; }
        public List<ScGiveaway> Giveaways { get; set; }
        public List<ScGiveaway> WishlistGiveaways { get; set; }

        public void Logout()
        {
            Cookies = new ScCookies();
            Enabled = false;
        }

        public class ScCookies
        {
            public string PhpSessId { get; set; }
            public string UserId { get; set; }
            public string UserC { get; set; }
            public string UserT { get; set; }
        }

        public class ScGiveaway
        {
            public string Name { get; set; }
            public string StoreId { get; set; }
            public int Price { get; set; }
            public string Code { get; set; }
            public string Link { get; set; }
            public bool Region { get; set; }
        }
    }
}