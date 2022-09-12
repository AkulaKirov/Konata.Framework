namespace Konata.Framework.AdaptiveEvent
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false)]
    public class AdaptiveEventHandlerAttribute:Attribute
    {
        /// <summary>
        /// 类中使用BaseEventPackage参数的方法数量
        /// 你喜欢写个方法再加个Attr么,好烦的
        /// 不用指示,自动识别[] 你要有其他需求就加你要的attr呗,这个识别不用
        /// </summary>
        public int SupportSameEventPackageFunction { get; set; } = 1;

        public AdaptiveEventHandlerAttribute(int supportSameEventPackageFunction=1)
        {
            SupportSameEventPackageFunction = supportSameEventPackageFunction;
        }
    }
}
