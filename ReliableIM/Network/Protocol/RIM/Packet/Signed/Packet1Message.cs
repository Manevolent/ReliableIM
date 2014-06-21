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
        /// The identity of the sender.
        /// </summary>
        private Identity from;

        /// <summary>
        /// The identity of the recipient.
        /// </summary>
        private Identity to;

        /// <summary>
        /// The body contained in this message.
        /// </summary>
        private string message;

        public Packet1Message(Identity to, string message)
        {
            this.to = to;

            this.message = message;
        }

        public Packet1Message()
        {
        }

        public Identity From
        {
            get
            {
                return from;
            }
        }

        public Identity To
        {
            get
            {
                return to;
            }
        }

        public string Message
        {
            get
            {
                return message;
            }
        }

        public override byte GetPacketID()
        {
            return 1;
        }

        protected override bool VerifySignature(SignatureAlgorithm signatureAlgorithm, Signature signature, bool direct)
        {
            this.from = signatureAlgorithm.Identity;

            //Enforce sender authenticity.
            return base.VerifySignature(signatureAlgorithm, signature, direct);
        }

        public override void Write(System.IO.BinaryWriter stream)
        {
            Identity.PROTOCOL.WritePacket(stream, to);
            stream.Write(message);
        }

        public override void Read(System.IO.BinaryReader stream)
        {
            to = (Identity) Identity.PROTOCOL.ReadPacket(stream);
            message = stream.ReadString();
        }
    }
}
