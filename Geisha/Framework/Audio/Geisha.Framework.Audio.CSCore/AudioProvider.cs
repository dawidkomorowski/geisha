using System.ComponentModel.Composition;
using System.IO;
using CSCore;
using CSCore.Codecs.WAV;
using CSCore.SoundOut;
using CSCore.Streams;
using Geisha.Common;

namespace Geisha.Framework.Audio.CSCore
{
    [Export(typeof(IAudioProvider))]
    internal class AudioProvider : IAudioProvider
    {
        private readonly ISoundOut _soundOut;
        private readonly SoundMixer _soundMixer;

        public AudioProvider()
        {
            if (!WasapiOut.IsSupportedOnCurrentPlatform)
                throw new GeishaException("WASAPI is not supported on current platform.");

            _soundOut = new WasapiOut();
            _soundMixer = new SoundMixer();

            _soundOut.Initialize(_soundMixer.ToWaveSource());
            _soundOut.Play();
        }

        public ISound CreateSound(Stream stream)
        {
            return new Sound(new CachedSoundSource(new WaveFileReader(stream)).ToSampleSource());
        }

        public void Play(ISound sound)
        {
            var soundImpl = (Sound) sound;
            _soundMixer.AddSound(soundImpl);
        }
    }
}