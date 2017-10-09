using Geisha.Common.Math;

namespace Geisha.Framework.Rendering
{
    public class Sprite
    {
        public ITexture SourceTexture { get; set; }
        public Vector2 SourceUV { get; set; }
        public Vector2 SourceDimension { get; set; }
        public Vector2 SourceAnchor { set; get; }
        public double PixelsPerUnit { get; set; }

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