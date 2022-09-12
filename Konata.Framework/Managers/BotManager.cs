using Konata.Core;
using Konata.Core.Common;
using Konata.Core.Interfaces;
using Konata.Core.Interfaces.Api;
using Konata.Framework.Sdk.Events.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Konata.Framework.Managers
{
    public class BotManager
    {
        public const string BOT_CONFIG_DIR = "bots";
        public static DirectoryInfo botConfigDir = new(BOT_CONFIG_DIR);
        public int BotCount => BotTable.Count;
        public Dictionary<uint, Bot> BotTable { get; } = new();

        public BotManager()
        {
            if (!botConfigDir.Exists)
                botConfigDir.Create();
        }

        public void Init()
        {
            foreach (var dir in botConfigDir.EnumerateDirectories())
            {
                BotKeyStore keyStore;
                BotDevice device;
                BotConfig config;

                var keyStoreFile = Path.Combine(dir.FullName, "keystore.json");
                var deviceFile = Path.Combine(dir.FullName, "device.json");
                var configFile = Path.Combine(dir.FullName, "config.json");

                if (!File.Exists(keyStoreFile) || !File.Exists(deviceFile))
                    continue;

                keyStore = JsonConvert.DeserializeObject<BotKeyStore>(File.ReadAllText(keyStoreFile));
                device = JsonConvert.DeserializeObject<BotDevice>(File.ReadAllText(deviceFile));
                if (File.Exists(configFile))
                    config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(keyStoreFile));
                else
                    config = BotConfig.Default();

                Bot bot = BotFather.Create(config, device, keyStore);
                BotTable.TryAdd(keyStore.Account.Uin, bot);
            }
        }

        public void Login()
        {
            foreach (var bot in BotTable)
                Login(bot.Value);
        }

        public void Login(Bot bot)
        {
            if (!bot.Login().Result) return;
            Console.WriteLine($"{bot.Uin} Online");
            bot.OnGroupMessage += Bot_OnGroupMessage;
        }

        private void Bot_OnGroupMessage(Bot sender, Core.Events.Model.GroupMessageEvent args)
        {
            GlobalRutine.EventManager.DispatchEvent(new MessageEvent()
            {
                Time = args.EventTime,
                Bot = sender,
                SubjectUin = args.GroupUin,
                SenderUin = args.MemberUin,
                Message = args.Message.Chain.ToString()
            });
        }
    }
}
