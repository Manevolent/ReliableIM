using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Symmetric.AES
{
    public sealed class AESKey : SymmetricKey
    {
        private readonly CipherMode cipherMode;
        private readonly PaddingMode paddingMode;

        private readonly byte[] iv;
        private readonly byte[] key;

        public AESKey(byte[] iv, byte[] key, CipherMode cipherMode, PaddingMode paddingMode)
        {
            this.iv = iv;
            this.key = key;

            this.cipherMode = cipherMode;
            this.paddingMode = paddingMode;
        }

        public override System.Security.Cryptography.ICryptoTransform CreateEncryptor()
        {
            return CreateCipher().CreateEncryptor(key, iv);
        }

        public override System.Security.Cryptography.ICryptoTransform CreateDecryptor()
        {
            return CreateCipher().CreateDecryptor(key, iv);
        }

        public Aes CreateCipher()
        {
            Aes aes = new AesCryptoServiceProvider();

            aes.Mode = cipherMode;
            aes.Padding = paddingMode;

            return aes;
        }

        public override HMAC CreateHMAC()
        {
            switch (key.Length)
            {
                case 16:
                    return new HMACSHA1(key); //160-bit
                case 32:
                    return new HMACSHA256(key); //256-bit
                case 64:
                    return new HMACSHA512(key); //512-bit
                default:
                    return new HMACSHA1(key); //Default: 160-bit.
            }
        }
    }
}
