using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konata.Framework.Sdk.Events.Messages
{
    public enum SenderType
    {
        Private,
        Group,
        Temp
    }
    public class MessageEvent : KonataEvent
    {
        public uint SubjectUin { get; set; }
        public uint SenderUin { get; set; }
        public string Message { get; set; }
    }
}
