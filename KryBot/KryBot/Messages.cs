using System;
using System.Drawing;
using System.Windows.Forms;
using KryBot.lang;
using static KryBot.Classes;
using static KryBot.Tools;

namespace KryBot
{
    public class Messages
    {
        public static Log Start()
        {
            var log = new Log
            {
                Content = $"{Application.ProductName} [{Application.ProductVersion}] \n",
                Color = Color.White
            };
            return log;
        }

        public static Log FileLoadFailed(string fileName)
        {
            var log = new Log
            {
                Content = $"{GetDateTime()}{strings.GetBotInConfig_Config} '{fileName}' {strings.GetBotInConfig_NotLoaded}",
                Color = Color.Red
            };
            return log;
        }

        public static string GetDateTime()
        {
            return $"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}] ";
        }

        public static Log ProfileLoaded()
        {
            var log = new Log
            {
                Content = GetDateTime() + $"{strings.LoadProfile_Load}\n",
                Color = Color.White
            };
            return log;
        }

        public static Log DllNotFount(string name)
        {
            var log = new Log
            {
                Content = GetDateTime() + $"{strings.DllNotLoad} '{name}' {strings.DllNotLoad_1}",
                Color = Color.White
            };
            return log;
        }

        public static Log DoFarm_Start()
        {
            var log = new Log
            {
                Content = GetDateTime() + $"{strings.DoFarm_Start}\n",
                Color = Color.White
            };
            return log;
        }

        public static Log DoFarm_Finish(string time)
        {
            var log = new Log
            {
                Content = GetDateTime() + $"{strings.DoFarm_Finish} за {time}\n",
                Color = Color.White
            };
            return log;
        }

        public static Log GiveawayJoined(string site, string name, int price, int points, int level)
        {
            Properties.Settings.Default.JoinsPerSession += 1;
            Properties.Settings.Default.JoinsTotal += 1;
            Properties.Settings.Default.Save();

            var log = new Log();

            if (level > 0)
            {
                log.Content =
                    $"{GetDateTime()}[{site}] {strings.GiveawayJoined_Join} '{name}' [{level}] {strings.GiveawayJoined_ConfirmedFor} {price} " +
                    $"{strings.GiveawayJoined_Points} ({points}) \n";
            }
            else
            {
                log.Content =
                    $"{GetDateTime()}[{site}] {strings.GiveawayJoined_Join} '{name}' {strings.GiveawayJoined_ConfirmedFor} {price} " +
                    $"{strings.GiveawayJoined_Points} ({points}) \n";
            }

            log.Color = Color.Green;
            return log;
        }

        public static Log GiveawayNotJoined(string site, string name, string message)
        {
            var log = new Log
            {
                Content =
                    $"{GetDateTime()}{{{site} {strings.GiveawayJoined_Join} '{name}' {strings.GiveawayNotJoined_NotConfirmed} {{{message}}}\n",
                Color = Color.Red
            };
            return log;
        }

        public static Log GiveawayHaveWon(string site, int count)
        {
            return ConstructLog(GetDateTime() + "{" + site + @"} " + $"{strings.GiveawaysHaveWon} ({count})",
                Color.Orange, true, true);
        }

        public static Log GroupJoined(string url)
        {
            return ConstructLog(strings.SteamGroupJoined + " {" + url + '}', Color.Yellow, true, true);
        }

        public static Log GroupNotJoinde(string url)
        {
            return ConstructLog(strings.SteamGroupNotJoined + " {" + url + '}', Color.Yellow, false, true);
        }
    }
}