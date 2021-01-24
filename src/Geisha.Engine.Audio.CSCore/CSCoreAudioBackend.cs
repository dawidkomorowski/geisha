using System;
using System.IO;
using Geisha.Common.Logging;
using Geisha.Engine.Audio.Backend;

namespace Geisha.Engine.Audio.CSCore
{
    /// <summary>
    ///     Audio backend implementation based on CSCore library. Tested to work on Windows.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public sealed class CSCoreAudioBackend : IAudioBackend, IDisposable
    {
        private static readonly ILog Log = LogFactory.Create(typeof(CSCoreAudioBackend));
        private readonly AudioPlayer _audioPlayer;

        // TODO Document it!
        /// <summary>
        /// 
        /// </summary>
        /// <param name="useDummyAudioDevice"></param>
        public CSCoreAudioBackend(bool useDummyAudioDevice = false)
        {
            if (useDummyAudioDevice)
            {
                Log.Info("Using dummy audio device.");
            }

            _audioPlayer = new AudioPlayer(useDummyAudioDevice);
        }

        /// <summary>
        ///     Audio player suitable for current platform.
        /// </summary>
        public IAudioPlayer AudioPlayer => _audioPlayer;

        /// <summary>
        ///     Creates new <see cref="ISound" /> from data in given stream.
        /// </summary>
        /// <param name="stream">Stream containing data of the sound.</param>
        /// <param name="soundFormat">Format of sound data in the <paramref name="stream" />.</param>
        /// <returns><see cref="ISound" /> that consists of sound data from the <paramref name="stream" />.</returns>
        public ISound CreateSound(Stream stream, SoundFormat soundFormat) => new Sound(new SharedMemoryStream(stream), soundFormat);

        public void Dispose()
        {
            _audioPlayer.Dispose();
        }
    }
}