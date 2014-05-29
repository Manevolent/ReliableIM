using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.SSL.Listener
{
    public class BasicSslAuthenticationListener : SslAuthenticationListener
    {
        public bool Authenticate(
                X509Certificate certificate,
                X509Chain chain,
                SslPolicyErrors policyErrors
            )
        {
            return true;
        }
    }
}
