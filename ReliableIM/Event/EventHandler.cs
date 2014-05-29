using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Event
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class EventHandler : System.Attribute
    {
        private EventPriority priority;
        private bool ignoreCancelled;

        public EventHandler(EventPriority priority, bool ignoreCancelled)
        {
            this.priority = priority;
            this.ignoreCancelled = ignoreCancelled;
        }

        public EventHandler(EventPriority priority)
            : this(priority, true)
        {
        }

        public EventHandler()
            : this(EventPriority.Normal)
        {

        }

        public bool IgnoreCancelled
        {
            get
            {
                return ignoreCancelled;
            }
            set
            {
                ignoreCancelled = value;
            }
        }
    }
}
