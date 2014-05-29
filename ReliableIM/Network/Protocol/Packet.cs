using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol
{
    abstract class Packet
    {
        /// <summary>
        /// Gets the ID this packet has been assigned. The type of
        /// packet the ID repesents depends on the protocol in use.
        /// </summary>
        /// <returns>Packet ID.</returns>
        public abstract byte GetPacketID();

        /// <summary>
        /// Write the packet data onto the given stream.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        public abstract void Write(BinaryWriter stream);

        /// <summary>
        /// Reads the packet data from the given stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        public abstract void Read(BinaryReader stream);

        /// <summary>
        /// Writes an array of bytes onto a stream.
        /// 
        /// If the array is larger than 65535 bytes, this method will throw
        /// an ArgumentOutOfRangeException.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="array">Array of bytes to write.</param>
        public static void WriteBytes(BinaryWriter stream, byte[] array)
        {
            if (array.Length > ushort.MaxValue)
                throw new ArgumentOutOfRangeException();

            stream.Write((ushort)array.Length);

            if (array.Length > 0)
                stream.Write(array);
        }

        /// <summary>
        /// Reads an array of bytes from the stream.
        /// 
        /// If the array is 0 bytes long, no attempt will be made to
        /// read any further data.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>Array of bytes read from the stream.</returns>
        public static byte[] ReadBytes(BinaryReader stream)
        {
            ushort length = stream.ReadUInt16();

            if (length == 0)
                return new byte[0];

            byte[] array = new byte[length];
            stream.Read(array, 0, length);
            return array;
        }
    }
}
