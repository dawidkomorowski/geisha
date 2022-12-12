using System;
using Geisha.Engine.Audio.Backend;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace Geisha.Engine.Audio.NAudio
{
    internal sealed class AudioPlayer : IAudioPlayer, IDisposable
    {
        private readonly WasapiOut _wasapiOut;
        private readonly Mixer _mixer;
        private bool _disposed;

        public AudioPlayer()
        {
            _wasapiOut = new WasapiOut(AudioClientShareMode.Shared, 50);
            _mixer = new Mixer();

            _wasapiOut.Init(_mixer, true);
            _wasapiOut.Play();
        }

        public bool EnableSound
        {
            get => _mixer.EnableSound;
            set => _mixer.EnableSound = value;
        }

        public IPlayback Play(ISound sound, bool playInLoop = false)
        {
            var playback = PlayInternal(sound);
            playback.PlayInLoop = playInLoop;
            playback.Play();
            return playback;
        }

        public void PlayOnce(ISound sound)
        {
            var playback = PlayInternal(sound);
            playback.Stopped += (_, _) => playback.Dispose();
            playback.Play();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _wasapiOut.Dispose();
            _mixer.Dispose();

            _disposed = true;
        }

        private IPlayback PlayInternal(ISound sound)
        {
            ThrowIfDisposed();

            var soundSampleProvider = new SoundSampleProvider((Sound)sound);
            var track = _mixer.AddTrack(soundSampleProvider);

            return new Playback(_mixer, track);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AudioPlayer));
        }
    }
}