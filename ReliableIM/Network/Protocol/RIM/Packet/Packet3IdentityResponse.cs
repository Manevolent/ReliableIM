using ReliableIM.Security.Signature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.RIM.Packet
{
    public sealed class Packet3IdentityResponse : ReliableIM.Network.Protocol.Packet
    {
        private SignatureAlgorithm signatureAlgorithm;
        private Signature signature;

        public Packet3IdentityResponse(SignatureAlgorithm signatureAlgorithm, Signature signature)
        {
            this.signatureAlgorithm = signatureAlgorithm;
            this.signature = signature;
        }

        public Packet3IdentityResponse()
        {
            //Do nothing.
        }

        public SignatureAlgorithm SignatureAlgorithm
        {
            get
            {
                return signatureAlgorithm;
            }
        }

        public Signature Signature
        {
            get
            {
                return signature;
            }
        }

        public override byte GetPacketID()
        {
            return 3;
        }

        public override void Write(System.IO.BinaryWriter stream)
        {
            SignatureAlgorithm.PROTOCOL.WritePacket(stream, signatureAlgorithm);
            Signature.PROTOCOL.WritePacket(stream, signature);
        }

        public override void Read(System.IO.BinaryReader stream)
        {
            signatureAlgorithm = (SignatureAlgorithm)SignatureAlgorithm.PROTOCOL.ReadPacket(stream);
            signature = (Signature) Signature.PROTOCOL.ReadPacket(stream);
        }
    }
}
