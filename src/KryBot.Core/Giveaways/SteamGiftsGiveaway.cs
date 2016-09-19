using System.Diagnostics.CodeAnalysis;

using KryBot.CommonResources.Localization;
using KryBot.Core.Modifiers;

namespace KryBot.Core.Giveaways
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public class SteamGiftsGiveaway : BaseGiveaway
	{
		[CanBeUsedLikeModifier(typeof(strings), nameof(strings.Price))]
		public int Price { get; set; }

		[CanBeUsedLikeModifier(typeof(strings), nameof(strings.Level))]
		public int Level { get; set; }

		public string Code { get; set; }

		public string Token { get; set; }

		public string Link { get; set; }

		public string Region { get; set; }

		[CanBeUsedLikeModifier(typeof(strings), nameof(strings.Copies))]
		public int Copies { get; set; } = 1;
	}
}