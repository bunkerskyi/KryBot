using System.Collections.Generic;

namespace KryBot
{
    public class PlayBlink
    {
        public PlayBlink()
        {
            Cookies = new PbCookies();
            Giveaways = new List<PbGiveaway>();
        }

        public bool Enabled { get; set; }
        public int Points { get; set; }
        public int Level { get; set; }
        public int MaxJoinValue { get; set; } = 50;
        public int PointReserv { get; set; }
        public PbCookies Cookies { get; set; }
        public List<PbGiveaway> Giveaways { get; set; }

        public void Logout()
        {
            Cookies = new PbCookies();
            Enabled = false;
        }

        public class PbCookies
        {
            public string PhpSessId { get; set; }
        }

        public class PbGiveaway
        {
            public string Name { get; set; }
            public string StoreId { get; set; }
            public int Price { get; set; }
            public int Level { get; set; }
            public string Id { get; set; }
        }
    }
}