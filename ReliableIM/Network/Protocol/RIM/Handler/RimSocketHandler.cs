using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.RIM.Handler
{
    public abstract class RimSocketHandler
    {
        private RimSocket socket;

        public RimSocketHandler(RimSocket socket)
        {
            this.socket = socket;
        }

        public RimSocket GetSocket()
        {
            return socket;
        }

    }
}
