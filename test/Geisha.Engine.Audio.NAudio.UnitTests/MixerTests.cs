using System;
using System.Linq;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.Audio.NAudio.UnitTests
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
            Assert.That(mixer.WaveFormat, Is.EqualTo(SupportedWaveFormat.IeeeFloat44100Channels2));
        }

        #endregion

        #region Methods

        [Test]
        public void AddTrack_ShouldThrowException_WhenMixerDisposed()
        {
            // Arrange
            var mixer = new Mixer();
            var soundData = GetRandomFloats();
            var sound = new SoundSampleProvider(new Sound(soundData, SoundFormat.Wav));

            mixer.Dispose();

            // Act
            // Assert
            Assert.That(() => mixer.AddTrack(sound), Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void AddTrack_ShouldReturnNewTrack()
        {
            // Arrange
            var mixer = new Mixer();
            var soundData = GetRandomFloats();
            var sound = new SoundSampleProvider(new Sound(soundData, SoundFormat.Wav));

            // Act
            var actual = mixer.AddTrack(sound);

            // Assert
            Assert.That(actual, Is.Not.Null);
        }

        [Test]
        public void RemoveTrack_ShouldThrowException_WhenMixerDisposed()
        {
            // Arrange
            var mixer = new Mixer();
            var soundData = GetRandomFloats();
            var sound = new SoundSampleProvider(new Sound(soundData, SoundFormat.Wav));

            var track = mixer.AddTrack(sound);
            mixer.Dispose();

            // Act
            // Assert
            Assert.That(() => mixer.RemoveTrack(track), Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void RemoveTrack_ShouldDisposeTrack()
        {
            // Arrange
            var mixer = new Mixer();
            var soundData = GetRandomFloats();
            var sound = new SoundSampleProvider(new Sound(soundData, SoundFormat.Wav));

            var track = mixer.AddTrack(sound);
            object? eventSender = null;
            track.Disposed += (sender, _) => eventSender = sender;

            // Act
            mixer.RemoveTrack(track);

            // Assert
            Assert.That(eventSender, Is.EqualTo(track));
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
            var sound = new SoundSampleProvider(new Sound(soundData, SoundFormat.Wav));
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
            var sound = new SoundSampleProvider(new Sound(soundData, SoundFormat.Wav));
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
            var sound = new SoundSampleProvider(new Sound(soundData, SoundFormat.Wav));
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
            var sound = new SoundSampleProvider(new Sound(soundData, SoundFormat.Wav));
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
            var sound = new SoundSampleProvider(new Sound(soundData, SoundFormat.Wav));
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
            var sound1 = new SoundSampleProvider(new Sound(new[] { 1f, 2f, 3f }, SoundFormat.Wav));
            var sound2 = new SoundSampleProvider(new Sound(new[] { 10f, 20f, 30f }, SoundFormat.Wav));
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
        public void Read_ShouldStopTrack_WhenItHasCompleted()
        {
            // Arrange
            var mixer = new Mixer();
            var soundData = GetRandomFloats();
            var sound = new SoundSampleProvider(new Sound(soundData, SoundFormat.Wav))
            {
                Position = soundData.Length
            };
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
        public void Dispose_ShouldDisposeAllAddedTracks()
        {
            // Arrange
            var mixer = new Mixer();
            var soundData = GetRandomFloats();
            var sound1 = new SoundSampleProvider(new Sound(soundData, SoundFormat.Wav));
            var sound2 = new SoundSampleProvider(new Sound(soundData, SoundFormat.Wav));
            var track1 = mixer.AddTrack(sound1);
            var track2 = mixer.AddTrack(sound2);

            object? eventSender1 = null;
            track1.Disposed += (sender, _) => eventSender1 = sender;

            object? eventSender2 = null;
            track2.Disposed += (sender, _) => eventSender2 = sender;

            // Act
            mixer.Dispose();

            // Assert
            Assert.That(eventSender1, Is.EqualTo(track1));
            Assert.That(eventSender2, Is.EqualTo(track2));
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
            var sound = new SoundSampleProvider(new Sound(soundData, SoundFormat.Wav));

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
            var sound = new SoundSampleProvider(new Sound(soundData, SoundFormat.Wav));

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
            var sound = new SoundSampleProvider(new Sound(soundData, SoundFormat.Wav));

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
            var sound = new SoundSampleProvider(new Sound(soundData, SoundFormat.Wav));

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
            var sound = new SoundSampleProvider(new Sound(soundData, SoundFormat.Wav));

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
            var sound = new SoundSampleProvider(new Sound(soundData, SoundFormat.Wav));

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
                floats[i] = Utils.Random.NextFloat(-1f, 1f);
            }

            return floats;
        }

        #endregion
    }
}