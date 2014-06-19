using ReliableIM.Network.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Signature.RSA
{
    public class RSASignatureAlgorithm : SignatureAlgorithm
    {
        private static readonly Dictionary<int, int> KEYSIZE_MAP = CreateKeySizeMap();
        private static Dictionary<int, int> CreateKeySizeMap()
        {
            Dictionary<int, int> keySizeMap = new Dictionary<int, int>();

            //Acceptable RSA + Hash strength for digital signature cipher suites:
            keySizeMap.Add(1024, 160);
            keySizeMap.Add(2048, 256);

            return keySizeMap;
        }

        private RSACryptoServiceProvider rsa;
        private HashAlgorithm hashAlgorithm;
        private RSAIdentity identity;

        public RSASignatureAlgorithm(RSACryptoServiceProvider rsa, HashAlgorithm hashAlgorithm)
        {
            //Go directly to the initializer:
            InitializeAlgorithm(rsa, hashAlgorithm);
        }

        public RSASignatureAlgorithm()
        {
            //Do nothing.
        }

        /// <summary>
        /// Initializes this instance of an RSA signature algorithm.
        /// </summary>
        /// <param name="rsa">RSA cryptographic service provider to use.</param>
        /// <param name="hashAlgorithm">Hash algorithm used to compute data digests for signing.</param>
        private void InitializeAlgorithm(RSACryptoServiceProvider rsa, HashAlgorithm hashAlgorithm)
        {
            if (!(hashAlgorithm is SHA1) && !(hashAlgorithm is SHA256)) 
                throw new SecurityException("Unacceptable hash algorithm");

            if (!KEYSIZE_MAP.ContainsKey(rsa.KeySize))
                throw new SecurityException("Unacceptable RSA key size: " + rsa.KeySize);
            this.rsa = rsa;

            int hashSize = KEYSIZE_MAP[rsa.KeySize];
            if (hashSize != hashAlgorithm.HashSize)
                throw new SecurityException("Unexpected hash size: " + hashAlgorithm.HashSize + " (expected " + hashSize + ")");
            this.hashAlgorithm = hashAlgorithm;

            //Compute the identity.
            RSAParameters parameters = rsa.ExportParameters(false);
            this.identity = new RSAIdentity(Checksum(parameters.Exponent, parameters.Modulus));
        }

        public override Signature Sign(byte[] data)
        {
            //Ensure we are using a private key to sign this data. If we sign it
            //with the public key, we would theoretically have to verify it with
            //the private key, which isn't distributed.
            if (rsa.PublicOnly)
                throw new SecurityException("Cannot sign data with public key.");

            lock (hashAlgorithm)
            {
                //Initialize the hash algorithm to ensure all previous buffers are clear.
                hashAlgorithm.Initialize();

                //Sign the message by hashing the data, and encrypting the resulting digest
                //with the private key of this signature algorithm.
                return new RSASignature(
                        data, //Supply the data itself.
                        identity, //Supply the identity to ensure parties know who signed the data.
                        rsa.SignData(hashAlgorithm.ComputeHash(data), hashAlgorithm) //Sign the data.
                );
            }
        }

        public override bool Verify(Signature signature)
        {
            lock (hashAlgorithm)
            {
                //Initialize the hash algorithm to ensure all previous buffers are clear.
                hashAlgorithm.Initialize();

                //Verify the signature's type is an RSA signature.
                if (!(signature is RSA.RSASignature))
                    throw new SecurityException("Signature is not an RSA signature.");

                //Verify the identity of the signature matches this signature algorithm.
                if (!signature.Identity.Equals(identity))
                    throw new SecurityException("Signature's identity does not match.");

                //Verify the data by hashing the message, then comparing the resulting
                //digest with the public-key-decrypted result of the data signature.
                return rsa.VerifyData(
                        signature.Data,
                        hashAlgorithm,
                        ((RSA.RSASignature)signature).Signature
                );
            }
        }

        private byte[] Checksum(params byte[][] sources)
        {
            lock (hashAlgorithm)
            {
                hashAlgorithm.Initialize();

                foreach (byte[] source in sources)
                {
                    hashAlgorithm.TransformBlock(source, 0, source.Length, null, 0);
                }

                hashAlgorithm.TransformFinalBlock(new byte[0], 0, 0);

                return hashAlgorithm.Hash;
            }
        }

        public override Identity GetIdentity()
        {
            return identity;
        }

// Packet

        public override byte GetPacketID()
        {
            return 1;
        }

        public override void Write(System.IO.BinaryWriter stream)
        {
            //Write the parameters.
            RSAParameters parameters = rsa.ExportParameters(false);
            Packet.WriteBytes(stream, parameters.Exponent);
            Packet.WriteBytes(stream, parameters.Modulus);
        }

        public override void Read(System.IO.BinaryReader stream)
        {
            //Read the parameters.
            RSAParameters parameters = new RSAParameters();
            parameters.Exponent = Packet.ReadBytes(stream);
            parameters.Modulus = Packet.ReadBytes(stream);

            //Import the parameters into a new RSA CSP.
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(parameters);

            //Initialize the algorithm.
            InitializeAlgorithm(rsa, new SHA1Managed());
        }

        public override string GetName()
        {
            return "RSA" + rsa.KeySize + "SHA" + hashAlgorithm.HashSize;
        }
    }
}
