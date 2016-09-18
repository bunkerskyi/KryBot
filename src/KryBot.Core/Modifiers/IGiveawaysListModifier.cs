using System.Collections.Generic;

using KryBot.Core.Giveaways;

namespace KryBot.Core.Modifiers
{
	public interface IGiveawaysListModifier<T> where T : BaseGiveaway
	{
		ModifierProperty ModifierProperty { get; set; }

		IEnumerable<T> ApplyModifier(IEnumerable<T> giveaways);
	}
}
