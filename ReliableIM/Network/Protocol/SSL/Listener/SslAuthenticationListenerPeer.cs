using OpenSSL.X509;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.SSL.Listener
{
    public class SslAuthenticationListenerPeer : ISslAuthenticationListener
    {
        public bool Authenticate(X509Certificate certificate, X509Chain chain, VerifyResult verifyResult)
        {
            return true;
        }
    }
}
