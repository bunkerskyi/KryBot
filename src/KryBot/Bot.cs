using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using KryBot.Sites;

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
            GameAways = new GameAways();
        }

        public bool Enabled { get; set; }

        public GameMiner GameMiner { get; set; }
        public SteamGifts SteamGifts { get; set; }
        public SteamCompanion SteamCompanion { get; set; }
        public SteamPortal SteamPortal { get; set; }
        public SteamTrade SteamTrade { get; set; }
        public PlayBlink PlayBlink { get; set; }
        public Steam Steam { get; set; }
        public GameAways GameAways { get; set; }

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
            catch (Exception ex)
            {
                MessageBox.Show(@"Ошибка", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            catch (Exception ex)
            {
                MessageBox.Show(@"Ошибка", ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}