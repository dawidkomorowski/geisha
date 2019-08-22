using System;
using Geisha.Engine.Audio;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Audio.Systems;
using Geisha.Engine.Core;
using Geisha.Engine.Core.SceneModel;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Audio.Systems
{
    [TestFixture]
    public class AudioSystemTests
    {
        private readonly GameTime _gameTime = new GameTime(TimeSpan.FromSeconds(0.1));
        private IAudioProvider _audioProvider;
        private AudioSystem _audioSystem;

        [SetUp]
        public void SetUp()
        {
            _audioProvider = Substitute.For<IAudioProvider>();
            _audioSystem = new AudioSystem(_audioProvider);
        }

        [Test]
        public void Update_ShouldCallPlayOnAudioProvider_WhenAudioSourceIsNotPlayingYet()
        {
            // Arrange
            var audioSceneBuilder = new AudioSceneBuilder();
            var entity = audioSceneBuilder.AddAudioSource(false);
            var audioSource = entity.GetComponent<AudioSourceComponent>();
            var scene = audioSceneBuilder.Build();

            // Act
            _audioSystem.Update(scene, _gameTime);

            // Assert
            _audioProvider.Received(1).Play(audioSource.Sound);
        }

        [Test]
        public void Update_ShouldNotCallPlayOnAudioProvider_WhenAudioSourceIsAlreadyPlaying()
        {
            // Arrange
            var audioSceneBuilder = new AudioSceneBuilder();
            var entity = audioSceneBuilder.AddAudioSource(true);
            var audioSource = entity.GetComponent<AudioSourceComponent>();
            var scene = audioSceneBuilder.Build();

            // Act
            _audioSystem.Update(scene, _gameTime);

            // Assert
            _audioProvider.DidNotReceive().Play(audioSource.Sound);
        }

        [Test]
        public void Update_ShouldChangeAudioSourceStateToPlaying_WhenAudioSourceIsNotPlayingYet()
        {
            // Arrange
            var audioSceneBuilder = new AudioSceneBuilder();
            var entity = audioSceneBuilder.AddAudioSource(false);
            var audioSource = entity.GetComponent<AudioSourceComponent>();
            var scene = audioSceneBuilder.Build();

            // Act
            _audioSystem.Update(scene, _gameTime);

            // Assert
            Assert.That(audioSource.IsPlaying, Is.True);
        }

        [Test]
        public void Update_ShouldLeaveAudioSourceStateAsPlaying_WhenAudioSourceIsAlreadyPlaying()
        {
            // Arrange
            var audioSceneBuilder = new AudioSceneBuilder();
            var entity = audioSceneBuilder.AddAudioSource(true);
            var audioSource = entity.GetComponent<AudioSourceComponent>();
            var scene = audioSceneBuilder.Build();

            // Act
            _audioSystem.Update(scene, _gameTime);

            // Assert
            Assert.That(audioSource.IsPlaying, Is.True);
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