/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
*/
using System;
using System.Drawing;
using System.Windows.Forms;
using KryBot.CommonResources.Localization;

namespace KryBot.Core
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
            return
                new Log(
                    $"{GetDateTime()} {{{site}}} {strings.GiveawayJoined_Join} \"{name}\" [{level}] {strings.GiveawayJoined_ConfirmedFor} {price} " +
                    $"{strings.GiveawayJoined_Points} ({points})", Color.Green);
        }

        public static Log GiveawayJoined(string site, string name, int price, int points)
        {
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
                Color.Orange, true);
        }

        public static Log GroupJoined(string url)
        {
            return new Log($"{GetDateTime()} {{Steam}} {strings.SteamGroupJoined} {{{url}}}", Color.Yellow, false);
        }

        public static Log GroupNotJoinde(string url)
        {
            return new Log($"{GetDateTime()} {{Steam}} {strings.SteamGroupNotJoined} {{{url}}}", Color.Yellow, false);
        }

        public static Log GroupAlreadyMember(string url)
        {
            return new Log($"{GetDateTime()} {{Steam}} {strings.SteamGroupAlreadyMember} {{{url}}}", Color.Yellow, false);
        }

        public static Log ParseProfile(string site, int points, int level, string username)
        {
            var str = $"{GetDateTime()} {{{site}}} {strings.LoginSuccess} {strings.Points}: {points}";

            if (level != -1)
            {
                str += $" {strings.Level}: {level}";
            }

            str += $" ({username})";

            return new Log(str, Color.Green, true);
        }

        public static Log ParseProfile(string site, int points, string username)
        {
            return
                new Log(
                    $"{GetDateTime()} {{{site}}} {strings.LoginSuccess} {strings.Points}: {points} ({username})",
                    Color.Green, true);
        }

        public static Log ParseProfile(string site, string username)
        {
            return new Log($"{GetDateTime()} {{{site}}} {strings.LoginSuccess} ({username})", Color.Green, true);
        }

        public static Log ParseProfileFailed(string site)
        {
            return new Log($"{GetDateTime()} {{{site}}} {strings.ParseProfile_LoginOrServerError}", Color.Red, false);
        }

        public static Log ParseProfileFailed(string site, string message)
        {
            return new Log($"{GetDateTime()} {{{site}}} {strings.AccountNotActive} {{{message}}}", Color.Red, false);
        }

        public static Log ParseGiveawaysEmpty(string content, string site)
        {
            return new Log($"{content}{GetDateTime()} {{{site}}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: 0",
                Color.White, true);
        }

        public static Log ParseGiveawaysEmpty(string site)
        {
            return new Log($"{GetDateTime()} {{{site}}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: 0",
                Color.White, true);
        }

        public static Log ParseGiveawaysFoundMatchGiveaways(string content, string site, string count)
        {
            return new Log(
                $"{content}{GetDateTime()} {{{site}}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: {count}",
                Color.White, true);
        }

        public static Log ParseGiveawaysFoundMatchGiveaways(string site, string count)
        {
            return new Log(
                $"{GetDateTime()} {{{site}}} {strings.ParseLoadGiveaways_FoundMatchGiveaways}: {count}",
                Color.White, true);
        }

        public static Log GamesInBlacklist(int count)
        {
            return new Log($"{GetDateTime()} {strings.GamesInBlacklist}: {count}", Color.White, true);
        }
    }
}