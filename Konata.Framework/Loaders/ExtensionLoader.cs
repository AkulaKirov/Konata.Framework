using Konata.Framework.Extensions;
using Konata.Framework.Sdk.Extensions;

namespace Konata.Framework.Loaders
{
    public class ExtensionLoader
    {
        public HostedExtension? LoadExtension(string pathToDir, ExtensionInfo extensionInfo)
        {
            var ealc = new ExtensionAssemblyLoadContext(extensionInfo.PackageName, Path.Combine(pathToDir, extensionInfo.EntryPoint));
            var assembly = ealc.PrimaryAssembly;
            if (assembly == null) return null;

            var type = assembly.GetTypes().First(x => x.GetInterface("IExtension") != null);
            if (type == null) return null;

            var extension = (IExtension)Activator.CreateInstance(type);
            if (extension == null) return null;
            return new HostedExtension(extension, ealc, extensionInfo);
        }
    }
}
