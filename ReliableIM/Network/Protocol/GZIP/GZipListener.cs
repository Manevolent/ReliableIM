using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.GZIP
{
    public class GZipListener : SocketListener
    {
        private readonly SocketListener baseSocket;
        private readonly CompressionLevel compressionLevel;

        public GZipListener(SocketListener baseSocket, CompressionLevel compressionLevel)
        {
            this.baseSocket = baseSocket;
            this.compressionLevel = compressionLevel;
        }

        public override System.Net.IPEndPoint GetBindAddress()
        {
            return baseSocket.GetBindAddress();
        }

        public override Socket Accept()
        {
            Socket client = baseSocket.Accept();

            if (!client.IsConnected())
                throw new IOException("Underlying socket connection failed.");

            return new GZipSocket(client, compressionLevel);
        }

        public override void Close()
        {
            baseSocket.Close();
        }

        public override void Start()
        {
            baseSocket.Start();
        }
    }
}
