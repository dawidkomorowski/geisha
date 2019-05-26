using System;
using Geisha.Framework.Audio;

namespace Geisha.Engine.Audio.Assets
{
    /// <summary>
    ///     Represents sound file content to be used to load <see cref="ISound" /> from a file into memory.
    /// </summary>
    public sealed class SoundFileContent
    {
        /// <summary>
        ///     Asset id.
        /// </summary>
        public Guid AssetId { get; set; }

        /// <summary>
        ///     Path to sound file.
        /// </summary>
        public string SoundFilePath { get; set; }
    }
}