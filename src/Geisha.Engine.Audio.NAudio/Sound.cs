namespace Geisha.Engine.Audio.NAudio
{
    internal sealed class Sound : ISound
    {
        public Sound(float[] samples, SoundFormat format)
        {
            Samples = samples;
            Format = format;
        }

        public float[] Samples { get; }
        public SoundFormat Format { get; }

        public void Dispose()
        {
        }
    }
}