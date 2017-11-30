// Program.cs
using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using CSCore.Streams;

namespace AudioProblem
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var soundOut = new WasapiOut();
            var soundMixer = new SoundMixer();

            var sound = LoadSound("Heroic Demise (New).mp3");

            soundOut.Initialize(soundMixer.ToWaveSource());
            soundOut.Play();

            soundMixer.AddSound(sound);

            // Use the same sample source to have the same sound in play after 5 seconds. 
            // So two sounds are playing at the same time but are phase shifted by 5 seconds.
            //Thread.Sleep(TimeSpan.FromSeconds(5));
            //soundMixer.AddSound(sound);
        }

        private static ISampleSource LoadSound(string filePath)
        {
            var waveFileReader = CodecFactory.Instance.GetCodec(filePath);
            return new CachedSoundSource(waveFileReader).ToSampleSource();
        }
    }
}