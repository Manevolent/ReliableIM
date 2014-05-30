using ReliableIM.Network.Protocol.RIM.Handler.Authorized;
using ReliableIM.Network.Protocol.RIM.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.RIM.Handler.Unauthorized
{
    public class RimPacketHandlerUnauthorized : RimPacketHandler
    {
        public RimPacketHandlerUnauthorized(IPacketStream packetReciever) : base(packetReciever)
        {
            
        }

        protected override void HandleConnect(Packet2Connect connect)
        {
            Console.WriteLine("Connection request with protocol #" + connect.ProtocolVersion);
        }
    }
}
