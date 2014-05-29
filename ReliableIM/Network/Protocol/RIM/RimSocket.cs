using ReliableIM.Network.Protocol.RIM.Handler;
using ReliableIM.Network.Protocol.RIM.Handler.Unauthorized;
using ReliableIM.Network.Protocol.RIM.Packet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.RIM
{
    public class RimSocket : Socket
    {
        private static PacketFactory<RimPacket> FACTORY = CreateDefaultFactory();
        private static PacketFactory<RimPacket> CreateDefaultFactory()
        {
            PacketFactory<RimPacket> factory = new PacketFactory<RimPacket>();

            factory.RegisterPacket(1, typeof(RimPacket1Ping));
            factory.RegisterPacket(2, typeof(RimPacket2Connect));

            factory.RegisterPacket(255, typeof(RimPacket255Disconnect));

            return factory;
        }

        private Socket baseSocket;

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

        public RimSocketHandler Handler { get; set; }

        public override bool IsConnected()
        {
            return baseSocket.IsConnected();
        }

        public override void Connect(System.Net.IPEndPoint endpoint)
        {
            //Connect the lower-level socket.
            baseSocket.Connect(endpoint);

            //Reset our socket handler.
            Handler = new RimSocketHandlerUnauthorized(this);

            //Start up a new spooler.


            //Authenticate the other socket as a peer.
            AuthenticatePeer();
        }

        public void AuthenticatePeer()
        {
            //Do some handshaking.
        }

        /// <summary>
        /// Writes a packet onto the underlying stream.
        /// </summary>
        /// <param name="packet">Packet to write.</param>
        public void WritePacket(RimPacket packet)
        {
            //Create a new binary writer to write the packet contents.
            BinaryWriter writer = new BinaryWriter(GetStream());

            //Write the packet ID to the stream.
            writer.Write(packet.GetPacketID());

            //Write the packet itself onto the stream.
            packet.Write(writer);

            //Flush the data down to the next layer.
            writer.Flush();
        }

        public RimPacket ReadPacket()
        {
            //Create a new binary reader to read the packet contents.
            BinaryReader reader = new BinaryReader(GetStream());

            //Read the packet ID to the stream.
            byte packetId = reader.ReadByte();

            //Construct a new packet instance from the ID read.
            RimPacket packet = FACTORY.CreateFromId(packetId);

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
            //Send a disconnect packet.
            WritePacket(new RimPacket255Disconnect(RimPacket255Disconnect.DisconnectReason.GeneralDisconnect));

            //Close the underlying socket.
            baseSocket.Close();
        }
    }
}
