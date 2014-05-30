using ReliableIM.Network.Protocol.SSL.Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.SSL
{
    public class SslListener : SocketListener
    {
        private SocketListener baseListener;
        private X509Certificate serverCertificate;
        private EncryptionPolicy encryptionPolicy;
        private SslAuthenticationListener authenticationListener;

        public SslListener(SocketListener baseListener, X509Certificate serverCertificate, EncryptionPolicy encryptionPolicy, SslAuthenticationListener listener)
        {
            //The listener is used to accept a socket capable of basic data transport, e.g. UDT.
            this.baseListener = baseListener;

            //The certificate is used in the AuthenticateAsServer SSL method.
            this.serverCertificate = serverCertificate;

            //Set the listener and encryption policy.
            this.encryptionPolicy = encryptionPolicy;
            this.authenticationListener = listener;
        }

        public SslListener(SocketListener baseListener, X509Certificate serverCertificate)
            : this(
                baseListener,
                serverCertificate,
                EncryptionPolicy.RequireEncryption,
                new DefaultSslAuthenticationListener()
            )
        {
            //Do nothing.
        }

        public override System.Net.IPEndPoint GetBindAddress()
        {
            return baseListener.GetBindAddress();
        }

        public override Socket Accept()
        {
            //Accept a lower-level socket, and wrap it into an SslSocket instance for management.
            SslSocket socket = new SslSocket(baseListener.Accept(), encryptionPolicy, authenticationListener);

            //Authenticate the remote endpoint with ourselves assuming the role of a server.
            socket.AuthenticateAsServer(serverCertificate);

            //Ensure the connection was successful; do not give a broken socket to the upper layer.
            if (!socket.IsConnected())
                throw new Exception("Unknown problem authenticating client.");

            //Return the connected socket.
            return socket;
        }

        public override void Close()
        {
            //Nothing to close at this layer; Ssl acts transparently at the stream level.
            baseListener.Close();
        }

        public override void Start()
        {
            baseListener.Start();
        }
    }
}
