/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
*/
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace KryBot.Core.Serializable.SteamPortal
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class JsonJoin
    {
        [JsonProperty(PropertyName = "error", Required = Required.Always)]
        public int Error { get; set; }

        [JsonProperty(PropertyName = "target_h", Required = Required.Always)]
        public TargetH TargetH { get; set; }
    }
}