using System.IO;

namespace Geisha.Framework.Audio
{
    /// <summary>
    ///     Defines interface of an audio backend that implements sound loading and playback.
    /// </summary>
    public interface IAudioProvider
    {
        /// <summary>
        ///     Creates new <see cref="ISound" /> from data in given stream.
        /// </summary>
        /// <param name="stream">Stream containing data of the sound.</param>
        /// <param name="soundFormat">Format of sound data in the <paramref name="stream" />.</param>
        /// <returns><see cref="ISound" /> that consists of sound data from the <paramref name="stream" />.</returns>
        ISound CreateSound(Stream stream, SoundFormat soundFormat);

        /// <summary>
        ///     Plays given sound.
        /// </summary>
        /// <param name="sound">Sound to be played.</param>
        void Play(ISound sound);
    }
}