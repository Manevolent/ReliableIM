using ReliableIM.Network.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Signature.DSA
{
    public sealed class DSASignature : Signature
    {
        private byte[] signature;

        public DSASignature(byte[] data, DSAIdentity identity, byte[] signature)
            : base(data, identity)
        {
            this.signature = signature;
        }

        public DSASignature()
            : base(null, null)
        {
            //Do nothing.
        }

        /// <summary>
        /// Gets the encrypted result of the digest computed by the signer.
        /// </summary>
        public byte[] Signature
        {
            get
            {
                return signature;
            }
        }

        public override void Write(System.IO.BinaryWriter stream)
        {
            base.Write(stream);

            Packet.WriteBytes(stream, signature);
        }

        public override void Read(System.IO.BinaryReader stream)
        {
            base.Read(stream);

            signature = Packet.ReadBytes(stream);
        }

        public override byte GetPacketID()
        {
            return 2;
        }
    }
}
