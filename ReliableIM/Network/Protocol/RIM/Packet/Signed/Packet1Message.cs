using ReliableIM.Security.Signature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.RIM.Packet.Signed
{
    public sealed class Packet1Message : PacketSigned
    {
        /// <summary>
        /// The fingerprint of the sender.
        /// </summary>
        private string from;

        /// <summary>
        /// The fingerprint of the recipient.
        /// </summary>
        private string to;

        /// <summary>
        /// The body contained in this message.
        /// </summary>
        private string message;

        public override byte GetPacketID()
        {
            return 4;
        }

        protected override bool VerifySignature(SignatureAlgorithm signatureAlgorithm, Signature signature)
        {
            //Enforce sender authenticity.
            return base.VerifySignature(signatureAlgorithm, signature) && true;
        }

        public override void Write(System.IO.BinaryWriter stream)
        {
            stream.Write(from);
            stream.Write(to);
            stream.Write(message);
        }

        public override void Read(System.IO.BinaryReader stream)
        {
            from = stream.ReadString();
            to = stream.ReadString();
            message = stream.ReadString();
        }
    }
}
