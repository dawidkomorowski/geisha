// SoundMixer.cs
using System;
using System.Collections.Generic;
using CSCore;

namespace AudioProblem
{
    internal class SoundMixer : ISampleSource
    {
        private readonly object _lock = new object();
        private readonly List<SoundSource> _soundSources = new List<SoundSource>();
        private float[] _internalBuffer;

        public SoundMixer()
        {
            var sampleRate = 44100;
            var bits = 32;
            var channels = 2;
            var audioEncoding = AudioEncoding.IeeeFloat;

            WaveFormat = new WaveFormat(sampleRate, bits, channels, audioEncoding);
        }

        public int Read(float[] buffer, int offset, int count)
        {
            var numberOfSamplesStoredInBuffer = 0;

            if (count > 0 && _soundSources.Count > 0)
                lock (_lock)
                {
                    Array.Clear(buffer, offset, count);

                    _internalBuffer = _internalBuffer.CheckBuffer(count);

                    for (var i = _soundSources.Count - 1; i >= 0; i--)
                    {
                        var soundSource = _soundSources[i];

                        // Here is the magic. Look at Read implementation.
                        soundSource.Read(_internalBuffer, count);

                        for (int j = offset, k = 0; k < soundSource.SamplesRead; j++, k++)
                        {
                            buffer[j] += _internalBuffer[k];
                        }

                        if (soundSource.SamplesRead > numberOfSamplesStoredInBuffer)
                            numberOfSamplesStoredInBuffer = soundSource.SamplesRead;

                        if (soundSource.SamplesRead == 0) _soundSources.Remove(soundSource);
                    }
                }

            return count;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool CanSeek => false;
        public WaveFormat WaveFormat { get; }

        public long Position
        {
            get => 0;
            set => throw new NotSupportedException($"{nameof(SoundMixer)} does not support setting the {nameof(Position)}.");
        }

        public long Length => 0;

        public void AddSound(ISampleSource sound)
        {
            lock (_lock)
            {
                _soundSources.Add(new SoundSource(sound));
            }
        }

        private class SoundSource
        {
            private readonly ISampleSource _sound;
            private long _position;

            public SoundSource(ISampleSource sound)
            {
                _sound = sound;
            }

            public int SamplesRead { get; private set; }

            public void Read(float[] buffer, int count)
            {
                // Set last remembered position (initially 0).
                // If this line is commented out, sound in my headphones is clear. But with this line it is crackling.
                // If this line is commented out, if two SoundSource use the same ISampleSource output is buggy,
                // but if line is present those are playing correctly but with crackling.
                _sound.Position = _position;

                // Read count of new samples.
                SamplesRead = _sound.Read(buffer, 0, count);

                // Remember position to be able to continue from where this SoundSource has finished last time.
                _position = _sound.Position;
            }
        }
    }
}