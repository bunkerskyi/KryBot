using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

using KryBot.Core.Giveaways;

namespace KryBot.Core.Modifiers
{
	public class GiveawayFiltering<T> : IGiveawaysListModifier<T> where T : BaseGiveaway
	{
		public object Value { get; set; }

		public ModifierProperty ModifierProperty { get; set; }

		public FilterOperation FilterOperation { get; set; }

		public IEnumerable<T> ApplyModifier(IEnumerable<T> giveaways)
		{
			var param = Expression.Parameter(typeof(T), "x");
			var propertyExpression = Expression.Property(param, ModifierProperty.PropertyName);
			Expression value = Expression.Constant(Value, Value.GetType());
			value = Expression.Convert(value, propertyExpression.Type);

			BinaryExpression binaryOperation;
			switch (FilterOperation)
			{
				case FilterOperation.Equal:
					binaryOperation = Expression.Equal(propertyExpression, value);
					break;
				case FilterOperation.LessThan:
					binaryOperation = Expression.LessThan(propertyExpression, value);
					break;
				case FilterOperation.GreaterThan:
					binaryOperation = Expression.GreaterThan(propertyExpression, value);
					break;
				case FilterOperation.LessThanOrEqual:
					binaryOperation = Expression.LessThanOrEqual(propertyExpression, value);
					break;
				case FilterOperation.GreaterThanOrEqual:
					binaryOperation = Expression.GreaterThanOrEqual(propertyExpression, value);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var lambda = Expression.Lambda<Func<T, bool>>(binaryOperation, param);
			var keySelector = lambda.Compile();
			return giveaways.Where(keySelector);
		}
	}
}
