using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol
{
    public class PacketFactory<T> where T : Packet
    {
        private Dictionary<byte, Type> packetMap = new Dictionary<Byte, Type>();

        public void registerPacket(byte packetId, Type classType)
        {
            if (!classType.IsSubclassOf(typeof(T)))
                throw new NotSupportedException();

            packetMap[packetId] = classType;
        }

        /// <summary>
        /// Creates a new packet from the given ID
        /// </summary>
        /// <param name="packetId">Packet ID, presumably read from the network stream</param>
        /// <returns>A new packet from the given ID, otherwise null if the ID is not known.</returns>
        public T createFromId(byte packetId)
        {
            if (!packetMap.ContainsKey(packetId))
                return default(T);
            else
                return (T)Activator.CreateInstance(packetMap[packetId]);
        }
    }
}
