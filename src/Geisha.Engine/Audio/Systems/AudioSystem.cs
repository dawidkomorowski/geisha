using System;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Audio.Systems
{
    internal sealed class AudioSystem : IAudioSystem
    {
        private readonly IAudioPlayer _audioPlayer;

        public AudioSystem(IAudioBackend audioBackend)
        {
            _audioPlayer = audioBackend.AudioPlayer;
        }

        public void ProcessAudio(Scene scene)
        {
            foreach (var entity in scene.AllEntities)
            {
                if (entity.HasComponent<AudioSourceComponent>())
                {
                    var audioSource = entity.GetComponent<AudioSourceComponent>();

                    if (!audioSource.IsPlaying)
                    {
                        var sound = audioSource.Sound ??
                                    throw new InvalidOperationException($"{nameof(AudioSourceComponent)}.{nameof(AudioSourceComponent.Sound)} cannot be null.");

                        _audioPlayer.PlayOnce(sound);
                        audioSource.IsPlaying = true;
                    }
                }
            }
        }
    }
}