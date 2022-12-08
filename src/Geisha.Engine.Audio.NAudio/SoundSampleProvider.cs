using System;
using NAudio.Wave;

namespace Geisha.Engine.Audio.NAudio
{
    internal sealed class SoundSampleProvider : ISampleProvider
    {
        private readonly Sound _sound;

        public SoundSampleProvider(Sound sound)
        {
            _sound = sound;
        }

        public int Position { get; set; }

        public int Read(float[] buffer, int offset, int count)
        {
            var read = Math.Min(count, _sound.Samples.Length - Position);
            Array.Copy(_sound.Samples, Position, buffer, offset, read);
            Position += read;
            return read;
        }

        public WaveFormat WaveFormat => SupportedWaveFormat.IeeeFloat44100Channels2;
    }
}