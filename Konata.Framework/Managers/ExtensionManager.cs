using Konata.Framework.Extensions;
using Konata.Framework.Loaders;

namespace Konata.Framework.Managers
{
    public class ExtensionManager
    {
        private static ExtensionManager instance;

        public static DirectoryInfo extensionDir = new(GlobalRutine.EXTENSION_DIR);

        public int ExtensionCount => HostedExtensionTable.Count;

        public Dictionary<string, HostedExtension> HostedExtensionTable { get; } = new();

        public static ExtensionManager Instance
        {
            get
            {
                return instance ??= new();
            }
        }
        private ExtensionManager()
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
                Console.WriteLine($"Loading extension: {extension.Value.Name}({extension.Key})");
                if (extension.Value.Load() && extension.Value.Enable())
                {
                    GlobalRutine.EventHandlerManager.RegisterExtensionEventHandlers(extension.Value);
                }
                else
                {
                    Console.WriteLine($"Failed to load extension: {extension.Value.Name}({extension.Key})");
                }
            }
        }
        public bool RegisterExtension(HostedExtension hostedExtension)
            => HostedExtensionTable.TryAdd(hostedExtension.PackageName, hostedExtension);
    }
}
