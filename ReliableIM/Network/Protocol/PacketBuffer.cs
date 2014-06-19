using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ReliableIM.Network.Protocol
{
    /// <summary>
    /// Provides a thread-safe, asynchronous handler for packets on a network stream.
    /// The implementation of decoding, handling, and streaming of packets is offered
    /// by the IPacketStream instance that this class relies on for its functionality.
    /// </summary>
    public sealed class PacketBuffer
    {
        private IPacketStream stream;
        private Thread thread;

        public PacketBuffer(IPacketStream stream)
        {
            this.stream = stream;
        }

        /// <summary>
        /// Starts the packet buffer. The packet stream associated with this buffer should have
        /// already set a valid PacketHandler instance before starting this operation.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            if (thread != null && thread.IsAlive)
                throw new InvalidOperationException("Packet buffer already started.");

            if (!stream.IsConnected())
                throw new InvalidOperationException("Packet stream is not connected.");

            if (stream.PacketHandler == null)
                throw new InvalidOperationException("Packet stream's handler has not been set.");

            //Create a new thread instance to handle the "run" method.
            thread = new Thread(new ThreadStart(run));

            //Background threads will not make the CLR pend for them to close on exit (Java's "daemon" threads)
            thread.IsBackground = true;

            //Ensure the priority is set to lowest, since these are background operations.
            thread.Priority = ThreadPriority.Lowest;

            //Start the thread.
            thread.Start();
        }

        private void run()
        {
            while (stream.IsConnected())
                stream.PacketHandler.HandlePacket(stream.ReadPacket());
        }
    }
}
