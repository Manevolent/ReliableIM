using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Signature
{
    public sealed class DefaultIdentityVerifier : IIdentityVerifier
    {
        private readonly Identity identity;

        /// <summary>
        /// Constructs a new default identity verifier.
        /// </summary>
        /// <param name="identity">Identity to verify.</param>
        public DefaultIdentityVerifier(Identity identity)
        {
            this.identity = identity;
        }

        public bool VerifyIdentity(Identity identity)
        {
            return this.identity.Equals(identity);
        }
    }
}
