using System;
using System.IO;
using CSCore;
using CSCore.Codecs.MP3;
using CSCore.SoundOut;

namespace AudioProblem
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var soundOut = new WasapiOut();
            var soundMixer = new SoundMixer();

            Console.WriteLine("Enter sound file path:");

            var readLine = Console.ReadLine();
            var filePath = string.IsNullOrEmpty(readLine) ? "Heroic Demise (New).mp3" : readLine;

            Console.WriteLine("Loading file...");
            var sound = LoadSound(filePath);
            Console.WriteLine("File loaded");

            soundOut.Initialize(soundMixer.ToWaveSource());
            soundOut.Play();

            var playAnother = true;
            while (playAnother)
            {
                Console.WriteLine("Enter - start playing sound\nEscape - stop application");
                var input = Console.ReadKey();

                if (input.Key == ConsoleKey.Enter)
                    soundMixer.AddSound(new DmoMp3Decoder(new SharedMemoryStream(sound)).ToSampleSource());

                if (input.Key == ConsoleKey.Escape)
                    playAnother = false;
            }

            soundOut.Stop();
        }

        private static MemoryStream LoadSound(string filePath)
        {
            //var waveSource = CodecFactory.Instance.GetCodec(filePath);

            var memoryStream = new MemoryStream(File.ReadAllBytes(filePath));
            //waveSource.WriteToStream(memoryStream);
            //return new CachedSoundSource(waveFileReader).ToSampleSource();
            return memoryStream;
        }
    }
}