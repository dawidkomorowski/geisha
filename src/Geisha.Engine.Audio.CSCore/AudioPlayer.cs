using System;
using CSCore;
using CSCore.Codecs.MP3;
using CSCore.Codecs.WAV;
using CSCore.SoundOut;
using Geisha.Common.Logging;

namespace Geisha.Engine.Audio.CSCore
{
    internal sealed class AudioPlayer : IAudioPlayer
    {
        private static readonly ILog Log = LogFactory.Create(typeof(AudioPlayer));
        private readonly SoundMixer _soundMixer;
        private readonly ISoundOut _soundOut;
        private bool _disposed;

        public AudioPlayer()
        {
            _soundOut = new WaveOut();
            _soundMixer = new SoundMixer();

            _soundOut.Initialize(_soundMixer.ToWaveSource());
            _soundOut.Play();
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

            IWaveSource waveSource = soundImpl.Format switch
            {
                SoundFormat.Wav => new WaveFileReader(soundImpl.SoundStream.MakeShared()),
                SoundFormat.Mp3 => new DmoMp3Decoder(soundImpl.SoundStream.MakeShared()),
                _ => throw new ArgumentOutOfRangeException($"Unsupported sound format: {soundImpl.Format}.")
            };

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
            if (_disposed) throw new ObjectDisposedException(nameof(AudioPlayer));
        }
    }
}