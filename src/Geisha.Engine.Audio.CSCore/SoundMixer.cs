﻿using System;
using System.Collections.Generic;
using CSCore;

namespace Geisha.Engine.Audio.CSCore
{
    // TODO Rename to Mixer?
    internal sealed class SoundMixer : ISampleSource
    {
        private readonly List<Track> _tracks = new List<Track>();
        private readonly object _tracksLock = new object();
        private bool _disposed;
        private float[]? _internalBuffer;

        public SoundMixer()
        {
            // TODO Accept as parameters?
            const int sampleRate = 44100;
            const int bits = 32;
            const int channels = 2;
            const AudioEncoding audioEncoding = AudioEncoding.IeeeFloat;

            WaveFormat = new WaveFormat(sampleRate, bits, channels, audioEncoding);
        }

        #region Implementation of ISampleSource

        public bool CanSeek => false;

        public WaveFormat WaveFormat { get; }

        public long Position
        {
            get
            {
                ThrowIfDisposed();
                return 0;
            }
            set => throw new NotSupportedException($"{nameof(SoundMixer)} does not support seeking.");
        }

        public long Length
        {
            get
            {
                ThrowIfDisposed();
                return 0;
            }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            Array.Clear(buffer, offset, count);

            lock (_tracksLock)
            {
                ThrowIfDisposed();

                if (count > 0 && _tracks.Count > 0)
                {
                    _internalBuffer = _internalBuffer.CheckBuffer(count);

                    for (var i = _tracks.Count - 1; i >= 0; i--)
                    {
                        var track = _tracks[i];
                        var samplesRead = track.Read(_internalBuffer, 0, count);

                        for (int j = offset, k = 0; k < samplesRead; j++, k++)
                        {
                            buffer[j] += _internalBuffer[k];
                        }

                        if (samplesRead == 0)
                        {
                            _tracks[i].Dispose();
                            _tracks.RemoveAt(i);
                        }
                    }

                    // TODO Normalize??
                }
            }

            return count;
        }

        public void Dispose()
        {
            lock (_tracksLock)
            {
                if (_disposed) return;

                foreach (var playback in _tracks)
                {
                    playback.Dispose();
                }

                _tracks.Clear();

                _disposed = true;
            }
        }

        #endregion


        // TODO Rename to AddTrack (rename tests)
        public ITrack AddSound(ISampleSource sampleSource)
        {
            lock (_tracksLock)
            {
                ThrowIfDisposed();
                ThrowIfInvalidWaveFormat(sampleSource);
                var track = new Track(sampleSource);
                _tracks.Add(track);
                return track;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(SoundMixer));
        }

        private void ThrowIfInvalidWaveFormat(ISampleSource sampleSource)
        {
            var soundWaveFormat = sampleSource.WaveFormat;

            if (soundWaveFormat.Channels != WaveFormat.Channels)
            {
                throw new ArgumentException($"Invalid sound format. Expected channels {WaveFormat.Channels} but received {soundWaveFormat.Channels}",
                    $"{nameof(sampleSource)}");
            }

            if (soundWaveFormat.SampleRate != WaveFormat.SampleRate)
            {
                throw new ArgumentException($"Invalid sound format. Expected sample rate {WaveFormat.SampleRate} but received {soundWaveFormat.SampleRate}",
                    $"{nameof(sampleSource)}");
            }

            if (soundWaveFormat.WaveFormatTag != WaveFormat.WaveFormatTag)
            {
                throw new ArgumentException(
                    $"Invalid sound format. Expected wave format {WaveFormat.WaveFormatTag} but received {soundWaveFormat.WaveFormatTag}",
                    $"{nameof(sampleSource)}");
            }
        }

        private sealed class Track : ISampleSource, ITrack
        {
            private readonly ISampleSource _sampleSource;

            public Track(ISampleSource sampleSource)
            {
                _sampleSource = sampleSource;
            }

            #region Implementation of ISampleSource

            public bool CanSeek => _sampleSource.CanSeek;
            public WaveFormat WaveFormat => _sampleSource.WaveFormat;

            public long Position
            {
                get => _sampleSource.Position;
                set => _sampleSource.Position = value;
            }

            public long Length => _sampleSource.Length;
            public int Read(float[] buffer, int offset, int count) => _sampleSource.Read(buffer, offset, count);

            public void Dispose()
            {
                _sampleSource.Dispose();
            }

            #endregion

            #region Implementation of ITrack

            public void Pause()
            {
            }

            #endregion
        }
    }
}