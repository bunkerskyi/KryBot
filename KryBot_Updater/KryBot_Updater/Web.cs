using System;
using RestSharp;
using RestSharp.Extensions;

namespace KryBot_Updater
{
    internal class Web
    {
        public static string Get(string url)
        {
            var client = new RestClient(url)
            {
                FollowRedirects = true,
            };

            var request = new RestRequest("", Method.GET);

            var response = client.Execute(request);

            return response.Content;
        }
    }
}