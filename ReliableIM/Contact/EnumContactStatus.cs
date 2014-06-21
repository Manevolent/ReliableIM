using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Contact
{
    public enum ContactStatus
    {
        /// <summary>
        /// The contact is online and ready to engage in conversation.
        /// </summary>
        Online,

        /// <summary>
        /// The contact is busy and may not respond.
        /// </summary>
        Busy,

        /// <summary>
        /// The contact is currently pretending to be offline.
        /// </summary>
        Invisible,

        /// <summary>
        /// The contact is not connected, or has specified an unknown status.
        /// </summary>
        Unknown
    }
}
