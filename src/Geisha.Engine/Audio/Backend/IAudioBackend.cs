﻿using System.IO;

namespace Geisha.Engine.Audio.Backend
{
    /// <summary>
    ///     Defines interface of audio backend used by the engine.
    /// </summary>
    /// <remarks>
    ///     Audio backend provides services for sound loading and audio playback.
    /// </remarks>
    public interface IAudioBackend
    {
        /// <summary>
        ///     Audio player suitable for current platform.
        /// </summary>
        IAudioPlayer AudioPlayer { get; }

        /// <summary>
        ///     Creates new <see cref="ISound" /> from data in given stream.
        /// </summary>
        /// <param name="stream">Stream containing data of the sound.</param>
        /// <param name="soundFormat">Format of sound data in the <paramref name="stream" />.</param>
        /// <returns><see cref="ISound" /> that consists of sound data from the <paramref name="stream" />.</returns>
        ISound CreateSound(Stream stream, SoundFormat soundFormat);
    }
}