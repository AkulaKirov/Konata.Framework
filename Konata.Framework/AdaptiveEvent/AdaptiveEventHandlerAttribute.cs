namespace Konata.Framework.AdaptiveEvent
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false)]
    public class AdaptiveEventHandlerAttribute:Attribute
    {
        /// <summary>
        /// ����ʹ��BaseEventPackage�����ķ�������
        /// ��ϲ��д�������ټӸ�Attrô,�÷���
        /// ����ָʾ,�Զ�ʶ��[] ��Ҫ����������ͼ���Ҫ��attr��,���ʶ����
        /// </summary>
        public int SupportSameEventPackageFunction { get; set; } = 1;

        public AdaptiveEventHandlerAttribute(int supportSameEventPackageFunction=1)
        {
            SupportSameEventPackageFunction = supportSameEventPackageFunction;
        }
    }
}
