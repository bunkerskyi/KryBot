using System;

using KryBot.Core.Helpers;

namespace KryBot.Core.Modifiers
{
	[AttributeUsage(AttributeTargets.Property)]
	public class CanBeUsedLikeModifierAttribute : Attribute
	{
		public CanBeUsedLikeModifierAttribute(Type resourceType,
											  string resourceName,
											  bool canBeUsedLikeModifier = true)
		{
			DisplayName = ResourceHelper.GetResourceLookup(resourceType, resourceName);
			CanBeUsedLikeModifier = canBeUsedLikeModifier;
		}

		public bool CanBeUsedLikeModifier { get; set; }

		public string DisplayName { get; set; }
	}
}
