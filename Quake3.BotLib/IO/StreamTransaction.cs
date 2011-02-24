using System;
using System.IO;

namespace Quake3.BotLib.IO
{
    public class StreamTransaction : IDisposable
    {
        private readonly Stream stream;
        private readonly long position;
        private bool committed;

        public StreamTransaction(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            this.stream = stream;
            this.position = stream.Position;
        }

        public void Commit()
        {
            this.committed = true;
        }

        public void Dispose()
        {
            if (!this.committed)
            {
                this.stream.Position = position;
            }
        }
    }
}