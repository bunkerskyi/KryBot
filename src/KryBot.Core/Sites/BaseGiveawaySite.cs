using System.Collections.Generic;
using System.Reflection;

using KryBot.Core.Giveaways;
using KryBot.Core.Modifiers;

namespace KryBot.Core.Sites
{
	public abstract class BaseGiveawaySite<T> where T : BaseGiveaway
	{
		public List<ModifierProperty> ModifierTargets { get; set; }

		protected void FillModifierTargets()
		{
			foreach (var propertyInfo in typeof(T).GetProperties())
			{
				var attr = propertyInfo.GetCustomAttribute(typeof(CanBeUsedLikeModifierAttribute)) as CanBeUsedLikeModifierAttribute;
				if (attr != null)
				{
					var modifierProperty = new ModifierProperty
					{
						DisplayName = attr.DisplayName,
						PropertyName = propertyInfo.Name
					};
					ModifierTargets.Add(modifierProperty);
				}
			}
		}

		public BaseGiveawaySite()
		{
			ModifierTargets = new List<ModifierProperty>();
			FillModifierTargets();
		}
	}
}
