using System;
using System.Collections.Generic;
using CSCore;

namespace Geisha.Framework.Audio.CSCore
{
    // TODO Add tests and docs
    internal class SoundMixer : ISampleSource
    {
        private readonly List<ISampleSource> _sampleSources = new List<ISampleSource>();
        private readonly object _sampleSourcesLock = new object();
        private bool _disposed;
        private float[] _internalBuffer;

        public SoundMixer()
        {
            // TODO Accept as parameters?
            const int sampleRate = 44100;
            const int bits = 32;
            const int channels = 2;
            const AudioEncoding audioEncoding = AudioEncoding.IeeeFloat;

            WaveFormat = new WaveFormat(sampleRate, bits, channels, audioEncoding);
        }

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

        public void Dispose()
        {
            lock (_sampleSourcesLock)
            {
                _disposed = true;

                foreach (var soundSource in _sampleSources)
                {
                    soundSource.Dispose();
                }

                _sampleSources.Clear();
            }
        }

        public bool CanSeek => !_disposed;
        public WaveFormat WaveFormat { get; }

        public long Position
        {
            get
            {
                ThrowIfDisposed();
                return 0;
            }
            set => throw new NotSupportedException($"{nameof(SoundMixer)} does not support seeking.");
        }

        public long Length
        {
            get
            {
                ThrowIfDisposed();
                return 0;
            }
        }

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
                throw new ArgumentException($"Invalid sound format. Expected channels {WaveFormat.Channels} but requested {soundWaveFormat.Channels}",
                    $"{nameof(sampleSource)}");
            }

            if (soundWaveFormat.SampleRate != WaveFormat.SampleRate)
            {
                throw new ArgumentException($"Invalid sound format. Expected sample rate {WaveFormat.SampleRate} but requested {soundWaveFormat.SampleRate}",
                    $"{nameof(sampleSource)}");
            }
        }
    }
}