namespace KryBot.CommonResources.Localization
{
	public static class List
	{
		private static readonly string[] LangList = {"English", "Русский"};
		private static readonly string[] ShortLangList = {"en-EN", "ru-RU"};

		public static string GetFullLang(string shortLang)
		{
			for (var i = 0; i < ShortLangList.Length; i++)
			{
				if (ShortLangList[i] == shortLang)
				{
					return LangList[i];
				}
			}
			return LangList[0];
		}

		public static string GetShortLang(string fullLang)
		{
			for (var i = 0; i < LangList.Length; i++)
			{
				if (LangList[i] == fullLang)
				{
					return ShortLangList[i];
				}
			}
			return ShortLangList[0];
		}

		public static string[] GetFullLangs()
		{
			return LangList;
		}
	}
}