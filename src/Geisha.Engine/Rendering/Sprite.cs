using Geisha.Common.Math;

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
    ///         Texture space coordinates are based on the origin in left upper corner of texture being a (0,0) point with axes
    ///         going x-right and y-down, up to dimension of texture.
    ///         <br />
    ///         Sprite space coordinates are based on the origin in left upper corner of sprite being a (0,0) point with axes
    ///         going x-right and y-down.
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
        /// <param name="sourceAnchor">Anchor point of sprite.</param>
        /// <param name="pixelsPerUnit">Conversion factor specifying how many pixels of source texture make a single unit.</param>
        // ReSharper disable once InconsistentNaming
        public Sprite(ITexture sourceTexture, Vector2 sourceUV, Vector2 sourceDimensions, Vector2 sourceAnchor, double pixelsPerUnit)
        {
            SourceTexture = sourceTexture;
            SourceUV = sourceUV;
            SourceDimension = sourceDimensions;
            SourceAnchor = sourceAnchor;
            PixelsPerUnit = pixelsPerUnit;
            Rectangle = ComputeRectangle();
        }

        /// <summary>
        ///     Texture that is source of sprite's raw image data.
        /// </summary>
        public ITexture SourceTexture { get; }

        /// <summary>
        ///     Coordinates in source texture space that is origin (upper left corner) of sprite's rectangular region of texture.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public Vector2 SourceUV { get; }

        // TODO Dimension or Dimensions? Typically dimensions is used to describe the size of something.
        /// <summary>
        ///     Dimensions in source texture space that is width and height of sprite's rectangular region of texture.
        /// </summary>
        public Vector2 SourceDimension { get; }

        // TODO Use name Pivot Point instead of Anchor?
        /// <summary>
        ///     Coordinates of point in sprite space that is used as an origin of sprite during rendering. In example anchor equal
        ///     half of <see cref="SourceDimension" /> makes rendering origin aligned with sprite's geometrical center.
        /// </summary>
        public Vector2 SourceAnchor { get; }

        /// <summary>
        ///     Conversion factor specifying how many pixels of source texture make a single unit.
        /// </summary>
        public double PixelsPerUnit { get; }

        /// <summary>
        ///     Rectangle based on <see cref="SourceDimension" />, <see cref="SourceAnchor" /> and <see cref="PixelsPerUnit" />
        ///     that represents sprite's raw geometry (in units) used in rendering.
        /// </summary>
        /// <remarks>
        ///     Rectangle has dimensions equal to <see cref="SourceDimension" /> converted to units with factor
        ///     <see cref="PixelsPerUnit" />. It is transformed relatively to coordinate system origin as defined by
        ///     <see cref="SourceAnchor" />.
        /// </remarks>
        public AxisAlignedRectangle Rectangle { get; }

        private AxisAlignedRectangle ComputeRectangle()
        {
            var centerFlipped = (SourceDimension / 2 - SourceAnchor) / PixelsPerUnit;
            var center = new Vector2(centerFlipped.X, -centerFlipped.Y);
            var dimension = SourceDimension / PixelsPerUnit;
            return new AxisAlignedRectangle(center, dimension);
        }
    }
}