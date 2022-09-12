using Konata.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konata.Framework.Sdk.Events
{
    public abstract class KonataEvent
    {
        public DateTime Time { get; set; }
        public Bot Bot { get; set; }
    }
}
