using ReliableIM.Network.Protocol.RIM.Packet;
using System;
using System.Collections.Generic;
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

            factory.registerPacket(1, typeof(RimPacket1Ping));

            return factory;
        }

        private Socket baseSocket;

        /// <summary>
        /// Creates a new RIM (Reliable IM) socket. This layer does not provide
        /// message encryption, protocol security, or reliability. Each of these
        /// should be provided in the reference socket.
        /// 
        /// A typical network stack is as follows:
        ///     RIM
        ///     SSL
        ///     UDP or TCP
        ///     IP
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

            //Authenticate the other socket as a peer.
            AuthenticatePeer();
        }

        public void AuthenticatePeer()
        {
            //Do some handshaking.
        }

        public override System.IO.Stream GetStream()
        {
            return baseSocket.GetStream();
        }

        public override void Close()
        {
            //Send a disconnect packet.

            baseSocket.Close();
        }
    }
}
