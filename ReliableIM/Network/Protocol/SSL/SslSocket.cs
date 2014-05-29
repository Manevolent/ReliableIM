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
    public class SslSocket : Socket
    {
        private Socket baseSocket;

        private SslStream sslStream;
        private EncryptionPolicy encryptionPolicy;
        private SslAuthenticationListener listener;

        /// <summary>
        /// Creates a new SSL socket, based on a lower-level transport socket.
        /// SSL does not implement reliability, and only provides layer security. Using
        /// UDT or TCP is required at this layer.
        /// </summary>
        /// <param name="baseSocket">Base socket to reference.</param>
        /// <param name="encryptionPolicy">Encryption policy to enforce on connections.</param>
        /// <param name="authenticationListener">SSL authentication listener responsible for checking remote certificates.</param>
        public SslSocket(Socket baseSocket, EncryptionPolicy encryptionPolicy, SslAuthenticationListener authenticationListener)
        {
            this.baseSocket = baseSocket;
            this.encryptionPolicy = encryptionPolicy;
            this.listener = authenticationListener;

            if (baseSocket.IsConnected())
                this.sslStream = new SslStream(
                        baseSocket.GetStream(), 
                        false, 
                        new RemoteCertificateValidationCallback(ValidationCallback),
                        new LocalCertificateSelectionCallback(SelectionCallback),
                        encryptionPolicy
                    );
        }

        /// <summary>
        /// Creates a new SSL socket, based on a lower-level transport socket.
        /// SSL does not implement reliability, and only provides layer security. Using
        /// UDT or TCP is required at this layer.
        /// 
        /// This method is shorthand for constructor SslSocket(Socket, EncryptionPolicy, SslAuthenticationListener).
        /// Creating SSL sockets with this contructor will initialize them with the policy
        /// RequireEncryption, and will use the standardized SSL authentication specification.
        /// </summary>
        /// <param name="baseSocket">Base socket to reference.</param>
        public SslSocket(Socket baseSocket) : this (
                baseSocket,
                EncryptionPolicy.RequireEncryption,
                new DefaultSslAuthenticationListener()
            )
        {
            //Do nothing.
        }

        //Bridge delegate to handle validation:
        private bool ValidationCallback (
                object sender, 
                X509Certificate certificate, 
                X509Chain chain, 
                SslPolicyErrors sslPolicyErrors
            )
        {
            //Default to the listener to provide validation.
            return listener.Authenticate(certificate, chain, sslPolicyErrors);
        }

        //Bridge delegate to handle local certificate selection:
        private X509Certificate SelectionCallback(
                object sender,
                string targetHost,
                X509CertificateCollection localCertificates,
                X509Certificate remoteCertificate,
                string[] acceptableIssuers
            )
        {
            return localCertificates.Count > 0 ? localCertificates[0] : null;
        }

        public override bool IsConnected()
        {
            return baseSocket.IsConnected() && sslStream != null && sslStream.IsAuthenticated;
        }

        public override void Connect(System.Net.IPEndPoint endpoint)
        {
            //Connect the base socket to the endpoint
            baseSocket.Connect(endpoint);

            //Wrap a new SSL stream around the connection.
            sslStream = new SslStream(
                        baseSocket.GetStream(),
                        false,
                        new RemoteCertificateValidationCallback(ValidationCallback),
                        new LocalCertificateSelectionCallback(SelectionCallback),
                        encryptionPolicy
                    );

            //Authenticate as a client. The SslListener class should authenticate the socket as a server.
            AuthenticateAsClient(endpoint.Address.ToString());
        }

        /// <summary>
        /// Authenticates this SSL socket as a client.
        /// </summary>
        /// <param name="host">Host that is being connected to.</param>
        public void AuthenticateAsClient(string host)
        {
            if (!baseSocket.IsConnected())
                throw new InvalidOperationException("Cannot authenticate SSL because the underlying socket is not connected.");

            sslStream.AuthenticateAsClient(host);
        }

        /// <summary>
        /// Authenticates this SSL socket as a server.
        /// </summary>
        /// <param name="serverCertificate">Server certificate to use for authentication.</param>
        public void AuthenticateAsServer(X509Certificate serverCertificate)
        {
            if (!baseSocket.IsConnected())
                throw new InvalidOperationException("Cannot authenticate SSL because the underlying socket is not connected.");

            sslStream.AuthenticateAsServer(serverCertificate);
        }

        public override System.IO.Stream GetStream()
        {
            return sslStream;
        }

        public override void Close()
        {
            sslStream.Close();
        }
    }
}
