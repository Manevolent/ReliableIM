using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.UDT
{
    public class UdtSocket : Socket
    {
        private Udt.Socket udtSocket;
        private Udt.NetworkStream networkStream;
        
        /// <summary>
        /// References another basic UDT socket.
        /// </summary>
        /// <param name="udtSocket">UDT socket to reference.</param>
        public UdtSocket(Udt.Socket udtSocket)
        {
            //Link the UDT socket given in the constructor to the class' instance of it:
            this.udtSocket = udtSocket;

            //Wrap a new UDT network stream around the socket given:
            this.networkStream = new Udt.NetworkStream(udtSocket);
        }

        /// <summary>
        /// Creates a new UDP over UDP  socket.
        /// </summary>
        public UdtSocket() : this (
                new Udt.Socket(
                    System.Net.Sockets.AddressFamily.InterNetwork, //IPv4
                    System.Net.Sockets.SocketType.Stream //UDP
                )
            )
        {
            //Do nothing.
        }

        /// <summary>
        /// Creates a new UDT over UDP socket, and binds the socket to the address given.
        /// </summary>
        /// <param name="endpoint">Endpoint to bind to.</param>
        public UdtSocket(IPEndPoint bindAddress) : this()
        {
            //Connect to the endpoint given.
            udtSocket.Bind(bindAddress);
        }

        public override bool IsConnected()
        {
            //From what I can tell, what we're looking for is the "Connected" state.
            //There appear to be others, however.
            return udtSocket.State == Udt.SocketState.Connected;
        }

        public override void Connect(System.Net.IPEndPoint endpoint)
        {
            if (udtSocket.State != Udt.SocketState.Open && 
                udtSocket.State != Udt.SocketState.Initial)
                throw new Exception("UDT socket not ready to connect.");

            //Connect this layer to the desired endpoint. UDT should handle this for us.
            udtSocket.Connect(endpoint);

            if (udtSocket.State != Udt.SocketState.Connected)
                throw new Exception("UDT connect failed.");

            //Wrap a new stream around the newly connected socket.
            networkStream = new Udt.NetworkStream(udtSocket);
        }

        public override System.IO.Stream GetStream()
        {
            return networkStream;
        }

        public override void Close()
        {
            //Close the UDT socket, also closing the UDP/TCP socket and any buffers.
            networkStream.Flush();
            networkStream.Close();
        }
    }
}
