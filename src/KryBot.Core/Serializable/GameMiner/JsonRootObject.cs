using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace KryBot.Core.Serializable.GameMiner
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
	public class JsonRootObject
	{
        [JsonProperty(PropertyName = "giveaways", Required = Required.Always)]
        public List<JsonGiveaway> Giveaways { get; set; }
        [JsonProperty(PropertyName = "last_page", Required = Required.Always)]
        public int LastPage { get; set; }
        [JsonProperty(PropertyName = "total", Required = Required.Always)]
        public int Total { get; set; }
        [JsonProperty(PropertyName = "page", Required = Required.Always)]
        public int Page { get; set; }
	}
}