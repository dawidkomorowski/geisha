using System.Diagnostics;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Audio.Systems;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Audio.Systems
{
    [TestFixture]
    public class AudioSystemTests
    {
        private IAudioPlayer _audioPlayer = null!;
        private AudioSystem _audioSystem = null!;
        private AudioScene _audioScene = null!;

        [SetUp]
        public void SetUp()
        {
            _audioPlayer = Substitute.For<IAudioPlayer>();
            var audioBackend = Substitute.For<IAudioBackend>();
            audioBackend.AudioPlayer.Returns(_audioPlayer);
            _audioSystem = new AudioSystem(audioBackend);

            _audioScene = new AudioScene(_audioSystem);
        }

        [Test]
        public void ProcessAudio_ShouldPlaySound_WhenAudioSourceIsNotPlayingYet()
        {
            // Arrange
            var audioSource = _audioScene.AddAudioSource(false);

            // Act
            _audioSystem.ProcessAudio();

            // Assert
            Debug.Assert(audioSource.Sound != null, "audioSource.Sound != null");
            _audioPlayer.Received(1).PlayOnce(audioSource.Sound);
            Assert.That(audioSource.IsPlaying, Is.True);
        }

        [Test]
        public void ProcessAudio_ShouldNotPlaySound_WhenAudioSourceIsNotPlayingYetButAudioSourceWasRemoved()
        {
            // Arrange
            var audioSource = _audioScene.AddAudioSource(false);

            audioSource.Entity.RemoveComponent(audioSource);

            // Act
            _audioSystem.ProcessAudio();

            // Assert
            _audioPlayer.DidNotReceive().PlayOnce(Arg.Any<ISound>());
            Assert.That(audioSource.IsPlaying, Is.False);
        }

        [Test]
        public void ProcessAudio_ShouldNotPlaySound_WhenAudioSourceIsNotPlayingYetButSoundIsNull()
        {
            // Arrange
            var audioSource = _audioScene.AddAudioSource(false);
            audioSource.Sound = null;

            // Act
            _audioSystem.ProcessAudio();

            // Assert
            _audioPlayer.DidNotReceiveWithAnyArgs().PlayOnce(null!);
            Assert.That(audioSource.IsPlaying, Is.False);
        }

        [Test]
        public void ProcessAudio_ShouldNotPlaySound_WhenAudioSourceIsAlreadyPlaying()
        {
            // Arrange
            var audioSource = _audioScene.AddAudioSource(true);

            // Act
            _audioSystem.ProcessAudio();

            // Assert
            Debug.Assert(audioSource.Sound != null, "audioSource.Sound != null");
            _audioPlayer.DidNotReceive().PlayOnce(audioSource.Sound);
            Assert.That(audioSource.IsPlaying, Is.True);
        }

        private class AudioScene
        {
            private readonly Scene _scene = TestSceneFactory.Create();

            public AudioScene(ISceneObserver observer)
            {
                _scene.AddObserver(observer);
            }

            public AudioSourceComponent AddAudioSource(bool isPlaying)
            {
                var entity = _scene.CreateEntity();
                var audioSource = entity.CreateComponent<AudioSourceComponent>();
                audioSource.Sound = Substitute.For<ISound>();
                audioSource.IsPlaying = isPlaying;

                return audioSource;
            }
        }
    }
}