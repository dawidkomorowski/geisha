using System;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Audio.Components
{
    // TODO It should be possible to specify whether to load sound into memory or to stream it from disk.
    // TODO For SFX it would be mainly from memory, while for music it could be from disk (saves a lot of memory, it is rare to play several tracks at the same time).
    /// <summary>
    ///     Audio source capable of playing a single sound.
    /// </summary>
    public sealed class AudioSourceComponent : IComponent
    {
        public static ComponentId Id { get; } = new ComponentId("Geisha.Engine.Audio.AudioSourceComponent");

        public ComponentId ComponentId => Id;

        /// <summary>
        ///     Sound attached to audio source.
        /// </summary>
        public ISound? Sound { get; set; }

        /// <summary>
        ///     Indicates whether this audio source is currently playing a sound.
        /// </summary>
        public bool IsPlaying { get; internal set; }
    }

    internal sealed class AudioSourceComponentFactory : IComponentFactory
    {
        public Type ComponentType { get; } = typeof(AudioSourceComponent);
        public ComponentId ComponentId => AudioSourceComponent.Id;
        public IComponent Create() => new AudioSourceComponent();
    }
}