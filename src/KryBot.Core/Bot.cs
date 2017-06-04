﻿using System.Collections.Generic;
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
            SteamGifts = new SteamGifts();
            SteamCompanion = new SteamCompanion();
            SteamPortal = new SteamPortal();
            SteamTrade = new SteamTrade();
            PlayBlink = new PlayBlink();
            Steam = new Steam();
            GameAways = new GameAways();
            InventoryGifts = new InventoryGifts();
            Blacklist = new Blacklist();
        }

        public bool Sort { get; set; }
        public bool SortToMore { get; set; }
        public bool SortToLess { get; set; }
        public bool Timer { get; set; }
        public bool WishlistSort { get; set; }
        public int TimerInterval { get; set; }
        public int TimerLoops { get; set; }

        public SteamGifts SteamGifts { get; set; }
        public SteamCompanion SteamCompanion { get; set; }
        public SteamPortal SteamPortal { get; set; }
        public SteamTrade SteamTrade { get; set; }
        public PlayBlink PlayBlink { get; set; }
        public Steam Steam { get; set; }
        public GameAways GameAways { get; set; }
        public InventoryGifts InventoryGifts { get; set; }
        public Blacklist Blacklist { get; set; }

        public void ClearGiveawayList()
        {
            SteamGifts.Giveaways = new List<SteamGiftsGiveaway>();
            SteamGifts.WishlistGiveaways = new List<SteamGiftsGiveaway>();
            SteamCompanion.Giveaways = new List<SteamCompanionGiveaway>();
            SteamCompanion.WishlistGiveaways = new List<SteamCompanionGiveaway>();
            SteamPortal.Giveaways = new List<SteamPortalGiveaway>();
            SteamTrade.Giveaways = new List<SteamTradeGiveaway>();
            PlayBlink.Giveaways = new List<PlayBlinkGiveaway>();
            GameAways.Giveaways = new List<GameAwaysGiveaway>();
            InventoryGifts.Giveaways = new List<InventoryGiftsGiveaway>();
        }

        public bool Save(string path = FilePaths.Profile)
        {
            ClearGiveawayList();

            return FileHelper.Save(this, path);
        }
    }
}