using Konata.Framework.Loaders;
using Konata.Framework.Sdk.Extensions;
using System.Reflection;

namespace Konata.Framework.Extensions
{
    /// <summary>
    /// 被识别并加载的拓展
    /// </summary>
    public sealed class HostedExtension
    {
        private bool isEnabled;
        private ExtensionInfo extensionInfo;
        private IExtension extension;
        private ExtensionAssemblyLoadContext context;


        public string Name => extensionInfo.Name;
        public string PackageName => extensionInfo.PackageName;
        public ExtensionInfo ExtensionInfo => extensionInfo;
        public bool IsEnabled => isEnabled;
        public IExtension Extension => extension;
        public Assembly PrimaryAssembly => context.PrimaryAssembly;

        internal HostedExtension(IExtension extension, ExtensionAssemblyLoadContext context, ExtensionInfo extensionInfo)
        {
            this.extension = extension;
            this.context = context;
            this.extensionInfo = extensionInfo;
        }

        public bool Load()
            => extension.OnLoad();
        public bool Enable()
        {
            isEnabled = extension.OnEnable();
            return isEnabled;
        }
        public void Disable()
        {
            extension.OnDisable();
            isEnabled = false;
        }
    }
}
