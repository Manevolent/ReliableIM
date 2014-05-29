using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.RIM.Packet
{
    public class RimPacket255Disconnect : RimPacket
    {
        public RimPacket255Disconnect(DisconnectReason disconnectReason)
        {
            this.Reason = disconnectReason;
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
            base.Write(stream);

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
                default:
                    stream.Write((byte)1);
                    break;
            }
        }

        public override void Read(System.IO.BinaryReader stream)
        {
            base.Read(stream);

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
            ProtocolException
        }
    }
}
