using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using KryBot.lang;
using Newtonsoft.Json;
using RestSharp;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace KryBot
{
    internal class Web
    {
        private static int requestInterval = 400;

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

        public static Classes.Log GameMinerJoinGiveaway(Classes.Bot bot, GameMiner.GmGiveaway giveaway)
        {

            Thread.Sleep(requestInterval);
            var response = Post("http://gameminer.net/",
                "/giveaway/enter/" + giveaway.Id + "?" + (giveaway.IsSandbox ? "sandbox" : "coal") + "_page=" +
                giveaway.Page, Generate.PostData_GameMiner(bot.GameMinerxsrf), new List<HttpHeader>(),
                Generate.Cookies_GameMiner(bot), bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                try
                {
                    var jsonresponse =
                        JsonConvert.DeserializeObject<GameMiner.JsonResponse>(response.RestResponse.Content);
                    if (jsonresponse.Status == "ok")
                    {
                        bot.GameMinerCoal = jsonresponse.Coal;
                        return Messages.GiveawayJoined("GameMiner", giveaway.Name, giveaway.Price, jsonresponse.Coal, 0);
                    }
                    var jresponse =
                        JsonConvert.DeserializeObject<GameMiner.JsonResponseError>(response.RestResponse.Content);
                    return Messages.GiveawayNotJoined("GameMiner", giveaway.Name, jresponse.Error.Message);
                }
                catch (JsonReaderException)
                {
                    return Messages.GiveawayNotJoined("GameMiner", giveaway.Name, response.RestResponse.Content);
                }
            }
            return Messages.GiveawayNotJoined("GameMiner", giveaway.Name, "Content is empty");
        }

        public static async Task<Classes.Log> GameMinerJoinGiveawayAsync(Classes.Bot bot, GameMiner.GmGiveaway giveaway)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = GameMinerJoinGiveaway(bot, giveaway);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Log SteamGiftsJoinGiveaway(Classes.Bot bot, SteamGifts.SgGiveaway giveaway)
        {
            Thread.Sleep(requestInterval);
            giveaway = Parse.SteamGiftsGetJoinData(giveaway, bot);

            if (giveaway.Token != null)
            {
                var response = Post("http://www.steamgifts.com/", "/ajax.php",
                    Generate.PostData_SteamGifts(giveaway.Token, giveaway.Code, "entry_insert"), new List<HttpHeader>(),
                    Generate.Cookies_SteamGifts(bot), bot.UserAgent);

                if (response.RestResponse.Content != null)
                {
                    var jsonresponse =
                        JsonConvert.DeserializeObject<SteamGifts.JsonResponseJoin>(response.RestResponse.Content);
                    if (jsonresponse.Type == "success")
                    {
                        bot.SteamGiftsPoint = jsonresponse.Points;
                        return Messages.GiveawayJoined("SteamGifts", giveaway.Name, giveaway.Price, jsonresponse.Points,
                            giveaway.Level);
                    }

                    var jresponse =
                        JsonConvert.DeserializeObject<GameMiner.JsonResponseError>(response.RestResponse.Content);
                    return Messages.GiveawayNotJoined("SteamGifts", giveaway.Name, jresponse.Error?.Message);
                }
            }
            return null;
        }

        public static async Task<Classes.Log> SteamGiftsJoinGiveawayAsync(Classes.Bot bot,
            SteamGifts.SgGiveaway giveaway)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = SteamGiftsJoinGiveaway(bot, giveaway);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Log SteamCompanionJoinGiveaway(Classes.Bot bot, SteamCompanion.ScGiveaway giveaway)
        {
            Thread.Sleep(requestInterval);
            var data = Parse.SteamCompanionGetJoinData(giveaway, bot);
            if (data != null && data.Success)
            {
                if (giveaway.Code != null)
                {
                    var list = new List<HttpHeader>();
                    var header = new HttpHeader
                    {
                        Name = "X-Requested-With",
                        Value = "XMLHttpRequest"
                    };
                    list.Add(header);

                    var response = Post("https://steamcompanion.com", "/gifts/steamcompanion.php",
                        Generate.PostData_SteamCompanion(giveaway.Code), list,
                        Generate.Cookies_SteamCompanion(bot), bot.UserAgent);

					if (response.RestResponse.Content.Split('"')[3].Split('"')[0] == "Success")
					{
						bot.SteamCompanionPoint = int.Parse(response.RestResponse.Content.Split(':')[2].Split(',')[0]);
						return Messages.GiveawayJoined("SteamCompanion", giveaway.Name, giveaway.Price,
							int.Parse(response.RestResponse.Content.Split(':')[2].Split(',')[0]), 0);
					}
					
                    return Messages.GiveawayNotJoined("SteamCompanion", giveaway.Name, response.RestResponse.Content);
                }
            }
            return data;
        }

        public static async Task<Classes.Log> SteamCompanionJoinGiveawayAsync(Classes.Bot bot,
            SteamCompanion.ScGiveaway giveaway)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = SteamCompanionJoinGiveaway(bot, giveaway);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Log SteamPortalJoinGiveaway(Classes.Bot bot, SteamPortal.SpGiveaway giveaway)
        {
            Thread.Sleep(requestInterval);
            if (giveaway.Code != null)
            {
                var list = new List<HttpHeader>();
                var header = new HttpHeader
                {
                    Name = "X-Requested-With",
                    Value = "XMLHttpRequest"
                };
                list.Add(header);

                var response = Post("http://steamportal.net/", "page/join",
                    Generate.PostData_SteamPortal(giveaway.Code), list,
                    Generate.Cookies_SteamPortal(bot), bot.UserAgent);
                if (response.RestResponse.Content.Contains("Joined already"))
                {
                    bot.SteamPortalPoints = int.Parse(response.RestResponse.Content.Split(':')[2].Split(',')[0]);
                    return Messages.GiveawayJoined("SteamPortal", giveaway.Name, giveaway.Price,
                        int.Parse(response.RestResponse.Content.Split(':')[2].Split(',')[0]), 0);
                }
                return Messages.GiveawayNotJoined("SteamPortal", giveaway.Name, "Error");
            }
            return null;
        }

        public static async Task<Classes.Log> SteamPortalJoinGiveawayAsync(Classes.Bot bot,
            SteamPortal.SpGiveaway giveaway)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                    var result = SteamPortalJoinGiveaway(bot, giveaway);
                    task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Log GameAwayslJoinGiveaway(Classes.Bot bot, GameAways.SaGiveaway giveaway)
        {
            Thread.Sleep(requestInterval);
            if (giveaway.Id != null)
            {
                var list = new List<HttpHeader>();
                var header = new HttpHeader
                {
                    Name = "X-Requested-With",
                    Value = "XMLHttpRequest"
                };
                list.Add(header);

                var response = Get(giveaway.Link, "",
                    Generate.GetData_GameAways(giveaway.Id),
                    Generate.Cookies_GameAways(bot), list, bot.UserAgent);
                var jsonresponse = JsonConvert.DeserializeObject<GameAways.JsonResponse>(response.RestResponse.Content);
                if (response.RestResponse.Content != "")
                {
                    bot.GameAwaysPoints = jsonresponse.Balance;
                    return Messages.GiveawayJoined("GameAways", giveaway.Name, giveaway.Price,
                        int.Parse(response.RestResponse.Content.Split(':')[2].Split(',')[0]), 0);
                }
                return Messages.GiveawayNotJoined("GameAways", giveaway.Name, "Error");
            }
            return null;
        }

        public static async Task<Classes.Log> GameAwaysJoinGiveawayAsync(Classes.Bot bot, GameAways.SaGiveaway giveaway)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = GameAwayslJoinGiveaway(bot, giveaway);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Log SteamTradeJoinGiveaway(Classes.Bot bot, SteamTrade.StGiveaway giveaway)
        {
            Thread.Sleep(requestInterval);
            giveaway = Parse.SteamTradeGetJoinData(giveaway, bot);
            if (giveaway.LinkJoin != null)
            {
                var response = Get("http://steamtrade.info", giveaway.LinkJoin, new List<Parameter>(),
                    Generate.Cookies_SteamTrade(bot),
                    new List<HttpHeader>(), bot.UserAgent);
                if (response.RestResponse.StatusCode == HttpStatusCode.OK)
                {
                    return Messages.GiveawayJoined("SteamTrade", giveaway.Name.Trim(), 0, 0, 0);
                }
                return Messages.GiveawayNotJoined("SteamTrade", giveaway.Name,
                    response.RestResponse.StatusCode.ToString());
            }
            return null;
        }

        public static async Task<Classes.Log> SteamTradeJoinGiveawayAsync(Classes.Bot bot,
            SteamTrade.StGiveaway giveaway)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = SteamTradeJoinGiveaway(bot, giveaway);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Response SteamTradeDoAuth(string url, string subUrl, List<Parameter> parameters,
            CookieContainer cookies, List<HttpHeader> headers, string userAgent, string proxy)
        {
            var client = new RestClient(url)
            {
                UserAgent = userAgent == "" ? Tools.UserAgent() : userAgent,
                FollowRedirects = false,
                CookieContainer = cookies
            };

            if (proxy != "")
            {
                client.Proxy = new WebProxy(proxy);
            }

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

        public static Classes.Response GameAwaysDoAuth(string url, string subUrl, List<Parameter> parameters,
            CookieContainer cookies, List<HttpHeader> headers, string userAgent, string proxy)
        {
            var client = new RestClient(url)
            {
                UserAgent = userAgent == "" ? Tools.UserAgent() : userAgent,
                FollowRedirects = false,
                CookieContainer = cookies
            };

            if (proxy != "")
            {
                client.Proxy = new WebProxy(proxy);
            }

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

        public static Classes.Log SteamJoinGroup(string url, string subUrl, List<Parameter> parameters, CookieContainer cookies,
            List<HttpHeader> headers, string userAgent)
        {
            var response = Post(url, subUrl, parameters, new List<HttpHeader>(), cookies, userAgent);
            if (response.RestResponse.Content != "")
            {
                HtmlDocument  htmlDoc = new HtmlDocument();
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

        public static string GetVersionInGitHub(string url)
        {
            var client = new RestClient(url)
            {
                FollowRedirects = true,
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

        public static Classes.Log SteamGiftsSyncAccount(Classes.Bot bot)
        {

            var xsrf = Get("http://www.steamgifts.com/account/profile/sync", "",
                new List<Parameter>(), Generate.Cookies_SteamGifts(bot), new List<HttpHeader>(), bot.UserAgent);

            if (xsrf.RestResponse.Content != null)
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(xsrf.RestResponse.Content);

                var xsrfToken = htmlDoc.DocumentNode.SelectSingleNode("//input[@name='xsrf_token']");
                if (xsrfToken != null)
                {
                    List<HttpHeader> headers = new List<HttpHeader>();
                    HttpHeader header = new HttpHeader
                    {
                        Name = "X-Requested-With",
                        Value = "XMLHttpRequest"
                    };
                    headers.Add(header);

                    var response = Post("http://www.steamgifts.com/", "ajax.php",
                        Generate.PostData_SteamGifts(xsrfToken.Attributes["value"].Value, "", "sync"), headers,
                        Generate.Cookies_SteamGifts(bot), bot.UserAgent);
                    if (response != null)
                    {
                        var result =
                            JsonConvert.DeserializeObject<SteamGifts.JsonResponseSyncAccount>(response.RestResponse.Content);
                        if (result.type == "success")
                        {
                            return Tools.ConstructLog($"{Messages.GetDateTime()} {{SteamGifts}} {result.msg}", Color.Green,
                                true, true);
                        }
                        return Tools.ConstructLog($"{Messages.GetDateTime()} {{SteamGifts}} {result.msg}", Color.Red, false,
                            true);
                    }
                }
            }
            return null;
        }

        public static async Task<Classes.Log> SteamGiftsSyncAccountAsync(Classes.Bot bot)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = SteamGiftsSyncAccount(bot);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Log SteamCompanionSyncAccount(Classes.Bot bot)
        {
            var response = Get("https://steamcompanion.com//settings/resync&success=true", "", new List<Parameter>(),
                Generate.Cookies_SteamCompanion(bot), new List<HttpHeader>(), bot.UserAgent);
            if (response.RestResponse.Content != "")
            {
                return Tools.ConstructLog($"{Messages.GetDateTime()} {{SteamCompanion}} Sync success!", Color.Green, true, true);
            }
            return Tools.ConstructLog($"{Messages.GetDateTime()} {{SteamCompanion}} Sync failed", Color.Red, false, true);
        }

        public static async Task<Classes.Log> SteamCompanionSyncAccountAsync(Classes.Bot bot)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = SteamCompanionSyncAccount(bot);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Log GameMinerSyncAccount(Classes.Bot bot)
        {
            var response = Post("http://gameminer.net/account/sync", "",
                    Generate.SyncPostData_GameMiner(bot.GameMinerxsrf), new List<HttpHeader>(), 
                    Generate.Cookies_GameMiner(bot), bot.UserAgent);

            if (response.RestResponse.Content != "")
            {
                if (response.RestResponse.StatusCode == HttpStatusCode.OK)
                {
                    return Tools.ConstructLog($"{Messages.GetDateTime()} {{GameMiner}} Sync success", Color.Green,
                        true, true);
                }
                return Tools.ConstructLog($"{Messages.GetDateTime()} {{GameMiner}} Sync failed", Color.Red, false,
                    true);
            }
            return Tools.ConstructLog($"{Messages.GetDateTime()} {{GameMiner}} {strings.ParseProfile_LoginOrServerError}", Color.Red, false,
                    true);
        }

        public static async Task<Classes.Log> GameMinerSyncAccountAsync(Classes.Bot bot)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = GameMinerSyncAccount(bot);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Classes.Log PlayBlinkJoinGiveaway(Classes.Bot bot, PlayBlink.PbGiveaway giveaway)
        {
            Thread.Sleep(1000);
            if (giveaway.Id != null)
            {
                var list = new List<HttpHeader>();
                var header = new HttpHeader
                {
                    Name = "X-Requested-With",
                    Value = "XMLHttpRequest"
                };
                list.Add(header);

                var response = Post("http://playblink.com/", $"?do=blink&game={giveaway.Id}&captcha=1",
                    Generate.PostData_PlayBlink(giveaway.Id), list,
                    Generate.Cookies_PlayBlink(bot), bot.UserAgent);

                if (response.RestResponse.StatusCode == HttpStatusCode.OK)
                {
                    if (response.RestResponse.Content != "")
                    {
                        HtmlDocument htmldoc = new HtmlDocument();
                        htmldoc.LoadHtml(response.RestResponse.Content);

                        var message =
                            htmldoc.DocumentNode.SelectSingleNode("//div[@class='msgbox success']");
                        if (message != null)
                        {
                            bot.PlayBlinkPoints = bot.PlayBlinkPoints - giveaway.Price;
                            return Messages.GiveawayJoined("PlayBlink", giveaway.Name, giveaway.Price,
                                bot.PlayBlinkPoints, 0);
                        }

                        var captcha = htmldoc.DocumentNode.SelectSingleNode("//div[@class='flash_rules']");
                        if (captcha != null)
                        {
                            bot.PlayBlinkPoints = bot.PlayBlinkPoints - giveaway.Price;
                            return Messages.GiveawayNotJoined("PlayBlink", giveaway.Name, "Captcha");
                        } 
                    }
                    return Messages.GiveawayNotJoined("PlayBlink", giveaway.Name, "Error");
                }
                return Messages.GiveawayNotJoined("PlayBlink", giveaway.Name, "Error");
            }
            return null;
        }

        public static async Task<Classes.Log> PlayBlinkJoinGiveawayAsync(Classes.Bot bot, PlayBlink.PbGiveaway giveaway)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                var result = PlayBlinkJoinGiveaway(bot, giveaway);
                task.SetResult(result);
            });

            return task.Task.Result;
        }
    }
}