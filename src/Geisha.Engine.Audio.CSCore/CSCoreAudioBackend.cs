using System.IO;

namespace Geisha.Engine.Audio.CSCore
{
    /// <summary>
    ///     Audio backend implementation based on CSCore library. Tested to work on Windows.
    /// </summary>
    public sealed class CSCoreAudioBackend : IAudioBackend
    {
        /// <summary>
        ///     Creates new <see cref="ISound" /> from data in given stream.
        /// </summary>
        /// <param name="stream">Stream containing data of the sound.</param>
        /// <param name="soundFormat">Format of sound data in the <paramref name="stream" />.</param>
        /// <returns><see cref="ISound" /> that consists of sound data from the <paramref name="stream" />.</returns>
        public ISound CreateSound(Stream stream, SoundFormat soundFormat)
        {
            return new Sound(new SharedMemoryStream(stream), soundFormat);
        }

        /// <summary>
        ///     Creates audio player.
        /// </summary>
        /// <returns>New instance of <see cref="IAudioPlayer" />.</returns>
        public IAudioPlayer CreateAudioPlayer()
        {
            return new AudioPlayer();
        }
    }
}