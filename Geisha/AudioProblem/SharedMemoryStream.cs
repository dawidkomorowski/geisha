using System;
using System.IO;

namespace AudioProblem
{
    internal class SharedMemoryStream : Stream
    {
        private readonly MemoryStream _sourceMemoryStream;

        public SharedMemoryStream(MemoryStream sourceMemoryStream)
        {
            if (!sourceMemoryStream.CanSeek) throw new ArgumentException("Source stream must support seeking.", nameof(sourceMemoryStream));

            _sourceMemoryStream = sourceMemoryStream;
        }

        public override bool CanRead => true;

        public override bool CanSeek
        {
            get
            {
                lock (_sourceMemoryStream)
                {
                    return _sourceMemoryStream.CanSeek;
                }
            }
        }

        public override bool CanWrite => false;

        public override long Length
        {
            get
            {
                lock (_sourceMemoryStream)
                {
                    return _sourceMemoryStream.Length;
                }
            }
        }

        public override long Position { get; set; }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            lock (_sourceMemoryStream)
            {
                var seek = _sourceMemoryStream.Seek(offset, origin);
                Position = _sourceMemoryStream.Position;
                return seek;
            }
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException($"{nameof(SharedMemoryStream)} is read only stream.");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (_sourceMemoryStream)
            {
                _sourceMemoryStream.Position = Position;
                var read = _sourceMemoryStream.Read(buffer, offset, count);
                Position = _sourceMemoryStream.Position;

                return read;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException($"{nameof(SharedMemoryStream)} is read only stream.");
        }
    }
}