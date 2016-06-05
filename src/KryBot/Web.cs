using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using KryBot.lang;
using Newtonsoft.Json;
using RestSharp;

namespace KryBot
{
    internal class Web
    {
        private static readonly int requestInterval = 400;

        public static Classes.Response Get(string url, string subUrl, List<Parameter> parameters,
            CookieContainer cookies, List<HttpHeader> headers, string userAgent)
        {
            var client = new RestClient(url)
            {
                UserAgent = userAgent == "" ? null : userAgent,
                FollowRedirects = true,
                CookieContainer = cookies
            };

            var request = new RestRequest(subUrl, Method.GET);

            foreach (var header in headers)
            {
                request.AddHeader(header.Name, header.Value);
            }

            foreach (var param in parameters)
            {
                request.AddParameter(param);
            }

            var response = client.Execute(request);

            var data = new Classes.Response
            {
                Cookies = client.CookieContainer,
                RestResponse = response
            };

            Thread.Sleep(requestInterval);

            return data;
        }

        public static Classes.Response Get(string url, string subUrl, List<Parameter> parameters,
            CookieContainer cookies, List<HttpHeader> headers)
        {
            var client = new RestClient(url)
            {
                FollowRedirects = true,
                CookieContainer = cookies
            };

            var request = new RestRequest(subUrl, Method.GET);

            foreach (var header in headers)
            {
                request.AddHeader(header.Name, header.Value);
            }

            foreach (var param in parameters)
            {
                request.AddParameter(param);
            }

            var response = client.Execute(request);

            var data = new Classes.Response
            {
                Cookies = client.CookieContainer,
                RestResponse = response
            };

            Thread.Sleep(requestInterval);

            return data;
        }

        public static async Task<Classes.Response> GetAsync(string url, string subUrl, List<Parameter> parameters,
            CookieContainer cookies, List<HttpHeader> headers, string userAgent)
        {
            var task = new TaskCompletionSource<Classes.Response>();
            await Task.Run(() =>
            {
                var result = Get(url, subUrl, parameters, cookies, headers, userAgent);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static async Task<Classes.Response> GetAsync(string url, string subUrl, List<Parameter> parameters,
            CookieContainer cookies, List<HttpHeader> headers)
        {
            var task = new TaskCompletionSource<Classes.Response>();
            await Task.Run(() =>
            {
                var result = Get(url, subUrl, parameters, cookies, headers);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Response Post(string url, string subUrl, List<Parameter> parameters,
            List<HttpHeader> headers, CookieContainer cookies, string userAgent)
        {
            var client = new RestClient(url)
            {
                UserAgent = userAgent == "" ? null : userAgent,
                FollowRedirects = true,
                CookieContainer = cookies
            };

            var request = new RestRequest(subUrl, Method.POST);

            foreach (var header in headers)
            {
                request.AddHeader(header.Name, header.Value);
            }

            foreach (var param in parameters)
            {
                request.AddParameter(param);
            }

            var response = client.Execute(request);
            var data = new Classes.Response
            {
                Cookies = client.CookieContainer,
                RestResponse = response
            };

            Thread.Sleep(requestInterval);

            return data;
        }

        public static async Task<Classes.Response> PostAsync(string url, string subUrl, List<Parameter> parameters,
            List<HttpHeader> headers, CookieContainer cookie, string userAgent)
        {
            var task = new TaskCompletionSource<Classes.Response>();
            await Task.Run(() =>
            {
                var result = Post(url, subUrl, parameters, headers, cookie, userAgent);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Response Post(string url, string subUrl, List<Parameter> parameters,
            List<HttpHeader> headers, CookieContainer cookies)
        {
            var client = new RestClient(url)
            {
                FollowRedirects = true,
                CookieContainer = cookies
            };

            var request = new RestRequest(subUrl, Method.POST);

            foreach (var header in headers)
            {
                request.AddHeader(header.Name, header.Value);
            }

            foreach (var param in parameters)
            {
                request.AddParameter(param);
            }

            var response = client.Execute(request);
            var data = new Classes.Response
            {
                Cookies = client.CookieContainer,
                RestResponse = response
            };

            Thread.Sleep(requestInterval);

            return data;
        }

        public static async Task<Classes.Response> PostAsync(string url, string subUrl, List<Parameter> parameters,
            List<HttpHeader> headers, CookieContainer cookie)
        {
            var task = new TaskCompletionSource<Classes.Response>();
            await Task.Run(() =>
            {
                var result = Post(url, subUrl, parameters, headers, cookie);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Log GameMinerJoinGiveaway(Bot bot, GameMiner.GmGiveaway gmGiveaway)
        {
            Thread.Sleep(requestInterval);
            var response = Post("http://gameminer.net/",
                "/giveaway/enter/" + gmGiveaway.Id + "?" + (gmGiveaway.IsSandbox ? "sandbox" : "coal") + "_page=" +
                gmGiveaway.Page, Generate.PostData_GameMiner(bot.GameMiner.Cookies.Xsrf), new List<HttpHeader>(),
                Generate.Cookies_GameMiner(bot), bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                try
                {
                    var jsonresponse =
                        JsonConvert.DeserializeObject<GameMiner.JsonResponse>(response.RestResponse.Content);
                    if (jsonresponse.Status == "ok")
                    {
                        bot.GameMiner.Coal = jsonresponse.Coal;
                        return Messages.GiveawayJoined("GameMiner", gmGiveaway.Name, gmGiveaway.Price, jsonresponse.Coal,
                            0);
                    }
                    var jresponse =
                        JsonConvert.DeserializeObject<GameMiner.JsonResponseError>(response.RestResponse.Content);
                    return Messages.GiveawayNotJoined("GameMiner", gmGiveaway.Name, jresponse.Error.Message);
                }
                catch (JsonReaderException)
                {
                    return Messages.GiveawayNotJoined("GameMiner", gmGiveaway.Name, response.RestResponse.Content);
                }
            }
            return Messages.GiveawayNotJoined("GameMiner", gmGiveaway.Name, "Content is empty");
        }

        public static async Task<Log> GameMinerJoinGiveawayAsync(Bot bot, GameMiner.GmGiveaway gmGiveaway)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = GameMinerJoinGiveaway(bot, gmGiveaway);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Log SteamGiftsJoinGiveaway(Bot bot, SteamGifts.SgGiveaway sgGiveaway)
        {
            Thread.Sleep(requestInterval);
            sgGiveaway = Parse.SteamGiftsGetJoinData(sgGiveaway, bot);

            if (sgGiveaway.Token != null)
            {
                var response = Post("https://www.steamgifts.com/", "/ajax.php",
                    Generate.PostData_SteamGifts(sgGiveaway.Token, sgGiveaway.Code, "entry_insert"),
                    new List<HttpHeader>(),
                    Generate.Cookies_SteamGifts(bot), bot.UserAgent);

                if (response.RestResponse.Content != null)
                {
                    var jsonresponse =
                        JsonConvert.DeserializeObject<SteamGifts.JsonResponseJoin>(response.RestResponse.Content);
                    if (jsonresponse.Type == "success")
                    {
                        bot.SteamGifts.Points = jsonresponse.Points;
                        return Messages.GiveawayJoined("SteamGifts", sgGiveaway.Name, sgGiveaway.Price,
                            jsonresponse.Points,
                            sgGiveaway.Level);
                    }

                    var jresponse =
                        JsonConvert.DeserializeObject<GameMiner.JsonResponseError>(response.RestResponse.Content);
                    return Messages.GiveawayNotJoined("SteamGifts", sgGiveaway.Name, jresponse.Error?.Message);
                }
            }
            return null;
        }

        public static async Task<Log> SteamGiftsJoinGiveawayAsync(Bot bot,
            SteamGifts.SgGiveaway sgGiveaway)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = SteamGiftsJoinGiveaway(bot, sgGiveaway);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Log SteamCompanionJoinGiveaway(Bot bot, SteamCompanion.ScGiveaway scGiveaway)
        {
            Thread.Sleep(requestInterval);
            var data = Parse.SteamCompanionGetJoinData(scGiveaway, bot);
            if (data != null && data.Success)
            {
                if (scGiveaway.Code != null)
                {
                    var list = new List<HttpHeader>();
                    var header = new HttpHeader
                    {
                        Name = "X-Requested-With",
                        Value = "XMLHttpRequest"
                    };
                    list.Add(header);

                    var response = Post("https://steamcompanion.com", "/gifts/steamcompanion.php",
                        Generate.PostData_SteamCompanion(scGiveaway.Code), list,
                        Generate.Cookies_SteamCompanion(bot));

                    if (response.RestResponse.Content.Split('"')[3].Split('"')[0] == "Success")
                    {
                        bot.SteamCompanion.Points = int.Parse(response.RestResponse.Content.Split(':')[2].Split(',')[0]);
                        return Messages.GiveawayJoined("SteamCompanion", scGiveaway.Name, scGiveaway.Price,
                            int.Parse(response.RestResponse.Content.Split(':')[2].Split(',')[0]), 0);
                    }

                    return Messages.GiveawayNotJoined("SteamCompanion", scGiveaway.Name, response.RestResponse.Content);
                }
            }
            return data;
        }

        public static async Task<Log> SteamCompanionJoinGiveawayAsync(Bot bot,
            SteamCompanion.ScGiveaway scGiveaway)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = SteamCompanionJoinGiveaway(bot, scGiveaway);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Log UseGamblelJoinGiveaway(Bot bot, UseGamble.UgGiveaway ugGiveaway)
        {
            Thread.Sleep(requestInterval);
            if (ugGiveaway.Code != null)
            {
                var list = new List<HttpHeader>();
                var header = new HttpHeader
                {
                    Name = "X-Requested-With",
                    Value = "XMLHttpRequest"
                };
                list.Add(header);

                var response = Post("http://usegamble.com/", "page/join",
                    Generate.PostData_UseGamble(ugGiveaway.Code), list,
                    Generate.Cookies_UseGamble(bot));
                var jresponse =
                    JsonConvert.DeserializeObject<UseGamble.JsonJoin>(response.RestResponse.Content.Replace(".", ""));
                if (jresponse.Error == 0)
                {
                    bot.UseGamble.Points = jresponse.target_h.my_coins;
                    return Messages.GiveawayJoined("UseGamble", ugGiveaway.Name, ugGiveaway.Price,
                        jresponse.target_h.my_coins, 0);
                }
                return Messages.GiveawayNotJoined("UseGamble", ugGiveaway.Name, "Error");
            }
            return null;
        }

        public static async Task<Log> UseGambleJoinGiveawayAsync(Bot bot,
            UseGamble.UgGiveaway ugGiveaway)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = UseGamblelJoinGiveaway(bot, ugGiveaway);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Log SteamTradeJoinGiveaway(Bot bot, SteamTrade.StGiveaway stGiveaway)
        {
            Thread.Sleep(requestInterval);
            stGiveaway = Parse.SteamTradeGetJoinData(stGiveaway, bot);
            if (stGiveaway.LinkJoin != null)
            {
                var response = Get("http://steamtrade.info", stGiveaway.LinkJoin, new List<Parameter>(),
                    Generate.Cookies_SteamTrade(bot),
                    new List<HttpHeader>());
                if (response.RestResponse.StatusCode == HttpStatusCode.OK)
                {
                    return Messages.GiveawayJoined("SteamTrade", stGiveaway.Name.Trim(), 0, 0, 0);
                }
                return Messages.GiveawayNotJoined("SteamTrade", stGiveaway.Name,
                    response.RestResponse.StatusCode.ToString());
            }
            return null;
        }

        public static async Task<Log> SteamTradeJoinGiveawayAsync(Bot bot,
            SteamTrade.StGiveaway stGiveaway)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = SteamTradeJoinGiveaway(bot, stGiveaway);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Response SteamTradeDoAuth(string url, string subUrl, List<Parameter> parameters,
            CookieContainer cookies, List<HttpHeader> headers)
        {
            var client = new RestClient(url)
            {
                FollowRedirects = false,
                CookieContainer = cookies
            };

            var request = new RestRequest(subUrl, Method.POST);

            foreach (var header in headers)
            {
                request.AddHeader(header.Name, header.Value);
            }

            foreach (var param in parameters)
            {
                request.AddParameter(param);
            }

            var response = client.Execute(request);
            var data = new Classes.Response
            {
                Cookies = client.CookieContainer,
                RestResponse = response
            };

            Thread.Sleep(requestInterval);

            return data;
        }

        public static Log SteamJoinGroup(string url, string subUrl, List<Parameter> parameters,
            CookieContainer cookies, List<HttpHeader> headers)
        {
            var response = Post(url, subUrl, parameters, new List<HttpHeader>(), cookies);
            if (response.RestResponse.Content != "")
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(response.RestResponse.Content);

                var node = htmlDoc.DocumentNode.SelectSingleNode("//a[@class='btn_blue_white_innerfade btn_medium']");
                if (node != null)
                {
                    return Messages.GroupJoined(url);
                }

                var error = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='error_ctn']");
                if (error != null && error.InnerText.Contains("You are already a member of this group."))
                {
                    return Messages.GroupAlreadyMember(url);
                }
            }
            return Messages.GroupNotJoinde(url);
        }

        public static async Task<Log> SteamJoinGroupAsync(string url, string subUrl, List<Parameter> parameters,
            CookieContainer cookies, List<HttpHeader> headers)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = SteamJoinGroup(url, subUrl, parameters, cookies, headers);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static string GetVersionInGitHub(string url)
        {
            var client = new RestClient(url)
            {
                FollowRedirects = true
            };

            var request = new RestRequest("", Method.GET);

            var response = client.Execute(request);

            return response.Content;
        }

        public static async Task<string> GetVersionInGitHubAsync(string url)
        {
            var task = new TaskCompletionSource<string>();
            await Task.Run(() =>
            {
                var result = GetVersionInGitHub(url);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Log SteamGiftsSyncAccount(Bot bot)
        {
            var xsrf = Get("https://www.steamgifts.com/account/profile/sync", "",
                new List<Parameter>(), Generate.Cookies_SteamGifts(bot), new List<HttpHeader>());

            if (xsrf.RestResponse.Content != null)
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(xsrf.RestResponse.Content);

                var xsrfToken = htmlDoc.DocumentNode.SelectSingleNode("//input[@name='xsrf_token']");
                if (xsrfToken != null)
                {
                    var headers = new List<HttpHeader>();
                    var header = new HttpHeader
                    {
                        Name = "X-Requested-With",
                        Value = "XMLHttpRequest"
                    };
                    headers.Add(header);

                    var response = Post("https://www.steamgifts.com/", "ajax.php",
                        Generate.PostData_SteamGifts(xsrfToken.Attributes["value"].Value, "", "sync"), headers,
                        Generate.Cookies_SteamGifts(bot), bot.UserAgent);
                    if (response != null)
                    {
                        var result =
                            JsonConvert.DeserializeObject<SteamGifts.JsonResponseSyncAccount>(
                                response.RestResponse.Content);
                        if (result.Type == "success")
                        {
                            return new Log($"{Messages.GetDateTime()} {{SteamGifts}} {result.Msg}",
                                Color.Green,
                                true, true);
                        }
                        return new Log($"{Messages.GetDateTime()} {{SteamGifts}} {result.Msg}", Color.Red,
                            false,
                            true);
                    }
                }
            }
            return null;
        }

        public static async Task<Log> SteamGiftsSyncAccountAsync(Bot bot)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = SteamGiftsSyncAccount(bot);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Log SteamCompanionSyncAccount(Bot bot)
        {
            var response = Get("https://steamcompanion.com//settings/resync&success=true", "", new List<Parameter>(),
                Generate.Cookies_SteamCompanion(bot), new List<HttpHeader>());
            if (response.RestResponse.Content != "")
            {
                return new Log($"{Messages.GetDateTime()} {{SteamCompanion}} Sync success!", Color.Green,
                    true, true);
            }
            return new Log($"{Messages.GetDateTime()} {{SteamCompanion}} Sync failed", Color.Red, false, true);
        }

        public static async Task<Log> SteamCompanionSyncAccountAsync(Bot bot)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = SteamCompanionSyncAccount(bot);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Log GameMinerSyncAccount(Bot bot)
        {
            var response = Post("http://gameminer.net/account/sync", "",
                Generate.SyncPostData_GameMiner(bot.GameMiner.Cookies.Xsrf), new List<HttpHeader>(),
                Generate.Cookies_GameMiner(bot), bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                if (response.RestResponse.StatusCode == HttpStatusCode.OK)
                {
                    return new Log($"{Messages.GetDateTime()} {{GameMiner}} Sync success", Color.Green,
                        true, true);
                }
                return new Log($"{Messages.GetDateTime()} {{GameMiner}} Sync failed", Color.Red, false,
                    true);
            }
            return
                new Log($"{Messages.GetDateTime()} {{GameMiner}} {strings.ParseProfile_LoginOrServerError}",
                    Color.Red, false,
                    true);
        }

        public static async Task<Log> GameMinerSyncAccountAsync(Bot bot)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = GameMinerSyncAccount(bot);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Log PlayBlinkJoinGiveaway(Bot bot, PlayBlink.PbGiveaway pbGiveaway)
        {
            Thread.Sleep(1000);
            if (pbGiveaway.Id != null)
            {
                var list = new List<HttpHeader>();
                var header = new HttpHeader
                {
                    Name = "X-Requested-With",
                    Value = "XMLHttpRequest"
                };
                list.Add(header);

                var response = Post("http://playblink.com/", $"?do=blink&game={pbGiveaway.Id}&captcha=1",
                    Generate.PostData_PlayBlink(pbGiveaway.Id), list,
                    Generate.Cookies_PlayBlink(bot));

                if (response.RestResponse.StatusCode == HttpStatusCode.OK)
                {
                    if (response.RestResponse.Content != "")
                    {
                        var htmldoc = new HtmlDocument();
                        htmldoc.LoadHtml(response.RestResponse.Content);

                        bot.PlayBlink.Points = bot.PlayBlink.Points - pbGiveaway.Price;

                        var message = htmldoc.DocumentNode.SelectSingleNode("//div[@class='msgbox success']");
                        if (message != null)
                        {
                            return Messages.GiveawayJoined("PlayBlink", pbGiveaway.Name, pbGiveaway.Price,
                                bot.PlayBlink.Points, 0);
                        }

                        var error = htmldoc.DocumentNode.SelectSingleNode("//div[@class='msgbox error']");
                        if (error != null)
                        {
                            return Messages.GiveawayNotJoined("PlayBlink", pbGiveaway.Name, error.InnerText);
                        }

                        var captcha = htmldoc.DocumentNode.SelectSingleNode("//div[@class='flash_rules']");
                        if (captcha != null)
                        {
                            return Messages.GiveawayNotJoined("PlayBlink", pbGiveaway.Name, "Captcha");
                        }
                    }
                    return Messages.GiveawayNotJoined("PlayBlink", pbGiveaway.Name, "Error");
                }
                return Messages.GiveawayNotJoined("PlayBlink", pbGiveaway.Name, "Error");
            }
            return null;
        }

        public static async Task<Log> PlayBlinkJoinGiveawayAsync(Bot bot, PlayBlink.PbGiveaway pbGiveaway)
        {
            var task = new TaskCompletionSource<Log>();
            await Task.Run(() =>
            {
                var result = PlayBlinkJoinGiveaway(bot, pbGiveaway);
                task.SetResult(result);
            });

            return task.Task.Result;
        }
    }
}