using ReliableIM.Network.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Signature
{
    public abstract class BinaryIdentity : Identity
    {
        private byte[] identity;

        public BinaryIdentity(byte[] identity)
        {
            this.identity = identity;
        }

        public byte[] Identity
        { 
            get
            {
                return identity;
            }
        }

        public override bool Equals(Identity identity)
        {
            return identity != null &&
                identity is BinaryIdentity 
                && ((BinaryIdentity)identity).identity.SequenceEqual(this.identity);
        }

        public override void Write(System.IO.BinaryWriter stream)
        {
            Packet.WriteBytes(stream, identity);
        }

        public override void Read(System.IO.BinaryReader stream)
        {
            identity = Packet.ReadBytes(stream);
        }
    }
}
