using System.ComponentModel;

namespace KryBot.Core.Modifiers
{
	public enum FilterOperation
	{
		[Description(">")]
		GreaterThan,
		[Description("<")]
		LessThan,
		[Description("=")]
		Equal,
		[Description("<=")]
		LessThanOrEqual,
		[Description(">=")]
		GreaterThanOrEqual
	}
}
