using System;
using System.Collections.Generic;
using System.Diagnostics;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Geisha.Engine.Audio.NAudio
{
    internal sealed class Mixer : ISampleProvider, IDisposable
    {
        private readonly List<Track> _tracks = new();
        private readonly object _tracksLock = new();
        private bool _disposed;
        private float[] _internalBuffer = Array.Empty<float>();

        public bool EnableSound { get; set; } = true;

        #region Implementation of ISampleProvider

        public WaveFormat WaveFormat => SupportedWaveFormat.IeeeFloat44100Channels2;

        public int Read(float[] buffer, int offset, int count)
        {
            Array.Clear(buffer, offset, count);

            lock (_tracksLock)
            {
                ThrowIfDisposed();

                if (count > 0 && _tracks.Count > 0)
                {
                    _internalBuffer = EnsureCapacity(count);

                    // This loop is backwards because track.Read() may result in removal of this track from _tracks.
                    for (var trackIndex = _tracks.Count - 1; trackIndex >= 0; trackIndex--)
                    {
                        var track = _tracks[trackIndex];
                        var read = track.Read(_internalBuffer, 0, count);

                        if (EnableSound)
                        {
                            for (var i = 0; i < read; i++)
                            {
                                buffer[offset + i] += _internalBuffer[i];
                            }
                        }
                    }

                    // TODO Normalize??
                }
            }

            return count;
        }

        #endregion

        public ITrack AddTrack(SoundSampleProvider soundSampleProvider)
        {
            lock (_tracksLock)
            {
                ThrowIfDisposed();
                Debug.Assert(soundSampleProvider.WaveFormat.Equals(WaveFormat), "soundSampleProvider.WaveFormat.Equals(WaveFormat)");
                var track = new Track(soundSampleProvider);
                _tracks.Add(track);
                return track;
            }
        }

        public void RemoveTrack(ITrack track)
        {
            lock (_tracksLock)
            {
                ThrowIfDisposed();
                var internalTrack = (Track)track;

                var removed = _tracks.Remove(internalTrack);
                Debug.Assert(removed, "removed");

                internalTrack.Dispose();
            }
        }

        public void Dispose()
        {
            lock (_tracksLock)
            {
                if (_disposed) return;

                foreach (var track in _tracks)
                {
                    track.Dispose();
                }

                _tracks.Clear();

                _disposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(Mixer));
        }

        private float[] EnsureCapacity(int count)
        {
            return _internalBuffer.Length >= count ? _internalBuffer : new float[count];
        }

        private sealed class Track : ITrack, IDisposable
        {
            private readonly SoundSampleProvider _soundSampleProvider;
            private readonly VolumeSampleProvider _volumeSampleProvider;
            private readonly object _lock = new();
            private bool _disposed;

            public Track(SoundSampleProvider soundSampleProvider)
            {
                _soundSampleProvider = soundSampleProvider;
                _volumeSampleProvider = new VolumeSampleProvider(_soundSampleProvider);
            }

            public int Read(float[] buffer, int offset, int count)
            {
                lock (_lock)
                {
                    ThrowIfDisposed();

                    if (IsPlaying)
                    {
                        var readTotal = _volumeSampleProvider.Read(buffer, offset, count);

                        if (PlayInLoop)
                        {
                            while (readTotal != count)
                            {
                                var read = _volumeSampleProvider.Read(buffer, offset + readTotal, count - readTotal);
                                readTotal += read;
                                if (read == 0)
                                {
                                    _soundSampleProvider.Position = 0;
                                }
                            }
                        }
                        else
                        {
                            if (readTotal == 0)
                            {
                                Stop();
                            }
                        }

                        return readTotal;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            #region Implementation of ITrack

            public bool IsPlaying { get; private set; }
            public bool PlayInLoop { get; set; }

            public double Volume
            {
                get => _volumeSampleProvider.Volume;
                set => _volumeSampleProvider.Volume = (float)Math.Clamp(value, 0d, 1d);
            }

            public event EventHandler? Stopped;
            public event EventHandler? Disposed;

            public void Play()
            {
                lock (_lock)
                {
                    ThrowIfDisposed();

                    IsPlaying = true;
                }
            }

            public void Pause()
            {
                lock (_lock)
                {
                    ThrowIfDisposed();

                    IsPlaying = false;
                }
            }

            public void Stop()
            {
                lock (_lock)
                {
                    ThrowIfDisposed();

                    IsPlaying = false;
                    _soundSampleProvider.Position = 0;

                    Stopped?.Invoke(this, EventArgs.Empty);
                }
            }

            #endregion

            public void Dispose()
            {
                lock (_lock)
                {
                    _disposed = true;

                    Disposed?.Invoke(this, EventArgs.Empty);
                }
            }

            private void ThrowIfDisposed()
            {
                if (_disposed) throw new ObjectDisposedException(nameof(Track));
            }
        }
    }
}