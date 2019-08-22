using Geisha.Engine.Audio.Components;
using Geisha.Engine.Core;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;

namespace Geisha.Engine.Audio.Systems
{
    internal class AudioSystem : IVariableTimeStepSystem
    {
        private readonly IAudioProvider _audioProvider;

        public AudioSystem(IAudioProvider audioProvider)
        {
            _audioProvider = audioProvider;
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
                        _audioProvider.Play(audioSource.Sound);
                        audioSource.IsPlaying = true;
                    }
                }
            }
        }
    }
}