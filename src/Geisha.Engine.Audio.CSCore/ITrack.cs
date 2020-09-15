using System;

namespace Geisha.Engine.Audio.CSCore
{
    internal interface ITrack
    {
        bool IsPlaying { get; }

        event EventHandler Stopped;
        event EventHandler Disposed;

        void Play();
        void Pause();
        void Stop();
    }
}