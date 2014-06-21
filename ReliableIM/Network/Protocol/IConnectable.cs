using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol
{
    public interface IConnectable
    {
        /// <summary>
        /// Connects to this endpoint.
        /// </summary>
        /// <returns>Packet stream around the established connection.</returns>
        IPacketStream Connect();
    }
}
