using System.Diagnostics.CodeAnalysis;

namespace KryBot.Core.Giveaways
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class SteamGiftsGiveaway : BaseGiveaway
    {
        public int Price { get; set; }

        public int Level { get; set; }

        public string Code { get; set; }

        public string Token { get; set; }

        public string Link { get; set; }

        public string Region { get; set; }

        public int Copies { get; set; } = 1;
    }
}