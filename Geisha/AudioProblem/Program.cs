using System;
using System.IO;
using CSCore;
using CSCore.Codecs.WAV;
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
            var filePath = string.IsNullOrEmpty(readLine) ? "Heroic Demise (New).wav" : readLine;

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
                    soundMixer.AddSound(new WaveFileReader(sound.MakeShared()).ToSampleSource());

                if (input.Key == ConsoleKey.Escape)
                    playAnother = false;
            }

            soundOut.Stop();
        }

        private static SharedMemoryStream LoadSound(string filePath)
        {
            return new SharedMemoryStream(File.ReadAllBytes(filePath));
        }
    }
}