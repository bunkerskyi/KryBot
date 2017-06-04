/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
*/
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace KryBot.Core
{
    public static class Web
    {
        private static readonly int requestInterval = 400;

        private static Response Get(string url)
        {
            var client = new RestClient(url)
            {
                FollowRedirects = true
            };

            var request = new RestRequest(string.Empty, Method.GET);

            var response = client.Execute(request);

            Thread.Sleep(requestInterval);

            return new Response(client.CookieContainer, response);
        }

        public static Response Get(string url, CookieContainer cookies, string userAgent)
        {
            var client = new RestClient(url)
            {
                UserAgent = userAgent,
                FollowRedirects = true,
                CookieContainer = cookies
            };

            var request = new RestRequest(string.Empty, Method.GET);

            var response = client.Execute(request);

            Thread.Sleep(requestInterval);

            return new Response(client.CookieContainer, response);
        }

        public static Response Get(string url, CookieContainer cookies)
        {
            var client = new RestClient(url)
            {
                FollowRedirects = true,
                CookieContainer = cookies
            };

            var request = new RestRequest(string.Empty, Method.GET);

            var response = client.Execute(request);

            Thread.Sleep(requestInterval);

            return new Response(client.CookieContainer, response);
        }

        public static async Task<Response> GetAsync(string url)
        {
            var task = new TaskCompletionSource<Response>();
            await Task.Run(() =>
            {
                var result = Get(url);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static async Task<Response> GetAsync(string url, CookieContainer cookies)
        {
            var task = new TaskCompletionSource<Response>();
            await Task.Run(() =>
            {
                var result = Get(url, cookies);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        public static Response Post(string url, List<Parameter> parameters,
            List<HttpHeader> headers, CookieContainer cookies, string userAgent)
        {
            var client = new RestClient(url)
            {
                UserAgent = userAgent == "" ? null : userAgent,
                FollowRedirects = true,
                CookieContainer = cookies
            };

            var request = new RestRequest(string.Empty, Method.POST);

            foreach (var header in headers)
            {
                request.AddHeader(header.Name, header.Value);
            }

            foreach (var param in parameters)
            {
                request.AddParameter(param);
            }

            var response = client.Execute(request);

            Thread.Sleep(requestInterval);

            return new Response(client.CookieContainer, response);
        }

        public static Response Post(string url, List<Parameter> parameters,
            List<HttpHeader> headers, CookieContainer cookies)
        {
            var client = new RestClient(url)
            {
                FollowRedirects = true,
                CookieContainer = cookies
            };

            var request = new RestRequest(string.Empty, Method.POST);

            foreach (var header in headers)
            {
                request.AddHeader(header.Name, header.Value);
            }

            foreach (var param in parameters)
            {
                request.AddParameter(param);
            }

            var response = client.Execute(request);
            Thread.Sleep(requestInterval);

            return new Response(client.CookieContainer, response);
        }

        public static Response Post(string url, List<Parameter> parameters, CookieContainer cookies)
        {
            var client = new RestClient(url)
            {
                FollowRedirects = true,
                CookieContainer = cookies
            };

            var request = new RestRequest(string.Empty, Method.POST);

            foreach (var param in parameters)
            {
                request.AddParameter(param);
            }

            var response = client.Execute(request);

            Thread.Sleep(requestInterval);

            return new Response(client.CookieContainer, response);
        }

        public static Response Post(string url, List<Parameter> parameters,
            CookieContainer cookies, string userAgent)
        {
            var client = new RestClient(url)
            {
                UserAgent = userAgent,
                FollowRedirects = true,
                CookieContainer = cookies
            };

            var request = new RestRequest(string.Empty, Method.POST);

            foreach (var param in parameters)
            {
                request.AddParameter(param);
            }

            var response = client.Execute(request);

            Thread.Sleep(requestInterval);

            return new Response(client.CookieContainer, response);
        }

        public static async Task<Response> PostAsync(string url, List<Parameter> parameters, CookieContainer cookies)
        {
            var task = new TaskCompletionSource<Response>();
            await Task.Run(() =>
            {
                var result = Post(url, parameters, cookies);
                task.SetResult(result);
            });

            return task.Task.Result;
        }

        private static string GetVersionInGitHub(string url)
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

        public class Response
        {
            public Response(CookieContainer cookies, IRestResponse response)
            {
                Cookies = cookies;
                RestResponse = response;
            }

            public CookieContainer Cookies { get; }
            public IRestResponse RestResponse { get; }
        }
    }
}