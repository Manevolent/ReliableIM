using ReliableIM.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network
{
    /// <summary>
    /// Represents a high-level message sender, or a peer capable of sending
    /// messages to the client that calls ReceiveMessage().
    /// </summary>
    public interface IMessageSender
    {
        /// <summary>
        /// Receives a message from this sender. This is a blocking operation,
        /// and may block until a message is received from the sender, or if
        /// the connection is lost.
        /// </summary>
        /// <returns>A received message.</returns>
        IMessage ReceiveMessage();
    }
}
