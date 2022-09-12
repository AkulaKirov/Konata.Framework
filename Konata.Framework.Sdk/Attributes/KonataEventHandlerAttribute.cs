namespace Konata.Framework.AdaptiveEvent
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple =false)]
    public class KonataEventHandlerAttribute:Attribute
    {
        /// <summary>
        /// 类中使用BaseEvent参数的方法数量
        /// </summary>
        //public int SupportSameEventPackageFunction { get; set; } = 1;

        //public AdaptiveEventHandlerAttribute(int supportSameEventPackageFunction=1)
        //{
        //    SupportSameEventPackageFunction = supportSameEventPackageFunction;
        //}

        public KonataEventHandlerAttribute() { } 
    }
}
