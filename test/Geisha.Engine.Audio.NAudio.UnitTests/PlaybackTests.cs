using System;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Audio.NAudio.UnitTests
{
    [TestFixture]
    public class PlaybackTests
    {
        private Mixer _mixer = null!;

        [SetUp]
        public void SetUp()
        {
            _mixer = new Mixer();
        }

        [TearDown]
        public void TearDown()
        {
            _mixer.Dispose();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsPlaying_ShouldGet_IsPlaying_OfTrack(bool isPlaying)
        {
            // Arrange
            var track = Substitute.For<ITrack>();
            track.IsPlaying.Returns(isPlaying);

            var playback = new Playback(_mixer, track);

            // Act
            // Assert
            Assert.That(playback.IsPlaying, Is.EqualTo(isPlaying));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void PlayInLoop_ShouldGet_PlayInLoop_OfTrack(bool playInLoop)
        {
            // Arrange
            var track = Substitute.For<ITrack>();
            track.PlayInLoop.Returns(playInLoop);

            var playback = new Playback(_mixer, track);

            // Act
            // Assert
            Assert.That(playback.PlayInLoop, Is.EqualTo(playInLoop));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void PlayInLoop_ShouldSet_PlayInLoop_OfTrack(bool playInLoop)
        {
            // Arrange
            var track = Substitute.For<ITrack>();
            var playback = new Playback(_mixer, track);

            // Act
            playback.PlayInLoop = playInLoop;

            // Assert
            track.Received(1).PlayInLoop = playInLoop;
        }

        [Test]
        public void Volume_ShouldGet_Volume_OfTrack()
        {
            // Arrange
            var track = Substitute.For<ITrack>();
            track.Volume.Returns(0.5);

            var playback = new Playback(_mixer, track);

            // Act
            // Assert
            Assert.That(playback.Volume, Is.EqualTo(0.5));
        }

        [Test]
        public void Volume_ShouldSet_Volume_OfTrack()
        {
            // Arrange
            var track = Substitute.For<ITrack>();
            var playback = new Playback(_mixer, track);

            // Act
            playback.Volume = 0.5;

            // Assert
            track.Received(1).Volume = 0.5;
        }

        [Test]
        public void Stopped_EventShouldBeRaised_WhenTrackIsStopped()
        {
            // Arrange
            var soundSampleProvider = new SoundSampleProvider(new Sound(Array.Empty<float>(), SoundFormat.Wav));
            var track = _mixer.AddTrack(soundSampleProvider);

            var playback = new Playback(_mixer, track);
            object? eventSender = null;
            playback.Stopped += (sender, _) => eventSender = sender;

            // Act
            track.Stop();

            // Assert
            Assert.That(eventSender, Is.EqualTo(playback));
        }

        [Test]
        public void Disposed_EventShouldBeRaised_WhenTrackIsDisposed()
        {
            // Arrange
            var soundSampleProvider = new SoundSampleProvider(new Sound(Array.Empty<float>(), SoundFormat.Wav));
            var track = _mixer.AddTrack(soundSampleProvider);

            var playback = new Playback(_mixer, track);
            object? eventSender = null;
            playback.Disposed += (sender, _) => eventSender = sender;

            // Act
            _mixer.RemoveTrack(track);

            // Assert
            Assert.That(eventSender, Is.EqualTo(playback));
        }

        [Test]
        public void Play_ShouldCallPlayOnTrack()
        {
            // Arrange
            var track = Substitute.For<ITrack>();
            var playback = new Playback(_mixer, track);

            // Act
            playback.Play();

            // Assert
            track.Received(1).Play();
        }

        [Test]
        public void Pause_ShouldCallPauseOnTrack()
        {
            // Arrange
            var track = Substitute.For<ITrack>();
            var playback = new Playback(_mixer, track);

            // Act
            playback.Pause();

            // Assert
            track.Received(1).Pause();
        }

        [Test]
        public void Stop_ShouldCallStopOnTrack()
        {
            // Arrange
            var track = Substitute.For<ITrack>();
            var playback = new Playback(_mixer, track);

            // Act
            playback.Stop();

            // Assert
            track.Received(1).Stop();
        }

        [Test]
        public void Dispose_ShouldDisposeTrack()
        {
            // Arrange
            var soundSampleProvider = new SoundSampleProvider(new Sound(Array.Empty<float>(), SoundFormat.Wav));
            var track = _mixer.AddTrack(soundSampleProvider);
            var playback = new Playback(_mixer, track);

            object? eventSender = null;
            track.Disposed += (sender, _) => eventSender = sender;

            // Act
            playback.Dispose();

            // Assert
            Assert.That(eventSender, Is.EqualTo(track));
        }
    }
}