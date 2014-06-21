using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Signature
{
    /// <summary>
    /// Represents an identity verifier that requires given identities to match a byte array. Typically used in a client.
    /// </summary>
    public sealed class BinaryIdentityVerifier : IIdentityVerifier
    {
        private readonly byte[] identity;

        /// <summary>
        /// Constructs a new binary identity verifier.
        /// </summary>
        /// <param name="identity">The identity fingerprint to match when verifying an identity.</param>
        public BinaryIdentityVerifier(byte[] identity)
        {
            this.identity = identity;
        }

        /// <summary>
        /// Constructs a new binary identity verifier.
        /// </summary>
        /// <param name="binaryIdentity">The binary identity to match when verifying an identity.</param>
        public BinaryIdentityVerifier(BinaryIdentity binaryIdentity)
        {
            this.identity = binaryIdentity.Identity;
        }

        public bool VerifyIdentity(Identity identity)
        {
            if (!(identity is BinaryIdentity))
                return false;

            return ((BinaryIdentity)identity).Identity.SequenceEqual(this.identity);
        }
    }
}
