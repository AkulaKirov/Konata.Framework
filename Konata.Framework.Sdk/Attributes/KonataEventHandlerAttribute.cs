namespace Konata.Framework.AdaptiveEvent
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple =false)]
    public class KonataEventHandlerAttribute:Attribute
    {
        /// <summary>
        /// ����ʹ��BaseEvent�����ķ�������
        /// </summary>
        //public int SupportSameEventPackageFunction { get; set; } = 1;

        //public AdaptiveEventHandlerAttribute(int supportSameEventPackageFunction=1)
        //{
        //    SupportSameEventPackageFunction = supportSameEventPackageFunction;
        //}

        public KonataEventHandlerAttribute() { } 
    }
}
