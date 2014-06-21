using ReliableIM.Security.Signature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.RIM.Packet
{
    public sealed class Packet4Signature : ReliableIM.Network.Protocol.Packet
    {
        private Signature signature;

        public Packet4Signature()
        {

        }

        public Packet4Signature(Signature signature)
        {
            this.signature = signature;
        }

        public override byte GetPacketID()
        {
            return 4;
        }

        /// <summary>
        /// Gets the signature sent with this packet.
        /// </summary>
        public Signature Signature
        {
            get
            {
                return signature;
            }
        }

        public override void Write(System.IO.BinaryWriter stream)
        {
            ReliableIM.Security.Signature.Signature.PROTOCOL.WritePacket(stream, signature);
        }

        public override void Read(System.IO.BinaryReader stream)
        {
            signature = (Signature) ReliableIM.Security.Signature.Signature.PROTOCOL.ReadPacket(stream);
        }
    }
}
