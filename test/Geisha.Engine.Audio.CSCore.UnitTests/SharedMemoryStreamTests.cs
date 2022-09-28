using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Geisha.Engine.Audio.CSCore.UnitTests
{
    [TestFixture]
    public class SharedMemoryStreamTests
    {
        #region Constructors

        [Test]
        public void ConstructorFromByteArray_ShouldCreateStreamFromByteArray()
        {
            // Arrange
            var bytes = GetRandomBytes();

            // Act
            var sharedMemoryStream = new SharedMemoryStream(bytes);

            // Assert
            var buffer = new byte[bytes.Length];
            sharedMemoryStream.Read(buffer, 0, buffer.Length);
            Assert.That(buffer, Is.EqualTo(bytes));
        }

        [Test]
        public void ConstructorFromStream_ShouldCreateStreamFromStream()
        {
            // Arrange
            var bytes = GetRandomBytes();
            using var stream = new MemoryStream(bytes);

            // Act
            var sharedMemoryStream = new SharedMemoryStream(stream);

            // Assert
            var buffer = new byte[bytes.Length];
            var read = sharedMemoryStream.Read(buffer, 0, buffer.Length);
            Assert.That(read, Is.EqualTo(buffer.Length));
            Assert.That(buffer, Is.EqualTo(bytes));
        }

        #endregion

        #region Properties

        [Test]
        public void CanRead_ShouldBeTrueIfNotDisposedAndFalseIfDisposed()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream = new SharedMemoryStream(bytes);

            // Assume
            Assume.That(sharedMemoryStream.CanRead, Is.True);

            // Act
            sharedMemoryStream.Dispose();

            // Assert
            Assert.That(sharedMemoryStream.CanRead, Is.False);
        }

        [Test]
        public void CanSeek_ShouldBeTrueIfNotDisposedAndFalseIfDisposed()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream = new SharedMemoryStream(bytes);

            // Assume
            Assume.That(sharedMemoryStream.CanSeek, Is.True);

            // Act
            sharedMemoryStream.Dispose();

            // Assert
            Assert.That(sharedMemoryStream.CanSeek, Is.False);
        }

        [Test]
        public void CanWrite_ShouldBeFalseAsStreamIsReadOnly()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream = new SharedMemoryStream(bytes);

            // Act
            // Assert
            Assert.That(sharedMemoryStream.CanWrite, Is.False);
        }

        [Test]
        public void Length_ShouldReturnStreamLength()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream = new SharedMemoryStream(bytes);

            // Act
            // Assert
            Assert.That(sharedMemoryStream.Length, Is.EqualTo(bytes.Length));
        }

        [Test]
        public void Length_ShouldThrowExceptionWhenStreamDisposed()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream = new SharedMemoryStream(bytes);

            sharedMemoryStream.Dispose();

            // Act
            // Assert
            Assert.That(() => sharedMemoryStream.Length, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void Position_Get_ShouldThrowExceptionWhenStreamDisposed()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream = new SharedMemoryStream(bytes);

            sharedMemoryStream.Dispose();

            // Act
            // Assert
            Assert.That(() => sharedMemoryStream.Position, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void Position_Set_ShouldThrowExceptionWhenStreamDisposed()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream = new SharedMemoryStream(bytes);

            sharedMemoryStream.Dispose();

            // Act
            // Assert
            Assert.That(() => sharedMemoryStream.Position = 0, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void Position_Get_ShouldReturnWhatWasSet()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream = new SharedMemoryStream(bytes);

            // Act
            sharedMemoryStream.Position = 123;

            // Assert
            Assert.That(sharedMemoryStream.Position, Is.EqualTo(123));
        }

        #endregion

        #region Methods

        [Test]
        public void MakeShared_ShouldThrowExceptionWhenStreamDisposed()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream = new SharedMemoryStream(bytes);

            sharedMemoryStream.Dispose();

            // Act
            // Assert
            Assert.That(() => sharedMemoryStream.MakeShared(), Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void MakeShared_ShouldReturnNewInstanceOfSharedMemoryStream()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream = new SharedMemoryStream(bytes);

            // Act
            var actual = sharedMemoryStream.MakeShared();

            // Assert
            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        public void MakeShared_ShouldReturnStreamThatProvidesAccessToTheSameData()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream = new SharedMemoryStream(bytes);

            // Act
            var actual = sharedMemoryStream.MakeShared();

            // Assert
            var buffer = new byte[bytes.Length];
            actual.Read(buffer, 0, buffer.Length);
            Assert.That(buffer, Is.EqualTo(bytes));
        }

        [Test]
        public void Flush_ShouldNotThrowException()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream = new SharedMemoryStream(bytes);

            // Act
            // Assert
            Assert.That(() => sharedMemoryStream.Flush(), Throws.Nothing);
        }

        [Test]
        public void Flush_ShouldNotThrowExceptionWhenStreamDisposed()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream = new SharedMemoryStream(bytes);

            sharedMemoryStream.Dispose();

            // Act
            // Assert
            Assert.That(() => sharedMemoryStream.Flush(), Throws.Nothing);
        }

        [Test]
        public void Seek_ShouldThrowExceptionWhenStreamDisposed()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream = new SharedMemoryStream(bytes);

            sharedMemoryStream.Dispose();

            // Act
            // Assert
            Assert.That(() => sharedMemoryStream.Seek(123, SeekOrigin.Begin), Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void Seek_ShouldMovePositionInStream()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream = new SharedMemoryStream(bytes);

            // Assume
            Assume.That(sharedMemoryStream.Position, Is.Zero);

            // Act
            var seek = sharedMemoryStream.Seek(123, SeekOrigin.Begin);

            // Assert
            Assert.That(seek, Is.EqualTo(123));
            Assert.That(sharedMemoryStream.Position, Is.EqualTo(123));
        }

        [Test]
        public void Seek_ShouldMovePositionInStreamIndependentlyForEachSharedInstance()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream1 = new SharedMemoryStream(bytes);
            var sharedMemoryStream2 = sharedMemoryStream1.MakeShared();

            sharedMemoryStream1.Position = 123;
            sharedMemoryStream2.Position = 321;

            // Assume
            Assume.That(sharedMemoryStream1.Position, Is.EqualTo(123));
            Assume.That(sharedMemoryStream2.Position, Is.EqualTo(321));

            // Act
            var seek1 = sharedMemoryStream1.Seek(10, SeekOrigin.Current);
            var seek2 = sharedMemoryStream2.Seek(100, SeekOrigin.Current);

            // Assert
            Assert.That(seek1, Is.EqualTo(133));
            Assert.That(sharedMemoryStream1.Position, Is.EqualTo(133));

            Assert.That(seek2, Is.EqualTo(421));
            Assert.That(sharedMemoryStream2.Position, Is.EqualTo(421));
        }

        [Test]
        public void SetLength_ShouldThrowNotSupportedExceptionAsStreamIsReadOnly()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream = new SharedMemoryStream(bytes);

            // Act
            // Assert
            Assert.That(() => sharedMemoryStream.SetLength(123), Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void Read_ShouldThrowExceptionWhenStreamDisposed()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream = new SharedMemoryStream(bytes);

            sharedMemoryStream.Dispose();

            // Act
            // Assert
            Assert.That(() => sharedMemoryStream.Read(new byte[100], 0, 100), Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void Read_ShouldReadBytesAndMovePositionInStream()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream = new SharedMemoryStream(bytes);

            // Assume
            Assume.That(sharedMemoryStream.Position, Is.Zero);

            // Act
            var buffer = new byte[123];
            var read = sharedMemoryStream.Read(buffer, 0, 123);

            // Assert
            Assert.That(read, Is.EqualTo(123));
            Assert.That(buffer, Is.EqualTo(bytes.Take(123)));
            Assert.That(sharedMemoryStream.Position, Is.EqualTo(123));
        }

        [Test]
        public void Read_ShouldReadBytesAndMovePositionInStreamIndependentlyForEachSharedInstance()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream1 = new SharedMemoryStream(bytes);
            var sharedMemoryStream2 = sharedMemoryStream1.MakeShared();

            sharedMemoryStream1.Position = 123;
            sharedMemoryStream2.Position = 321;

            // Assume
            Assume.That(sharedMemoryStream1.Position, Is.EqualTo(123));
            Assume.That(sharedMemoryStream2.Position, Is.EqualTo(321));

            // Act
            var buffer1 = new byte[10];
            var read1 = sharedMemoryStream1.Read(buffer1, 0, 10);

            var buffer2 = new byte[100];
            var read2 = sharedMemoryStream2.Read(buffer2, 0, 100);

            // Assert
            Assert.That(read1, Is.EqualTo(10));
            Assert.That(buffer1, Is.EqualTo(bytes.Skip(123).Take(10)));
            Assert.That(sharedMemoryStream1.Position, Is.EqualTo(133));

            Assert.That(read2, Is.EqualTo(100));
            Assert.That(buffer2, Is.EqualTo(bytes.Skip(321).Take(100)));
            Assert.That(sharedMemoryStream2.Position, Is.EqualTo(421));
        }

        [Test]
        public void Write_ShouldThrowNotSupportedExceptionAsStreamIsReadOnly()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream = new SharedMemoryStream(bytes);

            // Act
            // Assert
            Assert.That(() => sharedMemoryStream.Write(new byte[100], 0, 100), Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void Dispose_ShouldNotDisposeSharedResourcesUntilAllSharedInstancesAreDisposed()
        {
            // Arrange
            var bytes = GetRandomBytes();
            var sharedMemoryStream1 = new SharedMemoryStream(bytes);
            var sharedMemoryStream2 = sharedMemoryStream1.MakeShared();

            // Assume
            Assume.That(sharedMemoryStream1.Position, Is.EqualTo(0));
            Assume.That(sharedMemoryStream2.Position, Is.EqualTo(0));

            // Act
            sharedMemoryStream1.Dispose();

            // Assert
            Assert.That(() => sharedMemoryStream1.Seek(10, SeekOrigin.Current), Throws.TypeOf<ObjectDisposedException>());

            var seek = sharedMemoryStream2.Seek(100, SeekOrigin.Current);
            Assert.That(seek, Is.EqualTo(100));
            Assert.That(sharedMemoryStream2.Position, Is.EqualTo(100));
        }

        #endregion

        private static byte[] GetRandomBytes()
        {
            var random = new Random();
            var bytes = new byte[10000 * random.Next(1, 10)];
            random.NextBytes(bytes);
            return bytes;
        }
    }
}