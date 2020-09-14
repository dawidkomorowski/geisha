using System;

namespace Geisha.Engine.Audio.Backend
{
    /// <summary>
    ///     Represents ongoing sound playback and allows to control its behavior.
    /// </summary>
    /// <remarks>
    ///     When particular sound is no longer planned to be played then corresponding <see cref="IPlayback" /> object
    ///     should be disposed to release internal resources of <see cref="IAudioPlayer" />.
    /// </remarks>
    public interface IPlayback : IDisposable
    {
        /// <summary>
        ///     Indicates if <see cref="IPlayback" /> is in playing state and corresponding sound is being played.
        /// </summary>
        bool IsPlaying { get; }

        /// <summary>
        ///     Invoked when <see cref="IPlayback" /> is being stopped.
        /// </summary>
        event EventHandler Stopped;

        /// <summary>
        ///     Invoked when <see cref="IPlayback" /> is being disposed.
        /// </summary>
        event EventHandler Disposed;

        /// <summary>
        ///     Starts playback of sound from the current position.
        /// </summary>
        /// <remarks>Calling this method sets <see cref="IsPlaying" /> to <c>true</c>.</remarks>
        void Play();

        /// <summary>
        ///     Pauses playback of sound and keeps current position.
        /// </summary>
        /// <remarks>Calling this method sets <see cref="IsPlaying" /> to <c>false</c>.</remarks>
        void Pause();

        /// <summary>
        ///     Stops playback of sound and sets current position to the beginning.
        /// </summary>
        /// <remarks>Calling this method sets <see cref="IsPlaying" /> to <c>false</c>.</remarks>
        void Stop();
    }
}