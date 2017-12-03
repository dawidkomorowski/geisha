using System;
using System.Collections.Generic;
using CSCore;

namespace AudioProblem
{
    internal class SoundMixer : ISampleSource
    {
        private static long _totalSamplesRead;

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

            Array.Clear(buffer, offset, count);

            if (count > 0 && _soundSources.Count > 0)
                lock (_lock)
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
                            Console.WriteLine($"Total samples read: {_totalSamplesRead}");
                            _totalSamplesRead = 0;
                            soundSource.Reset();
                            _soundSources.Remove(soundSource);
                        }
                    }
                }

            return count;
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
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
            private bool _wasRead;

            public SoundSource(ISampleSource sound)
            {
                _sound = sound;
            }

            public int SamplesRead { get; private set; }

            public void Read(float[] buffer, int count)
            {
                if (!_wasRead)
                {
                    _wasRead = true;
                    Console.WriteLine("Sound started.");
                }

                SamplesRead = _sound.Read(buffer, 0, count);
                _totalSamplesRead += SamplesRead;
            }

            public void Reset() => _sound.Position = 0;
        }
    }
}