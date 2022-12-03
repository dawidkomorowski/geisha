using System;
using System.IO;
using Geisha.Engine.Audio.Backend;

namespace Geisha.Engine.Audio.NAudio
{
    /// <summary>
    ///     Audio backend implementation based on NAudio library. Tested to work on Windows.
    /// </summary>
    public sealed class NAudioAudioBackend : IAudioBackend, IDisposable
    {
        /// <summary>
        ///     Audio player suitable for current platform.
        /// </summary>
        public IAudioPlayer AudioPlayer { get; }

        /// <summary>
        ///     Creates new <see cref="ISound" /> from data in given stream.
        /// </summary>
        /// <param name="stream">Stream containing data of the sound.</param>
        /// <param name="soundFormat">Format of sound data in the <paramref name="stream" />.</param>
        /// <returns><see cref="ISound" /> that consists of sound data from the <paramref name="stream" />.</returns>
        public ISound CreateSound(Stream stream, SoundFormat soundFormat) => throw new NotImplementedException();

        /// <summary>
        /// Shuts down <see cref="NAudioAudioBackend"/>.
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}