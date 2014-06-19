using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.RIM.Packet
{
    public sealed class Packet255Disconnect : ReliableIM.Network.Protocol.Packet
    {
        public Packet255Disconnect(DisconnectReason disconnectReason)
        {
            this.Reason = disconnectReason;
        }

        public Packet255Disconnect()
            : this(DisconnectReason.GeneralDisconnect)
        {
        }

        /// <summary>
        /// The reason the disconnect occured.
        /// </summary>
        public DisconnectReason Reason { get; set; }

        public override byte GetPacketID()
        {
            return 255;
        }

        public override void Write(System.IO.BinaryWriter stream)
        {
            switch (Reason)
            {
                case DisconnectReason.AuthenticationFailed:
                    stream.Write((byte)2);
                    break;
                case DisconnectReason.AlreadyConnected:
                    stream.Write((byte)3);
                    break;
                case DisconnectReason.ProtocolException:
                    stream.Write((byte)4);
                    break;
                case DisconnectReason.UnexpectedPacket:
                    stream.Write((byte)5);
                    break;
                case DisconnectReason.ConnectionTimeout:
                    stream.Write((byte)6);
                    break;
                case DisconnectReason.UnsupportedVersion:
                    stream.Write((byte)7);
                    break;
                default:
                    stream.Write((byte)1);
                    break;
            }
        }

        public override void Read(System.IO.BinaryReader stream)
        {
            byte reasonId = stream.ReadByte();

            switch (reasonId)
            {
                case 2:
                    Reason = DisconnectReason.AuthenticationFailed;
                    break;
                case 3:
                    Reason = DisconnectReason.AlreadyConnected;
                    break;
                case 4:
                    Reason = DisconnectReason.ProtocolException;
                    break;
                case 5:
                    Reason = DisconnectReason.UnexpectedPacket;
                    break;
                case 6:
                    Reason = DisconnectReason.ConnectionTimeout;
                    break;
                case 7:
                    Reason = DisconnectReason.UnsupportedVersion;
                    break;
                default:
                    Reason = DisconnectReason.GeneralDisconnect;
                    break;
            }
        }

        public enum DisconnectReason
        {
            
            /// <summary>
            /// The remote client has exited or connection has been lost.
            /// </summary>
            GeneralDisconnect,

            /// <summary>
            /// The authentication process failed to complete successfuly.
            /// </summary>
            AuthenticationFailed,

            /// <summary>
            /// Another dupliacte, authenticated connection already exists.
            /// </summary>
            AlreadyConnected,

            /// <summary>
            /// A protocol exception was encountered, and the connection was terminated to preserve functionality.
            /// </summary>
            ProtocolException,

            /// <summary>
            /// A packet that was read was not expected, and the connection was terminated accordingly.
            /// </summary>
            UnexpectedPacket,

            /// <summary>
            /// A read or write timeout occured over the connection.
            /// </summary>
            ConnectionTimeout,

            /// <summary>
            /// The protocol version requested by the peer is not supported by the disconnecting endpoint.
            /// </summary>
            UnsupportedVersion
        }
    }
}
