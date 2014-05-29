using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.TCP
{
    public class TcpSocket : Socket
    {
        private System.Net.Sockets.TcpClient tcpClient;

        /// <summary>
        /// Creates a new managed TCP socket around the given native TcpClient instance.
        /// </summary>
        /// <param name="tcpClient">TcpClient to reference.</param>
        public TcpSocket(System.Net.Sockets.TcpClient tcpClient)
        {
            this.tcpClient = tcpClient;
        }

        /// <summary>
        /// Creates a new managed TCP socket on a random port.
        /// </summary>
        public TcpSocket() : this (new System.Net.Sockets.TcpClient())
        {
            //Do nothing.
        }

        public override bool IsConnected()
        {
            return tcpClient.Connected;
        }

        public override void Connect(System.Net.IPEndPoint endpoint)
        {
            tcpClient.Connect(endpoint);
        }

        public override System.IO.Stream GetStream()
        {
            return tcpClient.GetStream();
        }

        public override void Close()
        {
            tcpClient.Close();
        }
    }
}
