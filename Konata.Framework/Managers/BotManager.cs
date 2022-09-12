using Konata.Core;
using Konata.Core.Common;
using Konata.Core.Interfaces;
using Konata.Core.Interfaces.Api;
using Konata.Framework.Sdk.Events.Messages;
using Newtonsoft.Json;

namespace Konata.Framework.Managers
{
    public class BotManager
    {
        // TODO: 对 Andreal 进行一个抄
        private static BotManager instance;

        public static DirectoryInfo botConfigDir = new(GlobalRutine.BOT_CONFIG_DIR);
        public int BotCount => BotTable.Count;
        public Dictionary<uint, Bot> BotTable { get; } = new();

        public static BotManager Instance
        {
            get
            {
                return instance ??= new();
            }
        }

        private BotManager()
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

        private static void UpdateKeystore(uint qqid, BotKeyStore keystore)
        {
            var pth = Path.BotConfig(qqid);

            ConfigJson cfg = new() { KeyStore = keystore, Device = BotInfos[qqid].Device };

            File.WriteAllText(pth, JsonConvert.SerializeObject(cfg));
        }

        private async void Bot_OnFriendMessage(Bot sender, Core.Events.Model.FriendMessageEvent args)
        {
            var friend = sender.GetFriendList().Result.First(x => x.Uin == args.FriendUin);
            FriendMessageEvent friendMessageEvent = new()
            {
                Time = args.EventTime,
                Bot = sender,
                SubjectUin = args.FriendUin,
                SenderUin = args.FriendUin,
                Message = args.Message,
                Friend = friend,
            };
            await GlobalRutine.EventHandlerManager.DispatchEvent(friendMessageEvent);
        }

        private async void Bot_OnGroupMessage(Bot sender, Core.Events.Model.GroupMessageEvent args)
        {
            var group = sender.GetGroupList().Result.First(x => x.Uin == args.GroupUin);
            var member = sender.GetGroupMemberInfo(args.GroupUin, args.MemberUin).Result;
            GroupMessageEvent groupMessageEvent = new()
            {
                Time = args.EventTime,
                Bot = sender,
                SubjectUin = args.GroupUin,
                SenderUin = args.MemberUin,
                Message = args.Message,
                Group = group,
                Member = member,
            };
            await GlobalRutine.EventHandlerManager.DispatchEvent(groupMessageEvent);
        }
    }
}
