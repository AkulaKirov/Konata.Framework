using System.Reflection;
using System.Runtime.Loader;

namespace Konata.Framework.Loaders
{
    public class ExtensionAssemblyLoadContext : AssemblyLoadContext
    {
        public readonly Assembly PrimaryAssembly;
        public readonly DirectoryInfo ExtensionDir;
        public readonly NativeReferenceResolver Resolver;

        public ExtensionAssemblyLoadContext(string id, string entryPointPath) : base(id, true)
        {
            string absolutePath = Path.GetFullPath(entryPointPath);
            ExtensionDir = new DirectoryInfo(Path.GetDirectoryName(absolutePath));
            PrimaryAssembly = LoadFromAssemblyPath(absolutePath);
            Resolver = NativeReferenceResolver.Create(Path.Combine(ExtensionDir.FullName, $"{PrimaryAssembly.GetName().Name}.deps.json"));
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            string path = Resolver.ResolveManagedDllPath(assemblyName);
            if (path != "") return LoadFromAssemblyPath(path);
            else return base.Load(assemblyName);
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            string path = Resolver.ResolveUnmanagedDllPath(unmanagedDllName);
            if (path != "") return LoadUnmanagedDllFromPath(Path.Combine(ExtensionDir.FullName, path));
            else return base.LoadUnmanagedDll(unmanagedDllName);
        }
    }
}
