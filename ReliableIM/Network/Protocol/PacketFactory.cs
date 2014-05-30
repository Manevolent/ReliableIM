using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol
{
    public class PacketFactory
    {
        private Dictionary<byte, Type> packetMap = new Dictionary<Byte, Type>();

        /// <summary>
        /// Registers a packet into this packet factory.
        /// </summary>
        /// <param name="packetId">Packet ID to bind to.</param>
        /// <param name="classType">Class type, extending T, to bind to the packet ID.</param>
        public void RegisterPacket(byte packetId, Type classType)
        {
            packetMap[packetId] = classType;
        }

        /// <summary>
        /// Creates a new packet from the given ID
        /// </summary>
        /// <param name="packetId">Packet ID, presumably read from the network stream</param>
        /// <returns>A new packet from the given ID, otherwise null if the ID is not known.</returns>
        public Packet CreateFromId(byte packetId)
        {
            if (!packetMap.ContainsKey(packetId))
                return null;
            else
                return (Packet) Activator.CreateInstance(packetMap[packetId]);
        }
    }
}
