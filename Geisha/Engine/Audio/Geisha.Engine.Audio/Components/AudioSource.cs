using Geisha.Engine.Core.SceneModel;
using Geisha.Framework.Audio;

namespace Geisha.Engine.Audio.Components
{
    // TODO It should be possible to specify whether to load sound into memory or to stream it from disk.
    // TODO For SFX it would be mainly from memory, while for music it could be from disk (saves a lot of memory, it is rare to plays several tracks at the same time).
    /// <summary>
    ///     Audio source capable of playing a single sound.
    /// </summary>
    public sealed class AudioSource : IComponent
    {
        /// <summary>
        ///     Sound attached to audio source.
        /// </summary>
        public ISound Sound { get; set; }

        /// <summary>
        ///     Indicates whether this audio source is currently playing a sound.
        /// </summary>
        public bool IsPlaying { get; internal set; }
    }
}