using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.Proxy
{
    public interface IProxySocketFactory
    {
        /// <summary>
        /// Creates a socket and connects to an endpoint with it.
        /// </summary>
        /// <param name="hostname">Name of the host to connect to.</param>
        /// <param name="port">Port to connect to.</param>
        /// <returns>Connected proxy socket.</returns>
        Socket CreateSocket(string hostname, int port);
    }
}
