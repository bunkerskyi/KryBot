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

		public GameMiner GameMiner { get; }
		public SteamGifts SteamGifts { get; }
		public SteamCompanion SteamCompanion { get; }
		public SteamPortal SteamPortal { get; }
		public SteamTrade SteamTrade { get; }
		public PlayBlink PlayBlink { get; }
		public Steam Steam { get; }
		public GameAways GameAways { get; }

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