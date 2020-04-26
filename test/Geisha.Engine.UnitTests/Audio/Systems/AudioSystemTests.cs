using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Audio.Systems;
using Geisha.Engine.Core.SceneModel;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Audio.Systems
{
    [TestFixture]
    public class AudioSystemTests
    {
        private IAudioPlayer _audioPlayer;
        private AudioSystem _audioSystem;

        [SetUp]
        public void SetUp()
        {
            _audioPlayer = Substitute.For<IAudioPlayer>();
            var audioBackend = Substitute.For<IAudioBackend>();
            audioBackend.CreateAudioPlayer().Returns(_audioPlayer);
            _audioSystem = new AudioSystem(audioBackend);
        }

        [Test]
        public void ProcessAudio_ShouldPlaySound_WhenAudioSourceIsNotPlayingYet()
        {
            // Arrange
            var audioSceneBuilder = new AudioSceneBuilder();
            var entity = audioSceneBuilder.AddAudioSource(false);
            var audioSource = entity.GetComponent<AudioSourceComponent>();
            var scene = audioSceneBuilder.Build();

            // Act
            _audioSystem.ProcessAudio(scene);

            // Assert
            _audioPlayer.Received(1).Play(audioSource.Sound);
        }

        [Test]
        public void ProcessAudio_ShouldNotPlaySound_WhenAudioSourceIsAlreadyPlaying()
        {
            // Arrange
            var audioSceneBuilder = new AudioSceneBuilder();
            var entity = audioSceneBuilder.AddAudioSource(true);
            var audioSource = entity.GetComponent<AudioSourceComponent>();
            var scene = audioSceneBuilder.Build();

            // Act
            _audioSystem.ProcessAudio(scene);

            // Assert
            _audioPlayer.DidNotReceive().Play(audioSource.Sound);
        }

        [Test]
        public void ProcessAudio_ShouldChangeAudioSourceStateToPlaying_WhenAudioSourceIsNotPlayingYet()
        {
            // Arrange
            var audioSceneBuilder = new AudioSceneBuilder();
            var entity = audioSceneBuilder.AddAudioSource(false);
            var audioSource = entity.GetComponent<AudioSourceComponent>();
            var scene = audioSceneBuilder.Build();

            // Act
            _audioSystem.ProcessAudio(scene);

            // Assert
            Assert.That(audioSource.IsPlaying, Is.True);
        }

        [Test]
        public void ProcessAudio_ShouldLeaveAudioSourceStateAsPlaying_WhenAudioSourceIsAlreadyPlaying()
        {
            // Arrange
            var audioSceneBuilder = new AudioSceneBuilder();
            var entity = audioSceneBuilder.AddAudioSource(true);
            var audioSource = entity.GetComponent<AudioSourceComponent>();
            var scene = audioSceneBuilder.Build();

            // Act
            _audioSystem.ProcessAudio(scene);

            // Assert
            Assert.That(audioSource.IsPlaying, Is.True);
        }

        [Test]
        public void Dispose_ShouldDisposeAudioPlayer()
        {
            // Arrange
            // Act
            _audioSystem.Dispose();

            // Assert
            _audioPlayer.Received().Dispose();
        }

        private class AudioSceneBuilder
        {
            private readonly Scene _scene = new Scene();

            public Entity AddAudioSource(bool isPlaying)
            {
                var entity = new Entity();
                entity.AddComponent(new AudioSourceComponent
                {
                    Sound = Substitute.For<ISound>(),
                    IsPlaying = isPlaying
                });

                _scene.AddEntity(entity);

                return entity;
            }

            public Scene Build()
            {
                return _scene;
            }
        }
    }
}