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

		public GameMiner GameMiner { get; }
		public SteamGifts SteamGifts { get; }
		public SteamCompanion SteamCompanion { get; }
		public UseGamble UseGamble { get; }
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