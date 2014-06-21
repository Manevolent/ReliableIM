using ReliableIM.Network.Protocol.Proxy;
using ReliableIM.Network.Protocol.RIM;
using ReliableIM.Network.Protocol.SSL;
using ReliableIM.Network.Protocol.SSL.Listener;
using ReliableIM.Network.Protocol.TCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol
{
    public sealed class ConnectableProxySocket : IConnectable
    {
        private readonly static ushort DEFAULT_SERVICE_PORT = 1099;

        private readonly IProxySocketFactory proxySocket;
        private readonly string hostname;
        private readonly string fingerprint;

        public ConnectableProxySocket(IProxySocketFactory proxySocket, string hostname, string fingerprint) {
            this.proxySocket = proxySocket;
            this.hostname = hostname;
            this.fingerprint = fingerprint;
        }

        public IPacketStream Connect()
        {
            throw new NotSupportedException();
        }
    }
}
