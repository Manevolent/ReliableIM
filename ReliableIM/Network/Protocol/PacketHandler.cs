using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol
{
    public abstract class PacketHandler
    {
        private IPacketStream packetStream;

        public PacketHandler(IPacketStream packetStream)
        {
            this.packetStream = packetStream;
        }

        /// <summary>
        /// The stream being used to read and write packets.
        /// </summary>
        public IPacketStream Stream
        {
            get
            {
                return packetStream;
            }
        }

        /// <summary>
        /// Calls for this packet handler to handle a packet.
        /// </summary>
        /// <param name="packet">Packet to handle.</param>
        public abstract void HandlePacket(Packet packet);

        /// <summary>
        /// Calls for this packet handle to send a ping.
        /// </summary>
        public abstract void Ping();
    }
}
