using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Konata.Framework.AdaptiveEvent.Extensions;

namespace Konata.Framework.AdaptiveEvent.Utils
{
    public static class GenerateDelegateUtil
    {

        public static MethodInfo DeegateTypeInvokeMethod(Type delegateType)
        {
            if (!delegateType.IsSubclassOf(typeof(MulticastDelegate)))
            {
                throw new InvalidOperationException($"Return type must a delegate,can't {delegateType.Name}");
            }

            return delegateType.GetMethod("Invoke");
        }

        public static T? GenerateDelegate<T>(object instance, MethodInfo method,bool auto_generate_new_instance=false)
        {
            if (instance != null && method.DeclaringType != instance.GetType())
                throw new InvalidOperationException($"{method.Name} not in {instance.GetType()}!");

            MethodInfo delegateInfo = DeegateTypeInvokeMethod(typeof(T));

            var methodTypes = method.GetParameters().Select(m => m.ParameterType);
            var delegateTypes = delegateInfo.GetParameters().Select(d => d.ParameterType);
            var delegateArguments = delegateTypes.Select(Expression.Parameter).ToArray();


            var convertedArguments = methodTypes.Zip(
                delegateTypes, delegateArguments,
                (methodType, delegateType, delegateArgument) =>
                    methodType != delegateType
                        ? (Expression)Expression.Convert(delegateArgument, methodType)
                        : delegateArgument);

            // Create call.
            MethodCallExpression methodCall = Expression.Call(
                instance == null ? (method.IsStatic&&!auto_generate_new_instance ? null: Expression.New(method.DeclaringType)) : Expression.Constant(instance),
                method,
                convertedArguments
                );


            try
            {
                //void can't compare delgate type,make error
                Expression convertedMethodCall = delegateInfo.ReturnType == method.ReturnType
                                            ? (Expression)methodCall
                                            : Expression.Convert(methodCall, delegateInfo.ReturnType);
                //wow
                return Expression.Lambda<T>(
                    convertedMethodCall,
                    delegateArguments
                    ).Compile();
            }
            catch(Exception e)
            {
                return default;
            }


        }
    }
}
