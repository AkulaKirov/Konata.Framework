using Konata.Framework.AdaptiveEvent;
using Konata.Framework.Sdk.Events.Messages;

namespace SimpleExtension
{
    [KonataEventHandler]
    internal class EventListener
    {
        public async Task OnMessageEvent(MessageEvent messageEvent)
        {
            messageEvent.Intercept();
            Console.WriteLine(messageEvent.Chain.ToString());
        }
    }
}
