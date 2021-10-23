namespace Geisha.Engine.Audio.Assets.Serialization
{
    /// <summary>
    ///     Defines sound asset content to be used to load <see cref="ISound" /> from a file into memory.
    /// </summary>
    public sealed class SoundAssetContent
    {
        /// <summary>
        ///     Path to sound file.
        /// </summary>
        public string? SoundFilePath { get; set; }
    }
}