using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol
{
    public abstract class Socket
    {

        /// <summary>
        /// Finds connectivity status.
        /// </summary>
        /// <returns>True if the socket is connected.</returns>
        public abstract bool IsConnected();

        /// <summary>
        /// Connects to an endpoint.
        /// </summary>
        /// <param name="endpoint"></param>
        public abstract void Connect(IPEndPoint endpoint);

        /// <summary>
        /// Gets the stream associated with the socket.
        /// </summary>
        /// <returns>Socket stream.</returns>
        public abstract Stream GetStream();

        /// <summary>
        /// Closes and disposes of the socket.
        /// </summary>
        public abstract void Close();

    }
}
