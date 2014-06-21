using OpenSSL.Core;
using OpenSSL.X509;
using ReliableIM.Network.Protocol;
using ReliableIM.Network.Protocol.ESL;
using ReliableIM.Network.Protocol.GZIP;
using ReliableIM.Network.Protocol.RIM;
using ReliableIM.Network.Protocol.SSL;
using ReliableIM.Network.Protocol.TCP;
using ReliableIM.Network.Protocol.UDT;
using ReliableIM.Security.Certificate;
using ReliableIM.Security.Exchange.DH;
using ReliableIM.Security.Exchange.RSA;
using ReliableIM.Security.Signature;
using ReliableIM.Security.Signature.DSA;
using ReliableIM.Security.Signature.RSA;
using ReliableIM.Security.Symmetric.AES;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReliableIMClient
{
    static class Program
    {
        private static IPEndPoint serverAddress;
        private static bool started;

        private static readonly RSASignatureAlgorithm client =
                    new RSASignatureAlgorithm(
                        new RSACryptoServiceProvider(),
                        new SHA1Managed()
                    );

        private static readonly RSASignatureAlgorithm server =
                    new RSASignatureAlgorithm(
                        new RSACryptoServiceProvider(),
                        new SHA1Managed()
                    );
        
        static void ServerThread()
        {
            Console.WriteLine("SERVER: Starting...");
            Console.WriteLine(Application.ExecutablePath);

            RimListener listener = new RimListener(
                new GZipListener(
                    new EncryptedSocketListener(
                        new TcpListener(serverAddress),
                        new RSAKeyExchangeAlgorithm(server.RSA, new AnonymousIdentityVerifier(), new AESAlgorithmFactory(256, CipherMode.CBC, PaddingMode.PKCS7))
                    ),
                    CompressionLevel.Optimal
                ),
                server,
                new AnonymousIdentityVerifier()
            );

            listener.Start();

            started = true;

            Console.WriteLine("SERVER: Listener started. Accepting connections.");

            while (true)
            {
                Socket socket = listener.Accept();

                Console.WriteLine("SERVER: Client accepted.");

                while (socket.IsConnected());

                Console.WriteLine("SERVER: Disconnected.");
            }
        }

        /// <summary>
        /// The main entry point for the graphical user-interface.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Setup server address.
            serverAddress = new IPEndPoint(IPAddress.Loopback, 1099);

            //Start server.
            Thread thread = new Thread(ServerThread);
            thread.Start();
            while (!started) ;

            IIdentityVerifier serverIdentityVerifier = new BinaryIdentityVerifier((BinaryIdentity)server.Identity);

            for (int i = 0; i < 20; i++)
            {
                //Create client and connect.
                RimSocket clientSocket = new RimSocket(
                    new GZipSocket(
                        new EncryptedSocket(
                            new TcpSocket(),
                            new RSAKeyExchangeAlgorithm(client.RSA, serverIdentityVerifier, new AESAlgorithmFactory(256, CipherMode.CBC, PaddingMode.PKCS7))
                        ),
                        CompressionLevel.Optimal
                    ),
                    client,
                    serverIdentityVerifier
                );

                Console.WriteLine("CLIENT: Attempting to connect to server... (" + (i + 1) + ")");

                try
                {
                    clientSocket.Connect(serverAddress);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("CLIENT: Connect failed: " + ex.Message);
                    continue;
                }

                Console.WriteLine("CLIENT: Connected to server.");

                while (clientSocket.IsConnected())
                {
                    
                }

                Console.WriteLine("CLIENT: Disconnected.");
                break;
            }
        }
    }
}
