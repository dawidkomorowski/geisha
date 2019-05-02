﻿using System;
using Geisha.Common.Math.Serialization;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Assets
{
    /// <summary>
    ///     Represents sprite file content to be used to load <see cref="Sprite" /> from a file into memory.
    /// </summary>
    public class SpriteFileContent
    {
        /// <summary>
        ///     Asset id.
        /// </summary>
        public Guid AssetId { get; set; }

        /// <summary>
        ///     Asset id of texture asset.
        /// </summary>
        public Guid TextureAssetId { get; set; }

        /// <summary>
        ///     Source UV data for <see cref="Sprite" />.
        /// </summary>
        public SerializableVector2 SourceUV { get; set; }

        /// <summary>
        ///     Source dimension data for <see cref="Sprite" />.
        /// </summary>
        public SerializableVector2 SourceDimension { get; set; }

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