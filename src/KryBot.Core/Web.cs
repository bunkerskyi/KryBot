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

		public static Classes.Response Get(string url, List<Parameter> parameters,
			CookieContainer cookies, List<HttpHeader> headers, string userAgent)
		{
			var client = new RestClient(url)
			{
				UserAgent = userAgent == "" ? null : userAgent,
				FollowRedirects = true,
				CookieContainer = cookies
			};

			var request = new RestRequest(string.Empty, Method.GET);

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

		public static async Task<Classes.Response> GetAsync(string url, List<Parameter> parameters,
			CookieContainer cookies, List<HttpHeader> headers, string userAgent)
		{
			var task = new TaskCompletionSource<Classes.Response>();
			await Task.Run(() =>
			{
				var result = Get(url, parameters, cookies, headers, userAgent);
				task.SetResult(result);
			});

			return task.Task.Result;
		}

		public static Classes.Response Post(string url, List<Parameter> parameters,
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
			var data = new Classes.Response
			{
				Cookies = client.CookieContainer,
				RestResponse = response
			};

			Thread.Sleep(requestInterval);

			return data;
		}

		public static Classes.Response Post(string url, List<Parameter> parameters,
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
			var data = new Classes.Response
			{
				Cookies = client.CookieContainer,
				RestResponse = response
			};

			Thread.Sleep(requestInterval);

			return data;
		}

		public static Classes.Response SteamTradeDoAuth(string url, List<Parameter> parameters,
			CookieContainer cookies, List<HttpHeader> headers)
		{
			var client = new RestClient(url)
			{
				FollowRedirects = false,
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
			var data = new Classes.Response
			{
				Cookies = client.CookieContainer,
				RestResponse = response
			};

			Thread.Sleep(requestInterval);

			return data;
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
	}
}