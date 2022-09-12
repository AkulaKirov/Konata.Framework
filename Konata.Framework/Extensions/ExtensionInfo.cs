using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
