using ReliableIM.Security.Signature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.RIM.Packet
{
    public sealed class Packet2IdentityRequest : ReliableIM.Network.Protocol.Packet
    {
        private uint protocolVersion;
        private byte[] salt;

        public Packet2IdentityRequest(uint protocolVersion, byte[] salt)
        {
            this.protocolVersion = protocolVersion;
            this.salt = salt;
        }

        public Packet2IdentityRequest()
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

        /// <summary>
        /// Creates an identity response which will authenticate the identity of this party.
        /// </summary>
        /// <param name="signatureAlgorithm">Algorithm to sign the response with.</param>
        /// <returns>A signed, legitimate response to this request.</returns>
        public Packet3IdentityResponse CreateResponse(SignatureAlgorithm signatureAlgorithm)
        {
            return new Packet3IdentityResponse(signatureAlgorithm, signatureAlgorithm.Sign(salt));
        }

        public override void Write(System.IO.BinaryWriter stream)
        {
            stream.Write(protocolVersion);
            WriteBytes(stream, salt);
            WriteBytes(stream, new byte[10240]);
        }

        public override void Read(System.IO.BinaryReader stream)
        {
            protocolVersion = stream.ReadUInt32();
            salt = ReadBytes(stream);
            ReadBytes(stream);
        }
    }
}
