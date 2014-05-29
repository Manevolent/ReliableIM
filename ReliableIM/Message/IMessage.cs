using ReliableIM.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Message
{
    public interface IMessage
    {
        /// <summary>
        /// Gets the sender of the message.
        /// </summary>
        /// <returns>The origin the message came from.</returns>
        IMessageEndpoint GetSender();
        
        /// <summary>
        /// Gets the time the message was sent. The precision of
        /// this method may vary between different messages.
        /// </summary>
        /// <returns>Message timestamp.</returns>
        DateTime GetTimestamp();

        /// <summary>
        /// Gets the body of this message.
        /// </summary>
        /// <returns>Message body.</returns>
        MessageBody GetBody();
    }
}
