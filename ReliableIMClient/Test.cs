
using ReliableIM.Event;
using ReliableIM.Network.Protocol;
using ReliableIM.Network.Protocol.RIM;
using ReliableIM.Network.Protocol.SSL;
using ReliableIM.Network.Protocol.SSL.Listener;
using ReliableIM.Network.Protocol.UDT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReliableIMClient
{
    static class Test
    {
        private static RimListener listener;

        /// <summary>
        /// Test entry point.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
            X509Certificate serverCertificate = X509Certificate.CreateFromCertFile("Development.cer");

            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6969);

            RimSocket clientSocket = new RimSocket(new SslSocket(new UdtSocket(), EncryptionPolicy.RequireEncryption, new BasicSslAuthenticationListener()));
            listener = new RimListener(new SslListener(new UdtListener(endpoint), serverCertificate, EncryptionPolicy.RequireEncryption, new BasicSslAuthenticationListener()));

            Thread thread = new Thread(new ThreadStart(Accept));
            thread.Start();
           
            clientSocket.Connect(endpoint);

            Console.ReadLine();
        }

        public static void Accept()
        {
            listener.Start();
            Socket socket = listener.Accept();
            while (socket.IsConnected());
        }

        
    }
}
