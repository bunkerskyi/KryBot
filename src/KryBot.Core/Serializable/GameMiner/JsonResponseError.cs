using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace KryBot.Core.Serializable.GameMiner
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class JsonResponseError
    {
        [JsonProperty(PropertyName = "error", Required = Required.Always)]
        public JsonResponseErrorDetail Error { get; set; }
    }
}