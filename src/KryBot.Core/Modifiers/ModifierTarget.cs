namespace KryBot.Core.Modifiers
{
	public struct ModifierProperty
	{
		public ModifierProperty(string displayName, string propertyName)
		{
			DisplayName = displayName;
			PropertyName = propertyName;
		}

		public string DisplayName { get; set; }

		public string PropertyName { get; set; }
	}
}
