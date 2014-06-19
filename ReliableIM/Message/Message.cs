using ReliableIM.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Message
{
    public class Message
    {
        private readonly DateTime timestamp;
        private readonly string body;

        public Message(DateTime timestamp, string body)
        {
            this.timestamp = timestamp;
            this.body = body;
        }

        /// <summary>
        /// Gets the time the message was sent. The precision of
        /// this method may vary between different messages.
        /// </summary>
        /// <returns>Message timestamp.</returns>
        public DateTime GetTimestamp()
        {
            return timestamp;
        }

        /// <summary>
        /// Gets the body of this message.
        /// </summary>
        /// <returns>Message body.</returns>
        public string GetBody()
        {
            return body;
        }
    }
}
