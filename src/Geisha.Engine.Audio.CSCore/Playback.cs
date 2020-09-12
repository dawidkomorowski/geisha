using System;
using CSCore;
using Geisha.Engine.Audio.Backend;

namespace Geisha.Engine.Audio.CSCore
{
    internal sealed class Playback : IPlayback
    {
        public Playback(ISampleSource sampleSource)
        {
            SampleSource = sampleSource;
        }

        public ISampleSource SampleSource { get; }

        #region Implementation of IPlayback

        public event EventHandler? Played;
        public event EventHandler? Paused;
        public event EventHandler? Stopped;
        public event EventHandler? Disposed;

        public void Play()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            SampleSource.Dispose();
        }

        #endregion
    }
}