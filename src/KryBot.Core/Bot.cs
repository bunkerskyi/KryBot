using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using KryBot.Core.Giveaways;
using KryBot.Core.Helpers;
using KryBot.Core.Sites;

namespace KryBot.Core
{
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
	[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
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

		// НЕ ИЗМЕНЯТЬ У ПОЛЕЙ SET
		public GameMiner GameMiner { get; set; }
		public SteamGifts SteamGifts { get; set; }
		public SteamCompanion SteamCompanion { get; set; }
		public UseGamble UseGamble { get; set; }
		public SteamTrade SteamTrade { get; set; }
		public PlayBlink PlayBlink { get; set; }
		public Steam Steam { get; set; }
		public GameAways GameAways { get; set; }
		// НЕ ИЗМЕНЯТЬ У ПОЛЕЙ SET

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

		public bool Save(string path = FilePaths.Profile)
		{
			ClearGiveawayList();

			return FileHelper.Save(this, path);
		}
	}
}