using CSCore;

namespace Geisha.Framework.Audio.CSCore
{
    internal class Sound : ISound
    {
        public Sound(ISampleSource sampleSource)
        {
            SampleSource = sampleSource;
        }

        public ISampleSource SampleSource { get; }
    }
}