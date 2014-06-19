using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.GZIP
{
    public class GZipSocket : Socket
    {
        private readonly Socket baseSocket;
        private readonly CompressionLevel compressionLevel;

        private Stream innerStream;

        public GZipSocket(Socket baseSocket, CompressionLevel compressionLevel)
        {
            this.baseSocket = baseSocket;
            this.compressionLevel = compressionLevel;

            if (baseSocket.IsConnected())
                innerStream = new GZipStreamDuplex(baseSocket.GetStream());
        }

        public override bool IsConnected()
        {
            return baseSocket.IsConnected();
        }

        public override void Connect(System.Net.IPEndPoint endpoint)
        {
            baseSocket.Connect(endpoint);

            innerStream = new GZipStreamDuplex(baseSocket.GetStream());
        }

        public override System.IO.Stream GetStream()
        {
            return innerStream;
        }

        public override void Close()
        {
            innerStream.Close();

            if (baseSocket.IsConnected())
                baseSocket.Close();
        }
    }
}
