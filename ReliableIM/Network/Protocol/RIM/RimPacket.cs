using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.RIM
{
    public abstract class RimPacket : ReliableIM.Network.Protocol.Packet
    {
        public override void Write(System.IO.BinaryWriter stream)
        {

        }

        public override void Read(System.IO.BinaryReader stream)
        {

        }
    }
}
