using ReliableIM.Network.Protocol;
using ReliableIM.Security.Signature.RSA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Signature
{
    public abstract class SignatureAlgorithm : Packet
    {
        public static readonly PacketProtocol PROTOCOL = CreateDefaultProtocol();
        public static PacketProtocol CreateDefaultProtocol()
        {
            PacketFactory factory = new PacketFactory();

            factory.RegisterPacket(1, typeof(RSASignatureAlgorithm));

            return new PacketProtocol(factory);
        }

        /// <summary>
        /// Signs the given data using this algorithm.
        /// </summary>
        /// <param name="data">Data to sign.</param>
        /// <returns>Signed data.</returns>
        public abstract Signature Sign(byte[] data);

        /// <summary>
        /// Verifies the given signature.
        /// </summary>
        /// <param name="signature">Signature to check.</param>
        /// <returns>True if the signed data is valid, false otherwise.</returns>
        public abstract bool Verify(Signature signature);

        /// <summary>
        /// Gets the public identity of this signer.
        /// </summary>
        /// <returns>Public identifier.</returns>
        public abstract Identity GetIdentity();

        /// <summary>
        /// Gets the name of this signature algorithm.
        /// </summary>
        /// <returns>Algorithm name.</returns>
        public abstract string GetName();
    }
}
