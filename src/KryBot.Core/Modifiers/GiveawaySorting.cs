using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

using KryBot.Core.Giveaways;

namespace KryBot.Core.Modifiers
{
	public class GiveawaySorting<T> : IGiveawaysListModifier<T> where T : BaseGiveaway
	{
		public GiveawaySorting(ListSortDirection sortDirection = ListSortDirection.Descending)
		{
			SortDirection = sortDirection;
		}

		public ModifierProperty ModifierProperty { get; set; }

		public IEnumerable<T> ApplyModifier(IEnumerable<T> giveaways)
		{
			var param = Expression.Parameter(typeof(T), "x");
			var propertyExpression = Expression.Property(param, ModifierProperty.PropertyName);
			var property = Expression.Convert(propertyExpression, typeof(object));
			var lambda = Expression.Lambda<Func<T, object>>(property, param);
			var keySelector = lambda.Compile();

			var orderedEnumerable = giveaways as IOrderedEnumerable<T>;
			if (orderedEnumerable != null)
			{
				if (SortDirection == ListSortDirection.Ascending)
				{
					return orderedEnumerable.ThenBy(keySelector);
				}
				return orderedEnumerable.ThenByDescending(keySelector);
			}

			if (SortDirection == ListSortDirection.Ascending)
			{
				return giveaways.OrderBy(keySelector);
			}
			return giveaways.OrderByDescending(keySelector);
		}

		public ListSortDirection SortDirection { get; set; }
	}
}
