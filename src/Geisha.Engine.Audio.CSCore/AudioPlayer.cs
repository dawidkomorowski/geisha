using System;
using CSCore;
using CSCore.Codecs.MP3;
using CSCore.Codecs.WAV;
using CSCore.SoundOut;
using Geisha.Common.Logging;
using Geisha.Engine.Audio.Backend;

namespace Geisha.Engine.Audio.CSCore
{
    internal sealed class AudioPlayer : IAudioPlayer
    {
        private static readonly ILog Log = LogFactory.Create(typeof(AudioPlayer));
        private readonly Mixer _mixer;
        private readonly ISoundOut _soundOut;
        private bool _disposed;

        public AudioPlayer(bool useDummySoundOut)
        {
            _soundOut = useDummySoundOut ? (ISoundOut) new DummySoundOut() : new WasapiOut();
            _mixer = new Mixer();

            _soundOut.Initialize(_mixer.ToWaveSource());
            _soundOut.Play();
        }

        public IPlayback Play(ISound sound)
        {
            var playback = Play((Sound) sound);
            playback.Play();
            return playback;
        }

        public void PlayOnce(ISound sound)
        {
            var playback = Play((Sound) sound);
            playback.Stopped += (sender, args) => playback.Dispose();
            playback.Play();
        }

        private IPlayback Play(Sound sound)
        {
            ThrowIfDisposed();

            var sampleSource = GetSampleSourceForSound(sound);

            // TODO [Mono to Stereo conversion] Do something about it.
            if (sampleSource.WaveFormat.Channels == 1)
            {
                Log.Warn("Runtime sound format conversion from mono to stereo.");
                sampleSource = sampleSource.ToStereo();
            }

            var track = _mixer.AddTrack(sampleSource);

            return new Playback(_mixer, track);
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
            _soundOut.Dispose();

            _mixer.Dispose();

            _disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AudioPlayer));
        }
    }
}