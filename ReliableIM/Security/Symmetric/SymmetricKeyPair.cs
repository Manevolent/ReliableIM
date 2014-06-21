using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Symmetric
{
    public sealed class SymmetricKeyPair
    {
        private readonly SymmetricKey local;
        private readonly SymmetricKey remote;

        /// <summary>
        /// Creates a new symmetric key pair. Usually used to describe a key exchange.
        /// </summary>
        /// <param name="local">Locally held key.</param>
        /// <param name="remote">Remotely held key.</param>
        public SymmetricKeyPair(SymmetricKey local, SymmetricKey remote)
        {
            this.local = local;
            this.remote = remote;
        }

        /// <summary>
        /// Gets the local symmetric key.
        /// </summary>
        public SymmetricKey Local
        {
            get
            {
                return local;
            }
        }

        /// <summary>
        /// Gets the remote symmetric key.
        /// </summary>
        public SymmetricKey Remote
        {
            get
            {
                return remote;
            }
        }
    }
}
