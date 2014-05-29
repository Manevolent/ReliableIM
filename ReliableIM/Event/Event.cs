using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Event
{
    public abstract class Event
    {
        public bool IsCancelled { get; set; }
    }
}
