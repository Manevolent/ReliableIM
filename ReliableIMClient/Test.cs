
using ReliableIM.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIMClient
{
    static class Test
    {
        /// <summary>
        /// Test entry point.
        /// </summary>
        [STAThread]
        static void Main()
        {
            EventManager<Event> eventManager = new EventManager<Event>();
            eventManager.RegisterListener(new KekListener());
            eventManager.Execute(new KekEvent());
            Console.ReadLine();
        }
    }

    class KekEvent : Event
    {

    }

    class KekListener : IEventListener
    {
        [ReliableIM.Event.EventHandler]
        public void handleEvent(KekEvent test)
        {
            Console.WriteLine("Lel ur a fgt");
        }
    }
}
