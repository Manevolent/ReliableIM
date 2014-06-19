using ReliableIM.Security.Signature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.RIM
{
    public class RimListener : SocketListener
    {
        private readonly SocketListener baseListener;
        private readonly SignatureAlgorithm signatureAlgorithm;

        /// <summary>
        /// Creates a new RIM (Reliable IM) socket listener.
        /// </summary>
        /// <param name="baseListener">Base listener to reference.</param>
        /// <param name="signatureAlgorithm">Signature algorithm to reference.</param>
        public RimListener(SocketListener baseListener, SignatureAlgorithm signatureAlgorithm)
        {
            this.baseListener = baseListener;
            this.signatureAlgorithm = signatureAlgorithm;
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
            RimSocket socket = new RimSocket(baseListener.Accept(), signatureAlgorithm);

            //Authenticate the socket as a peer.
            socket.AuthenticatePeer();

            //Ensure the connection was successful; do not give a broken socket to the upper layer.
            if (!socket.IsConnected())
                throw new Exception("Unknown problem authenticating client.");

            //Return the connected socket.
            return socket;
        }

        public override void Start()
        {
            baseListener.Start();
        }
    }
}
