using System;
using System.Collections.Generic;
using System.IO;
using Geisha.Engine.Audio.Backend;
using NAudio.Wave;

namespace Geisha.Engine.Audio.NAudio
{
    /// <summary>
    ///     Audio backend implementation based on NAudio library. Tested to work on Windows.
    /// </summary>
    public sealed class NAudioAudioBackend : IAudioBackend, IDisposable
    {
        private readonly AudioPlayer _audioPlayer = new();

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
        public ISound CreateSound(Stream stream, SoundFormat soundFormat)
        {
            using WaveStream waveStream = soundFormat switch
            {
                SoundFormat.Wav => new WaveFileReader(stream),
                SoundFormat.Mp3 => new Mp3FileReader(stream),
                _ => throw new ArgumentOutOfRangeException(nameof(soundFormat), soundFormat, "Unsupported sound format.")
            };

            var sampleProvider = waveStream.ToSampleProvider();

            if (sampleProvider.WaveFormat.Channels == 1)
            {
                sampleProvider = sampleProvider.ToStereo();
            }

            var waveFormat = SupportedWaveFormat.IeeeFloat44100Channels2;

            if (!sampleProvider.WaveFormat.Equals(waveFormat))
            {
                throw new ArgumentException($"Unsupported wave format. Expected: {waveFormat}, Received: {sampleProvider.WaveFormat}");
            }

            var samplesList = new List<float>();
            var buffer = new float[waveFormat.SampleRate * waveFormat.Channels];
            int read;
            do
            {
                read = sampleProvider.Read(buffer, 0, buffer.Length);

                if (read == buffer.Length)
                {
                    samplesList.AddRange(buffer);
                }
                else
                {
                    for (var i = 0; i < read; i++)
                    {
                        samplesList.Add(buffer[i]);
                    }
                }
            } while (read != 0);

            return new Sound(samplesList.ToArray(), soundFormat);
        }

        /// <summary>
        /// Shuts down <see cref="NAudioAudioBackend"/>.
        /// </summary>
        public void Dispose()
        {
            _audioPlayer.Dispose();
        }
    }
}