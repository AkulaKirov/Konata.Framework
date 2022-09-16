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
        private static BotManager instance;
        private static Bot bot;

        public static DirectoryInfo botConfigDir = new(GlobalRutine.BOT_CONFIG_DIR);
        public static readonly BotConfig DefaultBotConfig = new()
        {
            TryReconnect = true,
            HighwayChunkSize = 8192,
            DefaultTimeout = 5000,
            Protocol = OicqProtocol.Android,
        };

        public static BotManager Instance
        {
            get
            {
                return instance ??= new();
            }
        }

        private BotManager()
        {
            
        }
        public void Init()
        {
            if (!botConfigDir.Exists)
                botConfigDir.Create();
            var configFile = botConfigDir.GetFiles("config.json").FirstOrDefault();
            var keyStoreFile = botConfigDir.GetFiles("keystore.json").FirstOrDefault();
            var deviceFile = botConfigDir.GetFiles("device.json").FirstOrDefault();
            if (keyStoreFile == null) return;
            else
            {
                var botConfig = configFile == null ? DefaultBotConfig : JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(configFile.FullName));
                var botKeyStore = keyStoreFile == null ? throw new Exception() : JsonConvert.DeserializeObject<BotKeyStore>(File.ReadAllText(keyStoreFile.FullName));
                var botDevice = deviceFile == null ? new BotDevice() : JsonConvert.DeserializeObject<BotDevice>(File.ReadAllText(deviceFile.FullName));
                AddBot(botKeyStore, botDevice, botConfig);
            }
        }
        public void AddBot(string uin, string password)
        {
            BotKeyStore botKeyStore = new BotKeyStore(uin, password);
            AddBot(botKeyStore, null, null);
        }
        public void AddBot(BotKeyStore botKeyStore, BotDevice? botDevice, BotConfig? botConfig = null)
        {
            if (botKeyStore == null)
                throw new ArgumentNullException(nameof(botKeyStore));
            if (botDevice == null)
                botDevice = new BotDevice();
            if (botConfig == null)
                botConfig = DefaultBotConfig;
            bot = BotFather.Create(botConfig, botDevice, botKeyStore);
        }
        public void RemoveBot()
        {
            bot.Logout();
            bot.Dispose();
        }
        public async Task<bool> Login()
        {
            if (bot == null) return false;
            bot.OnFriendMessage += Bot_OnFriendMessage;
            bot.OnGroupMessage += Bot_OnGroupMessage;

            return await bot.Login();
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