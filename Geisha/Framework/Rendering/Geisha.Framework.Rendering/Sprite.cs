using Geisha.Common.Math;

namespace Geisha.Framework.Rendering
{
    // TODO Add docs clarifying that in sprite origin is upper left corner and axes are x-right, y-down.
    // TODO Add docs clarifying what is texture space and what is sprite space.
    /// <summary>
    ///     Represents 2D rectangular image that is basic element of 2D game graphics.
    /// </summary>
    /// <remarks>
    ///     Sprite is 2D image based on texture. Often single texture is used for many sprites, each of them using only
    ///     part of texture.
    /// </remarks>
    public class Sprite
    {
        /// <summary>
        ///     Texture that is source of sprite's raw data.
        /// </summary>
        public ITexture SourceTexture { get; set; }

        /// <summary>
        ///     Coordinates in source texture space that are origin (left upper corner) of sprite's rectangular part of texture.
        /// </summary>
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
        ///     Conversion factor specifying how many pixels make a single unit.
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