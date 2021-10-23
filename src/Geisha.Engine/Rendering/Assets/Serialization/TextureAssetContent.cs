namespace Geisha.Engine.Rendering.Assets.Serialization
{
    /// <summary>
    ///     Defines texture asset content to be used to load <see cref="ITexture" /> from a file into memory.
    /// </summary>
    public sealed class TextureAssetContent
    {
        /// <summary>
        ///     Path to texture file.
        /// </summary>
        public string? TextureFilePath { get; set; }
    }
}