using System;
using Geisha.Engine.Audio.Backend;

namespace Geisha.Engine.Audio.CSCore
{
    internal sealed class Playback : IPlayback
    {
        private readonly Mixer _mixer;
        private readonly ITrack _track;

        public Playback(Mixer mixer, ITrack track)
        {
            _mixer = mixer;
            _track = track;
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
    }
}