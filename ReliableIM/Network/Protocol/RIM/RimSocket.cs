using ReliableIM.Network.Protocol.RIM.Handler;
using ReliableIM.Network.Protocol.RIM.Handler.Unauthorized;
using ReliableIM.Network.Protocol.RIM.Packet;
using ReliableIM.Security.Signature;
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
        private static readonly uint PROTOCOL_VERSION = 1;

         private static readonly PacketProtocol PROTOCOL = CreateDefaultProtocol();
        private static PacketProtocol CreateDefaultProtocol()
        {
            PacketFactory factory = new PacketFactory();

            factory.RegisterPacket(1, typeof(Packet1Ping));
            factory.RegisterPacket(2, typeof(Packet2IdentityRequest));
            factory.RegisterPacket(3, typeof(Packet3IdentityResponse));
            factory.RegisterPacket(4, typeof(Packet4Signature));

            factory.RegisterPacket(255, typeof(Packet255Disconnect));

            return new PacketProtocol(factory);
        }

        private Socket baseSocket;
        private PacketBuffer packetBuffer;
        private readonly SignatureAlgorithm signatureAlgorithm;

        /// <summary>
        /// Creates a new RIM (Reliable IM) socket. This layer does not provide
        /// message encryption, protocol security, or reliability. Each of these
        /// should be provided in the reference socket.
        /// 
        /// A typical protocol stack is as follows:
        ///     Application layer: RIM
        ///     Security layer:    SSL
        ///     Transport layer:   UDT/UDP or TCP
        ///     Network layer:     IP
        /// </summary>
        /// <param name="baseSocket">Base socket to reference.</param>
        /// <param name="signatureAlgorithm">Signature algorithm to reference.</param>
        public RimSocket(Socket baseSocket, SignatureAlgorithm signatureAlgorithm)
        {
            this.baseSocket = baseSocket;
            this.signatureAlgorithm = signatureAlgorithm;
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
            if (!baseSocket.IsConnected())
                throw new InvalidOperationException("Cannot authenticate when no transport connection is established");

            //Ensure this socket is handling an unauthorized peer. Using an authorized handler would be
            //a serious mistake.
            PacketHandler = new RimPacketHandlerUnauthorized(this, signatureAlgorithm);

            //Start up a new packet buffer.
            packetBuffer = new PacketBuffer(this);
            packetBuffer.Start();
        }

        public PacketHandler PacketHandler
        {
            get;
            set;
        }

        public PacketProtocol PacketProtocol
        {
            get
            {
                return PROTOCOL;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void WritePacket(ReliableIM.Network.Protocol.Packet packet)
        {
            //Create a new binary writer to write the packet contents.
            BinaryWriter writer = new BinaryWriter(GetStream());

            //Write the packet using this protocol.
            PROTOCOL.WritePacket(writer, packet);

            //Flush the packet to the network.
            writer.Flush();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ReliableIM.Network.Protocol.Packet ReadPacket()
        {
            //Create a new binary reader to read the packet contents.
            BinaryReader reader = new BinaryReader(GetStream());

            //Read the packet using this protocol.
            ReliableIM.Network.Protocol.Packet packet = PROTOCOL.ReadPacket(reader);

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
