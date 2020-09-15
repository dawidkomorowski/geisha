namespace Geisha.Engine.Audio.Backend
{
    /// <summary>
    ///     Defines interface of audio playback API.
    /// </summary>
    public interface IAudioPlayer
    {
        /// <summary>
        ///     Plays given sound and returns <see cref="IPlayback" /> object to control playback of the sound.
        /// </summary>
        /// <param name="sound">Sound to be played.</param>
        /// <returns><see cref="IPlayback" /> object which allows to control sound playback.</returns>
        IPlayback Play(ISound sound);

        /// <summary>
        ///     Plays given sound once.
        /// </summary>
        /// <param name="sound">Sound to be played.</param>
        void PlayOnce(ISound sound);
    }
}