using ReliableIM.Network.Protocol.RIM.Handler;
using ReliableIM.Network.Protocol.RIM.Handler.Unauthorized;
using ReliableIM.Network.Protocol.RIM.Packet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.RIM
{
    public class RimSocket : Socket, IPacketStream
    {
        private static PacketFactory FACTORY = CreateDefaultFactory();
        private static PacketFactory CreateDefaultFactory()
        {
            PacketFactory factory = new PacketFactory();

            factory.RegisterPacket(1, typeof(Packet1Ping));
            factory.RegisterPacket(2, typeof(Packet2Connect));

            factory.RegisterPacket(255, typeof(Packet255Disconnect));

            return factory;
        }

        private Socket baseSocket;
        private PacketBuffer packetBuffer;

        /// <summary>
        /// Creates a new RIM (Reliable IM) socket. This layer does not provide
        /// message encryption, protocol security, or reliability. Each of these
        /// should be provided in the reference socket.
        /// 
        /// A typical network stack is as follows:
        ///     Application layer: RIM
        ///     Security layer:    SSL
        ///     Transport layer:   UDT/UDP or TCP
        ///     Network layer:     IP
        /// </summary>
        /// <param name="baseSocket">Base socket to reference</param>
        public RimSocket(Socket baseSocket)
        {
            this.baseSocket = baseSocket;
        }

        public override bool IsConnected()
        {
            return baseSocket.IsConnected();
        }

        public override void Connect(System.Net.IPEndPoint endpoint)
        {
            //Connect the lower-level socket.
            baseSocket.Connect(endpoint);

            //Make sure it was successful.
            if (!baseSocket.IsConnected())
                throw new Exception("Lower-level socket connection failed.");

            //Authenticate the other socket as a peer.
            AuthenticatePeer();
        }

        public void AuthenticatePeer()
        {
            //Ensure this socket is handling an unauthorized peer.
            PacketHandler = new RimPacketHandlerUnauthorized(this);

            //Start up a new packet buffer.
            packetBuffer = new PacketBuffer(this);
            packetBuffer.Start();

            //Write a connection packet to the endpoint.
            WritePacket(new Packet2Connect(1));
        }

        public PacketHandler PacketHandler
        {
            get;
            set;
        }

        public PacketFactory PacketFactory
        {
            get
            {
                return FACTORY;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void WritePacket(ReliableIM.Network.Protocol.Packet packet)
        {
            //Create a new binary writer to write the packet contents.
            BinaryWriter writer = new BinaryWriter(GetStream());

            //Write the packet ID to the stream as a byte.
            writer.Write((byte) packet.GetPacketID());

            //Write the packet body itself onto the stream.
            packet.Write(writer);

            writer.Flush();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ReliableIM.Network.Protocol.Packet ReadPacket()
        {
            //Create a new binary reader to read the packet contents.
            BinaryReader reader = new BinaryReader(GetStream());

            //Read the packet ID byte from the stream.
            byte packetId = reader.ReadByte();

            //Construct a new packet instance from the ID read.
            ReliableIM.Network.Protocol.Packet packet = FACTORY.CreateFromId(packetId);

            //Read the packet contents from the stream.
            packet.Read(reader);

            //Return the packet to the caller.
            return packet;
        }

        public override System.IO.Stream GetStream()
        {
            return baseSocket.GetStream();
        }

        public override void Close()
        {
            //Close the underlying socket.
            baseSocket.Close();
        }
    }
}
