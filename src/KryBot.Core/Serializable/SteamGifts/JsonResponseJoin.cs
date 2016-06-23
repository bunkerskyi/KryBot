using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace KryBot.Core.Serializable.SteamGifts
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class JsonResponseJoin
    {
        [JsonProperty(PropertyName = "type", Required = Required.Always)]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "points", Required = Required.Always)]
        public int Points { get; set; }
    }
}