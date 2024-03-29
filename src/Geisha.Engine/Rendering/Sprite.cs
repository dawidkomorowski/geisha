﻿using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Rendering
{
    /// <summary>
    ///     Represents 2D rectangular image that is basic element of 2D game graphics.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Sprite is 2D image based on texture. Often single texture is used for many sprites, each of them using only
    ///         part of texture.
    ///     </para>
    ///     <para>
    ///         Texture coordinates are based on the origin in upper left corner of texture being a (0,0) point with axes going
    ///         x-right and y-down, up to dimensions of texture.
    ///         <br />
    ///         Sprite coordinates are based on the origin in upper left corner of sprite being a (0,0) point with axes going
    ///         x-right and y-down.
    ///     </para>
    /// </remarks>
    public sealed class Sprite
    {
        /// <summary>
        ///     Creates new instance of <see cref="Sprite" /> with specified properties.
        /// </summary>
        /// <param name="sourceTexture">Texture to be used as source image data for sprite.</param>
        /// <param name="sourceUV">Upper left corner of sprite's rectangular region of texture.</param>
        /// <param name="sourceDimensions">Dimensions, width and height, of sprite's rectangular region of texture.</param>
        /// <param name="pivot">Pivot point of sprite.</param>
        /// <param name="pixelsPerUnit">Conversion factor specifying how many pixels of source texture make a single unit.</param>
        // ReSharper disable once InconsistentNaming
        public Sprite(ITexture sourceTexture, Vector2 sourceUV, Vector2 sourceDimensions, Vector2 pivot, double pixelsPerUnit)
        {
            SourceTexture = sourceTexture;
            SourceUV = sourceUV;
            SourceDimensions = sourceDimensions;
            Pivot = pivot;
            PixelsPerUnit = pixelsPerUnit;
            Rectangle = ComputeRectangle();
        }

        /// <summary>
        ///     Texture that is source of sprite's raw image data.
        /// </summary>
        public ITexture SourceTexture { get; }

        /// <summary>
        ///     Upper left corner of sprite's rectangular region of texture in texture coordinates.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public Vector2 SourceUV { get; }

        /// <summary>
        ///     Dimensions, width and height, of sprite's rectangular region of texture.
        /// </summary>
        public Vector2 SourceDimensions { get; }

        /// <summary>
        ///     Pivot point defines origin for sprite transformations during the rendering. It is defined in sprite coordinates.
        /// </summary>
        /// <remarks>
        ///     For example a <see cref="Pivot" /> equal half the <see cref="SourceDimensions" /> makes rendering origin
        ///     aligned with sprite's rectangle center.
        /// </remarks>
        public Vector2 Pivot { get; }

        /// <summary>
        ///     Conversion factor specifying how many pixels of source texture make a single unit.
        /// </summary>
        public double PixelsPerUnit { get; }

        /// <summary>
        ///     Rectangle representing final sprite's geometry (in units) that is used for rendering. It is derived from
        ///     <see cref="SourceDimensions" />, <see cref="Pivot" /> and <see cref="PixelsPerUnit" />.
        /// </summary>
        /// <remarks>
        ///     Rectangle has dimensions equal to <see cref="SourceDimensions" /> converted to units with factor
        ///     <see cref="PixelsPerUnit" />. It is translated relatively to coordinate system origin by <see cref="Pivot" />, so
        ///     the pivot point is at (0,0).
        /// </remarks>
        public AxisAlignedRectangle Rectangle { get; }

        private AxisAlignedRectangle ComputeRectangle()
        {
            var centerFlipped = (SourceDimensions / 2 - Pivot) / PixelsPerUnit;
            var center = new Vector2(centerFlipped.X, -centerFlipped.Y);
            var dimensions = SourceDimensions / PixelsPerUnit;
            return new AxisAlignedRectangle(center, dimensions);
        }
    }
}