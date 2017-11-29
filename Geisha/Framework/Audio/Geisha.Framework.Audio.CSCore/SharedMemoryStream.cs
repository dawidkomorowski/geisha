using System;
using System.IO;

namespace Geisha.Framework.Audio.CSCore
{
    internal class SharedMemoryStream : Stream
    {
        private readonly MemoryStream _sourceMemoryStream;

        public SharedMemoryStream(MemoryStream sourceMemoryStream)
        {
            if (!_sourceMemoryStream.CanSeek) throw new ArgumentException("Source stream must support seeking.", nameof(sourceMemoryStream));

            _sourceMemoryStream = sourceMemoryStream;
        }

        public override bool CanRead => true;
        public override bool CanSeek => _sourceMemoryStream.CanSeek;
        public override bool CanWrite => false;
        public override long Length => _sourceMemoryStream.Length;
        public override long Position { get; set; }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException($"{nameof(SharedMemoryStream)} is read only stream.");
        }
    }
}