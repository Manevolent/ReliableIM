using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.SSL.Listener
{
    public class DefaultSslAuthenticationListener : SslAuthenticationListener
    {
        public bool Authenticate(
                X509Certificate certificate,
                X509Chain chain,
                SslPolicyErrors policyErrors
            )
        {
            //Only allow SSL handshakes with absolutely no errors.
            return policyErrors == SslPolicyErrors.None;
        }
    }
}
