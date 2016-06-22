using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace KryBot.Core.Serializable.UseGamble
{
	[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
	public class TargetH
	{
        [JsonProperty(PropertyName = "my_coins", Required = Required.Always)]
        public int MyCoins { get; set; }
	}
}