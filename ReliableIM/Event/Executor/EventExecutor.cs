using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Event.Executor
{
    /// <summary>
    /// Provides basic functionality for executing events in the event manager.
    /// </summary>
    /// <typeparam name="T">The type of event to execute.</typeparam>
    public abstract class EventExecutor<T>
    {
        /// <summary>
        /// Finds the cancel ignoration status of this executor.
        /// </summary>
        /// <returns>True if this executor is currently ignoring cancelled events.</returns>
        public abstract bool IsIgnoringCancelled();

        /// <summary>
        /// Fires the executor with a given event.
        /// </summary>
        /// <param name="e">Event to pass as a paramter.</param>
        public abstract void Execute(T e);
    }
}
