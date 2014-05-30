using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.RIM.Packet
{
    public class Packet1Ping : ReliableIM.Network.Protocol.Packet
    {
        private long pingKey = 0;
        private PingMode pingMode = PingMode.Unknown;

        public Packet1Ping()
        {

        }

        public Packet1Ping(long pingKey, PingMode pingMode)
        {
            this.pingKey = pingKey;
        }

        public Packet1Ping(long pingKey)
            : this(pingKey, PingMode.Request)
        {
            //Do nothing.
        }

        /// <summary>
        /// Gets the key used to match this ping.
        /// </summary>
        public long Key
        {
            get
            {
                return pingKey;
            }
        }

        /// <summary>
        /// Creates an acknowledgement to this ping.
        /// </summary>
        /// <returns>A proper acknowledgement.</returns>
        public Packet1Ping CreateAcknowledgement()
        {
            if (pingMode != PingMode.Request)
                throw new InvalidOperationException("Can't acknowledge a non-request ping");

            return new Packet1Ping(pingKey, PingMode.Response);
        }

        /// <summary>
        /// Gets the mode this ping assumes.
        /// </summary>
        public PingMode Mode
        {
            get
            {
                return pingMode;
            }
        }

        public override byte GetPacketID()
        {
            return 1;
        }

        public override void Write(System.IO.BinaryWriter stream)
        {
            stream.Write((long) pingKey);

            switch (pingMode)
            {
                case PingMode.Request:
                    stream.Write((byte) 1);
                    break;
                case PingMode.Response:
                    stream.Write((byte) 2);
                    break;
                default:
                    stream.Write((byte) 0);
                    break;
            }
        }

        public override void Read(System.IO.BinaryReader stream)
        {
            pingKey = stream.ReadInt16();

            byte pingModeId = stream.ReadByte();

            switch (pingModeId)
            {
                case 1:
                    pingMode = PingMode.Request;
                    break;
                case 2:
                    pingMode = PingMode.Response;
                    break;
                default:
                    pingMode = PingMode.Unknown;
                    break;
            }
        }

        public enum PingMode
        {
            /// <summary>
            /// This ping is requesting an echo.
            /// </summary>
            Request,

            /// <summary>
            /// This ping is a response to a previous echo.
            /// </summary>
            Response,

            /// <summary>
            /// No ping mode is known.
            /// </summary>
            Unknown
        }
    }
}
