using System;
using System.Collections.Generic;
using CSCore;

namespace Geisha.Engine.Audio.CSCore
{
    /// <inheritdoc />
    /// <summary>
    ///     Audio stream that allows mixing of multiple streams.
    /// </summary>
    /// <remarks>
    ///     <see cref="SoundMixer" /> allows to dynamically add new audio streams as an input to mixing. At any point in
    ///     time it is an audio stream that is a mix of all added but not already completed input audio streams. If an input
    ///     audio stream is read to end it is removed from mixing and disposed. <see cref="SoundMixer" /> class is thread safe.
    /// </remarks>
    public sealed class SoundMixer : ISampleSource
    {
        private readonly List<ISampleSource> _sampleSources = new List<ISampleSource>();
        private readonly object _sampleSourcesLock = new object();
        private bool _disposed;
        private float[]? _internalBuffer;

        /// <summary>
        ///     Initializes new instance of the <see cref="SoundMixer" /> class.
        /// </summary>
        public SoundMixer()
        {
            // TODO Accept as parameters?
            const int sampleRate = 44100;
            const int bits = 32;
            const int channels = 2;
            const AudioEncoding audioEncoding = AudioEncoding.IeeeFloat;

            WaveFormat = new WaveFormat(sampleRate, bits, channels, audioEncoding);
        }

        #region Implementation of ISampleSource

        /// <inheritdoc />
        public bool CanSeek => false;

        /// <inheritdoc />
        public WaveFormat WaveFormat { get; }

        /// <summary>
        ///     <see cref="T:Geisha.Engine.Audio.CSCore.SoundMixer" /> does not support position. This property returns 0 or
        ///     throws <see cref="T:System.NotSupportedException" /> when set.
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        public long Position
        {
            get
            {
                ThrowIfDisposed();
                return 0;
            }
            set => throw new NotSupportedException($"{nameof(SoundMixer)} does not support seeking.");
        }

        /// <summary>
        ///     <see cref="T:Geisha.Engine.Audio.CSCore.SoundMixer" /> does not support length. This property returns 0.
        /// </summary>
        public long Length
        {
            get
            {
                ThrowIfDisposed();
                return 0;
            }
        }

        /// <inheritdoc />
        public int Read(float[] buffer, int offset, int count)
        {
            Array.Clear(buffer, offset, count);

            lock (_sampleSourcesLock)
            {
                ThrowIfDisposed();

                if (count > 0 && _sampleSources.Count > 0)
                {
                    _internalBuffer = _internalBuffer.CheckBuffer(count);

                    for (var i = _sampleSources.Count - 1; i >= 0; i--)
                    {
                        var sampleSource = _sampleSources[i];
                        var samplesRead = sampleSource.Read(_internalBuffer, 0, count);

                        for (int j = offset, k = 0; k < samplesRead; j++, k++)
                        {
                            buffer[j] += _internalBuffer[k];
                        }

                        if (samplesRead == 0)
                        {
                            _sampleSources.Remove(sampleSource);
                            sampleSource.Dispose();
                        }
                    }

                    // TODO Normalize??
                }
            }

            return count;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Disposes all sample sources added to sound mixer and removes references to them.
        /// </summary>
        public void Dispose()
        {
            lock (_sampleSourcesLock)
            {
                if (_disposed) return;

                foreach (var soundSource in _sampleSources)
                {
                    soundSource.Dispose();
                }

                _sampleSources.Clear();

                _disposed = true;
            }
        }

        #endregion

        /// <summary>
        ///     Adds provided audio stream as an input to mixing.
        /// </summary>
        /// <param name="sampleSource">An audio stream to be mixed in.</param>
        /// <remarks>
        ///     Provided audio stream must be of the same <see cref="T:CSCore.WaveFormat" /> as this instance of
        ///     <see cref="SoundMixer" />, otherwise an exception is thrown.
        /// </remarks>
        /// <exception cref="ArgumentException"></exception>
        public void AddSound(ISampleSource sampleSource)
        {
            lock (_sampleSourcesLock)
            {
                ThrowIfDisposed();
                ThrowIfInvalidWaveFormat(sampleSource);
                _sampleSources.Add(sampleSource);
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(SoundMixer));
        }

        private void ThrowIfInvalidWaveFormat(ISampleSource sampleSource)
        {
            var soundWaveFormat = sampleSource.WaveFormat;

            if (soundWaveFormat.Channels != WaveFormat.Channels)
            {
                throw new ArgumentException($"Invalid sound format. Expected channels {WaveFormat.Channels} but received {soundWaveFormat.Channels}",
                    $"{nameof(sampleSource)}");
            }

            if (soundWaveFormat.SampleRate != WaveFormat.SampleRate)
            {
                throw new ArgumentException($"Invalid sound format. Expected sample rate {WaveFormat.SampleRate} but received {soundWaveFormat.SampleRate}",
                    $"{nameof(sampleSource)}");
            }

            if (soundWaveFormat.WaveFormatTag != WaveFormat.WaveFormatTag)
            {
                throw new ArgumentException(
                    $"Invalid sound format. Expected wave format {WaveFormat.WaveFormatTag} but received {soundWaveFormat.WaveFormatTag}",
                    $"{nameof(sampleSource)}");
            }
        }
    }
}