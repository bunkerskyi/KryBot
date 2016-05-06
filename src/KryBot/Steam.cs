namespace KryBot
{
    public class Steam
    {
        public Steam()
        {
            Cookies = new SteamCookies();
        }

        public bool Enabled { get; set; }
        public string ProfileLink { get; set; }
        public SteamCookies Cookies { get; set; }

        public void Logout()
        {
            Cookies = new SteamCookies();
            Enabled = false;
        }

        public class SteamCookies
        {
            public string Sessid { get; set; }
            public string Login { get; set; }
            public string RememberLogin { get; set; }
        }
    }
}