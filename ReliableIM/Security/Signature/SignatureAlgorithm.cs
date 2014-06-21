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
    public abstract class SignatureAlgorithm : Packet
    {
        public static readonly PacketProtocol PROTOCOL = CreateDefaultProtocol();
        public static PacketProtocol CreateDefaultProtocol()
        {
            PacketFactory factory = new PacketFactory();

            factory.RegisterPacket(1, typeof(RSASignatureAlgorithm));
            factory.RegisterPacket(2, typeof(DSASignatureAlgorithm));

            return new PacketProtocol(factory);
        }

        /// <summary>
        /// Signs the given data using this algorithm.
        /// </summary>
        /// <param name="data">Data to sign.</param>
        /// <returns>Signed data.</returns>
        public abstract Signature Sign(byte[] data);

        /// <summary>
        /// Gets a flag indicating whether or not this signature algorithm is capable of signing messages.
        /// </summary>
        public abstract bool CanSign
        {
            get;
        }

        /// <summary>
        /// Verifies the given signature.
        /// </summary>
        /// <param name="signature">Signature to check.</param>
        /// <returns>True if the signed data is valid, false otherwise.</returns>
        public abstract bool Verify(Signature signature);

        /// <summary>
        /// Gets the public identity of this signer.
        /// </summary>
        public abstract Identity Identity
        {
            get;
        }

        /// <summary>
        /// Gets the name of this signature algorithm.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// Creates an identity verifier for this signature algorithm.
        /// </summary>
        /// <returns>An identity verifier capable of verifying this signer's identity.</returns>
        public abstract IIdentityVerifier CreateIdentityVerifier();
    }
}
