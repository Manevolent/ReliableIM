using ReliableIM.Security.Symmetric;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Exchange
{
    public abstract class KeyExchangeAlgorithm
    {
        /// <summary>
        /// Exchanges keys with another party, computing a symmetric key pair for use with futher cryptographic communications.
        /// </summary>
        /// <param name="stream">Stream to exchange keys over.</param>
        /// <returns>A symmetric key pair for use with futher cryptographic communication.</returns>
        public abstract SymmetricKeyPair Exchange(Stream stream);
    }
}
