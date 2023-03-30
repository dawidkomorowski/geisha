namespace Geisha.Engine.Audio.Backend
{
    /// <summary>
    ///     Defines interface of audio playback API.
    /// </summary>
    public interface IAudioPlayer
    {
        /// <summary>
        ///     Gets or sets value controlling whether sound output is enabled. Default value is <c>true</c>.
        /// </summary>
        /// <remarks>
        ///     When <c>true</c> the sound output is enabled. When <c>false</c> the sound output is disabled however all
        ///     playbacks are still progressing internal state.
        /// </remarks>
        bool EnableSound { get; set; }

        /// <summary>
        ///     Gets or sets master volume. Valid range is from <c>0.0</c> meaning no audio, to <c>1.0</c> meaning maximum audio
        ///     volume.
        /// </summary>
        double Volume { get; set; }

        /// <summary>
        ///     Plays given sound and returns <see cref="IPlayback" /> object to control playback of the sound.
        /// </summary>
        /// <param name="sound">Sound to be played.</param>
        /// <param name="playInLoop">Whether the sound should be played in a loop.</param>
        /// <returns><see cref="IPlayback" /> object which allows to control sound playback.</returns>
        IPlayback Play(ISound sound, bool playInLoop = false);

        /// <summary>
        ///     Plays given sound once.
        /// </summary>
        /// <param name="sound">Sound to be played.</param>
        void PlayOnce(ISound sound);
    }
}