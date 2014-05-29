using ReliableIM.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Contact
{
    public interface IContact : IMessageEndpoint
    {
        /// <summary>
        /// Gets the network username of this contact.
        /// </summary>
        /// <returns>Network username.</returns>
        string GetUsername();

        /// <summary>
        /// Gets the current client-set display name of this contact.
        /// </summary>
        /// <returns>Display name.</returns>
        string GetDisplayName();

        /// <summary>
        /// Finds the network online status of this contact.
        /// </summary>
        /// <returns>
        /// True if the sender is currently online, and available to recieve messages, 
        /// regardless of the contact's online status (i.e. invisible)
        /// </returns>
        bool IsConnected();

        /// <summary>
        /// Finds the contact status this contact is currently using. Typically,
        /// UNKNOWN is returned if IsConnected() proves false.
        /// </summary>
        /// <returns>Contact status.</returns>
        EnumContactStatus GetStatus();
    }
}
