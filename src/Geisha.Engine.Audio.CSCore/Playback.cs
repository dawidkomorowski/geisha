using System;
using Geisha.Engine.Audio.Backend;

namespace Geisha.Engine.Audio.CSCore
{
    internal sealed class Playback : IPlayback
    {
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
            throw new NotImplementedException();
        }
    }
}