using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol
{
    public interface IPacketStream
    {
        /// <summary>
        /// The packet handler responsible for handling read packets.
        /// </summary>
        PacketHandler PacketHandler { get; set; }

        /// <summary>
        /// The packet factory responsible for creating packets from their identifiers.
        /// </summary>
        PacketFactory PacketFactory { get; }

        /// <summary>
        /// Reads a packet from the underlying stream.
        /// </summary>
        /// <returns>Read packet, null if none was ready.</returns>
        Packet ReadPacket();

        /// <summary>
        /// Writes a packet onto the underlying stream.
        /// </summary>
        /// <param name="packet">Packet to write.</param>
        void WritePacket(Packet packet);

        /// <summary>
        /// Closes the receiver.
        /// </summary>
        void Close();

        /// <summary>
        /// Finds connectivity status.
        /// </summary>
        /// <returns>True if the packet stream is connected.</returns>
        bool IsConnected();
    }
}
