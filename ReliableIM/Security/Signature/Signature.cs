using ReliableIM.Network.Protocol;
using ReliableIM.Security.Signature.DSA;
using ReliableIM.Security.Signature.RSA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Signature
{
    public abstract class Signature : Packet
    {
        public static readonly PacketProtocol PROTOCOL = CreateDefaultProtocol();
        public static PacketProtocol CreateDefaultProtocol()
        {
            PacketFactory factory = new PacketFactory();

            factory.RegisterPacket(1, typeof(RSASignature));
            factory.RegisterPacket(2, typeof(DSASignature));

            return new PacketProtocol(factory);
        }

        private byte[] data;
        private Identity identity;

        /// <summary>
        /// Creates a new signature object.
        /// </summary>
        /// <param name="data">The data that was signed.</param>
        /// <param name="identity">The identity of the signer.</param>
        public Signature(byte[] data, Identity identity)
        {
            this.data = data;
            this.identity = identity;
        }

        /// <summary>
        /// Gets the data that this signer has signed.
        /// </summary>
        public byte[] Data
        {
            get
            {
                return data;
            }
        }
        
        /// <summary>
        /// Gets the identity of the signer.
        /// </summary>
        public Identity Identity
        {
            get
            {
                return identity;
            }
        }

        public override void Write(System.IO.BinaryWriter stream)
        {
            //Write signature data.
            Packet.WriteBytes(stream, data);

            //Write cryptographic signature. 
            ReliableIM.Security.Signature.Identity.PROTOCOL.WritePacket(stream, identity);
        }

        public override void Read(System.IO.BinaryReader stream)
        {
            data = Packet.ReadBytes(stream);

            identity = (Identity) ReliableIM.Security.Signature.Identity.PROTOCOL.ReadPacket(stream);
        }
    }
}
