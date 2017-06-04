/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
*/
using System.Collections.Generic;
using System.Linq;
using KryBot.Core.Giveaways;

namespace KryBot.Core
{
    public class Blacklist
    {
        public Blacklist()
        {
            Items = new List<BlacklistItem>();
        }

        public List<BlacklistItem> Items { get; }

        /// <summary>
        ///     Remove all blacklisted games from list of <paramref name="giveaways" />.
        /// </summary>
        /// <param name="giveaways"> List of Giweways. </param>
        public void RemoveGames<T>(IList<T> giveaways) where T : BaseGiveaway
        {
            if (Items == null) return;
            for (var i = 0; i < giveaways.Count; i++)
            {
                if (Items.Any(item => giveaways[i].StoreId == item.Id))
                {
                    giveaways.Remove(giveaways[i]);
                    i--;
                }
                else if (Items.Any(item => giveaways[i].Name == item.Name))
                {
                    giveaways.Remove(giveaways[i]);
                    i--;
                }
            }
        }

        public class BlacklistItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}