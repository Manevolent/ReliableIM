using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Signature.RSA
{
    public sealed class RSAIdentity : BinaryIdentity
    {
        public RSAIdentity(byte[] identity) : base(identity)
        {
            //Do nothing.
        }

        public RSAIdentity(RSAParameters parameters)
            : this(Checksum(parameters))
        {
            //Do nothing.
        }

        public RSAIdentity() : base(null)
        {
            //Do nothing.
        }

        public override byte GetPacketID()
        {
            return 1;
        }


        public static byte[] Checksum(RSAParameters parameters)
        {
            SHA1 hashAlgorithm = new SHA1Managed();

            hashAlgorithm.Initialize();

            hashAlgorithm.TransformBlock(parameters.Exponent, 0, parameters.Exponent.Length, null, 0);
            hashAlgorithm.TransformBlock(parameters.Modulus, 0, parameters.Modulus.Length, null, 0);
            hashAlgorithm.TransformFinalBlock(new byte[0], 0, 0);

            return hashAlgorithm.Hash;
        }
    }
}
