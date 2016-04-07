namespace KryBot
{
    public class SteamPortal
    {
        public class SpGiveaway
        {
            public string Name { get; set; }
            public string StoreId { get; set; }
            public int Price { get; set; }
            public string Code { get; set; }
            public string Region { get; set; }
        }

        public class JsonJoin
        {
            public int error { get; set; }
            public TargetH target_h { get; set; }
        }

        public class TargetH
        {
            public int my_coins { get; set; }
        }
    }
}