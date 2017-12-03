// SoundMixer.cs

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

        private readonly bool _useStreamPosition;

        public SoundMixer(bool useStreamPosition = true)
        {
            var sampleRate = 44100;
            var bits = 32;
            var channels = 2;
            var audioEncoding = AudioEncoding.IeeeFloat;

            WaveFormat = new WaveFormat(sampleRate, bits, channels, audioEncoding);

            _useStreamPosition = useStreamPosition;
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

                        // Here is the magic. Look at Read implementation.
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
                _soundSources.Add(new SoundSource(sound, _useStreamPosition));
            }
        }

        private class SoundSource
        {
            private readonly ISampleSource _sound;
            private TimeSpan _position;
            private bool _wasRead;

            private readonly bool _useStreamPosition;

            public SoundSource(ISampleSource sound, bool useStreamPosition)
            {
                _sound = sound;
                _useStreamPosition = useStreamPosition;
            }

            public int SamplesRead { get; private set; }

            public void Read(float[] buffer, int count)
            {
                if (!_wasRead)
                {
                    _wasRead = true;
                    Console.WriteLine("Sound started.");
                }
                // Set last remembered position (initially 0).
                // If this line is commented out, sound in my headphones is clear. But with this line it is crackling.
                // If this line is commented out, if two SoundSource use the same ISampleSource output is buggy,
                // but if line is present those are playing correctly but with crackling.
                if (_useStreamPosition)
                {
                    //_sound.Position = _position;
                    _sound.SetPosition(_position);
                }

                // Read count of new samples.
                SamplesRead = _sound.Read(buffer, 0, count);

                // Remember position to be able to continue from where this SoundSource has finished last time.
                //_position = _sound.Position;
                _position = _sound.GetPosition();

                _totalSamplesRead += SamplesRead;
            }

            public void Reset() => _sound.Position = 0;
        }
    }
}