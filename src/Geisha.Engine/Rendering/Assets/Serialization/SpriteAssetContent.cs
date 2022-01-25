using System;
using Geisha.Common.Math.Serialization;

namespace Geisha.Engine.Rendering.Assets.Serialization
{
    /// <summary>
    ///     Defines sprite asset content to be used to load <see cref="Sprite" /> from a file into memory.
    /// </summary>
    public sealed class SpriteAssetContent
    {
        /// <summary>
        ///     Asset id of texture asset.
        /// </summary>
        public Guid TextureAssetId { get; set; }

        /// <summary>
        ///     Source UV data for <see cref="Sprite" />.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public SerializableVector2 SourceUV { get; set; }

        /// <summary>
        ///     Source dimensions data for <see cref="Sprite" />.
        /// </summary>
        public SerializableVector2 SourceDimensions { get; set; }

        /// <summary>
        ///     Source anchor data for <see cref="Sprite" />.
        /// </summary>
        public SerializableVector2 SourceAnchor { get; set; }

        /// <summary>
        ///     Pixels per unit data for <see cref="Sprite" />.
        /// </summary>
        public double PixelsPerUnit { get; set; }
    }
}