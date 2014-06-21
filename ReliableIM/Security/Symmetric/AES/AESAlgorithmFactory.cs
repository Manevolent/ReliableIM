using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Symmetric.AES
{
    public sealed class AESAlgorithmFactory : SymmetricAlgorithmFactory
    {
        private readonly int blockSize;

        private readonly CipherMode cipherMode;
        private readonly PaddingMode paddingMode;

        /// <summary>
        /// Creates a new AES algorithm factory.
        /// </summary>
        /// <param name="blockSize">Block size, in bits, of the produced AES ciphers.</param>
        /// <param name="cipherMode">Cipher mode to use when creating ciphers.</param>
        /// <param name="paddingMode">Padding mode to use when creating ciphers.</param>
        public AESAlgorithmFactory(int blockSize, CipherMode cipherMode, PaddingMode paddingMode)
        {
            this.blockSize = blockSize;

            this.cipherMode = cipherMode;
            this.paddingMode = paddingMode;
        }

        public override SymmetricKey CreateKey(byte[] initializationVector, byte[] cipherKey)
        {
            if (initializationVector.Length != 16)
                throw new ArgumentOutOfRangeException("Cipher IV is not 16 bytes long.", "initializationVector");

            if (cipherKey.Length * 8 != blockSize)
                throw new ArgumentOutOfRangeException("Cipher block size is not " + blockSize + " bits long.", "cipherKey");

            return new AESKey(initializationVector, cipherKey, cipherMode, paddingMode);
        }

        public override byte[] CreateIV(byte[] cipherKey)
        {
            return new byte[16];
        }

        public override int BlockSize
        {
            get
            {
                return blockSize;
            }
        }
    }
}
