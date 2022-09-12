using Konata.Framework.Managers;

namespace Konata.Framework
{
    public static class GlobalRutine
    {
        // TODO: 把 Logger 的实现切到 Microsoft.Extension.Logging
        public const string EXTENSION_DIR = "extensions";
        public const string BOT_CONFIG_DIR = "bots";
        public const string CONFIG_DIR = "configs";
        public static EventHandlerManager EventHandlerManager
            => EventHandlerManager.Instance;
        public static ExtensionManager ExtensionManager
            => ExtensionManager.Instance;
        public static BotManager BotManager
            => BotManager.Instance;


        public static void Startup()
        {
            Console.WriteLine("Initializing BotManager.");
            BotManager.Init();
            Console.WriteLine("Initializing ExtensionManager.");
            ExtensionManager.Init();
            Console.WriteLine("Startup completed.");
        }
    }
}
