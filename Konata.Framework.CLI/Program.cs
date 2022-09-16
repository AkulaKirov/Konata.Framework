using Konata.Framework.Extensions;
using Konata.Framework.Managers;
using Newtonsoft.Json;

namespace Konata.Framework.CLI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GlobalRutine.Startup();
            GlobalRutine.BotManager.Login();
        }
    }
}