using System;

namespace Geisha.Engine.Audio.Backend
{
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