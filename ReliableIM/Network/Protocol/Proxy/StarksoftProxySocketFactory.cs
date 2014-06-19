using ReliableIM.Network.Protocol.TCP;
using Starksoft.Net.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.Proxy
{
    public sealed class StarksoftProxySocketFactory : IProxySocketFactory
    {
        private readonly IProxyClient client;

        public StarksoftProxySocketFactory(IProxyClient client)
        {
            this.client = client;
        }

        public Socket CreateSocket(string hostname, int port)
        {
            return new TcpSocket(client.CreateConnection(hostname, port));
        }
    }
}
