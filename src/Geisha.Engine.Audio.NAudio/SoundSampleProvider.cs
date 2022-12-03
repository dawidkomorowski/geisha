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

        public int Read(float[] buffer, int offset, int count) => throw new System.NotImplementedException();

        public WaveFormat WaveFormat => SupportedWaveFormat.IeeeFloat44100Channels2;
    }
}