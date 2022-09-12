using System.Diagnostics;
using System.Linq.Expressions;

namespace Konata.Framework.AdaptiveEvent.Utils
{
    public static class ObjectUtil
    {
        /// <summary>
        /// 两个对象进行交换
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
        /// 获取方法调用链指定深度方法名
        /// </summary>
        /// <param name="index">
        /// 深度层级
        /// <para>深度定义：</para>
        /// <para>0:该函数本身,1:调用该函数的调用方,2:调用方的调用方...</para>
        /// </param>
        /// <param name="onlymethodname">是否仅返回方法名称</param>
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
    /// 对象属性深拷贝
    /// [基于表达式树]
    /// </summary>
    /// <typeparam name="TIn">复制类别</typeparam>
    /// <typeparam name="TOut">目标类别</typeparam>
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