namespace KryBot
{
    public class Bot
    {
        public Bot()
        {
            GameMiner = new GameMiner();
            SteamGifts = new SteamGifts();
            SteamCompanion = new SteamCompanion();
            SteamPortal = new SteamPortal();
            SteamTrade = new SteamTrade();
            PlayBlink = new PlayBlink();
            Steam = new Steam();
        }

        public bool Enabled { get; set; }
        public string UserAgent { get; set; }

        public GameMiner GameMiner { get; set; }
        public SteamGifts SteamGifts { get; set; }
        public SteamCompanion SteamCompanion { get; set; }
        public SteamPortal SteamPortal { get; set; }
        public SteamTrade SteamTrade { get; set; }
        public PlayBlink PlayBlink { get; set; }
        public Steam Steam { get; set; }
    }
}
