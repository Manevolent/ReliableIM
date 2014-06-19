using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Contact
{
    public class Group : Endpoint
    {
        private readonly Contact owner;
        private readonly List<Contact> members;

        public Group(Contact owner, List<Contact> members, string fingerprint, string displayName) : base(fingerprint, displayName)
        {
            this.owner = owner;
            this.members = members;
        }

        /// <summary>
        /// Gets the owner of this group.
        /// </summary>
        /// <returns>Group owner.</returns>
        public Contact Owner
        {
            get
            {
                return owner;
            }
        }

        /// <summary>
        /// Gets the list of members in the group. The current client
        /// and owner are also included.
        /// </summary>
        /// <returns>Member list.</returns>
        public List<Contact> Members
        {
            get
            {
                return members;
            }
        }

        public override void SendMessage(Message.Message message)
        {
            foreach (Contact contact in members)
                contact.SendMessage(message);
        }

        public override Message.Message ReceiveMessage()
        {
            throw new NotImplementedException();
        }

        public override void Connect()
        {
            foreach (Contact contact in members)
                try
                {
                    contact.Connect();
                }
                catch (Exception ex)
                {
                    //Do nothing.
                }
        }
    }
}
