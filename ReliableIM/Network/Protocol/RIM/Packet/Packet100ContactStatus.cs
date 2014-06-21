using ReliableIM.Contact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.RIM.Packet
{
    public sealed class Packet100ContactStatus : Network.Protocol.Packet
    {
        private ContactStatus contactStatus;

        public Packet100ContactStatus(ContactStatus contactStatus)
        {
            this.contactStatus = contactStatus;
        }

        public Packet100ContactStatus()
            : this(ContactStatus.Unknown)
        {
            //Do nothing.
        }

        public ContactStatus Status
        {
            get
            {
                return contactStatus;
            }
        }

        public override byte GetPacketID()
        {
            return 100;
        }

        public override void Write(System.IO.BinaryWriter stream)
        {
            switch (contactStatus)
            {
                case ContactStatus.Online:
                    stream.Write((byte)1);
                    break;
                case ContactStatus.Busy:
                    stream.Write((byte)2);
                    break;
                case ContactStatus.Invisible:
                    stream.Write((byte)3);
                    break;
                default:
                    stream.Write((byte)0);
                    break;
            }
        }

        public override void Read(System.IO.BinaryReader stream)
        {
            byte contactStatusId = stream.ReadByte();
            switch (contactStatusId)
            {
                case 1:
                    contactStatus = ContactStatus.Online;
                    break;
                case 2:
                    contactStatus = ContactStatus.Busy;
                    break;
                case 3:
                    contactStatus = ContactStatus.Invisible;
                    break;
                default:
                    contactStatus = ContactStatus.Unknown;
                    break;
            }
        }
    }
}
