using System;
using System.IO;

namespace Geisha.Framework.Audio.CSCore
{
    // TODO add docs
    internal sealed class SharedMemoryStream : Stream
    {
        private readonly object _lock;
        private readonly RefCounter _refCounter;
        private readonly MemoryStream _internalMemoryStream;
        private bool _disposed;
        private long _position;

        public SharedMemoryStream(byte[] buffer) : this(new object(), new RefCounter(), new MemoryStream(buffer))
        {
        }

        public SharedMemoryStream(Stream stream) : this(new object(), new RefCounter(), new MemoryStream())
        {
            lock (_lock)
            {
                stream.CopyTo(_internalMemoryStream);
            }
        }

        private SharedMemoryStream(object @lock, RefCounter refCounter, MemoryStream internalMemoryStream)
        {
            _lock = @lock;

            lock (_lock)
            {
                _refCounter = refCounter;
                _internalMemoryStream = internalMemoryStream;

                _refCounter.Count++;
            }
        }

        public override bool CanRead
        {
            get
            {
                lock (_lock)
                {
                    return !_disposed;
                }
            }
        }

        public override bool CanSeek
        {
            get
            {
                lock (_lock)
                {
                    return !_disposed;
                }
            }
        }

        public override bool CanWrite => false;

        public override long Length
        {
            get
            {
                lock (_lock)
                {
                    ThrowIfDisposed();
                    return _internalMemoryStream.Length;
                }
            }
        }

        public override long Position
        {
            get
            {
                lock (_lock)
                {
                    ThrowIfDisposed();
                    return _position;
                }
            }
            set
            {
                lock (_lock)
                {
                    ThrowIfDisposed();
                    _position = value;
                }
            }
        }

        // Creates another shallow copy of stream that uses the same underlying MemoryStream
        public SharedMemoryStream MakeShared()
        {
            lock (_lock)
            {
                ThrowIfDisposed();
                return new SharedMemoryStream(_lock, _refCounter, _internalMemoryStream);
            }
        }

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            lock (_lock)
            {
                ThrowIfDisposed();

                _internalMemoryStream.Position = Position;
                var seek = _internalMemoryStream.Seek(offset, origin);
                Position = _internalMemoryStream.Position;

                return seek;
            }
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException($"{nameof(SharedMemoryStream)} is read only stream.");
        }

        // Uses position that is unique for each copy of shared stream
        // to read underlying MemoryStream that is common for all shared copies
        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (_lock)
            {
                ThrowIfDisposed();

                _internalMemoryStream.Position = Position;
                var read = _internalMemoryStream.Read(buffer, offset, count);
                Position = _internalMemoryStream.Position;

                return read;
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException($"{nameof(SharedMemoryStream)} is read only stream.");
        }

        // Reference counting to dispose underlying MemoryStream when all shared copies are disposed
        protected override void Dispose(bool disposing)
        {
            lock (_lock)
            {
                if (disposing)
                {
                    _disposed = true;
                    _refCounter.Count--;
                    if (_refCounter.Count == 0) _internalMemoryStream?.Dispose();
                }

                base.Dispose(disposing);
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(SharedMemoryStream));
        }

        private class RefCounter
        {
            public int Count;
        }
    }
}