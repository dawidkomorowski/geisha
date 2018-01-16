using System;
using System.Collections.Generic;
using CSCore;

namespace AudioProblem
{
    internal class SoundMixer : ISampleSource
    {
        private readonly List<SoundSource> _soundSources = new List<SoundSource>();
        private readonly object _soundSourcesLock = new object();
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
            var numberOfSamplesStoredInBuffer = 0;

            Array.Clear(buffer, offset, count);

            lock (_soundSourcesLock)
            {
                ThrowIfDisposed();

                if (count > 0 && _soundSources.Count > 0)
                {
                    _internalBuffer = _internalBuffer.CheckBuffer(count);

                    for (var i = _soundSources.Count - 1; i >= 0; i--)
                    {
                        var soundSource = _soundSources[i];
                        soundSource.Read(_internalBuffer, count);

                        for (int j = offset, k = 0; k < soundSource.SamplesRead; j++, k++)
                        {
                            buffer[j] += _internalBuffer[k];
                        }

                        if (soundSource.SamplesRead > numberOfSamplesStoredInBuffer)
                            numberOfSamplesStoredInBuffer = soundSource.SamplesRead;

                        if (soundSource.SamplesRead == 0)
                        {
                            _soundSources.Remove(soundSource);
                            soundSource.Dispose();
                        }
                    }

                    // TODO Normalize??
                }
            }

            return count;
        }

        public void Dispose()
        {
            lock (_soundSourcesLock)
            {
                _disposed = true;

                foreach (var soundSource in _soundSources)
                {
                    soundSource.Dispose();
                }

                _soundSources.Clear();
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

        public void AddSound(ISampleSource sound)
        {
            lock (_soundSourcesLock)
            {
                ThrowIfDisposed();
                // TODO Check wave format compatibility?
                _soundSources.Add(new SoundSource(sound));
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(SoundMixer));
        }

        private class SoundSource : IDisposable
        {
            private readonly ISampleSource _sound;

            public SoundSource(ISampleSource sound)
            {
                _sound = sound;
            }

            public int SamplesRead { get; private set; }

            public void Dispose()
            {
                _sound.Dispose();
            }

            public void Read(float[] buffer, int count)
            {
                SamplesRead = _sound.Read(buffer, 0, count);
            }
        }
    }
}