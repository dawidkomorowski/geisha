using System;
using System.IO;
using CSCore;
using CSCore.Codecs.MP3;
using CSCore.Codecs.WAV;
using CSCore.SoundOut;
using Geisha.Common;
using Geisha.Common.Logging;

namespace Geisha.Framework.Audio.CSCore
{
    internal class AudioProvider : IAudioProvider, IDisposable
    {
        private static readonly ILog Log = LogFactory.Create(typeof(AudioProvider));
        private readonly SoundMixer _soundMixer;
        private readonly ISoundOut _soundOut;
        private bool _disposed;

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
            ThrowIfDisposed();
            return new Sound(new SharedMemoryStream(stream), soundFormat);
        }

        public void Play(ISound sound)
        {
            Play((Sound) sound);
        }

        private void Play(Sound sound)
        {
            ThrowIfDisposed();

            var sampleSource = GetSampleSourceForSound(sound);

            // TODO [Mono to Stereo conversion] Do something about it.
            if (sampleSource.WaveFormat.Channels == 1)
            {
                Log.Warn("Runtime sound format conversion from mono to stereo.");
                sampleSource = sampleSource.ToStereo();
            }

            _soundMixer.AddSound(sampleSource);
        }

        private static ISampleSource GetSampleSourceForSound(Sound sound)
        {
            var soundImpl = sound;
            IWaveSource waveSource;

            switch (soundImpl.Format)
            {
                case SoundFormat.Wav:
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

        public void Dispose()
        {
            if (_disposed) return;

            _soundOut.Stop();
            _soundOut.WaitForStopped();

            _soundMixer.Dispose();
            _soundOut.Dispose();

            _disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AudioProvider));
        }
    }
}