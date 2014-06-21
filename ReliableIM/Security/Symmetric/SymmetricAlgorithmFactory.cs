using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Symmetric
{
    public abstract class SymmetricAlgorithmFactory
    {
        /// <summary>
        /// Gets the block size, in bits, of the symmetric algorithms created by this factory.
        /// </summary>
        public abstract int BlockSize { get; }

        /// <summary>
        /// Creates a new symmetric key, which will in turn create a symmetric algorithm.
        /// </summary>
        /// <param name="initializationVector">Initialization vector (IV) to use when creating this symmetric algorithm.</param>
        /// <param name="cipherKey">Cipher key to use when creating this symmetric algorithm.</param>
        /// <returns>A symmetric key capable of creating a symmetric algorithm.</returns>
        public abstract SymmetricKey CreateKey(byte[] initializationVector, byte[] cipherKey);

        /// <summary>
        /// Computes an initialization vector (IV) for use with the given cipher key.
        /// </summary>
        /// <param name="cipherKey">Cipher key to generate an initialization vector (IV) for.</param>
        /// <returns>An initialization vector (IV) for use with the given cipher key.</returns>
        public abstract byte[] CreateIV(byte[] cipherKey);
    }
}
