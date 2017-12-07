// Program.cs

using System;
using System.IO;
using System.Threading;
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

            var sound = LoadSound("Heroic Demise (New).wav");

            soundOut.Initialize(soundMixer.ToWaveSource());
            soundOut.Play();

            // Play first from shallow copy of shared stream
            soundMixer.AddSound(new WaveFileReader(sound.MakeShared()).ToSampleSource());

            Thread.Sleep(TimeSpan.FromSeconds(5));

            // Play second from another shallow copy of shared stream
            soundMixer.AddSound(new WaveFileReader(sound.MakeShared()).ToSampleSource());

            Thread.Sleep(TimeSpan.FromSeconds(5));

            soundOut.Stop();
        }

        private static SharedMemoryStream LoadSound(string filePath)
        {
            return new SharedMemoryStream(File.ReadAllBytes(filePath));
        }
    }
}