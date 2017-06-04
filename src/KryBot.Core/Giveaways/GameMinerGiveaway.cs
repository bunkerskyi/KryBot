/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
*/
using System.Diagnostics.CodeAnalysis;

namespace KryBot.Core.Giveaways
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class GameMinerGiveaway : BaseGiveaway
    {
        public int Price { get; set; }

        public int Page { get; set; }

        public string Region { get; set; }

        public string Id { get; set; }

        public bool IsSandbox { get; set; }

        public bool IsRegular { get; set; }

        public bool IsGolden { get; set; }
    }
}