namespace Geisha.Framework.Audio
{
    /// <summary>
    ///     Represents a sound.
    /// </summary>
    public interface ISound
    {
        /// <summary>
        ///     Sound format of this sound.
        /// </summary>
        SoundFormat Format { get; }
    }
}