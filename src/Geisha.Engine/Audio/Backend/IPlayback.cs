using System;

namespace Geisha.Engine.Audio.Backend
{
    // TODO Add docs
    public interface IPlayback : IDisposable
    {
        bool IsPlaying { get; }

        event EventHandler Stopped;
        event EventHandler Disposed;

        void Play();
        void Pause();
        void Stop();
    }
}