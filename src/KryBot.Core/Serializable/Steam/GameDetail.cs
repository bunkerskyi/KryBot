using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace KryBot.Core.Serializable.Steam
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
	public class GameDetail
	{
        [JsonProperty(PropertyName = "success", Required = Required.Always)]
        public bool Success { get; set; }
        [JsonProperty(PropertyName = "data", Required = Required.Always)]
        public GameDetailData Data { get; set; }
	}
}