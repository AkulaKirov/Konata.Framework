using Newtonsoft.Json;

namespace Konata.Framework.Extensions
{
    internal static class ExtensionInfoReader
    {
        public static ExtensionInfo? Read(string json)
        {
            return JsonConvert.DeserializeObject<ExtensionInfo>(json);
        }

        public static ExtensionInfo? ReadFromDirectory(string path)
        {
            var filePath = Path.Combine(path, "package.json");
            if (!File.Exists(filePath)) return null;
            return Read(File.ReadAllText(filePath));
        }

        public static ExtensionInfo? ReadFromPackage(Stream zipFileStream)
        {
            throw new NotImplementedException();
        }
    }
}
