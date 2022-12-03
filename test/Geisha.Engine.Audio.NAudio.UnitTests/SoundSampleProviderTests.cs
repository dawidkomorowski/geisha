using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.Audio.NAudio.UnitTests
{
    public class SoundSampleProviderTests
    {
        private Sound _sound = null!;
        private SoundSampleProvider _soundSampleProvider = null!;

        [SetUp]
        public void Setup()
        {
            var samples = new float[1000];
            for (var i = 0; i < samples.Length; i++)
            {
                samples[i] = Utils.Random.NextFloat(-1f, 1f);
            }

            _sound = new Sound(samples, SoundFormat.Wav);
            _soundSampleProvider = new SoundSampleProvider(_sound);
        }

        [TestCase(500, 500)]
        [TestCase(1000, 1000)]
        [TestCase(1500, 1000)]
        public void Read_ShouldReadSamples(int count, int expectedRead)
        {
            // Arrange
            var buffer = new float[count];

            // Act
            var read = _soundSampleProvider.Read(buffer, 0, count);

            // Assert
            Assert.That(read, Is.EqualTo(expectedRead));
            Assert.That(buffer[..expectedRead], Is.EqualTo(_sound.Samples[..expectedRead]));
            Assert.That(_soundSampleProvider.Position, Is.EqualTo(expectedRead));
        }

        [Test]
        public void Read_ShouldReadConsecutiveSamples_WhenCalledMultipleTimes()
        {
            // Arrange
            var buffer = new float[400];

            // Act
            var read = _soundSampleProvider.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.That(read, Is.EqualTo(400));
            Assert.That(buffer, Is.EqualTo(_sound.Samples[..400]));
            Assert.That(_soundSampleProvider.Position, Is.EqualTo(400));

            // Act
            read = _soundSampleProvider.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.That(read, Is.EqualTo(400));
            Assert.That(buffer, Is.EqualTo(_sound.Samples[400..800]));
            Assert.That(_soundSampleProvider.Position, Is.EqualTo(800));

            // Act
            read = _soundSampleProvider.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.That(read, Is.EqualTo(200));
            Assert.That(buffer[..200], Is.EqualTo(_sound.Samples[800..1000]));
            Assert.That(_soundSampleProvider.Position, Is.EqualTo(1000));
        }

        [Test]
        public void Read_ShouldReadZeroSamples_WhenPositionReachedTheEnd()
        {
            // Arrange
            var buffer = new float[800];

            // Act
            var read = _soundSampleProvider.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.That(read, Is.EqualTo(800));
            Assert.That(buffer, Is.EqualTo(_sound.Samples[..800]));
            Assert.That(_soundSampleProvider.Position, Is.EqualTo(800));

            // Act
            read = _soundSampleProvider.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.That(read, Is.EqualTo(200));
            Assert.That(buffer[..200], Is.EqualTo(_sound.Samples[800..1000]));
            Assert.That(_soundSampleProvider.Position, Is.EqualTo(1000));

            // Act
            read = _soundSampleProvider.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.That(read, Is.EqualTo(0));
            Assert.That(_soundSampleProvider.Position, Is.EqualTo(1000));
        }

        [Test]
        public void Read_ShouldReadSamples_FillingOutputBufferStartingFromOffset()
        {
            // Arrange
            var buffer = new float[1000];

            // Act
            var read = _soundSampleProvider.Read(buffer, 200, 500);

            // Assert
            Assert.That(read, Is.EqualTo(500));
            Assert.That(buffer[200..700], Is.EqualTo(_sound.Samples[..500]));
            Assert.That(_soundSampleProvider.Position, Is.EqualTo(500));
        }

        [Test]
        public void Read_ShouldReadSamples_StartingFromPosition()
        {
            // Arrange
            var buffer = new float[500];
            _soundSampleProvider.Position = 200;

            // Act
            var read = _soundSampleProvider.Read(buffer, 0, 500);

            // Assert
            Assert.That(read, Is.EqualTo(500));
            Assert.That(buffer, Is.EqualTo(_sound.Samples[200..700]));
            Assert.That(_soundSampleProvider.Position, Is.EqualTo(700));
        }
    }
}