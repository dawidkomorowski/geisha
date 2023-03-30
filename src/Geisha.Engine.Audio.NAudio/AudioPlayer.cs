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

        public double Volume
        {
            get => _waveOutEvent.Volume;
            set => _waveOutEvent.Volume = (float)Math.Clamp(value, 0d, 1d);
        }

        public IPlayback Play(ISound sound, bool playInLoop = false, double volume = 1.0)
        {
            var playback = PlayInternal(sound);
            playback.PlayInLoop = playInLoop;
            playback.Volume = volume;
            playback.Play();
            return playback;
        }

        public void PlayOnce(ISound sound, double volume = 1.0)
        {
            var playback = PlayInternal(sound);
            playback.Stopped += (_, _) => playback.Dispose();
            playback.Volume = volume;
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