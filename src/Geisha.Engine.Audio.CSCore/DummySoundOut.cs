using System;
using CSCore;
using CSCore.SoundOut;

namespace Geisha.Engine.Audio.CSCore
{
    // TODO How should it be implemented? Should it start dummy thread reading wave source over time to fake sound playback?
    internal sealed class DummySoundOut : ISoundOut
    {
        public void Dispose()
        {
        }

        public void Play()
        {
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
        }

        public void Initialize(IWaveSource source)
        {
        }

        public float Volume { get; set; }
        public IWaveSource WaveSource { get; }
        public PlaybackState PlaybackState { get; }
        public event EventHandler<PlaybackStoppedEventArgs> Stopped;
    }
}