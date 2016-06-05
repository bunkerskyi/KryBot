using KryBot.Functional.Cookies;

namespace KryBot.Functional.Sites
{
	public class Steam
	{
		public Steam()
		{
			Cookies = new SteamCookie();
		}

		public bool Enabled { get; set; }
		public string ProfileLink { get; set; }
		public SteamCookie Cookies { get; set; }

		public void Logout()
		{
			Cookies = new SteamCookie();
			Enabled = false;
		}
	}
}