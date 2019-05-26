using System;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Assets
{
    /// <summary>
    ///     Represents texture file content to be used to load <see cref="ITexture" /> from a file into memory.
    /// </summary>
    public sealed class TextureFileContent
    {
        /// <summary>
        ///     Asset id.
        /// </summary>
        public Guid AssetId { get; set; }

        /// <summary>
        ///     Path to texture file.
        /// </summary>
        public string TextureFilePath { get; set; }
    }
}