using System;
using Geisha.Engine.Audio.Backend;
using NAudio.Wave;

namespace Geisha.Engine.Audio.NAudio
{
    internal sealed class AudioPlayer : IAudioPlayer, IDisposable
    {
        private readonly WaveOutEvent _waveOutEvent;
        private readonly Mixer _mixer;
        private bool _disposed;

        public AudioPlayer()
        {
            _waveOutEvent = new WaveOutEvent
            {
                DesiredLatency = 50
            };
            _mixer = new Mixer();

            _waveOutEvent.Init(_mixer, true);
            _waveOutEvent.Play();
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

            _waveOutEvent.Dispose();
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