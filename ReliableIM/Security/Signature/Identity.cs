using ReliableIM.Network.Protocol;
using ReliableIM.Security.Signature.RSA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Signature
{
    public abstract class Identity : Packet
    {
        public static readonly PacketProtocol PROTOCOL = CreateDefaultProtocol();
        public static PacketProtocol CreateDefaultProtocol()
        {
            PacketFactory factory = new PacketFactory();

            factory.RegisterPacket(1, typeof(RSAIdentity));

            return new PacketProtocol(factory);
        }

        /// <summary>
        /// Finds if another identity matches this one.
        /// </summary>
        /// <param name="identity">Identity to match.</param>
        /// <returns>True if the identity matches, false otherwise.</returns>
        public abstract bool Equals(Identity identity);
    }
}
