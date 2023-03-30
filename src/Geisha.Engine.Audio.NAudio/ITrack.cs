using System;

namespace Geisha.Engine.Audio.NAudio
{
    internal interface ITrack
    {
        bool IsPlaying { get; }
        bool PlayInLoop { get; set; }
        double Volume { get; set; }

        event EventHandler Stopped;
        event EventHandler Disposed;

        void Play();
        void Pause();
        void Stop();
    }
}