using System.Diagnostics.CodeAnalysis;

namespace KryBot.Core.Giveaways
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class SteamCompanionGiveaway : BaseGiveaway
    {
        public int Price { get; set; }

        public string Code { get; set; }

        public string Link { get; set; }

        public bool Region { get; set; }
    }
}