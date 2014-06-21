using ReliableIM.Network.Protocol.RIM.Handler.Authorized;
using ReliableIM.Network.Protocol.RIM.Packet;
using ReliableIM.Security.Signature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.RIM.Handler.Unauthorized
{
    public class RimPacketHandlerUnauthorized : RimPacketHandler
    {
        /// <summary>
        /// The signature algorithm used to verify our identity.
        /// </summary>
        private readonly SignatureAlgorithm signatureAlgorithm;

        /// <summary>
        /// The identity verifier used to verify the remote identity.
        /// </summary>
        private readonly IIdentityVerifier identityVerifier;

        /// <summary>
        /// The salt used to verify the endpoint's identity.
        /// </summary>
        private readonly byte[] salt;

        /// <summary>
        /// A flag set to indicate the endpoint's identity has been requested.
        /// </summary>
        private bool requestedIdentity;

        /// <summary>
        /// A flag set to indicate the endpoint's identity has been received.
        /// </summary>
        private bool receivedIdentity;

        /// <summary>
        /// Constructs a new unauthorized RIM packet handler.
        /// </summary>
        /// <param name="packetStream">A packet stream to respond to handled packets with.</param>
        /// <param name="signatureAlgorithm">The local signature algorithm capable of creating signatures.</param>
        /// <param name="identityVerifier">An identity verifier used to verify the identity of the remote peer.</param>
        public RimPacketHandlerUnauthorized(IPacketStream packetStream, SignatureAlgorithm signatureAlgorithm, IIdentityVerifier identityVerifier) : base(packetStream)
        {
            if (!signatureAlgorithm.CanSign)
                throw new ArgumentException("Signature algorithm is not capable of signing messages.", "signatureAlgorithm");

            this.signatureAlgorithm = signatureAlgorithm;

            this.identityVerifier = identityVerifier;

            //Generate a 512-bit salt to verify the endpoint.
            salt = new byte[64];
            new Random().NextBytes(salt);

            //Write a connection packet to the endpoint.
            requestedIdentity = true;
            Stream.WritePacket(new Packet2IdentityRequest(1, salt));
        }

        protected override void HandleIdentityRequest(Packet2IdentityRequest request)
        {
            //Ensure this version is supported.
            if (request.ProtocolVersion != 1)
            {
                Disconnect(Packet255Disconnect.DisconnectReason.UnsupportedVersion);
                return;
            }

            //Respond to the request for identity.
            Stream.WritePacket(request.CreateResponse(signatureAlgorithm));
        }

        protected override void HandleIdentityResponse(Packet3IdentityResponse response)
        {
            //Ensure the identity has not already been received, and this is not duplicate.
            if (receivedIdentity)
            {
                Disconnect(Packet255Disconnect.DisconnectReason.UnexpectedPacket);
                return;
            }

            //Set the identity received flag to true to prevent future identities from being read.
            receivedIdentity = true;

            //Ensure the signed data matches our salt, and we've sent a request.
            if (!requestedIdentity || salt == null || !response.Signature.Data.SequenceEqual(salt))
            {
                Disconnect(Packet255Disconnect.DisconnectReason.AuthenticationFailed);
                return;
            }

            //Ensure the identity is not ours, which would mean we're connecting to ourselves.
            if (response.SignatureAlgorithm.Identity.Equals(signatureAlgorithm.Identity))
            {
                Disconnect(Packet255Disconnect.DisconnectReason.AuthenticationFailed);
                return;
            }

            //Ensure the identity matches the expected identity.
            if (!identityVerifier.VerifyIdentity(response.SignatureAlgorithm.Identity))
            {
                Disconnect(Packet255Disconnect.DisconnectReason.AuthenticationFailed);
                return;
            }

            //Ensure the signature created by the remote party is valid.
            if (!response.SignatureAlgorithm.Verify(response.Signature))
            {
                Disconnect(Packet255Disconnect.DisconnectReason.AuthenticationFailed);
                return;
            }

            /*
             * ================== WARNING ==================
             * 
             * Beyond this point, the peer is trusted to be
             * a valid recipient of data destined to his
             * identity (see: Identity class). The actual
             * authentication process occurs in the
             * implementation of the signature algorithm the
             * endpoint has chosen, which may be faulty or
             * have a vulnerability. It is up to the peer who
             * requests the identity to ensure the algorithm
             * chosen by the endpoint is safe to use for
             * identity authentication.
             * 
             * Should an issue be discovered in a signature
             * algorithm, when used it should always throw a
             * SecurityException, detailing the vulnerability
             * present by using its standard for
             * authentication. This exception will be caught
             * by the PacketHandler processing the handshake
             * and by specification should disconnect.
             * 
             * =============================================
            */

            //Initialize the new authorized handler.
            Stream.PacketHandler = new RimPacketHandlerAuthorized(Stream, new Contact.Contact(response.SignatureAlgorithm, null), signatureAlgorithm);
        }
    }
}
