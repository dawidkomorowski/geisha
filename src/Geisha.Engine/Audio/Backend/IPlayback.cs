using System;

namespace Geisha.Engine.Audio.Backend
{
    public interface IPlayback : IDisposable
    {
        event EventHandler Played;
        event EventHandler Paused;
        event EventHandler Stopped;
        event EventHandler Disposed;

        void Play();
        void Pause();
        void Stop();
    }
}