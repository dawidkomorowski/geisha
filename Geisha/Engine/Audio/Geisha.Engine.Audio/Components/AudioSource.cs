using Geisha.Engine.Core.SceneModel;
using Geisha.Framework.Audio;

namespace Geisha.Engine.Audio.Components
{
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