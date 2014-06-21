using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Signature.DSA
{
    public sealed class DSAIdentity : BinaryIdentity
    {
        public DSAIdentity(byte[] identity)
            : base(identity)
        {
            //Do nothing.
        }

        public DSAIdentity(DSAParameters parameters)
            : this(Checksum(parameters))
        {
            //Do nothing.
        }

        public DSAIdentity() : base(null)
        {
            //Do nothing.
        }

        public override byte GetPacketID()
        {
            return 2;
        }

        public static byte[] Checksum(DSAParameters parameters)
        {
            SHA1 hashAlgorithm = new SHA1Managed();

            hashAlgorithm.Initialize();

            hashAlgorithm.TransformBlock(parameters.P, 0, parameters.P.Length, null, 0);
            hashAlgorithm.TransformBlock(parameters.Q, 0, parameters.Q.Length, null, 0);
            hashAlgorithm.TransformBlock(parameters.G, 0, parameters.G.Length, null, 0);
            hashAlgorithm.TransformBlock(parameters.Y, 0, parameters.Y.Length, null, 0);
            hashAlgorithm.TransformFinalBlock(new byte[0], 0, 0);

            return hashAlgorithm.Hash;
        }
    }
}
