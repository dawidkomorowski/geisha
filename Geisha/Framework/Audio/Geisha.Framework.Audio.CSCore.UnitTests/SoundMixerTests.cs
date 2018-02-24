using System;
using System.Linq;
using CSCore;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Framework.Audio.CSCore.UnitTests
{
    [TestFixture]
    public class SoundMixerTests
    {
        #region Constructor

        [Test]
        public void Constructor_ShouldSetWaveFormat()
        {
            // Arrange
            // Act
            var soundMixer = new SoundMixer();

            // Assert
            var waveFormat = soundMixer.WaveFormat;
            Assert.That(waveFormat.SampleRate, Is.EqualTo(44100));
            Assert.That(waveFormat.BitsPerSample, Is.EqualTo(32));
            Assert.That(waveFormat.Channels, Is.EqualTo(2));
            Assert.That(waveFormat.WaveFormatTag, Is.EqualTo(AudioEncoding.IeeeFloat));
        }

        #endregion

        #region Properties

        [Test]
        public void CanSeek_ShouldReturnFalse_AsSoundMixerDoesNotSupportSeeking()
        {
            // Arrange
            var soundMixer = new SoundMixer();

            // Act
            var actual = soundMixer.CanSeek;

            // Assert
            Assert.That(actual, Is.False);
        }

        [Test]
        public void Position_Get_ShouldReturnZero_AsSoundMixerIsInfiniteSampleSource()
        {
            // Arrange
            var soundMixer = new SoundMixer();

            // Act
            var actual = soundMixer.Position;

            // Assert
            Assert.That(actual, Is.Zero);
        }

        [Test]
        public void Position_Get_ShouldThrowException_WhenSoundMixerDisposed()
        {
            // Arrange
            var soundMixer = new SoundMixer();
            soundMixer.Dispose();

            // Act
            // Assert
            Assert.That(() => soundMixer.Position, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void Position_Set_ShouldThrowException_AsSoundMixerDoesNotSupportSeeking()
        {
            // Arrange
            var soundMixer = new SoundMixer();

            // Act
            // Assert
            Assert.That(() => soundMixer.Position = 123, Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void Length_Get_ShouldReturnZero_AsSoundMixerIsInfiniteSampleSource()
        {
            // Arrange
            var soundMixer = new SoundMixer();

            // Act
            var actual = soundMixer.Length;

            // Assert
            Assert.That(actual, Is.Zero);
        }

        [Test]
        public void Length_Get_ShouldThrowException_WhenSoundMixerDisposed()
        {
            // Arrange
            var soundMixer = new SoundMixer();
            soundMixer.Dispose();

            // Act
            // Assert
            Assert.That(() => soundMixer.Length, Throws.TypeOf<ObjectDisposedException>());
        }

        #endregion

        #region Methods

        [Test]
        public void AddSound_ShouldThrowException_WhenSoundMixerDisposed()
        {
            // Arrange
            var soundMixer = new SoundMixer();
            var sound = Substitute.For<ISampleSource>();

            soundMixer.Dispose();

            // Act
            // Assert
            Assert.That(() => soundMixer.AddSound(sound), Throws.TypeOf<ObjectDisposedException>());
        }

        [TestCase(1)]
        [TestCase(3)]
        public void AddSound_ShouldThrowException_WhenSoundDoesNotHaveTwoChannels(int channels)
        {
            // Arrange
            var soundMixer = new SoundMixer();
            var sound = Substitute.For<ISampleSource>();
            sound.WaveFormat.Returns(new WaveFormat(44100, 32, channels, AudioEncoding.IeeeFloat));

            // Act
            // Assert
            Assert.That(() => soundMixer.AddSound(sound), Throws.ArgumentException.With.Message.Contain("channels"));
        }

        [TestCase(32000)]
        [TestCase(48000)]
        public void AddSound_ShouldThrowException_WhenSoundUsesOtherSampleRateThan44100(int sampleRate)
        {
            // Arrange
            var soundMixer = new SoundMixer();
            var sound = Substitute.For<ISampleSource>();
            sound.WaveFormat.Returns(new WaveFormat(sampleRate, 32, 2, AudioEncoding.IeeeFloat));

            // Act
            // Assert
            Assert.That(() => soundMixer.AddSound(sound), Throws.ArgumentException.With.Message.Contain("sample rate"));
        }

        [Test]
        public void AddSound_ShouldThrowException_WhenSoundUsesNotMatchingWaveFormat()
        {
            // Arrange
            var soundMixer = new SoundMixer();
            var sound = Substitute.For<ISampleSource>();
            sound.WaveFormat.Returns(new WaveFormat(44100, 32, 2, AudioEncoding.WAVE_FORMAT_FLAC));

            // Act
            // Assert
            Assert.That(() => soundMixer.AddSound(sound), Throws.ArgumentException.With.Message.Contain("wave format"));
        }

        [Test]
        public void AddSound_ShouldThrowNothing_WhenSoundMatchesSoundMixerWaveFormat()
        {
            // Arrange
            var soundMixer = new SoundMixer();
            var sound = Substitute.For<ISampleSource>();
            sound.WaveFormat.Returns(new WaveFormat(44100, 32, 2, AudioEncoding.IeeeFloat));

            // Act
            // Assert
            Assert.That(() => soundMixer.AddSound(sound), Throws.Nothing);
        }

        [Test]
        public void Read_ShouldThrowException_WhenSoundMixerDisposed()
        {
            // Arrange
            var soundMixer = new SoundMixer();
            soundMixer.Dispose();

            // Act
            // Assert
            Assert.That(() => soundMixer.Read(new float[100], 0, 100), Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void Read_ShouldFillBufferWithZeroes_WhenNoSoundAdded()
        {
            // Arrange
            var soundMixer = new SoundMixer();
            var buffer = GetRandomFloats();

            // Act
            soundMixer.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.That(buffer, Is.EqualTo(new float[buffer.Length]));
        }

        [Test]
        public void Read_ShouldFillBufferWithSoundData_WhenSoundAdded()
        {
            // Arrange
            var soundMixer = new SoundMixer();
            var soundData = GetRandomFloats();
            var sound = new TestSampleSource(soundData);
            soundMixer.AddSound(sound);

            var buffer = new float[soundData.Length];

            // Act
            soundMixer.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.That(buffer, Is.EqualTo(soundData));
        }

        [Test]
        public void Read_ShouldFillBufferWithSoundDataFromTheOffset()
        {
            // Arrange
            var soundMixer = new SoundMixer();
            var soundData = GetRandomFloats();
            var sound = new TestSampleSource(soundData);
            soundMixer.AddSound(sound);

            var buffer = new float[soundData.Length];

            // Act
            soundMixer.Read(buffer, 123, 456);

            // Assert
            Assert.That(buffer.Take(123), Is.EqualTo(Enumerable.Repeat(0f, 123)));
            Assert.That(buffer.Skip(123).Take(456), Is.EqualTo(soundData.Take(456)));
            Assert.That(buffer.Skip(123).Skip(456), Is.EqualTo(Enumerable.Repeat(0f, buffer.Length - (123 + 456))));
        }

        [Test]
        public void Read_ShouldMixTwoSoundsByAddition()
        {
            // Arrange
            var soundMixer = new SoundMixer();
            var sound1 = new TestSampleSource(new[] {1f, 2f, 3f});
            soundMixer.AddSound(sound1);
            var sound2 = new TestSampleSource(new[] {10f, 20f, 30f});
            soundMixer.AddSound(sound2);

            var buffer = new float[3];

            // Act
            soundMixer.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.That(buffer, Is.EqualTo(new[] {11f, 22f, 33f}));
        }

        [Test]
        public void Read_ShouldNotDisposeSampleSource_WhenThereIsMoreToReadFromIt()
        {
            // Arrange
            var soundMixer = new SoundMixer();
            var soundData = GetRandomFloats();
            var sound = new TestSampleSource(soundData);
            soundMixer.AddSound(sound);

            var buffer = new float[soundData.Length];

            // Act
            soundMixer.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.That(sound.IsDisposed, Is.False);
        }

        [Test]
        public void Read_ShouldDisposeSampleSource_WhenThereIsNoMoreToReadFromIt()
        {
            // Arrange
            var soundMixer = new SoundMixer();
            var soundData = GetRandomFloats();
            var sound = new TestSampleSource(soundData);
            sound.Position = sound.Length;
            soundMixer.AddSound(sound);

            var buffer = new float[soundData.Length];

            // Act
            soundMixer.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.That(sound.IsDisposed, Is.True);
        }

        [Test]
        public void Dispose_ShouldDisposeAllAddedSampleSources()
        {
            // Arrange
            var soundMixer = new SoundMixer();
            var soundData = GetRandomFloats();
            var sound1 = new TestSampleSource(soundData);
            soundMixer.AddSound(sound1);
            var sound2 = new TestSampleSource(soundData);
            soundMixer.AddSound(sound2);

            // Assume
            Assume.That(sound1.IsDisposed, Is.False);
            Assume.That(sound2.IsDisposed, Is.False);

            // Act
            soundMixer.Dispose();

            // Assert
            Assert.That(sound1.IsDisposed, Is.True);
            Assert.That(sound2.IsDisposed, Is.True);
        }

        #endregion

        #region Helpers

        private static float[] GetRandomFloats()
        {
            var random = new Random();
            var floats = new float[10000 * random.Next(1, 10)];

            for (var i = 0; i < floats.Length; i++)
            {
                floats[i] = (float) random.NextDouble();
            }

            return floats;
        }

        private class TestSampleSource : ISampleSource
        {
            private readonly float[] _data;

            public TestSampleSource(float[] data)
            {
                _data = data;
            }

            public int Read(float[] buffer, int offset, int count)
            {
                var dataLeft = Length - Position;
                var dataToRead = dataLeft > count ? count : dataLeft;
                for (int i = 0; i < dataToRead; i++)
                {
                    buffer[offset + i] = _data[Position++];
                }

                return (int) dataToRead;
            }

            public void Dispose()
            {
                IsDisposed = true;
            }

            public bool CanSeek { get; } = true;
            public WaveFormat WaveFormat { get; } = new WaveFormat(44100, 32, 2, AudioEncoding.IeeeFloat);
            public long Position { get; set; }
            public long Length => _data.Length;

            public bool IsDisposed { get; private set; }
        }

        #endregion
    }
}