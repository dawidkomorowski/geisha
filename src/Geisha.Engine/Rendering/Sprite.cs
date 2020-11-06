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
        ///     Creates new instance of <see cref="Sprite" /> based on specified <see cref="ITexture" />.
        /// </summary>
        /// <param name="sourceTexture">Texture to be used as source image data for sprite.</param>
        public Sprite(ITexture sourceTexture)
        {
            SourceTexture = sourceTexture;
        }

        /// <summary>
        ///     Texture that is source of sprite's raw image data.
        /// </summary>
        public ITexture SourceTexture { get; }

        /// <summary>
        ///     Coordinates in source texture space that are origin (left upper corner) of sprite's rectangular part of texture.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public Vector2 SourceUV { get; set; }

        /// <summary>
        ///     Dimension in source texture space that is width and height of sprite's rectangular part of texture.
        /// </summary>
        public Vector2 SourceDimension { get; set; }

        /// <summary>
        ///     Coordinates of point in sprite space that is used as an origin of sprite during rendering. In example anchor equal
        ///     half of <see cref="SourceDimension" /> makes rendering origin aligned with sprite's geometrical center.
        /// </summary>
        public Vector2 SourceAnchor { set; get; }

        /// <summary>
        ///     Conversion factor specifying how many pixels of source texture make a single unit.
        /// </summary>
        public double PixelsPerUnit { get; set; }

        /// <summary>
        ///     Rectangle based on <see cref="SourceDimension" />, <see cref="SourceAnchor" /> and <see cref="PixelsPerUnit" />
        ///     that represents sprite's raw geometry (in units) used in rendering.
        /// </summary>
        /// <remarks>
        ///     Rectangle has dimension equal to <see cref="SourceDimension" /> converted to units with factor
        ///     <see cref="PixelsPerUnit" />. It is transformed relatively to coordinate system origin as defined by
        ///     <see cref="SourceAnchor" />.
        /// </remarks>
        public Rectangle Rectangle
        {
            get
            {
                var centerFlipped = (SourceDimension / 2 - SourceAnchor) / PixelsPerUnit;
                var center = new Vector2(centerFlipped.X, -centerFlipped.Y);
                var dimension = SourceDimension / PixelsPerUnit;
                return new Rectangle(center, dimension);
            }
        }
    }
}