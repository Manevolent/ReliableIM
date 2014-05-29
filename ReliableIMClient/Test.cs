using ReliableIM.Network.Protocol.SSL;
using ReliableIM.Network.Protocol.SSL.Listener;
using ReliableIM.Network.Protocol.TCP;
using ReliableIM.Network.Protocol.UDT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIMClient
{
    static class Test
    {
        /// <summary>
        /// Test entry point.
        /// </summary>
        [STAThread]
        static void Main()
        {
            SslSocket socket = new SslSocket(
                    new TcpSocket(), 
                    EncryptionPolicy.AllowNoEncryption,
                    new BasicSslAuthenticationListener()
                );

            socket.Connect(new IPEndPoint(IPAddress.Parse("74.125.226.7"), 443));
        }
    }
}
