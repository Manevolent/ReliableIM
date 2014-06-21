using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Security.Signature.DSA
{
    public sealed class DSASignatureAlgorithm : SignatureAlgorithm
    {
        private readonly SHA1Managed hashAlgorithm = new SHA1Managed();

        private System.Security.Cryptography.DSA dsa;
        private DSAIdentity identity;

        public DSASignatureAlgorithm(System.Security.Cryptography.DSA dsa)
        {
            InitializeAlgorithm(dsa);
        }

        public DSASignatureAlgorithm()
        {
            //Do nothing.
        }

        /// <summary>
        /// Initializes this instance of an DSA signature algorithm.
        /// </summary>
        /// <param name="dsa">DSA cryptographic service provider to use.</param>
        private void InitializeAlgorithm(System.Security.Cryptography.DSA dsa)
        {
            this.dsa = dsa;

            //Compute the identity.
            DSAParameters parameters = dsa.ExportParameters(false);
            this.identity = new DSAIdentity(parameters);
        }

        public override Signature Sign(byte[] data)
        {
            //Synchronize around the hash algorithm to prevent collisions.
            lock(hashAlgorithm)
            {
                //Initialize the hash algorithm.
                hashAlgorithm.Initialize();

                //Sign the data, providing the identity of the signer and the original data.
                return new DSASignature(data, identity, dsa.CreateSignature(hashAlgorithm.ComputeHash(data)));
            }
        }

        public override bool CanSign
        {
            get
            {
                return true;
            }
        }

        public override bool Verify(Signature signature)
        {
            //Verify the signature's type is an RSA signature.
            if (!(signature is DSASignature))
                throw new SecurityException("Signature is not an DSA signature.");

            //Verify the identity of the signature matches this signature algorithm.
            if (!signature.Identity.Equals(identity))
                throw new SecurityException("Signature's identity does not match.");

            //Synchronize around the hash algorithm to prevent collisions.
            lock (hashAlgorithm)
            {
                //Initialize the hash algorithm.
                hashAlgorithm.Initialize();

                //Verify the signature, re-computing the hash and supplying the given signature array.
                return dsa.VerifySignature(hashAlgorithm.ComputeHash(signature.Data), ((DSASignature)signature).Signature);
            }
        }

        public override IIdentityVerifier CreateIdentityVerifier()
        {
            return new BinaryIdentityVerifier(identity);
        }

        public override Identity Identity
        {
            get
            {
                return identity;
            }
        }

        public override byte GetPacketID()
        {
            return 2;
        }

        public override string Name
        {
            get
            {
                return "DSA";
            }
        }

        public override void Write(System.IO.BinaryWriter stream)
        {
            //Write P, Q, G, and Y.
            DSAParameters parameters = dsa.ExportParameters(false);

            WriteBytes(stream, parameters.P);
            WriteBytes(stream, parameters.Q);
            WriteBytes(stream, parameters.G);
            WriteBytes(stream, parameters.Y);
        }

        public override void Read(System.IO.BinaryReader stream)
        {
            //Read P, Q, G, and Y.
            DSAParameters parameters = new DSAParameters();
            parameters.P = ReadBytes(stream);
            parameters.Q = ReadBytes(stream);
            parameters.G = ReadBytes(stream);
            parameters.Y = ReadBytes(stream);

            //Set up the new DSA provider.
            DSACryptoServiceProvider dsa = new System.Security.Cryptography.DSACryptoServiceProvider();
            dsa.ImportParameters(parameters);

            //Initialize the algorithm.
            InitializeAlgorithm(dsa);
        }
    }
}
