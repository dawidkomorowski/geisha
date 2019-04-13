using System;

namespace Geisha.Framework.Audio
{
    /// <summary>
    ///     Represents a sound.
    /// </summary>
    public interface ISound : IDisposable
    {
        /// <summary>
        ///     Sound format of this sound.
        /// </summary>
        SoundFormat Format { get; }
    }
}