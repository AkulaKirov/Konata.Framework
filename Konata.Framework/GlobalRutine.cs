using Konata.Framework.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konata.Framework
{
    public static class GlobalRutine
    {
        public static EventManager EventManager { get; private set; } = new();
        public static ExtensionManager ExtensionManager { get; private set; } = new();
        public static BotManager BotManager { get; private set; } = new();

        public static void StartUp()
        {
            EventManager.Init();
            ExtensionManager.Init();
            BotManager.Init();
        }
    }
}
