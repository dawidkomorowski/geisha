using CSCore;
using Geisha.Engine.Audio.Backend;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Audio.CSCore.UnitTests
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
        public void IsPlaying_ShouldReturnIsPlayingOfTrack(bool isPlaying)
        {
            // Arrange
            var track = Substitute.For<ITrack>();
            track.IsPlaying.Returns(isPlaying);

            IPlayback playback = new Playback(_mixer, track);

            // Act
            // Assert
            Assert.That(playback.IsPlaying, Is.EqualTo(isPlaying));
        }

        [Test]
        public void Stopped_EventShouldBeRaised_WhenTrackIsStopped()
        {
            // Arrange
            var sampleSource = Substitute.For<ISampleSource>();
            sampleSource.WaveFormat.Returns(new WaveFormat(44100, 32, 2, AudioEncoding.IeeeFloat));
            var track = _mixer.AddTrack(sampleSource);

            IPlayback playback = new Playback(_mixer, track);
            object? eventSender = null;
            playback.Stopped += (sender, args) => eventSender = sender;

            // Act
            track.Stop();

            // Assert
            Assert.That(eventSender, Is.EqualTo(playback));
        }

        [Test]
        public void Disposed_EventShouldBeRaised_WhenTrackIsDisposed()
        {
            // Arrange
            var sampleSource = Substitute.For<ISampleSource>();
            sampleSource.WaveFormat.Returns(new WaveFormat(44100, 32, 2, AudioEncoding.IeeeFloat));
            var track = _mixer.AddTrack(sampleSource);

            IPlayback playback = new Playback(_mixer, track);
            object? eventSender = null;
            playback.Disposed += (sender, args) => eventSender = sender;

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
            IPlayback playback = new Playback(_mixer, track);

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
            IPlayback playback = new Playback(_mixer, track);

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
            IPlayback playback = new Playback(_mixer, track);

            // Act
            playback.Stop();

            // Assert
            track.Received(1).Stop();
        }

        [Test]
        public void Dispose_ShouldDisposeSampleSource()
        {
            // Arrange
            var sampleSource = Substitute.For<ISampleSource>();
            sampleSource.WaveFormat.Returns(new WaveFormat(44100, 32, 2, AudioEncoding.IeeeFloat));
            var track = _mixer.AddTrack(sampleSource);
            IPlayback playback = new Playback(_mixer, track);

            // Act
            playback.Dispose();

            // Assert
            sampleSource.Received(1).Dispose();
        }
    }
}