using Konata.Core.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konata.Framework.Sdk.Events.Messages
{
    public class MessageEvent : KonataEvent
    {
        public uint SubjectUin { get; set; }
        public uint SenderUin { get; set; }
        public MessageStruct Message { get; set; }
        public MessageChain Chain => Message.Chain;
    }
}
