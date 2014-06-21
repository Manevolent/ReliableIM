using ReliableIM.Network;
using ReliableIM.Network.Protocol;
using ReliableIM.Network.Protocol.RIM.Packet;
using ReliableIM.Security.Signature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Contact
{
    public class Contact : Endpoint
    {
        private readonly SignatureAlgorithm signatureAlgorithm;
        private readonly IConnectable connectable;

        private IPacketStream packetStream = null;
        private ContactStatus contactStatus = ContactStatus.Unknown;

        /// <summary>
        /// Constructs a new Contact.
        /// </summary>
        /// <param name="signatureAlgorithm">Signature algorithm used to identify the contact.</param>
        /// <param name="connectable">A connectable instance used to connect to the contact.</param>
        public Contact(SignatureAlgorithm signatureAlgorithm, IConnectable connectable) : base (signatureAlgorithm)
        {
            this.signatureAlgorithm = signatureAlgorithm;
            this.connectable = connectable;
        }
        
        /// <summary>
        /// Gets or sets the packet stream used to communicate over a network with this contact.
        /// </summary>
        public IPacketStream PacketStream
        {
            get
            {
                return packetStream;
            }
            set
            {
                packetStream = value;
            }
        }

        /// <summary>
        /// Gets the network online status of this contact.
        /// </summary>
        /// <returns>
        /// True if the sender is currently online, and available to recieve messages, 
        /// regardless of the contact's online status (i.e. invisible)
        /// </returns>
        public bool IsConnected()
        {
            return packetStream != null && packetStream.IsConnected();
        }

        /// <summary>
        /// Gets or sets the contact status this contact is currently using.
        /// </summary>
        public ContactStatus Status
        {
            get
            {
                if (!IsConnected())
                    return ContactStatus.Unknown;

                return contactStatus;
            }
            set
            {
                contactStatus = value;
            }
        }

        /// <summary>
        /// Gets the signature algorithm this contact is currently using.
        /// </summary>
        /// <returns></returns>
        public SignatureAlgorithm SignatureAlgorithm
        {
            get
            {
                return signatureAlgorithm;
            }
        }

        public override void SendSignature(Signature signature)
        {
            if (IsConnected())
            {
                PacketStream.WritePacket(new Packet4Signature(signature));
            }
            else
            {
                //Back-buffer (delayed messages).
            }
        }

        public override void Connect()
        {
            if (IsConnected())
                return; //No need to connect.

            //Make a request for connection at the lower level.
            PacketStream = connectable.Connect();
        }

        public override bool IsValidSender(Identity identity)
        {
            return Identity.Equals(identity);
        }
    }
}
