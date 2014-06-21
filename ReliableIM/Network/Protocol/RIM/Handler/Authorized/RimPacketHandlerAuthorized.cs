using ReliableIM.Network.Protocol.RIM.Packet;
using ReliableIM.Network.Protocol.RIM.Packet.Signed;
using ReliableIM.Security.Signature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.RIM.Handler.Authorized
{
    public class RimPacketHandlerAuthorized : RimPacketHandler
    {
        private static readonly PacketFactory SIGNED_FACTORY = CreateSignedFactory();
        public static PacketFactory CreateSignedFactory()
        {
            PacketFactory factory = new PacketFactory();

            factory.RegisterPacket(1, typeof(Packet1Message));

            return factory;
        }

        /// <summary>
        /// A reference to the contact this packet handler is responsible for.
        /// </summary>
        private Contact.Contact contact;

        public RimPacketHandlerAuthorized(IPacketStream packetStream, Contact.Contact contact, SignatureAlgorithm localSignatureAlgorithm) : base(packetStream)
        {
            this.contact = contact;

            Console.WriteLine("Connected to " + contact.Identity.GetFingerprintString() + ".");

            Stream.WritePacket(new Packet4Signature(new Packet1Message(contact.Identity, "Kekkoning is stronk!").ToSignature(localSignatureAlgorithm)));
        }

        protected override void HandleSignature(Packet4Signature signature)
        {
            //Find an appropriate signature algorithm to use with this signature.
            SignatureAlgorithm selectedAlgorithm = contact.SignatureAlgorithm;

            //Read the packet contents (This is an important process. At this point, we validate the sender.)
            PacketSigned packet = PacketSigned.FromSignature(
                signature.Signature,
                contact.Identity,
                selectedAlgorithm,
                SIGNED_FACTORY
            );
            
            //Warning: Sender is now verified and trusted.

            //Handle the packet:
            switch (packet.GetPacketID())
            {
                case 1:
                    HandleMessage((Packet1Message)packet);
                    break;
                default:
                    throw new Exception("Unknown signed packet ID: " + packet.GetPacketID());
            }
        }

//
        private void HandleMessage(Packet1Message message)
        {
            
        }
//

        protected override void HandleContactStatus(Packet100ContactStatus contactStatus)
        {
            contact.Status = contactStatus.Status;
        }
    }
}
