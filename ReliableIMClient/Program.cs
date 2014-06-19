using OpenSSL.Core;
using OpenSSL.X509;
using ReliableIM.Network.Protocol;
using ReliableIM.Network.Protocol.GZIP;
using ReliableIM.Network.Protocol.RIM;
using ReliableIM.Network.Protocol.SSL;
using ReliableIM.Network.Protocol.TCP;
using ReliableIM.Network.Protocol.UDT;
using ReliableIM.Security.Certificate;
using ReliableIM.Security.Signature.RSA;
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
        
        static void ServerThread()
        {
            Console.WriteLine("SERVER: Starting...");
            Console.WriteLine(Application.ExecutablePath);

            RimListener listener = new RimListener(
                new GZipListener(
                    new SslListener(
                        new TcpListener(serverAddress),
                        CertificateGenerator.GenerateSelfSignedCertificate(
                                "CN=localhost",
                                "CN=localhost",
                                CertificateGenerator.GenerateCACertificate("CN=localhost")
                        )
                    ),
                    CompressionLevel.Optimal
                ),
                new RSASignatureAlgorithm(
                    new RSACryptoServiceProvider(),
                    new SHA1Managed()
                )
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

            for (int i = 0; i < 20; i++)
            {
                //Create client and connect.
                RimSocket clientSocket = new RimSocket(
                    new GZipSocket(
                        new SslSocket(
                            new TcpSocket()
                        ),
                        CompressionLevel.Optimal
                    ),
                    new RSASignatureAlgorithm(
                        new RSACryptoServiceProvider(),
                        new SHA1Managed()
                    )
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

                //Hold connection.
                while (clientSocket.IsConnected());

                Console.WriteLine("CLIENT: Disconnected.");
                break;
            }
        }
    }
}
