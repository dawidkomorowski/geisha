using System;
using System.ComponentModel.Composition;
using System.IO;
using CSCore;
using CSCore.Codecs.MP3;
using CSCore.Codecs.WAV;
using CSCore.SoundOut;
using Geisha.Common;

namespace Geisha.Framework.Audio.CSCore
{
    [Export(typeof(IAudioProvider))]
    internal class AudioProvider : IAudioProvider
    {
        private readonly SoundMixer _soundMixer;
        private readonly ISoundOut _soundOut;

        public AudioProvider()
        {
            if (!WasapiOut.IsSupportedOnCurrentPlatform)
                throw new GeishaException("WASAPI is not supported on current platform.");

            _soundOut = new WasapiOut();
            _soundMixer = new SoundMixer();

            _soundOut.Initialize(_soundMixer.ToWaveSource());
            _soundOut.Play();
        }

        public ISound CreateSound(Stream stream, SoundFormat soundFormat)
        {
            return new Sound(new SharedMemoryStream(stream), soundFormat);
        }

        public void Play(ISound sound)
        {
            var sampleSource = GetSampleSourceForSound(sound);
            _soundMixer.AddSound(sampleSource);
        }

        private static ISampleSource GetSampleSourceForSound(ISound sound)
        {
            var soundImpl = (Sound) sound;
            IWaveSource waveSource;

            switch (soundImpl.Format)
            {
                case SoundFormat.Wave:
                    waveSource = new WaveFileReader(soundImpl.SoundStream.MakeShared());
                    break;
                case SoundFormat.Mp3:
                    waveSource = new DmoMp3Decoder(soundImpl.SoundStream.MakeShared());
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unsupported sound format: {soundImpl.Format}.");
            }

            return waveSource.ToSampleSource();
        }
    }
}