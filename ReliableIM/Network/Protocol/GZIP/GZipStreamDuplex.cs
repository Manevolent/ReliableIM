using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.GZIP
{
    class GZipStreamDuplex : Stream
    {
        private readonly MemoryStream inputBuffer = new MemoryStream();
        private readonly MemoryStream outputBuffer = new MemoryStream();
        
        private readonly Stream baseStream;

        public GZipStreamDuplex(Stream stream)
        {
            this.baseStream = stream;
        }

        public override bool CanRead
        {
            get { return baseStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return baseStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return baseStream.CanWrite; }
        }

        public override void Flush()
        {
            //Create an output buffer for the stream.
            byte[] compressedBuffer;

            using (var compressed = new MemoryStream())
            {
                using (var gzipStream = new GZipStream(compressed, CompressionMode.Compress))
                {
                    //Copy the output buffer's contents onto the GZip stream.
                    byte[] uncompressedBuffer = outputBuffer.ToArray();

                    gzipStream.Write(uncompressedBuffer, 0, uncompressedBuffer.Length);
                }

                //Read the compressed buffer.
                compressedBuffer = compressed.ToArray();
            }

            if (compressedBuffer.Length <= 0)
                return; //No flush necessary.

            //Write the compressed contents to the underlying stream.
            Packet.WriteBytes(new BinaryWriter(baseStream), compressedBuffer);

            baseStream.Flush();

            //Reset the output buffer for future write operations.
            outputBuffer.Position = 0;
            outputBuffer.SetLength(0);
        }

        public override long Length
        {
            get {
                return baseStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return baseStream.Position;
            }
            set
            {
                baseStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            //Check for bytes buffered that weren't recieved last time.
            //This operation shifts any unread data to the left, at position 0.
            long available = inputBuffer.Length - inputBuffer.Position;
            if (available > 0)
            {
                byte[] availableData = new byte[available];
                int availableRead = inputBuffer.Read(availableData, 0, availableData.Length);

                //Check to be sure the available data is enough to fix on the stream.
                if (availableRead > 0)
                {
                    //Reset the input buffer's position and write the un-read data.
                    inputBuffer.Position = 0;
                    inputBuffer.Write(availableData, 0, availableRead);

                    //Reset the length and position to match the unread data's length.
                    inputBuffer.SetLength(availableRead);
                    inputBuffer.Position = availableRead;
                }
            }
            else
            {
                //No bytes were missed last read, reset the stream.
                inputBuffer.Position = 0;
                inputBuffer.SetLength(0);
            }

            //While we don't have enough bytes for the upper-level stream, read more from the network.
            while (inputBuffer.Length < count)
            {
                //Create a new GZip stream around an array of bytes on the underlying stream.
                GZipStream gzipStream = new GZipStream(
                    new MemoryStream(
                        Packet.ReadBytes(
                            new BinaryReader(
                                baseStream
                            )
                        )
                    ),
                    CompressionMode.Decompress
                );

                //Copy the decompressed bytes to the input buffer.
                gzipStream.CopyTo(inputBuffer);

                //Dispose of the data.
                gzipStream.Dispose();
            }

            //Reset the position of the input buffer, now full of fresh data.
            inputBuffer.Position = 0;

            //Return the data required by the operation to the upper-level.
            return inputBuffer.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return baseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            baseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            outputBuffer.Write(buffer, offset, count);
        }
    }
}
