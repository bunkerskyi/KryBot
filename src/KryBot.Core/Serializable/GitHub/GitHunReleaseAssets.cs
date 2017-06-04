/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
*/
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace KryBot.Core.Serializable.GitHub
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class GitHunReleaseAssets
    {
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        internal string Name { get; set; }

        [JsonProperty(PropertyName = "browser_download_url", Required = Required.Always)]
        internal string DownloadUrl { get; set; }
    }
}