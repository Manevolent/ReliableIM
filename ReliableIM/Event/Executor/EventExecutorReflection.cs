using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Event.Executor
{
    public sealed class EventExecutorReflection<T> : EventExecutor<T>
    {
        private IEventListener eventListener;
        private System.Reflection.MethodBase method;
        private EventHandler eventHandler;

        public EventExecutorReflection(
                IEventListener eventListener, 
                System.Reflection.MethodBase method, 
                EventHandler eventHandler
            )
        {
            this.eventListener = eventListener;
            this.method = method;
            this.eventHandler = eventHandler;
        }

        public override void Execute(T e)
        {
            method.Invoke(eventListener, new object[] { e });
        }

        public override bool IsIgnoringCancelled()
        {
            return eventHandler.IgnoreCancelled;
        }
    }
}
