using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.UDT
{
    public class UdtListener : SocketListener
    {
        private Udt.Socket udtSocket;

        public UdtListener(Udt.Socket udtSocket)
        {
            //Link the UDT socket given in the constructor to the class' instance of it:
            this.udtSocket = udtSocket;
        }

        public UdtListener(IPEndPoint bindAddress) : this (
                new Udt.Socket(
                    System.Net.Sockets.AddressFamily.InterNetwork, //IPv4
                    System.Net.Sockets.SocketType.Stream //UDP
                )
            )
        {
            //Do nothing
            udtSocket.Bind(bindAddress);
        }

        public UdtListener(int port)
            : this(new IPEndPoint(IPAddress.Any, port))
        {
            //Do nothing
        }

        public override System.Net.IPEndPoint GetBindAddress()
        {
            return udtSocket.LocalEndPoint;
        }

        public override Socket Accept()
        {
            if (udtSocket.State != Udt.SocketState.Listening)
                throw new Exception("UDT socket is not in a listening state.");
            
            return new UdtSocket(udtSocket.Accept());
        }

        public override void Close()
        {
            udtSocket.Close();
        }

        public override void Start()
        {
            udtSocket.Listen(10);
        }
    }
}
