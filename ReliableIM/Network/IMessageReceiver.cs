using ReliableIM.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network
{
    /// <summary>
    /// Represents a high-level message receiver, or a peer capable of receiving
    /// sent messages. 
    /// </summary>
    public interface IMessageReceiver
    {
        /// <summary>
        /// Sends a message to the receiver. A proper message with from and to
        /// headers will be sent with the message body, depending on how we are
        /// connected to this receiver.
        /// </summary>
        /// <param name="message">Message body to send.</param>
        public void SendMessage(MessageBody message);
    }
}
