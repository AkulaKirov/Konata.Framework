using Microsoft.Extensions.DependencyModel;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace Konata.Framework.Loaders
{
    public class NativeReferenceResolver
    {
        private const string runtimeRIDUrl = @"https://raw.githubusercontent.com/dotnet/runtime/main/src/libraries/Microsoft.NETCore.Platforms/src/runtime.json";
        private const string runtimeRIDPath = "runtime.json";
        private static Dictionary<string, string[]> runtimeRIDs = new();

        protected DependencyContext dependency;
        protected string dirPath;

        private static NativeReferenceResolver instance = new();
        private NativeReferenceResolver()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string runtimeRIDJson;
                try
                {
                    var result = httpClient.GetAsync(runtimeRIDUrl).Result;
                    if (result?.IsSuccessStatusCode == true)
                    {
                        runtimeRIDJson = result.Content.ReadAsStringAsync().Result;
                        File.WriteAllTextAsync(runtimeRIDPath, runtimeRIDJson);
                    }
                    else
                    {
                        runtimeRIDJson = File.ReadAllText(runtimeRIDPath);
                    }
                }
                catch
                {
                    runtimeRIDJson = File.ReadAllText(runtimeRIDPath);
                }
                using (JsonDocument document = JsonDocument.Parse(runtimeRIDJson))
                {
                    JsonElement runtimes = document.RootElement.GetProperty("runtimes");
                    foreach (var runtime in runtimes.EnumerateObject())
                    {
                        string name = runtime.Name;
                        string[] fallback = runtime.Value.GetProperty("#import").EnumerateArray().Select(x => x.GetString()!).ToArray();
                        runtimeRIDs.Add(name, fallback);
                    }
                }
            }
        }

        public static NativeReferenceResolver Create(string path)
        {
            var clone = (NativeReferenceResolver)instance.MemberwiseClone();
            using (var depsReader = new DependencyContextJsonReader())
            {
                clone.dependency = depsReader.Read(File.OpenRead(path));
                clone.dirPath = Path.GetDirectoryName(path);
            }
            return clone;
        }

        public string ResolveManagedDllPath(AssemblyName assemblyName)
        {
            foreach (var file in Directory.EnumerateFiles(dirPath))
            {
                if (Path.GetFileNameWithoutExtension(file) == assemblyName.Name)
                {
                    return file;
                }
            }
            return "";
        }

        public string ResolveUnmanagedDllPath(string unmanagedDllName)
        {
            Queue<string> rids = new();
            rids.Enqueue(RuntimeInformation.RuntimeIdentifier);
            while (rids.Count > 0)
            {
                string rid = rids.Dequeue();
                foreach (var fallback in GetRIDFallback(rid))
                {
                    rids.Enqueue(fallback);
                }

                var nativeLibs = dependency.GetRuntimeNativeAssets(rid);
                var nativeLibPath = nativeLibs.FirstOrDefault(x => unmanagedDllName == Path.GetFileNameWithoutExtension(x), "");
                if (nativeLibPath != "")
                {
                    return nativeLibPath;
                }
            }
            return "";
        }

        public string[]? GetRIDFallback(string rid)
        {
            if (runtimeRIDs.TryGetValue(rid, out var fallback))
            {
                return fallback;
            }
            else
            {
                return null;
            }
        }
    }
}
