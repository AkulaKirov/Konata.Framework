namespace Konata.Framework.AdaptiveEvent
{
    public abstract class BaseEventPackage
    {
        private bool _intercepted = false;
        public bool IsIntercepted => _intercepted;

        public void Intercept()
            => _intercepted = true;
    }
}
