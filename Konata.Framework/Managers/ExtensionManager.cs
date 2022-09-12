using Konata.Framework.Extensions;
using Konata.Framework.Loaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konata.Framework.Managers
{
    public class ExtensionManager
    {
        public const string EXTENSION_DIR = "extensions";
        public static DirectoryInfo extensionDir = new(EXTENSION_DIR);

        public int ExtensionCount => HostedExtensionTable.Count;

        public Dictionary<string, HostedExtension> HostedExtensionTable { get; } = new();

        public ExtensionManager() 
        {
            if (!extensionDir.Exists)
                extensionDir.Create();
        }

        public void Init()
        {
            DiscoverExtensions();
            LoadExtensions();
        }

        public void DiscoverExtensions()
        {
            foreach (var dir in extensionDir.EnumerateDirectories())
            {
                if (dir.EnumerateFiles().Any(x => x.Name == "package.json"))
                {
                    var extensionInfo = ExtensionInfoReader.ReadFromDirectory(dir.FullName);
                    var loader = new ExtensionLoader();
                    var hostedExtension = loader.LoadExtension(dir.FullName, extensionInfo);
                    if (hostedExtension != null)
                        RegisterExtension(hostedExtension);
                }
            }
        }
        public void LoadExtensions()
        {
            foreach (var extension in HostedExtensionTable)
            {
                extension.Value.Load();
                extension.Value.Enable();
                GlobalRutine.EventManager.RegisterEventListener(extension.Value.ExecuteAsync);
            }
        }
        public bool RegisterExtension(HostedExtension hostedExtension)
            => HostedExtensionTable.TryAdd(hostedExtension.PackageName, hostedExtension);
    }
}
