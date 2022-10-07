using System;
using System.Linq;
using CSCore;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Audio.CSCore.UnitTests
{
    [TestFixture]
    public class MixerTests
    {
        #region Constructor

        [Test]
        public void Constructor_ShouldSetWaveFormat()
        {
            // Arrange
            // Act
            var mixer = new Mixer();

            // Assert
            var waveFormat = mixer.WaveFormat;
            Assert.That(waveFormat.SampleRate, Is.EqualTo(44100));
            Assert.That(waveFormat.BitsPerSample, Is.EqualTo(32));
            Assert.That(waveFormat.Channels, Is.EqualTo(2));
            Assert.That(waveFormat.WaveFormatTag, Is.EqualTo(AudioEncoding.IeeeFloat));
        }

        #endregion

        #region Properties

        [Test]
        public void CanSeek_ShouldReturnFalse_AsMixerDoesNotSupportSeeking()
        {
            // Arrange
            var mixer = new Mixer();

            // Act
            var actual = mixer.CanSeek;

            // Assert
            Assert.That(actual, Is.False);
        }

        [Test]
        public void Position_Get_ShouldReturnZero_AsMixerIsInfiniteSampleSource()
        {
            // Arrange
            var mixer = new Mixer();

            // Act
            var actual = mixer.Position;

            // Assert
            Assert.That(actual, Is.Zero);
        }

        [Test]
        public void Position_Get_ShouldThrowException_WhenMixerDisposed()
        {
            // Arrange
            var mixer = new Mixer();
            mixer.Dispose();

            // Act
            // Assert
            Assert.That(() => mixer.Position, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void Position_Set_ShouldThrowException_AsMixerDoesNotSupportSeeking()
        {
            // Arrange
            var mixer = new Mixer();

            // Act
            // Assert
            Assert.That(() => mixer.Position = 123, Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void Length_Get_ShouldReturnZero_AsMixerIsInfiniteSampleSource()
        {
            // Arrange
            var mixer = new Mixer();

            // Act
            var actual = mixer.Length;

            // Assert
            Assert.That(actual, Is.Zero);
        }

        [Test]
        public void Length_Get_ShouldThrowException_WhenMixerDisposed()
        {
            // Arrange
            var mixer = new Mixer();
            mixer.Dispose();

            // Act
            // Assert
            Assert.That(() => mixer.Length, Throws.TypeOf<ObjectDisposedException>());
        }

        #endregion

        #region Methods

        [Test]
        public void AddTrack_ShouldThrowException_WhenMixerDisposed()
        {
            // Arrange
            var mixer = new Mixer();
            var sound = Substitute.For<ISampleSource>();

            mixer.Dispose();

            // Act
            // Assert
            Assert.That(() => mixer.AddTrack(sound), Throws.TypeOf<ObjectDisposedException>());
        }

        [TestCase(1)]
        [TestCase(3)]
        public void AddTrack_ShouldThrowException_WhenSampleSourceDoesNotHaveTwoChannels(int channels)
        {
            // Arrange
            var mixer = new Mixer();
            var sound = Substitute.For<ISampleSource>();
            sound.WaveFormat.Returns(new WaveFormat(44100, 32, channels, AudioEncoding.IeeeFloat));

            // Act
            // Assert
            Assert.That(() => mixer.AddTrack(sound), Throws.ArgumentException.With.Message.Contain("channels"));
        }

        [TestCase(32000)]
        [TestCase(48000)]
        public void AddTrack_ShouldThrowException_WhenSampleSourceUsesOtherSampleRateThan44100(int sampleRate)
        {
            // Arrange
            var mixer = new Mixer();
            var sound = Substitute.For<ISampleSource>();
            sound.WaveFormat.Returns(new WaveFormat(sampleRate, 32, 2, AudioEncoding.IeeeFloat));

            // Act
            // Assert
            Assert.That(() => mixer.AddTrack(sound), Throws.ArgumentException.With.Message.Contain("sample rate"));
        }

        [Test]
        public void AddTrack_ShouldThrowException_WhenSampleSourceUsesNotMatchingWaveFormat()
        {
            // Arrange
            var mixer = new Mixer();
            var sound = Substitute.For<ISampleSource>();
            sound.WaveFormat.Returns(new WaveFormat(44100, 32, 2, AudioEncoding.WAVE_FORMAT_FLAC));

            // Act
            // Assert
            Assert.That(() => mixer.AddTrack(sound), Throws.ArgumentException.With.Message.Contain("wave format"));
        }

        [Test]
        public void AddTrack_ShouldThrowNothing_WhenSampleSourceMatchesMixerWaveFormat()
        {
            // Arrange
            var mixer = new Mixer();
            var sound = Substitute.For<ISampleSource>();
            sound.WaveFormat.Returns(new WaveFormat(44100, 32, 2, AudioEncoding.IeeeFloat));

            // Act
            // Assert
            Assert.That(() => mixer.AddTrack(sound), Throws.Nothing);
        }

        [Test]
        public void RemoveTrack_ShouldThrowException_WhenMixerDisposed()
        {
            // Arrange
            var mixer = new Mixer();
            var soundData = GetRandomFloats();
            var sound = new TestSampleSource(soundData);

            var track = mixer.AddTrack(sound);
            mixer.Dispose();

            // Act
            // Assert
            Assert.That(() => mixer.RemoveTrack(track), Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void RemoveTrack_ShouldDisposeSampleSource()
        {
            // Arrange
            var mixer = new Mixer();
            var soundData = GetRandomFloats();
            var sound = new TestSampleSource(soundData);

            var track = mixer.AddTrack(sound);

            // Act
            mixer.RemoveTrack(track);

            // Assert
            Assert.That(sound.IsDisposed, Is.True);
        }

        [Test]
        public void Read_ShouldThrowException_WhenMixerDisposed()
        {
            // Arrange
            var mixer = new Mixer();
            mixer.Dispose();

            // Act
            // Assert
            Assert.That(() => mixer.Read(new float[100], 0, 100), Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void Read_ShouldFillBufferWithZeroes_WhenNoTrackAdded()
        {
            // Arrange
            var mixer = new Mixer();
            var buffer = GetRandomFloats();

            // Act
            mixer.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.That(buffer, Is.EqualTo(new float[buffer.Length]));
        }

        [Test]
        public void Read_ShouldFillBufferWithZeroes_WhenTrackAddedButNotPlayed()
        {
            // Arrange
            var mixer = new Mixer();
            var soundData = GetRandomFloats();
            var sound = new TestSampleSource(soundData);
            mixer.AddTrack(sound);

            var buffer = new float[soundData.Length];

            // Act
            mixer.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.That(buffer, Is.EqualTo(new float[buffer.Length]));
        }

        [Test]
        public void Read_ShouldFillBufferWithZeroes_WhenTrackAddedAndPlayed_ButSoundDisabled()
        {
            // Arrange
            var mixer = new Mixer();
            mixer.EnableSound = false;

            var soundData = GetRandomFloats();
            var sound = new TestSampleSource(soundData);
            var track = mixer.AddTrack(sound);
            track.Play();

            var buffer = new float[soundData.Length];

            // Act
            mixer.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.That(buffer, Is.EqualTo(new float[buffer.Length]));
        }

        [Test]
        public void Read_ShouldPlayTrackToTheEnd_WhenTrackAddedAndPlayed_ButSoundDisabled()
        {
            // Arrange
            var mixer = new Mixer();
            mixer.EnableSound = false;

            var soundData = GetRandomFloats();
            var sound = new TestSampleSource(soundData);
            var track = mixer.AddTrack(sound);
            track.Play();

            // Assume
            Assume.That(track.IsPlaying, Is.True);

            var buffer = new float[soundData.Length];

            // Act
            mixer.Read(buffer, 0, buffer.Length);
            mixer.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.That(track.IsPlaying, Is.False);
        }

        [Test]
        public void Read_ShouldFillBufferWithSoundData_WhenTrackAddedAndPlayed()
        {
            // Arrange
            var mixer = new Mixer();
            var soundData = GetRandomFloats();
            var sound = new TestSampleSource(soundData);
            var track = mixer.AddTrack(sound);
            track.Play();

            var buffer = new float[soundData.Length];

            // Act
            mixer.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.That(buffer, Is.EqualTo(soundData));
        }

        [Test]
        public void Read_ShouldFillBufferWithSoundDataFromTheOffset()
        {
            // Arrange
            var mixer = new Mixer();
            var soundData = GetRandomFloats();
            var sound = new TestSampleSource(soundData);
            var track = mixer.AddTrack(sound);
            track.Play();

            var buffer = new float[soundData.Length];

            // Act
            mixer.Read(buffer, 123, 456);

            // Assert
            Assert.That(buffer.Take(123), Is.EqualTo(Enumerable.Repeat(0f, 123)));
            Assert.That(buffer.Skip(123).Take(456), Is.EqualTo(soundData.Take(456)));
            Assert.That(buffer.Skip(123).Skip(456), Is.EqualTo(Enumerable.Repeat(0f, buffer.Length - (123 + 456))));
        }

        [Test]
        public void Read_ShouldMixTwoSoundsByAddition()
        {
            // Arrange
            var mixer = new Mixer();
            var sound1 = new TestSampleSource(new[] { 1f, 2f, 3f });
            var sound2 = new TestSampleSource(new[] { 10f, 20f, 30f });
            var track1 = mixer.AddTrack(sound1);
            var track2 = mixer.AddTrack(sound2);

            track1.Play();
            track2.Play();

            var buffer = new float[3];

            // Act
            mixer.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.That(buffer, Is.EqualTo(new[] { 11f, 22f, 33f }));
        }

        [Test]
        public void Read_ShouldNotDisposeSampleSource_WhenThereIsMoreToReadFromIt()
        {
            // Arrange
            var mixer = new Mixer();
            var soundData = GetRandomFloats();
            var sound = new TestSampleSource(soundData);
            var track = mixer.AddTrack(sound);
            track.Play();

            var buffer = new float[soundData.Length];

            // Act
            mixer.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.That(sound.IsDisposed, Is.False);
        }

        [Test]
        public void Read_ShouldStopTrack_WhenItHasCompleted()
        {
            // Arrange
            var mixer = new Mixer();
            var soundData = GetRandomFloats();
            var sound = new TestSampleSource(soundData);
            sound.Position = sound.Length;
            var track = mixer.AddTrack(sound);
            track.Play();

            var buffer = new float[soundData.Length];

            // Act
            mixer.Read(buffer, 0, buffer.Length);

            // Assert
            Assert.That(track.IsPlaying, Is.False);
            Assert.That(sound.Position, Is.Zero);
        }

        [Test]
        public void Dispose_ShouldDisposeAllAddedSampleSources()
        {
            // Arrange
            var mixer = new Mixer();
            var soundData = GetRandomFloats();
            var sound1 = new TestSampleSource(soundData);
            var sound2 = new TestSampleSource(soundData);
            mixer.AddTrack(sound1);
            mixer.AddTrack(sound2);

            // Assume
            Assume.That(sound1.IsDisposed, Is.False);
            Assume.That(sound2.IsDisposed, Is.False);

            // Act
            mixer.Dispose();

            // Assert
            Assert.That(sound1.IsDisposed, Is.True);
            Assert.That(sound2.IsDisposed, Is.True);
        }

        #endregion

        #region Track usecases

        [Test]
        public void TrackCanBePaused()
        {
            // Arrange
            const int fullCount = 1000;
            const int halfCount = fullCount / 2;

            var mixer = new Mixer();
            var soundData = GetRandomFloats(fullCount);
            var sound = new TestSampleSource(soundData);

            var track = mixer.AddTrack(sound);
            track.Play();

            var buffer = new float[fullCount];

            // Act
            mixer.Read(buffer, 0, halfCount);
            track.Pause();
            mixer.Read(buffer, halfCount, halfCount);

            // Assert
            Assert.That(track.IsPlaying, Is.False);
            var expectedBuffer = soundData.ToArray();
            Array.Clear(expectedBuffer, halfCount, halfCount);
            Assert.That(buffer, Is.EqualTo(expectedBuffer));
        }

        [Test]
        public void TrackCanBePausedAndResumed()
        {
            // Arrange
            const int fullCount = 1000;
            const int halfCount = fullCount / 2;
            const int quarterCount = fullCount / 4;

            var mixer = new Mixer();
            var soundData = GetRandomFloats(fullCount);
            var sound = new TestSampleSource(soundData);

            var track = mixer.AddTrack(sound);
            track.Play();

            var buffer = new float[fullCount];

            // Act
            mixer.Read(buffer, 0, halfCount);
            track.Pause();
            mixer.Read(buffer, halfCount, quarterCount);
            track.Play();
            mixer.Read(buffer, halfCount + quarterCount, quarterCount);

            // Assert
            Assert.That(track.IsPlaying, Is.True);
            var expectedBuffer = soundData.ToArray();
            Array.Copy(soundData, halfCount, expectedBuffer, halfCount + quarterCount, quarterCount);
            Array.Clear(expectedBuffer, halfCount, quarterCount);
            Assert.That(buffer, Is.EqualTo(expectedBuffer));
        }

        [Test]
        public void TrackCanBeStopped()
        {
            // Arrange
            const int fullCount = 1000;
            const int halfCount = fullCount / 2;

            var mixer = new Mixer();
            var soundData = GetRandomFloats(fullCount);
            var sound = new TestSampleSource(soundData);

            var track = mixer.AddTrack(sound);
            track.Play();

            var buffer = new float[fullCount];

            // Act
            mixer.Read(buffer, 0, halfCount);
            track.Stop();
            mixer.Read(buffer, halfCount, halfCount);

            // Assert
            Assert.That(track.IsPlaying, Is.False);
            var expectedBuffer = soundData.ToArray();
            Array.Clear(expectedBuffer, halfCount, halfCount);
            Assert.That(buffer, Is.EqualTo(expectedBuffer));
        }

        [Test]
        public void TrackCanBeStoppedAndPlayedFromTheBeginning()
        {
            // Arrange
            const int fullCount = 1000;
            const int halfCount = fullCount / 2;
            const int quarterCount = fullCount / 4;

            var mixer = new Mixer();
            var soundData = GetRandomFloats(fullCount);
            var sound = new TestSampleSource(soundData);

            var track = mixer.AddTrack(sound);
            track.Play();

            var buffer = new float[fullCount];

            // Act
            mixer.Read(buffer, 0, halfCount);
            track.Stop();
            mixer.Read(buffer, halfCount, quarterCount);
            track.Play();
            mixer.Read(buffer, halfCount + quarterCount, quarterCount);

            // Assert
            Assert.That(track.IsPlaying, Is.True);
            var expectedBuffer = soundData.ToArray();
            Array.Copy(soundData, 0, expectedBuffer, halfCount + quarterCount, quarterCount);
            Array.Clear(expectedBuffer, halfCount, quarterCount);
            Assert.That(buffer, Is.EqualTo(expectedBuffer));
        }

        [Test]
        public void TrackNotifiesWithEventWhenItIsStopped()
        {
            // Arrange
            var mixer = new Mixer();
            var soundData = GetRandomFloats();
            var sound = new TestSampleSource(soundData);

            var track = mixer.AddTrack(sound);

            object? eventSender = null;
            track.Stopped += (sender, args) => { eventSender = sender; };

            // Assume
            Assume.That(eventSender, Is.Null);

            // Act
            track.Stop();

            // Assert
            Assert.That(eventSender, Is.EqualTo(track));
        }

        [Test]
        public void TrackNotifiesWithEventWhenItIsDisposed()
        {
            // Arrange
            var mixer = new Mixer();
            var soundData = GetRandomFloats();
            var sound = new TestSampleSource(soundData);

            var track = mixer.AddTrack(sound);

            object? eventSender = null;
            track.Disposed += (sender, args) => { eventSender = sender; };

            // Assume
            Assume.That(eventSender, Is.Null);

            // Act
            mixer.RemoveTrack(track);

            // Assert
            Assert.That(eventSender, Is.EqualTo(track));
        }

        #endregion

        #region Helpers

        private static float[] GetRandomFloats(int count = 1000)
        {
            var floats = new float[count];

            for (var i = 0; i < floats.Length; i++)
            {
                floats[i] = i;
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
                for (var i = 0; i < dataToRead; i++)
                {
                    buffer[offset + i] = _data[Position++];
                }

                return (int)dataToRead;
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