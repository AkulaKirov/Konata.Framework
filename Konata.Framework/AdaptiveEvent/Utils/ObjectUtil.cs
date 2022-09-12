using System.Diagnostics;
using System.Linq.Expressions;

namespace Konata.Framework.AdaptiveEvent.Utils
{
    public static class ObjectUtil
    {
        /// <summary>
        /// ����������н���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        public static void Swap<T>(ref T t1, ref T t2)
        {
            T t3 = t1;
            t1 = t2;
            t2 = t3;
        }

        /// <summary>
        /// ��ȡ����������ָ����ȷ�����
        /// </summary>
        /// <param name="index">
        /// ��Ȳ㼶
        /// <para>��ȶ��壺</para>
        /// <para>0:�ú�������,1:���øú����ĵ��÷�,2:���÷��ĵ��÷�...</para>
        /// </param>
        /// <param name="onlymethodname">�Ƿ�����ط�������</param>
        /// <returns></returns>
        public static string GetCurrentMethodTraceName(int index = 2, bool onlymethodname = true)
        {
            if (index < 0)
            {
                return null;
            }
            try
            {
                StackTrace trace = new StackTrace();
                string methodName = trace.GetFrame(index).GetMethod().Name;
                if (!onlymethodname)
                {
                    string className = trace.GetFrame(index).GetMethod().DeclaringType.ToString();
                    methodName = className + "." + methodName;
                }
                return methodName;
            }
            catch
            {
                return null;
            }
        }
    }

    /// <summary>
    /// �����������
    /// [���ڱ��ʽ��]
    /// </summary>
    /// <typeparam name="TIn">�������</typeparam>
    /// <typeparam name="TOut">Ŀ�����</typeparam>
    public static class TransExp<TIn, TOut>
    {
        private static readonly Func<TIn, TOut> cache = GetFunc();
        private static Func<TIn, TOut> GetFunc()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();

            foreach (var item in typeof(TOut).GetProperties())
            {
                if (!item.CanWrite) continue;
                MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
                MemberBinding memberBinding = Expression.Bind(item, property);
                memberBindingList.Add(memberBinding);
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

            return lambda.Compile();
        }

        public static TOut Trans(TIn tIn)
        {
            return cache(tIn);
        }
    }
}