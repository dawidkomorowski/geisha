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
        ///     Gets or sets whether to play in a loop.
        /// </summary>
        /// <remarks>
        ///     If <c>true</c> the playback is played infinitely in a loop, otherwise the
        ///     playback is stopped when it reaches the end. Default value is <c>false</c>.
        /// </remarks>
        bool PlayInLoop { get; set; }

        /// <summary>
        ///     Gets or sets volume of playback. Valid range is from <c>0.0</c> meaning no audio, to <c>1.0</c> meaning maximum
        ///     audio volume.
        /// </summary>
        double Volume { get; set; }

        // TODO Should there be a way to schedule event handler on main thread?
        /// <summary>
        ///     Invoked when <see cref="IPlayback" /> was stopped.
        /// </summary>
        /// <remarks>This event may be invoked from another thread (audio thread).</remarks>
        event EventHandler Stopped;

        /// <summary>
        ///     Invoked when <see cref="IPlayback" /> was disposed.
        /// </summary>
        /// <remarks>This event may be invoked from another thread (audio thread).</remarks>
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