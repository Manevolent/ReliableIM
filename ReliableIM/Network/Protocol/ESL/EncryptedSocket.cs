using ReliableIM.Security.Exchange;
using ReliableIM.Security.Symmetric;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.ESL
{
    public sealed class EncryptedSocket : Socket
    {
        private readonly Socket baseSocket;
        private readonly KeyExchangeAlgorithm keyExchangeAlgorithm;

        private Stream stream;

        public EncryptedSocket(Socket baseSocket, KeyExchangeAlgorithm keyExchangeAlgorithm)
        {
            this.baseSocket = baseSocket;
            this.keyExchangeAlgorithm = keyExchangeAlgorithm;

            if (baseSocket.IsConnected())
                this.stream = new EncryptedStreamDuplex(baseSocket.GetStream(), keyExchangeAlgorithm.Exchange(baseSocket.GetStream()));
        }

        public override bool IsConnected()
        {
            return baseSocket.IsConnected();
        }

        public override void Connect(System.Net.IPEndPoint endpoint)
        {
            baseSocket.Connect(endpoint);

            this.stream = new EncryptedStreamDuplex(baseSocket.GetStream(), keyExchangeAlgorithm.Exchange(baseSocket.GetStream()));
        }

        public override System.IO.Stream GetStream()
        {
            return stream;
        }

        public override void Close()
        {
            stream.Dispose();

            baseSocket.Close();
        }
    }
}
