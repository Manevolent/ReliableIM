using ReliableIM.Security.Symmetric;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol.ESL
{
    public sealed class EncryptedStreamDuplex : Stream
    {
        private readonly MemoryStream inputBuffer = new MemoryStream();
        private readonly MemoryStream outputBuffer = new MemoryStream();

        private readonly Stream baseStream;

        private readonly SymmetricKeyPair symmetricKeyPair;

        private readonly HMAC inputHmac;
        private readonly HMAC outputHmac;

        public EncryptedStreamDuplex(Stream baseStream, SymmetricKeyPair symmetricKeyPair)
        {
            this.baseStream = baseStream;
            this.symmetricKeyPair = symmetricKeyPair;

            //Create an input and output HMAC for use with authenticating messages.
            this.inputHmac = symmetricKeyPair.Local.CreateHMAC();
            this.outputHmac = symmetricKeyPair.Remote.CreateHMAC();
        }

        public override bool CanRead
        {
            get
            {
                return baseStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override bool CanWrite
        {
            get
            {
                return baseStream.CanWrite;
            }
        }

        public override void Flush()
        {
            //Create an output buffer for the stream.
            byte[] encryptedBuffer;

            using (var compressed = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(compressed, symmetricKeyPair.Local.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    //Copy the output buffer's contents onto the crypto stream.
                    byte[] unencryptedBuffer = outputBuffer.ToArray();

                    cryptoStream.Write(unencryptedBuffer, 0, unencryptedBuffer.Length);
                }

                //Read the compressed buffer.
                encryptedBuffer = compressed.ToArray();
            }

            if (encryptedBuffer.Length <= 0)
                return; //No flush necessary.

            //Write the encrypted contents to the underlying stream with an HMAC.
            BinaryWriter binaryWriter = new BinaryWriter(baseStream);
            Packet.WriteBytes(binaryWriter, encryptedBuffer);
            binaryWriter.Write(outputHmac.ComputeHash(encryptedBuffer));
            binaryWriter.Flush();

            //Reset the output buffer for future write operations.
            outputBuffer.Position = 0;
            outputBuffer.SetLength(0);
        }

        public override long Length
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public override long Position
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
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
                //Read in the encrypted data.
                BinaryReader binaryReader = new BinaryReader(baseStream);
                byte[] encryptedBuffer = Packet.ReadBytes(binaryReader);

                //Read in the HMAC and ensure the data has not been tampered.
                byte[] hmac = new byte[inputHmac.HashSize / 8];
                baseStream.Read(hmac, 0, hmac.Length);
                if (!inputHmac.ComputeHash(encryptedBuffer).SequenceEqual(hmac))
                    throw new SecurityException("Recieved HMAC does not match expected cryptographic digest.");

                //Create a new crypto stream around an array of bytes on the underlying stream.
                CryptoStream cryptoStream = new CryptoStream(
                    new MemoryStream(
                        encryptedBuffer
                    ),
                    symmetricKeyPair.Remote.CreateDecryptor(),
                    CryptoStreamMode.Read
                );

                //Copy the decompressed bytes to the input buffer.
                cryptoStream.CopyTo(inputBuffer);

                //Dispose of the data.
                cryptoStream.Dispose();
            }

            //Reset the position of the input buffer, now full of fresh data.
            inputBuffer.Position = 0;

            //Return the data required by the operation to the upper-level.
            return inputBuffer.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            //Write the data onto the output buffer.
            outputBuffer.Write(buffer, offset, count);
        }
    }
}
