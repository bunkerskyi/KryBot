namespace KryBot
{
    public class GameAways
    {
        public class SaGiveaway
        {
            public string Name { get; set; }
            public int Price { get; set; }
            public string Link { get; set; }
            public string Id { get; set; }
        }

        public class JsonResponse
        {
            public string LeaveButtonLabel { get; set; }
            public string EntriesCount { get; set; }
            public string UserEntries { get; set; }
            public int Balance { get; set; }
            public int EntriesRemaining { get; set; }
            public int Result { get; set; }
        }
    }
}