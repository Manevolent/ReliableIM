using ReliableIM.Network.Protocol;
using ReliableIM.Security.Symmetric;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Exchange.DH
{
    public sealed class ECDHKeyExchangeAlgorithm : KeyExchangeAlgorithm
    {
        private readonly int keySize;
        private readonly CngAlgorithm symmetricKeyHashAlgorithm;
        private readonly SymmetricAlgorithmFactory factory;

        public ECDHKeyExchangeAlgorithm(int keySize, CngAlgorithm symmetricKeyHashAlgorithm, SymmetricAlgorithmFactory factory)
        {
            this.factory = factory;
            this.keySize = keySize;
            this.symmetricKeyHashAlgorithm = symmetricKeyHashAlgorithm;
        }

        public override SymmetricKeyPair Exchange(System.IO.Stream stream)
        {
            //Construct a binary stream around the underlying stream.
            BinaryWriter binaryWriter = new BinaryWriter(stream);
            BinaryReader binaryReader = new BinaryReader(stream);

            //Generate an elliptic-curve Diffie-Hellman key for private use.
            ECDiffieHellmanCng local = new ECDiffieHellmanCng(keySize);
            local.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
            local.HashAlgorithm = symmetricKeyHashAlgorithm;

            //Send our ECDH public key to the other party.
            Packet.WriteBytes(binaryWriter, local.PublicKey.ToByteArray());
            binaryWriter.Flush();

            //Recieve the ECDH public key from the other party.
             ECDiffieHellmanCng remote = new ECDiffieHellmanCng(
                    CngKey.Import(
                        Packet.ReadBytes(binaryReader),
                        CngKeyBlobFormat.EccPublicBlob
                    )
            );

            //Compute the shared secret using the ECDH keys transmitted.
            byte[] sharedSecret = local.DeriveKeyMaterial(remote.PublicKey);
            SymmetricKey sharedKey = factory.CreateKey(factory.CreateIV(sharedSecret), sharedSecret);

            //Return a keypair to the upper level, using the shared key for both directions.
            return new SymmetricKeyPair(sharedKey, sharedKey);
        }
    }
}
