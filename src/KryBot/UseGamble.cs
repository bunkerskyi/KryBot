using System.Collections.Generic;

namespace KryBot
{
    public class UseGamble
    {
        public UseGamble()
        {
            Cookies = new UgCookies();
            Giveaways = new List<UgGiveaway>();
        }

        public bool Enabled { get; set; }
        public string ProfileLink { get; set; }
        public int Points { get; set; }
        public int MaxJoinValue { get; set; } = 30;
        public int PointsReserv { get; set; }
        public UgCookies Cookies { get; set; }
        public List<UgGiveaway> Giveaways { get; set; }

        public void Logout()
        {
            Cookies = new UgCookies();
            Enabled = false;
        }

        public class UgCookies
        {
            public string PhpSessId { get; set; }
        }

        public class UgGiveaway
        {
            public string Name { get; set; }
            public string StoreId { get; set; }
            public int Price { get; set; }
            public string Code { get; set; }
            public string Region { get; set; }
        }

        public class JsonJoin
        {
            public int Error { get; set; }
            public TargetH target_h { get; set; }
        }

        public class TargetH
        {
            public int my_coins { get; set; }
        }
    }
}