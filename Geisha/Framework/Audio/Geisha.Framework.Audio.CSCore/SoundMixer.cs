using System;
using System.Collections.Generic;
using CSCore;

namespace Geisha.Framework.Audio.CSCore
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

        public void AddSound(Sound sound)
        {
            CheckSoundWaveFormat(sound);

            lock (_lock)
            {
                _soundSources.Add(new SoundSource(sound));
            }
        }

        private void CheckSoundWaveFormat(Sound sound)
        {
            var soundWaveFormat = sound.SampleSource.WaveFormat;

            if (soundWaveFormat.Channels != WaveFormat.Channels)
                throw new ArgumentException($"Invalid sound format. Expected channels {WaveFormat.Channels} but requested {soundWaveFormat.Channels}",
                    $"{nameof(sound)}");

            if (soundWaveFormat.SampleRate != WaveFormat.SampleRate)
                throw new ArgumentException($"Invalid sound format. Expected sample rate {WaveFormat.SampleRate} but requested {soundWaveFormat.SampleRate}",
                    $"{nameof(sound)}");
        }

        private class SoundSource
        {
            public SoundSource(Sound sound)
            {
                Sound = sound;
            }

            public Sound Sound { get; }
            public long Position { get; private set; }
            public int SamplesRead { get; private set; }

            public void Read(float[] buffer, int count)
            {
                if (Sound.SampleSource.Position != Position) ; Sound.SampleSource.Position = Position;

                SamplesRead = Sound.SampleSource.Read(buffer, 0, count);
                Position = Sound.SampleSource.Position;
            }
        }
    }
}