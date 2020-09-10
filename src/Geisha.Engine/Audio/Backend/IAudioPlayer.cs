namespace Geisha.Engine.Audio.Backend
{
    /// <summary>
    ///     Defines interface of an audio player capable of audio playback.
    /// </summary>
    public interface IAudioPlayer
    {
        /// <summary>
        ///     Plays given sound.
        /// </summary>
        /// <param name="sound">Sound to be played.</param>
        void Play(ISound sound);
    }
}