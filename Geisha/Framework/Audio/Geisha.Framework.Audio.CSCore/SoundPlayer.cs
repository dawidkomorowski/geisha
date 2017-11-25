using System.ComponentModel.Composition;
using System.IO;
using CSCore.Codecs.WAV;
using CSCore.SoundOut;
using CSCore.Streams;
using Geisha.Common;

namespace Geisha.Framework.Audio.CSCore
{
    [Export(typeof(ISoundPlayer))]
    internal class SoundPlayer : ISoundPlayer
    {
        private readonly ISoundOut _soundOut;
        private readonly ISoundOut _soundOut2;

        public SoundPlayer()
        {
            if (!WasapiOut.IsSupportedOnCurrentPlatform)
                throw new GeishaException("WASAPI is not supported on current platform.");

            _soundOut = new WasapiOut();
            _soundOut2 = new WasapiOut();
        }

        public ISound CreateSound(Stream stream)
        {
            var waveSource = new CachedSoundSource(new WaveFileReader(stream));
            return new Sound(waveSource);
        }

        public void Play(ISound sound)
        {
            var soundImpl = (Sound) sound;

            if (_soundOut.PlaybackState != PlaybackState.Playing)
            {
                _soundOut.Initialize(soundImpl.WaveSource);
                _soundOut.Play();
            }
            else
            {
                _soundOut2.Initialize(soundImpl.WaveSource);
                _soundOut2.Play();
            }
        }
    }
}