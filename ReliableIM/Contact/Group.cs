using ReliableIM.Security.Signature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Contact
{
    public sealed class Group : Endpoint
    {
        private readonly Contact owner;
        private readonly List<Contact> members;

        /// <summary>
        /// Constructs a new Group.
        /// </summary>
        /// <param name="owner">Owner of the group.</param>
        /// <param name="members">Group member list.</param>
        /// <param name="identity">Group identity.</param>
        public Group(Contact owner, List<Contact> members, Identity identity) : base(identity)
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

        public override void SendSignature(Signature signature)
        {
            foreach (Contact contact in members)
                contact.SendSignature(signature);
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

        public override bool IsValidSender(Identity identity)
        {
            foreach (Contact contact in members)
                if (contact.Identity.Equals(identity))
                    return true;

            return false;
        }
    }
}
