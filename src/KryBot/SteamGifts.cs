using System.Collections.Generic;

namespace KryBot
{
    public class SteamGifts
    {
        public SteamGifts()
        {
            Cookies = new SgCookies();
            Giveaways = new List<SgGiveaway>();
            WishlistGiveaways = new List<SgGiveaway>();
            RegionGiveaways = new List<SgGiveaway>();
            MinNumberCopiesGiveaways = new List<SgGiveaway>();
        }

        public bool Enabled { get; set; }
        public bool Group { get; set; }
        public bool Region { get; set; }
        public bool MinNumberCopies { get; set; }
        public bool Regular { get; set; } = true;
        public bool WishList { get; set; }
        public bool SortLevel { get; set; }
        public bool SortToLessLevel { get; set; }
        public int Points { get; set; }
        public int Level { get; set; }
        public int JoinPointLimit { get; set; } = 300;
        public int PointsReserv { get; set; }
        public int NumberCopies { get; set; }
        public int MinLevel { get; set; }
        public SgCookies Cookies { get; set; }
        public List<SgGiveaway> Giveaways { get; set; }
        public List<SgGiveaway> RegionGiveaways { get; set; }
        public List<SgGiveaway> MinNumberCopiesGiveaways { get; set; }
        public List<SgGiveaway> WishlistGiveaways { get; set; }

        public void Logout()
        {
            Cookies = new SgCookies();
            Enabled = false;
        }

        public class SgCookies
        {
            public string PhpSessId { get; set; }
        }

        public class SgGiveaway
        {
            public string Name { get; set; }
            public string StoreId { get; set; }
            public int Price { get; set; }
            public int Level { get; set; }
            public string Code { get; set; }
            public string Token { get; set; }
            public string Link { get; set; }
            public string Region { get; set; }
            public string MinNumberCopies { get; set; }
        }

        public class JsonResponseJoin
        {
            public string Type { get; set; }
            public string EntryCount { get; set; }
            public int Points { get; set; }
        }

        public class JsonResponseSyncAccount
        {
            public string Type { get; set; }
            public string Msg { get; set; }
        }
    }
}