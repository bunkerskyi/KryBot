using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace KryBot.Core.Serializable.GameMiner
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public class JsonResponseErrorDetail
	{
        [JsonProperty(PropertyName = "message", Required = Required.Always)]
        public string Message { get; set; }
	}
}