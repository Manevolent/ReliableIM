using ReliableIM.Network.Protocol;
using ReliableIM.Security.Signature;
using ReliableIM.Security.Signature.RSA;
using ReliableIM.Security.Symmetric;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Exchange.RSA
{
    public sealed class RSAKeyExchangeAlgorithm : KeyExchangeAlgorithm
    {
        private readonly RSACryptoServiceProvider localRsa;
        private readonly IIdentityVerifier identityVerifier;
        private readonly SymmetricAlgorithmFactory factory;

        /// <summary>
        /// Constructs a new RSA key exchange algorithm for use with exchanging RSA identities and verifying them to produce
        /// a two-way symmetrical cipher pair.
        /// </summary>
        /// <param name="localRsa">Local RSA instance to use during key exchange.</param>
        /// <param name="identityIdentifier">A key identifier to use when verifying remote identities.</param>
        /// <param name="factory">A symmetric key algorithm factory to use when generating and creating keypairs.</param>
        public RSAKeyExchangeAlgorithm(RSACryptoServiceProvider localRsa, IIdentityVerifier identityIdentifier, SymmetricAlgorithmFactory factory)
        {
            this.localRsa = localRsa;
            this.identityVerifier = identityIdentifier;
            this.factory = factory;
        }

        public override Symmetric.SymmetricKeyPair Exchange(System.IO.Stream stream)
        {
            BinaryReader binaryReader = new BinaryReader(stream);
            BinaryWriter binaryWriter = new BinaryWriter(stream);

            //Write our local RSA public key.
            WritePublicParameters(binaryWriter, localRsa.ExportParameters(false));
            binaryWriter.Flush();

            //Read the RSA public key of the remote party.
            RSAParameters remoteParameters = ReadPublicParameters(binaryReader);

            //Verify the remote party's public key.
            if (identityVerifier.VerifyIdentity(new RSAIdentity(remoteParameters)))
            {
                binaryWriter.Write(true);
                binaryWriter.Flush();
            }
            else
            {
                binaryWriter.Write(false);
                binaryWriter.Flush();

                throw new SecurityException("The remote party's RSA public key was denied by the local identifier.");
            }

            //Read the status code from the remote party, and ensure our local public key wasn't denied.
            if (!binaryReader.ReadBoolean())
                throw new SecurityException("The local RSA public key was denied by the remote party's identifier.");

            //Construct an RSA instance and import the remote public key.
            RSACryptoServiceProvider remoteRsa = new RSACryptoServiceProvider();
            remoteRsa.ImportParameters(remoteParameters);

            //Generate an AES key for ourselves. This will be sent to the remote party.
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] localCipherKey = new byte[factory.BlockSize / 8];
            rng.GetBytes(localCipherKey);

            //Encrypt and send the generated AES key using remote party's public key using PKCS#1 v1.5 padding.
            Packet.WriteBytes(binaryWriter, remoteRsa.Encrypt(localCipherKey, false));

            //Read the remote party's generated AES key, recovering the original byte size using the padding.
            byte[] remoteCipherKey = localRsa.Decrypt(Packet.ReadBytes(binaryReader), false);

            //Construct the symmetric key pair and return the value to the upper-level.
            return new SymmetricKeyPair(
                factory.CreateKey(factory.CreateIV(localCipherKey), localCipherKey),
                factory.CreateKey(factory.CreateIV(remoteCipherKey), remoteCipherKey)
            );
        }

        private static RSAParameters ReadPublicParameters(BinaryReader stream)
        {
            RSAParameters parameters = new RSAParameters();
            parameters.Exponent = Packet.ReadBytes(stream);
            parameters.Modulus = Packet.ReadBytes(stream);

            return parameters;
        }

        private static void WritePublicParameters(BinaryWriter stream, RSAParameters parameters)
        {
            Packet.WriteBytes(stream, parameters.Exponent);
            Packet.WriteBytes(stream, parameters.Modulus);
        }
    }
}
