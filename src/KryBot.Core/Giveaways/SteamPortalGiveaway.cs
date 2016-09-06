using System.Diagnostics.CodeAnalysis;

namespace KryBot.Core.Giveaways
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class SteamPortalGiveaway : BaseGiveaway
    {
        public int Price { get; set; }

        public string Code { get; set; }

        public string Region { get; set; }
    }
}