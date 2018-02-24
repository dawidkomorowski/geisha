using System.ComponentModel.Composition;
using Geisha.Engine.Audio.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Framework.Audio;

namespace Geisha.Engine.Audio.Systems
{
    [Export(typeof(IVariableTimeStepSystem))]
    internal class AudioSystem : IVariableTimeStepSystem
    {
        private readonly IAudioProvider _audioProvider;

        [ImportingConstructor]
        public AudioSystem(IAudioProvider audioProvider)
        {
            _audioProvider = audioProvider;
        }

        public int Priority { get; set; } = 4;

        public void Update(Scene scene, double deltaTime)
        {
            foreach (var entity in scene.AllEntities)
            {
                if (entity.HasComponent<AudioSource>())
                {
                    var audioSource = entity.GetComponent<AudioSource>();

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