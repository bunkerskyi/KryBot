using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace KryBot.Core.Serializable.GameAways
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	public class JsonJoinResponse
	{
        [JsonProperty(PropertyName = "balance", Required = Required.Always)]
        public int Balance { get; set; }
	}
}