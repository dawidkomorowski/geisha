﻿using System;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Core;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Audio.Systems
{
    internal sealed class AudioSystem : IVariableTimeStepSystem, IDisposable
    {
        private readonly IAudioPlayer _audioPlayer;

        public AudioSystem(IAudioBackend audioBackend)
        {
            _audioPlayer = audioBackend.CreateAudioPlayer();
        }

        public string Name => GetType().FullName;

        public void Update(Scene scene, GameTime gameTime)
        {
            foreach (var entity in scene.AllEntities)
            {
                if (entity.HasComponent<AudioSourceComponent>())
                {
                    var audioSource = entity.GetComponent<AudioSourceComponent>();

                    if (!audioSource.IsPlaying)
                    {
                        _audioPlayer.Play(audioSource.Sound);
                        audioSource.IsPlaying = true;
                    }
                }
            }
        }

        public void Dispose()
        {
            _audioPlayer?.Dispose();
        }
    }
}