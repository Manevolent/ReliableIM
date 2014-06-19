using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol
{
    /// <summary>
    /// Provides a managed wrapper around a socket listener, or a socket in a
    /// server-state. Different implementations of a socket listener will perform
    /// differently and may include their own protocols at its level.
    /// </summary>
    public abstract class SocketListener
    {
        /// <summary>
        /// Gets the address this server socket is bound to.
        /// </summary>
        /// <returns>Bind address.</returns>
        public abstract IPEndPoint GetBindAddress();

        /// <summary>
        /// Accepts a new socket of the listener's type. This method will block until
        /// a socket is accepted.
        /// </summary>
        /// <returns>Accepted socket.</returns>
        public abstract Socket Accept();

        /// <summary>
        /// Closes the listener.
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Starts the listener.
        /// </summary>
        public abstract void Start();
    }
}
