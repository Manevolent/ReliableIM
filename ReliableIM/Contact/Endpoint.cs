using ReliableIM.Network;
using ReliableIM.Security.Signature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Contact
{
    /// <summary>
    /// Represents the basic endpoint structure in the network topology.
    /// </summary>
    public abstract class Endpoint
    {
        private readonly Identity identity;
        private string displayName;

        /// <summary>
        /// Constructs a new endpoint instance.
        /// </summary>
        /// <param name="identity">Endpoint identity.</param>
        /// <param name="displayName">Display name of the endpoint.</param>
        public Endpoint(Identity identity, string displayName)
        {
            this.identity = identity;
            this.displayName = displayName;
        }

        /// <summary>
        /// Constructs a new endpoint instance.
        /// </summary>
        /// <param name="identity">Endpoint identity.</param>
        public Endpoint(Identity identity) : this(identity, identity.GetFingerprintString())
        {
            //Do nothing.
        }

        /// <summary>
        /// Constructs a new endpoint instance.
        /// </summary>
        /// <param name="signatureAlgorithm">Signature algorithm of the endpoint.</param>
        public Endpoint(SignatureAlgorithm signatureAlgorithm)
            : this(signatureAlgorithm.Identity)
        {
            //Do nothing.
        }

        /// <summary>
        /// Gets the network fingerprint of this endpoint. This is
        /// used to identify the endpoint across multiple users and
        /// sessions.
        /// </summary>
        /// <returns>Network fingerprint.</returns>
        public Identity Identity
        {
            get
            {
                return identity;
            }
        }

        /// <summary>
        /// Gets or sets the current display name of this endpoint. This is
        /// determined by the remote setting of the contact or group.
        /// </summary>
        /// <returns>Display name.</returns>
        public string DisplayName
        {
            get
            {
                return displayName;
            }
            set
            {
                displayName = value;
            }
        }

        /// <summary>
        /// Sends a signature to the endpoint.
        /// </summary>
        /// <param name="signature">Signature to send.</param>
        public abstract void SendSignature(Signature signature);

        /// <summary>
        /// Finds if a sender's identity is valid for this endpoint. This is used to verify
        /// a message's From address, where the To address matches this endpoint.
        /// 
        /// For contacts, this message will always check to be sure the identity matches
        /// the contact's identity. For groups, the identity must match another members'.
        /// </summary>
        /// <param name="identity">Identity of the sender.</param>
        /// <returns>True if the sender is valid, false otherwise.</returns>
        public abstract bool IsValidSender(Identity identity);

        /// <summary>
        /// Connects to this endpoint.
        /// </summary>
        public abstract void Connect();

        public void SendSignature(Message.Message message)
        {
            throw new NotImplementedException();
        }
    }
}
