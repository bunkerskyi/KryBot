using System;
using System.Drawing;
using System.Windows.Forms;
using KryBot.lang;

namespace KryBot
{
    public static class Messages
    {
        public static Log Start()
        {
            return new Log($"{Application.ProductName} [{Application.ProductVersion}]");
        }

        public static Log FileLoadFailed(string fileName)
        {
            return
                new Log(
                    $"{GetDateTime()} {strings.GetBotInConfig_Config} \"{fileName}\" {strings.GetBotInConfig_NotLoaded}",
                    Color.Red);
        }

        public static string GetDateTime()
        {
            return $"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}]";
        }

        public static Log ProfileLoaded()
        {
            return new Log($"{GetDateTime()} {strings.LoadProfile_Load}");
        }

        public static Log DoFarm_Start()
        {
            return new Log($"{GetDateTime()} {strings.Start}");
        }

        public static Log DoFarm_Finish(string time)
        {
            return new Log($"{GetDateTime()} {strings.Finish} за {time}");
        }

        public static Log GiveawayJoined(string site, string name, int price, int points, int level)
        {
            Properties.Settings.Default.JoinsPerSession += 1;
            Properties.Settings.Default.JoinsTotal += 1;
            Properties.Settings.Default.Save();

            if (level > 0)
            {
                return
                    new Log(
                        $"{GetDateTime()} {{{site}}} {strings.GiveawayJoined_Join} \"{name}\" [{level}] {strings.GiveawayJoined_ConfirmedFor} {price} " +
                        $"{strings.GiveawayJoined_Points} ({points})", Color.Green);
            }
            return
                new Log(
                    $"{GetDateTime()} {{{site}}} {strings.GiveawayJoined_Join} \"{name}\" {strings.GiveawayJoined_ConfirmedFor} {price} " +
                    $"{strings.GiveawayJoined_Points} ({points})", Color.Green);
        }

        public static Log GiveawayNotJoined(string site, string name, string message)
        {
            return
                new Log(
                    $"{GetDateTime()} {{{site}}} {strings.GiveawayJoined_Join} \"{name}\" {strings.GiveawayNotJoined_NotConfirmed} {{{message}}}",
                    Color.Red);
        }

        public static Log GiveawayHaveWon(string site, int count, string url)
        {
            return new Log($"{GetDateTime()} {{{site}}} {strings.GiveawaysHaveWon} ({count}) {{{url}}}",
                Color.Orange, true, true);
        }

        public static Log GroupJoined(string url)
        {
            return new Log($"{GetDateTime()} {{Steam}} {strings.SteamGroupJoined} {{{url}}}", Color.Yellow, false,
                true);
        }

        public static Log GroupNotJoinde(string url)
        {
            return new Log($"{GetDateTime()} {{Steam}} {strings.SteamGroupNotJoined} {{{url}}}", Color.Yellow,
                false, true);
        }

        public static Log GroupAlreadyMember(string url)
        {
            return new Log($"{GetDateTime()} {{Steam}} {strings.SteamGroupAlreadyMember} {{{url}}}", Color.Yellow,
                false, true);
        }

        public static Log ParseProfile(string site, int points, int level, string username)
        {
            var str = $"{GetDateTime()} {{{site}}} {strings.LoginSuccess} {strings.ParseProfile_Points}: {points}";

            if (level != -1)
            {
                str += $" {strings.ParseProfile_Level}: {level}";
            }

            str += $" ({username})";

            return new Log(str, Color.Green, true, true);
        }

        public static Log ParseProfile(string site, int points, string username)
        {
            return
                new Log(
                    $"{GetDateTime()} {{{site}}} {strings.LoginSuccess} {strings.ParseProfile_Points}: {points} ({username})",
                    Color.Green, true, true);
        }

        public static Log ParseProfile(string site, string username)
        {
            return new Log($"{GetDateTime()} {{{site}}} {strings.LoginSuccess} ({username})", Color.Green, true,
                true);
        }

        public static Log ParseProfileFailed(string site)
        {
            return new Log($"{GetDateTime()} {{{site}}} {strings.ParseProfile_LoginOrServerError}", Color.Red,
                false, true);
        }
    }
}