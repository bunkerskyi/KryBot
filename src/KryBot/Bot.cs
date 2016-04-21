using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

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

        public void ClearGiveawayList()
        {
            GameMiner.Giveaways = new List<GameMiner.GmGiveaway>();
            SteamGifts.Giveaways = new List<SteamGifts.SgGiveaway>();
            SteamGifts.WishlistGiveaways = new List<SteamGifts.SgGiveaway>();
            SteamCompanion.Giveaways = new List<SteamCompanion.ScGiveaway>();
            SteamCompanion.WishlistGiveaways = new List<SteamCompanion.ScGiveaway>();
            SteamPortal.Giveaways = new List<SteamPortal.SpGiveaway>();
            SteamTrade.Giveaways = new List<SteamTrade.StGiveaway>();
            PlayBlink.Giveaways = new List<PlayBlink.PbGiveaway>(); 
        }

        public bool Save()
        {
            ClearGiveawayList();

            try
            {
                using (var fileStream = new FileStream("profile.xml", FileMode.Create, FileAccess.Write))
                {
                    var serializer = new XmlSerializer(typeof(Bot));
                    serializer.Serialize(fileStream, this);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Save(string path)
        {
            ClearGiveawayList();

            try
            {
                using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    var serializer = new XmlSerializer(typeof(Bot));
                    serializer.Serialize(fileStream, this);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
