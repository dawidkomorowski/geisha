using Geisha.Common.Math.Definition;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Assets
{
    /// <summary>
    ///     Represents sprite file content to be used to load <see cref="Sprite" /> from a file into memory.
    /// </summary>
    public class SpriteFile
    {
        /// <summary>
        ///     File path to texture file.
        /// </summary>
        public string SourceTextureFilePath { get; set; }

        /// <summary>
        ///     Source UV data for <see cref="Sprite" />.
        /// </summary>
        public Vector2Definition SourceUV { get; set; }

        /// <summary>
        ///     Source dimension data for <see cref="Sprite" />.
        /// </summary>
        public Vector2Definition SourceDimension { get; set; }

        /// <summary>
        ///     Source anchor data for <see cref="Sprite" />.
        /// </summary>
        public Vector2Definition SourceAnchor { get; set; }

        /// <summary>
        ///     Pixels per unit data for <see cref="Sprite" />.
        /// </summary>
        public double PixelsPerUnit { get; set; }
    }
}