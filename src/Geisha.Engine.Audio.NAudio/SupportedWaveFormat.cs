using NAudio.Wave;

namespace Geisha.Engine.Audio.NAudio
{
    internal static class SupportedWaveFormat
    {
        public static WaveFormat IeeeFloat44100Channels2 { get; } = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
    }
}