using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReliableIM.Event.Executor;
using System.Reflection;

namespace ReliableIM.Event
{
    public class EventManager<T> where T : Event
    {
        private Dictionary<Type, List<EventExecutor<T>>> executorMap = new Dictionary<Type, List<EventExecutor<T>>>();
        private Type managerType = typeof(T); //Just for quick reference.
        
        public EventManager()
        {
            if (managerType == null || !managerType.IsClass || !managerType.IsAbstract)
                throw new ArgumentException("Event type must be an abstract class");
        }

        public void RegisterListener(IEventListener eventListener)
        {
            Type listenerType = eventListener.GetType();
            Type handlerType = typeof(EventHandler);
            MethodInfo[] methods = listenerType.GetMethods();

            foreach (MethodInfo method in methods)
            {
                Attribute methodAttribute = method.GetCustomAttribute(handlerType, false);

                if (methodAttribute != null && methodAttribute is EventHandler)
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters.Length == 1)
                        RegisterExecutor(parameters[0].ParameterType, new EventExecutorReflection<T>(eventListener, method));
                }
            }
        }

        public void RegisterExecutor(Type eventType, EventExecutor<T> executor) {
            if (eventType == null || executor == null)
                throw new ArgumentException("Parameter cannot be null.");

            if (eventType.IsAbstract)
                throw new ArgumentException("Event type the executor is responsible for is abstract, and cannot be instantiated directly.");

            if (!eventType.IsSubclassOf(managerType))
                throw new ArgumentException("Event type the executor is responsible for is not a superclass of the event manager's event type.");

            if (!executorMap.ContainsKey(eventType) || executorMap[eventType] == null)
                executorMap[eventType] = new List<EventExecutor<T>>();

            executorMap[eventType].Add(executor);
        }

        public T Execute(T e) {
            Type eventType = e.GetType();

            if (!executorMap.ContainsKey(eventType))
                return e;

            List<EventExecutor<T>> executors = executorMap[eventType];

            if (executors == null)
                return e;

            foreach (EventExecutor<T> eventExecutor in executors)
                if (!eventExecutor.IsIgnoringCancelled() || !e.IsCancelled)
                    eventExecutor.Execute(e);

            return e;
        }
    }
}
