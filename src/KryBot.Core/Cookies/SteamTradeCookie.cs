namespace KryBot.Core.Cookies
{
	public class SteamTradeCookie : BaseCookie
	{
		public string DleUserId { get; set; }

		public string DlePassword { get; set; }

		public string PassHash { get; set; }
	}
}