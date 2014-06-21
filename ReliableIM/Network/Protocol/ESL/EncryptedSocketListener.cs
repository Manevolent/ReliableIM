using ReliableIM.Security.Exchange;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.ESL
{
    public sealed class EncryptedSocketListener : SocketListener
    {
        private readonly SocketListener baseListener;
        private readonly KeyExchangeAlgorithm keyExchangeAlgorithm;

        public EncryptedSocketListener(SocketListener baseListener, KeyExchangeAlgorithm keyExchangeAlgorithm)
        {
            this.baseListener = baseListener;
            this.keyExchangeAlgorithm = keyExchangeAlgorithm;
        }

        public override System.Net.IPEndPoint GetBindAddress()
        {
            return baseListener.GetBindAddress();
        }

        public override Socket Accept()
        {
            return new EncryptedSocket(baseListener.Accept(), keyExchangeAlgorithm);
        }

        public override void Close()
        {
            baseListener.Close();
        }

        public override void Start()
        {
            baseListener.Start();
        }
    }
}
