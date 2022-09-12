using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konata.Framework.Sdk.Events
{
    public abstract class BaseEvent
    {
        private bool _intercepted = false;
        public bool IsIntercepted => _intercepted;
        public void Intercept()
            => _intercepted = true;
    }
}
