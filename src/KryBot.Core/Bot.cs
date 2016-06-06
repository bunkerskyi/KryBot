using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using KryBot.Core.Giveaways;
using KryBot.Core.Sites;

namespace KryBot.Core
{
	public class Bot
	{
		public Bot()
		{
			GameMiner = new GameMiner();
			SteamGifts = new SteamGifts();
			SteamCompanion = new SteamCompanion();
			UseGamble = new UseGamble();
			SteamTrade = new SteamTrade();
			PlayBlink = new PlayBlink();
			Steam = new Steam();
			GameAways = new GameAways();
		}

		public string UserAgent { get; set; }

		// НЕ ЗАБИРАТЬ У ПОЛЕЙ SET
		public GameMiner GameMiner { get; set; }
		public SteamGifts SteamGifts { get; set; }
		public SteamCompanion SteamCompanion { get; set; }
		public UseGamble UseGamble { get; set; }
		public SteamTrade SteamTrade { get; set; }
		public PlayBlink PlayBlink { get; set; }
		public Steam Steam { get; set; }
		public GameAways GameAways { get; set; }
		// НЕ ЗАБИРАТЬ У ПОЛЕЙ SET

		public void ClearGiveawayList()
		{
			GameMiner.Giveaways = new List<GameMinerGiveaway>();
			SteamGifts.Giveaways = new List<SteamGiftsGiveaway>();
			SteamGifts.WishlistGiveaways = new List<SteamGiftsGiveaway>();
			SteamCompanion.Giveaways = new List<SteamCompanionGiveaway>();
			SteamCompanion.WishlistGiveaways = new List<SteamCompanionGiveaway>();
			UseGamble.Giveaways = new List<UseGambleGiveaway>();
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
				MessageBox.Show(ex.Message, @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				MessageBox.Show(ex.Message, @"Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
		}
	}
}