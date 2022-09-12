using Konata.Framework.AdaptiveEvent;
using Konata.Framework.AdaptiveEvent.Utils;
using Konata.Framework.Extensions;
using Konata.Framework.Sdk.Events;
using System.Reflection;

namespace Konata.Framework.Managers
{
    public class EventHandlerManager
    {
        private static EventHandlerManager instance;

        private Dictionary<Type, object> eventHandlerCache = new Dictionary<Type, object>();
        private Dictionary<Type, SortedList<int, KeyValuePair<string, Func<BaseEvent, Task>>>> eventHandlerDeleagte = new Dictionary<Type, SortedList<int, KeyValuePair<string, Func<BaseEvent, Task>>>>();


        private EventHandlerManager() { }

        public static EventHandlerManager Instance
        {
            get
            {
                return instance ??= new();
            }
        }

        public void RegisterExtensionEventHandlers(HostedExtension hostedExtension)
        {
            var method_list = new List<MethodInfo>();
            var assembly = hostedExtension.PrimaryAssembly;
            foreach (var type in assembly.GetTypes())
            {
                // 判断构造函数是否含参
                if (type.GetConstructor(Type.EmptyTypes) == null)
                    continue;

                // 判断是否拥有特性
                var _attribute = type.GetCustomAttribute<KonataEventHandlerAttribute>();
                if (_attribute is null)
                    continue;

                method_list.Clear();
                foreach (var method in type.GetMethods())
                {
                    var _params = method.GetParameters();
                    if (_params.Length != 1)
                        continue;
                    if (!_params[0].ParameterType.IsSubclassOf(typeof(BaseEvent)))
                        continue;
                    method_list.Add(method);
                }

                //if (_attribute.SupportSameEventPackageFunction != method_list.Count)
                //    continue;

                if (method_list.Count > 0)
                {
                    var instance = Activator.CreateInstance(type);
                    eventHandlerCache.Add(type, instance);
                    foreach (var method in method_list)
                    {
                        var packagetype = method.GetParameters()[0].ParameterType;
                        var method_delegate = GenerateDelegateUtil.GenerateDelegate<Func<BaseEvent, Task>>(instance, method);
                        if (method_delegate != null)
                        {

                            if (!eventHandlerDeleagte.TryGetValue(packagetype, out var _list))
                            {
                                _list = new SortedList<int, KeyValuePair<string, Func<BaseEvent, Task>>>();
                                eventHandlerDeleagte.Add(packagetype, _list);
                                _list.Add(1, new KeyValuePair<string, Func<BaseEvent, Task>>(hostedExtension.PackageName, method_delegate));
                            }
                            else
                            {
                                _list.Add(1, new KeyValuePair<string, Func<BaseEvent, Task>>(hostedExtension.PackageName, method_delegate));
                            }
                        }
                    }
                }
            }
        }


        public async Task DispatchEvent<T>(T eventPackage)
            where T : BaseEvent
        {
            var type = typeof(T);
            do
            {
                if (eventHandlerDeleagte.TryGetValue(type, out var _list))
                {
                    List<Task> tasks = new List<Task>();
                    int priority = _list.First().Key;
                    foreach (var _action in _list)
                    {
                        if (_action.Key > priority)
                        {
                            Task.WaitAll(tasks.ToArray());
                            if (eventPackage.IsIntercepted)
                                return;
                            else
                                tasks.Clear();
                        }
                        tasks.Add(_action.Value.Value(eventPackage));
                    }
                }
                type = type.BaseType;
            } while (type != typeof(BaseEvent));
        }

    }
}
