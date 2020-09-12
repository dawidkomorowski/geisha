﻿using System;
using CSCore;
using CSCore.Codecs.MP3;
using CSCore.Codecs.WAV;
using CSCore.SoundOut;
using Geisha.Common.Logging;
using Geisha.Engine.Audio.Backend;

namespace Geisha.Engine.Audio.CSCore
{
    internal sealed class AudioPlayer : IAudioPlayer
    {
        private static readonly ILog Log = LogFactory.Create(typeof(AudioPlayer));
        private readonly Mixer _mixer;
        private readonly ISoundOut _soundOut;
        private bool _disposed;

        public AudioPlayer()
        {
            _soundOut = new WaveOut();
            _mixer = new Mixer();

            _soundOut.Initialize(_mixer.ToWaveSource());
            _soundOut.Play();
        }

        public void Play(ISound sound)
        {
            Play((Sound) sound);
        }

        public IPlayback PlayNew(ISound sound) => throw new NotImplementedException();

        private void Play(Sound sound)
        {
            ThrowIfDisposed();

            var sampleSource = GetSampleSourceForSound(sound);

            // TODO [Mono to Stereo conversion] Do something about it.
            if (sampleSource.WaveFormat.Channels == 1)
            {
                Log.Warn("Runtime sound format conversion from mono to stereo.");
                sampleSource = sampleSource.ToStereo();
            }

            _mixer.AddTrack(sampleSource);
        }

        private static ISampleSource GetSampleSourceForSound(Sound sound)
        {
            var soundImpl = sound;

            IWaveSource waveSource = soundImpl.Format switch
            {
                SoundFormat.Wav => new WaveFileReader(soundImpl.SoundStream.MakeShared()),
                SoundFormat.Mp3 => new DmoMp3Decoder(soundImpl.SoundStream.MakeShared()),
                _ => throw new ArgumentOutOfRangeException($"Unsupported sound format: {soundImpl.Format}.")
            };

            return waveSource.ToSampleSource();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _soundOut.Stop();
            _soundOut.WaitForStopped();
            _soundOut.Dispose();

            _mixer.Dispose();

            _disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(AudioPlayer));
        }
    }
}