using Konata.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konata.Framework.Sdk.Events.Messages
{
    public class FriendMessageEvent : MessageEvent
    {
        public BotFriend Friend { get; set; }
    }
}
