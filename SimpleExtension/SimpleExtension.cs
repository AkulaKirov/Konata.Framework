using Konata.Framework.Sdk.Extensions;

namespace SimpleExtension
{
    internal class SimpleExtension : IExtension
    {
        public string PackageName => "Konata.Example.SimpleExtension";

        public void OnDisable()
        {
            Console.WriteLine("SimpleExtension Disabled.");
        }

        public bool OnEnable()
        {
            Console.WriteLine("SimpleExtension Enabled.");
            return true;
        }

        public bool OnLoad()
        {
            Console.WriteLine("SimpleExtension Loaded.");
            return true;
        }
    }
}
