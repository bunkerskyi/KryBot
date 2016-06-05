using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using KryBot.Functional.Giveaways;
using KryBot.Functional.Sites;

namespace KryBot.Functional
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
			GameMiner.Giveaways = new List<GameMinerGiveaway>();
			SteamGifts.Giveaways = new List<SteamGiftsGiveaway>();
			SteamGifts.WishlistGiveaways = new List<SteamGiftsGiveaway>();
			SteamCompanion.Giveaways = new List<SteamCompanionGiveaway>();
			SteamCompanion.WishlistGiveaways = new List<SteamCompanionGiveaway>();
			SteamPortal.Giveaways = new List<SteamPortalGiveaway>();
			SteamTrade.Giveaways = new List<SteamTradeGiveaway>();
			PlayBlink.Giveaways = new List<PlayBlinkGiveaway>();
		}

		public bool Save()
		{
			ClearGiveawayList();

			try
			{
				using (var fileStream = new FileStream("profile.xml", FileMode.Create, FileAccess.Write))
				{
					var serializer = new XmlSerializer(typeof (Bot));
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
					var serializer = new XmlSerializer(typeof (Bot));
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