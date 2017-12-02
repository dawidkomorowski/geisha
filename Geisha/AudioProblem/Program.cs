// Program.cs

using System;
using System.IO;
using CSCore;
using CSCore.Codecs;
using CSCore.Codecs.MP3;
using CSCore.Codecs.WAV;
using CSCore.SoundOut;
using CSCore.Streams;

namespace AudioProblem
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var soundOut = new WasapiOut();

            Console.WriteLine("Use stream position? (y/n)");
            var input = Console.ReadKey();
            var useStreamPosition = input.KeyChar == 'y';
            var soundMixer = new SoundMixer(useStreamPosition);

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
                input = Console.ReadKey();

                if (input.Key == ConsoleKey.Enter)
                    soundMixer.AddSound(new DmoMp3Decoder(new SharedMemoryStream(sound)).ToSampleSource());

                if (input.Key == ConsoleKey.Escape)
                    playAnother = false;
            }

            soundOut.Stop();

            // Use the same sample source to have the same sound in play after 5 seconds. 
            // So two sounds are playing at the same time but are phase shifted by 5 seconds.
            //Thread.Sleep(TimeSpan.FromSeconds(5));
            //soundMixer.AddSound(sound);
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