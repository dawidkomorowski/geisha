using System;
using System.IO;

namespace Geisha.Engine.Audio.CSCore
{
    /// <inheritdoc />
    /// <summary>
    ///     Stream whose backing store is memory shared by all instances of the <see cref="SharedMemoryStream" /> class created
    ///     with <see cref="MakeShared" /> method.
    /// </summary>
    /// <remarks>
    ///     <see cref="SharedMemoryStream" /> implements read only shared-memory stream that allows having multiple
    ///     streams reading the same memory. Underlying memory is occupied until all instances of
    ///     <see cref="SharedMemoryStream" /> sharing the same memory are disposed. <see cref="SharedMemoryStream" /> class is
    ///     thread safe.
    /// </remarks>
    public sealed class SharedMemoryStream : Stream
    {
        private readonly MemoryStream _internalMemoryStream;
        private readonly object _lock;
        private readonly RefCounter _refCounter;
        private bool _disposed;
        private long _position;

        /// <summary>
        ///     Initializes new instance of the <see cref="SharedMemoryStream" /> class based on the specified byte array.
        /// </summary>
        /// <param name="buffer">The array of unsigned bytes from which to create the current stream.</param>
        /// <remarks>
        ///     Provided <paramref name="buffer" /> is used as actual backing store of the stream therefore any modification
        ///     of the <paramref name="buffer" /> will be reflected in stream itself.
        /// </remarks>
        public SharedMemoryStream(byte[] buffer) : this(new object(), new RefCounter(), new MemoryStream(buffer))
        {
        }

        /// <summary>
        ///     Initializes new instance of the <see cref="SharedMemoryStream" /> class based on data copied from the specified
        ///     stream.
        /// </summary>
        /// <param name="stream">The stream from which data is copied to create the current stream.</param>
        /// <remarks>
        ///     Provided <paramref name="stream" /> is used as source from which data is copied into backing store of the
        ///     current stream therefore any modification of the <paramref name="stream" /> will not be reflected in the current
        ///     steam itself.
        /// </remarks>
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

        /// <inheritdoc />
        /// <summary>
        ///     Gets a value indicating whether the current stream supports reading.
        /// </summary>
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

        /// <inheritdoc />
        /// <summary>
        ///     Gets a value indicating whether the current stream supports seeking.
        /// </summary>
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

        /// <inheritdoc />
        /// <summary>
        ///     Gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite => false;

        /// <inheritdoc />
        /// <summary>
        ///     Gets the length of the stream in bytes.
        /// </summary>
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

        /// <inheritdoc />
        /// <summary>
        ///     Gets or sets the current position within the stream.
        /// </summary>
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

        /// <summary>
        ///     Creates new <see cref="SharedMemoryStream" /> that shares memory with current instance.
        /// </summary>
        /// <returns>New instance of <see cref="SharedMemoryStream" /> that shares memory with current instance.</returns>
        public SharedMemoryStream MakeShared()
        {
            lock (_lock)
            {
                ThrowIfDisposed();
                return new SharedMemoryStream(_lock, _refCounter, _internalMemoryStream);
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Overrides the <see cref="M:Geisha.Engine.Audio.CSCore.SharedMemoryStream.Flush" /> method so that no action is
        ///     performed.
        /// </summary>
        public override void Flush()
        {
        }

        /// <inheritdoc />
        /// <summary>
        ///     Sets the position within the current stream to the specified value.
        /// </summary>
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

        /// <inheritdoc />
        /// <summary>
        ///     Throws <see cref="NotSupportedException" /> as stream is read only.
        /// </summary>
        public override void SetLength(long value)
        {
            throw new NotSupportedException($"{nameof(SharedMemoryStream)} is read only stream.");
        }

        /// <inheritdoc />
        /// <summary>
        ///     Reads a block of bytes from the current stream and writes the data to a buffer.
        /// </summary>
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

        /// <inheritdoc />
        /// <summary>
        ///     Throws <see cref="T:System.NotSupportedException" /> as stream is read only.
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException($"{nameof(SharedMemoryStream)} is read only stream.");
        }

        protected override void Dispose(bool disposing)
        {
            lock (_lock)
            {
                if (_disposed) return;

                if (disposing)
                {
                    _refCounter.Count--;
                    if (_refCounter.Count == 0) _internalMemoryStream.Dispose();

                    _disposed = true;
                }

                base.Dispose(disposing);
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(SharedMemoryStream));
        }

        // TODO Maybe it should implement Increment and Decrement and by its onw handle disposal?
        private class RefCounter
        {
            public int Count;
        }
    }
}