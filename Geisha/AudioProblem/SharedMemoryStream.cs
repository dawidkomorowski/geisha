using System;
using System.IO;

namespace AudioProblem
{
    internal sealed class SharedMemoryStream : Stream
    {
        private readonly RefCounter _refCounter;
        private readonly MemoryStream _sourceMemoryStream;

        public SharedMemoryStream(byte[] buffer) : this(new MemoryStream(buffer), new RefCounter())
        {
        }

        private SharedMemoryStream(MemoryStream sourceMemoryStream, RefCounter refCounter)
        {
            lock (sourceMemoryStream)
            {
                _sourceMemoryStream = sourceMemoryStream;
                _refCounter = refCounter;

                _refCounter.Count++;
            }
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

        public SharedMemoryStream MakeShared()
        {
            lock (_sourceMemoryStream)
            {
                return new SharedMemoryStream(_sourceMemoryStream, _refCounter);
            }
        }

        public override void Flush()
        {
            //TODO what does the Flush do?
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

        // TODO CheckIfDisposed?
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_sourceMemoryStream)
                {
                    _refCounter.Count--;
                    if (_refCounter.Count == 0) _sourceMemoryStream?.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private class RefCounter
        {
            public int Count;
        }
    }
}