using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Konata.Framework.Extensions
{
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class ExtensionInfo
    {
        public string Name { get; set; }
        public string PackageName { get; set; }
        public Version Version { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string EntryPoint { get; set; }
    }
}
