using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.RIM.Packet
{
    public class RimPacket1Ping : RimPacket
    {
        public override byte GetPacketID()
        {
            return 1;
        }
    }
}
