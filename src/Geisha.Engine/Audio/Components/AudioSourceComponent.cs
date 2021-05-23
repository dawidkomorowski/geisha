using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Audio.Components
{
    // TODO It should be possible to specify whether to load sound into memory or to stream it from disk.
    // TODO For SFX it would be mainly from memory, while for music it could be from disk (saves a lot of memory, it is rare to play several tracks at the same time).
    /// <summary>
    ///     Audio source capable of playing a single sound.
    /// </summary>
    [ComponentId("Geisha.Engine.Audio.AudioSourceComponent")]
    public sealed class AudioSourceComponent : Component
    {
        /// <summary>
        ///     Sound attached to audio source.
        /// </summary>
        public ISound? Sound { get; set; }

        /// <summary>
        ///     Indicates whether this audio source is currently playing a sound.
        /// </summary>
        public bool IsPlaying { get; internal set; }
    }

    internal sealed class AudioSourceComponentFactory : ComponentFactory<AudioSourceComponent>
    {
        protected override AudioSourceComponent CreateComponent() => new AudioSourceComponent();
    }
}