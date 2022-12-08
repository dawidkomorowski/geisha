using System;
using Geisha.Engine.Audio.Backend;

namespace Geisha.Engine.Audio.NAudio
{
    internal sealed class Playback : IPlayback
    {
        private readonly Mixer _mixer;
        private readonly ITrack _track;

        public Playback(Mixer mixer, ITrack track)
        {
            _mixer = mixer;
            _track = track;

            _track.Stopped += TrackOnStopped;
            _track.Disposed += TrackOnDisposed;
        }

        #region Implementation of IPlayback

        public bool IsPlaying => _track.IsPlaying;

        public event EventHandler? Stopped;

        public event EventHandler? Disposed;

        public void Play()
        {
            _track.Play();
        }

        public void Pause()
        {
            _track.Pause();
        }

        public void Stop()
        {
            _track.Stop();
        }

        public void Dispose()
        {
            _mixer.RemoveTrack(_track);
        }

        #endregion

        private void TrackOnStopped(object? sender, EventArgs e)
        {
            Stopped?.Invoke(this, EventArgs.Empty);
        }

        private void TrackOnDisposed(object? sender, EventArgs e)
        {
            Disposed?.Invoke(this, EventArgs.Empty);
        }
    }
}