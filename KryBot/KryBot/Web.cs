using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using RestSharp;

namespace KryBot
{
    internal class Web
    {
        public static Classes.Response Get(string url, string subUrl, List<Parameter> parameters,
            CookieContainer cookies, List<HttpHeader> headers, string userAgent, string proxy)
        {
            try
            {
                var client = new RestClient(url)
                {
                    UserAgent = userAgent == "" ? null : userAgent,
                    FollowRedirects = true,
                    CookieContainer = cookies
                };

                if (proxy != "")
                {
                    client.Proxy = new WebProxy(proxy);
                }

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

                Thread.Sleep(200);

                return data;
            }
            catch (TimeoutException)
            {
                return null;
            }
        }

        public static async Task<Classes.Response> GetAsync(string url, string subUrl, List<Parameter> parameters,
            CookieContainer cookies, List<HttpHeader> headers, string userAgent, string proxy)
        {
            var task = new TaskCompletionSource<Classes.Response>();
            await Task.Run(() =>
            {
                try
                {
                    var result = Get(url, subUrl, parameters, cookies, headers, userAgent, proxy);
                    task.SetResult(result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            });

            return task.Task.Result;
        }

        public static Classes.Response Post(string url, string subUrl, List<Parameter> parameters,
            List<HttpHeader> headers, CookieContainer cookies, string userAgent, string proxy)
        {
            try
            {
                var client = new RestClient(url)
                {
                    UserAgent = userAgent == "" ? null : userAgent,
                    FollowRedirects = true,
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

                return data;
            }
            catch (TimeoutException)
            {
                return null;
            }
        }

        public static async Task<Classes.Response> PostAsync(string url, string subUrl, List<Parameter> parameters,
            List<HttpHeader> headers, CookieContainer cookie, string userAgent, string proxy)
        {
            var task = new TaskCompletionSource<Classes.Response>();
            await Task.Run(() =>
            {
                try
                {
                    var result = Post(url, subUrl, parameters, headers, cookie, userAgent, proxy);
                    task.SetResult(result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            });

            return task.Task.Result;
        }

        public static Classes.Log GameMinerJoinGiveaway(Classes.Bot bot, GameMiner.GmGiveaway giveaway)
        {

            Thread.Sleep(200);
            var response = Post("http://gameminer.net/",
                "/giveaway/enter/" + giveaway.Id + "?" + (giveaway.IsSandbox ? "sandbox" : "coal") + "_page=" +
                giveaway.Page, Generate.PostData_GameMiner(bot.GameMinerxsrf), new List<HttpHeader>(),
                Generate.Cookies_GameMiner(bot.GameMinerToken, bot.GameMinerxsrf, "ru_RU"), bot.UserAgent,
                bot.Proxy ?? "");
            try
            {
                var jsonresponse = JsonConvert.DeserializeObject<GameMiner.JsonResponse>(response.RestResponse.Content);
                if (jsonresponse.Status == "ok")
                {
                    bot.GameMinerCoal = jsonresponse.Coal;
                    return Messages.GiveawayJoined("GameMiner", giveaway.Name, giveaway.Price, jsonresponse.Coal, 0);
                }
                var jresponse = JsonConvert.DeserializeObject<GameMiner.JsonResponseError>(response.RestResponse.Content);
                return Messages.GiveawayNotJoined("GameMiner", giveaway.Name, jresponse.Error.Message);
            }
            catch (JsonReaderException)
            {
                return Messages.GiveawayNotJoined("GameMiner", giveaway.Name, response.RestResponse.Content);
            }
        }

        public static async Task<Classes.Log> GameMinerJoinGiveawayAsync(Classes.Bot bot, GameMiner.GmGiveaway giveaway)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                try
                {
                    var result = GameMinerJoinGiveaway(bot, giveaway);
                    task.SetResult(result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            });

            return task.Task.Result;
        }

        public static Classes.Log SteamGiftsJoinGiveaway(Classes.Bot bot, SteamGifts.SgGiveaway giveaway)
        {
            Thread.Sleep(100);
            giveaway = Parse.SteamGiftsGetJoinData(giveaway, bot.SteamGiftsPhpSessId, bot.UserAgent, bot.Proxy ?? "");
            if (giveaway.Token != null)
            {
                var response = Post("http://www.steamgifts.com/", "/ajax.php",
                    Generate.PostData_SteamGifts(giveaway.Token, giveaway.Code, "entry_insert"), new List<HttpHeader>(),
                    Generate.Cookies_SteamGifts(bot.SteamGiftsPhpSessId), bot.UserAgent, bot.Proxy ?? "");
                if (response != null)
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
                    try
                    {
                        return Messages.GiveawayNotJoined("SteamGifts", giveaway.Name, jresponse.Error.Message);
                    }
                    catch (NullReferenceException)
                    {
                    }
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
                try
                {
                    var result = SteamGiftsJoinGiveaway(bot, giveaway);
                    task.SetResult(result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            });

            return task.Task.Result;
        }

        public static Classes.Log SteamCompanionJoinGiveaway(Classes.Bot bot, SteamCompanion.ScGiveaway giveaway)
        {
            Thread.Sleep(300);
            var data = Parse.SteamCompanionGetJoinData(giveaway, bot.SteamCompanionPhpSessId, bot.SteamCompanionUserId,
                bot.SteamCompanionUserC,
                bot.SteamCompanionUserT, bot.UserAgent, bot.Name, bot.Proxy ?? "", bot.SteamSessid,
                bot.SteamCompanionAutoJoin, bot.SteamLogin);
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
                        Generate.Cookies_SteamCompanion(bot.SteamCompanionPhpSessId, bot.SteamCompanionUserC,
                            bot.SteamCompanionUserId, bot.SteamCompanionUserT), bot.UserAgent, bot.Proxy ?? "");
							
					try
					{
						if (response.RestResponse.Content.Split('"')[3].Split('"')[0] == "Success")
						{
							bot.SteamCompanionPoint = int.Parse(response.RestResponse.Content.Split(':')[2].Split(',')[0]);
							return Messages.GiveawayJoined("SteamCompanion", giveaway.Name, giveaway.Price,
								int.Parse(response.RestResponse.Content.Split(':')[2].Split(',')[0]), 0);
						}
					}
					catch(IndexOutOfRangeException)
					{
						//MessageBox.Show(@"IndexOutOfRangeException: " + response.RestResponse.Content, @"Неожиданное сообщение от сервера");
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
                try
                {
                    var result = SteamCompanionJoinGiveaway(bot, giveaway);
                    task.SetResult(result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            });

            return task.Task.Result;
        }

        public static Classes.Log SteamPortalJoinGiveaway(Classes.Bot bot, SteamPortal.SpGiveaway giveaway)
        {
            Thread.Sleep(100);
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
                    Generate.Cookies_SteamPortal(bot.SteamPortalPhpSessId), bot.UserAgent, bot.Proxy ?? "");
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
                try
                {
                    var result = SteamPortalJoinGiveaway(bot, giveaway);
                    task.SetResult(result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            });

            return task.Task.Result;
        }

        public static Classes.Log GameAwayslJoinGiveaway(Classes.Bot bot, GameAways.SaGiveaway giveaway)
        {
            Thread.Sleep(100);
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
                    Generate.Cookies_GameAways(bot.GameAwaysPhpSessId), list, bot.UserAgent, bot.Proxy ?? "");
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
                try
                {
                    var result = GameAwayslJoinGiveaway(bot, giveaway);
                    task.SetResult(result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            });

            return task.Task.Result;
        }

        public static Classes.Log SteamTradeJoinGiveaway(Classes.Bot bot, SteamTrade.StGiveaway giveaway)
        {
            Thread.Sleep(100);
            giveaway = Parse.SteamTradeGetJoinData(giveaway, bot.SteamTradePhpSessId, bot.UserAgent, bot.Name,
                bot.Proxy ?? "", bot.SteamTradeDleUserId, bot.SteamTradeDlePassword, bot.SteamTradePassHash);
            if (giveaway.LinkJoin != null)
            {
                var response = Get("http://steamtrade.info", giveaway.LinkJoin, new List<Parameter>(),
                    Generate.Cookies_SteamTrade(bot.SteamTradePhpSessId, bot.SteamTradeDleUserId,
                        bot.SteamTradeDlePassword, bot.SteamTradePassHash),
                    new List<HttpHeader>(), bot.UserAgent,
                    bot.Proxy);
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
                try
                {
                    var result = SteamTradeJoinGiveaway(bot, giveaway);
                    task.SetResult(result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
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

            return data;
        }

        public static bool SteamJoinGroup(string url, string subUrl, List<Parameter> parameters, CookieContainer cookies,
            List<HttpHeader> headers, string userAgent, string proxy)
        {
            var response = Post(url, subUrl, parameters, new List<HttpHeader>(), cookies, userAgent, proxy);
            if (response.RestResponse.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }

        public static string GetVersionInGitHub(string url)
        {
            try
            {
                var client = new RestClient(url)
                {
                    FollowRedirects = true,
                };

                var request = new RestRequest("", Method.GET);

                var response = client.Execute(request);

                return response.Content;
            }
            catch (TimeoutException)
            {
                return null;
            }
        }

        public static async Task<string> GetVersionInGitHubAsync(string url)
        {
            var task = new TaskCompletionSource<string>();
            await Task.Run(() =>
            {
                try
                {
                    var result = GetVersionInGitHub(url);
                    task.SetResult(result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            });

            return task.Task.Result;
        }

        public static Classes.Log SteamGiftsSyncAccount(Classes.Bot bot)
        {

            var xsrf = Get("http://www.steamgifts.com/account/profile/sync", "",
                new List<Parameter>(), Generate.Cookies_SteamGifts(bot.SteamGiftsPhpSessId), new List<HttpHeader>(), "", "");
            if (xsrf != null)
            {
                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(xsrf.RestResponse.Content);

                string xsrfToken;
                try
                {
                    xsrfToken =
                        htmlDoc.DocumentNode.SelectSingleNode("//input[@name='xsrf_token']").Attributes["value"].Value;
                }
                catch (NullReferenceException)
                {
                    return null;
                }

                List<HttpHeader> headers = new List<HttpHeader>();
                HttpHeader header = new HttpHeader
                {
                    Name = "X-Requested-With",
                    Value = "XMLHttpRequest"
                };
                headers.Add(header);

                var response = Post("http://www.steamgifts.com/", "ajax.php",
                    Generate.PostData_SteamGifts(xsrfToken, "", "sync"), headers,
                    Generate.Cookies_SteamGifts(bot.SteamGiftsPhpSessId), "", "");
                if (response != null)
                {
                    var result =
                        JsonConvert.DeserializeObject<SteamGifts.JsonResponseSyncAccount>(response.RestResponse.Content);
                    if (result.type == "success")
                    {
                        return Tools.ConstructLog(Messages.GetDateTime() + "{SteamGifts} " + result.msg, Color.Green,
                            true, true);
                    }
                    return Tools.ConstructLog(Messages.GetDateTime() + "{SteamGifts} " + result.msg, Color.Red, false,
                        true);
                }
            }
            return null;
        }

        public static async Task<Classes.Log> SteamGiftsSyncAccountAsync(Classes.Bot bot)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                try
                {
                    var result = SteamGiftsSyncAccount(bot);
                    task.SetResult(result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            });

            return task.Task.Result;
        }

        public static Classes.Log SteamCompanionSyncAccount(Classes.Bot bot)
        {
            var response = Get("https://steamcompanion.com//settings/resync&success=true", "", new List<Parameter>(),
                Generate.Cookies_SteamCompanion(bot.SteamCompanionPhpSessId, bot.SteamCompanionUserC,
                    bot.SteamCompanionUserId, bot.SteamCompanionUserT), new List<HttpHeader>(), "", "");
            if (response != null)
            {
                return Tools.ConstructLog(Messages.GetDateTime() + "{SteamGifts} Success!", Color.Green, true, true);
            }
            return Tools.ConstructLog(Messages.GetDateTime() + "{SteamGifts} Failed", Color.Red, false, true);
        }

        public static async Task<Classes.Log> SteamCompanionSyncAccountAsync(Classes.Bot bot)
        {
            var task = new TaskCompletionSource<Classes.Log>();
            await Task.Run(() =>
            {
                try
                {
                    var result = SteamCompanionSyncAccount(bot);
                    task.SetResult(result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            });

            return task.Task.Result;
        }
    }
}