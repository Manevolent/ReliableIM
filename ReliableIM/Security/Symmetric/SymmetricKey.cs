using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Symmetric
{
    public abstract class SymmetricKey
    {
        /// <summary>
        /// Creates a new encryptor using this symmetric key.
        /// </summary>
        /// <returns>A encryptor cipher.</returns>
        public abstract ICryptoTransform CreateEncryptor();

        /// <summary>
        /// Creates a new decryptor using this symmetric key.
        /// </summary>
        /// <returns>A decryptor cipher.</returns>
        public abstract ICryptoTransform CreateDecryptor();

        /// <summary>
        /// Creates a hash-based message authentication code (HMAC) for use with this symmetric key.
        /// </summary>
        /// <returns></returns>
        public abstract HMAC CreateHMAC();
    }
}
