using Konata.Framework.AdaptiveEvent;
using Konata.Framework.AdaptiveEvent.Utils;
using System.Reflection;

namespace Konata.Framework.Managers
{
    //how to use:
    //public class MyEvent : BaseEventPackage
    //{

    //}

    //public class My2Event : BaseEventPackage
    //{

    //}

    //[AdaptiveEventHandler(SupportSameEventPackageFunction =2)]
    //class TestHandler
    //{
    //    public async Task CallMe(MyEvent package)
    //    {
    //        Console.WriteLine($"TestHandeler:{package.Id}");
    //    }

    //    public async Task CallMeruaruarua(My2Event package)
    //    {
    //        Console.WriteLine($"TestHandeler_ruarua:{package.Id}");
    //    }
    //}

    //[AdaptiveEventHandler]
    //class Test2Handler
    //{
    //    public async Task CallMeeeeeeee(My2Event package)
    //    {
    //        Console.WriteLine($"Test2Handler:{package.Id}");
    //    }
    //}


    //just do this
    //EventHandlerManager.Instance.RegisterAssemblyEventHandlers(typeof(Program).Assembly);

    //then call
    //await EventHandlerManager.Instance.Active(new My2Event{Id = 1919,});
    //see public async Task Active<T>(T eventpackage)

    public class EventHandlerManager
    {
        private static EventHandlerManager _instance = null;

        private Dictionary<Type, object> eventHandlercache = new Dictionary<Type, object>();
        private Dictionary<Type, List<Func<BaseEventPackage, Task>>> eventHandlerdeleagte = new Dictionary<Type, List<Func<BaseEventPackage, Task>>>();


        private EventHandlerManager() { }

        public static EventHandlerManager Instance
        {
            get
            {
                return _instance ?? (_instance = new EventHandlerManager());
            }
        }

        public void RegisterAssemblyEventHandlers(Assembly assembly)
        {
            var _method_list = new List<MethodInfo>();
            foreach (var type in assembly.GetTypes())
            {

                if (type.GetConstructor(Type.EmptyTypes) == null)
                    continue;
                var _attribute = type.GetCustomAttribute<AdaptiveEventHandlerAttribute>();
                if (_attribute is null)
                    continue;

                _method_list.Clear();
                foreach (var method in type.GetMethods())
                {
                    var _params = method.GetParameters();
                    if (_params.Length != 1)
                        continue;
                    if (!_params[0].ParameterType.IsSubclassOf(typeof(BaseEventPackage)))
                        continue;
                    _method_list.Add(method);
                }

                if (_attribute.SupportSameEventPackageFunction != _method_list.Count)
                    continue;
                if (_method_list.Count > 0)
                {
                    var _instance = Activator.CreateInstance(type);
                    eventHandlercache.Add(type, _instance);
                    foreach (var method in _method_list)
                    {
                        var packagetype = method.GetParameters()[0].ParameterType;
                        var method_delegate = GenerateDelegateUtil.GenerateDelegate<Func<BaseEventPackage, Task>>(_instance, method);
                        if (method_delegate != null)
                        {

                            if (!eventHandlerdeleagte.TryGetValue(packagetype, out var _list))
                            {
                                _list = new List<Func<BaseEventPackage, Task>>();
                                eventHandlerdeleagte.Add(packagetype, _list);
                                _list.Add(method_delegate);
                            }
                            else
                            {
                                _list.Add(method_delegate);
                            }
                        }
                    }
                }
            }
        }


        public async Task Active<T>(T eventpackage)
            where T : BaseEventPackage
        {
            if (eventHandlerdeleagte.TryGetValue(typeof(T), out var _list))
            {
                foreach (var _action in _list)
                {
                    await _action(eventpackage);
                }
            }
        }

    }
}
