/*
* This is a personal academic project. Dear PVS-Studio, please check it.
* PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
*/
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace KryBot.Core.Serializable.GameMiner
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class JsonGiveaway
    {
        [JsonProperty(PropertyName = "golden", Required = Required.Always)]
        public bool Golden { get; set; }

        [JsonProperty(PropertyName = "code", Required = Required.Always)]
        public string Code { get; set; }

        [JsonProperty(PropertyName = "price", Required = Required.Always)]
        public int Price { get; set; }

        [JsonProperty(PropertyName = "sandbox")]
        public object Sandbox { get; set; }

        [JsonProperty(PropertyName = "game", Required = Required.Always)]
        public JsonGame Game { get; set; }

        [JsonProperty(PropertyName = "regionlock_type_id")]
        public string RegionlockTypeId { get; set; }
    }
}