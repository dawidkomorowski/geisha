using CSCore;

namespace Geisha.Framework.Audio.CSCore
{
    internal class Sound : ISound
    {
        public Sound(IWaveSource waveSource)
        {
            WaveSource = waveSource;
        }

        public IWaveSource WaveSource { get; }
    }
}