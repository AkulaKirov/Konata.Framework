using Konata.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konata.Framework.Sdk.Events.Messages
{
    public class GroupMessageEvent : MessageEvent
    {
        public BotGroup Group { get; set; }
        public BotMember Member { get; set; }
    }
}
