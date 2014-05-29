using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.TCP
{
    public class TcpListener : SocketListener
    {
        private System.Net.Sockets.TcpListener tcpListener;

        public TcpListener(System.Net.Sockets.TcpListener tcpListener)
        {
            this.tcpListener = tcpListener;
        }

        public override System.Net.IPEndPoint GetBindAddress()
        {
            throw new NotImplementedException();
        }

        public override Socket Accept()
        {
            return new TcpSocket(tcpListener.AcceptTcpClient());
        }

        public override void Close()
        {
            tcpListener.Stop();
        }
    }
}
