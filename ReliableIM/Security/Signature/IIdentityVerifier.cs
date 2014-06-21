using ReliableIM.Security.Signature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Signature
{
    public interface IIdentityVerifier
    {
        /// <summary>
        /// Verifies a given identity.
        /// </summary>
        /// <param name="identity">The identity to verify</param>
        /// <returns>True if the identification was successful, false otherwise.</returns>
        bool VerifyIdentity(Identity identity);
    }
}
