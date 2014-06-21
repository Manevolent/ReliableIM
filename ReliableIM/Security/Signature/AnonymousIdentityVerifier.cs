using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Signature
{
    /// <summary>
    /// Represents an identity verifier that allows all given identities. Typically used in a server.
    /// </summary>
    public sealed class AnonymousIdentityVerifier : IIdentityVerifier
    {
        public bool VerifyIdentity(Identity identity)
        {
            return true;
        }
    }
}
