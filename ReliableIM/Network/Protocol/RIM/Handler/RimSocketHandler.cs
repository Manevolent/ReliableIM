using ReliableIM.Network.Protocol.RIM.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.RIM.Handler
{
    public abstract class RimPacketHandler : PacketHandler
    {
        private Random pingRandom;
        private DateTime lastPing;
        private bool pinging;
        private long lastPingKey;

        public RimPacketHandler(IPacketStream packetReceiver) : base (packetReceiver)
        {
            //Initialize a new randomizer to use for ping keys.
            pingRandom = new Random();
        }

        public sealed override void HandlePacket(Protocol.Packet packet)
        {
            try {
                if (packet == null)
                    throw new Exception("Cannot handle null packet");

                switch (packet.GetPacketID())
                {
                    case 1:
                        HandlePing((Packet1Ping) packet);
                        break;
                    case 2:
                        HandleIdentityRequest((Packet2IdentityRequest)packet);
                        break;
                    case 3:
                        HandleIdentityResponse((Packet3IdentityResponse)packet);
                        break;
                    case 4:
                        HandleSignature((Packet4Signature)packet);
                        break;
                    case 100:
                        HandleContactStatus((Packet100ContactStatus)packet);
                        break;
                    case 255:
                        HandleDisconnect((Packet255Disconnect)packet);
                        break;
                    default:
                        Disconnect(Packet255Disconnect.DisconnectReason.UnexpectedPacket);
                        break;
                }
            } catch (Exception ex) {
                Disconnect(Packet255Disconnect.DisconnectReason.ProtocolException);
            }
        }

        public sealed override void Ping()
        {
            double milliseconds = DateTime.Now.Subtract(lastPing).TotalMilliseconds;

            if (pinging)
            {
                //Check if the ping was lost.
                if (milliseconds >= 10000d)
                {
                    pinging = false;
                    Disconnect(Packet255Disconnect.DisconnectReason.ConnectionTimeout);
                }
            }
            else
            {
                //Check to see if sufficient time has passed to send another ping.
                if (milliseconds >= 1000d)
                {
                    pinging = true;
                    lastPingKey = pingRandom.Next();
                    Stream.WritePacket(new Packet1Ping(lastPingKey));
                }
            }
        }

//
        private void HandlePing(Packet1Ping ping)
        {
            if (ping.Mode == Packet1Ping.PingMode.Response)
            {
                if (pinging && ping.Key == lastPingKey)
                {
                    pinging = false; //Stop pinging.
                    lastPing = DateTime.Now;
                }
            }
            else if (ping.Mode == Packet1Ping.PingMode.Request)
            {
                //Acknowledge all request pings.
                Stream.WritePacket(ping.CreateAcknowledgement());
            }
            else
            {
                //No other ping types exist other that requests and responses.
                Disconnect(Packet255Disconnect.DisconnectReason.UnexpectedPacket);
            }
        }
        protected virtual void HandleIdentityRequest(Packet2IdentityRequest request)
        {
            Disconnect(Packet255Disconnect.DisconnectReason.UnexpectedPacket);
        }
        protected virtual void HandleIdentityResponse(Packet3IdentityResponse response)
        {
            Disconnect(Packet255Disconnect.DisconnectReason.UnexpectedPacket);
        }
        protected virtual void HandleSignature(Packet4Signature signature)
        {
            Disconnect(Packet255Disconnect.DisconnectReason.UnexpectedPacket);
        }
        protected virtual void HandleContactStatus(Packet100ContactStatus contactStatus)
        {
            Disconnect(Packet255Disconnect.DisconnectReason.UnexpectedPacket);
        }
        protected virtual void HandleDisconnect(Packet255Disconnect disconnect)
        {
            Console.WriteLine("Disconnected by endpoint: " + disconnect.Reason);
            Stream.Close();
        }
//

        public void Disconnect(Packet255Disconnect.DisconnectReason reason)
        {
            //Send a disconnect packet.
            Console.WriteLine("Disconnecting from endpoint: " + reason);
            Stream.WritePacket(new Packet255Disconnect(reason));
            Stream.Close();
        }

        public void Disconnect()
        {
            Disconnect(Packet255Disconnect.DisconnectReason.GeneralDisconnect);
        }
    }
}
