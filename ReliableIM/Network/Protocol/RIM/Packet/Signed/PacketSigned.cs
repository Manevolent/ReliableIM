using ReliableIM.Security.Signature;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.RIM.Packet.Signed
{
    public abstract class PacketSigned : ReliableIM.Network.Protocol.Packet
    {
        /// <summary>
        /// Verifies the signature of the packet.
        /// </summary>
        /// <param name="signatureAlgorithm">Algorithm to verify the signature with.</param>
        /// <param name="signature">Signature this packet was sent with.</param>
        /// <returns>True if the verification was successful.</returns>
        protected virtual bool VerifySignature(SignatureAlgorithm signatureAlgorithm, Signature signature)
        {
            //Do not enforce sender identification at this level; only verify the signer's authenticity.
            return signatureAlgorithm.Verify(signature);
        }

        /// <summary>
        /// Converts this signed packet to a signature using the provided signature algorithm.
        /// </summary>
        /// <param name="signatureAlgorithm">Signature algorithm to sign the packet contents with.</param>
        /// <returns>Packet signature.</returns>
        public Signature ToSignature(SignatureAlgorithm signatureAlgorithm)
        {
            //Create the streams responsible for compiling the packet down to a signable byte array.
            MemoryStream signaturePacketBuffer = new MemoryStream();
            BinaryWriter packetWriter = new BinaryWriter(signaturePacketBuffer);

            //Write a cryptographic salt to the packet, randomizing the resulting signature.
            packetWriter.Write(new Random().Next());

            //Write the packet ID to the signature buffer.
            packetWriter.Write(GetPacketID());

            //Write the packet contents to the signature buffer.
            Write(packetWriter);

            //Sign the packet with the given algorithm.
            return signatureAlgorithm.Sign(signaturePacketBuffer.ToArray());
        }

        /// <summary>
        /// Converts a signature to a signed packet. Take note that during this process,
        /// the signature will be verified based on the desired algorithm.
        /// </summary>
        /// <param name="signature">Signature to convert.</param>
        /// <param name="signatureAlgorithm">Algorithm to verify the signature with.</param>
        /// <param name="packetFactory">Packet factory to create the packet from.</param>
        /// <returns>Signed packet.</returns>
        public static PacketSigned FromSignature(Signature signature, SignatureAlgorithm signatureAlgorithm, PacketFactory packetFactory)
        {
            if (signatureAlgorithm == null)
                throw new SecurityException("Null signature algorithm provided.");

            MemoryStream signaturePacketBuffer = new MemoryStream(signature.Data);
            BinaryReader stream = new BinaryReader(signaturePacketBuffer);

            //Read the cryptographic salt from the packet, skipping it to advance forward.
            stream.ReadInt32();

            //Read the ID from the packet.
            byte packetId = stream.ReadByte();

            //Create the packet from the factory using the ID read.
            ReliableIM.Network.Protocol.Packet packet = packetFactory.CreateFromId(packetId);
            if (!(packet is PacketSigned))
                throw new SecurityException("Packet type not acceptable.");

            //Read the packet contents into the factory's packet.
            packet.Read(stream);

            //Verify the packet's signature against the given algorithm.
            if (!((PacketSigned)packet).VerifySignature(signatureAlgorithm, signature))
                throw new SecurityException("Signature verification failed.");

            //Warning: At this point the signature has been verified, and authenticity
            //of the packet's origin (regardless of route) is no longer questioned.

            return (PacketSigned) packet;
        }
    }
}
