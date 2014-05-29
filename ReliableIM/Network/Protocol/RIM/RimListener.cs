using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.RIM
{
    public class RimListener : SocketListener
    {
        private SocketListener baseListener;

        /// <summary>
        /// Creates a new RIM (Reliable IM) socket listener.
        /// </summary>
        /// <param name="baseListener">Base listener to reference.</param>
        public RimListener(SocketListener baseListener)
        {
            this.baseListener = baseListener;
        }

        public override void Close()
        {
            //Nothing to close at this layer.
            baseListener.Close();
        }

        public override System.Net.IPEndPoint GetBindAddress()
        {
            return baseListener.GetBindAddress();
        }

        public override Socket Accept()
        {
            //Accept a client socket from the lower-level listener.
            RimSocket socket = new RimSocket(baseListener.Accept());

            //Authenticate the socket as a peer.
            socket.AuthenticatePeer();

            //Ensure the connection was successful; do not give a broken socket to the upper layer.
            if (!socket.IsConnected())
                throw new Exception("Unknown problem authenticating client.");

            //Return the connected socket.
            return socket;
        }
    }
}
