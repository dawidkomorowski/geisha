using System.Collections.Generic;
using Geisha.Engine.Audio.Backend;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Audio.Systems
{
    internal sealed class AudioSystem : IAudioSystem, ISceneObserver
    {
        private readonly IAudioPlayer _audioPlayer;
        private readonly List<AudioSourceComponent> _audioSourceComponents = new List<AudioSourceComponent>();

        public AudioSystem(IAudioBackend audioBackend)
        {
            _audioPlayer = audioBackend.AudioPlayer;
        }

        #region Implementation of IAudioSystem

        public void ProcessAudio()
        {
            foreach (var audioSource in _audioSourceComponents)
            {
                if (!audioSource.IsPlaying && audioSource.Sound != null)
                {
                    _audioPlayer.PlayOnce(audioSource.Sound);
                    audioSource.IsPlaying = true;
                }
            }
        }

        #endregion

        #region Implementation of ISceneObserver

        public void OnEntityCreated(Entity entity)
        {
        }

        public void OnEntityRemoved(Entity entity)
        {
        }

        public void OnEntityParentChanged(Entity entity, Entity? oldParent, Entity? newParent)
        {
        }

        public void OnComponentCreated(Component component)
        {
            if (component is AudioSourceComponent audioSourceComponent)
            {
                _audioSourceComponents.Add(audioSourceComponent);
            }
        }

        public void OnComponentRemoved(Component component)
        {
            if (component is AudioSourceComponent audioSourceComponent)
            {
                _audioSourceComponents.Remove(audioSourceComponent);
            }
        }

        #endregion
    }
}