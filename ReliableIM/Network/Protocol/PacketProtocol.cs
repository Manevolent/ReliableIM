using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol
{
    /// <summary>
    /// Represents a method of reading and writing packets to and from a stream.
    /// </summary>
    public sealed class PacketProtocol
    {
        private readonly PacketFactory packetFactory;

        /// <summary>
        /// Creates a new packet protocol.
        /// </summary>
        /// <param name="packetFactory">Packet factory to use for the protocol.</param>
        public PacketProtocol(PacketFactory packetFactory)
        {
            this.packetFactory = packetFactory;
        }

        /// <summary>
        /// Writes a packet onto a stream.
        /// </summary>
        /// <param name="stream">Stream to write the packet to.</param>
        /// <param name="packet">Packet to write.</param>
        public void WritePacket(BinaryWriter stream, Packet packet)
        {
            //Write the packet Id.
            stream.Write(packet.GetPacketID());

            //Write the packet body.
            packet.Write(stream);
        }

        /// <summary>
        /// Reads a packet from a stream.
        /// </summary>
        /// <param name="stream">Stream to read a packet from.</param>
        /// <returns>A packet.</returns>
        public Packet ReadPacket(BinaryReader stream)
        {
            //Read the packet Id.
            byte packetId = stream.ReadByte();

            //Attempt to create the packet object from its Id.
            Packet packet = packetFactory.CreateFromId(packetId);

            //Read the packet body.
            packet.Read(stream);

            //Return to the upper level.
            return packet;
        }
    }
}
