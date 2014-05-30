using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.RIM.Packet
{
    public class Packet2Connect : ReliableIM.Network.Protocol.Packet
    {
        private uint protocolVersion;

        public Packet2Connect(uint protocolVersion)
        {
            this.protocolVersion = protocolVersion;
        }

        public Packet2Connect()
        {

        }

        public override byte GetPacketID()
        {
            return 2;
        }

        public uint ProtocolVersion
        {
            get
            {
                return protocolVersion;
            }
        }

        public override void Write(System.IO.BinaryWriter stream)
        {
            stream.Write(protocolVersion);
        }

        public override void Read(System.IO.BinaryReader stream)
        {
            protocolVersion = stream.ReadUInt32();
        }
    }
}
