using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Signature
{
    public sealed class SignatureStore
    {
        private Dictionary<Identity, SignatureAlgorithm> signatures = new Dictionary<Identity, SignatureAlgorithm>();

        public SignatureStore()
        {
        }

        public SignatureAlgorithm GetAlgorithm(Identity identity)
        {
            return signatures[identity];
        }

        public bool HasIdentity(Identity identity)
        {
            return signatures.ContainsKey(identity);
        }
    }
}
